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
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Dapper.Extra.Utilities
{
	/// <summary>
	/// Abstract class for an object that interacts with an <see cref="ISqlQueries{T}"/>.
	/// </summary>
	/// <typeparam name="T">The table type.</typeparam>
	public abstract class IAccessObject<T> : IAccessObjectSync<T>, IAccessObjectAsync<T>
		where T : class
	{
		#region Bulk

		/// <summary>
		/// Deletes the rows with the given keys.
		/// </summary>
		/// <param name="keys">The keys for the rows to delete.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of deleted rows.</returns>
		public abstract int BulkDelete(IEnumerable<object> keys, int commandTimeout = 30);

		/// <summary>
		/// Deletes the rows with the given keys.
		/// </summary>
		/// <param name="keys">The keys for the rows to delete.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of deleted rows.</returns>
		public abstract int BulkDelete(IEnumerable<int> keys, int commandTimeout = 30);

		/// <summary>
		/// Deletes the given rows.
		/// </summary>
		/// <param name="objs">The objects to delete.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of deleted rows.</returns>
		public abstract int BulkDelete(IEnumerable<T> objs, int commandTimeout = 30);

		/// <summary>
		/// Deletes the rows with the given keys.
		/// </summary>
		/// <param name="keys">The keys for the rows to delete.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of deleted rows.</returns>
		public async Task<int> BulkDeleteAsync(IEnumerable<object> keys, int commandTimeout = 30)
		{
			return await Task.Run(() => BulkDelete(keys, commandTimeout));
		}

		/// <summary>
		/// Deletes the rows with the given keys.
		/// </summary>
		/// <param name="keys">The keys for the rows to delete.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of deleted rows.</returns>
		public async Task<int> BulkDeleteAsync(IEnumerable<int> keys, int commandTimeout = 30)
		{
			return await Task.Run(() => BulkDelete(keys, commandTimeout));
		}

		/// <summary>
		/// Deletes the given rows.
		/// </summary>
		/// <param name="objs">The objects to delete.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of deleted rows.</returns>
		public async Task<int> BulkDeleteAsync(IEnumerable<T> objs, int commandTimeout = 30)
		{
			return await Task.Run(() => BulkDelete(objs, commandTimeout));
		}

		/// <summary>
		/// Selects the rows with the given keys.
		/// </summary>
		/// <param name="keys">The keys of the rows to select.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows with the given keys.</returns>
		public abstract IEnumerable<T> BulkGet(IEnumerable<object> keys, int commandTimeout = 30);

		/// <summary>
		/// Selects the rows with the given keys.
		/// </summary>
		/// <param name="keys">The keys of the rows to select.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows with the given keys.</returns>
		public abstract IEnumerable<T> BulkGet(IEnumerable<int> keys, int commandTimeout = 30);

		/// <summary>
		/// Selects the rows with the given keys.
		/// </summary>
		/// <param name="objs">The objects to select.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given keys.</returns>
		public abstract IEnumerable<T> BulkGet(IEnumerable<T> objs, int commandTimeout = 30);

		/// <summary>
		/// Selects the rows with the given keys.
		/// </summary>
		/// <param name="keys">The keys of the rows to select.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows with the given keys.</returns>
		public async Task<IEnumerable<T>> BulkGetAsync(IEnumerable<object> keys, int commandTimeout = 30)
		{
			return await Task.Run(() => BulkGet(keys, commandTimeout));
		}

		/// <summary>
		/// Selects the rows with the given keys.
		/// </summary>
		/// <param name="keys">The keys of the rows to select.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows with the given keys.</returns>
		public async Task<IEnumerable<T>> BulkGetAsync(IEnumerable<int> keys, int commandTimeout = 30)
		{
			return await Task.Run(() => BulkGet(keys, commandTimeout));
		}

		/// <summary>
		/// Selects the rows with the given keys.
		/// </summary>
		/// <param name="objs">The objects to select.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given keys.</returns>
		public async Task<IEnumerable<T>> BulkGetAsync(IEnumerable<T> objs, int commandTimeout = 30)
		{
			return await Task.Run(() => BulkGet(objs, commandTimeout));
		}

		/// <summary>
		/// Inserts the given rows.
		/// </summary>
		/// <param name="objs">The objects to insert.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		public abstract void BulkInsert(IEnumerable<T> objs, int commandTimeout = 30);

		/// <summary>
		/// Inserts the given rows.
		/// </summary>
		/// <param name="objs">The objects to insert.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		public async Task BulkInsertAsync(IEnumerable<T> objs, int commandTimeout = 30)
		{
			await Task.Run(() => BulkInsert(objs, commandTimeout));
		}

		/// <summary>
		/// Inserts the given rows if they do not exist.
		/// </summary>
		/// <param name="objs">The objects to insert.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of rows inserted.</returns>
		public abstract int BulkInsertIfNotExists(IEnumerable<T> objs, int commandTimeout = 30);

		/// <summary>
		/// Inserts the given rows if they do not exist.
		/// </summary>
		/// <param name="objs">The objects to insert.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of rows inserted.</returns>
		public async Task<int> BulkInsertIfNotExistsAsync(IEnumerable<T> objs, int commandTimeout = 30)
		{
			return await Task.Run(() => BulkInsertIfNotExists(objs, commandTimeout));
		}

		/// <summary>
		/// Updates the given rows.
		/// </summary>
		/// <param name="objs">The objects to update.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of updated rows.</returns>
		public abstract int BulkUpdate(IEnumerable<T> objs, int commandTimeout = 30);

		/// <summary>
		/// Updates the given rows.
		/// </summary>
		/// <param name="objs">The objects to update.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of updated rows.</returns>
		public async Task<int> BulkUpdateAsync(IEnumerable<T> objs, int commandTimeout = 30)
		{
			return await Task.Run(() => BulkUpdate(objs, commandTimeout));
		}

		/// <summary>
		/// Upserts the given rows.
		/// </summary>
		/// <param name="objs">The objects to upsert.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of upserted rows.</returns>
		public abstract int BulkUpsert(IEnumerable<T> objs, int commandTimeout = 30);

		/// <summary>
		/// Upserts the given rows.
		/// </summary>
		/// <param name="objs">The objects to upsert.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of upserted rows.</returns>
		public async Task<int> BulkUpsertAsync(IEnumerable<T> objs, int commandTimeout = 30)
		{
			return await Task.Run(() => BulkUpsert(objs, commandTimeout));
		}

		#endregion Bulk

		#region Other Methods

		/// <summary>
		/// Deletes the row with the given key.
		/// </summary>
		/// <param name="key">The key of the row to delete.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the row was deleted; false otherwise.</returns>
		public abstract bool Delete(object key, int commandTimeout = 30);

		/// <summary>
		/// Deletes the given row.
		/// </summary>
		/// <param name="obj">The object to delete.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the row was deleted; false otherwise.</returns>
		public abstract bool Delete(T obj, int commandTimeout = 30);

		/// <summary>
		/// Deletes the row with the given key.
		/// </summary>
		/// <param name="key">The key of the row to delete.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the row was deleted; false otherwise.</returns>
		public async Task<bool> DeleteAsync(object key, int commandTimeout = 30)
		{
			return await Task.Run(() => Delete(key, commandTimeout));
		}

		/// <summary>
		/// Deletes the given row.
		/// </summary>
		/// <param name="obj">The object to delete.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the row was deleted; false otherwise.</returns>
		public async Task<bool> DeleteAsync(T obj, int commandTimeout = 30)
		{
			return await Task.Run(() => Delete(obj, commandTimeout));
		}

		/// <summary>
		/// Deletes the rows that match the given condition.
		/// </summary>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of deleted rows.</returns>
		public abstract int DeleteList(string whereCondition = "", object param = null, int commandTimeout = 30);

		/// <summary>
		/// Deletes the rows that match the given condition.
		/// </summary>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of deleted rows.</returns>
		public async Task<int> DeleteListAsync(string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			return await Task.Run(() => DeleteList(whereCondition, param, commandTimeout));
		}

		/// <summary>
		/// Selects the row with the given key.
		/// </summary>
		/// <param name="key">The key of the row to select.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The row with the given key.</returns>
		public abstract T Get(object key, int commandTimeout = 30);

		/// <summary>
		/// Selects a row.
		/// </summary>
		/// <param name="obj">The object to select.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The selected row if it exists; otherwise null.</returns>
		public abstract T Get(T obj, int commandTimeout = 30);

		/// <summary>
		/// Selects the row with the given key.
		/// </summary>
		/// <param name="key">The key of the row to select.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The row with the given key.</returns>
		public async Task<T> GetAsync(object key, int commandTimeout = 30)
		{
			return await Task.Run(() => Get(key, commandTimeout));
		}

		/// <summary>
		/// Selects a row.
		/// </summary>
		/// <param name="obj">The object to select.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The selected row if it exists; otherwise null.</returns>
		public async Task<T> GetAsync(T obj, int commandTimeout = 30)
		{
			return await Task.Run(() => Get(obj, commandTimeout));
		}

		/// <summary>
		/// Selects the rows that match the given condition.
		/// </summary>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public abstract IEnumerable<T> GetDistinct(Type columnFilter, string whereCondition = "", object param = null, bool buffered = true, int commandTimeout = 30);

		/// <summary>
		/// Selects the rows that match the given condition.
		/// </summary>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public async Task<IEnumerable<T>> GetDistinctAsync(Type columnFilter, string whereCondition = "", object param = null, bool buffered = true, int commandTimeout = 30)
		{
			return await Task.Run(() => GetDistinct(columnFilter, whereCondition, param, buffered, commandTimeout));
		}

		/// <summary>
		/// Selects the rows that match the given condition.
		/// </summary>
		/// <param name="limit">The maximum number of rows.</param>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public abstract IEnumerable<T> GetDistinctLimit(int limit, Type columnFilter, string whereCondition = "", object param = null, bool buffered = true, int commandTimeout = 30);

		/// <summary>
		/// Selects the rows that match the given condition.
		/// </summary>
		/// <param name="limit">The maximum number of rows.</param>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public async Task<IEnumerable<T>> GetDistinctLimitAsync(int limit, Type columnFilter, string whereCondition = "", object param = null, bool buffered = true, int commandTimeout = 30)
		{
			return await Task.Run(() => GetDistinctLimit(limit, columnFilter, whereCondition, param, buffered, commandTimeout));
		}

		/// <summary>
		/// Selects the rows with the given keys.
		/// </summary>
		/// <typeparam name="KeyType">The key type.</typeparam>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The keys that match the given condition.</returns>
		public abstract IEnumerable<KeyType> GetKeys<KeyType>(string whereCondition = "", object param = null, bool buffered = true, int commandTimeout = 30);

		/// <summary>
		/// Selects the keys that match the given condition.
		/// </summary>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The keys that match the given condition.</returns>
		public abstract IEnumerable<T> GetKeys(string whereCondition = "", object param = null, bool buffered = true, int commandTimeout = 30);

		/// <summary>
		/// Selects the rows with the given keys.
		/// </summary>
		/// <typeparam name="KeyType">The key type.</typeparam>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The keys that match the given condition.</returns>
		public async Task<IEnumerable<KeyType>> GetKeysAsync<KeyType>(string whereCondition = "", object param = null, bool buffered = true, int commandTimeout = 30)
		{
			return await Task.Run(() => GetKeys<KeyType>(whereCondition, param, buffered, commandTimeout));
		}

		/// <summary>
		/// Selects the keys that match the given condition.
		/// </summary>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The keys that match the given condition.</returns>
		public async Task<IEnumerable<T>> GetKeysAsync(string whereCondition = "", object param = null, bool buffered = true, int commandTimeout = 30)
		{
			return await Task.Run(() => GetKeys(whereCondition, param, buffered, commandTimeout));
		}

		/// <summary>
		/// Selects a limited number of rows that match the given condition.
		/// </summary>
		/// <param name="limit">The maximum number of rows.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The limited number of rows that match the given condition.</returns>
		public abstract IEnumerable<T> GetLimit(int limit, string whereCondition = "", object param = null, bool buffered = true, int commandTimeout = 30);

		/// <summary>
		/// Selects a limited number of rows that match the given condition.
		/// </summary>
		/// <param name="limit">The maximum number of rows.</param>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>A limited number of rows that match the given condition.</returns>
		public abstract IEnumerable<T> GetLimit(int limit, Type columnFilter, string whereCondition = "", object param = null, bool buffered = true, int commandTimeout = 30);

		/// <summary>
		/// Selects a limited number of rows that match the given condition.
		/// </summary>
		/// <param name="limit">The maximum number of rows.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The limited number of rows that match the given condition.</returns>
		public async Task<IEnumerable<T>> GetLimitAsync(int limit, string whereCondition = "", object param = null, bool buffered = true, int commandTimeout = 30)
		{
			return await Task.Run(() => GetLimit(limit, whereCondition, param, buffered, commandTimeout));
		}

		/// <summary>
		/// Selects a limited number of rows that match the given condition.
		/// </summary>
		/// <param name="limit">The maximum number of rows.</param>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>A limited number of rows that match the given condition.</returns>
		public async Task<IEnumerable<T>> GetLimitAsync(int limit, Type columnFilter, string whereCondition = "", object param = null, bool buffered = true, int commandTimeout = 30)
		{
			return await Task.Run(() => GetLimit(limit, columnFilter, whereCondition, param, buffered, commandTimeout));
		}

		/// <summary>
		/// Selects the rows that match the given condition.
		/// </summary>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public abstract IEnumerable<T> GetList(string whereCondition = "", object param = null, bool buffered = true, int commandTimeout = 30);

		/// <summary>
		/// Selects the rows that match the given condition.
		/// </summary>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public abstract IEnumerable<T> GetList(Type columnFilter, string whereCondition = "", object param = null, bool buffered = true, int commandTimeout = 30);

		/// <summary>
		/// Selects the rows that match the given condition.
		/// </summary>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public async Task<IEnumerable<T>> GetListAsync(string whereCondition = "", object param = null, bool buffered = true, int commandTimeout = 30)
		{
			return await Task.Run(() => GetList(whereCondition, param, buffered, commandTimeout));
		}

		/// <summary>
		/// Selects the rows that match the given condition.
		/// </summary>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public async Task<IEnumerable<T>> GetListAsync(Type columnFilter, string whereCondition = "", object param = null, bool buffered = true, int commandTimeout = 30)
		{
			return await Task.Run(() => GetList(columnFilter, whereCondition, param, buffered, commandTimeout));
		}

		/// <summary>
		/// Inserts a row.
		/// </summary>
		/// <param name="obj">The object to insert.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		public abstract void Insert(T obj, int commandTimeout = 30);

		/// <summary>
		/// Inserts a row.
		/// </summary>
		/// <param name="obj">The object to insert.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		public async Task InsertAsync(T obj, int commandTimeout = 30)
		{
			await Task.Run(() => Insert(obj, commandTimeout));
		}

		/// <summary>
		/// Inserts a row if it does not exist.
		/// </summary>
		/// <param name="obj">The object to insert.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the the row was inserted; false otherwise.</returns>
		public abstract bool InsertIfNotExists(T obj, int commandTimeout = 30);

		/// <summary>
		/// Inserts a row if it does not exist.
		/// </summary>
		/// <param name="obj">The object to insert.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the the row was inserted; false otherwise.</returns>
		public async Task<bool> InsertIfNotExistsAsync(T obj, int commandTimeout = 30)
		{
			return await Task.Run(() => InsertIfNotExists(obj, commandTimeout));
		}

		/// <summary>
		/// Counts the number of rows that match the given condition.
		/// </summary>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of rows that match the given condition.</returns>
		public abstract int RecordCount(string whereCondition = "", object param = null, int commandTimeout = 30);

		/// <summary>
		/// Counts the number of rows that match the given condition.
		/// </summary>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of rows that match the given condition.</returns>
		public async Task<int> RecordCountAsync(string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			return await Task.Run(() => RecordCount(whereCondition, param, commandTimeout));
		}

		/// <summary>
		/// Truncates all rows.
		/// </summary>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		public abstract void Truncate(int commandTimeout = 30);

		/// <summary>
		/// Truncates all rows.
		/// </summary>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		public async Task TruncateAsync(int commandTimeout = 30)
		{
			await Task.Run(() => Truncate(commandTimeout));
		}

		/// <summary>
		/// Updates a row.
		/// </summary>
		/// <param name="obj">The object to update.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the row was updated; false otherwise.</returns>
		public abstract bool Update(object obj, int commandTimeout = 30);

		/// <summary>
		/// Updates a row.
		/// </summary>
		/// <param name="obj">The object to update.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the row was updated; false otherwise.</returns>
		public abstract bool Update(T obj, int commandTimeout = 30);

		/// <summary>
		/// Updates a row.
		/// </summary>
		/// <param name="obj">The object to update.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the row was updated; false otherwise.</returns>
		public async Task<bool> UpdateAsync(object obj, int commandTimeout = 30)
		{
			return await Task.Run(() => Update(obj, commandTimeout));
		}

		/// <summary>
		/// Updates a row.
		/// </summary>
		/// <param name="obj">The object to update.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the row was updated; false otherwise.</returns>
		public async Task<bool> UpdateAsync(T obj, int commandTimeout = 30)
		{
			return await Task.Run(() => Update(obj, commandTimeout));
		}

		/// <summary>
		/// Upserts a row.
		/// </summary>
		/// <param name="obj">The object to upsert.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the object was upserted; false otherwise.</returns>
		public abstract bool Upsert(T obj, int commandTimeout = 30);

		/// <summary>
		/// Upserts a row.
		/// </summary>
		/// <param name="obj">The object to upsert.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the object was upserted; false otherwise.</returns>
		public async Task<bool> UpsertAsync(T obj, int commandTimeout = 30)
		{
			return await Task.Run(() => Upsert(obj, commandTimeout));
		}

		#endregion Other Methods

		#region WhereCondition

		/// <summary>
		/// Deletes the rows that match the given condition asynchronously.
		/// </summary>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of deleted rows.</returns>
		public async Task<int> DeleteListAsync(Expression<Func<T, bool>> whereExpr, int commandTimeout = 30)
		{
			return await Task.Run(() => DeleteList(whereExpr, commandTimeout));
		}

		/// <summary>
		/// Selects the rows that match the given condition asynchronously.
		/// </summary>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public async Task<IEnumerable<T>> GetDistinctAsync(Type columnFilter, Expression<Func<T, bool>> whereExpr, bool buffered = true, int commandTimeout = 30)
		{
			return await Task.Run(() => GetDistinct(columnFilter, whereExpr, buffered, commandTimeout));
		}

		/// <summary>
		/// Selects the rows that match the given condition asynchronously.
		/// </summary>
		/// <param name="limit">The maximum number of rows.</param>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public async Task<IEnumerable<T>> GetDistinctLimitAsync(int limit, Type columnFilter, Expression<Func<T, bool>> whereExpr, bool buffered = true, int commandTimeout = 30)
		{
			return await Task.Run(() => GetDistinctLimit(limit, columnFilter, whereExpr, buffered, commandTimeout));
		}

		/// <summary>
		/// Selects the rows with the given keys asynchronously.
		/// </summary>
		/// <typeparam name="KeyType">The key type.</typeparam>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The keys that match the given condition.</returns>
		public async Task<IEnumerable<KeyType>> GetKeysAsync<KeyType>(Expression<Func<T, bool>> whereExpr, bool buffered = true, int commandTimeout = 30)
		{
			return await Task.Run(() => GetKeys<KeyType>(whereExpr, buffered, commandTimeout));
		}

		/// <summary>
		/// Selects the keys that match the given condition asynchronously.
		/// </summary>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The keys that match the given condition.</returns>
		public async Task<IEnumerable<T>> GetKeysAsync(Expression<Func<T, bool>> whereExpr, bool buffered = true, int commandTimeout = 30)
		{
			return await Task.Run(() => GetKeys(whereExpr, buffered, commandTimeout));
		}

		/// <summary>
		/// Selects the rows that match the given condition asynchronously.
		/// </summary>
		/// <param name="limit">The maximum number of rows.</param>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public async Task<IEnumerable<T>> GetLimitAsync(int limit, Expression<Func<T, bool>> whereExpr, bool buffered = true, int commandTimeout = 30)
		{
			return await Task.Run(() => GetLimit(limit, whereExpr, buffered, commandTimeout));
		}

		/// <summary>
		/// Selects the rows that match the given condition asynchronously.
		/// </summary>
		/// <param name="limit">The maximum number of rows.</param>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public async Task<IEnumerable<T>> GetLimitAsync(int limit, Type columnFilter, Expression<Func<T, bool>> whereExpr, bool buffered = true, int commandTimeout = 30)
		{
			return await Task.Run(() => GetLimit(limit, columnFilter, whereExpr, buffered, commandTimeout));
		}

		/// <summary>
		/// Selects the rows that match the given condition asynchronously.
		/// </summary>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public async Task<IEnumerable<T>> GetListAsync(Expression<Func<T, bool>> whereExpr, bool buffered = true, int commandTimeout = 30)
		{
			return await Task.Run(() => GetList(whereExpr, buffered, commandTimeout));
		}

		/// <summary>
		/// Selects the rows that match the given condition asynchronously.
		/// </summary>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public async Task<IEnumerable<T>> GetListAsync(Type columnFilter, Expression<Func<T, bool>> whereExpr, bool buffered = true, int commandTimeout = 30)
		{
			return await Task.Run(() => GetList(columnFilter, whereExpr, buffered, commandTimeout));
		}

		/// <summary>
		/// Counts the number of rows that match the given condition asynchronously.
		/// </summary>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of rows that match the given condition.</returns>
		public async Task<int> RecordCountAsync(Expression<Func<T, bool>> whereExpr, int commandTimeout = 30)
		{
			return await Task.Run(() => RecordCount(whereExpr, commandTimeout));
		}

		/// <summary>
		/// Deletes the rows that match the given condition.
		/// </summary>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of deleted rows.</returns>
		public abstract int DeleteList(Expression<Func<T, bool>> whereExpr, int commandTimeout = 30);

		/// <summary>
		/// Selects the rows that match the given condition.
		/// </summary>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public abstract IEnumerable<T> GetDistinct(Type columnFilter, Expression<Func<T, bool>> whereExpr, bool buffered = true, int commandTimeout = 30);

		/// <summary>
		/// Selects the rows that match the given condition.
		/// </summary>
		/// <param name="limit">The maximum number of rows.</param>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public abstract IEnumerable<T> GetDistinctLimit(int limit, Type columnFilter, Expression<Func<T, bool>> whereExpr, bool buffered = true, int commandTimeout = 30);

		/// <summary>
		/// Selects the rows with the given keys.
		/// </summary>
		/// <typeparam name="KeyType">The key type.</typeparam>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The keys that match the given condition.</returns>
		public abstract IEnumerable<KeyType> GetKeys<KeyType>(Expression<Func<T, bool>> whereExpr, bool buffered = true, int commandTimeout = 30);

		/// <summary>
		/// Selects the keys that match the given condition.
		/// </summary>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The keys that match the given condition.</returns>
		public abstract IEnumerable<T> GetKeys(Expression<Func<T, bool>> whereExpr, bool buffered = true, int commandTimeout = 30);

		/// <summary>
		/// Selects a limited number of rows that match the given condition.
		/// </summary>
		/// <param name="limit">The maximum number of rows.</param>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The limited number of rows that match the given condition.</returns>
		public abstract IEnumerable<T> GetLimit(int limit, Expression<Func<T, bool>> whereExpr, bool buffered = true, int commandTimeout = 30);

		/// <summary>
		/// Selects a limited number of rows that match the given condition.
		/// </summary>
		/// <param name="limit">The maximum number of rows.</param>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>A limited number of rows that match the given condition.</returns>
		public abstract IEnumerable<T> GetLimit(int limit, Type columnFilter, Expression<Func<T, bool>> whereExpr, bool buffered = true, int commandTimeout = 30);

		/// <summary>
		/// Selects the rows that match the given condition.
		/// </summary>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public abstract IEnumerable<T> GetList(Expression<Func<T, bool>> whereExpr, bool buffered = true, int commandTimeout = 30);

		/// <summary>
		/// Selects the rows that match the given condition.
		/// </summary>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public abstract IEnumerable<T> GetList(Type columnFilter, Expression<Func<T, bool>> whereExpr, bool buffered = true, int commandTimeout = 30);

		/// <summary>
		/// Counts the number of rows that match the given condition.
		/// </summary>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of rows that match the given condition.</returns>
		public abstract int RecordCount(Expression<Func<T, bool>> whereExpr, int commandTimeout = 30);

		#endregion WhereCondition
	}
}
