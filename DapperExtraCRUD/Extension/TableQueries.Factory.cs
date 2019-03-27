using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using Dapper.Extension.Interfaces;

namespace Dapper.Extension
{
	public partial class TableQueries<T> where T : class
	{
		public class Factory
		{
			private const string BulkTempStagingTable = "#_DappBulkTable_";

			private static readonly HashSet<Type> validTypes = new HashSet<Type>() {
				typeof(byte),
				typeof(sbyte),
				typeof(short),
				typeof(ushort),
				typeof(int),
				typeof(uint),
				typeof(long),
				typeof(ulong),
				typeof(float),
				typeof(double),
				typeof(decimal),
				typeof(bool),
				typeof(string),
				typeof(char),
				typeof(Guid),
				typeof(DateTime),
				typeof(DateTimeOffset),
				typeof(TimeSpan),
				typeof(byte[]),
				typeof(byte?),
				typeof(sbyte?),
				typeof(short?),
				typeof(ushort?),
				typeof(int?),
				typeof(uint?),
				typeof(long?),
				typeof(ulong?),
				typeof(float?),
				typeof(double?),
				typeof(decimal?),
				typeof(bool?),
				typeof(char?),
				typeof(Guid?),
				typeof(DateTime?),
				typeof(DateTimeOffset?),
				typeof(TimeSpan?),
			};

			private static bool Predicate(PropertyInfo prop)
			{
				if (prop.CanRead && prop.CanWrite
					&& prop.GetCustomAttribute<IgnoreAttribute>(true) == null
					&& (prop.GetCustomAttribute<IgnoreSelectAttribute>(true) == null || prop.GetCustomAttribute<IgnoreInsertAttribute>(true) == null
						|| prop.GetCustomAttribute<IgnoreUpdateAttribute>(true) == null)) {
					Type propertyType = prop.PropertyType;
					if (validTypes.Contains(propertyType)) {
						return true;
					}
					if (propertyType.IsGenericType && typeof(IEnumerable<>).IsAssignableFrom(propertyType)) {
						Type underlyingType = propertyType.GetGenericArguments()[0];
						return underlyingType == typeof(byte);
					}
					propertyType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
					if (propertyType.IsEnum) {
						return true;
					}
					return propertyType.GetInterfaces().Any(ty => ty.IsGenericType && ty.GetGenericTypeDefinition() == typeof(SqlMapper.TypeHandler<>));
				}
				return false;
			}

			internal Factory()
			{
				TableAttribute = typeof(T).GetCustomAttribute<TableAttribute>(true);
				TableName = TableAttribute?.Name ?? typeof(T).Name;

				IEnumerable<PropertyInfo> properties = typeof(T).GetProperties(TableAttribute?.BindingFlags ?? (BindingFlags.Public | BindingFlags.Instance));
				Properties = properties.Where(Predicate).ToArray();


				KeyProperties = Properties.Where(prop => prop.GetCustomAttribute<KeyAttribute>(true) != null).ToArray();
				AutoKeyProperties = KeyProperties.Where(prop => !prop.GetCustomAttribute<KeyAttribute>(true).Required).ToArray();
				SelectProperties = GetProperties(Array.Empty<PropertyInfo>(), (prop) => true, typeof(IgnoreSelectAttribute), typeof(IgnoreAttribute));
				InsertProperties = GetProperties(AutoKeyProperties, (prop) => { var attr = prop.GetCustomAttribute<IgnoreInsertAttribute>(true); return attr == null || attr.HasValue; }, typeof(IgnoreAttribute));
				UpdateProperties = GetProperties(KeyProperties, (prop) => { var attr = prop.GetCustomAttribute<IgnoreUpdateAttribute>(true); return attr == null || attr.HasValue; }, typeof(IgnoreAttribute));

				if (InsertProperties.Length == 0) {
					InsertProperties = Properties;
				}
				if (UpdateProperties.Length == 0) {
					UpdateProperties = Properties;
				}

				PropertyInfo[] MatchUpdateProperties = UpdateProperties.Where(prop => prop.GetCustomAttribute<MatchUpdateAttribute>(true) != null).ToArray();
				if (MatchUpdateProperties.Length > 0) {
					UpdateProperties = UpdateProperties.Where(prop => !MatchUpdateProperties.Contains(prop) || prop.GetCustomAttribute<IgnoreUpdateAttribute>(true)?.Value != null).ToArray();
				}
				PropertyInfo[] MatchDeleteProperties = Properties.Where(prop => prop.GetCustomAttribute<KeyAttribute>(true) != null || prop.GetCustomAttribute<MatchDeleteAttribute>(true) != null).ToArray();

				if (KeyProperties.Length == Properties.Length) {
					AutoKeyProperties = KeyProperties = Array.Empty<PropertyInfo>();
				}

				Columns = GetColumnNames(Properties);
				KeyColumns = GetColumnNames(KeyProperties);
				AutoKeyColumns = GetColumnNames(AutoKeyProperties);
				SelectColumns = GetColumnNames(SelectProperties);
				UpdateColumns = GetColumnNames(UpdateProperties);
				InsertColumns = GetColumnNames(InsertProperties);

				if (KeyProperties.Length == 0 || KeyProperties.Length == Properties.Length) {
					EqualityProperties = Properties;
					EqualityColumns = Columns;
					UpdateEqualityProperties = EqualityProperties;
					DeleteEqualityProperties = EqualityProperties;
					UpdateEqualityColumns = Columns;
					DeleteEqualityColumns = Columns;
				}
				else {
					EqualityProperties = KeyProperties;
					EqualityColumns = KeyColumns;
					UpdateEqualityProperties = EqualityProperties.Union(MatchUpdateProperties).ToArray();
					DeleteEqualityProperties = EqualityProperties.Union(MatchDeleteProperties).ToArray();
					UpdateEqualityColumns = GetColumnNames(UpdateEqualityProperties);
					DeleteEqualityColumns = GetColumnNames(DeleteEqualityProperties);
				}

				InsertDefaults = new IDefaultAttribute[InsertProperties.Length];
				for (int i = 0; i < InsertProperties.Length; i++) {
					InsertDefaults[i] = InsertProperties[i].GetCustomAttribute<IgnoreInsertAttribute>(true);
					if (InsertDefaults[i] == null || !InsertDefaults[i].HasValue) {
						InsertDefaults[i] = null;
					}
				}
				UpdateDefaults = new IDefaultAttribute[UpdateProperties.Length];
				for (int i = 0; i < UpdateProperties.Length; i++) {
					UpdateDefaults[i] = (IDefaultAttribute) UpdateProperties[i].GetCustomAttribute<IgnoreUpdateAttribute>(true)
						?? UpdateProperties[i].GetCustomAttribute<MatchUpdateAttribute>(true);
					if (UpdateDefaults[i] == null || !UpdateDefaults[i].HasValue) {
						UpdateDefaults[i] = null;
					}
				}

				whereEquals = "WHERE " + GetEqualsParams(" AND ", EqualityProperties, EqualityColumns);
				whereUpdateEquals = "WHERE " + GetEqualsParams(" AND ", UpdateEqualityProperties, UpdateEqualityColumns);
				whereDeleteEquals = "WHERE " + GetEqualsParams(" AND ", DeleteEqualityProperties, DeleteEqualityColumns);
				whereDeleteExistsBulk = "WHERE EXISTS (\nSELECT * FROM [" + BulkTempStagingTable + "]\nWHERE " + GetTempAndEqualsParams(DeleteEqualityColumns) + ")";

				outputInserted = GetOutput("INSERTED", Properties) + "\n";
				outputInsertedKeys = KeyProperties.Length == 0 ? outputInserted : (GetOutput("INSERTED", KeyProperties) + "\n");
				outputDeleted = GetOutput("DELETED", Properties) + "\n";
				outputDeletedKeys = KeyProperties.Length == 0 ? outputDeleted : (GetOutput("DELETED", KeyProperties) + "\n");

				paramsInsert = "[" + string.Join("],[", InsertColumns) + "]";
				insertTableParams = "INSERT " + TableName + " (" + paramsInsert + ")\n";

				deleteQuery = "DELETE FROM " + TableName + "\n";
				deleteSingleQuery = deleteQuery + whereDeleteEquals;
				paramsSelectFrom = GetAsParams(SelectProperties) + " FROM " + TableName + "\n";
				selectSingleQuery = "SELECT " + paramsSelectFrom + whereEquals;

				List<object> _valuesInserted = new List<object>();
				_valuesInserted.Add("VALUES (");
				IEnumerable<object> valuesInsertedList = GetValues(InsertProperties, InsertDefaults);
				_valuesInserted.AddRange(valuesInsertedList);
				_valuesInserted.Add(")\n");
				valuesInserted = CombineStrings(_valuesInserted);

				List<object> _updateSetParams = new List<object>();
				_updateSetParams.Add("UPDATE " + TableName + "\nSET ");
				IEnumerable<object> updateSetParamsList = GetSetParams(UpdateColumns, UpdateProperties, UpdateDefaults);
				_updateSetParams.AddRange(updateSetParamsList);
				updateSetParams = CombineStrings(_updateSetParams);

				List<object> _bulkUpdateTableSetParams = new List<object>(GetTempSetParams(UpdateColumns, UpdateDefaults));
				bulkUpdateSetParams = CombineStrings(_bulkUpdateTableSetParams);
			}

