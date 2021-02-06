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
using System.Linq.Expressions;

namespace Dapper
{
	/// <summary>
	/// Dapper extension methods for <see cref="IDbConnection"/> and <see cref="SqlConnection"/>.
	/// </summary>
	public static class DapperExtraWhereExtensions
	{
		/// <summary>
		/// Deletes the rows that match the given condition.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of deleted rows.</returns>
		public static int DeleteList<T>(this IDbConnection connection, Expression<Func<T, bool>> whereExpr, IDbTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			ISqlQueries<T> queries = ExtraCrud.Queries<T>();
			WhereConditionData<T> data = queries.Compile(whereExpr);
			int count = queries.DeleteList(connection, data.WhereCondition, data.Param, transaction, commandTimeout);
			return count;
		}

		/// <summary>
		/// Deletes the rows that match the given condition asynchronously.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of deleted rows.</returns>
		public static async Task<int> DeleteListAsync<T>(this IDbConnection connection, Expression<Func<T, bool>> whereExpr, IDbTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			return await Task.Run(() => DeleteList(connection, whereExpr, transaction, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Selects the rows that match the given condition.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public static IEnumerable<T> GetDistinct<T>(this IDbConnection connection, Type columnFilter, Expression<Func<T, bool>> whereExpr, IDbTransaction transaction = null, bool buffered = true, int commandTimeout = 30)
			where T : class
		{
			ISqlQueries<T> queries = ExtraCrud.Queries<T>();
			WhereConditionData<T> data = queries.Compile(whereExpr);
			IEnumerable<T> list = queries.GetDistinct(connection, columnFilter, data.WhereCondition, data.Param, transaction, buffered, commandTimeout);
			return list;
		}

		/// <summary>
		/// Selects the rows that match the given condition asynchronously.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public static async Task<IEnumerable<T>> GetDistinctAsync<T>(this IDbConnection connection, Type columnFilter, Expression<Func<T, bool>> whereExpr, IDbTransaction transaction = null, bool buffered = true, int commandTimeout = 30)
			where T : class
		{
			return await Task.Run(() => GetDistinct(connection, columnFilter, whereExpr, transaction, buffered, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Selects the rows that match the given condition.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="limit">The maximum number of rows.</param>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public static IEnumerable<T> GetDistinctLimit<T>(this IDbConnection connection, int limit, Type columnFilter, Expression<Func<T, bool>> whereExpr, IDbTransaction transaction = null, bool buffered = true, int commandTimeout = 30)
			where T : class
		{
			ISqlQueries<T> queries = ExtraCrud.Queries<T>();
			WhereConditionData<T> data = queries.Compile(whereExpr);
			IEnumerable<T> list = queries.GetDistinctLimit(connection, limit, columnFilter, data.WhereCondition, data.Param, transaction, buffered, commandTimeout);
			return list;
		}

		/// <summary>
		/// Selects the rows that match the given condition asynchronously.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="limit">The maximum number of rows.</param>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public static async Task<IEnumerable<T>> GetDistinctLimitAsync<T>(this IDbConnection connection, int limit, Type columnFilter, Expression<Func<T, bool>> whereExpr, IDbTransaction transaction = null, bool buffered = true, int commandTimeout = 30)
			where T : class
		{
			return await Task.Run(() => GetDistinctLimit(connection, limit, columnFilter, whereExpr, transaction, buffered, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Selects the rows with the given keys.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <typeparam name="KeyType">The key type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The keys that match the given condition.</returns>
		public static IEnumerable<KeyType> GetKeys<T, KeyType>(this IDbConnection connection, Expression<Func<T, bool>> whereExpr, IDbTransaction transaction = null, bool buffered = true, int commandTimeout = 30)
			where T : class
		{
			ISqlQueries<T> queries = ExtraCrud.Queries<T>();
			WhereConditionData<T> data = queries.Compile(whereExpr);
			IEnumerable<object> keys = queries.GetKeysKeys(connection, data.WhereCondition, data.Param, transaction, buffered, commandTimeout);
			if (typeof(KeyType) == typeof(long)) {
				if (keys.Any()) {
					Type type = keys.First().GetType();
					if (type == typeof(int)) {
						keys = keys.Select(k => (object) (long) (int) k);
					}
				}
			}
			IEnumerable<KeyType> castedKeys = keys.Select(k => (KeyType) k);
			return castedKeys;
		}

		/// <summary>
		/// Selects the keys that match the given condition.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The keys that match the given condition.</returns>
		public static IEnumerable<T> GetKeys<T>(this IDbConnection connection, Expression<Func<T, bool>> whereExpr, IDbTransaction transaction = null, bool buffered = true, int commandTimeout = 30)
			where T : class
		{
			ISqlQueries<T> queries = ExtraCrud.Queries<T>();
			WhereConditionData<T> data = queries.Compile(whereExpr);
			IEnumerable<T> keys = queries.GetKeys(connection, data.WhereCondition, data.Param, transaction, buffered, commandTimeout);
			return keys;
		}

		/// <summary>
		/// Selects the rows with the given keys asynchronously.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <typeparam name="KeyType">The key type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The keys that match the given condition.</returns>
		public static async Task<IEnumerable<KeyType>> GetKeysAsync<T, KeyType>(this IDbConnection connection, Expression<Func<T, bool>> whereExpr, IDbTransaction transaction = null, bool buffered = true, int commandTimeout = 30)
			where T : class
		{
			return await Task.Run(() => GetKeys<T, KeyType>(connection, whereExpr, transaction, buffered, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Selects the keys that match the given condition asynchronously.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The keys that match the given condition.</returns>
		public static async Task<IEnumerable<T>> GetKeysAsync<T>(this IDbConnection connection, Expression<Func<T, bool>> whereExpr, IDbTransaction transaction = null, bool buffered = true, int commandTimeout = 30)
			where T : class
		{
			return await Task.Run(() => GetKeys(connection, whereExpr, transaction, buffered, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Selects a limited number of rows that match the given condition.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="limit">The maximum number of rows.</param>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The limited number of rows that match the given condition.</returns>
		public static IEnumerable<T> GetLimit<T>(this IDbConnection connection, int limit, Expression<Func<T, bool>> whereExpr, IDbTransaction transaction = null, bool buffered = true, int commandTimeout = 30)
			where T : class
		{
			ISqlQueries<T> queries = ExtraCrud.Queries<T>();
			WhereConditionData<T> data = queries.Compile(whereExpr);
			IEnumerable<T> list = queries.GetLimit(connection, limit, data.WhereCondition, data.Param, transaction, buffered, commandTimeout);
			return list;
		}

		/// <summary>
		/// Selects a limited number of rows that match the given condition.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="limit">The maximum number of rows.</param>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>A limited number of rows that match the given condition.</returns>
		public static IEnumerable<T> GetLimit<T>(this IDbConnection connection, int limit, Type columnFilter, Expression<Func<T, bool>> whereExpr, IDbTransaction transaction = null, bool buffered = true, int commandTimeout = 30)
			where T : class
		{
			ISqlQueries<T> queries = ExtraCrud.Queries<T>();
			WhereConditionData<T> data = queries.Compile(whereExpr);
			IEnumerable<T> list = queries.GetFilterLimit(connection, limit, columnFilter, data.WhereCondition, data.Param, transaction, buffered, commandTimeout);
			return list;
		}

		/// <summary>
		/// Selects the rows that match the given condition asynchronously.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="limit">The maximum number of rows.</param>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public static async Task<IEnumerable<T>> GetLimitAsync<T>(this IDbConnection connection, int limit, Expression<Func<T, bool>> whereExpr, IDbTransaction transaction = null, bool buffered = true, int commandTimeout = 30)
			where T : class
		{
			return await Task.Run(() => GetLimit(connection, limit, whereExpr, transaction, buffered, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Selects the rows that match the given condition asynchronously.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="limit">The maximum number of rows.</param>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public static async Task<IEnumerable<T>> GetLimitAsync<T>(this IDbConnection connection, int limit, Type columnFilter, Expression<Func<T, bool>> whereExpr, IDbTransaction transaction = null, bool buffered = true, int commandTimeout = 30)
			where T : class
		{
			return await Task.Run(() => GetLimit(connection, limit, columnFilter, whereExpr, transaction, buffered, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Selects the rows that match the given condition.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public static IEnumerable<T> GetList<T>(this IDbConnection connection, Expression<Func<T, bool>> whereExpr, IDbTransaction transaction = null, bool buffered = true, int commandTimeout = 30)
			where T : class
		{
			ISqlQueries<T> queries = ExtraCrud.Queries<T>();
			WhereConditionData<T> data = queries.Compile(whereExpr);
			IEnumerable<T> list = queries.GetList(connection, data.WhereCondition, data.Param, transaction, buffered, commandTimeout);
			return list;
		}

		/// <summary>
		/// Selects the rows that match the given condition.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public static IEnumerable<T> GetList<T>(this IDbConnection connection, Type columnFilter, Expression<Func<T, bool>> whereExpr, IDbTransaction transaction = null, bool buffered = true, int commandTimeout = 30)
			where T : class
		{
			ISqlQueries<T> queries = ExtraCrud.Queries<T>();
			WhereConditionData<T> data = queries.Compile(whereExpr);
			IEnumerable<T> list = queries.GetFilter(connection, columnFilter, data.WhereCondition, data.Param, transaction, buffered, commandTimeout);
			return list;
		}

		/// <summary>
		/// Selects the rows that match the given condition asynchronously.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public static async Task<IEnumerable<T>> GetListAsync<T>(this IDbConnection connection, Expression<Func<T, bool>> whereExpr, IDbTransaction transaction = null, bool buffered = true, int commandTimeout = 30)
			where T : class
		{
			return await Task.Run(() => GetList(connection, whereExpr, transaction, buffered, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Selects the rows that match the given condition asynchronously.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public static async Task<IEnumerable<T>> GetListAsync<T>(this IDbConnection connection, Type columnFilter, Expression<Func<T, bool>> whereExpr, IDbTransaction transaction = null, bool buffered = true, int commandTimeout = 30)
			where T : class
		{
			return await Task.Run(() => GetList(connection, columnFilter, whereExpr, transaction, buffered, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Counts the number of rows that match the given condition.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of rows that match the given condition.</returns>
		public static int RecordCount<T>(this IDbConnection connection, Expression<Func<T, bool>> whereExpr, IDbTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			ISqlQueries<T> queries = ExtraCrud.Queries<T>();
			WhereConditionData<T> data = queries.Compile(whereExpr);
			int count = queries.RecordCount(connection, data.WhereCondition, data.Param, transaction, commandTimeout);
			return count;
		}

		/// <summary>
		/// Counts the number of rows that match the given condition asynchronously.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of rows that match the given condition.</returns>
		public static async Task<int> RecordCountAsync<T>(this IDbConnection connection, Expression<Func<T, bool>> whereExpr, IDbTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			return await Task.Run(() => RecordCount(connection, whereExpr, transaction, commandTimeout)).ConfigureAwait(false);
		}
	}
}
