using Dapper.Extra.Internal;
using Fasterflect;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

namespace Dapper.Extra
{
	/// <summary>
	/// SQL utilities.
	/// </summary>
	public static class ExtraUtil
	{
		internal static Dictionary<Type, DbType> DbTypeMap = new Dictionary<Type, DbType>() {
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
		/// Returns the <see cref="DbType"/> that represents the given <see cref="Type"/>. This can be used for switch
		/// statements when handling objects being passed to an SQL database.
		/// </summary>
		/// <param name="type">The type to determine the <see cref="DbType"/> for.</param>
		/// <returns>The <see cref="DbType"/> representing the type if one exists; otherwise <see cref="DbType.Object"/>.</returns>
		public static DbType GetDbType(Type type)
		{
			if (!DbTypeMap.TryGetValue(type, out DbType dbType)) {
				dbType = type.IsEnum ? DbType.Int32 : DbType.Object;
			}
			return dbType;
		}

		/// <summary>
		/// Returns whether the value of an object with the given type should be quoted in SQL.
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
					return type == typeof(Guid) || type == typeof(DateTimeOffset) || type == typeof(TimeSpan);
			}
			return false;
		}

		/// <summary>
		/// Returns the SQL string and <see cref="DbType"/> that represent an object.
		/// </summary>
		/// <param name="value">The object to obtain the SQL value of.</param>
		/// <param name="str">The SQL string representation of the value. This will be <see langword="null"/> if the value cannot be determined.</param>
		/// <returns>The <see cref="DbType"/> that represents the object's <see cref="Type"/>.</returns>
		public static DbType SqlValue(object value, out string str)
		{
			if (value == null) {
				str = "NULL";
				return DbType.Object;
			}
			DbType dbType = GetDbType(value.GetType());
			switch (dbType) {
				case DbType.AnsiStringFixedLength:
				case DbType.StringFixedLength:
					if (value is char ch) {
						str = ch == '\'' ? "''''" : "'" + value.ToString() + "'";
						break;
					}
					goto case DbType.Xml;
				case DbType.AnsiString:
				case DbType.String:
				case DbType.Xml:
					str = "'" + value.ToString().Replace("'", "''") + "'";
					break;
				case DbType.Boolean:
					str = ((bool)value) ? "TRUE" : "FALSE";
					break;
				case DbType.Byte:
				case DbType.Int16:
				case DbType.Int32:
				case DbType.Int64:
				case DbType.SByte:
				case DbType.UInt16:
				case DbType.UInt32:
				case DbType.UInt64:
					str = value.ToString();
					break;
				case DbType.Guid:
					str = "'" + value.ToString() + "'";
					break;
				case DbType.Binary:
				case DbType.Object:
					str = null;
					break;
				case DbType.Time:
					str = ((TimeSpan)value).ToString("'HH:mm:ss:fff'");
					break;
				case DbType.Single:
				case DbType.Double:
				case DbType.VarNumeric:
				case DbType.Decimal:
				case DbType.Currency:
					str = ((decimal)value).ToString();
					break;
				case DbType.Date:
					str = ((DateTime)value).ToString("'yyyy-MM-dd'");
					break;
				case DbType.DateTime:
				case DbType.DateTime2:
					str = ((DateTime)value).ToString("'yyyy-MM-dd HH:mm:ss:fff'");
					break;
				case DbType.DateTimeOffset:
					str = ((DateTimeOffset)value).ToString("'dddd, MMM dd yyyy HH:mm:ss:fff zzz'", CultureInfo.InvariantCulture);
					break;
				default:
					throw new InvalidOperationException("Unknown DbType: " + dbType.ToString());
			}
			return dbType;
		}

		/// <summary>
		/// Inserts data into a table using <see cref="SqlBulkCopy"/>.
		/// </summary>
		/// <typeparam name="T">The type of object to insert.</typeparam>
		/// <param name="connection">The database connection.</param>
		/// <param name="objs">The objects to insert into the table.</param>
		/// <param name="transaction">The transaction for the connection, or null if an internal transaction should be used.</param>
		/// <param name="tableName">The name of the table.</param>
		/// <param name="factory">The factory from an <see cref="SqlBuilder{T}"/>.</param>
		/// <param name="columns">The column mappings for the table.</param>
		/// <param name="commandTimeout">The command timeout in seconds. 0 or null prevent a timeout.</param>
		/// <param name="options">The bulk copy options used when transfering data.</param>
		public static void BulkInsert<T>(SqlConnection connection, IEnumerable<T> objs, SqlTransaction transaction, string tableName, DataReaderFactory factory,
			IEnumerable<SqlColumn> columns, int commandTimeout = 30, SqlBulkCopyOptions options = SqlBulkCopyOptions.Default)
			where T : class
		{
			DbDataReader dataReader = factory.Create(objs);
			BulkInsert(connection, dataReader, transaction, tableName, columns.Select(c => c.Property.Name), columns.Select(c => c.ColumnName), commandTimeout, options);
		}

		/// <summary>
		/// Inserts data into a table using <see cref="SqlBulkCopy"/>.
		/// </summary>
		/// <param name="connection">The database connection.</param>
		/// <param name="dataReader">The source of data to be inserted.</param>
		/// <param name="transaction">The transaction for the connection, or null if an internal transaction should be used.</param>
		/// <param name="tableName">The name of the table.</param>
		/// <param name="propertyNames">The property names that map to the column names.</param>
		/// <param name="columnNames">The column names that map to the properties.</param>
		/// <param name="commandTimeout">The command timeout in seconds. 0 or null prevent a timeout.</param>
		/// <param name="options">The bulk copy options used when transfering data.</param>
		public static void BulkInsert(SqlConnection connection, DbDataReader dataReader, SqlTransaction transaction, string tableName, IEnumerable<string> propertyNames,
			IEnumerable<string> columnNames, int commandTimeout = 30, SqlBulkCopyOptions options = SqlBulkCopyOptions.Default)
		{
			using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection, options, transaction)) {
				bulkCopy.DestinationTableName = tableName;
				bulkCopy.BulkCopyTimeout = commandTimeout;
				var columns = propertyNames.Zip(columnNames, (p, c) => new { PropertyName = p, ColumnName = c });
				foreach (var column in columns) {
					bulkCopy.ColumnMappings.Add(column.PropertyName, column.ColumnName);
				}
				bulkCopy.WriteToServer(dataReader);
			}
		}
	}
}
