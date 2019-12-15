#region License
// Released under MIT License 
// License: https://www.mit.edu/~amini/LICENSE.md
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
using System.Linq;
using System.Reflection;
using Dapper.Extra.Internal;

namespace Dapper.Extra
{
	/// <summary>
	/// Utilities and metadata accessors for Dapper.ExtraCRUD.
	/// </summary>
	public static class ExtraCrud
	{
		internal static readonly IReadOnlyCollection<Type> ValidAutoKeyTypes = new List<Type>()
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

		private static readonly ConcurrentDictionary<Type, object> BuilderCache = new ConcurrentDictionary<Type, object>();

		/// <summary>
		/// The default dialect for tables.
		/// </summary>
		public static SqlDialect Dialect { get; set; } = SqlDialect.SQLServer;

		/// <summary>
		/// Creates or gets a cached <see cref="SqlBuilder{T}"/>.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <returns>The <see cref="SqlBuilder{T}"/>.</returns>
		public static SqlBuilder<T> Builder<T>() where T : class
		{
			Type type = typeof(T);
			if (BuilderCache.TryGetValue(type, out object obj)) {
				return (SqlBuilder<T>)obj;
			}
			SqlTypeInfo typeInfo = new SqlTypeInfo(type, Dialect);
			SqlBuilder<T> builder = new SqlBuilder<T>(typeInfo);
			return (SqlBuilder<T>)BuilderCache.GetOrAdd(type, builder);
		}

		/// <summary>
		/// Returns the dialect of the connected database.
		/// </summary>
		/// <param name="conn">The connection to use.</param>
		/// <returns>The dialect of the connected database.</returns>
		public static SqlDialect DetectDialect(IDbConnection conn)
		{
			bool notOpen = conn.State != ConnectionState.Open;
			if (notOpen)
				conn.Open();
			SqlDialect dialect = _DetectDialect(conn);
			if (notOpen)
				conn.Close();
			return dialect;
		}

		/// <summary>
		/// Compares two objects of the given type and determines if they are equal.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		public static IEqualityComparer<T> EqualityComparer<T>() where T : class
		{
			IEqualityComparer<T> comparer = Builder<T>().EqualityComparer;
			return comparer;
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
		/// Returns whether a property will be mapped. These must be writable and be of a valid Dapper/SQL type.
		/// </summary>
		/// <param name="property">The <see cref="System.Reflection.PropertyInfo"/> representing the column.</param>
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
			if (ExtraUtil.DbTypeMap.ContainsKey(type) || type.IsEnum)
				return true;
			if (type.IsGenericType && typeof(IEnumerable<>).IsAssignableFrom(type)) {
				Type genericArgType = type.GetGenericArguments()[0];
				return genericArgType == typeof(byte);
			}
			bool success = type.GetInterfaces().Any(ty => ty == typeof(Dapper.SqlMapper.ITypeHandler));
			return success;
		}

		/// <summary>
		/// Clears the cache of queries and builders for the given type.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		public static void Purge<T>() where T : class
		{
			Type type = typeof(T);
			BuilderCache.TryRemove(type, out _);
		}

		/// <summary>
		/// Clears the cache of queries and builders. This is not recommended unless you run out of memory.
		/// </summary>
		public static void PurgeCache()
		{
			BuilderCache.Clear();
		}

		/// <summary>
		/// Creates or gets the queries and commands for a given type.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <returns>The queries for the given type.</returns>
		public static ISqlQueries<T> Queries<T>() where T : class
		{
			ISqlQueries<T> queries = Builder<T>().Queries;
			return queries;
		}

		/// <summary>
		/// Creates or gets a cached <see cref="SqlTypeInfo"/>.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <returns>The <see cref="SqlTypeInfo"/>.</returns>
		public static SqlTypeInfo TypeInfo<T>() where T : class
		{
			SqlTypeInfo typeInfo = Builder<T>().Info;
			return typeInfo;
		}

		/// <summary>
		/// Sets the adapter for the queries/builder. This can be a custom adapter. The builder 
		/// for the given type will be purged from the cache if it is not using the given adapter.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="adapter">The adapter used to generate SQL commands.</param>
		public static void SetAdapter<T>(ISqlAdapter adapter) where T : class
		{
			if (adapter == null) {
				throw new ArgumentNullException(nameof(adapter));
			}
			Type type = typeof(T);
			SqlBuilder<T> builder;
			if (BuilderCache.TryGetValue(type, out object obj)) {
				builder = (SqlBuilder<T>)obj;
				if (ReferenceEquals(builder.Info.Adapter, adapter)) {
					return;
				}
			}
			SqlTypeInfo typeInfo = new SqlTypeInfo(type, adapter);
			builder = new SqlBuilder<T>(typeInfo);
			BuilderCache.AddOrUpdate(type, builder, (t, old) => builder);
		}

		private static SqlDialect _DetectDialect(IDbConnection conn)
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
				return SqlDialect.SQLServer;
			}
			catch { }

			// MySQL
			try {
				int z = conn.QuerySingle<int>("SELECT 1 as `x`");
				int? c = conn.QueryFirstOrDefault<int?>("SELECT LAST_INSERT_ID() as Id");
				//decimal b = conn.QuerySingle<decimal>("SELECT POW(1,1)"); // MySQL
				//string s = conn.QuerySingle<string>("SELECT @@version"); // SQLServer + MySQL
				//int a = conn.QuerySingle<int>("SELECT 1 LIMIT 1");
				return SqlDialect.MySQL;
			}
			catch { }

			try {
				int? a = conn.QueryFirstOrDefault<int?>("SELECT LAST_INSERT_ROWID() as Id");
				return SqlDialect.SQLite;
			}
			catch { }

			// PostgreSQL
			try {
				string s = conn.QuerySingle<string>("'a' || 'b'"); // Oracle + PostgreSQL
																   //DateTime now = conn.QuerySingle<DateTime>("NOW()"); // PostgreSQL + MySQL
				int? a = conn.QueryFirstOrDefault<int?>("SELECT LASTVAL() as Id");
				return SqlDialect.PostgreSQL;
			}
			catch { }

			// Oracle
			//try {
			//	int a = conn.QuerySingle<int>("SELECT BITAND(1,1)");
			//	decimal b = conn.QuerySingle<decimal>("POWER(1,1)");
			//	return SqlDialect.Oracle;
			//}
			//catch { }
			throw new InvalidOperationException("Unknown RDBMS");
		}
	}
}
