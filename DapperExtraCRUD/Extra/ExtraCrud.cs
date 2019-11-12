using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.Extra.Internal;

namespace Dapper.Extra
{
	public static class ExtraCrud
	{
		public static SqlSyntax Syntax { get; set; } = SqlSyntax.SQLServer;
		public static bool ThreadSafe { get; set; } = true;
		private static ConcurrentDictionary<Type, object> Cache = new ConcurrentDictionary<Type, object>();

		public static SqlBuilder<T> Builder<T>() where T : class
		{
			Type type = typeof(SqlBuilder<T>);
			if (Cache.TryGetValue(type, out object builder)) {
				return (SqlBuilder<T>) builder;
			}
			SqlBuilder<T> b = new SqlBuilder<T>(new SqlTypeInfo(type), ThreadSafe ? System.Threading.LazyThreadSafetyMode.ExecutionAndPublication : System.Threading.LazyThreadSafetyMode.None);
			return (SqlBuilder<T>) Cache.GetOrAdd(type, b);
		}

		public static SqlBuilder<T, KeyType> Builder<T, KeyType>() where T : class
		{
			SqlBuilder<T> builder = Builder<T>();
			return builder.Create<KeyType>();
		}

		public static SqlQueries<T> Queries<T>() where T : class
		{
			Type type = typeof(SqlQueries<T>);
			if (Cache.TryGetValue(type, out object queries)) {
				return (SqlQueries<T>) queries;
			}
			SqlBuilder<T> builder = Builder<T>();
			return (SqlQueries<T>) Cache.GetOrAdd(type, builder.Queries);
		}

		public static SqlQueries<T, KeyType> Queries<T, KeyType>() where T : class
		{
			Type type = typeof(SqlQueries<T, KeyType>);
			if (Cache.TryGetValue(type, out object queries)) {
				return (SqlQueries<T, KeyType>) queries;
			}
			SqlBuilder<T, KeyType> builder = Builder<T, KeyType>();
			return (SqlQueries<T, KeyType>) Cache.GetOrAdd(type, builder.Queries);
		}

		public static IEqualityComparer<T> EqualityComparer<T>() where T : class
		{
			return Builder<T>().EqualityComparer;
		}

		public static void ClearCache()
		{
			Cache.Clear();
		}
	}
}
