using Dapper.Extra.Internal;
using Fasterflect;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace Dapper.Extra
{
	public static class ExtraUtil
	{
		/// <summary>
		/// Converts a <see cref="StringComparison"/> to the equivilent <see cref="StringComparer"/>.
		/// </summary>
		/// <param name="comparison">The <see cref="StringComparison"/>.</param>
		/// <returns>A <see cref="StringComparer"/> to compare strings.</returns>
		public static IEqualityComparer<string> ToComparer(StringComparison comparison)
		{
			IEqualityComparer<string> comparer;
			switch (comparison) {
				case StringComparison.CurrentCulture:
					comparer = StringComparer.CurrentCulture;
					break;
				case StringComparison.CurrentCultureIgnoreCase:
					comparer = StringComparer.CurrentCultureIgnoreCase;
					break;
				case StringComparison.InvariantCulture:
					comparer = StringComparer.InvariantCulture;
					break;
				case StringComparison.InvariantCultureIgnoreCase:
					comparer = StringComparer.InvariantCultureIgnoreCase;
					break;
				case StringComparison.Ordinal:
					comparer = StringComparer.Ordinal;
					break;
				case StringComparison.OrdinalIgnoreCase:
					comparer = StringComparer.OrdinalIgnoreCase;
					break;
				default:
					throw new NotSupportedException(comparison.ToString());
			}
			return comparer;
		}

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
					str = ((bool)value) ? "TRUE" : "FALSE";
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
					else if (type == typeof(DateTimeOffset))
						value = default(DateTimeOffset);
					else if (type == typeof(Guid))
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
			IEnumerable<SqlColumn> columns, int? commandTimeout, SqlBulkCopyOptions options)
			where T : class
		{
			DbDataReader dataReader = factory.Create(objs);
			using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection, options, transaction)) {
				bulkCopy.DestinationTableName = tableName;
				bulkCopy.BulkCopyTimeout = commandTimeout ?? 0;
				foreach (SqlColumn column in columns) {
					bulkCopy.ColumnMappings.Add(column.Property.Name, column.ColumnName);
				}
				bulkCopy.WriteToServer(dataReader);
			}
		}
	}
}
