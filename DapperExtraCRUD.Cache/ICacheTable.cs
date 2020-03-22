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
using System.Threading.Tasks;

namespace Dapper.Extra.Cache
{
	/// <summary>
	/// Non-generic interface for <see cref="DbCacheTable{T, R}"/>.
	/// </summary>
	public interface ICacheTable
	{
		/// <summary>
		/// Begins a transaction.
		/// </summary>
		/// <returns>The transaction.</returns>
		DbCacheTransaction BeginTransaction();

		/// <summary>
		/// Adds the table to a transaction
		/// </summary>
		/// <param name="transaction">The transaction</param>
		void BeginTransaction(DbCacheTransaction transaction);

		/// <summary>
		/// The current transaction for the cache.
		/// </summary>
		/// <returns>The current transaction for the cache if it exists; otherwise null.</returns>
		DbCacheTransaction Transaction { get; }

		/// <summary>
		/// The table information.
		/// </summary>
		SqlTypeInfo Info { get; }

		/// <summary>
		/// Attempts to remove an object matching a key.
		/// </summary>
		/// <param name="key">The key to remove.</param>
		/// <returns>True if the object was removed; false otherwise.</returns>
		bool RemoveKey(object key);

		/// <summary>
		/// Removes the objects matching the specified keys from the cache.
		/// </summary>
		/// <param name="keys">The keys of the objects to remove.</param>
		void RemoveKeys(IEnumerable<object> keys);
	}

