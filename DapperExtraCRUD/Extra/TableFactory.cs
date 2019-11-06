using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
			return propertyType.GetInterfaces().Any(ty => ty == typeof(SqlMapper.ITypeHandler));
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
	}
}