			private static object[] CombineStrings(List<object> input)
			{
				List<object> result = new List<object>();
				StringBuilder sb = new StringBuilder();
				for (int i = 0; i < input.Count; i++) {
					if (input[i] is string str) {
						sb.Append(str);
					}
					else {
						if (sb.Length > 0) {
							result.Add(sb.ToString());
							sb.Clear();
						}
						result.Add(input[i]);
					}
				}
				result.Add(sb.ToString());
				return result.ToArray();
			}

			private static string NoKeyExceptionString { get; set; } = $"Type {typeof(T).Name} does not have a KeyProperty.";

			private static bool NoKeyProperty(IDbConnection connection, T obj, IDbTransaction transaction, int? commandTimeout)
			{
				throw new InvalidOperationException(NoKeyExceptionString);
			}

			private static int NoKeyProperty(SqlConnection connection, IEnumerable<T> objs, SqlTransaction transaction, int? commandTimeout)
			{
				throw new InvalidOperationException(NoKeyExceptionString);
			}

			private static IEnumerable<T> NoKeyProperty(SqlConnection connection, IEnumerable<T> objs, SqlTransaction transaction, bool buffered, int? commandTimeout)
			{
				throw new InvalidOperationException(NoKeyExceptionString);
			}

			private static IEnumerable<T> NoKeyProperty(IDbConnection connection, string whereCondition, object param, IDbTransaction transaction, bool buffered, int? commandTimeout)
			{
				throw new InvalidOperationException(NoKeyExceptionString);
			}

			private static void BulkInsert_(SqlConnection connection, IEnumerable<T> objs, SqlTransaction transaction, string tableName, string[] columns, PropertyInfo[] properties, int? commandTimeout = null)
			{
				var dataReader = FastMember.ObjectReader.Create<T>(objs, properties.Select(p => p.Name).ToArray());
				using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default | SqlBulkCopyOptions.KeepIdentity | SqlBulkCopyOptions.KeepNulls, transaction)) {
					bulkCopy.DestinationTableName = tableName ?? TableData<T>.TableName;
					bulkCopy.BulkCopyTimeout = commandTimeout ?? 0;
					for (int i = 0; i < columns.Length; i++) {
						bulkCopy.ColumnMappings.Add(properties[i].Name, columns[i]);
					}
					bulkCopy.WriteToServer(dataReader);
				}
			}


			/*
			public TableQueries<T>.Delegates.RemoveDuplicatesFunc RemoveDuplicatesFunc { get; protected set; }
			*/