	/// <summary>
	/// The interface for <see cref="DbCacheTable{T, R}"/>.
	/// </summary>
	/// <typeparam name="T">The table type.</typeparam>
	/// <typeparam name="R">The cached item type.</typeparam>
	public interface ICacheTable<T, R> : IEnumerable<R>
		where T : class
		where R : CacheItem<T>, new()
	{
		/// <summary>
		/// The cached dictionary of key value pairs.
		/// </summary>
		ICacheStorage<T, R> Items { get; }

		/// <summary>
		/// Returns a cached object if it exists, otherwise null.
		/// </summary>
		/// <param name="key">The object to select.</param>
		/// <returns>The object if it exists in the cache; otherwise null.</returns>
		R TryGet(T key);

		/// <summary>
		/// Returns a cached object by key if it exists, otherwise null.
		/// </summary>
		/// <param name="key">The key of the row to select.</param>
		/// <returns>The object if it exists in the cache; otherwise null.</returns>
		R TryGet(object key);

		/// <summary>
		/// Removes the object from the cache.
		/// </summary>
		/// <param name="value">The object to remove.</param>
		bool Remove(T value);

		/// <summary>
		/// Removes the objects from the cache.
		/// </summary>
		/// <param name="values">The objects to remove.</param>
		void Remove(IEnumerable<T> values);

		/// <summary>
		/// Removes the objects from the cache matching the predicate.
		/// </summary>
		/// <param name="predicate">The function that determines what to remove.</param>
		void Remove(Func<R, bool> predicate);

		/// <summary>
		/// Returns a cached object by key if it exists, otherwise it calls <see cref="Get(object, int)"/>.
		/// </summary>
		/// <param name="key">The key of the row to select.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The selected row if it exists; otherwise null.</returns>
		R this[object key, int commandTimeout = 30] { get; }

		/// <summary>
		/// Returns a cached object if it exists, otherwise it calls <see cref="Get(T, int)"/>.
		/// </summary>
		/// <param name="obj">The object to select.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The selected row if it exists; otherwise null.</returns>
		R this[T obj, int commandTimeout = 30] { get; }

		#region Bulk

		/// <summary>
		/// Deletes the rows with the given keys.
		/// </summary>
		/// <param name="keys">The keys for the rows to delete.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of deleted rows.</returns>
		int BulkDelete(IEnumerable<object> keys, int commandTimeout = 30);

		/// <summary>
		/// Deletes the given rows.
		/// </summary>
		/// <param name="objs">The objects to delete.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of deleted rows.</returns>
		int BulkDelete(IEnumerable<T> objs, int commandTimeout = 30);

		/// <summary>
		/// Selects the rows with the given keys.
		/// </summary>
		/// <param name="keys">The keys of the rows to select.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows with the given keys.</returns>
		IEnumerable<R> BulkGet(IEnumerable<object> keys, int commandTimeout = 30);

		/// <summary>
		/// Selects the rows with the given keys.
		/// </summary>
		/// <param name="objs">The objects to select.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given keys.</returns>
		IEnumerable<R> BulkGet(IEnumerable<T> objs, int commandTimeout = 30);

		/// <summary>
		/// Inserts the given rows.
		/// </summary>
		/// <param name="objs">The objects to insert.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		void BulkInsert(IEnumerable<T> objs, int commandTimeout = 30);

		/// <summary>
		/// Inserts the given rows if they do not exist.
		/// </summary>
		/// <param name="objs">The objects to insert.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of rows inserted.</returns>
		int BulkInsertIfNotExists(IEnumerable<T> objs, int commandTimeout = 30);

		/// <summary>
		/// Updates the given rows.
		/// </summary>
		/// <param name="objs">The objects to update.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of updated rows.</returns>
		int BulkUpdate(IEnumerable<T> objs, int commandTimeout = 30);

		/// <summary>
		/// Upserts the given rows.
		/// </summary>
		/// <param name="objs">The objects to upsert.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of upserted rows.</returns>
		int BulkUpsert(IEnumerable<T> objs, int commandTimeout = 30);

		#endregion Bulk

		#region Sync

		/// <summary>
		/// Deletes the row with the given key.
		/// </summary>
		/// <param name="key">The key of the row to delete.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the row was deleted; false otherwise.</returns>
		bool Delete(object key, int commandTimeout = 30);

		/// <summary>
		/// Deletes the given row.
		/// </summary>
		/// <param name="obj">The object to delete.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the row was deleted; false otherwise.</returns>
		bool Delete(T obj, int commandTimeout = 30);

		/// <summary>
		/// Deletes the rows that match the given condition.
		/// </summary>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of deleted rows.</returns>
		int DeleteList(string whereCondition = "", object param = null, int commandTimeout = 30);

		/// <summary>
		/// Selects the row with the given key.
		/// </summary>
		/// <param name="key">The key of the row to select.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The row with the given key.</returns>
		R Get(object key, int commandTimeout = 30);

		/// <summary>
		/// Selects a row.
		/// </summary>
		/// <param name="obj">The object to select.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The selected row if it exists; otherwise null.</returns>
		R Get(T obj, int commandTimeout = 30);

		/// <summary>
		/// Selects the rows that match the given condition.
		/// </summary>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		IEnumerable<T> GetDistinct(Type columnFilter, string whereCondition = "", object param = null, int commandTimeout = 30);

		/// <summary>
		/// Selects the rows that match the given condition.
		/// </summary>
		/// <param name="limit">The maximum number of rows.</param>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		IEnumerable<T> GetDistinctLimit(int limit, Type columnFilter, string whereCondition = "", object param = null, int commandTimeout = 30);

		/// <summary>
		/// Selects the rows with the given keys.
		/// </summary>
		/// <typeparam name="KeyType">The key type.</typeparam>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The keys that match the given condition.</returns>
		IEnumerable<KeyType> GetKeys<KeyType>(string whereCondition = "", object param = null, int commandTimeout = 30);

		/// <summary>
		/// Selects the keys that match the given condition.
		/// </summary>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The keys that match the given condition.</returns>
		IEnumerable<T> GetKeys(string whereCondition = "", object param = null, int commandTimeout = 30);

		/// <summary>
		/// Selects a limited number of rows that match the given condition.
		/// </summary>
		/// <param name="limit">The maximum number of rows.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The limited number of rows that match the given condition.</returns>
		IEnumerable<R> GetLimit(int limit, string whereCondition = "", object param = null, int commandTimeout = 30);

		/// <summary>
		/// Selects a limited number of rows that match the given condition.
		/// </summary>
		///  <param name="limit">The maximum number of rows.</param>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>A limited number of rows that match the given condition.</returns>
		IEnumerable<T> GetLimit(int limit, Type columnFilter, string whereCondition = "", object param = null,  int commandTimeout = 30);

		/// <summary>
		/// Selects the rows that match the given condition.
		/// </summary>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		IEnumerable<R> GetList(string whereCondition = "", object param = null, int commandTimeout = 30);

		/// <summary>
		/// Selects the rows that match the given condition.
		/// </summary>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		IEnumerable<T> GetList(Type columnFilter, string whereCondition = "", object param = null, int commandTimeout = 30);

		/// <summary>
		/// Inserts a row.
		/// </summary>
		/// <param name="obj">The object to insert.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		R Insert(T obj, int commandTimeout = 30);

		/// <summary>
		/// Inserts a row if it does not exist.
		/// </summary>
		/// <param name="obj">The object to insert.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the the row was inserted; false otherwise.</returns>
		R InsertIfNotExists(T obj, int commandTimeout = 30);

		/// <summary>
		/// Counts the number of rows that match the given condition.
		/// </summary>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of rows that match the given condition.</returns>
		int RecordCount(string whereCondition = "", object param = null, int commandTimeout = 30);

		/// <summary>
		/// Truncates all rows.
		/// </summary>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		void Truncate(int commandTimeout = 30);

		/// <summary>
		/// Updates a row.
		/// </summary>
		/// <param name="obj">The object to update.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the row was updated; false otherwise.</returns>
		bool Update(object obj, int commandTimeout = 30);

		/// <summary>
		/// Updates a row.
		/// </summary>
		/// <param name="obj">The object to update.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the row was updated; false otherwise.</returns>
		bool Update(T obj, int commandTimeout = 30);

		/// <summary>
		/// Upserts a row.
		/// </summary>
		/// <param name="obj">The object to upsert.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the object was upserted; false otherwise.</returns>
		bool Upsert(T obj, int commandTimeout = 30);

		#endregion Sync

		#region Async

		/// <summary>
		/// Deletes the row with the given key asynchronously.
		/// </summary>
		/// <param name="key">The key of the row to delete.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the row was deleted; false otherwise.</returns>
		Task<bool> DeleteAsync(object key, int commandTimeout = 30);

		/// <summary>
		/// Deletes the given row asynchronously.
		/// </summary>
		/// <param name="obj">The object to delete.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the row was deleted; false otherwise.</returns>
		Task<bool> DeleteAsync(T obj, int commandTimeout = 30);

		/// <summary>
		/// Deletes the rows that match the given condition asynchronously.
		/// </summary>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of deleted rows.</returns>
		Task<int> DeleteListAsync(string whereCondition = "", object param = null, int commandTimeout = 30);

		/// <summary>
		/// Selects the row with the given key asynchronously.
		/// </summary>
		/// <param name="key">The key of the row to select.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The row with the given key.</returns>
		Task<R> GetAsync(object key, int commandTimeout = 30);

		/// <summary>
		/// Selects a row asynchronously.
		/// </summary>
		/// <param name="obj">The object to select.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The selected row if it exists; otherwise null.</returns>
		Task<R> GetAsync(T obj, int commandTimeout = 30);

		/// <summary>
		/// Selects the rows that match the given condition asynchronously.
		/// </summary>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		Task<IEnumerable<T>> GetDistinctAsync(Type columnFilter, string whereCondition = "", object param = null, int commandTimeout = 30);

		/// <summary>
		/// Selects the rows that match the given condition asynchronously.
		/// </summary>
		/// <param name="limit">The maximum number of rows.</param>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		Task<IEnumerable<T>> GetDistinctLimitAsync(int limit, Type columnFilter, string whereCondition = "", object param = null, int commandTimeout = 30);

		/// <summary>
		/// Selects the rows with the given keys asynchronously.
		/// </summary>
		/// <typeparam name="KeyType">The key type.</typeparam>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The keys that match the given condition.</returns>
		Task<IEnumerable<KeyType>> GetKeysAsync<KeyType>(string whereCondition = "", object param = null, int commandTimeout = 30);

		/// <summary>
		/// Selects the keys that match the given condition asynchronously.
		/// </summary>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The keys that match the given condition.</returns>
		Task<IEnumerable<T>> GetKeysAsync(string whereCondition = "", object param = null, int commandTimeout = 30);

		/// <summary>
		/// Selects a limited number of rows that match the given condition asynchronously.
		/// </summary>
		/// <param name="limit">The maximum number of rows.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The limited number of rows that match the given condition.</returns>
		Task<IEnumerable<R>> GetLimitAsync(int limit, string whereCondition = "", object param = null, int commandTimeout = 30);

		/// <summary>
		/// Selects a limited number of rows that match the given condition asynchronously.
		/// </summary>
		/// <param name="limit">The maximum number of rows.</param>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>A limited number of rows that match the given condition.</returns>
		Task<IEnumerable<T>> GetLimitAsync(int limit, Type columnFilter, string whereCondition = "", object param = null, int commandTimeout = 30);

		/// <summary>
		/// Selects the rows that match the given condition asynchronously.
		/// </summary>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		Task<IEnumerable<R>> GetListAsync(string whereCondition = "", object param = null, int commandTimeout = 30);

		/// <summary>
		/// Selects the rows that match the given condition asynchronously.
		/// </summary>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		Task<IEnumerable<T>> GetListAsync(Type columnFilter, string whereCondition = "", object param = null, int commandTimeout = 30);

		/// <summary>
		/// Inserts a row asynchronously.
		/// </summary>
		/// <param name="obj">The object to insert.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		Task<R> InsertAsync(T obj, int commandTimeout = 30);

		/// <summary>
		/// Inserts a row if it does not exist asynchronously.
		/// </summary>
		/// <param name="obj">The object to insert.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the the row was inserted; false otherwise.</returns>
		Task<R> InsertIfNotExistsAsync(T obj, int commandTimeout = 30);

		/// <summary>
		/// Counts the number of rows that match the given condition asynchronously.
		/// </summary>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of rows that match the given condition.</returns>
		Task<int> RecordCountAsync(string whereCondition = "", object param = null, int commandTimeout = 30);

		/// <summary>
		/// Updates a row asynchronously.
		/// </summary>
		/// <param name="obj">The object to update.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the row was updated; false otherwise.</returns>
		Task<bool> UpdateAsync(object obj, int commandTimeout = 30);

		/// <summary>
		/// Updates a row asynchronously.
		/// </summary>
		/// <param name="obj">The object to update.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the row was updated; false otherwise.</returns>
		Task<bool> UpdateAsync(T obj, int commandTimeout = 30);

		/// <summary>
		/// Upserts a row asynchronously.
		/// </summary>
		/// <param name="obj">The object to upsert.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the object was upserted; false otherwise.</returns>
		Task<bool> UpsertAsync(T obj, int commandTimeout = 30);

		#endregion Async
	}
}
