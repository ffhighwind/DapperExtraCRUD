using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Dapper.Extra.Interfaces;

namespace Dapper.Extra
{
	public static class TableFactory
	{
		private static SqlSyntax _DefaultSyntax = SqlSyntax.SQLServer;
		public static SqlSyntax DefaultSyntax {
			get => _DefaultSyntax;
			set {
				switch (value) {
					case SqlSyntax.MySQL:
					case SqlSyntax.PostgreSQL:
					case SqlSyntax.SQLite:
					case SqlSyntax.SQLServer:
						_DefaultSyntax = value;
						break;
					case SqlSyntax.Oracle:
						throw new InvalidOperationException("Oracle syntax is not supported.");
					default:
						_DefaultSyntax = SqlSyntax.SQLServer;
						break;
				}
			}
		}

		public static SqlSyntax DetectSyntax(IDbConnection conn)
		{
			bool notOpen = conn.State != ConnectionState.Open;
			if (notOpen)
				conn.Open();
			SqlSyntax syntax = _DetectSyntax(conn);
			if (notOpen)
				conn.Close();
			return syntax;
		}

		public static SqlSyntax _DetectSyntax(IDbConnection conn)
		{
			// SQLServer
			try {
				//int c = conn.QuerySingle<int>("SELECT 1 as [x]");
				//int? b = conn.QueryFirstOrDefault<int?>("SELECT CAST(SCOPE_IDENTITY() as INT) as [Id]");
				string s = conn.QuerySingle<string>("SELECT 'a' + 'b'");
				int a = conn.QuerySingle<int>("SELECT TOP(1) * FROM (SELECT 1) as X(Id)");
				//int a = conn.QuerySingle<int>("SELECT SQUARE(1)");
				//DateTime date = conn.QuerySingle<DateTime>("SELECT GETDATE()");
				//string s = conn.QuerySingle<string>("SELECT RTRIM(LTRIM(' a '))");
				return SqlSyntax.SQLServer;
			}
			catch { }

			// MySQL
			try {
				int z = conn.QuerySingle<int>("SELECT 1 as `x`");
				int? c = conn.QueryFirstOrDefault<int?>("SELECT LAST_INSERT_ID() as Id");
				//decimal b = conn.QuerySingle<decimal>("SELECT POW(1,1)"); // MySQL
				//string s = conn.QuerySingle<string>("SELECT @@version"); // SQLServer + MySQL
				//int a = conn.QuerySingle<int>("SELECT 1 LIMIT 1");
				return SqlSyntax.MySQL;
			}
			catch { }

			try {
				int? a = conn.QueryFirstOrDefault<int?>("SELECT LAST_INSERT_ROWID() as Id");
				return SqlSyntax.SQLite;
			}
			catch { }

			// PostgreSQL
			try {
				string s = conn.QuerySingle<string>("'a' || 'b'"); // Oracle + PostgreSQL
																   //DateTime now = conn.QuerySingle<DateTime>("NOW()"); // PostgreSQL + MySQL
				int? a = conn.QueryFirstOrDefault<int?>("SELECT LASTVAL() as Id");
				return SqlSyntax.PostgreSQL;
			}
			catch { }

			// Oracle
			try {
				int a = conn.QuerySingle<int>("SELECT BITAND(1,1)");
				decimal b = conn.QuerySingle<decimal>("POWER(1,1)");
				return SqlSyntax.Oracle;
			}
			catch { }
			throw new InvalidOperationException("Unknown RDBMS");
		}

		public static TableFactory<T> Create<T>(SqlSyntax syntax)
			where T : class
		{
			if (syntax == SqlSyntax.Oracle)
				throw new InvalidOperationException("Oracle syntax is not supported.");
			return new TableFactory<T>(syntax);
		}

		public static TableFactory<T> Create<T>()
			where T : class
		{
			return new TableFactory<T>(_DefaultSyntax);
		}

		internal static readonly IReadOnlyCollection<Type> ValidPropertyTypes = new Type[]
		{
			typeof(string),
			typeof(int),
			typeof(DateTime),
			typeof(decimal),
			typeof(bool),
			typeof(double),
			typeof(char),
			typeof(float),
			typeof(byte[]),
			typeof(byte),
			typeof(short),
			typeof(long),
			typeof(Guid),
			typeof(DateTimeOffset),
			typeof(TimeSpan),
			typeof(uint),
			typeof(ushort),
			typeof(ulong),
			typeof(sbyte),
		};

		internal static readonly IReadOnlyCollection<Type> ValidAutoKeyTypes = new List<Type>()
		{
			typeof(int),
			typeof(short),
			typeof(byte),
			typeof(long),
			typeof(uint),
			typeof(ulong),
			typeof(ushort),
			typeof(sbyte),
		};

		/// <summary>
		/// [Column("Name")] or the name of the property.
		/// </summary>
		/// <param name="property">The <see cref="PropertyInfo"/> representing the column.</param>
		/// <returns>The name of the column.</returns>
		public static string GetColumnName(PropertyInfo property)
		{
			return property.GetCustomAttribute<ColumnAttribute>(true)?.Name ?? property.Name;
		}