			public TableQueries<T>.Data Create()
			{
				TableQueries<T>.Data queries = new TableQueries<T>.Data();
				queries.Columns = Columns;
				queries.KeyColumns = KeyColumns;
				queries.Properties = Properties;
				queries.KeyProperties = KeyProperties;
				queries.AutoKeyProperties = AutoKeyProperties;
				queries.EqualityProperties = EqualityProperties;

				string deleteListQuery = deleteQuery + outputDeleted;
				string bulkDeleteQuery = deleteQuery + whereDeleteExistsBulk;
				string bulkDeleteListQuery = deleteListQuery + whereDeleteExistsBulk;

				string bulkInsertNotExistsOutputQuery = insertTableParams + outputInserted + "SELECT " + paramsInsert + "\nFROM " + BulkTempStagingTable + "\nWHERE NOT EXISTS (\nSELECT * FROM " + TableName + "\nWHERE " + GetTempAndEqualsParams(EqualityColumns) + ")";
				string createStagingTableQuery = CreateTableQuery(BulkTempStagingTable, Columns);
				string bulkInsertNotExistsQuery = insertTableParams + "SELECT " + paramsInsert + "\nFROM " + BulkTempStagingTable + "\nWHERE NOT EXISTS (\nSELECT * FROM " + TableName + "\nWHERE " + GetTempAndEqualsParams(EqualityColumns) + ")";

				queries.GetListFunc = (connection, whereCondition, param, transaction, buffered, commandTimeout) =>
				{
					string query = "SELECT " + paramsSelectFrom + whereCondition;
					IEnumerable<T> list = connection.Query<T>(query, param, transaction, buffered, commandTimeout);
					return list;
				};

				queries.GetTopFunc = (connection, limit, whereCondition, param, transaction, buffered, commandTimeout) =>
				{
					string query = "SELECT TOP(" + limit + ") " + paramsSelectFrom + whereCondition;
					IEnumerable<T> list = connection.Query<T>(query, param, transaction, buffered, commandTimeout);
					return list;
				};

				queries.GetDistinctFunc = (connection, whereCondition, param, transaction, buffered, commandTimeout) =>
				{
					string query = "SELECT DISTINCT " + paramsSelectFrom + whereCondition;
					IEnumerable<T> list = connection.Query<T>(query, param, transaction, buffered, commandTimeout);
					return list;
				};

				queries.GetFunc = (connection, obj, transaction, commandTimeout) =>
				{
					IEnumerable<T> value = connection.Query<T>(selectSingleQuery, obj, transaction, true, commandTimeout);
					return value.FirstOrDefault();
				};

				queries.GetDictFunc = (connection, obj, transaction, commandTimeout) =>
				{
					IEnumerable<T> value = connection.Query<T>(selectSingleQuery, obj, transaction, true, commandTimeout);
					return value.FirstOrDefault();
				};

				string countQuery = "SELECT COUNT(*) FROM " + TableName + "\n";
				queries.RecordCountFunc = (connection, whereCondition, param, transaction, commandTimeout) =>
				{
					string query = countQuery + whereCondition;
					return connection.Query<int>(query, param, transaction, true, commandTimeout).First();
				};

				string truncateQuery = countQuery + ";TRUNCATE TABLE " + TableName;
				queries.DeleteWhereFunc = (connection, whereCondition, param, transaction, commandTimeout) =>
				{
					if (whereCondition.Length == 0) {
						IEnumerable<int> list = connection.Query<int>(truncateQuery, param, transaction, true, commandTimeout);
						int count = list.First();
						return count;
					}
					else {
						int count = connection.Execute(deleteQuery + whereCondition, param, transaction, commandTimeout);
						return count;
					}
				};

				queries.DeleteFunc = (connection, obj, transaction, commandTimeout) =>
				{
					return 0 < connection.Execute(deleteSingleQuery, obj, transaction, commandTimeout);
				};

				queries.DeleteDictFunc = (connection, obj, transaction, commandTimeout) =>
				{
					return 0 < connection.Execute(deleteSingleQuery, obj, transaction, commandTimeout);
				};

				queries.DeleteListFunc = (connection, whereCondition, param, transaction, buffered, commandTimeout) =>
				{
					string query = deleteListQuery + whereCondition;
					IEnumerable<T> list = connection.Query<T>(query, param, transaction, buffered, commandTimeout);
					return list;
				};

				string dropStagingTableQuery = "DROP TABLE " + BulkTempStagingTable;
				string createEqualityStagingTableQuery = CreateTableQuery(BulkTempStagingTable, EqualityColumns);
				queries.BulkDeleteFunc = (connection, objs, transaction, commandTimeout) =>
				{
					connection.Execute(createEqualityStagingTableQuery, null, transaction, commandTimeout);
					BulkInsert_(connection, objs, transaction, BulkTempStagingTable, EqualityColumns, EqualityProperties, commandTimeout);
					int count = connection.Execute(bulkDeleteQuery, null, transaction, commandTimeout);
					connection.Execute(dropStagingTableQuery, null, transaction, commandTimeout);
					return count;
				};

				queries.BulkDeleteListFunc = (connection, objs, transaction, buffered, commandTimeout) =>
				{
					connection.Execute(createEqualityStagingTableQuery, null, transaction, commandTimeout);
					BulkInsert_(connection, objs, transaction, BulkTempStagingTable, EqualityColumns, EqualityProperties, commandTimeout);
					IEnumerable<T> list = connection.Query<T>(bulkDeleteListQuery, null, transaction, buffered, commandTimeout);
					if (buffered) {
						connection.Execute(dropStagingTableQuery, null, transaction, commandTimeout);
					}
					return list;
				};

				if (KeyProperties.Length == 0) {
					// No updates
					queries.GetKeysFunc = NoKeyProperty;
					queries.UpdateFunc = NoKeyProperty;
					queries.BulkUpdateFunc = NoKeyProperty;
					queries.BulkUpdateListFunc = NoKeyProperty;

					if (valuesInserted.Length == 1) {
						// No updates
						// constant insert string
						string insertQuery = insertTableParams + valuesInserted[0].ToString();
						queries.InsertFunc = (connection, obj, transaction, commandTimeout) =>
						{
							connection.Execute(insertQuery, obj, transaction, commandTimeout);
							return obj;
						};

						string upsertQuery = "IF NOT EXISTS (\nSELECT TOP(1) * FROM " + TableName + "\n" + whereEquals + ")\n" + insertTableParams + valuesInserted;
						queries.UpsertFunc = (connection, obj, transaction, commandTimeout) =>
						{
							connection.Execute(upsertQuery, obj, transaction, commandTimeout);
							return obj;
						};

						string bulkInsertListQuery = insertTableParams + "SELECT * FROM " + BulkTempStagingTable;

						queries.BulkInsertFunc = (connection, objs, transaction, buffered, commandTimeout) =>
						{
							BulkInsert_(connection, objs, transaction, TableName, InsertColumns, InsertProperties, commandTimeout);
							return objs;
						};

						queries.BulkUpsertListFunc = (connection, objs, transaction, buffered, commandTimeout) =>
						{
							connection.Execute(createStagingTableQuery, null, transaction, commandTimeout);
							BulkInsert_(connection, objs, transaction, BulkTempStagingTable, Columns, Properties, commandTimeout);
							IEnumerable<T> list = connection.Query<T>(bulkInsertNotExistsOutputQuery, null, transaction, buffered, commandTimeout);
							if (buffered) {
								connection.Execute(dropStagingTableQuery, null, transaction, commandTimeout);
							}
							return list;
						};

						queries.BulkUpsertFunc = (connection, objs, transaction, commandTimeout) =>
						{
							connection.Execute(createStagingTableQuery, null, transaction, commandTimeout);
							BulkInsert_(connection, objs, transaction, BulkTempStagingTable, Columns, Properties, commandTimeout);
							int countInsert = connection.Execute(bulkInsertNotExistsQuery, null, transaction, commandTimeout);
							connection.Execute(dropStagingTableQuery, null, transaction, commandTimeout);
							return countInsert;
						};
					}
					else {
						// No updates
						// not constant insert string
						queries.InsertFunc = (connection, obj, transaction, commandTimeout) =>
						{
							string query = insertTableParams + string.Concat(valuesInserted);
							connection.Execute(query, obj, transaction, commandTimeout);
							return obj;
						};

						string upsertQuery = "IF NOT EXISTS (\nSELECT TOP(1) * FROM " + TableName + "\n" + whereEquals + ")\n" + insertTableParams;
						queries.UpsertFunc = (connection, obj, transaction, commandTimeout) =>
						{
							string query = upsertQuery + string.Concat(valuesInserted);
							connection.Execute(query, obj, transaction, commandTimeout);
							return obj;
						};

						queries.BulkInsertFunc = (connection, objs, transaction, buffered, commandTimeout) =>
						{
							foreach (T obj in objs) {
								queries.InsertFunc(connection, obj, transaction, commandTimeout);
							}
							return objs;
						};

						queries.BulkUpsertListFunc = (connection, objs, transaction, buffered, commandTimeout) =>
						{
							foreach (T obj in objs) {
								queries.UpsertFunc(connection, obj, transaction, commandTimeout);
							}
							return objs;
						};

						queries.BulkUpsertFunc = (connection, objs, transaction, commandTimeout) =>
						{
							foreach (T obj in objs) {
								queries.UpsertFunc(connection, obj, transaction, commandTimeout);
							}
							return objs.Count();
						};
					}
				}
				else {
					// Updates allowed
					string selectListKeysQuery = "SELECT " + GetAsParams(KeyProperties) + " FROM " + TableName + "\n";
					queries.GetKeysFunc = (connection, whereCondition, param, transaction, buffered, commandTimeout) =>
					{
						string query = selectListKeysQuery + whereCondition;
						return connection.Query<T>(query, param, transaction, buffered, commandTimeout);
					};

					if (valuesInserted.Length == 1) {
						// Constant insert strings
						string insertQuery = insertTableParams + outputInsertedKeys + valuesInserted[0].ToString();
						queries.InsertFunc = (connection, obj, transaction, commandTimeout) =>
						{
							IEnumerable<dynamic> key = connection.Query<dynamic>(insertQuery, obj, transaction, true, commandTimeout);
							dynamic keyVal = key.First();
							TableData<T>.SetKey(obj, (IDictionary<string, object>) keyVal);
							return obj;
						};

						string bulkInsertListQuery = insertTableParams + outputInserted + "SELECT * FROM " + BulkTempStagingTable;
						string createInsertStagingTableQuery = CreateTableQuery(BulkTempStagingTable, InsertColumns);
						queries.BulkInsertFunc = (connection, objs, transaction, buffered, commandTimeout) =>
						{
							connection.Execute(createInsertStagingTableQuery, null, transaction, commandTimeout);
							BulkInsert_(connection, objs, transaction, BulkTempStagingTable, InsertColumns, InsertProperties, commandTimeout);
							IEnumerable<T> result = connection.Query<T>(bulkInsertListQuery, null, transaction, buffered, commandTimeout);
							if (buffered) {
								connection.Execute(dropStagingTableQuery, null, transaction, commandTimeout);
							}
							return result;
						};
					}
					else {
						// not constant insert string
						string insertQuery = insertTableParams + outputInsertedKeys;
						queries.InsertFunc = (connection, obj, transaction, commandTimeout) =>
						{
							string query = insertQuery + string.Concat(valuesInserted);
							IEnumerable<dynamic> key = connection.Query<dynamic>(query, obj, transaction, true, commandTimeout);
							dynamic keyVal = key.First();
							TableData<T>.SetKey(obj, (IDictionary<string, object>) keyVal);
							return obj;
						};

						queries.BulkInsertFunc = (connection, objs, transaction, buffered, commandTimeout) =>
						{
							foreach (T obj in objs) {
								queries.InsertFunc(connection, obj, transaction, commandTimeout);
							}
							return objs;
						};
					}

					string bulkUpdateQueryStart = "UPDATE " + TableName + "\nSET ";
					string bulkUpdateQueryEnd = "FROM " + BulkTempStagingTable + "\nWHERE " + GetTempAndEqualsParams(UpdateEqualityColumns);

					if (updateSetParams.Length == 1) {
						// Constant update strings
						string updateQuery = updateSetParams[0].ToString() + whereUpdateEquals;
						queries.UpdateFunc = (connection, obj, transaction, commandTimeout) =>
						{
							return 0 < connection.Execute(updateQuery, obj, transaction, commandTimeout);
						};

						string createUpdateStagingTableQuery = CreateTableQuery(BulkTempStagingTable, UpdateColumns);
						string bulkUpdateQuery = bulkUpdateQueryStart + bulkUpdateSetParams[0].ToString() + "\n" + bulkUpdateQueryEnd;
						queries.BulkUpdateFunc = (connection, objs, transaction, commandTimeout) =>
						{
							connection.Execute(createUpdateStagingTableQuery, null, transaction, commandTimeout);
							BulkInsert_(connection, objs, transaction, BulkTempStagingTable, UpdateColumns, UpdateProperties, commandTimeout);
							int count = connection.Execute(bulkUpdateQuery, null, transaction, commandTimeout);
							connection.Execute(dropStagingTableQuery, null, transaction, commandTimeout);
							return count;
						};

						string bulkUpdateListQuery = bulkUpdateQueryStart + bulkUpdateSetParams[0].ToString() + "\n" + outputDeleted + bulkUpdateQueryEnd;
						queries.BulkUpdateListFunc = (connection, objs, transaction, buffered, commandTimeout) =>
						{
							connection.Execute(createStagingTableQuery, null, transaction, commandTimeout);
							BulkInsert_(connection, objs, transaction, BulkTempStagingTable, Columns, Properties, commandTimeout);
							IEnumerable<T> list = connection.Query<T>(bulkUpdateListQuery, null, transaction, buffered, commandTimeout);
							if (buffered) {
								connection.Execute(dropStagingTableQuery, null, transaction, commandTimeout);
							}
							return list;
						};
					}
					else {
						queries.UpdateFunc = (connection, obj, transaction, commandTimeout) =>
						{
							string query = string.Concat(updateSetParams) + whereUpdateEquals;
							return 0 < connection.Execute(query, obj, transaction, commandTimeout);
						};

						queries.BulkUpdateFunc = (connection, objs, transaction, commandTimeout) =>
						{
							int updatedCount = 0;
							foreach (T obj in objs) {
								if (queries.UpdateFunc(connection, obj, transaction, commandTimeout)) {
									updatedCount++;
								}
							}
							return updatedCount;
						};

						queries.BulkUpdateListFunc = (connection, objs, transaction, buffered, commandTimeout) =>
						{
							List<T> result = new List<T>();
							foreach (T obj in objs) {
								if (queries.UpdateFunc(connection, obj, transaction, commandTimeout)) {
									result.Add(obj);
								}
							}
							return result;
						};
					}

					string upsertQueryStart = "IF NOT EXISTS (\nSELECT TOP(1) * FROM " + TableName + "\n" + whereEquals + ")\n" + insertTableParams + outputInsertedKeys;
					string upsertQueryEnd = outputDeleted + whereUpdateEquals;
					if (updateSetParams.Length == 1 && valuesInserted.Length == 1) {
						string upsertQuery = upsertQueryStart + valuesInserted[0].ToString() + "\n\nELSE\n\n" + updateSetParams[0].ToString() + upsertQueryEnd;
						queries.UpsertFunc = (connection, obj, transaction, commandTimeout) =>
						{
							dynamic key = connection.Query<dynamic>(upsertQuery, obj, transaction, true, commandTimeout).First();
							TableData<T>.SetKey(obj, (IDictionary<string, object>) key);
							return obj;
						};

						string bulkUpdateQuery = bulkUpdateQueryStart + bulkUpdateSetParams[0].ToString() + "\n" + bulkUpdateQueryEnd;
						queries.BulkUpsertFunc = (connection, objs, transaction, commandTimeout) =>
						{
							connection.Execute(createStagingTableQuery, null, transaction, commandTimeout);
							BulkInsert_(connection, objs, transaction, BulkTempStagingTable, Columns, Properties, commandTimeout);
							int countUpdate = connection.Execute(bulkUpdateQuery, null, transaction, commandTimeout);
							int countInsert = connection.Execute(bulkInsertNotExistsQuery, null, transaction, commandTimeout);
							connection.Execute(dropStagingTableQuery, null, transaction, commandTimeout);
							return countInsert;
						};

						string bulkUpdateListQuery = bulkUpdateQueryStart + bulkUpdateSetParams[0].ToString() + "\n" + outputDeleted + bulkUpdateQueryEnd;
						queries.BulkUpsertListFunc = (connection, objs, transaction, buffered, commandTimeout) =>
						{
							connection.Execute(createStagingTableQuery, null, transaction, commandTimeout);
							BulkInsert_(connection, objs, transaction, BulkTempStagingTable, Columns, Properties, commandTimeout);
							IEnumerable<T> result = connection.Query<T>(bulkUpdateListQuery, null, transaction, buffered, commandTimeout);
							IEnumerable<T> insertedList = connection.Query<T>(bulkInsertNotExistsOutputQuery, null, transaction, buffered, commandTimeout);
							if (buffered) {
								connection.Execute(dropStagingTableQuery, null, transaction, commandTimeout);
							}
							return result.Union(insertedList);
						};
					}
					else {
						queries.UpsertFunc = (connection, obj, transaction, commandTimeout) =>
						{
							string query = upsertQueryStart + string.Concat(valuesInserted) + "\n\nELSE\n\n" + string.Concat(updateSetParams) + upsertQueryEnd;
							dynamic key = connection.Query<dynamic>(query, obj, transaction, true, commandTimeout).First();
							TableData<T>.SetKey(obj, (IDictionary<string, object>) key);
							return obj;
						};

						queries.BulkUpsertListFunc = (connection, objs, transaction, buffered, commandTimeout) =>
						{
							List<T> result = new List<T>();
							foreach (T obj in objs) {
								T newObj = queries.UpsertFunc(connection, obj, transaction, commandTimeout);
								result.Add(newObj);
							}
							return result;
						};

						queries.BulkUpsertFunc = (connection, objs, transaction, commandTimeout) =>
						{
							int insertedCount = 0;
							foreach (T obj in objs) {
								T newObj = queries.UpsertFunc(connection, obj, transaction, commandTimeout);
								insertedCount++;
							}
							return insertedCount;
						};
					}
				}

				return queries;
			}

