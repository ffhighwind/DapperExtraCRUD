using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Dapper.Extra.Interfaces;
using Dapper.Extra.Persistence.Interfaces;
using Dapper.Extra.Persistence.Internal;

namespace Dapper.Extra.Persistence
{
	public sealed class DbCacheTable<T> : ICacheTable<T, CacheItem<T>>, ICacheTable, IReadOnlyDictionary<T, CacheItem<T>> // ICacheStorage<T, T>
		where T : class
	{
		internal DbCacheTable(string connectionString)
		{
			if (TableData<T>.KeyProperties.Count == 0)
				throw new InvalidOperationException(typeof(T).FullName + " is not usable with " + nameof(DbCache) + " without a valid key.");
			DAO = new DataAccessObject<T>(connectionString);
			AAO = new AutoAccessObject<T>(connectionString);
			Access = AAO;
			AutoCache = new CacheAutoStorage<T>(Cache);
			Storage = AutoCache;
		}

		private readonly IDictionary<T, CacheItem<T>> Cache = new Dictionary<T, CacheItem<T>>();
		private ICacheStorage<T> Storage;
		private readonly CacheAutoStorage<T> AutoCache;
		private IAccessObjectSync<T> Access;
		private readonly DataAccessObject<T> DAO;
		private readonly AutoAccessObject<T> AAO;

		public CacheItem<T> Find(T key, int? commandTimeout = null)
		{
			if (!Storage.TryGetValue(key, out CacheItem<T> value)) {
				T obj = Access.Get(key, commandTimeout);
				if (obj != null) {
					value = Storage.AddOrUpdate(obj);
				}
			}
			return value;
		}

		public CacheItem<T> Find<KeyType>(KeyType key, int? commandTimeout = null)
		{
			T obj = TableData<T>.CreateObject<KeyType>(key);
			CacheItem<T> ret = Find(obj, commandTimeout);
			return ret;
		}

		public CacheItem<T> RemoveKey<KeyType>(KeyType key)
		{
			T obj = TableData<T>.CreateObject<KeyType>(key);
			CacheItem<T> ret = Remove(obj);
			return ret;
		}

		public void RemoveKeys<KeyType>(IEnumerable<KeyType> keys)
		{
			IEnumerable<T> objs = keys.Select(key => TableData<T>.CreateObject<KeyType>(key));
			foreach (T obj in objs) {
				Remove(obj);
			}
		}

		#region ICacheTable
		public DbCacheTransaction BeginTransaction()
		{
			if (Access != AutoCache) {
				throw new InvalidOperationException("Cache is already part of another transaction.");
			}
			try {
				DAO.Connection.Open();
				DAO.Transaction = DAO.Connection.BeginTransaction();
				DbCacheTransaction transaction = new DbCacheTransaction(DAO.Transaction);
				CacheTransactionStorage<T> storage = new CacheTransactionStorage<T>(Cache, CloseTransaction);
				Storage = storage;
				transaction.TransactionStorage.Add(storage);
				Access = DAO;
				return transaction;
			}
			catch {
				DAO.Transaction = null;
				if (DAO.Connection.State == System.Data.ConnectionState.Open) {
					DAO.Connection.Close();
				}
				throw;
			}
		}

		public void BeginTransaction(DbCacheTransaction transaction)
		{
			if (Access != AutoCache) {
				throw new InvalidOperationException("Cache is already part of another transaction.");
			}
			DAO.Connection = transaction.Transaction.Connection;
			DAO.Transaction = transaction.Transaction;
			Storage = new CacheTransactionStorage<T>(Cache, CloseTransaction);
			Access = DAO;
		}

		private void CloseTransaction()
		{
			if (DAO.Transaction != null) {
				DAO.Transaction.Dispose();
				DAO.Transaction = null;
				DAO.Connection.Close();
			}
			Storage = AutoCache;
			Access = AAO;
		}

		public Type CachedType => typeof(T);

		Type ICacheTable.KeyType => typeof(T);
		#endregion ICacheTable

		#region IReadOnlyDictionary<T, CacheItem<T>>
		public IEnumerable<T> Keys => Storage.Keys;

		public IEnumerable<CacheItem<T>> Values => Storage.Values;

		public int Count => Storage.Count;

		public CacheItem<T> this[T key] => Storage[key];

		public bool ContainsKey(T key)
		{
			return Storage.ContainsKey(key);
		}

		public bool TryGetValue(T key, out CacheItem<T> value)
		{
			return Storage.TryGetValue(key, out value);
		}

		IEnumerator<KeyValuePair<T, CacheItem<T>>> IEnumerable<KeyValuePair<T, CacheItem<T>>>.GetEnumerator()
		{
			return Storage.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return Storage.GetEnumerator();
		}
		#endregion IReadOnlyDictionary<T, CacheItem<T>>

		#region ICacheStorage<T, T>
		internal CacheItem<T> AddOrUpdate(T value)
		{
			CacheItem<T> item = Storage.AddOrUpdate(value);
			return item;
		}

		internal List<CacheItem<T>> AddOrUpdate(IEnumerable<T> values)
		{
			List<CacheItem<T>> list = Storage.AddOrUpdate(values);
			return list;
		}

		internal CacheItem<T> Add(T value)
		{
			CacheItem<T> item = Storage.Add(value);
			return item;
		}

		internal List<CacheItem<T>> Add(IEnumerable<T> values)
		{
			List<CacheItem<T>> list = Storage.Add(values);
			return list;
		}
		#endregion ICacheStorage<T, T>

		#region ICacheTable<T, CacheItem<T>>
		public void Remove(IEnumerable<T> values)
		{
			Storage.Remove(values);
		}

		public void RemoveKeys(IEnumerable<T> keys)
		{
			Storage.RemoveKeys(keys);
		}

		public bool Contains(T value)
		{
			bool success = Storage.Contains(value);
			return success;
		}

		public CacheItem<T> Remove(T obj)
		{
			CacheItem<T> item = Storage.RemoveKey(obj);
			return item;
		}

		public CacheItem<T> RemoveKey(T key)
		{
			CacheItem<T> item = Storage.RemoveKey(key);
			return item;
		}

		public void Clear()
		{
			Storage.Clear();
		}

		public IEnumerable<T> GetKeys(string whereCondition = "", object param = null, int? commandTimeout = null)
		{
			IEnumerable<T> keys = Access.GetKeys(whereCondition, param, commandTimeout);
			return keys;
		}

		public bool Delete(T key, int? commandTimeout = null)
		{
			bool success = Access.Delete(key, commandTimeout);
			Storage.RemoveKey(key);
			return success;
		}

		public int BulkDelete(IEnumerable<T> keys, int? commandTimeout = null)
		{
			int count = Access.BulkDelete(keys, commandTimeout);
			Storage.RemoveKeys(keys);
			return count;
		}

		public IEnumerable<T> DeleteList(string whereCondition = "", object param = null, int? commandTimeout = null)
		{
			IEnumerable<T> keys = Access.DeleteList(whereCondition, param, commandTimeout);
			Storage.RemoveKeys(keys);
			return keys;
		}

		public CacheItem<T> Get(T key, int? commandTimeout = null)
		{
			T obj = Access.Get(key, commandTimeout);
			if (obj == null) {
				CacheItem<T> ret = Storage.RemoveKey(obj);
				return null;
			}
			CacheItem<T> value = Storage.AddOrUpdate(obj);
			return value;
		}

		public int Delete(string whereCondition = "", object param = null, int? commandTimeout = null)
		{
			IEnumerable<T> keys = DeleteList(whereCondition, param, commandTimeout);
			Storage.RemoveKeys(keys);
			return keys.Count();
		}

		public CacheItem<T> Insert(T obj, int? commandTimeout = null)
		{
			T item = Access.Insert(obj, commandTimeout);
			CacheItem<T> value = Storage.AddOrUpdate(obj);
			return value;
		}

		public bool Update(T obj, int? commandTimeout = null)
		{
			if (!Access.Update(obj, commandTimeout)) {
				Storage.AddOrUpdate(obj);
				return true;
			}
			CacheItem<T> ret = Storage.RemoveKey(obj);
			return false;
		}

		public int BulkUpdate(IEnumerable<T> objs, int? commandTimeout = null)
		{
			int count = Access.BulkUpdate(objs, commandTimeout);
			return count;
		}

		public CacheItem<T> Upsert(T obj, int? commandTimeout = null)
		{
			bool updated = Access.Upsert(obj, commandTimeout);
			CacheItem<T> ret = Storage.AddOrUpdate(obj);
			return ret;
		}

		public IEnumerable<CacheItem<T>> GetList(string whereCondition = "", object param = null, int? commandTimeout = null)
		{
			IEnumerable<T> list = Access.GetList(whereCondition, param, commandTimeout);
			List<CacheItem<T>> result = Storage.AddOrUpdate(list);
			return result;
		}

		public IEnumerable<CacheItem<T>> GetLimit(int limit, string whereCondition = "", object param = null, int? commandTimeout = null)
		{
			IEnumerable<T> list = Access.GetLimit(limit, whereCondition, param, commandTimeout);
			List<CacheItem<T>> result = Storage.AddOrUpdate(list);
			return result;
		}

		public IEnumerable<CacheItem<T>> GetDistinct(string whereCondition = "", object param = null, int? commandTimeout = null)
		{
			IEnumerable<T> list = Access.GetDistinct(whereCondition, param, commandTimeout);
			List<CacheItem<T>> result = Storage.AddOrUpdate(list);
			return result;
		}

		public int RecordCount(string whereCondition = "", object param = null, int? commandTimeout = null)
		{
			int count = Access.RecordCount(whereCondition, param, commandTimeout);
			return count;
		}

		public IEnumerable<CacheItem<T>> GetDistinctLimit(int limit, string whereCondition = "", object param = null, int? commandTimeout = null)
		{
			IEnumerable<T> list = Access.GetDistinctLimit(limit, whereCondition, commandTimeout);
			List<CacheItem<T>> result = Storage.AddOrUpdate(list);
			return result;
		}

		public IEnumerable<KeyType> GetKeys<KeyType>(string whereCondition = "", object param = null, int? commandTimeout = null)
		{
			IEnumerable<KeyType> keys = Access.GetKeys<KeyType>(whereCondition, param, commandTimeout);
			return keys;
		}

		public bool Delete<KeyType>(KeyType key, int? commandTimeout = null)
		{
			bool success = Access.Delete<KeyType>(key, commandTimeout);
			T obj = TableData<T>.CreateObject<KeyType>(key);
			CacheItem<T> ret = Storage.Remove(obj);
			return success;
		}

		public CacheItem<T> Get<KeyType>(KeyType key, int? commandTimeout = null)
		{
			T obj = Access.Get<KeyType>(key, commandTimeout);
			CacheItem<T> ret = Storage.AddOrUpdate(obj);
			return ret;
		}

		public int BulkDelete<KeyType>(IEnumerable<KeyType> keys, int? commandTimeout = null)
		{
			int count = Access.BulkDelete<KeyType>(keys, commandTimeout);
			Storage.RemoveKeys(keys);
			return count;
		}
		#endregion ICacheTable<T, CacheItem<T>>
	}
}
