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
using System.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Dapper.Extra.Utilities
{
	/// <summary>
	/// Interface for an object that interacts with an <see cref="Dapper.Extra.ISqlQueries{T}"/>.
	/// </summary>
	/// <typeparam name="T">The table type.</typeparam>
	public interface ITransactionAccessObjectAsync<T> where T : class
	{
		#region Bulk

		/// <summary>
		/// Deletes the rows with the given keys.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="keys">The keys for the rows to delete.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of deleted rows.</returns>
		Task<int> BulkDeleteAsync(IDbTransaction transaction, IEnumerable<object> keys, int commandTimeout = 30);

		/// <summary>
		/// Deletes the rows with the given keys.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="keys">The keys for the rows to delete.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of deleted rows.</returns>
		Task<int> BulkDeleteAsync(IDbTransaction transaction, IEnumerable<int> keys, int commandTimeout = 30);

		/// <summary>
		/// Deletes the given rows.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="objs">The objects to delete.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of deleted rows.</returns>
		Task<int> BulkDeleteAsync(IDbTransaction transaction, IEnumerable<T> objs, int commandTimeout = 30);

		/// <summary>
		/// Selects the rows with the given keys.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="keys">The keys of the rows to select.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows with the given keys.</returns>
		Task<IEnumerable<T>> BulkGetAsync(IDbTransaction transaction, IEnumerable<object> keys, int commandTimeout = 30);

		/// <summary>
		/// Selects the rows with the given keys.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="keys">The keys of the rows to select.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows with the given keys.</returns>
		Task<IEnumerable<T>> BulkGetAsync(IDbTransaction transaction, IEnumerable<int> keys, int commandTimeout = 30);

		/// <summary>
		/// Selects the rows with the given keys.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="objs">The objects to select.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given keys.</returns>
		Task<IEnumerable<T>> BulkGetAsync(IDbTransaction transaction, IEnumerable<T> objs, int commandTimeout = 30);

		/// <summary>
		/// Inserts the given rows.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="objs">The objects to insert.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		Task BulkInsertAsync(IDbTransaction transaction, IEnumerable<T> objs, int commandTimeout = 30);

		/// <summary>
		/// Inserts the given rows if they do not exist.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="objs">The objects to insert.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of rows inserted.</returns>
		Task<int> BulkInsertIfNotExistsAsync(IDbTransaction transaction, IEnumerable<T> objs, int commandTimeout = 30);

		/// <summary>
		/// Updates the given rows.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="objs">The objects to update.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of updated rows.</returns>
		Task<int> BulkUpdateAsync(IDbTransaction transaction, IEnumerable<T> objs, int commandTimeout = 30);

		/// <summary>
		/// Upserts the given rows.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="objs">The objects to upsert.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of upserted rows.</returns>
		Task<int> BulkUpsertAsync(IDbTransaction transaction, IEnumerable<T> objs, int commandTimeout = 30);

		#endregion Bulk

		#region Other Methods

		/// <summary>
		/// Deletes the row with the given key.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="key">The key of the row to delete.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the row was deleted; false otherwise.</returns>
		Task<bool> DeleteAsync(IDbTransaction transaction, object key, int commandTimeout = 30);

		/// <summary>
		/// Deletes the given row.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="obj">The object to delete.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the row was deleted; false otherwise.</returns>
		Task<bool> DeleteAsync(IDbTransaction transaction, T obj, int commandTimeout = 30);

		/// <summary>
		/// Deletes the rows that match the given condition.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of deleted rows.</returns>
		Task<int> DeleteListAsync(IDbTransaction transaction, string whereCondition = "", object param = null, int commandTimeout = 30);

		/// <summary>
		/// Selects the row with the given key.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="key">The key of the row to select.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The row with the given key.</returns>
		Task<T> GetAsync(IDbTransaction transaction, object key, int commandTimeout = 30);

		/// <summary>
		/// Selects a row.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="obj">The object to select.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The selected row if it exists; otherwise null.</returns>
		Task<T> GetAsync(IDbTransaction transaction, T obj, int commandTimeout = 30);

		/// <summary>
		/// Selects the rows that match the given condition.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		Task<IEnumerable<T>> GetDistinctAsync(IDbTransaction transaction, Type columnFilter, string whereCondition = "", object param = null, bool buffered = true, int commandTimeout = 30);

		/// <summary>
		/// Selects the rows that match the given condition.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="limit">The maximum number of rows.</param>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		Task<IEnumerable<T>> GetDistinctLimitAsync(IDbTransaction transaction, int limit, Type columnFilter, string whereCondition = "", object param = null, bool buffered = true, int commandTimeout = 30);

		/// <summary>
		/// Selects the rows with the given keys.
		/// </summary>
		/// <typeparam name="KeyType">The key type.</typeparam>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The keys that match the given condition.</returns>
		Task<IEnumerable<KeyType>> GetKeysAsync<KeyType>(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int commandTimeout = 30);

		/// <summary>
		/// Selects the keys that match the given condition.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The keys that match the given condition.</returns>
		Task<IEnumerable<T>> GetKeysAsync(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int commandTimeout = 30);

		/// <summary>
		/// Selects a limited number of rows that match the given condition.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="limit">The maximum number of rows.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The limited number of rows that match the given condition.</returns>
		Task<IEnumerable<T>> GetLimitAsync(IDbTransaction transaction, int limit, string whereCondition = "", object param = null, bool buffered = true, int commandTimeout = 30);

		/// <summary>
		/// Selects a limited number of rows that match the given condition.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="limit">The maximum number of rows.</param>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>A limited number of rows that match the given condition.</returns>
		Task<IEnumerable<T>> GetLimitAsync(IDbTransaction transaction, int limit, Type columnFilter, string whereCondition = "", object param = null, bool buffered = true, int commandTimeout = 30);

		/// <summary>
		/// Selects the rows that match the given condition.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		Task<IEnumerable<T>> GetListAsync(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int commandTimeout = 30);

		/// <summary>
		/// Selects the rows that match the given condition.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		Task<IEnumerable<T>> GetListAsync(IDbTransaction transaction, Type columnFilter, string whereCondition = "", object param = null, bool buffered = true, int commandTimeout = 30);

		/// <summary>
		/// Inserts a row.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="obj">The object to insert.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		Task InsertAsync(IDbTransaction transaction, T obj, int commandTimeout = 30);

		/// <summary>
		/// Inserts a row if it does not exist.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="obj">The object to insert.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the the row was inserted; false otherwise.</returns>
		Task<bool> InsertIfNotExistsAsync(IDbTransaction transaction, T obj, int commandTimeout = 30);

		/// <summary>
		/// Counts the number of rows that match the given condition.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of rows that match the given condition.</returns>
		Task<int> RecordCountAsync(IDbTransaction transaction, string whereCondition = "", object param = null, int commandTimeout = 30);

		/// <summary>
		/// Truncates all rows.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		Task TruncateAsync(IDbTransaction transaction, int commandTimeout = 30);

		/// <summary>
		/// Updates a row.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="obj">The object to update.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the row was updated; false otherwise.</returns>
		Task<bool> UpdateAsync(IDbTransaction transaction, object obj, int commandTimeout = 30);

		/// <summary>
		/// Updates a row.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="obj">The object to update.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the row was updated; false otherwise.</returns>
		Task<bool> UpdateAsync(IDbTransaction transaction, T obj, int commandTimeout = 30);

		/// <summary>
		/// Upserts a row.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="obj">The object to upsert.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the object was upserted; false otherwise.</returns>
		Task<bool> UpsertAsync(IDbTransaction transaction, T obj, int commandTimeout = 30);

		#endregion Other Methods

		#region WhereCondition

		/// <summary>
		/// Deletes the rows that match the given condition asynchronously.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of deleted rows.</returns>
		Task<int> DeleteListAsync(IDbTransaction transaction, Expression<Func<T, bool>> whereExpr, int commandTimeout = 30);

		/// <summary>
		/// Selects the rows that match the given condition asynchronously.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		Task<IEnumerable<T>> GetDistinctAsync(IDbTransaction transaction, Type columnFilter, Expression<Func<T, bool>> whereExpr, bool buffered = true, int commandTimeout = 30);

		/// <summary>
		/// Selects the rows that match the given condition asynchronously.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="limit">The maximum number of rows.</param>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		Task<IEnumerable<T>> GetDistinctLimitAsync(IDbTransaction transaction, int limit, Type columnFilter, Expression<Func<T, bool>> whereExpr, bool buffered = true, int commandTimeout = 30);

		/// <summary>
		/// Selects the rows with the given keys asynchronously.
		/// </summary>
		/// <typeparam name="KeyType">The key type.</typeparam>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The keys that match the given condition.</returns>
		Task<IEnumerable<KeyType>> GetKeysAsync<KeyType>(IDbTransaction transaction, Expression<Func<T, bool>> whereExpr, bool buffered = true, int commandTimeout = 30);

		/// <summary>
		/// Selects the keys that match the given condition asynchronously.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The keys that match the given condition.</returns>
		Task<IEnumerable<T>> GetKeysAsync(IDbTransaction transaction, Expression<Func<T, bool>> whereExpr, bool buffered = true, int commandTimeout = 30);

		/// <summary>
		/// Selects the rows that match the given condition asynchronously.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="limit">The maximum number of rows.</param>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		Task<IEnumerable<T>> GetLimitAsync(IDbTransaction transaction, int limit, Expression<Func<T, bool>> whereExpr, bool buffered = true, int commandTimeout = 30);

		/// <summary>
		/// Selects the rows that match the given condition asynchronously.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="limit">The maximum number of rows.</param>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		Task<IEnumerable<T>> GetLimitAsync(IDbTransaction transaction, int limit, Type columnFilter, Expression<Func<T, bool>> whereExpr, bool buffered = true, int commandTimeout = 30);

		/// <summary>
		/// Selects the rows that match the given condition asynchronously.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		Task<IEnumerable<T>> GetListAsync(IDbTransaction transaction, Expression<Func<T, bool>> whereExpr, bool buffered = true, int commandTimeout = 30);

		/// <summary>
		/// Selects the rows that match the given condition asynchronously.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		Task<IEnumerable<T>> GetListAsync(IDbTransaction transaction, Type columnFilter, Expression<Func<T, bool>> whereExpr, bool buffered = true, int commandTimeout = 30);

		/// <summary>
		/// Counts the number of rows that match the given condition asynchronously.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of rows that match the given condition.</returns>
		Task<int> RecordCountAsync(IDbTransaction transaction, Expression<Func<T, bool>> whereExpr, int commandTimeout = 30);

		#endregion WhereCondition
	}
}
