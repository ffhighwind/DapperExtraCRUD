#region License
// Released under MIT License 
// License: https://opensource.org/licenses/MIT
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
using System.Collections.Generic;
using Dapper.Extra.Cache.Internal;
using Dapper.Extra.Utilities;
using Fasterflect;
using System.Collections.Concurrent;
using System.Linq;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Dapper.Extra.Cache
{
	/// <summary>
	/// A cache for objects in a database.
	/// </summary>
	/// <typeparam name="T">The table type.</typeparam>
	/// <typeparam name="R">The cached item type.</typeparam>
	public sealed class DbCacheTable<T, R> : ICacheTable<T, R>, ICacheTable
		where T : class
		where R : CacheItem<T>, new()
	{
		internal DbCacheTable(string connectionString)
		{
			ConnectionString = connectionString;
			Builder = ExtraCrud.Builder<T>();
			if (Builder.Info.KeyColumns.Count == 0)
				throw new InvalidOperationException(typeof(T).FullName + " is not usable without a valid key.");
			DAO = new DataAccessObject<T>(connectionString);
			AAO = new AutoAccessObject<T>(connectionString);
			Access = AAO;
			AutoCache = new CacheAutoStorage<T, R>();
			Items = AutoCache;
			CreateFromKey = Builder.ObjectFromKey;
			AutoKeyColumn = Builder.Info.AutoKeyColumn;
			AutoSyncInsert = Builder.Queries.InsertAutoSync != null;
			AutoSyncUpdate = Builder.Queries.UpdateAutoSync != null;
		}

		private readonly Func<object, T> CreateFromKey;

		/// <summary>
		/// The cached dictionary of key value pairs.
		/// </summary>
		public ICacheStorage<T, R> Items { get; private set; }

		private readonly CacheAutoStorage<T, R> AutoCache;

		/// <summary>
		/// The access object currently being used by the cache. This can be used if you do not want to store
		/// the results of a query in the cache.
		/// </summary>
		public IAccessObject<T> Access { get; private set; }
		private readonly DataAccessObject<T> DAO;
		private readonly AutoAccessObject<T> AAO;
		private readonly SqlBuilder<T> Builder;
		private readonly SqlColumn AutoKeyColumn;
		private readonly bool AutoSyncInsert;
		private readonly bool AutoSyncUpdate;
		private readonly string ConnectionString;

		/// <summary>
		/// The current transaction for the cache.
		/// </summary>
		/// <returns>The current transaction for the cache if it exists; otherwise null.</returns>
		public DbCacheTransaction Transaction { get; private set; }

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>An enumerator that iterates through the collection.</returns>
		public IEnumerator<R> GetEnumerator()
		{
			return Items.Values.GetEnumerator();
		}

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>An enumerator that iterates through the collection.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return Items.Values.GetEnumerator();
		}

		/// <summary>
		/// Returns a cached object if it exists, otherwise null.
		/// </summary>
		/// <param name="key">The object to select.</param>
		/// <returns>The object if it exists in the cache; otherwise null.</returns>
		public R TryGet(T key)
		{
			if (key == null)
				return null;
			Items.TryGetValue(key, out R value);
			return value;
		}

		/// <summary>
		/// Returns a cached object by key if it exists, otherwise null.
		/// </summary>
		/// <param name="key">The key of the row to select.</param>
		/// <returns>The object if it exists in the cache; otherwise null.</returns>
		public R TryGet(object key)
		{
			if (key == null)
				return null;
			T obj = CreateFromKey(key);
			Items.TryGetValue(obj, out R value);
			return value;
		}

		/// <summary>
		/// Attempts to remove an object matching a key.
		/// </summary>
		/// <param name="key">The key to remove.</param>
		/// <returns>True if the object was removed; false otherwise.</returns>
		public bool RemoveKey(object key)
		{
			bool success = Items.RemoveKey(key);
			return success;
		}

		/// <summary>
		/// Removes the objects matching the specified keys from the cache.
		/// </summary>
		/// <param name="keys">The keys of the objects to remove.</param>
		public void RemoveKeys(IEnumerable<object> keys)
		{
			Items.RemoveKeys(keys);
		}

		/// <summary>
		/// Removes the object from the cache.
		/// </summary>
		/// <param name="value">The object to remove.</param>
		public bool Remove(T value)
		{
			bool success = Items.Remove(value);
			return success;
		}

		/// <summary>
		/// Removes the objects from the cache.
		/// </summary>
		/// <param name="values">The objects to remove.</param>
		public void Remove(IEnumerable<T> values)
		{
			Items.Remove(values);
		}

		/// <summary>
		/// Removes the objects from the cache matching the predicate.
		/// </summary>
		/// <param name="predicate">The function that determines what to remove.</param>
		public void Remove(Func<R, bool> predicate)
		{
			foreach (R item in Items.Values.Where(predicate).ToList()) {
				Items.Remove(item.CacheValue);
			}
		}

		private long MaxAutoKey()
		{
			IEnumerable<long> max = Access.GetKeys<long>($"WHERE {AutoKeyColumn.ColumnName} = (SELECT MAX({AutoKeyColumn.ColumnName}) FROM {Info.TableName})");
			return max.Any() ? max.First() : long.MinValue;
		}

		/// <summary>
		/// Returns a cached object if it exists, otherwise it calls <see cref="Get(T, int)"/>.
		/// </summary>
		/// <param name="obj">The object to select.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The selected row if it exists; otherwise null.</returns>
		public R this[T obj, int commandTimeout = 30] {
			get {
				if (obj == null)
					return null;
				if (!Items.TryGetValue(obj, out R value)) {
					value = Get(obj, commandTimeout);
				}
				return value;
			}
		}

		/// <summary>
		/// Returns a cached object by key if it exists, otherwise it calls <see cref="Get(object, int)"/>.
		/// </summary>
		/// <param name="key">The key of the row to select.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The selected row if it exists; otherwise null.</returns>
		public R this[object key, int commandTimeout = 30] {
			get {
				if (key == null)
					return null;
				T obj = CreateFromKey(key);
				R ret = this[obj, commandTimeout];
				return ret;
			}
		}

		#region ICacheTable
		/// <summary>
		/// Begins a transaction.
		/// </summary>
		/// <returns>The transaction.</returns>
		public DbCacheTransaction BeginTransaction()
		{
			try {
				DAO.Connection.Open();
				DAO.Transaction = DAO.Connection.BeginTransaction();
				DbCacheTransaction transaction = new DbCacheTransaction(DAO.Transaction);
				BeginTransaction(transaction);
				return transaction;
			}
			catch (Exception ex) {
				DAO.Transaction = null;
				Transaction = null;
				if (DAO.Connection.State == ConnectionState.Open) {
					DAO.Connection.Close();
				}
				throw;
			}
		}

		/// <summary>
		/// Adds the table to a transaction.
		/// </summary>
		/// <param name="transaction">The transaction</param>
		public void BeginTransaction(DbCacheTransaction transaction)
		{
			if (Transaction != null)
				throw new InvalidOperationException("Cache is already part of a transaction.");
			if (transaction.Connection.State != ConnectionState.Open)
				throw new InvalidOperationException("The transaction is closed.");
			DAO.Connection = transaction.Transaction.Connection;
			DAO.Transaction = transaction.Transaction;
			CacheTransactionStorage<T, R> storage = new CacheTransactionStorage<T, R>(Builder, AutoCache.Cache, transaction, CloseTransaction);
			Items = storage;
			transaction.TransactionStorage.Add(storage);
			Access = DAO;
			Transaction = transaction;
		}

		private void CloseTransaction()
		{
			if (Transaction != null) {
				IDbConnection connection = DAO.Transaction?.Connection;
				IDbTransaction transaction = DAO.Transaction;
				Transaction = null;
				DAO.Transaction = null;
				DAO.Connection = new SqlConnection(ConnectionString);
				Items = AutoCache;
				Access = AAO;
				transaction?.Dispose();
				if (connection != null && connection.State == ConnectionState.Open) {
					connection.Close();
				}
			}
		}

		/// <summary>
		/// The table information.
		/// </summary>
		public SqlTypeInfo Info => Builder.Info;
		#endregion ICacheTable

		#region Bulk

		/// <summary>
		/// Deletes the rows with the given keys.
		/// </summary>
		/// <param name="keys">The keys for the rows to delete.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of deleted rows.</returns>
		public int BulkDelete(IEnumerable<object> keys, int commandTimeout = 30)
		{
			int count = Access.BulkDelete(keys, commandTimeout);
			Items.RemoveKeys(keys);
			return count;
		}

		/// <summary>
		/// Deletes the given rows.
		/// </summary>
		/// <param name="objs">The objects to delete.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of deleted rows.</returns>
		public int BulkDelete(IEnumerable<T> objs, int commandTimeout = 30)
		{
			int count = Access.BulkDelete(objs, commandTimeout);
			Items.Remove(objs);
			return count;
		}

		/// <summary>
		/// Selects the rows with the given keys.
		/// </summary>
		/// <param name="keys">The keys of the rows to select.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows with the given keys.</returns>
		public IEnumerable<R> BulkGet(IEnumerable<object> keys, int commandTimeout = 30)
		{
			IEnumerable<T> list = Access.BulkGet(keys, commandTimeout);
			IEnumerable<R> result = Items.Add(list);
			return result;
		}

		/// <summary>
		/// Selects the rows with the given keys.
		/// </summary>
		/// <param name="objs">The objects to select.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given keys.</returns>
		public IEnumerable<R> BulkGet(IEnumerable<T> objs, int commandTimeout = 30)
		{
			IEnumerable<T> list = Access.BulkGet(objs, commandTimeout);
			IEnumerable<R> result = Items.Add(list);
			return result;
		}

		/// <summary>
		/// Inserts the given rows.
		/// </summary>
		/// <param name="objs">The objects to insert.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		public void BulkInsert(IEnumerable<T> objs, int commandTimeout = 30)
		{
			if (AutoKeyColumn != null) {
				long maxAutoKey = MaxAutoKey();
				Access.BulkInsert(objs, commandTimeout);
				GetList($"WHERE {AutoKeyColumn.ColumnName} > {maxAutoKey}", commandTimeout);
			}
			else {
				Access.BulkInsert(objs, commandTimeout);
				if (AutoSyncInsert)
					BulkGet(objs);
				else
					Items.Add(objs);
			}
		}

		/// <summary>
		/// Inserts the given rows if they do not exist.
		/// </summary>
		/// <param name="objs">The objects to insert.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of rows inserted.</returns>
		public int BulkInsertIfNotExists(IEnumerable<T> objs, int commandTimeout = 30)
		{
			int count;
			if (AutoKeyColumn != null) {
				long maxAutoKey = MaxAutoKey();
				count = Access.BulkInsertIfNotExists(objs, commandTimeout);
				GetList($"WHERE {AutoKeyColumn.ColumnName} > {maxAutoKey}", commandTimeout);
			}
			else {
				count = Access.BulkInsertIfNotExists(objs, commandTimeout);
				BulkGet(objs);
			}
			return count;
		}

		/// <summary>
		/// Updates the given rows.
		/// </summary>
		/// <param name="objs">The objects to update.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of updated rows.</returns>
		public int BulkUpdate(IEnumerable<T> objs, int commandTimeout = 30)
		{
			int count = Access.BulkUpdate(objs, commandTimeout);
			if (AutoSyncUpdate)
				BulkGet(objs);
			else {
				foreach (T obj in objs) {
					if (Items.TryGetValue(obj, out R item)) {
						item.CacheValue = obj;
					}
				}
			}
			return count;
		}

		/// <summary>
		/// Upserts the given rows.
		/// </summary>
		/// <param name="objs">The objects to upsert.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of upserted rows.</returns>
		public int BulkUpsert(IEnumerable<T> objs, int commandTimeout = 30)
		{
			int count;
			if (AutoKeyColumn != null) {
				long maxAutoKey = MaxAutoKey();
				Items.Add(objs);
				count = Access.BulkUpsert(objs, commandTimeout);
				GetList($"WHERE {AutoKeyColumn.ColumnName} > {maxAutoKey}", commandTimeout);
			}
			else {
				count = Access.BulkUpsert(objs, commandTimeout);
				if (AutoSyncInsert || AutoSyncUpdate) {
					BulkGet(objs, commandTimeout);
				}
				else {
					Items.Add(objs);
				}
			}
			return count;
		}

		#endregion Bulk

		#region Sync

		/// <summary>
		/// Deletes the row with the given key.
		/// </summary>
		/// <param name="key">The key of the row to delete.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the row was deleted; false otherwise.</returns>
		public bool Delete(object key, int commandTimeout = 30)
		{
			bool success = Access.Delete(key, commandTimeout);
			_ = Items.RemoveKey(key);
			return success;
		}

		/// <summary>
		/// Deletes the given row.
		/// </summary>
		/// <param name="obj">The object to delete.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the row was deleted; false otherwise.</returns>
		public bool Delete(T obj, int commandTimeout = 30)
		{
			bool success = Access.Delete(obj, commandTimeout);
			_ = Items.Remove(obj);
			return success;
		}

		/// <summary>
		/// Deletes the rows that match the given condition.
		/// </summary>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of deleted rows.</returns>
		public int DeleteList(string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			List<T> keys;
			if (string.IsNullOrWhiteSpace(whereCondition)) {
				Items.Clear();
				keys = new List<T>();
			}
			else
				keys = Access.GetKeys(whereCondition, param, true, commandTimeout).AsList();
			int count = Access.DeleteList(whereCondition, param, commandTimeout);
			Items.Remove(keys);
			return count;
		}

		/// <summary>
		/// Selects the row with the given key.
		/// </summary>
		/// <param name="key">The key of the row to select.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The row with the given key.</returns>
		public R Get(object key, int commandTimeout = 30)
		{
			T obj = Access.Get(key, commandTimeout);
			R item;
			if (obj == null) {
				item = null;
				Items.Remove(obj);
			}
			else {
				item = Items.Add(obj);
			}
			return item;
		}

		/// <summary>
		/// Selects a row.
		/// </summary>
		/// <param name="obj">The object to select.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The selected row if it exists; otherwise null.</returns>
		public R Get(T obj, int commandTimeout = 30)
		{
			obj = Access.Get(obj, commandTimeout);
			R item;
			if (obj == null) {
				item = null;
				Items.Remove(obj);
			}
			else {
				item = Items.Add(obj);
			}
			return item;
		}

		/// <summary>
		/// Selects the rows that match the given condition.
		/// </summary>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public IEnumerable<T> GetDistinct(Type columnFilter, string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			List<T> list = Access.GetDistinct(columnFilter, whereCondition, param, true, commandTimeout).AsList();
			return list;
		}

		/// <summary>
		/// Selects the rows that match the given condition.
		/// </summary>
		/// <param name="limit">The maximum number of rows.</param>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public IEnumerable<T> GetDistinctLimit(int limit, Type columnFilter, string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			List<T> list = Access.GetDistinctLimit(limit, columnFilter, whereCondition, param, true, commandTimeout).AsList();
			return list;
		}

		/// <summary>
		/// Selects the rows with the given keys.
		/// </summary>
		/// <typeparam name="KeyType">The key type.</typeparam>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The keys that match the given condition.</returns>
		public IEnumerable<KeyType> GetKeys<KeyType>(string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			List<KeyType> keys = Access.GetKeys<KeyType>(whereCondition, param, true, commandTimeout).AsList();
			return keys;
		}

		/// <summary>
		/// Selects the keys that match the given condition.
		/// </summary>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The keys that match the given condition.</returns>
		public IEnumerable<T> GetKeys(string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			List<T> keys = Access.GetKeys(whereCondition, param, true, commandTimeout).AsList();
			return keys;
		}

		/// <summary>
		/// Selects a limited number of rows that match the given condition.
		/// </summary>
		/// <param name="limit">The maximum number of rows.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The limited number of rows that match the given condition.</returns>
		public IEnumerable<R> GetLimit(int limit, string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			List<T> list = Access.GetLimit(limit, whereCondition, param, true, commandTimeout).AsList();
			List<R> result = Items.Add(list);
			return result;
		}

		/// <summary>
		/// Selects a limited number of rows that match the given condition.
		/// </summary>
		/// <param name="limit">The maximum number of rows.</param>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>A limited number of rows that match the given condition.</returns>
		public IEnumerable<T> GetLimit(int limit, Type columnFilter, string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			List<T> list = Access.GetLimit(limit, columnFilter, whereCondition, param, true, commandTimeout).AsList();
			return list;
		}

		/// <summary>
		/// Selects the rows that match the given condition.
		/// </summary>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public IEnumerable<R> GetList(string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			List<T> list = Access.GetList(whereCondition, param, true, commandTimeout).AsList();
			List<R> result = Items.Add(list);
			return result;
		}

		/// <summary>
		/// Selects the rows that match the given condition.
		/// </summary>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public IEnumerable<T> GetList(Type columnFilter, string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			List<T> list = Access.GetList(columnFilter, whereCondition, param, true, commandTimeout).AsList();
			return list;
		}

		/// <summary>
		/// Inserts a row.
		/// </summary>
		/// <param name="obj">The object to insert.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		public R Insert(T obj, int commandTimeout = 30)
		{
			Access.Insert(obj, commandTimeout);
			R item = Items.Add(obj);
			return item;
		}

		/// <summary>
		/// Inserts a row if it does not exist.
		/// </summary>
		/// <param name="obj">The object to insert.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the the row was inserted; false otherwise.</returns>
		public R InsertIfNotExists(T obj, int commandTimeout = 30)
		{
			if (!Access.InsertIfNotExists(obj, commandTimeout))
				return null;
			R item = Items.Add(obj);
			return item;
		}

		/// <summary>
		/// Counts the number of rows that match the given condition.
		/// </summary>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of rows that match the given condition.</returns>
		public int RecordCount(string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			int count = Access.RecordCount(whereCondition, param, commandTimeout);
			return count;
		}

		/// <summary>
		/// Truncates all rows.
		/// </summary>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		public void Truncate(int commandTimeout = 30)
		{
			Access.Truncate(commandTimeout);
			Items.Clear();
		}

		/// <summary>
		/// ObjectMappers for Update.
		/// </summary>
		private readonly ConcurrentDictionary<Type, MapperPair> mappers = new ConcurrentDictionary<Type, MapperPair>();

		internal class MapperPair
		{
			public ObjectMapper KeyMapper;
			public ObjectMapper ValueMapper;
		}

		/// <summary>
		/// Updates a row.
		/// </summary>
		/// <param name="obj">The object to update.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the row was updated; false otherwise.</returns>
		public bool Update(object obj, int commandTimeout = 30)
		{
			bool success = Access.Update(obj, commandTimeout);
			if (success) {
				Type type = obj.GetType();
				if (!mappers.TryGetValue(type, out MapperPair mapperPair)) {
					mapperPair = new MapperPair();
					mapperPair.KeyMapper = Reflect.Mapper(type, typeof(T), Info.KeyColumns.Select(c => c.ColumnName).ToArray());
					string[] props = Builder.GetSharedColumns(type, Info.UpdateColumns).Select(c => c.ColumnName).ToArray();
					mapperPair.ValueMapper = Reflect.Mapper(type, typeof(T), props);
					_ = mappers.TryAdd(type, mapperPair);
				}
				T value = (T) System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(T));
				mapperPair.KeyMapper(obj, value);
				if (Items.TryGetValue(value, out R item)) {
					mapperPair.ValueMapper(obj, item.CacheValue);
				}
			}
			return success;
		}


		/// <summary>
		/// Updates a row.
		/// </summary>
		/// <param name="obj">The object to update.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the row was updated; false otherwise.</returns>
		public bool Update(T obj, int commandTimeout = 30)
		{
			bool success = Access.Update(obj, commandTimeout);
			if (success) {
				_ = Items.Add(obj);
			}
			return success;
		}

		/// <summary>
		/// Upserts a row.
		/// </summary>
		/// <param name="obj">The object to upsert.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the object was upserted; false otherwise.</returns>
		public bool Upsert(T obj, int commandTimeout = 30)
		{
			bool success = Access.Upsert(obj, commandTimeout);
			_ = Items.Add(obj);
			return success;
		}

		#endregion Sync

		#region Async

		/// <summary>
		/// Deletes the row with the given key asynchronously.
		/// </summary>
		/// <param name="key">The key of the row to delete.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the row was deleted; false otherwise.</returns>
		public async Task<bool> DeleteAsync(object key, int commandTimeout = 30)
		{
			return await Task.Run(() => Delete(key, commandTimeout));
		}

		/// <summary>
		/// Deletes the given row asynchronously.
		/// </summary>
		/// <param name="obj">The object to delete.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the row was deleted; false otherwise.</returns>
		public async Task<bool> DeleteAsync(T obj, int commandTimeout = 30)
		{
			return await Task.Run(() => Delete(obj, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Selects the row with the given key asynchronously.
		/// </summary>
		/// <param name="key">The key of the row to select.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The row with the given key.</returns>
		public async Task<R> GetAsync(object key, int commandTimeout = 30)
		{
			return await Task.Run(() => Get(key, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Selects a row asynchronously.
		/// </summary>
		/// <param name="obj">The object to select.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The selected row if it exists; otherwise null.</returns>
		public async Task<R> GetAsync(T obj, int commandTimeout = 30)
		{
			return await Task.Run(() => Get(obj, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Selects the rows that match the given condition asynchronously.
		/// </summary>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public async Task<IEnumerable<T>> GetDistinctAsync(Type columnFilter, string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			return await Task.Run(() => GetDistinct(columnFilter, whereCondition, param, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Selects the rows that match the given condition asynchronously.
		/// </summary>
		/// <param name="limit">The maximum number of rows.</param>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public async Task<IEnumerable<T>> GetDistinctLimitAsync(int limit, Type columnFilter, string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			return await Task.Run(() => GetDistinctLimit(limit, columnFilter, whereCondition, param, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Selects the rows with the given keys asynchronously.
		/// </summary>
		/// <typeparam name="KeyType">The key type.</typeparam>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The keys that match the given condition.</returns>
		public async Task<IEnumerable<KeyType>> GetKeysAsync<KeyType>(string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			return await Task.Run(() => GetKeys<KeyType>(whereCondition, param, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Selects the keys that match the given condition asynchronously.
		/// </summary>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The keys that match the given condition.</returns>
		public async Task<IEnumerable<T>> GetKeysAsync(string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			return await Task.Run(() => GetKeys(whereCondition, param, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Selects a limited number of rows that match the given condition asynchronously.
		/// </summary>
		/// <param name="limit">The maximum number of rows.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The limited number of rows that match the given condition.</returns>
		public async Task<IEnumerable<R>> GetLimitAsync(int limit, string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			return await Task.Run(() => GetLimit(limit, whereCondition, param, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Selects a limited number of rows that match the given condition asynchronously.
		/// </summary>
		///  <param name="limit">The maximum number of rows.</param>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>A limited number of rows that match the given condition.</returns>
		public async Task<IEnumerable<T>> GetLimitAsync(int limit, Type columnFilter, string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			return await Task.Run(() => GetLimit(limit, columnFilter, whereCondition, param, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Selects the rows that match the given condition asynchronously.
		/// </summary>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public async Task<IEnumerable<R>> GetListAsync(string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			return await Task.Run(() => GetList(whereCondition, param, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Selects the rows that match the given condition asynchronously.
		/// </summary>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public async Task<IEnumerable<T>> GetListAsync(Type columnFilter, string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			return await Task.Run(() => GetList(columnFilter, whereCondition, param, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Inserts a row asynchronously.
		/// </summary>
		/// <param name="obj">The object to insert.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		public async Task<R> InsertAsync(T obj, int commandTimeout = 30)
		{
			return await Task.Run(() => Insert(obj, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Inserts a row if it does not exist asynchronously.
		/// </summary>
		/// <param name="obj">The object to insert.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the the row was inserted; false otherwise.</returns>
		public async Task<R> InsertIfNotExistsAsync(T obj, int commandTimeout = 30)
		{
			return await Task.Run(() => InsertIfNotExists(obj, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Counts the number of rows that match the given condition asynchronously.
		/// </summary>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of rows that match the given condition.</returns>
		public async Task<int> RecordCountAsync(string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			return await Task.Run(() => RecordCount(whereCondition, param, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Updates a row asynchronously.
		/// </summary>
		/// <param name="obj">The object to update.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the row was updated; false otherwise.</returns>
		public async Task<bool> UpdateAsync(object obj, int commandTimeout = 30)
		{
			return await Task.Run(() => Update(obj, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Updates a row asynchronously.
		/// </summary>
		/// <param name="obj">The object to update.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the row was updated; false otherwise.</returns>
		public async Task<bool> UpdateAsync(T obj, int commandTimeout = 30)
		{
			return await Task.Run(() => Update(obj, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Upserts a row asynchronously.
		/// </summary>
		/// <param name="obj">The object to upsert.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the object was upserted; false otherwise.</returns>
		public async Task<bool> UpsertAsync(T obj, int commandTimeout = 30)
		{
			return await Task.Run(() => Upsert(obj, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Deletes the rows that match the given condition asynchronously.
		/// </summary>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of deleted rows.</returns>
		public async Task<int> DeleteListAsync(string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			return await Task.Run(() => DeleteList(whereCondition, param, commandTimeout)).ConfigureAwait(false);
		}

		#endregion Async
	}
}