			public TableQueries<T, KeyType>.Data Create<KeyType>()
			{
				if (KeyProperties.Length != 1) {
					string errorMsg = typeof(T).Name + " requires a single key";
					throw new InvalidOperationException(errorMsg);
				}
				TableQueries<T, KeyType>.Data queries = new TableQueries<T, KeyType>.Data();

				string deleteListKeysQuery = deleteQuery + outputDeletedKeys;
				queries.DeleteKeysWhereFunc = (connection, whereCondition, param, transaction, buffered, commandTimeout) =>
				{
					string query = deleteListKeysQuery + whereCondition;
					return connection.Query<KeyType>(query, param, transaction, buffered, commandTimeout);
				};

				string selectListKeysQuery = "SELECT " + GetAsParams(KeyProperties.Length == 0 ? SelectProperties : KeyProperties) + " FROM " + TableName + "\n";
				queries.GetKeysWhereFunc = (connection, whereCondition, param, transaction, buffered, commandTimeout) =>
				{
					string query = selectListKeysQuery + whereCondition;
					return connection.Query<KeyType>(query, param, transaction, buffered, commandTimeout);
				};

				string keyCol = KeyColumns[0];
				queries.DeleteKeyFunc = (connection, key, transaction, commandTimeout) =>
				{
					//IDictionary<string, object> keyObj = new ExpandoObject();
					DynamicParameters keyObj = new DynamicParameters();
					keyObj.Add(keyCol, key);
					return 0 < connection.Execute(deleteSingleQuery, keyObj, transaction, commandTimeout);
				};

				queries.GetKeyFunc = (connection, key, transaction, commandTimeout) =>
				{
					//IDictionary<string, object> keyObj = new ExpandoObject();
					DynamicParameters keyObj = new DynamicParameters();
					keyObj.Add(keyCol, key);
					return connection.Query<T>(selectSingleQuery, keyObj, transaction, true, commandTimeout).FirstOrDefault();
				};

				queries.DeleteKeysWhereFunc = (connection, whereCondition, param, transaction, buffered, commandTimeout) =>
				{
					string query = deleteListKeysQuery + whereCondition;
					return connection.Query<KeyType>(query, param, transaction, buffered, commandTimeout);
				};

				string whereKeyInList = "WHERE [" + keyCol + "] in @Keys";
				string bulkDeleteQuery = deleteQuery + whereKeyInList;
				queries.BulkDeleteFunc = (connection, keys, transaction, commandTimeout) =>
				{
					int count = 0;
					foreach (IEnumerable<KeyType> Keys in Partition<KeyType>(keys, 2000)) {
						int deleted = connection.Execute(bulkDeleteQuery, new { Keys }, transaction, commandTimeout);
						count += deleted;
					}
					return count;
				};

				string bulkDeleteKeysQuery = deleteListKeysQuery + whereKeyInList;
				queries.BulkDeleteListFunc = (connection, keys, transaction, buffered, commandTimeout) =>
				{
					List<KeyType> result = new List<KeyType>();
					foreach (IEnumerable<KeyType> Keys in Partition<KeyType>(keys, 2000)) {
						IEnumerable<KeyType> list = connection.Query<KeyType>(bulkDeleteKeysQuery, new { Keys }, transaction, buffered, commandTimeout);
						result.AddRange(list);
					}
					return result;
				};
				return queries;
			}

