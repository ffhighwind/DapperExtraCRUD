#region License
// Released under MIT License 
// License: https://opensource.org/licenses/MIT
// Home page: https://github.com/ffhighwind/DapperExtraCRUD

// Copyright(c) 2018 Wesley Hamilton

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
#endregion

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Fasterflect;
using Dapper.Extra.Internal;
using System.Linq.Expressions;

namespace Dapper.Extra
{
	/// <summary>
	/// Stores metadata and generates SQL commands for the given type.
	/// </summary>
	/// <typeparam name="T">The table type.</typeparam>
	public sealed class SqlBuilder<T> : ISqlBuilder
		where T : class
	{
		private readonly ConcurrentDictionary<Type, string> SelectMap = new ConcurrentDictionary<Type, string>();

		/// <summary>
		/// Stores strings to reduce memory usage.
		/// </summary>
		/// <remarks> String.Intern could be used to cache these strings, but this would prevent clearing the caches.</remarks>
		private readonly ConcurrentDictionary<string, string> StringCache = new ConcurrentDictionary<string, string>();

		private Tuple<Expression<Func<T, bool>>, Tuple<string, IDictionary<string, object>>> CachedWhereCondition;
		public Tuple<string, IDictionary<string, object>> CreateCachedWhereCondition(Expression<Func<T, bool>> whereExpr)
		{
			var temp = CachedWhereCondition;
			if (temp == null || temp.Item1 != whereExpr) {
				string whereCondition = "WHERE " + Utilities.WhereConditionGenerator.Create(whereExpr, out IDictionary<string, object> param);
				temp = Tuple.Create(whereExpr, Tuple.Create(whereCondition, param));
				CachedWhereCondition = temp;
			}
			return temp.Item2;
		}


		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlBuilder{T}"/> class.
		/// </summary>
		/// <param name="typeInfo">The information about the table.</param>
		/// <param name="threadSafety">The thread safety level to assign the lazy delegates.</param>
		public SqlBuilder(SqlTypeInfo typeInfo, LazyThreadSafetyMode threadSafety = LazyThreadSafetyMode.ExecutionAndPublication)
		{
			if (typeInfo.Type.IsGenericTypeDefinition && typeInfo.Type.GetGenericTypeDefinition() == typeof(List<>))
				throw new InvalidOperationException("List<> is not a valid table type.");
			if (typeInfo.Type.IsArray)
				throw new InvalidOperationException("Array<> is not a valid table type.");
			if (typeInfo.Type == typeof(string))
				throw new InvalidOperationException("String is not a valid table type.");
			Info = typeInfo;
			BulkStagingTable = Info.Adapter.CreateTempTableName(Info.Type.Name + (Math.Abs(Info.Type.FullName.GetHashCode()) % 99793));
			SqlQueries<T> queries = new SqlQueries<T>() {
				InsertAutoSync = CreateAutoSync(Info.InsertAutoSyncColumns),
				UpdateAutoSync = CreateAutoSync(typeInfo.UpdateAutoSyncColumns),
				LazyUpdateObj = new Lazy<DbObjBool<T>>(() => CreateUpdateObj()),
				LazyBulkDelete = new Lazy<DbListInt<T>>(() => CreateBulkDelete(), threadSafety),
				LazyBulkGet = new Lazy<DbListList<T>>(() => CreateBulkGet(), threadSafety),
				LazyBulkInsert = new Lazy<DbListVoid<T>>(() => CreateBulkInsert(), threadSafety),
				LazyBulkInsertIfNotExists = new Lazy<DbListInt<T>>(() => CreateBulkInsertIfNotExists(), threadSafety),
				LazyBulkUpdate = new Lazy<DbListInt<T>>(() => CreateBulkUpdate(), threadSafety),
				LazyBulkUpsert = new Lazy<DbListInt<T>>(() => CreateBulkUpsert(), threadSafety),
				LazyTruncate = new Lazy<DbVoid>(() => CreateTruncate(), threadSafety),
				LazyDeleteList = new Lazy<DbWhereInt<T>>(() => CreateDeleteList(), threadSafety),
				LazyGetDistinct = new Lazy<DbTypeWhereList<T>>(() => CreateGetDistinct(), threadSafety),
				LazyGetDistinctLimit = new Lazy<DbTypeLimitList<T>>(() => CreateGetDistinctLimit(), threadSafety),
				LazyGetKeys = new Lazy<DbWhereList<T>>(() => CreateGetKeys(), threadSafety),
				LazyGetLimit = new Lazy<DbLimitList<T>>(() => CreateGetLimit(), threadSafety),
				LazyInsertIfNotExists = new Lazy<DbTBool<T>>(() => CreateInsertIfNotExists(), threadSafety),
				LazyRecordCount = new Lazy<DbWhereInt<T>>(() => CreateRecordCount(), threadSafety),
				LazyUpsert = new Lazy<DbTBool<T>>(() => CreateUpsert(), threadSafety),
				LazyGetFilter = new Lazy<DbTypeWhereList<T>>(() => CreateGetFilterList(), threadSafety),
				LazyGetFilterLimit = new Lazy<DbTypeLimitList<T>>(() => CreateGetFilterLimit(), threadSafety),
			};
			Queries = queries;
			queries.Insert = CreateInsert();
			queries.Update = CreateUpdate();
			queries.Delete = CreateDelete();
			queries.Get = CreateGet();
			queries.GetList = CreateGetList();
			DataReaderFactory = new DataReaderFactory(typeof(T), typeInfo.Columns.Where(c => c.Getter != null).Select(c => c.Property));

			if (typeInfo.EqualityColumns.Count == 1) {
				if ((EqualityColumns[0].Type == typeof(string) && Adapter.StringComparer != StringComparer.Ordinal) || EqualityColumns[0].Type == typeof(byte[]))
					EqualityComparer = new TableEqualityComparer<T>(Info);
				else
					EqualityComparer = new TableKeyEqualityComparer<T>(TableName, EqualityColumns[0]);
				Type type = typeInfo.EqualityColumns[0].Type;
				TypeCode typeCode = Type.GetTypeCode(type);
				switch (typeCode) {
					case TypeCode.Int16:
						Create<short>(threadSafety);
						break;
					case TypeCode.Int32:
						Create<int>(threadSafety);
						break;
					case TypeCode.Int64:
						Create<long>(threadSafety);
						break;
					case TypeCode.SByte:
						Create<sbyte>(threadSafety);
						break;
					case TypeCode.Single:
						Create<float>(threadSafety);
						break;
					case TypeCode.String:
						Create<string>(threadSafety);
						break;
					case TypeCode.UInt16:
						Create<ushort>(threadSafety);
						break;
					case TypeCode.Double:
						Create<double>(threadSafety);
						break;
					case TypeCode.UInt32:
						Create<uint>(threadSafety);
						break;
					case TypeCode.UInt64:
						Create<ulong>(threadSafety);
						break;
					case TypeCode.Byte:
						Create<byte>(threadSafety);
						break;
					case TypeCode.Char:
						Create<char>(threadSafety);
						break;
					case TypeCode.DateTime:
						Create<DateTime>(threadSafety);
						break;
					case TypeCode.Decimal:
						Create<decimal>(threadSafety);
						break;
					default:
						if (type == typeof(Guid))
							Create<Guid>(threadSafety);
						else if (type == typeof(DateTimeOffset))
							Create<DateTimeOffset>(threadSafety);
						else if (type == typeof(TimeSpan))
							Create<TimeSpan>(threadSafety);
						else if (type == typeof(byte[]))
							Create<byte[]>(threadSafety);
						else
							Create("Unsupported SQL key type: " + type, threadSafety);
						break;
				}
			}
			else {
				EqualityComparer = new TableEqualityComparer<T>(Info);
				Create("Composite key does not support this operation", threadSafety);
			}
			string paramsSelect = ParamsSelect(Info.SelectColumns);
			SelectMap.GetOrAdd(typeof(T), paramsSelect);
		}

		#endregion

		#region Properties

		/// <summary>
		///  All valid columns for the given type.
		/// </summary>
		public IReadOnlyList<SqlColumn> Columns => Info.Columns;

		/// <summary>
		/// Creates an object from a single value key. This can be used by a dictionary where <typeparamref name="T"/> is the key.
		/// </summary>
		public Func<object, T> ObjectFromKey { get; private set; }

		/// <summary>
		/// Generates <see cref="DbDataReader"/> for this type.
		/// </summary>
		public DataReaderFactory DataReaderFactory { get; private set; }

		/// <summary>
		/// The columns that determine uniqueness. This is every column if there are no keys.
		/// </summary>
		public IReadOnlyList<SqlColumn> EqualityColumns => Info.EqualityColumns;

		/// <summary>
		/// Compares two objects of the given type and determines if they are equal.
		/// </summary>
		public IEqualityComparer<T> EqualityComparer { get; private set; }

		/// <summary>
		/// Stores metadata for for the given type.
		/// </summary>
		public SqlTypeInfo Info { get; private set; }

		/// <summary>
		/// The queries and commands for this type.
		/// </summary>
		public ISqlQueries<T> Queries { get; private set; }

		/// <summary>
		/// The dialect used to generate SQL commands.
		/// </summary>
		public SqlDialect Dialect => Info.Dialect;

		/// <summary>
		/// The quoted table name or the class name.
		/// </summary>
		public string TableName => Info.TableName;

		/// <summary>
		/// The temporary table name for bulk operations.
		/// </summary>
		private ISqlAdapter Adapter => Info.Adapter;

		/// <summary>
		/// The temporary table name for bulk operations.
		/// </summary>
		private string BulkStagingTable { get; set; }

		#endregion

		#region Methods


		/// <summary>
		/// Gets the subset of columns that match the property names of <paramref name="type"/>.
		/// </summary>
		/// <param name="type">The type whose properties to match.</param>
		/// <param name="columns">A list of columns from <see cref="Info"/>. </param>
		/// <returns>The subset of columns that match the property names of <paramref name="type"/>.</returns>
		public IEnumerable<SqlColumn> GetSharedColumns(Type type, IEnumerable<SqlColumn> columns)
		{
			if (type == typeof(T))
				return columns;
			Dictionary<string, SqlColumn> map = new Dictionary<string, SqlColumn>();
			foreach (SqlColumn column in columns) {
				map.Add(column.Property.Name, column);
			}
			List<SqlColumn> list = new List<SqlColumn>();
			foreach (PropertyInfo prop in type.GetProperties(BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(p => p.CanWrite)) {
				if (map.TryGetValue(prop.Name, out SqlColumn column) && column.Type.IsAssignableFrom(prop.PropertyType)) {
					list.Add(column);
				}
			}
			if (list.Count == 0)
				throw new InvalidOperationException(type.FullName + " does not have any matching columns with " + typeof(T).FullName);
			return list;
		}


		/// <summary>
		/// Creates a function that syncs the given columns.
		/// </summary>
		/// <param name="columns">The columns that will be queried and updated.</param>
		/// <returns>An auto-sync query.</returns>
		public DbTVoid<T> CreateAutoSync(IEnumerable<SqlColumn> columns)
		{
			if (!columns.Any())
				return null;
			string paramsSelect = ParamsSelect(columns);
			string whereEquals = WhereEquals(EqualityColumns);
			MemberSetter[] setters = columns.Select(c => c.Setter).ToArray();
			string[] names = columns.Select(c => c.Property.Name).ToArray();
			return (connection, obj, transaction, commandTimeout) => {
				string cmd = $"SELECT {paramsSelect}\nFROM {TableName}\nWHERE \t{whereEquals}";
				IDictionary<string, object> value;
				try {
					value = connection.QueryFirstOrDefault(cmd, obj, transaction, commandTimeout);
				}
				catch (Exception ex) {
					throw new InvalidOperationException(ex.Message + "\n" + cmd);
				}
				if (value != null) {
					for (int i = 0; i < setters.Length; i++) {
						setters[i](obj, value[names[i]]);
					}
				}
			};
		}

		internal string ColumnNames(IEnumerable<SqlColumn> columns)
		{
			return Store(string.Join(",", columns.Select(c => c.ColumnName)));
		}

		/// <summary>
		/// Creates single key queries.
		/// </summary>
		/// <typeparam name="KeyType">The key type.</typeparam>
		/// <param name="threadSafety">Determines the thread safe access for the</param>
		internal void Create<KeyType>(LazyThreadSafetyMode threadSafety)
		{
			SqlQueries<T> queries = (SqlQueries<T>) Queries;
			MemberSetter setter = EqualityColumns[0].Setter;
			ObjectFromKey = (key) => {
				object result = System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(T));
				setter(result, key);
				return (T) result;
			};
			queries.LazyBulkDeleteKeys = new Lazy<DbKeysInt<T>>(() => CreateBulkDeleteKeys<KeyType>(), threadSafety);
			queries.LazyBulkGetKeys = new Lazy<DbKeysList<T>>(() => CreateBulkGetKeys<KeyType>(), threadSafety);
			queries.LazyGetKeysKeys = new Lazy<DbWhereKeys>(() => CreateGetKeysKeys<KeyType>(), threadSafety);
			queries.GetKey = CreateGetKey();
			queries.DeleteKey = CreateDeleteKey();
		}

		internal void Create(string msg, LazyThreadSafetyMode threadSafety)
		{
			SqlQueries<T> queries = (SqlQueries<T>) Queries;
			queries.LazyBulkDeleteKeys = new Lazy<DbKeysInt<T>>(() => throw new InvalidOperationException(msg), threadSafety);
			queries.LazyBulkGetKeys = new Lazy<DbKeysList<T>>(() => throw new InvalidOperationException(msg), threadSafety);
			queries.LazyGetKeysKeys = new Lazy<DbWhereKeys>(() => throw new InvalidOperationException(msg), threadSafety);
			queries.GetKey = (connection, key, transaction, commandTimeout) => throw new InvalidOperationException(msg);
			queries.DeleteKey = (connection, key, transaction, commandTimeout) => throw new InvalidOperationException(msg);
		}

		internal string DeleteCmd()
		{
			return Store("DELETE FROM " + TableName + "\n");
		}

		internal string DropBulkTableCmd()
		{
			return Store(Info.Adapter.DropTempTableIfExists(BulkStagingTable));
		}

		internal string InsertedValues(IEnumerable<SqlColumn> columns)
		{
			return Store(SqlBuilderHelper.InsertedValues(columns));
		}

		internal string InsertIntoCmd()
		{
			return Store($"INSERT INTO {TableName} ({ColumnNames(Info.InsertColumns)})\n");
		}

		internal string ParamsSelect()
		{
			return ParamsSelect(Info.SelectColumns);
		}

		internal string ParamsSelect(IEnumerable<SqlColumn> columns)
		{
			return Store(SqlBuilderHelper.SelectedColumns(columns));
		}

		internal string ParamsSelectFromTable()
		{
			return Store(SqlBuilderHelper.SelectedColumns(Info.SelectColumns) + "\nFROM " + TableName + "\n");
		}

		internal string ParamsSelectFromTableBulk()
		{
			return Store(SqlBuilderHelper.SelectedColumns(Info.SelectColumns, TableName) + "\nFROM " + TableName + "\n");
		}

		internal string SelectAutoKey()
		{
			return Store(Info.Adapter.SelectIdentityQuery(Info.AutoKeyColumn.Type));
		}

		internal string SelectIntoStagingTable(IEnumerable<SqlColumn> columns)
		{
			return Store(SqlBuilderHelper.SelectIntoTableQuery(TableName, BulkStagingTable, columns));
		}

		/// <summary>
		/// Checks the cache for a string and returns it if it exists, otherwise it adds the string to the cache.
		/// </summary>
		/// <param name="str">The string to add/search for.</param>
		/// <remarks> String.Intern could be used to cache these strings, but this would prevent clearing the caches.</remarks>
		internal string Store(string str)
		{
			return StringCache.GetOrAdd(str, str);
		}

		internal string TruncateTableQuery()
		{
			return Store(Info.Adapter.TruncateTable(TableName));
		}

		internal string UpdateSet(IEnumerable<SqlColumn> columns)
		{
			return Store(SqlBuilderHelper.UpdateSet(columns));
		}

		internal string UpdateSetTables()
		{
			// "\nSET \t" + Params
			return Store(SqlBuilderHelper.UpdateSetTables(BulkStagingTable, TableName, Info.UpdateColumns));
		}

		internal string WhereEquals(IEnumerable<SqlColumn> columns)
		{
			return Store(SqlBuilderHelper.WhereEquals(columns));
		}

		internal string WhereEqualsTables(IEnumerable<SqlColumn> columns)
		{
			return Store(SqlBuilderHelper.WhereEqualsTables(BulkStagingTable, TableName, columns));
		}

		private static void DoNothing(IDbConnection connection, IDbTransaction transaction, int commandTimeout)
		{
		}

		private static int DoNothing(IDbConnection connection, IEnumerable<T> objs, IDbTransaction transaction, int commandTimeout)
		{
			return 0;
		}

		private static bool DoNothing(IDbConnection connection, object obj, IDbTransaction transaction, int commandTimeout)
		{
			return false;
		}

		private static int DoNothing(IDbConnection connection, string whereCondition, object param, IDbTransaction transaction, int commandTimeout)
		{
			return 0;
		}

		private static bool DoNothing(IDbConnection connection, T obj, IDbTransaction transaction, int commandTimeout)
		{
			return false;
		}

		private static void DoNothingList(IDbConnection connection, IEnumerable<T> objs, IDbTransaction transaction, int commandTimeout)
		{
		}

		private static void DoNothingVoid(IDbConnection connection, T obj, IDbTransaction transaction, int commandTimeout)
		{
		}

		private DbListInt<T> CreateBulkDelete()
		{
			if (Info.DeleteKeyColumns.Count == 0) // NoDeletesAttribute
				return DoNothing;
			string dropBulkTableCmd = DropBulkTableCmd();
			string selectEqualityIntoStagingCmd = SelectIntoStagingTable(EqualityColumns);
			string equalsTables = WhereEqualsTables(EqualityColumns);
			return (connection, objs, transaction, commandTimeout) => {
				bool wasClosed = connection.State != ConnectionState.Open;
				if (wasClosed)
					connection.Open();
				// Drop Staging Table
				try {
					connection.Execute(dropBulkTableCmd, null, transaction, commandTimeout);
				}
				catch (Exception ex) {
					throw new InvalidOperationException(ex.Message + "\n" + dropBulkTableCmd);
				}
				// Create Staging Table
				try {
					connection.Execute(selectEqualityIntoStagingCmd, null, transaction, commandTimeout);
				}
				catch (Exception ex) {
					throw new InvalidOperationException(ex.Message + "\n" + selectEqualityIntoStagingCmd);
				}
				// Copy to Staging Table
				Adapter.BulkInsert(connection, objs, transaction, BulkStagingTable, DataReaderFactory, EqualityColumns, commandTimeout,
					SqlBulkCopyOptions.KeepIdentity | SqlBulkCopyOptions.KeepNulls | SqlBulkCopyOptions.TableLock);
				// Bulk Delete
				string bulkDeleteCmd = $"DELETE FROM {TableName} FROM {TableName} INNER JOIN {BulkStagingTable} ON {equalsTables}";
				int count;
				try {
					count = connection.Execute(bulkDeleteCmd, null, transaction, commandTimeout);
				}
				catch (Exception ex) {
					throw new InvalidOperationException(ex.Message + "\n" + bulkDeleteCmd);
				}
				// Drop Staging Table
				try {
					connection.Execute(dropBulkTableCmd, null, transaction, commandTimeout);
				}
				catch (Exception ex) {
					throw new InvalidOperationException(ex.Message + "\n" + dropBulkTableCmd);
				}
				if (wasClosed)
					connection.Close();
				return count;
			};
		}

		private DbKeysInt<T> CreateBulkDeleteKeys<KeyType>()
		{
			if (Info.DeleteKeyColumns.Count == 0) {
				//	NoDeletesAttribute
				return (connection, keys, transaction, commandTimeout) => {
					return 0;
				};
			}
			string deleteCmd = DeleteCmd();
			string keyName = EqualityColumns[0].ColumnName;
			return (connection, keys, transaction, commandTimeout) => {
				int count = 0;
				string bulkDeleteCmd = $"{deleteCmd}WHERE \t{keyName} in @Keys";
				foreach (IEnumerable<KeyType> Keys in ExtraUtil.Partition(keys.Select(k => (KeyType) k), 2000)) {
					try {
						int deleted = connection.Execute(bulkDeleteCmd, new { Keys }, transaction, commandTimeout);
						count += deleted;
					}
					catch (Exception ex) {
						throw new InvalidOperationException(ex.Message + "\n" + bulkDeleteCmd);
					}
				}
				return count;
			};
		}

		private DbListList<T> CreateBulkGet()
		{
			string dropBulkTableCmd = DropBulkTableCmd();
			string selectEqualityIntoStagingCmd = SelectIntoStagingTable(EqualityColumns);
			string paramsSelectFromTableBulk = ParamsSelectFromTableBulk();
			string equalsTables = WhereEqualsTables(EqualityColumns);
			return (connection, objs, transaction, commandTimeout) => {
				bool wasClosed = connection.State != ConnectionState.Open;
				if (wasClosed)
					connection.Open();
				// Drop Staging Table
				try {
					connection.Execute(dropBulkTableCmd, null, transaction, commandTimeout);
				}
				catch (Exception ex) {
					throw new InvalidOperationException(ex.Message + "\n" + dropBulkTableCmd);
				}
				// Create Staging Table
				try {
					connection.Execute(selectEqualityIntoStagingCmd, null, transaction, commandTimeout);
				}
				catch (Exception ex) {
					throw new InvalidOperationException(ex.Message + "\n" + selectEqualityIntoStagingCmd);
				}
				// Copy to Staging Table
				Adapter.BulkInsert(connection, objs, transaction, BulkStagingTable, DataReaderFactory, EqualityColumns, commandTimeout,
					SqlBulkCopyOptions.KeepIdentity | SqlBulkCopyOptions.KeepNulls | SqlBulkCopyOptions.TableLock);
				// Bulk Get
				string bulkGetQuery = $"SELECT {paramsSelectFromTableBulk}\tINNER JOIN {BulkStagingTable} ON {equalsTables}";
				List<T> result;
				try {
					result = connection.Query<T>(bulkGetQuery, null, transaction, true, commandTimeout).AsList();
				}
				catch (Exception ex) {
					throw new InvalidOperationException(ex.Message + "\n" + bulkGetQuery);
				}
				// Drop Staging Table
				try {
					connection.Execute(dropBulkTableCmd, null, transaction, commandTimeout);
				}
				catch (Exception ex) {
					throw new InvalidOperationException(ex.Message + "\n" + dropBulkTableCmd);
				}
				if (wasClosed)
					connection.Close();
				return result;
			};
		}

		private DbKeysList<T> CreateBulkGetKeys<KeyType>()
		{
			string paramsSelectFromTableBulk = ParamsSelectFromTableBulk();
			string keyName = EqualityColumns[0].ColumnName;
			return (connection, keys, transaction, commandTimeout) => {
				List<T> result = new List<T>();
				string bulkGetKeysQuery = $"SELECT {paramsSelectFromTableBulk}WHERE \t{keyName} in @Keys";
				foreach (IEnumerable<KeyType> Keys in ExtraUtil.Partition(keys.Select(k => (KeyType) k), 2000)) {
					try {
						IEnumerable<T> list = connection.Query<T>(bulkGetKeysQuery, new { Keys }, transaction, true, commandTimeout);
						result.AddRange(list);
					}
					catch (Exception ex) {
						throw new InvalidOperationException(ex.Message + "\n" + bulkGetKeysQuery);
					}
				}
				return result;
			};
		}

		private DbListVoid<T> CreateBulkInsert()
		{
			if (!Info.InsertColumns.Any())
				return DoNothingList;
			return (connection, objs, transaction, commandTimeout) => {
				Adapter.BulkInsert(connection, objs, transaction, TableName, DataReaderFactory, Info.InsertColumns, commandTimeout,
					SqlBulkCopyOptions.KeepIdentity | SqlBulkCopyOptions.KeepNulls | SqlBulkCopyOptions.CheckConstraints | SqlBulkCopyOptions.FireTriggers | SqlBulkCopyOptions.TableLock);
			};
		}

		private DbListInt<T> CreateBulkInsertIfNotExists()
		{
			if (!Info.InsertColumns.Any())
				return DoNothing;
			string selectInsertIntoStagingCmd = SelectIntoStagingTable(Info.BulkInsertIfNotExistsColumns);
			string dropBulkTableCmd = DropBulkTableCmd();
			string equalsTables = WhereEqualsTables(EqualityColumns);
			string insertColumns = ColumnNames(Info.InsertColumns);
			string insertIntoCmd = InsertIntoCmd();
			string insertedValues = InsertedValues(Info.InsertColumns);
			return (connection, objs, transaction, commandTimeout) => {
				bool wasClosed = connection.State != ConnectionState.Open;
				if (wasClosed)
					connection.Open();
				// Drop Staging Table
				try {
					connection.Execute(dropBulkTableCmd, null, transaction, commandTimeout);
				}
				catch (Exception ex) {
					throw new InvalidOperationException(ex.Message + "\n" + dropBulkTableCmd);
				}
				// Create Staging Table
				try {
					connection.Execute(selectInsertIntoStagingCmd, null, transaction, commandTimeout);
				}
				catch (Exception ex) {
					throw new InvalidOperationException(ex.Message + "\n" + selectInsertIntoStagingCmd);
				}
				// Copy to Staging Table
				Adapter.BulkInsert(connection, objs, transaction, BulkStagingTable, DataReaderFactory, Info.BulkInsertIfNotExistsColumns, commandTimeout,
					SqlBulkCopyOptions.KeepIdentity | SqlBulkCopyOptions.KeepNulls | SqlBulkCopyOptions.TableLock);
				// Bulk InsertIfNotExists
				string bulkInsertIfNotExistsCmd = $"{insertIntoCmd}\nSELECT {insertColumns}\nFROM {BulkStagingTable}\nWHERE NOT EXISTS (\nSELECT * FROM {TableName}\nWHERE \t{equalsTables})";
				int count;
				try {
					count = connection.Execute(bulkInsertIfNotExistsCmd, null, transaction, commandTimeout);
				}
				catch (Exception ex) {
					throw new InvalidOperationException(ex.Message + "\n" + bulkInsertIfNotExistsCmd);
				}
				// Drop Staging Table
				try {
					connection.Execute(dropBulkTableCmd, null, transaction, commandTimeout);
				}
				catch (Exception ex) {
					throw new InvalidOperationException(ex.Message + "\n" + dropBulkTableCmd);
				}
				if (wasClosed)
					connection.Close();
				return count;
			};
		}

		private DbListInt<T> CreateBulkUpdate()
		{
			if (Info.UpdateKeyColumns.Count == 0) // NoUpdatesAttribute
				return DoNothing;
			string dropBulkTableCmd = DropBulkTableCmd();
			string bulkUpdateSetParams = UpdateSetTables();
			string selectEqualityIntoStagingCmd = SelectIntoStagingTable(Info.BulkUpdateColumns);
			string updateEquals = WhereEqualsTables(Info.UpdateKeyColumns);
			return (connection, objs, transaction, commandTimeout) => {
				bool wasClosed = connection.State != ConnectionState.Open;
				if (wasClosed)
					connection.Open();
				// Drop Staging Table
				try {
					connection.Execute(dropBulkTableCmd, null, transaction, commandTimeout);
				}
				catch (Exception ex) {
					throw new InvalidOperationException(ex.Message + "\n" + dropBulkTableCmd);
				}
				// Create Staging Table
				try {
					connection.Execute(selectEqualityIntoStagingCmd, null, transaction, commandTimeout);
				}
				catch (Exception ex) {
					throw new InvalidOperationException(ex.Message + "\n" + selectEqualityIntoStagingCmd);
				}
				// Copy to Staging Table
				Adapter.BulkInsert(connection, objs, transaction, BulkStagingTable, DataReaderFactory, Info.BulkUpdateColumns, commandTimeout,
					SqlBulkCopyOptions.KeepIdentity | SqlBulkCopyOptions.KeepNulls | SqlBulkCopyOptions.TableLock);
				string bulkUpdateCmd = $"UPDATE {TableName}{bulkUpdateSetParams}\nFROM {BulkStagingTable}\nWHERE \t{updateEquals}";
				// Bulk Update
				int count;
				try {
					count = connection.Execute(bulkUpdateCmd, null, transaction, commandTimeout);
				}
				catch (Exception ex) {
					throw new InvalidOperationException(ex.Message + "\n" + bulkUpdateCmd);
				}
				// Drop Staging Table
				try {
					connection.Execute(dropBulkTableCmd, null, transaction, commandTimeout);
				}
				catch (Exception ex) {
					throw new InvalidOperationException(ex.Message + "\n" + dropBulkTableCmd);
				}
				if (wasClosed)
					connection.Close();
				return count;
			};
		}

		private DbListInt<T> CreateBulkUpsert()
		{
			if (!Info.UpdateColumns.Any()) {
				if (!Info.InsertColumns.Any())
					return DoNothing;
				// Insert if not exists
				return Queries.BulkInsertIfNotExists;
			}
			DbListInt<T> bulkUpdate = Queries.BulkUpdate;
			if (!Info.InsertColumns.Any()) {
				// Update only
				return (connection, objs, transaction, commandTimeout) => {
					int count = bulkUpdate(connection, objs, transaction, commandTimeout);
					return 0;
				};
			}
			string dropBulkTableCmd = DropBulkTableCmd();
			// Insert or Update
			string bulkUpdateSetParams = UpdateSetTables();
			string updateEquals = WhereEqualsTables(Info.UpdateKeyColumns);
			string selectUpsertIntoStagingCmd = SelectIntoStagingTable(Info.UpsertColumns);
			string equalsTables = WhereEqualsTables(EqualityColumns);
			string insertIntoCmd = InsertIntoCmd();
			string insertColumns = ColumnNames(Info.InsertColumns);
			return (connection, objs, transaction, commandTimeout) => {
				bool wasClosed = connection.State != ConnectionState.Open;
				if (wasClosed)
					connection.Open();
				// Drop Staging Table
				try {
					connection.Execute(dropBulkTableCmd, null, transaction, commandTimeout);
				}
				catch (Exception ex) {
					throw new InvalidOperationException(ex.Message + "\n" + dropBulkTableCmd);
				}
				// Create Staging Table
				try {
					connection.Execute(selectUpsertIntoStagingCmd, null, transaction, commandTimeout);
				}
				catch (Exception ex) {
					throw new InvalidOperationException(ex.Message + "\n" + selectUpsertIntoStagingCmd);
				}
				// Copy to Staging Table
				Adapter.BulkInsert(connection, objs, transaction, BulkStagingTable, DataReaderFactory, Info.UpsertColumns, commandTimeout,
					SqlBulkCopyOptions.KeepIdentity | SqlBulkCopyOptions.KeepNulls | SqlBulkCopyOptions.TableLock);
				// Bulk Update
				int countUpdate;
				string bulkUpdateCmd = $"UPDATE {TableName}{bulkUpdateSetParams}\nFROM {BulkStagingTable}\nWHERE \t{updateEquals}";
				try {
					countUpdate = connection.Execute(bulkUpdateCmd, null, transaction, commandTimeout);
				}
				catch (Exception ex) {
					throw new InvalidOperationException(ex.Message + "\n" + bulkUpdateCmd);
				}
				// Bulk InsertIfNotExists
				string bulkInsertIfNotExistsCmd = $"{insertIntoCmd}\nSELECT {insertColumns}\nFROM {BulkStagingTable}\nWHERE NOT EXISTS (\nSELECT * FROM {TableName}\nWHERE \t{equalsTables})";
				int countInsert;
				try {
					countInsert = connection.Execute(bulkInsertIfNotExistsCmd, null, transaction, commandTimeout);
				}
				catch (Exception ex) {
					throw new InvalidOperationException(ex.Message + "\n" + bulkInsertIfNotExistsCmd);
				}
				// Drop Staging Table
				try {
					connection.Execute(dropBulkTableCmd, null, transaction, commandTimeout);
				}
				catch (Exception ex) {
					throw new InvalidOperationException(ex.Message + "\n" + dropBulkTableCmd);
				}
				if (wasClosed)
					connection.Close();
				return countInsert;
			};
		}

		private DbTBool<T> CreateDelete()
		{
			if (Info.DeleteKeyColumns.Count == 0) // NoDeletesAttribute
				return DoNothing;
			string deleteCmd = DeleteCmd();
			string deleteEquals = WhereEquals(Info.DeleteKeyColumns);
			return (connection, obj, transaction, commandTimeout) => {
				string cmd = deleteCmd + "WHERE \t" + deleteEquals;
				try {
					int count = connection.Execute(cmd, obj, transaction, commandTimeout);
					return count > 0;
				}
				catch (Exception ex) {
					throw new InvalidOperationException(ex.Message + "\n" + cmd);
				}
			};
		}

		private DbKeyBool CreateDeleteKey()
		{
			if (Info.DeleteKeyColumns.Count == 0) {
				//	NoDeletesAttribute
				return (connection, obj, transaction, commandTimeout) => {
					return false;
				};
			}
			string deleteCmd = DeleteCmd();
			string deleteEquals = WhereEquals(Info.DeleteKeyColumns);
			string keyName = EqualityColumns[0].Property.Name;
			return (connection, key, transaction, commandTimeout) => {
				string cmd = $"{deleteCmd}WHERE \t{deleteEquals}";
				IDictionary<string, object> obj = new ExpandoObject();
				obj.Add(keyName, key);
				try {
					int count = connection.Execute(cmd, obj, transaction, commandTimeout);
					return count > 0;
				}
				catch (Exception ex) {
					throw new InvalidOperationException(ex.Message + "\n" + cmd);
				}
			};
		}

		private DbWhereInt<T> CreateDeleteList()
		{
			if (Info.DeleteKeyColumns.Count == 0) // NoDeletesAttribute
				return DoNothing;
			string deleteCmd = DeleteCmd();
			return (connection, whereCondition, param, transaction, commandTimeout) => {
				string cmd = deleteCmd + whereCondition;
				try {
					int count = connection.Execute(cmd, param, transaction, commandTimeout);
					return count;
				}
				catch (Exception ex) {
					throw new InvalidOperationException(ex.Message + "\n" + cmd);
				}
			};
		}

		private DbTT<T> CreateGet()
		{
			string paramsSelectFromTable = ParamsSelectFromTable();
			string whereEquals = WhereEquals(EqualityColumns);
			return (connection, obj, transaction, commandTimeout) => {
				string query = $"SELECT {paramsSelectFromTable}WHERE \t{whereEquals}";
				try {
					T val = connection.QueryFirstOrDefault<T>(query, obj, transaction, commandTimeout);
					return val;
				}
				catch (Exception ex) {
					throw new InvalidOperationException(ex.Message + "\n" + query);
				}
			};
		}

		private DbTypeWhereList<T> CreateGetDistinct()
		{
			return (connection, type, whereCondition, param, transaction, buffered, commandTimeout) => {
				if (!SelectMap.TryGetValue(type, out string paramsSelect)) {
					paramsSelect = CreateParamsSelect(type);
				}
				string query = $"SELECT DISTINCT {paramsSelect}\nFROM {TableName}\n{whereCondition}";
				try {
					IEnumerable<T> result = connection.Query<T>(query, param, transaction, buffered, commandTimeout);
					return result;
				}
				catch (Exception ex) {
					throw new InvalidOperationException(ex.Message + "\n" + query);
				}
			};
		}

		private DbTypeLimitList<T> CreateGetDistinctLimit()
		{
			string limitQuery = Adapter.LimitQuery;
			return (connection, limit, type, whereCondition, param, transaction, buffered, commandTimeout) => {
				if (!SelectMap.TryGetValue(type, out string paramsSelect)) {
					paramsSelect = CreateParamsSelect(type);
				}
				string subQuery = $"{paramsSelect}\nFROM {TableName}\n{whereCondition}";
				string query = "SELECT " + string.Format(limitQuery, limit, subQuery);
				try {
					IEnumerable<T> result = connection.Query<T>(query, param, transaction, false, commandTimeout);
					return result;
				}
				catch (Exception ex) {
					throw new InvalidOperationException(ex.Message + "\n" + query);
				}
			};
		}

		private DbTypeLimitList<T> CreateGetFilterLimit()
		{
			string limitQuery = Info.Adapter.LimitQuery;
			return (connection, limit, type, whereCondition, param, transaction, buffered, commandTimeout) => {
				if (!SelectMap.TryGetValue(type, out string paramsSelect)) {
					paramsSelect = CreateParamsSelect(type);
				}
				string subQuery = $"{paramsSelect}\nFROM {TableName}\n{whereCondition}";
				string query = "SELECT " + string.Format(limitQuery, limit, subQuery);
				try {
					IEnumerable<T> result = connection.Query<T>(query, param, transaction, false, commandTimeout);
					return result;
				}
				catch (Exception ex) {
					throw new InvalidOperationException(ex.Message + "\n" + query);
				}
			};
		}

		private DbTypeWhereList<T> CreateGetFilterList()
		{
			return (connection, type, whereCondition, param, transaction, buffered, commandTimeout) => {
				if (!SelectMap.TryGetValue(type, out string paramsSelect)) {
					paramsSelect = CreateParamsSelect(type);
				}
				string query = $"SELECT {paramsSelect}\nFROM {TableName}\n{whereCondition}";
				try {
					IEnumerable<T> result = connection.Query<T>(query, param, transaction, buffered, commandTimeout);
					return result;
				}
				catch (Exception ex) {
					throw new InvalidOperationException(ex.Message + "\n" + query);
				}
			};
		}

		private DbKeyObj<T> CreateGetKey()
		{
			string paramsSelectFromTable = ParamsSelectFromTable();
			string whereEquals = WhereEquals(EqualityColumns);
			string keyName = EqualityColumns[0].Property.Name;
			return (connection, key, transaction, commandTimeout) => {
				string query = $"SELECT {paramsSelectFromTable}WHERE \t{whereEquals}";
				IDictionary<string, object> obj = new ExpandoObject();
				obj.Add(keyName, key);
				try {
					T val = connection.QueryFirstOrDefault<T>(query, obj, transaction, commandTimeout);
					return val;
				}
				catch (Exception ex) {
					throw new InvalidOperationException(ex.Message + "\n" + query);
				}
			};
		}

		private DbWhereList<T> CreateGetKeys()
		{
			string paramsSelectKeys = ParamsSelect(EqualityColumns);
			return (connection, whereCondition, param, transaction, buffered, commandTimeout) => {
				string query = $"SELECT {paramsSelectKeys}\nFROM {TableName}\n{whereCondition}";
				try {
					IEnumerable<T> result = connection.Query<T>(query, param, transaction, buffered, commandTimeout);
					return result;
				}
				catch (Exception ex) {
					throw new InvalidOperationException(ex.Message + "\n" + query);
				}
			};
		}

		private DbWhereKeys CreateGetKeysKeys<KeyType>()
		{
			string paramsSelectKeys = ParamsSelect(EqualityColumns);
			return (connection, whereCondition, param, transaction, buffered, commandTimeout) => {
				string query = $"SELECT {paramsSelectKeys}\nFROM {TableName}\n{whereCondition}";
				try {
					IEnumerable<KeyType> result = connection.Query<KeyType>(query, param, transaction, buffered, commandTimeout);
					return result.Select(r => (object) r);
				}
				catch (Exception ex) {
					throw new InvalidOperationException(ex.Message + "\n" + query);
				}
			};
		}

		private DbLimitList<T> CreateGetLimit()
		{
			string paramsSelectFromTable = ParamsSelectFromTable();
			string limitQuery = Info.Adapter.LimitQuery;
			return (connection, limit, whereCondition, param, transaction, buffered, commandTimeout) => {
				string subQuery = $"{paramsSelectFromTable}{whereCondition}";
				string query = "SELECT " + string.Format(limitQuery, limit, subQuery);
				try {
					IEnumerable<T> result = connection.Query<T>(query, param, transaction, false, commandTimeout);
					return result;
				}
				catch (Exception ex) {
					throw new InvalidOperationException(ex.Message + "\n" + query);
				}
			};
		}

		private DbWhereList<T> CreateGetList()
		{
			string paramsSelectFromTable = ParamsSelectFromTable();
			return (connection, whereCondition, param, transaction, buffered, commandTimeout) => {
				string query = $"SELECT {paramsSelectFromTable}{whereCondition}";
				try {
					IEnumerable<T> result = connection.Query<T>(query, param, transaction, buffered, commandTimeout);
					return result;
				}
				catch (Exception ex) {
					throw new InvalidOperationException(ex.Message + "\n" + query);
				}
			};
		}

		private DbTVoid<T> CreateInsert()
		{
			if (!Info.InsertColumns.Any()) // NoInsertsAttribute
				return DoNothingVoid;
			string insertIntoCmd = InsertIntoCmd();
			string insertedValues = InsertedValues(Info.InsertColumns);
			if (Info.AutoKeyColumn == null) {
				if (!Info.InsertAutoSyncColumns.Any()) {
					return (connection, obj, transaction, commandTimeout) => {
						string cmd = insertIntoCmd + insertedValues;
						try {
							connection.Execute(cmd, obj, transaction, commandTimeout);
						}
						catch (Exception ex) {
							throw new InvalidOperationException(ex.Message + "\n" + cmd);
						}
					};
				}
				DbTVoid<T> insertAutoSync = Queries.InsertAutoSync;
				return (connection, obj, transaction, commandTimeout) => {
					string cmd = insertIntoCmd + insertedValues;
					try {
						connection.Execute(cmd, obj, transaction, commandTimeout);
					}
					catch (Exception ex) {
						throw new InvalidOperationException(ex.Message + "\n" + cmd);
					}
					insertAutoSync(connection, obj, transaction, commandTimeout);
				};
			}
			string selectAutoKey = SelectAutoKey();
			MemberSetter autoKeySetter = Info.AutoKeyColumn.Setter;
			if (!Info.InsertAutoSyncColumns.Any()) {
				return (connection, obj, transaction, commandTimeout) => {
					string cmd = insertIntoCmd + insertedValues + ";\n" + selectAutoKey;
					IDictionary<string, object> key;
					try {
						key = connection.QueryFirst(cmd, obj, transaction, commandTimeout);
					}
					catch (Exception ex) {
						throw new InvalidOperationException(ex.Message + "\n" + cmd);
					}
					autoKeySetter(obj, key.Values.First());
				};
			}
			else {
				DbTVoid<T> insertAutoSync = Queries.InsertAutoSync;
				return (connection, obj, transaction, commandTimeout) => {
					string cmd = insertIntoCmd + insertedValues + ";\n" + selectAutoKey;
					IDictionary<string, object> key;
					try {
						key = connection.QueryFirst(cmd, obj, transaction, commandTimeout);
					}
					catch (Exception ex) {
						throw new InvalidOperationException(ex.Message + "\n" + cmd);
					}
					autoKeySetter(obj, key.Values.First());
					insertAutoSync(connection, obj, transaction, commandTimeout);
				};
			}
		}

		private DbTBool<T> CreateInsertIfNotExists()
		{
			if (!Info.InsertColumns.Any())
				return DoNothing;

			string insertIntoCmd = InsertIntoCmd();
			string insertedValues = InsertedValues(Info.InsertColumns);
			string whereEquals = WhereEquals(EqualityColumns);
			if (Info.AutoKeyColumn == null) {
				if (!Info.InsertAutoSyncColumns.Any()) {
					return (connection, obj, transaction, commandTimeout) => {
						string cmd = $"IF NOT EXISTS (\nSELECT * FROM {TableName}\nWHERE \t{whereEquals})\n{insertIntoCmd}{insertedValues}";
						try {
							int count = connection.Execute(cmd, obj, transaction, commandTimeout);
							return count > 0;
						}
						catch (Exception ex) {
							throw new InvalidOperationException(ex.Message + "\n" + cmd);
						}
					};
				}
				DbTVoid<T> insertAutoSync = Queries.InsertAutoSync;
				return (connection, obj, transaction, commandTimeout) => {
					string cmd = $"IF NOT EXISTS (\nSELECT * FROM {TableName}\nWHERE \t{whereEquals})\n{insertIntoCmd}{insertedValues}";
					int count;
					try {
						count = connection.Execute(cmd, obj, transaction, commandTimeout);
					}
					catch (Exception ex) {
						throw new InvalidOperationException(ex.Message + "\n" + cmd);
					}
					insertAutoSync(connection, obj, transaction, commandTimeout);
					return count > 0;
				};
			}
			string selectAutoKey = SelectAutoKey();
			MemberSetter autoKeySetter = Info.AutoKeyColumn.Setter;
			if (!Info.InsertAutoSyncColumns.Any()) {
				return (connection, obj, transaction, commandTimeout) => {
					string cmd = $"IF NOT EXISTS (\nSELECT * FROM {TableName}\nWHERE \t{whereEquals})\n{insertIntoCmd}{insertedValues};\n{selectAutoKey}";
					object key;
					try {
						key = connection.QueryFirst<dynamic>(cmd, obj, transaction, commandTimeout).Id;
					}
					catch (Exception ex) {
						throw new InvalidOperationException(ex.Message + "\n" + cmd);
					}
					if (key != null) {
						autoKeySetter(obj, key);
						return true;
					}
					return false;
				};
			}
			else {
				DbTVoid<T> insertAutoSync = Queries.InsertAutoSync;
				return (connection, obj, transaction, commandTimeout) => {
					string cmd = $"IF NOT EXISTS (\nSELECT * FROM {TableName}\nWHERE \t{whereEquals})\n{insertIntoCmd}{insertedValues};\n{selectAutoKey}";
					object key;
					try {
						key = connection.QueryFirst<dynamic>(cmd, obj, transaction, commandTimeout).Id;
					}
					catch (Exception ex) {
						throw new InvalidOperationException(ex.Message + "\n" + cmd);
					}
					if (key != null) {
						insertAutoSync(connection, obj, transaction, commandTimeout);
						autoKeySetter(obj, key);
						return true;
					}
					return false;
				};
			}
		}

		private string CreateParamsSelect(Type type)
		{
			IEnumerable<SqlColumn> columns = GetSharedColumns(type, Info.SelectColumns);
			string paramsSelect = ParamsSelect(columns);
			return SelectMap.GetOrAdd(type, paramsSelect);
		}

		private DbWhereInt<T> CreateRecordCount()
		{
			return (connection, whereCondition, param, transaction, commandTimeout) => {
				string query = $"SELECT COUNT(*) FROM {TableName}\n{whereCondition}";
				try {
					int count = connection.Query<int>(query, param, transaction, true, commandTimeout).First();
					return count;
				}
				catch (Exception ex) {
					throw new InvalidOperationException(ex.Message + "\n" + query);
				}
			};
		}

		private DbVoid CreateTruncate()
		{
			if (Info.DeleteKeyColumns.Count == 0) // NoDeletesAttribute
				return DoNothing;
			string cmd = Store(Info.Adapter.TruncateTable(TableName));
			return (connection, transaction, commandTimeout) => {
				try {
					int count = connection.Execute(cmd, null, transaction, commandTimeout);
				}
				catch (Exception ex) {
					throw new InvalidOperationException(ex.Message + "\n" + cmd);
				}
			};
		}

		private DbTBool<T> CreateUpdate()
		{
			if (Info.UpdateKeyColumns.Count == 0) // NoUpdatesAttribute
				return DoNothing;
			string whereUpdateEquals = WhereEquals(Info.UpdateKeyColumns);
			string updateSet = UpdateSet(Info.UpdateColumns);
			if (!Info.UpdateAutoSyncColumns.Any()) {
				return (connection, obj, transaction, commandTimeout) => {
					string cmd = $"UPDATE {TableName}{updateSet}\nWHERE \t{whereUpdateEquals}";
					try {
						int count = connection.Execute(cmd, obj, transaction, commandTimeout);
						return count > 0;
					}
					catch (Exception ex) {
						throw new InvalidOperationException(ex.Message + "\n" + cmd);
					}
				};
			}
			DbTVoid<T> updateAutoSync = Queries.UpdateAutoSync;
			return (connection, obj, transaction, commandTimeout) => {
				string cmd = $"UPDATE {TableName}{updateSet}\nWHERE \t{whereUpdateEquals}";
				int count;
				try {
					count = connection.Execute(cmd, obj, transaction, commandTimeout);
				}
				catch (Exception ex) {
					throw new InvalidOperationException(ex.Message + "\n" + cmd);
				}
				bool success = count > 0;
				if (success) {
					updateAutoSync(connection, obj, transaction, commandTimeout);
				}
				return success;
			};
		}

		private DbObjBool<T> CreateUpdateObj()
		{
			if (Info.UpdateKeyColumns.Count == 0) // NoUpdatesAttribute
				return DoNothing;
			string whereUpdateEquals = WhereEquals(Info.UpdateKeyColumns);
			ConcurrentDictionary<Type, string> UpdateSetMap = new ConcurrentDictionary<Type, string>();
			return (connection, obj, transaction, commandTimeout) => {
				Type type = obj.GetType();
				if (UpdateSetMap.TryGetValue(type, out string updateSet)) {
					IEnumerable<SqlColumn> columns = GetSharedColumns(type, Info.UpdateColumns);
					//HashSet<string> keyColumnNames = new HashSet<string>(Info.UpdateKeyColumns.Select(c => c.Property.Name));
					updateSet = UpdateSet(columns);
					UpdateSetMap.GetOrAdd(type, updateSet);
				}
				string cmd = $"UPDATE {TableName}{updateSet}\nWHERE \t{whereUpdateEquals}";
				try {
					int count = connection.Execute(cmd, obj, transaction, commandTimeout);
					return count > 0;
				}
				catch (Exception ex) {
					throw new InvalidOperationException(ex.Message + "\n" + cmd);
				}
			};
		}

		private DbTBool<T> CreateUpsert()
		{
			if (!Info.UpdateColumns.Any()) {
				if (!Info.InsertColumns.Any())
					return DoNothing;
				// Insert if not exists
				return Queries.InsertIfNotExists;
			}
			DbTBool<T> update = Queries.Update;
			if (!Info.InsertColumns.Any()) {
				// Update only
				return (connection, obj, transaction, commandTimeout) => {
					bool success = update(connection, obj, transaction, commandTimeout);
					return false;
				};
			}
			DbTBool<T> insertIfNotExists = Queries.InsertIfNotExists;
			return (connection, obj, transaction, commandTimeout) => {
				bool success = update(connection, obj, transaction, commandTimeout);
				if (success)
					return false;
				success = insertIfNotExists(connection, obj, transaction, commandTimeout);
				return success;
			};
		}

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		public override string ToString()
		{
			return "SqlBuilder " + Info.Type.FullName;
		}

		#endregion
	}
}
