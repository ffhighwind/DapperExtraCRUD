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
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper.Extra;

namespace Dapper
{
	/// <summary>
	/// Dapper extension methods for <see cref="IDbConnection"/> and <see cref="SqlConnection"/>.
	/// </summary>
	public static class DapperExtraExtensions
	{
		/// <summary>
		/// Deletes the rows with the given keys.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="keys">The keys for the rows to delete.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of deleted rows.</returns>
		public static int BulkDelete<T>(this IDbConnection connection, IEnumerable<object> keys, IDbTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			int count = ExtraCrud.Queries<T>().BulkDeleteKeys(connection, keys, transaction, commandTimeout);
			return count;
		}

		/// <summary>
		/// Deletes the given rows.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="objs">The objects to delete.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of deleted rows.</returns>
		public static int BulkDelete<T>(this IDbConnection connection, IEnumerable<T> objs, IDbTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			int count = ExtraCrud.Queries<T>().BulkDelete(connection, objs, transaction, commandTimeout);
			return count;
		}

		/// <summary>
		/// Deletes the rows with the given keys asynchronously.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="keys">The keys for the rows to delete.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of deleted rows.</returns>
		public static async Task<int> BulkDeleteAsync<T>(this IDbConnection connection, IEnumerable<object> keys, IDbTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			return await Task.Run(() => BulkDelete<T>(connection, keys, transaction, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Deletes the given rows asynchronously.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="objs">The objects to delete.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of deleted rows.</returns>
		public static async Task<int> BulkDeleteAsync<T>(this IDbConnection connection, IEnumerable<T> objs, IDbTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			return await Task.Run(() => BulkDelete<T>(connection, objs, transaction, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Selects the rows with the given keys.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="keys">The keys of the rows to select.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows with the given keys.</returns>
		public static IEnumerable<T> BulkGet<T>(this IDbConnection connection, IEnumerable<object> keys, IDbTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			IEnumerable<T> list = ExtraCrud.Queries<T>().BulkGetKeys(connection, keys, transaction, commandTimeout);
			return list;
		}

		/// <summary>
		/// Selects the rows with the given keys.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="objs">The objects to select.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given keys.</returns>
		public static IEnumerable<T> BulkGet<T>(this IDbConnection connection, IEnumerable<T> objs, IDbTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			IEnumerable<T> result = ExtraCrud.Queries<T>().BulkGet(connection, objs, transaction, commandTimeout);
			return result;
		}

		/// <summary>
		/// Selects the rows with the given keys asynchronously.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="keys">The keys of the rows to select.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows with the given keys.</returns>
		public static async Task<IEnumerable<T>> BulkGetAsync<T>(this IDbConnection connection, IEnumerable<object> keys, IDbTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			return await Task.Run(() => BulkGet<T>(connection, keys, transaction, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Selects the rows with the given keys asynchronously.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="objs">The objects to select.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given keys.</returns>
		public static async Task<IEnumerable<T>> BulkGetAsync<T>(this IDbConnection connection, IEnumerable<T> objs, IDbTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			return await Task.Run(() => BulkGet<T>(connection, objs, transaction, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Inserts the given rows.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="objs">The objects to insert.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		public static void BulkInsert<T>(this IDbConnection connection, IEnumerable<T> objs, IDbTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			ExtraCrud.Queries<T>().BulkInsert(connection, objs, transaction, commandTimeout);
		}

		/// <summary>
		/// Inserts the given rows asynchronously.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="objs">The objects to insert.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		public static async Task BulkInsertAsync<T>(this IDbConnection connection, IEnumerable<T> objs, IDbTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			await Task.Run(() => BulkInsert(connection, objs, transaction, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Inserts the given rows if they do not exist.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="objs">The objects to insert.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of rows inserted.</returns>
		public static int BulkInsertIfNotExists<T>(this IDbConnection connection, IEnumerable<T> objs, IDbTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			return ExtraCrud.Queries<T>().BulkInsertIfNotExists(connection, objs, transaction, commandTimeout);
		}

		/// <summary>
		/// Inserts the given rows if they do not exist asynchronously.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="objs">The objects to insert.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of rows inserted.</returns>
		public static async Task<int> BulkInsertIfNotExistsAsync<T>(this IDbConnection connection, IEnumerable<T> objs, IDbTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			return await Task.Run(() => BulkInsertIfNotExists(connection, objs, transaction, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Updates the given rows.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="objs">The objects to update.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of updated rows.</returns>
		public static int BulkUpdate<T>(this IDbConnection connection, IEnumerable<T> objs, IDbTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			int count = ExtraCrud.Queries<T>().BulkUpdate(connection, objs, transaction, commandTimeout);
			return count;
		}

		/// <summary>
		/// Updates the given rows asynchronously.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="objs">The objects to update.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of updated rows.</returns>
		public static async Task<int> BulkUpdateAsync<T>(this IDbConnection connection, IEnumerable<T> objs, IDbTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			return await Task.Run(() => BulkUpdate(connection, objs, transaction, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Upserts the given rows.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="objs">The objects to upsert.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of upserted rows.</returns>
		public static int BulkUpsert<T>(this IDbConnection connection, IEnumerable<T> objs, IDbTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			int count = ExtraCrud.Queries<T>().BulkUpsert(connection, objs, transaction, commandTimeout);
			return count;
		}

		/// <summary>
		/// Upserts the given rows.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="objs">The objects to upsert.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of upserted rows.</returns>
		public static async Task<int> BulkUpsertAsync<T>(this IDbConnection connection, IEnumerable<T> objs, IDbTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			return await Task.Run(() => BulkUpsert(connection, objs, transaction, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Deletes the row with the given key.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="key">The key of the row to delete.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the row was deleted; false otherwise.</returns>
		public static bool Delete<T>(this IDbConnection connection, object key, IDbTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			bool success = ExtraCrud.Queries<T>().DeleteKey(connection, key, transaction, commandTimeout);
			return success;
		}

		/// <summary>
		/// Deletes the given row.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="obj">The object to delete.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the row was deleted; false otherwise.</returns>
		public static bool Delete<T>(this IDbConnection connection, T obj, IDbTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			bool success = ExtraCrud.Queries<T>().Delete(connection, obj, transaction, commandTimeout);
			return success;
		}

		/// <summary>
		/// Truncates all rows.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		public static void Truncate<T>(this IDbConnection connection, IDbTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			ExtraCrud.Queries<T>().Truncate(connection, transaction, commandTimeout);
		}

		/// <summary>
		/// Truncates all rows asynchronously.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		public static async Task TruncateAsync<T>(this IDbConnection connection, IDbTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			await Task.Run(() => Truncate<T>(connection, transaction, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Deletes the row with the given key asynchronously.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="key">The key of the row to delete.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the row was deleted; false otherwise.</returns>
		public static async Task<bool> DeleteAsync<T>(this IDbConnection connection, object key, IDbTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			return await Task.Run(() => Delete<T>(connection, key, transaction, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Deletes the given row asynchronously.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="obj">The object to delete.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the row was deleted; false otherwise.</returns>
		public static async Task<bool> DeleteAsync<T>(this IDbConnection connection, T obj, IDbTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			return await Task.Run(() => Delete<T>(connection, obj, transaction, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Deletes all rows.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of deleted rows.</returns>
		public static int DeleteList<T>(this IDbConnection connection, IDbTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			int count = ExtraCrud.Queries<T>().DeleteList(connection, "", null, transaction, commandTimeout);
			return count;
		}

		/// <summary>
		/// Deletes the rows that match the given condition.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of deleted rows.</returns>
		public static int DeleteList<T>(this IDbConnection connection, string whereCondition, object param = null, IDbTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			int count = ExtraCrud.Queries<T>().DeleteList(connection, whereCondition, param, transaction, commandTimeout);
			return count;
		}

		/// <summary>
		/// Deletes all rows asynchronously.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of deleted rows.</returns>
		public static async Task<int> DeleteListAsync<T>(this IDbConnection connection, IDbTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			return await Task.Run(() => DeleteList<T>(connection, "", null, transaction, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Deletes the rows that match the given condition asynchronously.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of deleted rows.</returns>
		public static async Task<int> DeleteListAsync<T>(this IDbConnection connection, string whereCondition, object param = null, IDbTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			return await Task.Run(() => DeleteList<T>(connection, whereCondition, param, transaction, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Selects the row with the given key.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="key">The key of the row to select.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The row with the given key.</returns>
		public static T Get<T>(this IDbConnection connection, object key, IDbTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			T value = ExtraCrud.Queries<T>().GetKey(connection, key, transaction, commandTimeout);
			return value;
		}

		/// <summary>
		/// Selects a row.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="obj">The object to select.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The selected row if it exists; otherwise null.</returns>
		public static T Get<T>(this IDbConnection connection, T obj, IDbTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			T result = ExtraCrud.Queries<T>().Get(connection, obj, transaction, commandTimeout);
			return result;
		}

		/// <summary>
		/// Selects the row with the given key asynchronously.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="key">The key of the row to select.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The row with the given key.</returns>
		public static async Task<T> GetAsync<T>(this IDbConnection connection, object key, IDbTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			return await Task.Run(() => Get<T>(connection, key, transaction, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Selects a row asynchronously.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="obj">The object to select.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The selected row if it exists; otherwise null.</returns>
		public static async Task<T> GetAsync<T>(this IDbConnection connection, T obj, IDbTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			return await Task.Run(() => Get(connection, obj, transaction, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Selects the rows that match the given condition.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public static IEnumerable<T> GetDistinct<T>(this IDbConnection connection, Type columnFilter, IDbTransaction transaction, bool buffered = true, int commandTimeout = 30)
			where T : class
		{
			IEnumerable<T> list = ExtraCrud.Queries<T>().GetDistinct(connection, columnFilter, "", null, transaction, buffered, commandTimeout);
			return list;
		}

		/// <summary>
		/// Selects the rows that match the given condition.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public static IEnumerable<T> GetDistinct<T>(this IDbConnection connection, Type columnFilter, string whereCondition = "", object param = null, IDbTransaction transaction = null, bool buffered = true, int commandTimeout = 30)
			where T : class
		{
			IEnumerable<T> list = ExtraCrud.Queries<T>().GetDistinct(connection, columnFilter, whereCondition, param, transaction, buffered, commandTimeout);
			return list;
		}

		/// <summary>
		/// Selects the rows that match the given condition asynchronously.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public static async Task<IEnumerable<T>> GetDistinctAsync<T>(this IDbConnection connection, Type columnFilter, IDbTransaction transaction, bool buffered = true, int commandTimeout = 30)
			where T : class
		{
			return await Task.Run(() => GetDistinct<T>(connection, columnFilter, transaction, buffered, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Selects the rows that match the given condition asynchronously.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public static async Task<IEnumerable<T>> GetDistinctAsync<T>(this IDbConnection connection, Type columnFilter, string whereCondition = "", object param = null, IDbTransaction transaction = null, bool buffered = true, int commandTimeout = 30)
			where T : class
		{
			return await Task.Run(() => GetDistinct<T>(connection, columnFilter, whereCondition, param, transaction, buffered, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Selects the rows that match the given condition.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="limit">The maximum number of rows.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public static IEnumerable<T> GetDistinctLimit<T>(this IDbConnection connection, int limit, Type columnFilter, IDbTransaction transaction, bool buffered = true, int commandTimeout = 30)
			where T : class
		{
			IEnumerable<T> list = ExtraCrud.Queries<T>().GetDistinctLimit(connection, limit, columnFilter, "", null, transaction, buffered, commandTimeout);
			return list;
		}

		/// <summary>
		/// Selects the rows that match the given condition.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="limit">The maximum number of rows.</param>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public static IEnumerable<T> GetDistinctLimit<T>(this IDbConnection connection, int limit, Type columnFilter, string whereCondition = "", object param = null, IDbTransaction transaction = null, bool buffered = true, int commandTimeout = 30)
			where T : class
		{
			IEnumerable<T> list = ExtraCrud.Queries<T>().GetDistinctLimit(connection, limit, columnFilter, whereCondition, param, transaction, buffered, commandTimeout);
			return list;
		}

		/// <summary>
		/// Selects the rows that match the given condition asynchronously.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="limit">The maximum number of rows.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public static async Task<IEnumerable<T>> GetDistinctLimitAsync<T>(this IDbConnection connection, int limit, Type columnFilter, IDbTransaction transaction, bool buffered = true, int commandTimeout = 30)
			where T : class
		{
			return await Task.Run(() => GetDistinctLimit<T>(connection, limit, columnFilter, transaction, buffered, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Selects the rows that match the given condition asynchronously.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="limit">The maximum number of rows.</param>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public static async Task<IEnumerable<T>> GetDistinctLimitAsync<T>(this IDbConnection connection, int limit, Type columnFilter, string whereCondition, object param = null, IDbTransaction transaction = null, bool buffered = true, int commandTimeout = 30)
			where T : class
		{
			return await Task.Run(() => GetDistinctLimit<T>(connection, limit, columnFilter, whereCondition, param, transaction, buffered, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Selects all keys.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <typeparam name="KeyType">The key type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>All keys.</returns>
		public static IEnumerable<KeyType> GetKeys<T, KeyType>(this IDbConnection connection, IDbTransaction transaction = null, bool buffered = true, int commandTimeout = 30)
			where T : class
		{
			IEnumerable<KeyType> keys = GetKeys<T, KeyType>(connection, "", null, transaction, buffered, commandTimeout);
			return keys;
		}

		/// <summary>
		/// Selects all keys.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>All keys.</returns>
		public static IEnumerable<T> GetKeys<T>(this IDbConnection connection, IDbTransaction transaction = null, bool buffered = true, int commandTimeout = 30)
			where T : class
		{
			IEnumerable<T> keys = ExtraCrud.Queries<T>().GetKeys(connection, "", null, transaction, buffered, commandTimeout);
			return keys;
		}

		/// <summary>
		/// Selects the rows with the given keys.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <typeparam name="KeyType">The key type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The keys that match the given condition.</returns>
		public static IEnumerable<KeyType> GetKeys<T, KeyType>(this IDbConnection connection, string whereCondition, object param = null, IDbTransaction transaction = null, bool buffered = true, int commandTimeout = 30)
			where T : class
		{
			IEnumerable<object> keys = ExtraCrud.Queries<T>().GetKeysKeys(connection, whereCondition, param, transaction, buffered, commandTimeout);
			if (typeof(KeyType) == typeof(long)) {
				if (keys.Any()) {
					Type type = keys.First().GetType();
					if (type == typeof(int)) {
						keys = keys.Select(k => (object)(long)(int)k);
					}
				}
			}
			IEnumerable<KeyType> castedKeys = keys.Select(k => (KeyType)k);
			return castedKeys;
		}

		/// <summary>
		/// Selects the keys that match the given condition.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The keys that match the given condition.</returns>
		public static IEnumerable<T> GetKeys<T>(this IDbConnection connection, string whereCondition, object param = null, IDbTransaction transaction = null, bool buffered = true, int commandTimeout = 30)
			where T : class
		{
			IEnumerable<T> keys = ExtraCrud.Queries<T>().GetKeys(connection, whereCondition, param, transaction, buffered, commandTimeout);
			return keys;
		}

		/// <summary>
		/// Selects all keys asynchronously.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <typeparam name="KeyType">The key type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>All keys.</returns>
		public static async Task<IEnumerable<KeyType>> GetKeysAsync<T, KeyType>(this IDbConnection connection, IDbTransaction transaction = null, bool buffered = true, int commandTimeout = 30)
			where T : class
		{
			return await Task.Run(() => GetKeys<T, KeyType>(connection, "", null, transaction, buffered, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Selects all keys asynchronously.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>All keys.</returns>
		public static async Task<IEnumerable<T>> GetKeysAsync<T>(this IDbConnection connection, IDbTransaction transaction = null, bool buffered = true, int commandTimeout = 30)
			where T : class
		{
			return await Task.Run(() => GetKeys<T>(connection, "", null, transaction, buffered, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Selects the rows with the given keys asynchronously.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <typeparam name="KeyType">The key type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The keys that match the given condition.</returns>
		public static async Task<IEnumerable<KeyType>> GetKeysAsync<T, KeyType>(this IDbConnection connection, string whereCondition, object param = null, IDbTransaction transaction = null, bool buffered = true, int commandTimeout = 30)
			where T : class
		{
			return await Task.Run(() => GetKeys<T, KeyType>(connection, whereCondition, param, transaction, buffered, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Selects the keys that match the given condition asynchronously.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The keys that match the given condition.</returns>
		public static async Task<IEnumerable<T>> GetKeysAsync<T>(this IDbConnection connection, string whereCondition, object param = null, IDbTransaction transaction = null, bool buffered = true, int commandTimeout = 30)
			where T : class
		{
			return await Task.Run(() => GetKeys<T>(connection, whereCondition, param, transaction, buffered, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Selects a limited number of rows.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="limit">The maximum number of rows.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>A limited number of rows.</returns>
		public static IEnumerable<T> GetLimit<T>(this IDbConnection connection, int limit, IDbTransaction transaction = null, bool buffered = true, int commandTimeout = 30)
			where T : class
		{
			IEnumerable<T> list = ExtraCrud.Queries<T>().GetLimit(connection, limit, "", null, transaction, buffered, commandTimeout);
			return list;
		}

		/// <summary>
		/// Selects a limited number of rows that match the given condition.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="limit">The maximum number of rows.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The limited number of rows that match the given condition.</returns>
		public static IEnumerable<T> GetLimit<T>(this IDbConnection connection, int limit, string whereCondition, object param = null, IDbTransaction transaction = null, bool buffered = true, int commandTimeout = 30)
			where T : class
		{
			IEnumerable<T> list = ExtraCrud.Queries<T>().GetLimit(connection, limit, whereCondition, param, transaction, buffered, commandTimeout);
			return list;
		}

		/// <summary>
		/// Selects a limited number of rows.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="limit">The maximum number of rows.</param>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>A limited number of rows.</returns>
		public static IEnumerable<T> GetLimit<T>(this IDbConnection connection, Type columnFilter, int limit, IDbTransaction transaction, bool buffered = true, int commandTimeout = 30)
			where T : class
		{
			IEnumerable<T> list = ExtraCrud.Queries<T>().GetFilterLimit(connection, limit, columnFilter, "", null, transaction, buffered, commandTimeout);
			return list;
		}

		/// <summary>
		/// Selects a limited number of rows that match the given condition.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="limit">The maximum number of rows.</param>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>A limited number of rows that match the given condition.</returns>
		public static IEnumerable<T> GetLimit<T>(this IDbConnection connection, int limit, Type columnFilter, string whereCondition, object param = null, IDbTransaction transaction = null, bool buffered = true, int commandTimeout = 30)
			where T : class
		{
			IEnumerable<T> list = ExtraCrud.Queries<T>().GetFilterLimit(connection, limit, columnFilter, whereCondition, param, transaction, buffered, commandTimeout);
			return list;
		}

		/// <summary>
		/// Selects a limited number of rows asynchronously.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="limit">The maximum number of rows.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public static async Task<IEnumerable<T>> GetLimitAsync<T>(this IDbConnection connection, int limit, IDbTransaction transaction, bool buffered = true, int commandTimeout = 30)
			where T : class
		{
			return await Task.Run(() => GetLimit<T>(connection, limit, "", null, transaction, buffered, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Selects the rows that match the given condition asynchronously.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="limit">The maximum number of rows.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public static async Task<IEnumerable<T>> GetLimitAsync<T>(this IDbConnection connection, int limit, string whereCondition, object param = null, IDbTransaction transaction = null, bool buffered = true, int commandTimeout = 30)
			where T : class
		{
			return await Task.Run(() => GetLimit<T>(connection, limit, whereCondition, param, transaction, buffered, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Selects the rows that match the given condition asynchronously.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="limit">The maximum number of rows.</param>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public static async Task<IEnumerable<T>> GetLimitAsync<T>(this IDbConnection connection, int limit, Type columnFilter, IDbTransaction transaction = null, bool buffered = true, int commandTimeout = 30)
			where T : class
		{
			return await Task.Run(() => GetLimit<T>(connection, limit, columnFilter, "", null, transaction, buffered, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Selects the rows that match the given condition asynchronously.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="limit">The maximum number of rows.</param>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public static async Task<IEnumerable<T>> GetLimitAsync<T>(this IDbConnection connection, int limit, Type columnFilter, string whereCondition, object param = null, IDbTransaction transaction = null, bool buffered = true, int commandTimeout = 30)
			where T : class
		{
			return await Task.Run(() => GetLimit<T>(connection, limit, columnFilter, whereCondition, param, transaction, buffered, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Selects all rows.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>All rows.</returns>
		public static IEnumerable<T> GetList<T>(this IDbConnection connection, IDbTransaction transaction = null, bool buffered = true, int commandTimeout = 30)
			where T : class
		{
			IEnumerable<T> list = ExtraCrud.Queries<T>().GetList(connection, "", null, transaction, buffered, commandTimeout);
			return list;
		}

		/// <summary>
		/// Selects the rows that match the given condition.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public static IEnumerable<T> GetList<T>(this IDbConnection connection, string whereCondition, object param = null, IDbTransaction transaction = null, bool buffered = true, int commandTimeout = 30)
			where T : class
		{
			IEnumerable<T> list = ExtraCrud.Queries<T>().GetList(connection, whereCondition, param, transaction, buffered, commandTimeout);
			return list;
		}

		/// <summary>
		/// Selects all rows.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>All rows.</returns>
		public static IEnumerable<T> GetList<T>(this IDbConnection connection, Type columnFilter, IDbTransaction transaction, bool buffered = true, int commandTimeout = 30)
			where T : class
		{
			IEnumerable<T> list = ExtraCrud.Queries<T>().GetFilter(connection, columnFilter, "", null, transaction, buffered, commandTimeout);
			return list;
		}

		/// <summary>
		/// Selects the rows that match the given condition.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public static IEnumerable<T> GetList<T>(this IDbConnection connection, Type columnFilter, string whereCondition, object param = null, IDbTransaction transaction = null, bool buffered = true, int commandTimeout = 30)
			where T : class
		{
			IEnumerable<T> list = ExtraCrud.Queries<T>().GetFilter(connection, columnFilter, whereCondition, param, transaction, buffered, commandTimeout);
			return list;
		}

		/// <summary>
		/// Selects all rows asynchronously.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>All rows.</returns>
		public static async Task<IEnumerable<T>> GetListAsync<T>(this IDbConnection connection, IDbTransaction transaction = null, bool buffered = true, int commandTimeout = 30)
			where T : class
		{
			return await Task.Run(() => GetList<T>(connection, "", null, transaction, buffered, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Selects the rows that match the given condition asynchronously.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public static async Task<IEnumerable<T>> GetListAsync<T>(this IDbConnection connection, string whereCondition, object param = null, IDbTransaction transaction = null, bool buffered = true, int commandTimeout = 30)
			where T : class
		{
			return await Task.Run(() => GetList<T>(connection, whereCondition, param, transaction, buffered, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Selects the rows that match the given condition asynchronously.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public static async Task<IEnumerable<T>> GetListAsync<T>(this IDbConnection connection, Type columnFilter, IDbTransaction transaction, bool buffered = true, int commandTimeout = 30)
			where T : class
		{
			return await Task.Run(() => GetList<T>(connection, columnFilter, "", null, transaction, buffered, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Selects the rows that match the given condition asynchronously.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public static async Task<IEnumerable<T>> GetListAsync<T>(this IDbConnection connection, Type columnFilter, string whereCondition, object param = null, IDbTransaction transaction = null, bool buffered = true, int commandTimeout = 30)
			where T : class
		{
			return await Task.Run(() => GetList<T>(connection, columnFilter, whereCondition, param, transaction, buffered, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Inserts a row.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="obj">The object to insert.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		public static void Insert<T>(this IDbConnection connection, T obj, IDbTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			ExtraCrud.Queries<T>().Insert(connection, obj, transaction, commandTimeout);
		}

		/// <summary>
		/// Inserts a row asynchronously.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="obj">The object to insert.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		public static async Task InsertAsync<T>(this IDbConnection connection, T obj, IDbTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			await Task.Run(() => Insert(connection, obj, transaction, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Inserts a row if it does not exist.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="obj">The object to insert.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the the row was inserted; false otherwise.</returns>
		public static bool InsertIfNotExists<T>(this IDbConnection connection, T obj, IDbTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			return ExtraCrud.Queries<T>().InsertIfNotExists(connection, obj, transaction, commandTimeout);
		}

		/// <summary>
		/// Inserts a row if it does not exist asynchronously.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="obj">The object to insert.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the the row was inserted; false otherwise.</returns>
		public static async Task<bool> InsertIfNotExistsAsync<T>(this IDbConnection connection, T obj, IDbTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			return await Task.Run(() => InsertIfNotExists(connection, obj, transaction, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Counts all rows.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of rows.</returns>
		public static int RecordCount<T>(this IDbConnection connection, IDbTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			int count = ExtraCrud.Queries<T>().RecordCount(connection, "", null, transaction, commandTimeout);
			return count;
		}

		/// <summary>
		/// Counts the number of rows that match the given condition.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of rows that match the given condition.</returns>
		public static int RecordCount<T>(this IDbConnection connection, string whereCondition, object param = null, IDbTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			int count = ExtraCrud.Queries<T>().RecordCount(connection, whereCondition, param, transaction, commandTimeout);
			return count;
		}

		/// <summary>
		/// Counts the number of rows asynchronously.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of rows.</returns>
		public static async Task<int> RecordCountAsync<T>(this IDbConnection connection, IDbTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			return await Task.Run(() => RecordCount<T>(connection, "", null, transaction, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Counts the number of rows that match the given condition asynchronously.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of rows that match the given condition.</returns>
		public static async Task<int> RecordCountAsync<T>(this IDbConnection connection, string whereCondition, object param = null, IDbTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			return await Task.Run(() => RecordCount<T>(connection, whereCondition, param, transaction, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Updates a row.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="obj">The object to update.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the row was updated; false otherwise.</returns>
		public static bool Update<T>(this IDbConnection connection, object obj, IDbTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			bool success = ExtraCrud.Queries<T>().UpdateObj(connection, obj, transaction, commandTimeout);
			return success;
		}

		/// <summary>
		/// Updates a row.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="obj">The object to update.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the row was updated; false otherwise.</returns>
		public static bool Update<T>(this IDbConnection connection, T obj, IDbTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			bool success = ExtraCrud.Queries<T>().Update(connection, obj, transaction, commandTimeout);
			return success;
		}

		/// <summary>
		/// Updates a row asynchronously.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="obj">The object to update.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the row was updated; false otherwise.</returns>
		public static async Task<bool> UpdateAsync<T>(this IDbConnection connection, object obj, IDbTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			return await Task.Run(() => Update<T>(connection, obj, transaction, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Updates a row asynchronously.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="obj">The object to update.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the row was updated; false otherwise.</returns>
		public static async Task<bool> UpdateAsync<T>(this IDbConnection connection, T obj, IDbTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			return await Task.Run(() => Update<T>(connection, obj, transaction, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Upserts a row.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="obj">The object to upsert.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the object was upserted; false otherwise.</returns>
		public static bool Upsert<T>(this IDbConnection connection, T obj, IDbTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			bool updated = ExtraCrud.Queries<T>().Upsert(connection, obj, transaction, commandTimeout);
			return updated;
		}

		/// <summary>
		/// Upserts a row asynchronously.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="obj">The object to upsert.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the object was upserted; false otherwise.</returns>
		public static async Task<bool> UpsertAsync<T>(this IDbConnection connection, T obj, IDbTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			return await Task.Run(() => Upsert(connection, obj, transaction, commandTimeout)).ConfigureAwait(false);
		}
	}
}