			/// <summary>
			/// [Table("Name")] or the class name
			/// </summary>
			public string TableName { get; private set; }

			public TableAttribute TableAttribute { get; private set; }

			public PropertyInfo[] Properties { get; protected set; }
			public PropertyInfo[] KeyProperties { get; protected set; }
			public PropertyInfo[] AutoKeyProperties { get; protected set; }
			public PropertyInfo[] SelectProperties { get; protected set; }
			public PropertyInfo[] UpdateProperties { get; protected set; }
			public PropertyInfo[] InsertProperties { get; protected set; }
			public PropertyInfo[] EqualityProperties { get; protected set; }
			public PropertyInfo[] MatchUpdateProperties { get; protected set; }
			public PropertyInfo[] UpdateEqualityProperties { get; protected set; }
			public PropertyInfo[] DeleteEqualityProperties { get; protected set; }

			public static string[] Columns { get; protected set; }
			public static string[] KeyColumns { get; protected set; }
			public string[] AutoKeyColumns { get; private set; }
			public string[] SelectColumns { get; private set; }
			public string[] UpdateColumns { get; private set; }
			public string[] InsertColumns { get; private set; }
			public string[] EqualityColumns { get; private set; }
			public string[] UpdateEqualityColumns { get; private set; }
			public string[] DeleteEqualityColumns { get; private set; }

