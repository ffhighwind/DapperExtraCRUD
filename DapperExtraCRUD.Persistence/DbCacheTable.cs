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
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Dapper.Extra.Internal;
using Dapper.Extra.Persistence.Interfaces;
using Dapper.Extra.Persistence.Internal;
using Dapper.Extra.Utilities;

namespace Dapper.Extra.Persistence
{
	public sealed class DbCacheTable<T> : ICacheTable<T, CacheItem<T>>, ICacheTable, IReadOnlyDictionary<T, CacheItem<T>>
		where T : class
	{
		internal DbCacheTable(string connectionString)
		{
			var info = ExtraCrud.TypeInfo<T>();
			if (info.KeyColumns.Count == 0)
				throw new InvalidOperationException(typeof(T).FullName + " is not usable with " + nameof(DbCache) + " without a valid key.");
			Cache = new Dictionary<T, CacheItem<T>>(ExtraCrud.EqualityComparer<T>());
			DAO = new DataAccessObject<T>(connectionString);
			AAO = new AutoAccessObject<T>(connectionString);
			Access = AAO;
			AutoCache = new CacheAutoStorage<T>(Cache);
			Storage = AutoCache;
			CreateFromKey = Builder.CreateFromKey;
		}

		private readonly Func<object, T> CreateFromKey;

		private readonly IDictionary<T, CacheItem<T>> Cache;
		private ICacheStorage<T> Storage;
		private readonly CacheAutoStorage<T> AutoCache;
		private IAccessObjectSync<T> Access;
		private readonly DataAccessObject<T> DAO;
		private readonly AutoAccessObject<T> AAO;
		private readonly SqlBuilder<T> Builder;

		public CacheItem<T> Find(T key, int commandTimeout = 30)
		{
			if (!Storage.TryGetValue(key, out CacheItem<T> value)) {
				T obj = Access.Get(key, commandTimeout);
				if (obj != null) {
					value = Storage.AddOrUpdate(obj);
				}
			}
			return value;
		}

		public CacheItem<T> Find(object key, int commandTimeout = 30)
		{
			T obj = CreateFromKey(key);
			CacheItem<T> ret = Find(obj, commandTimeout);
			return ret;
		}

		public CacheItem<T> RemoveKey(object key)
		{
			T obj = CreateFromKey(key);
			CacheItem<T> ret = Remove(obj);
			return ret;
		}

		public void RemoveKeys(IEnumerable<object> keys)
		{
			IEnumerable<T> objs = keys.Select(key => CreateFromKey(key));
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

		public void RemoveKeys(IEnumerable<object> keys)
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

		public IEnumerable<T> GetKeys(string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			IEnumerable<T> keys = Access.GetKeys(whereCondition, param, commandTimeout);
			return keys;
		}

		public bool Delete(T key, int commandTimeout = 30)
		{
			bool success = Access.Delete(key, commandTimeout);
			Storage.RemoveKey(key);
			return success;
		}

		public int BulkDelete(IEnumerable<T> keys, int commandTimeout = 30)
		{
			int count = Access.BulkDelete(keys, commandTimeout);
			Storage.RemoveKeys(keys);
			return count;
		}

		public IEnumerable<T> DeleteList(string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			IEnumerable<T> keys = Access.GetKeys<T>(whereCondition, param, commandTimeout);
			int count = Access.Delete(whereCondition, param, commandTimeout);
			Storage.RemoveKeys(keys);
			return keys;
		}

		public CacheItem<T> Get(T key, int commandTimeout = 30)
		{
			T obj = Access.Get(key, commandTimeout);
			if (obj == null) {
				CacheItem<T> ret = Storage.RemoveKey(obj);
				return null;
			}
			CacheItem<T> value = Storage.AddOrUpdate(obj);
			return value;
		}

		public int Delete(string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			IEnumerable<T> keys = DeleteList(whereCondition, param, commandTimeout);
			Storage.RemoveKeys(keys);
			return keys.Count();
		}

		public CacheItem<T> Insert(T obj, int commandTimeout = 30)
		{
			Access.Insert(obj, commandTimeout);
			CacheItem<T> value = Storage.Add(obj);
			return value;
		}

		public bool Update(T obj, int commandTimeout = 30)
		{
			if (!Access.Update(obj, commandTimeout)) {
				Storage.AddOrUpdate(obj);
				return true;
			}
			CacheItem<T> ret = Storage.RemoveKey(obj);
			return false;
		}

		public int BulkUpdate(IEnumerable<T> objs, int commandTimeout = 30)
		{
			int count = Access.BulkUpdate(objs, commandTimeout);
			Storage.AddOrUpdate(objs);
			return count;
		}

		public CacheItem<T> Upsert(T obj, int commandTimeout = 30)
		{
			bool updated = Access.Upsert(obj, commandTimeout);
			CacheItem<T> ret = Storage.AddOrUpdate(obj);
			return ret;
		}

		public IEnumerable<CacheItem<T>> GetList(string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			IEnumerable<T> list = Access.GetList(whereCondition, param, commandTimeout);
			List<CacheItem<T>> result = Storage.AddOrUpdate(list);
			return result;
		}

		public IEnumerable<CacheItem<T>> GetLimit(int limit, string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			IEnumerable<T> list = Access.GetLimit(limit, whereCondition, param, commandTimeout);
			List<CacheItem<T>> result = Storage.AddOrUpdate(list);
			return result;
		}

		public IEnumerable<CacheItem<T>> GetDistinct(Type columnFilter, string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			IEnumerable<T> list = Access.GetDistinct(columnFilter, whereCondition, param, commandTimeout);
			List<CacheItem<T>> result = Storage.AddOrUpdate(list);
			return result;
		}

		public int RecordCount(string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			int count = Access.RecordCount(whereCondition, param, commandTimeout);
			return count;
		}

		public IEnumerable<CacheItem<T>> GetDistinctLimit(int limit, string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			IEnumerable<T> list = Access.GetDistinctLimit(limit, whereCondition, commandTimeout);
			List<CacheItem<T>> result = Storage.AddOrUpdate(list);
			return result;
		}

		public IEnumerable<KeyType> GetKeys<KeyType>(string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			IEnumerable<KeyType> keys = Access.GetKeys<KeyType>(whereCondition, param, commandTimeout);
			return keys;
		}

		public bool Delete<KeyType>(KeyType key, int commandTimeout = 30)
		{
			bool success = Access.Delete<KeyType>(key, commandTimeout);
			T obj = CreateFromKey(key);
			CacheItem<T> ret = Storage.Remove(obj);
			return success;
		}

		public CacheItem<T> Get<KeyType>(KeyType key, int commandTimeout = 30)
		{
			T obj = Access.Get<KeyType>(key, commandTimeout);
			CacheItem<T> ret = Storage.AddOrUpdate(obj);
			return ret;
		}

		public int BulkDelete<KeyType>(IEnumerable<KeyType> keys, int commandTimeout = 30)
		{
			int count = Access.BulkDelete<KeyType>(keys, commandTimeout);
			Storage.RemoveKeys(keys);
			return count;
		}

		public List<CacheItem<T>> BulkInsert(IEnumerable<T> objs, int commandTimeout = 30)
		{
			IEnumerable<T> list;
			if (TableData<T>.AutoKeyProperty != null) {
				long max = Access.GetKeys<long?>("WHERE " + TableData<T>.AutoKeyColumn + " = (SELECT MAX(" + TableData<T>.AutoKeyColumn + ") FROM " + TableData<T>.TableName + ")").FirstOrDefault() ?? int.MinValue;
				Access.BulkInsert(objs, commandTimeout);
				list = Access.GetList("WHERE " + TableData<T>.AutoKeyColumn + " > " + max);
			}
			else {
				Access.BulkInsert(objs, commandTimeout);
				list = TableData<T>.InsertKeyProperties.Count != 0
					? Access.BulkGet(objs, commandTimeout)
					: objs;
			}
			List<CacheItem<T>> result = Storage.Add(list);
			return result;
		}

		public List<CacheItem<T>> BulkGet(IEnumerable<T> objs, int commandTimeout = 30)
		{
			List<T> list = Access.BulkGet(objs, commandTimeout);
			List<CacheItem<T>> result = Storage.AddOrUpdate(list);
			return result;
		}

		public List<CacheItem<T>> BulkGet<KeyType>(IEnumerable<KeyType> keys, int commandTimeout = 30)
		{
			List<T> list = Access.BulkGet<KeyType>(keys, commandTimeout);
			List<CacheItem<T>> result = Storage.AddOrUpdate(list);
			return result;
		}
		#endregion ICacheTable<T, CacheItem<T>>
	}
}
