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

		Ret Find(T obj, int? commandTimeout = null);
		Ret Find<KeyType>(KeyType obj, int? commandTimeout = null);
		IEnumerable<T> GetKeys(string whereCondition = "", object param = null, int? commandTimeout = null);
		IEnumerable<KeyType> GetKeys<KeyType>(string whereCondition = "", object param = null, int? commandTimeout = null);

		bool Delete(T obj, int? commandTimeout = null);
		int Delete(string whereCondition = "", object param = null, int? commandTimeout = null);
		bool Delete<KeyType>(KeyType key, int? commandTimeout = null);

		Ret Insert(T obj, int? commandTimeout = null);
		bool Update(T obj, int? commandTimeout = null);
		Ret Upsert(T obj, int? commandTimeout = null);

		Ret Get(T obj, int? commandTimeout = null);
		Ret Get<KeyType>(KeyType key, int? commandTimeout = null);
		IEnumerable<Ret> GetList(string whereCondition = "", object param = null, int? commandTimeout = null);
		IEnumerable<Ret> GetLimit(int limit, string whereCondition = "", object param = null, int? commandTimeout = null);
		IEnumerable<Ret> GetDistinct(string whereCondition = "", object param = null, int? commandTimeout = null);
		IEnumerable<Ret> GetDistinctLimit(int limit, string whereCondition = "", object param = null, int? commandTimeout = null);

		int RecordCount(string whereCondition = "", object param = null, int? commandTimeout = null);

		List<CacheItem<T>> BulkInsert(IEnumerable<T> objs, int? commandTimeout = null);
		List<CacheItem<T>> BulkGet(IEnumerable<T> objs, int? commandTimeout = null);
		List<CacheItem<T>> BulkGet<KeyType>(IEnumerable<KeyType> objs, int? commandTimeout = null);
		int BulkUpdate(IEnumerable<T> objs, int? commandTimeout = null);
		int BulkDelete(IEnumerable<T> objs, int? commandTimeout = null);
		int BulkDelete<KeyType>(IEnumerable<KeyType> keys, int? commandTimeout = null);
	}
}