			public IDefaultAttribute[] InsertDefaults { get; private set; }
			public IDefaultAttribute[] UpdateDefaults { get; private set; }


			private string whereEquals { get; set; }
			private string whereUpdateEquals { get; set; }
			private string whereDeleteEquals { get; set; }
			private string paramsInsert { get; set; }
			private string insertTableParams { get; set; }
			private object[] valuesInserted { get; set; } // list of strings and IHasDefaultAttribute
			private string outputInserted { get; set; }
			private string outputInsertedKeys { get; set; }
			private string outputDeleted { get; set; }
			private string outputDeletedKeys { get; set; }
			private object[] updateSetParams { get; set; } // list of strings and IHasDefaultAttribute
			private string whereDeleteExistsBulk { get; set; }
			private object[] bulkUpdateSetParams { get; set; } // list of strings and IHasDefaultAttribute

			protected string deleteQuery { get; set; }
			protected string deleteSingleQuery { get; set; }
			protected string paramsSelectFrom { get; set; }
			protected string selectSingleQuery { get; set; }

			/// <summary>
			/// [Column("Name")] or the name of the properties.
			/// </summary>
			public string[] GetColumnNames(params PropertyInfo[] properties)
			{
				string[] columnNames = new string[properties.Length];
				for (int i = 0; i < properties.Length; i++) {
					columnNames[i] = properties[i].Name;
					ColumnAttribute colAttr = properties[i].GetCustomAttribute<ColumnAttribute>(true);
					if (colAttr != null) {
						columnNames[i] = colAttr.Name.Replace("'", "''");
					}
				}
				return columnNames;
			}

