// Released under MIT License 
// Copyright(c) 2018 Wesley Hamilton
// License: https://www.mit.edu/~amini/LICENSE.md
// Home page: https://github.com/ffhighwind/DapperExtraCRUD

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dapper.Extra.Annotations;
using static Dapper.SqlMapper;

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
			if (DbTypeMap.TryGetValue(type, out dbType))
				return true;
			if (type.IsEnum) {
				dbType = DbType.Int32;
				return true;
			}
			return false;
		}

		private static readonly IReadOnlyCollection<Type> ValidAutoKeyTypes = new List<Type>()
		{
			typeof(int),
			typeof(long),
			typeof(short),
			typeof(byte),
			typeof(uint),
			typeof(ulong),
			typeof(ushort),
			typeof(sbyte),
		};

		/// <summary>
		/// Returns whether the ToString() value of an object with the given type should be quoted in an SQL command.
		/// </summary>
		/// <param name="type">The type of object.</param>
		/// <returns>True if the type should be quoted; false otherwise.</returns>
		public static bool IsQuotedSqlType(Type type)
		{
			TypeCode typeCode = Type.GetTypeCode(type);
			switch (typeCode) {
				case TypeCode.Char:
				case TypeCode.DateTime:
				case TypeCode.String:
					return true;
				case TypeCode.Object:
					return type == typeof(TimeSpan) || type == typeof(DateTimeOffset) || type == typeof(Guid);
				//case TypeCode.Boolean:
				//case TypeCode.SByte:
				//case TypeCode.Byte:
				//case TypeCode.Int16:
				//case TypeCode.UInt16:
				//case TypeCode.Int32:
				//case TypeCode.UInt32:
				//case TypeCode.Int64:
				//case TypeCode.UInt64:
				//case TypeCode.Single:
				//case TypeCode.Double:
				//case TypeCode.Decimal:
				//case TypeCode.DBNull:
				default:
					return false;
			}
		}

		public static bool SqlValue(object value, out string str)
		{
			if (value == null) {
				str = "NULL";
				return true;
			}
			Type type = value.GetType();
			type = Nullable.GetUnderlyingType(type) ?? type;
			TypeCode typeCode = Type.GetTypeCode(type);
			switch (typeCode) {
				case TypeCode.Object:
					if (type == typeof(TimeSpan) || type == typeof(DateTimeOffset) || type == typeof(Guid)) {
						str = null;
						return false;
					}
					break;
				case TypeCode.Single:
				case TypeCode.Double:
				case TypeCode.DateTime:
					str = null;
					return false;
				case TypeCode.Boolean:
					str = ((bool) value) ? "TRUE" : "FALSE";
					return true;
				case TypeCode.String:
					str = "'" + value.ToString().Replace("'", "''") + "'";
					return true;
				case TypeCode.Char:
					str = (char)value == '\'' ? "''''" : "'" + value.ToString() + "'";
					return true;
				case TypeCode.Decimal:
				case TypeCode.SByte:
				case TypeCode.Byte:
				case TypeCode.Int16:
				case TypeCode.UInt16:
				case TypeCode.Int32:
				case TypeCode.UInt32:
				case TypeCode.Int64:
				case TypeCode.UInt64:
					str = value.ToString();
					return true;
				//case TypeCode.DBNull:
				case TypeCode.Empty:
					str = "NULL";
					return true;
			}
			throw new InvalidOperationException("Invalid SQL type " + type.Name.ToString());
		}

		public static bool SqlDefaultValue(Type type, out object value)
		{
			if (!type.IsValueType) {
				value = "NULL";
				return true;
			}
			TypeCode typeCode = Type.GetTypeCode(type);
			switch (typeCode) {
				case TypeCode.Object:
					if (type == typeof(TimeSpan))
						value = default(TimeSpan);
					else if(type == typeof(DateTimeOffset))
						value = default(DateTimeOffset);
					else if(type == typeof(Guid))
						value = default(Guid);
					else
						break;
					return false;
				case TypeCode.Boolean:
					value = "FALSE";
					return true;
				case TypeCode.Char:
					value = "'\0'";
					return true;
				case TypeCode.SByte:
				case TypeCode.Byte:
				case TypeCode.Int16:
				case TypeCode.UInt16:
				case TypeCode.Int32:
				case TypeCode.UInt32:
				case TypeCode.Int64:
				case TypeCode.UInt64:
				case TypeCode.Single:
				case TypeCode.Double:
				case TypeCode.Decimal:
					value = "0";
					return true;
				case TypeCode.DateTime:
					value = default(DateTime);
					return false;
				//case TypeCode.String:
				//	value = "NULL";
				//	return true;
				case TypeCode.Empty:
				case TypeCode.DBNull:
				default:
					break;
			}
			value = null;
			return false;
		}

		/// <summary>
		/// Returns whether a property will be mapped. These must be writable and be of a valid Dapper/SQL type.
		/// </summary>
		/// <param name="property">The <see cref="PropertyInfo"/> representing the column.</param>
		/// <returns>True if the given property will be mapped; otherwise false.</returns>
		public static bool IsValidProperty(PropertyInfo property)
		{
			if (!property.CanWrite)
				return false;
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
			bool success = type.GetInterfaces().Any(ty => ty == typeof(ITypeHandler));
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
			FastMember.ObjectReader dataReader = FastMember.ObjectReader.Create<T>(objs, columnProperties.Select(p => p.Name).ToArray());
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
