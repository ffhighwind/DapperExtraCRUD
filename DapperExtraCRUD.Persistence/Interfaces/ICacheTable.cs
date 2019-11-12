using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Extra.Persistence.Interfaces
{
	public interface ICacheTable
	{
		DbCacheTransaction BeginTransaction();
		void BeginTransaction(DbCacheTransaction transaction);
		Type CachedType { get; }
		Type KeyType { get; }
	}

	public interface ICacheTable<T, Ret>
		where T : class
	{
		Ret Remove(T obj);
		void Remove(IEnumerable<T> values);
		Ret RemoveKey<KeyType>(KeyType key);
		void RemoveKeys<KeyType>(IEnumerable<KeyType> keys);
		bool Contains(T value);
		void Clear();

		Ret Find(T obj, int commandTimeout = 30);
		Ret Find<KeyType>(KeyType obj, int commandTimeout = 30);
		IEnumerable<T> GetKeys(string whereCondition = "", object param = null, int commandTimeout = 30);
		IEnumerable<KeyType> GetKeys<KeyType>(string whereCondition = "", object param = null, int commandTimeout = 30);

		bool Delete(T obj, int commandTimeout = 30);
		int Delete(string whereCondition = "", object param = null, int commandTimeout = 30);
		bool Delete<KeyType>(KeyType key, int commandTimeout = 30);

		Ret Insert(T obj, int commandTimeout = 30);
		bool Update(T obj, int commandTimeout = 30);
		Ret Upsert(T obj, int commandTimeout = 30);

		Ret Get(T obj, int commandTimeout = 30);
		Ret Get<KeyType>(KeyType key, int commandTimeout = 30);
		IEnumerable<Ret> GetList(string whereCondition = "", object param = null, int commandTimeout = 30);
		IEnumerable<Ret> GetLimit(int limit, string whereCondition = "", object param = null, int commandTimeout = 30);
		IEnumerable<Ret> GetDistinct(string whereCondition = "", object param = null, int commandTimeout = 30);
		IEnumerable<Ret> GetDistinctLimit(int limit, string whereCondition = "", object param = null, int commandTimeout = 30);

		int RecordCount(string whereCondition = "", object param = null, int commandTimeout = 30);

		List<CacheItem<T>> BulkInsert(IEnumerable<T> objs, int commandTimeout = 30);
		List<CacheItem<T>> BulkGet(IEnumerable<T> objs, int commandTimeout = 30);
		List<CacheItem<T>> BulkGet<KeyType>(IEnumerable<KeyType> objs, int commandTimeout = 30);
		int BulkUpdate(IEnumerable<T> objs, int commandTimeout = 30);
		int BulkDelete(IEnumerable<T> objs, int commandTimeout = 30);
		int BulkDelete<KeyType>(IEnumerable<KeyType> keys, int commandTimeout = 30);
	}
}