			private string CreateTableQuery(string tempTable, string[] columns)
			{
				return $"DROP TABLE IF EXISTS " + tempTable + "; SELECT TOP(0) " + string.Join(",", columns) + " INTO " + tempTable + " FROM " + TableName;
			}

			private static IEnumerable<IEnumerable<Ty>> Partition<Ty>(IEnumerable<Ty> source, int size)
			{
				while (source.Any()) {
					yield return source.Take(size);
					source = source.Skip(size);
				}
			}

			/// <summary>
			/// Returns a <see cref="PropertyInfo"/>[] from the base Properties except the ones specific to be ignored.
			/// </summary>
			/// <param name="ignoredProperties">PropertyInfos in this list are ignored.</param>
			/// <param name="ignoredAttributes">PropertyInfos with these any of these Attributes are ignored.</param>
			/// <returns>A <see cref="PropertyInfo"/>[] from the base Properties except the ones specific to be ignored.</returns>
			protected PropertyInfo[] GetProperties(PropertyInfo[] ignoredProperties, Func<PropertyInfo, bool> accepted, params Type[] ignoredAttributes)
			{
				List<PropertyInfo> properties = new List<PropertyInfo>();
				for (int i = 0; i < Properties.Length; i++) {
					if (ignoredProperties.Contains(Properties[i])) {
						continue;
					}
					Attribute ignoredAttr = null;
					for (int j = 0; j < ignoredAttributes.Length; j++) {
						ignoredAttr = Properties[i].GetCustomAttribute(ignoredAttributes[j], true);
						if (ignoredAttr != null) {
							break;
						}
					}
					if (ignoredAttr == null && accepted(Properties[i])) {
						properties.Add(Properties[i]);
					}
				}
				return properties.Count == Properties.Length
					? Properties : properties.ToArray();
			}

			/// <summary>
			/// OUTPUT INSERTED.[a] as A
			/// </summary>
			private string GetOutput(string type, PropertyInfo[] properties)
			{
				if (properties.Length == 0)
					return "";
				string[] columnNames = GetColumnNames(properties);
				StringBuilder sb = new StringBuilder();
				for (int i = 0; i < columnNames.Length; i++) {
					sb.AppendFormat("\t,{0}.[{1}]", type, columnNames[i]);
					if (columnNames[i] != properties[i].Name) {
						sb.AppendFormat(" as [{0}]", properties[i].Name);
					}
					sb.Append("\n");
				}
				return "OUTPUT \t" + sb.Remove(0, 2).ToString();
			}

			/// <summary>
			/// #BulkTempTable_.[x] = TableName.[x] AND #BulkTempTable_.[y] = TableName.[y]
			/// </summary>
			private string GetTempAndEqualsParams(string[] columns)
			{
				if (columns.Length == 0)
					return "";
				StringBuilder sb = new StringBuilder();
				for (int i = 0; i < columns.Length; i++) {
					sb.AppendFormat("\t AND {1}.[{0}] = [{2}].[{0}]\n", columns[i], TableName, BulkTempStagingTable);
				}
				return "\t" + sb.Remove(0, 6).ToString();
			}

			/// <summary>
			/// #BulkTempTable_.[x] = TableName.[x] as [name]
			/// </summary>
			private string GetAsParams(PropertyInfo[] properties)
			{
				if (properties.Length == 0)
					return "";

				string[] columnNames = GetColumnNames(properties);
				StringBuilder sb = new StringBuilder();
				for (int i = 0; i < properties.Length; i++) {
					sb.Append("[" + columnNames[i] + "]");
					if (properties[i].Name != columnNames[i]) {
						sb.Append(" as [" + properties[i].Name + "]");
					}
					sb.Append(',');
				}
				sb.Remove(sb.Length - 1, 1);
				return sb.ToString();
			}

			/// <summary>
			/// x = @x, y = @y
			/// x = @x AND y = @y
			/// x = @x OR  y = @y
			/// </summary>
			private string GetEqualsParams(string joinString, PropertyInfo[] properties, string[] columnNames)
			{
				if (properties.Length == 0)
					return "";
				StringBuilder sb = new StringBuilder();
				for (int i = 0; i < columnNames.Length; i++) {
					sb.Append("\t").Append(joinString).Append('[').Append(columnNames[i]).Append("] = @").Append(properties[i].Name).Append('\n');
				}
				return sb.Remove(0, joinString.Length + 1).Insert(0, '\t').ToString();
			}

			/// <summary>
			/// TableName.[x] = #BulkTempTable_.[x], TableName.[y] = getdate()
			/// </summary>
			private IEnumerable<object> GetTempSetParams(string[] columnNames, IDefaultAttribute[] defaultValues)
			{
				if (columnNames.Length == 0)
					return new List<object>() { "" };
				List<object> result = new List<object>();
				for (int i = 0; i < columnNames.Length; i++) {
					result.Add("\t" + TableName + ".[" + columnNames[i] + "] = ");
					if (defaultValues[i] == null) {
						result.Add("[");
						result.Add(BulkTempStagingTable);
						result.Add("].[");
						result.Add(columnNames[i]);
						result.Add("]");
					}
					else {
						if (defaultValues[i].IsConstant) {
							result.Add(defaultValues[i].Value());
						}
						else {
							result.Add(defaultValues[i]);
						}
					}
					result.Add(",\n");
				}
				result[result.Count - 1] = "\n"; // replace
				return result;
			}


