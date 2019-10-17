using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Extension
{
	public class TableCache<T, KeyType> : TableCacheBase<T, KeyType, T> where T : class
	{
		public TableCache(DataAccessObject<T, KeyType> dao) : base(dao, TableCacheDelegates<T>._Constructor, TableCacheDelegates<T>._Updater)
		{
		}

		public TableCache(string connString) : this(new DataAccessObject<T, KeyType>(connString))
		{
		}
	}

	public class TableCache<T> : TableCacheBase<T, T> where T : class
	{
		public TableCache(DataAccessObject<T> dao) : base(dao, TableCacheDelegates<T>._Constructor, TableCacheDelegates<T>._Updater)
		{
		}

		public TableCache(string connString) : this(new DataAccessObject<T>(connString))
		{
		}
	}

	internal class TableCacheDelegates<T> where T : class
	{
		public static T _Constructor(T value) => value;
		public static T _Updater(T newObj, T oldObj)
		{
			TableData<T>.Copy(newObj, oldObj);
			return oldObj;
		}
	}
}