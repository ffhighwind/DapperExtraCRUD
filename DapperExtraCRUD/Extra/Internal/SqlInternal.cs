using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Extra.Internal
{
	public static class SqlInternal
	{
		private static Dictionary<Type, DbType> DbTypeMap = new Dictionary<Type, DbType>()
		{
			[typeof(byte)] = DbType.Byte,
			[typeof(sbyte)] = DbType.SByte,
			[typeof(short)] = DbType.Int16,
			[typeof(ushort)] = DbType.UInt16,
			[typeof(int)] = DbType.Int32,
			[typeof(uint)] = DbType.UInt32,
			[typeof(long)] = DbType.Int64,
			[typeof(ulong)] = DbType.UInt64,
			[typeof(float)] = DbType.Single,
			[typeof(double)] = DbType.Double,
			[typeof(decimal)] = DbType.Decimal,
			[typeof(bool)] = DbType.Boolean,
			[typeof(string)] = DbType.String,
			[typeof(char)] = DbType.StringFixedLength,
			[typeof(Guid)] = DbType.Guid,
			[typeof(DateTime)] = DbType.DateTime,
			[typeof(DateTimeOffset)] = DbType.DateTimeOffset,
			[typeof(TimeSpan)] = DbType.Time,
			[typeof(byte[])] = DbType.Binary,
			[typeof(byte?)] = DbType.Byte,
			[typeof(sbyte?)] = DbType.SByte,
			[typeof(short?)] = DbType.Int16,
			[typeof(ushort?)] = DbType.UInt16,
			[typeof(int?)] = DbType.Int32,
			[typeof(uint?)] = DbType.UInt32,
			[typeof(long?)] = DbType.Int64,
			[typeof(ulong?)] = DbType.UInt64,
			[typeof(float?)] = DbType.Single,
			[typeof(double?)] = DbType.Double,
			[typeof(decimal?)] = DbType.Decimal,
			[typeof(bool?)] = DbType.Boolean,
			[typeof(char?)] = DbType.StringFixedLength,
			[typeof(Guid?)] = DbType.Guid,
			[typeof(DateTime?)] = DbType.DateTime,
			[typeof(DateTimeOffset?)] = DbType.DateTimeOffset,
			[typeof(TimeSpan?)] = DbType.Time,
			[typeof(object)] = DbType.Object
		};

		/// <summary>
		/// Returns the <see cref="DbType"/> that represents the given <see cref="Type"/>.
		/// </summary>
		/// <param name="type">The type to determine the <see cref="DbType"/> for.</param>
		/// <param name="dbType">The <see cref="DbType"/> representing the type if one exists; otherwise null.</param>
		/// <returns>True if there is a <see cref="DbType"/> representing the given <see cref="Type"/>.</returns>
		public static bool TryGetDbType(Type type, out DbType dbType)
		{
			return DbTypeMap.TryGetValue(type, out dbType);
		}

		private static readonly IReadOnlyCollection<Type> ValidAutoKeyTypes = new List<Type>()
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
		/// Returns the name of the column. This is determined by the <see cref="ColumnAttribute"/> or the name of the property.
		/// </summary>
		/// <param name="property">The <see cref="PropertyInfo"/> representing the column.</param>
		/// <returns>The name of the column.</returns>
		public static string GetColumnName(PropertyInfo property)
		{
			return property.GetCustomAttribute<ColumnAttribute>(true)?.Name ?? property.Name;
		}

		/// <summary>
		/// Returns whether a property will be mapped.
		/// </summary>
		/// <param name="property">The <see cref="PropertyInfo"/> representing the column.</param>
		/// <returns>True if the given property will be mapped; otherwise false.</returns>
		public static bool IsValidProperty(PropertyInfo property)
		{
			if (!property.CanWrite || !property.CanRead)
				return false;
			if (property.GetCustomAttribute<KeyAttribute>(false) == null) {
				if (property.GetCustomAttribute<IgnoreAttribute>(false) != null
					|| (property.GetCustomAttribute<IgnoreSelectAttribute>(false) != null
						&& property.GetCustomAttribute<IgnoreInsertAttribute>(false) != null
						&& property.GetCustomAttribute<IgnoreUpdateAttribute>(false) != null)) {
					return false;
				}
			}
			bool success = IsValidType(property.PropertyType);
			return success;
		}

		/// <summary>
		/// Returns whether a property with the given type will be mapped.
		/// </summary>
		/// <param name="type">The type to check.</param>
		/// <returns>True if a property with the given type will be mapped; false otherwise.</returns>
		public static bool IsValidType(Type type)
		{
			if (type == typeof(object))
				return false;
			//type = Nullable.GetUnderlyingType(type) ?? type;
			if (DbTypeMap.ContainsKey(type) || type.IsEnum)
				return true;
			if (type.IsGenericType && typeof(IEnumerable<>).IsAssignableFrom(type)) {
				Type genericArgType = type.GetGenericArguments()[0];
				return genericArgType == typeof(byte);
			}
			bool success = type.GetInterfaces().Any(ty => ty == typeof(Dapper.SqlMapper.ITypeHandler));
			return success;
		}

		/// <summary>
		/// Returns whether the given type is valid for an autoincrement key.
		/// </summary>
		/// <param name="type">The type to check.</param>
		/// <returns>True if the type is valid for an autoincrement key; false otherwise.</returns>
		public static bool IsValidAutoIncrementType(Type type)
		{
			bool success = ValidAutoKeyTypes.Contains(type) || type.IsEnum;
			return success;
		}

		/// <summary>
		/// Inserts data into a table using <see cref="SqlBulkCopy"/>.
		/// </summary>
		/// <typeparam name="T">The type of object to insert.</typeparam>
		/// <param name="connection">The database connection.</param>
		/// <param name="objs">The objects to insert into the table.</param>
		/// <param name="transaction">The transaction for the connection, or null if an internal transaction should be used.</param>
		/// <param name="tableName">The name of the table.</param>
		/// <param name="columnProperties">The properties that map to the columns in the destination table.</param>
		/// <param name="columnNames">The names of the columns in the destination table.</param>
		/// <param name="commandTimeout">The command timeout in seconds. By default this is 30 seconds.</param>
		/// <param name="options">The bulk copy options used when transfering data.</param>
		public static void BulkInsert<T>(SqlConnection connection, IEnumerable<T> objs, SqlTransaction transaction, string tableName, IEnumerable<PropertyInfo> columnProperties,
			string[] columnNames, int? commandTimeout, SqlBulkCopyOptions options)
			where T : class
		{
			FastMember.ObjectReader dataReader = FastMember.ObjectReader.Create<T>(objs, columnNames);
			using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection, options, transaction)) {
				bulkCopy.DestinationTableName = tableName;
				bulkCopy.BulkCopyTimeout = commandTimeout ?? 0;
				int i = 0;
				foreach(PropertyInfo property in columnProperties) { 
					bulkCopy.ColumnMappings.Add(property.Name, columnNames[i]);
					i++;
				}
				bulkCopy.WriteToServer(dataReader);
			}
		}

		/// <summary>
		/// Inserts data into a table using <see cref="SqlBulkCopy"/>.
		/// </summary>
		/// <typeparam name="T">The type of object to insert.</typeparam>
		/// <param name="connection">The database connection.</param>
		/// <param name="objs">The objects to insert into the table.</param>
		/// <param name="transaction">The transaction for the connection, or null if an internal transaction should be used.</param>
		/// <param name="tableName">The name of the table.</param>
		/// <param name="columns">The column mappings for the table.</param>
		/// <param name="commandTimeout">The command timeout in seconds. By default this is 30 seconds.</param>
		/// <param name="options">The bulk copy options used when transfering data.</param>
		public static void BulkInsert<T>(SqlConnection connection, IEnumerable<T> objs, SqlTransaction transaction, string tableName, IEnumerable<SqlColumn> columns,
			int? commandTimeout, SqlBulkCopyOptions options)
			where T : class
		{
			BulkInsert<T>(connection, objs, transaction, tableName, columns.Select(c => c.Property), columns.Select(c => c.ColumnName).ToArray(), commandTimeout, options);
		}

		/// <summary>
		/// Returns the syntax of the connected database.
		/// </summary>
		/// <param name="conn">The connection to use.</param>
		/// <returns>The syntax of the connected database.</returns>
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

		private static SqlSyntax _DetectSyntax(IDbConnection conn)
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
			//try {
			//	int a = conn.QuerySingle<int>("SELECT BITAND(1,1)");
			//	decimal b = conn.QuerySingle<decimal>("POWER(1,1)");
			//	return SqlSyntax.Oracle;
			//}
			//catch { }
			throw new InvalidOperationException("Unknown RDBMS");
		}
	}
}