		public static bool IsValidProperty(PropertyInfo prop)
		{
			if (!prop.CanRead || !prop.CanWrite
				|| prop.GetCustomAttribute<IgnoreAttribute>(true) != null
				|| (prop.GetCustomAttribute<IgnoreSelectAttribute>(true) != null
					&& prop.GetCustomAttribute<IgnoreInsertAttribute>(true) != null
					&& prop.GetCustomAttribute<IgnoreUpdateAttribute>(true) != null)) {
				return false;
			}
			Type propertyType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
			if (ValidPropertyTypes.Contains(propertyType) || propertyType.IsEnum)
				return true;
			if (propertyType.IsGenericType && typeof(IEnumerable<>).IsAssignableFrom(propertyType)) {
				Type underlyingType = propertyType.GetGenericArguments()[0];
				return underlyingType == typeof(byte);
			}
			return propertyType.GetInterfaces().Any(ty => ty.IsGenericType && ty.GetGenericTypeDefinition() == typeof(SqlMapper.TypeHandler<>));
		}

		public static void BulkInsert<T>(SqlConnection connection, IEnumerable<T> objs, SqlTransaction transaction, string tableName, IReadOnlyList<string> columns,
			IReadOnlyList<PropertyInfo> properties, int? commandTimeout, SqlBulkCopyOptions options)
			where T : class
		{
			FastMember.ObjectReader dataReader = FastMember.ObjectReader.Create<T>(objs, properties.Select(p => p.Name).ToArray());
			using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection, options, transaction)) {
				bulkCopy.DestinationTableName = tableName ?? TableData<T>.TableName;
				bulkCopy.BulkCopyTimeout = commandTimeout ?? 0;
				for (int i = 0; i < columns.Count; i++) {
					bulkCopy.ColumnMappings.Add(properties[i].Name, columns[i]);
				}
				bulkCopy.WriteToServer(dataReader);
			}
		}

		internal class FilterComparer : IEqualityComparer<object>
		{
			public new bool Equals(object x, object y)
			{

				return object.ReferenceEquals(x, y);
			}

			public int GetHashCode(object obj)
			{
				return System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(obj);
			}
		}

		internal static FilterComparer DefaultFilterComparer = new FilterComparer();
	}


	public class TableFactory<T>
		where T : class
	{
		/// <summary>
		/// [Table("Name")] or the class name
		/// </summary>
		public string TableName { get; private set; }
		public string BulkStagingTable { get; private set; }
		public TableAttribute TableAttribute { get; private set; }
		public SqlSyntax Syntax { get; private set; }
		private char quoteL { get; set; }
		private char quoteR { get; set; }
		private string quoteRstr { get; set; }
		private string escapeQuoteR { get; set; }
		private string selectIdentityQuery { get; set; }

		public IReadOnlyList<PropertyInfo> Properties { get; private set; }
		public IReadOnlyList<PropertyInfo> KeyProperties { get; private set; }
		public PropertyInfo AutoKeyProperty { get; private set; }
		public IReadOnlyList<PropertyInfo> SelectProperties { get; private set; }
		public IReadOnlyList<PropertyInfo> UpdateProperties { get; private set; }
		public IReadOnlyList<PropertyInfo> InsertProperties { get; private set; }
		public IReadOnlyList<PropertyInfo> EqualityProperties { get; private set; }
		public IReadOnlyList<PropertyInfo> UpdateEqualityProperties { get; private set; }
		public IReadOnlyList<PropertyInfo> DeleteEqualityProperties { get; private set; }
		public IReadOnlyList<PropertyInfo> BulkUpdateProperties { get; private set; }

		public IReadOnlyList<string> Columns { get; private set; }
		public IReadOnlyList<string> KeyColumns { get; private set; }
		public string AutoKeyColumn { get; private set; }
		public IReadOnlyList<string> SelectColumns { get; private set; }
		public IReadOnlyList<string> UpdateColumns { get; private set; }
		public IReadOnlyList<string> InsertColumns { get; private set; }
		public IReadOnlyList<string> EqualityColumns { get; private set; }
		public IReadOnlyList<string> UpdateEqualityColumns { get; private set; }
		public IReadOnlyList<string> DeleteEqualityColumns { get; private set; }
		public IReadOnlyList<string> BulkUpdateColumns { get; private set; }

		public IDefaultAttribute[] InsertDefaults { get; private set; }
		public IDefaultAttribute[] UpdateDefaults { get; private set; }

		private readonly string whereEquals;
		private readonly string whereUpdateEquals;
		private readonly string whereDeleteEquals;
		private readonly string paramsInsert;
		private readonly string insertTableParams;
		private readonly string valuesInserted;
		private readonly string updateSetParams;
		private readonly string whereDeleteExistsBulk;
		private readonly string bulkUpdateSetParams;
		private readonly string paramsSelectFrom;

		private readonly string deleteQuery;
		private readonly string deleteSingleQuery;
		private readonly string selectSingleQuery;
		private readonly string dropStagingTableQuery;
		private string dropStagingQuery { get; set; }
		private readonly string selectKeysQuery;

		internal TableFactory(SqlSyntax syntax)
		{
			Syntax = syntax;
			string tableName = typeof(T).GetCustomAttribute<TableAttribute>(true)?.Name ?? typeof(T).Name;
			TableName = EscapeIdentifier(tableName);
			BulkStagingTable = EscapeIdentifier("#_" + tableName);

			Properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(TableFactory.IsValidProperty).ToArray();
			if (Properties.Count == 0)
				throw new InvalidOperationException(typeof(T).FullName + " does not have any valid properties.");

			KeyProperties = Properties.Where(prop => prop.GetCustomAttribute<KeyAttribute>(true) != null).ToArray();
			PropertyInfo[] autoKeyProperties = KeyProperties.Where(prop => prop.GetCustomAttribute<KeyAttribute>(true).AutoIncrement).ToArray();
			if (autoKeyProperties.Length > 0) {
				if (autoKeyProperties.Length > 1)
					throw new InvalidOperationException(typeof(T).FullName + " cannot have multiple autoincrement keys.");
				AutoKeyProperty = autoKeyProperties[0];
				AutoKeyColumn = TableFactory.GetColumnName(AutoKeyProperty);
				if (!TableFactory.ValidAutoKeyTypes.Contains(AutoKeyProperty.PropertyType) && !AutoKeyProperty.PropertyType.IsEnum)
					throw new InvalidOperationException("Autoincrement key must be an integral type: " + typeof(T).FullName + "." + AutoKeyProperty.Name);
				if (KeyProperties.Count != 1)
					throw new InvalidOperationException(typeof(T).FullName + " cannot have both an autoincrement and composite key.");
			}

			SelectProperties = Properties.Where(prop => prop.GetCustomAttribute<KeyAttribute>(true) != null || prop.GetCustomAttribute<IgnoreSelectAttribute>(true) == null).ToArray();

			bool noInserts = typeof(T).GetCustomAttribute<NoInsertsAttribute>(true) != null;
			InsertProperties = noInserts ? Array.Empty<PropertyInfo>() : Properties.Where(prop =>
				{
					if (prop == AutoKeyProperty)
						return false;
					IgnoreInsertAttribute attr = prop.GetCustomAttribute<IgnoreInsertAttribute>(true);
					return attr == null || attr.HasValue;
				}).ToArray();
			bool noUpdates = typeof(T).GetCustomAttribute<NoUpdatesAttribute>(true) != null;
			UpdateProperties = noUpdates ? Array.Empty<PropertyInfo>() : Properties.Where(prop =>
				{
					if (prop.GetCustomAttribute<KeyAttribute>(true) != null)
						return false;
					IgnoreUpdateAttribute iattr = prop.GetCustomAttribute<IgnoreUpdateAttribute>(true);
					if (iattr != null && !iattr.HasValue)
						return false;
					MatchUpdateAttribute mattr = prop.GetCustomAttribute<MatchUpdateAttribute>(true);
					return mattr == null || mattr.HasValue;
				}).ToArray();
			BulkUpdateProperties = noUpdates ? Array.Empty<PropertyInfo>() : KeyProperties.Concat(UpdateProperties).ToArray();
			HandleSyntax(tableName);

			Columns = GetColumnNames(Properties);
			KeyColumns = GetColumnNames(KeyProperties);
			SelectColumns = GetColumnNames(SelectProperties);
			UpdateColumns = GetColumnNames(UpdateProperties);
			InsertColumns = GetColumnNames(InsertProperties);
			BulkUpdateColumns = GetColumnNames(BulkUpdateProperties);

			if (KeyProperties.Count == 0) {
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
				DeleteEqualityProperties = Properties.Where(prop => prop.GetCustomAttribute<KeyAttribute>(true) != null
					|| prop.GetCustomAttribute<MatchDeleteAttribute>(true) != null).ToArray();
				UpdateEqualityProperties = Properties.Where(prop => prop.GetCustomAttribute<IgnoreUpdateAttribute>(true) == null
					&& (prop.GetCustomAttribute<KeyAttribute>(true) != null || prop.GetCustomAttribute<MatchUpdateAttribute>(true) != null)).ToArray();
				UpdateEqualityColumns = GetColumnNames(UpdateEqualityProperties);
				DeleteEqualityColumns = GetColumnNames(DeleteEqualityProperties);
			}
			bool noDeletes = typeof(T).GetCustomAttribute<NoDeletesAttribute>(true) != null;
			if (noDeletes) {
				DeleteEqualityColumns = Array.Empty<string>();
				DeleteEqualityProperties = Array.Empty<PropertyInfo>();
			}

			InsertDefaults = InsertProperties.Select(prop => prop.GetCustomAttribute<IgnoreInsertAttribute>(true)).ToArray();
			UpdateDefaults = UpdateProperties.Select(prop => (IDefaultAttribute) prop.GetCustomAttribute<IgnoreUpdateAttribute>(true)
				?? prop.GetCustomAttribute<MatchUpdateAttribute>(true)).ToArray();

			whereEquals = GetWhereEqualsParams(EqualityProperties);
			whereUpdateEquals = GetWhereEqualsParams(UpdateEqualityProperties);
			whereDeleteEquals = GetWhereEqualsParams(DeleteEqualityProperties);
			whereDeleteExistsBulk = "WHERE EXISTS (\nSELECT * FROM " + BulkStagingTable + GetTempAndEquals(BulkStagingTable, DeleteEqualityColumns) + ")";

			paramsInsert = string.Join(",", InsertColumns);
			insertTableParams = "INSERT " + TableName + " (" + paramsInsert + ")\n";
			valuesInserted = "VALUES (" + GetValues(InsertProperties, InsertDefaults) + ")\n";

			deleteQuery = "DELETE FROM " + TableName + "\n";
			deleteSingleQuery = deleteQuery + whereDeleteEquals;
			paramsSelectFrom = GetAsParams(SelectProperties) + " FROM " + TableName + "\n";
			selectSingleQuery = "SELECT " + paramsSelectFrom + whereEquals;

			updateSetParams = "UPDATE " + TableName + GetSetParams(UpdateColumns, UpdateProperties, UpdateDefaults);
			bulkUpdateSetParams = GetTempSetParams(BulkStagingTable, UpdateColumns, UpdateDefaults);
			dropStagingTableQuery = "DROP TABLE " + BulkStagingTable;
			selectKeysQuery = "SELECT " + GetAsParams(KeyProperties) + " FROM " + TableName + "\n";
		}

		private void HandleSyntax(string tableName)
		{
			switch (Syntax) {
				case SqlSyntax.PostgreSQL:
					quoteL = '"';
					quoteR = '"';
					escapeQuoteR = "\"\"";
					selectIdentityQuery = "SELECT LASTVAL() as \"Id\";";
					BulkStagingTable = EscapeIdentifier("#_" + tableName);
					dropStagingQuery = $"DROP TABLE IF EXISTS {BulkStagingTable};";
					break;
				case SqlSyntax.SQLite:
					quoteL = '"';
					quoteR = '"';
					escapeQuoteR = "\"\"";
					selectIdentityQuery = "SELECT LAST_INSERT_ROWID() as \"Id\";";
					BulkStagingTable = EscapeIdentifier("#_" + tableName);
					dropStagingQuery = $"DROP TABLE IF EXISTS {BulkStagingTable};";
					break;
				case SqlSyntax.MySQL:
					quoteL = '`';
					quoteR = '`';
					escapeQuoteR = "``";
					selectIdentityQuery = "SELECT LAST_INSERT_ID() as `Id`;";
					BulkStagingTable = EscapeIdentifier("__" + tableName); // # is comment in mySQL
					dropStagingQuery = $"DROP TEMPORARY TABLE IF EXISTS {BulkStagingTable};";
					break;
				case SqlSyntax.Oracle:
					quoteL = '\'';
					quoteR = '\'';
					escapeQuoteR = "''";
					selectIdentityQuery = null; // SEQUENCE?
					BulkStagingTable = EscapeIdentifier("__" + tableName);
					dropStagingQuery = $@"
BEGIN
	EXECUTE IMMEDIATE 'DROP TABLE __{tableName.Replace("'", "''")}';
EXCEPTION
	WHEN OTHERS THEN NULL;
END;";
					goto default;
				case SqlSyntax.SQLServer:
				default:
					quoteL = '[';
					quoteR = ']';
					escapeQuoteR = "[]]";
					selectIdentityQuery = AutoKeyProperty == null || (AutoKeyProperty.PropertyType != typeof(long) && AutoKeyProperty.PropertyType != typeof(ulong))
						? "SELECT CAST(SCOPE_IDENTITY() as INT) as [Id];"
						: "SELECT CAST(SCOPE_IDENTITY() as BIGINT) as [Id];";
					BulkStagingTable = EscapeIdentifier("#_" + tableName);
					dropStagingQuery = $@"
IF EXISTS (
	SELECT * from INFORMATION_SCHEMA.TABLES 
WHERE TABLE_NAME = '#_{tableName.Replace("'", "''")}' 
	AND TABLE_SCHEMA = 'dbo'
) 
DROP TABLE dbo.{BulkStagingTable};";
					break;
			}
			quoteRstr = quoteR.ToString();
		}

		#region NoKeyProperty
		private static readonly string NoKeyExceptionString = $"Type {typeof(T).Name} does not have a KeyProperty.";

		private static bool NoKeyProperty(IDbConnection connection, T obj, IDbTransaction transaction, int? commandTimeout)
		{
			throw new InvalidOperationException(NoKeyExceptionString);
		}

		private static bool NoKeyProperty(IDbConnection connection, T obj, object filter, IDbTransaction transaction, int? commandTimeout)
		{
			throw new InvalidOperationException(NoKeyExceptionString);
		}

		private static int NoKeyProperty(SqlConnection connection, IEnumerable<T> objs, SqlTransaction transaction, int? commandTimeout)
		{
			throw new InvalidOperationException(NoKeyExceptionString);
		}

		private static IEnumerable<T> NoKeyProperty(IDbConnection connection, string whereCondition, object param, IDbTransaction transaction, bool buffered, int? commandTimeout)
		{
			throw new InvalidOperationException(NoKeyExceptionString);
		}
		#endregion NoKeyProperty

		#region DoNothing
		private static int DoNothing(SqlConnection connection, IEnumerable<T> objs, SqlTransaction transaction, int? commandTimeout)
		{
			return 0;
		}

		private static void DoNothingSqlList(SqlConnection connection, IEnumerable<T> objs, SqlTransaction transaction, int? commandTimeout)
		{
		}

		private static bool DoNothing(IDbConnection connection, T obj, IDbTransaction transaction, int? commandTimeout)
		{
			return false;
		}

		private static bool DoNothing(IDbConnection connection, T obj, object filter, IDbTransaction transaction, int? commandTimeout)
		{
			return false;
		}

		private static int DoNothing(IDbConnection connection, string whereCondition, object param, IDbTransaction transaction, int? commandTimeout)
		{
			return 0;
		}

		private static T DoNothingDbObjObj(IDbConnection connection, T obj, IDbTransaction transaction, int? commandTimeout)
		{
			return null;
		}
		#endregion DoNothing

		private static readonly Dictionary<object, string> SetParamsFilterMap = new Dictionary<object, string>(TableFactory.DefaultFilterComparer);

		public TableQueries<T> Create()
		{
			TableQueries<T> queries = new TableQueries<T>();
			queries.Columns = Columns;
			queries.KeyColumns = KeyColumns;
			queries.Properties = Properties;
			queries.KeyProperties = KeyProperties;
			queries.AutoKeyProperty = AutoKeyProperty;
			queries.EqualityProperties = EqualityProperties;

			string createStagingTableQuery = dropStagingTableQuery + SelectIntoTableQuery(BulkStagingTable, Columns);
			string bulkInsertIfNotExistsQuery = insertTableParams + "SELECT " + paramsInsert + "\nFROM " + BulkStagingTable + "\nWHERE NOT EXISTS (\nSELECT * FROM " + TableName + GetTempAndEquals(BulkStagingTable, EqualityColumns) + ")";
			string countQuery = "SELECT COUNT(*) FROM " + TableName + "\n";

			///
			/// Selects
			///
			queries.Get = (connection, obj, transaction, commandTimeout) =>
			{
				T val = connection.Query<T>(selectSingleQuery, obj, transaction, true, commandTimeout).FirstOrDefault();
				return val;
			};
			queries.GetList = (connection, whereCondition, param, transaction, buffered, commandTimeout) =>
			{
				string query = "SELECT " + paramsSelectFrom + whereCondition;
				IEnumerable<T> result = connection.Query<T>(query, param, transaction, buffered, commandTimeout);
				return result;
			};
			queries.GetLimit = (connection, limit, whereCondition, param, transaction, buffered, commandTimeout) =>
			{
				string query = "SELECT " + paramsSelectFrom + whereCondition;
				IEnumerable<T> result = connection.Query<T>(query, param, transaction, false, commandTimeout).Take(limit);
				if (buffered)
					result = result.ToList();
				return result;
			};
			queries.GetDistinct = (connection, whereCondition, param, transaction, buffered, commandTimeout) =>
			{
				string query = "SELECT DISTINCT " + paramsSelectFrom + whereCondition;
				IEnumerable<T> result = connection.Query<T>(query, param, transaction, buffered, commandTimeout);
				return result;
			};
			queries.GetDistinctLimit = (connection, limit, whereCondition, param, transaction, buffered, commandTimeout) =>
			{
				string query = "SELECT DISTINCT " + paramsSelectFrom + whereCondition;
				IEnumerable<T> result = connection.Query<T>(query, param, transaction, false, commandTimeout).Take(limit);
				if (buffered)
					result = result.ToList();
				return result;
			};
			queries.RecordCount = (connection, whereCondition, param, transaction, commandTimeout) =>
			{
				string query = countQuery + whereCondition;
				int count = connection.Query<int>(query, param, transaction, true, commandTimeout).First();
				return count;
			};
			if (KeyProperties.Count == 0) {
				queries.GetKeys = NoKeyProperty;
			}
			else {
				queries.GetKeys = (connection, whereCondition, param, transaction, buffered, commandTimeout) =>
				{
					string query = selectKeysQuery + whereCondition;
					IEnumerable<T> result = connection.Query<T>(query, param, transaction, buffered, commandTimeout);
					return result;
				};
			}

			///
			/// Deletes
			///
			if (DeleteEqualityProperties.Count == 0) {
				//	NoDeltesAttribute
				queries.BulkDelete = DoNothing;
				queries.Delete = DoNothing;
				queries.DeleteWhere = DoNothing;
			}
			else {
				string bulkDeleteQuery = deleteQuery + whereDeleteExistsBulk;
				string bulkDeleteListQuery = deleteQuery + whereDeleteExistsBulk;
				string truncateQuery = countQuery + ";\nTRUNCATE TABLE " + TableName;
				queries.DeleteWhere = (connection, whereCondition, param, transaction, commandTimeout) =>
				{
					string sql = whereCondition.Length == 0 ? truncateQuery : deleteQuery + whereCondition;
					int count = connection.Execute(sql, param, transaction, commandTimeout);
					return count;
				};
				queries.Delete = (connection, obj, transaction, commandTimeout) =>
				{
					int count = connection.Execute(deleteSingleQuery, obj, transaction, commandTimeout);
					return count > 0;
				};
				string createEqualityStagingTableQuery = dropStagingTableQuery + SelectIntoTableQuery(BulkStagingTable, EqualityColumns);
				queries.BulkDelete = (connection, objs, transaction, commandTimeout) =>
				{
					connection.Execute(createEqualityStagingTableQuery, null, transaction, commandTimeout);
					TableFactory.BulkInsert(connection, objs, transaction, BulkStagingTable, EqualityColumns, EqualityProperties, commandTimeout,
						SqlBulkCopyOptions.KeepIdentity | SqlBulkCopyOptions.KeepNulls | SqlBulkCopyOptions.TableLock);
					int count = connection.Execute(bulkDeleteQuery, null, transaction, commandTimeout);
					connection.Execute(dropStagingTableQuery, null, transaction, commandTimeout);
					return count;
				};
			}

			///
			/// Inserts
			///
			string insertQuery = insertTableParams + valuesInserted;
			string insertNotExistsQuery = "IF NOT EXISTS (\nSELECT * FROM " + TableName + "\n" + whereEquals + ")\n" + insertTableParams + valuesInserted;
			if (InsertProperties.Count == 0) {
				// NoInsertsAttribute
				queries.Insert = DoNothingDbObjObj;
				queries.InsertIfNotExists = DoNothingDbObjObj;
				queries.BulkInsert = DoNothingSqlList;
				queries.BulkInsertIfNotExists = DoNothing;
			}
			else {
				queries.BulkInsert = (connection, objs, transaction, commandTimeout) =>
				{
					TableFactory.BulkInsert(connection, objs, transaction, TableName, InsertColumns, InsertProperties, commandTimeout,
						SqlBulkCopyOptions.KeepIdentity | SqlBulkCopyOptions.KeepNulls | SqlBulkCopyOptions.CheckConstraints | SqlBulkCopyOptions.FireTriggers | SqlBulkCopyOptions.TableLock);
				};
				queries.BulkInsertIfNotExists = (connection, objs, transaction, commandTimeout) =>
				{
					connection.Execute(createStagingTableQuery, null, transaction, commandTimeout);
					TableFactory.BulkInsert(connection, objs, transaction, BulkStagingTable, Columns, Properties, commandTimeout,
						SqlBulkCopyOptions.KeepIdentity | SqlBulkCopyOptions.KeepNulls | SqlBulkCopyOptions.TableLock);
					int count = connection.Execute(bulkInsertIfNotExistsQuery, null, transaction, commandTimeout);
					connection.Execute(dropStagingTableQuery, null, transaction, commandTimeout);
					return count;
				};
				if (AutoKeyProperty == null) {
					queries.Insert = (connection, obj, transaction, commandTimeout) =>
					{
						connection.Execute(insertQuery, obj, transaction, commandTimeout);
						return obj;
					};
					queries.InsertIfNotExists = (connection, obj, transaction, commandTimeout) =>
					{
						connection.Execute(insertNotExistsQuery, obj, transaction, commandTimeout);
						return obj;
					};
				}
				else {
					string insertSelectIdentityQuery = insertQuery + selectIdentityQuery;
					queries.Insert = (connection, obj, transaction, commandTimeout) =>
					{
						dynamic key = connection.QueryFirstOrDefault<dynamic>(insertSelectIdentityQuery, obj, transaction, commandTimeout);
						TableData<T>.SetKey(obj, (IDictionary<string, object>) key);
						return obj;
					};
					string insertNotExistsSelectIdentityQuery = insertNotExistsQuery + selectIdentityQuery;
					queries.InsertIfNotExists = (connection, obj, transaction, commandTimeout) =>
					{
						dynamic key = connection.QueryFirstOrDefault<dynamic>(insertNotExistsQuery, obj, transaction, commandTimeout);
						TableData<T>.SetKey(obj, (IDictionary<string, object>) key);
						return obj;
					};
				}
			}

			///
			/// Updates
			/// 
			string updateQuery = updateSetParams + whereUpdateEquals;
			string bulkUpdateQuery = "UPDATE " + TableName + "\nSET " + bulkUpdateSetParams + "\nFROM " + BulkStagingTable
				+ GetTempAndEquals(BulkStagingTable, UpdateEqualityColumns);
			if (UpdateProperties.Count == 0) {
				// NoUpdatesAttribute
				queries.Update = DoNothing;
				queries.BulkUpdate = DoNothing;
				queries.UpdateFilter = DoNothing;
			}
			else if (KeyProperties.Count == 0) {
				queries.Update = NoKeyProperty;
				queries.BulkUpdate = NoKeyProperty;
				queries.UpdateFilter = NoKeyProperty;
			}
			else {
				queries.Update = (connection, obj, transaction, commandTimeout) =>
				{
					int count = connection.Execute(updateQuery, obj, transaction, commandTimeout);
					return count > 0;
				};
				queries.UpdateFilter = (connection, obj, filter, transaction, commandTimeout) =>
				{
					int count;
					if (filter == null) {
						count = connection.Execute(updateQuery, obj, transaction, commandTimeout);
					}
					else {
						string setParams = LookupSetParams(filter);
						string query = "UPDATE " + TableName + setParams + whereUpdateEquals;
						count = connection.Execute(updateQuery, obj, transaction, commandTimeout);
					}
					return count > 0;
				};
				string createUpdateStagingTableQuery = dropStagingTableQuery + SelectIntoTableQuery(BulkStagingTable, BulkUpdateColumns);
				queries.BulkUpdate = (connection, objs, transaction, commandTimeout) =>
				{
					connection.Execute(createUpdateStagingTableQuery, null, transaction, commandTimeout);
					TableFactory.BulkInsert(connection, objs, transaction, BulkStagingTable, BulkUpdateColumns, BulkUpdateProperties, commandTimeout,
						SqlBulkCopyOptions.KeepIdentity | SqlBulkCopyOptions.KeepNulls | SqlBulkCopyOptions.TableLock);
					int count = connection.Execute(bulkUpdateQuery, null, transaction, commandTimeout);
					connection.Execute(dropStagingTableQuery, null, transaction, commandTimeout);
					return count;
				};
			}

			///
			/// Upsert
			///
			if (KeyProperties.Count == 0 || UpdateProperties.Count == 0) {
				if (InsertProperties.Count == 0) {
					queries.Upsert = DoNothingDbObjObj;
					queries.BulkUpsert = DoNothing;
				}
				else {
					// Insert if not exists
					queries.Upsert = queries.InsertIfNotExists;
					queries.BulkUpsert = queries.BulkInsertIfNotExists;
				}
			}
			else if (InsertProperties.Count == 0) {
				// Update only
				queries.Upsert = (connection, obj, transaction, commandTimeout) =>
				{
					int count = connection.Execute(updateQuery, obj, transaction, commandTimeout);
					return count > 1 ? obj : null;
				};
				queries.BulkUpsert = queries.BulkUpdate;
			}
			else {
				// Insert or Update
				string upsertQuery = "IF NOT EXISTS (\nSELECT * FROM " + TableName + "\n" + whereEquals + ")\n" + insertTableParams
					+ valuesInserted + "\n\nELSE\n\n" + updateSetParams + whereUpdateEquals;
				queries.Upsert = (connection, obj, transaction, commandTimeout) =>
				{
					dynamic key = connection.Query<dynamic>(upsertQuery, obj, transaction, true, commandTimeout).First();
					TableData<T>.SetKey(obj, (IDictionary<string, object>) key);
					return obj;
				};
				queries.BulkUpsert = (connection, objs, transaction, commandTimeout) =>
				{
					connection.Execute(createStagingTableQuery, null, transaction, commandTimeout);
					TableFactory.BulkInsert(connection, objs, transaction, BulkStagingTable, Columns, Properties, commandTimeout,
						SqlBulkCopyOptions.KeepIdentity | SqlBulkCopyOptions.KeepNulls | SqlBulkCopyOptions.TableLock);
					int countUpdate = connection.Execute(bulkUpdateQuery, null, transaction, commandTimeout);
					int countInsert = connection.Execute(bulkInsertIfNotExistsQuery, null, transaction, commandTimeout);
					connection.Execute(dropStagingTableQuery, null, transaction, commandTimeout);
					return countInsert;
				};
			}
			return queries;
		}

		public TableQueries<T, KeyType> Create<KeyType>()
		{
			if (KeyProperties.Count != 1)
				throw new InvalidOperationException(typeof(T).Name + " requires a single key");
			TableQueries<T, KeyType> queries = new TableQueries<T, KeyType>();
			queries.GetKeys = (connection, whereCondition, param, transaction, buffered, commandTimeout) =>
			{
				string query = selectKeysQuery + whereCondition;
				IEnumerable<KeyType> result = connection.Query<KeyType>(query, param, transaction, buffered, commandTimeout);
				return result;
			};
			string bulkDeleteQuery = deleteQuery + "WHERE " + KeyColumns[0] + " in @Keys";
			queries.BulkDelete = (connection, keys, transaction, commandTimeout) =>
			{
				int count = 0;
				foreach (IEnumerable<KeyType> Keys in Partition<KeyType>(keys.AsList(), 2000)) {
					int deleted = connection.Execute(bulkDeleteQuery, new { Keys }, transaction, commandTimeout);
					count += deleted;
				}
				return count;
			};
			queries.Get = (connection, key, transaction, commandTimeout) =>
			{
				T obj = connection.QuerySingleOrDefault<T>(selectSingleQuery, key, transaction, commandTimeout);
				return obj;
			};
			queries.Delete = (connection, key, transaction, commandTimeout) =>
			{
				int count = connection.Execute(deleteSingleQuery, key, transaction, commandTimeout);
				return count > 0;
			};
			return queries;
		}

		#region Utility Methods
		/// <summary>
		/// [Column("Name")] or the name of the properties.
		/// </summary>
		public string[] GetColumnNames(IEnumerable<PropertyInfo> properties)
		{
			string[] columns = properties.Select(p => EscapeIdentifier(TableFactory.GetColumnName(p))).ToArray();
			return columns;
		}

		public string EscapeIdentifier(string identifier)
		{
			if (identifier.Any(c => c == ' ' || c == quoteL || c == quoteR || c == '\t')) {
				return quoteL + identifier.Replace(quoteRstr, escapeQuoteR) + quoteR;
			}
			return identifier;
		}

		private string SelectIntoTableQuery(string tableName, IEnumerable<string> columns, string whereCondition = "WHERE 1=0")
		{
			string sql = $"\nSELECT {string.Join(",", columns)} INTO {tableName} FROM {TableName} " + whereCondition;
			return sql;
		}

		private static IEnumerable<List<Ty>> Partition<Ty>(IList<Ty> source, int size)
		{
			for (int i = 0; i < source.Count; i += size)
				yield return new List<Ty>(source.Skip(i).Take(size));
		}

		/// <summary>
		/// Combines columns for equality tests when copying from the bulk staging table into the actual table.
		/// WHERE #BulkTempTable_.[x] = TableName.[x] AND #BulkTempTable_.[y] = TableName.[y]
		/// </summary>
		private string GetTempAndEquals(string stagingTable, IReadOnlyList<string> columns)
		{
			if (columns.Count == 0)
				return "";
			StringBuilder sb = new StringBuilder(40 * columns.Count);
			sb.Append("\nWHERE \t");
			for (int i = 0; i < columns.Count; i++) {
				sb.AppendFormat("{1}.{0} = {2}.{0}\n\tAND", columns[i], TableName, stagingTable);
			}
			return sb.Remove(sb.Length - 5, 5).ToString();
		}

		/// <summary>
		/// x as [nameX], y, z as [nameZ]
		/// </summary>
		private string GetAsParams(IReadOnlyList<PropertyInfo> properties)
		{
			if (properties.Count == 0)
				return "";
			string[] columnNames = GetColumnNames(properties);
			StringBuilder sb = new StringBuilder(40 * properties.Count);
			for (int i = 0; i < properties.Count; i++) {
				sb.Append(columnNames[i]);
				if (properties[i].Name != columnNames[i]) {
					sb.Append(" as " + properties[i].Name);
				}
				sb.Append(',');
			}
			sb.Remove(sb.Length, 1);
			return sb.ToString();
		}

		/// <summary>
		/// Combines parameters for equality testing.<para></para>
		/// WHERE x = @x AND y = @y
		/// </summary>
		private string GetWhereEqualsParams(IReadOnlyList<PropertyInfo> properties, string joinString = " AND ")
		{
			if (properties.Count == 0)
				return "";
			string[] columnNames = GetColumnNames(properties);
			StringBuilder sb = new StringBuilder(40 * properties.Count + 7);
			sb.Append("WHERE \t");
			for (int i = 0; i < columnNames.Length; i++) {
				sb.Append($"{columnNames[i]} = @{properties[i].Name}\n\t{joinString}");
			}
			return sb.Remove(sb.Length - joinString.Length - 2, joinString.Length + 2).ToString();
		}

		/// <summary>
		/// Used for copying from the bulk staging table into the actual table.<para></para>
		/// TableName.[x] = #TableName_.[x], TableName.[y] = getdate()
		/// </summary>
		private string GetTempSetParams(string stagingTable, IReadOnlyList<string> columnNames, IReadOnlyList<IDefaultAttribute> defaultValues)
		{
			if (columnNames.Count == 0)
				return "";
			StringBuilder sb = new StringBuilder(40 * columnNames.Count);
			for (int i = 0; i < columnNames.Count; i++) {
				if (defaultValues[i] == null)
					sb.Append($"\t{TableName}.{columnNames[i]} = {stagingTable}.{columnNames[i]},\n");
				else if (defaultValues[i].HasValue)
					sb.Append($"\t{TableName}.{columnNames[i]} = {defaultValues[i].Value},\n");
			}
			if (sb.Length > 0)
				sb.Remove(sb.Length - 2, 2);
			return sb.ToString();
		}

		/// <summary>
		/// Used for joining parameters together for update commands in the SET section.<para></para>
		/// SET [x] = [x], [y] = getdate()
		/// </summary>
		private string GetSetParams(IReadOnlyList<string> columnNames, IReadOnlyList<PropertyInfo> properties, IReadOnlyList<IDefaultAttribute> defaultValues)
		{
			if (columnNames.Count == 0)
				return "";
			StringBuilder sb = new StringBuilder(40 * columnNames.Count);
			sb.Append("\nSET");
			for (int i = 0; i < columnNames.Count; i++) {
				if (defaultValues[i] == null)
					sb.Append($"\t{columnNames[i]} = @{properties[i].Name},\n");
				else if (defaultValues[i].HasValue)
					sb.Append($"\t{columnNames[i]} = {defaultValues[i].Value},\n");
			}
			if (sb.Length > 0)
				sb.Remove(sb.Length - 2, 1);
			return sb.ToString();
		}

		/// <summary>
		/// Used for joining parameters together for update commands in the VALUES section.<para></para>
		/// VALUES(@a,@b,@c)<para></para>
		/// </summary>
		private string GetValues(IReadOnlyList<PropertyInfo> properties, IReadOnlyList<IDefaultAttribute> defaultValues)
		{
			if (properties.Count == 0)
				return "";
			StringBuilder sb = new StringBuilder(20 * properties.Count);
			for (int i = 0; i < properties.Count; i++) {
				if (defaultValues[i] == null)
					sb.Append("@" + properties[i].Name + ",");
				else if (defaultValues[i].HasValue)
					sb.Append(defaultValues[i].Value + ",");
			}
			sb.Remove(sb.Length - 1, 1);
			return sb.ToString();
		}

		private string LookupSetParams(object obj)
		{
			Type ty = obj.GetType();
			if (SetParamsFilterMap.TryGetValue(ty, out string setParams))
				return setParams;
			if (!ty.IsClass)
				throw new InvalidOperationException("Filters can only be of type class");
			if (ty.BaseType != typeof(object) || ty.GetInterfaces().Any())
				throw new InvalidOperationException(ty.FullName + " is not a valid filter type. Filter objects should be anonymous classes or POCOs.");
			List<PropertyInfo> props = new List<PropertyInfo>(10);
			List<PropertyInfo> matches = new List<PropertyInfo>(10);
			foreach (PropertyInfo prop in ty.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanRead && TableFactory.ValidPropertyTypes.Contains(p.PropertyType))) {
				PropertyInfo match = Properties.FirstOrDefault(p => p.Name == prop.Name);
				if (match == null || UpdateEqualityProperties.Any(p => p.Name == prop.Name))
					continue;
				props.Add(prop);
				matches.Add(match);
			}
			if (!props.Any())
				throw new InvalidOperationException(ty.FullName + " must contain a non-key property with the same name as " + typeof(T).FullName);
			IDefaultAttribute[] defaults = matches.Select(prop => (IDefaultAttribute) prop.GetCustomAttribute<IgnoreUpdateAttribute>(true)
			   ?? prop.GetCustomAttribute<MatchUpdateAttribute>(true)).ToArray();
			string[] columns = GetColumnNames(props);
			setParams = GetSetParams(columns, props, defaults);
			SetParamsFilterMap.Add(ty, setParams);
			if (SetParamsFilterMap.Count > 75)
				SetParamsFilterMap.Clear(); // should be impossible to be this large
			return setParams;
		}

		#endregion Utility Methods

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
			string sql = "SELECT DISTINCT [" + string.Join(",", GetColumnNames(Properties.Where(prop => !KeyProperties.Contains(prop)).ToArray())) + "\nINTO " + BulkTempStagingTable + " FROM " + TableName;
			int currCount = RecordCount(connection, "", null, transaction, commandTimeout);
			connection.Execute(sql, null, transaction, commandTimeout);
			connection.Execute("TRUNCATE TABLE " + TableName, null, transaction, commandTimeout);
			int count = connection.Execute("INSERT INTO " + TableName + " SELECT * FROM " + BulkTempStagingTable, null, transaction, commandTimeout);
			connection.Execute("DROP TABLE " + BulkTempStagingTable, null, transaction, commandTimeout);
			return currCount - count;
		}

		private int RemoveDuplicatesKey_(IDbConnection connection, string keyColumn, IDbTransaction transaction = null, int? commandTimeout = null)
		{
			return connection.Execute(@"WHILE EXISTS (SELECT COUNT(*) FROM " + TableName + " GROUP BY " + keyColumn + " HAVING COUNT(*) > 1" + @")
BEGIN
	DELETE FROM " + TableName + @" WHERE " + keyColumn + @" IN 
	(
		SELECT MIN(" + keyColumn + @") as DeleteID
		" + " FROM " + TableName + " GROUP BY " + keyColumn + " HAVING COUNT(*) > 1" + @"
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
					sb.Append("(" + KeyColumns[0] + " = @p" + k);
					param.Add("@p" + k, KeyProperties[0].GetValue(obj));
					k++;
					for (int j = 1; j < KeyColumns.Length; j++) {
						sb.Append(" AND " + KeyColumns[j] + " = @p" + k);
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