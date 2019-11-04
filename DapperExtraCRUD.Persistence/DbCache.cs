using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.Extra.Interfaces;
using Dapper.Extra.Persistence.Internal;

namespace Dapper.Extra.Persistence
{
	public class DbCache
	{
		protected internal IDictionary<Type, object> Map = new Dictionary<Type, object>();
		protected internal string ConnectionString { get; set; }

		public DbCache(string connectionString)
		{
			ConnectionString = connectionString;
		}

		public DbCacheTable<T> CreateTable<T>()
			where T : class
		{
			if (!Map.TryGetValue(typeof(T), out object cache)) {
				cache = new DbCacheTable<T>(ConnectionString);
				Map[typeof(T)] = cache;
			}
			return (DbCacheTable<T>) cache;
		}
	}
}