			/// <summary>
			/// [x] = [x], [y] = getdate()
			/// </summary>
			private IEnumerable<object> GetSetParams(string[] columnNames, PropertyInfo[] properties, IDefaultAttribute[] defaultValues)
			{
				if (columnNames.Length == 0)
					return new List<object>() { "" };
				List<object> result = new List<object>();
				for (int i = 0; i < columnNames.Length; i++) {
					result.Add("\t[");
					result.Add(columnNames[i]);
					result.Add("] = ");
					if (defaultValues[i] == null) {
						result.Add("@");
						result.Add(properties[i].Name);
					}
					else {
						if (defaultValues[i].IsConstant) {
							result.Add(defaultValues[i].Value());
						}
						else {
							result.Add(defaultValues[i]);
						}
					}
					result.Add(",\n");
				}
				result[result.Count - 1] = "\n"; // replace
				return result;
			}

			/// <summary>
			/// @a,@b,@c
			/// </summary>
			private IEnumerable<object> GetValues(PropertyInfo[] properties, IDefaultAttribute[] defaultValues)
			{
				if (properties.Length == 0)
					return new List<object>() { "" };
				List<object> result = new List<object>();
				for (int i = 0; i < properties.Length; i++) {
					if (defaultValues[i] == null) {
						result.Add("@");
						result.Add(properties[i].Name);
					}
					else {
						if (defaultValues[i].IsConstant) {
							result.Add(defaultValues[i].Value());
						}
						else {
							result.Add(defaultValues[i]);
						}
					}
					result.Add(",");
				}
				result.RemoveAt(result.Count - 1);
				return result;
			}

			//private string DefaultValue(PropertyInfo property, IHasDefaultAttribute defaultValue, int index)
			//{
			//	return defaultValue == null ? ("@" + property.Name) : (defaultValue.IsConstant ? defaultValue.Value() : "@__my_DefaultVal0" + index);
			//}

			/*
			#endregion ITableQueries<T>

			public override int RemoveDuplicates(IDbConnection connection, IDbTransaction transaction, int? commandTimeout = null)
			{
				if (KeyProperties.Length == 1) {
					try {
						return RemoveDuplicatesKey_(connection, KeyColumns[0], transaction, commandTimeout);
					}
					catch { }
				}
				else if (KeyProperties.Length == 0) {
					try {
						connection.Execute(@"ALTER TABLE " + TableName + " ADD _TempIDColumn INT IDENTITY(1,1)", null, transaction, commandTimeout);
						int count = RemoveDuplicatesKey_(connection, "_TempIDColumn", transaction, commandTimeout);
						connection.Execute(@"ALTER TABLE " + TableName + " DROP COLUMN _TempIDColumn", null, transaction, commandTimeout);
						return count;
					}
					catch { }
				}
				return RemoveDuplicates_(connection, transaction, commandTimeout);
			}

			private int RemoveDuplicates_(IDbConnection connection, IDbTransaction transaction = null, int? commandTimeout = null)
			{
				//connection.Execute("DROP TABLE " + BulkTempStagingTable, null, transaction, commandTimeout);
				string sql = "SELECT DISTINCT [" + string.Join("],[", GetColumnNames(Properties.Where(prop => !KeyProperties.Contains(prop)).ToArray())) + "]\nINTO " + BulkTempStagingTable + " FROM " + TableName;
				int currCount = RecordCount(connection, "", null, transaction, commandTimeout);
				connection.Execute(sql, null, transaction, commandTimeout);
				connection.Execute("TRUNCATE TABLE " + TableName, null, transaction, commandTimeout);
				int count = connection.Execute("INSERT INTO " + TableName + " SELECT * FROM " + BulkTempStagingTable, null, transaction, commandTimeout);
				connection.Execute("DROP TABLE " + BulkTempStagingTable, null, transaction, commandTimeout);
				return currCount - count;
			}

			private int RemoveDuplicatesKey_(IDbConnection connection, string keyColumn, IDbTransaction transaction = null, int? commandTimeout = null)
			{
				return connection.Execute(@"WHILE EXISTS (SELECT COUNT(*) FROM [" + TableName + "] GROUP BY [" + keyColumn + "] HAVING COUNT(*) > 1" + @")
	BEGIN
		DELETE FROM " + TableName + @" WHERE " + keyColumn + @" IN 
		(
			SELECT MIN([" + keyColumn + @"]) as [DeleteID]
			" + " FROM " + TableName + " GROUP BY [" + keyColumn + "] HAVING COUNT(*) > 1" + @"
		)
	END", null, transaction, commandTimeout);
			}

			private IEnumerable<Tuple<string, DynamicParameters>> CreateDynamicParams(IEnumerable<T> objs)
			{
				int max = 2000 / EqualityProperties.Length;

				foreach (IEnumerable<T> part in Partition<T>(objs, max)) {
					int k = 0;
					StringBuilder sb = new StringBuilder("WHERE ");
					DynamicParameters param = new DynamicParameters();

					foreach (T obj in part) {
						sb.Append("([" + KeyColumns[0] + "] = @p" + k);
						param.Add("@p" + k, KeyProperties[0].GetValue(obj));
						k++;
						for (int j = 1; j < KeyColumns.Length; j++) {
							sb.Append(" AND [" + KeyColumns[j] + "] = @p" + k);
							param.Add("@p" + k, KeyProperties[j].GetValue(obj));
							k++;
						}
						sb.Append(")");
						sb.Append(" OR ");
					}
					sb.Remove(sb.Length - 4, 4);
					yield return new Tuple<string, DynamicParameters>(sb.ToString(), param);
				}
			}
			*/
		}
	}
}