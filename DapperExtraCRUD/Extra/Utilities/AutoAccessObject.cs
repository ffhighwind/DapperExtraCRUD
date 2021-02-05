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
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Dapper.Extra.Utilities
{
	/// <summary>
	/// An object that interacts with an <see cref="ISqlQueries{T}"/>.
	/// Automatically connects to a database and performs SQL operations in a thread-safe way.
	/// </summary>
	/// <typeparam name="T">The table type.</typeparam>
	public class AutoAccessObject<T> : IAccessObject<T>, ITransactionAccessObjectSync<T>, ITransactionAccessObjectAsync<T>
		where T : class
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AutoAccessObject{T}"/> class.
		/// </summary>
		/// <param name="connectionString">The connection string used for SQL commands. This is ignored for transaction based commands.</param>
		public AutoAccessObject(string connectionString = null)
		{
			ConnectionString = connectionString;
			var builder = ExtraCrud.Builder<T>();
			Queries = builder.Queries;
		}

		/// <summary>
		/// The connection string used for SQL commands. This is ignored for transaction based commands.
		/// </summary>
		public string ConnectionString { get; set; }

		/// <summary>
		/// The SQL commands for a given type.
		/// </summary>
		protected ISqlQueries<T> Queries { get; private set; }

		#region Bulk

		/// <summary>
		/// Deletes the rows with the given keys.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="keys">The keys for the rows to delete.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of deleted rows.</returns>
		public int BulkDelete(IDbTransaction transaction, IEnumerable<object> keys, int commandTimeout = 30)
		{
			int count = Queries.BulkDeleteKeys(transaction.Connection, keys, transaction, commandTimeout);
			return count;
		}

		/// <summary>
		/// Deletes the given rows.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="objs">The objects to delete.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of deleted rows.</returns>
		public int BulkDelete(IDbTransaction transaction, IEnumerable<T> objs, int commandTimeout = 30)
		{
			int count = Queries.BulkDelete(transaction.Connection, objs, transaction, commandTimeout);
			return count;
		}

		/// <summary>
		/// Deletes the rows with the given keys.
		/// </summary>
		/// <param name="keys">The keys for the rows to delete.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of deleted rows.</returns>
		public override int BulkDelete(IEnumerable<object> keys, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				int count = Queries.BulkDeleteKeys(conn, keys, null, commandTimeout);
				return count;
			}
		}

		/// <summary>
		/// Deletes the given rows.
		/// </summary>
		/// <param name="objs">The objects to delete.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of deleted rows.</returns>
		public override int BulkDelete(IEnumerable<T> objs, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				int count = Queries.BulkDelete(conn, objs, null, commandTimeout);
				return count;
			}
		}

		/// <summary>
		/// Deletes the rows with the given keys.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="keys">The keys for the rows to delete.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of deleted rows.</returns>
		public async Task<int> BulkDeleteAsync(IDbTransaction transaction, IEnumerable<object> keys, int commandTimeout = 30)
		{
			return await Task.Run(() => BulkDelete(transaction, keys, commandTimeout));
		}

		/// <summary>
		/// Deletes the given rows.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="objs">The objects to delete.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of deleted rows.</returns>
		public async Task<int> BulkDeleteAsync(IDbTransaction transaction, IEnumerable<T> objs, int commandTimeout = 30)
		{
			return await Task.Run(() => BulkDelete(transaction, objs, commandTimeout));
		}

		/// <summary>
		/// Selects the rows with the given keys.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="keys">The keys of the rows to select.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows with the given keys.</returns>
		public IEnumerable<T> BulkGet(IDbTransaction transaction, IEnumerable<object> keys, int commandTimeout = 30)
		{
			IEnumerable<T> list = Queries.BulkGetKeys(transaction.Connection, keys, transaction, commandTimeout);
			return list;
		}

		/// <summary>
		/// Selects the rows with the given keys.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="objs">The objects to select.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given keys.</returns>
		public IEnumerable<T> BulkGet(IDbTransaction transaction, IEnumerable<T> objs, int commandTimeout = 30)
		{
			IEnumerable<T> list = Queries.BulkGet(transaction.Connection, objs, transaction, commandTimeout);
			return list;
		}

		/// <summary>
		/// Selects the rows with the given keys.
		/// </summary>
		/// <param name="keys">The keys of the rows to select.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows with the given keys.</returns>
		public override IEnumerable<T> BulkGet(IEnumerable<object> keys, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				List<T> list = Queries.BulkGetKeys(conn, keys, null, commandTimeout).AsList();
				return list;
			}
		}

		/// <summary>
		/// Selects the rows with the given keys.
		/// </summary>
		/// <param name="objs">The objects to select.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given keys.</returns>
		public override IEnumerable<T> BulkGet(IEnumerable<T> objs, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				List<T> list = Queries.BulkGet(conn, objs, null, commandTimeout).AsList();
				return list;
			}
		}

		/// <summary>
		/// Selects the rows with the given keys.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="keys">The keys of the rows to select.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows with the given keys.</returns>
		public async Task<IEnumerable<T>> BulkGetAsync(IDbTransaction transaction, IEnumerable<object> keys, int commandTimeout = 30)
		{
			return await Task.Run(() => BulkGet(transaction, keys, commandTimeout));
		}

		/// <summary>
		/// Selects the rows with the given keys.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="objs">The objects to select.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given keys.</returns>
		public async Task<IEnumerable<T>> BulkGetAsync(IDbTransaction transaction, IEnumerable<T> objs, int commandTimeout = 30)
		{
			return await Task.Run(() => BulkGet(transaction, objs, commandTimeout));
		}

		/// <summary>
		/// Inserts the given rows.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="objs">The objects to insert.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		public void BulkInsert(IDbTransaction transaction, IEnumerable<T> objs, int commandTimeout = 30)
		{
			Queries.BulkInsert(transaction.Connection, objs, transaction, commandTimeout);
		}

		/// <summary>
		/// Inserts the given rows.
		/// </summary>
		/// <param name="objs">The objects to insert.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		public override void BulkInsert(IEnumerable<T> objs, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				Queries.BulkInsert(conn, objs, null, commandTimeout);
			}
		}

		/// <summary>
		/// Inserts the given rows.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="objs">The objects to insert.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		public async Task BulkInsertAsync(IDbTransaction transaction, IEnumerable<T> objs, int commandTimeout = 30)
		{
			await Task.Run(() => BulkInsert(transaction, objs, commandTimeout));
		}

		/// <summary>
		/// Inserts the given rows if they do not exist.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="objs">The objects to insert.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of rows inserted.</returns>
		public int BulkInsertIfNotExists(IDbTransaction transaction, IEnumerable<T> objs, int commandTimeout = 30)
		{
			int count = Queries.BulkInsertIfNotExists(transaction.Connection, objs, transaction, commandTimeout);
			return count;
		}

		/// <summary>
		/// Inserts the given rows if they do not exist.
		/// </summary>
		/// <param name="objs">The objects to insert.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of rows inserted.</returns>
		public override int BulkInsertIfNotExists(IEnumerable<T> objs, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				int count = Queries.BulkInsertIfNotExists(conn, objs, null, commandTimeout);
				return count;
			}
		}

		/// <summary>
		/// Inserts the given rows if they do not exist.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="objs">The objects to insert.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of rows inserted.</returns>
		public async Task<int> BulkInsertIfNotExistsAsync(IDbTransaction transaction, IEnumerable<T> objs, int commandTimeout = 30)
		{
			return await Task.Run(() => BulkInsertIfNotExists(transaction, objs, commandTimeout));
		}

		/// <summary>
		/// Updates the given rows.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="objs">The objects to update.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of updated rows.</returns>
		public int BulkUpdate(IDbTransaction transaction, IEnumerable<T> objs, int commandTimeout = 30)
		{
			int count = Queries.BulkUpdate(transaction.Connection, objs, transaction, commandTimeout);
			return count;
		}

		/// <summary>
		/// Updates the given rows.
		/// </summary>
		/// <param name="objs">The objects to update.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of updated rows.</returns>
		public override int BulkUpdate(IEnumerable<T> objs, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				int count = Queries.BulkUpdate(conn, objs, null, commandTimeout);
				return count;
			}
		}

		/// <summary>
		/// Updates the given rows.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="objs">The objects to update.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of updated rows.</returns>
		public async Task<int> BulkUpdateAsync(IDbTransaction transaction, IEnumerable<T> objs, int commandTimeout = 30)
		{
			return await Task.Run(() => BulkUpdate(transaction, objs, commandTimeout));
		}

		/// <summary>
		/// Upserts the given rows.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="objs">The objects to upsert.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of upserted rows.</returns>
		public int BulkUpsert(IDbTransaction transaction, IEnumerable<T> objs, int commandTimeout = 30)
		{
			int count = Queries.BulkUpsert(transaction.Connection, objs, transaction, commandTimeout);
			return count;
		}

		/// <summary>
		/// Upserts the given rows.
		/// </summary>
		/// <param name="objs">The objects to upsert.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of upserted rows.</returns>
		public override int BulkUpsert(IEnumerable<T> objs, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				int count = Queries.BulkUpsert(conn, objs, null, commandTimeout);
				return count;
			}
		}

		/// <summary>
		/// Upserts the given rows.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="objs">The objects to upsert.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of upserted rows.</returns>
		public async Task<int> BulkUpsertAsync(IDbTransaction transaction, IEnumerable<T> objs, int commandTimeout = 30)
		{
			return await Task.Run(() => BulkUpsert(transaction, objs, commandTimeout));
		}

		#endregion Bulk

		#region Other Methods

		/// <summary>
		/// Deletes the row with the given key.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="key">The key of the row to delete.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the row was deleted; false otherwise.</returns>
		public bool Delete(IDbTransaction transaction, object key, int commandTimeout = 30)
		{
			bool success = Queries.DeleteKey(transaction.Connection, key, transaction, commandTimeout);
			return success;
		}

		/// <summary>
		/// Deletes the given row.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="obj">The object to delete.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the row was deleted; false otherwise.</returns>
		public bool Delete(IDbTransaction transaction, T obj, int commandTimeout = 30)
		{
			bool success = Queries.Delete(transaction.Connection, obj, transaction, commandTimeout);
			return success;
		}

		/// <summary>
		/// Deletes the row with the given key.
		/// </summary>
		/// <param name="key">The key of the row to delete.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the row was deleted; false otherwise.</returns>
		public override bool Delete(object key, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				bool success = Queries.DeleteKey(conn, key, null, commandTimeout);
				return success;
			}
		}

		/// <summary>
		/// Deletes the given row.
		/// </summary>
		/// <param name="obj">The object to delete.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the row was deleted; false otherwise.</returns>
		public override bool Delete(T obj, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				bool success = Queries.Delete(conn, obj, null, commandTimeout);
				return success;
			}
		}

		/// <summary>
		/// Deletes the row with the given key.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="key">The key of the row to delete.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the row was deleted; false otherwise.</returns>
		public async Task<bool> DeleteAsync(IDbTransaction transaction, object key, int commandTimeout = 30)
		{
			return await Task.Run(() => Delete(transaction, key, commandTimeout));
		}

		/// <summary>
		/// Deletes the given row.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="obj">The object to delete.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the row was deleted; false otherwise.</returns>
		public async Task<bool> DeleteAsync(IDbTransaction transaction, T obj, int commandTimeout = 30)
		{
			return await Task.Run(() => Delete(transaction, obj, commandTimeout));
		}

		/// <summary>
		/// Deletes the rows that match the given condition.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of deleted rows.</returns>
		public int DeleteList(IDbTransaction transaction, string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			int count = Queries.DeleteList(transaction.Connection, whereCondition, param, transaction, commandTimeout);
			return count;
		}

		/// <summary>
		/// Deletes the rows that match the given condition.
		/// </summary>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of deleted rows.</returns>
		public override int DeleteList(string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				int count = Queries.DeleteList(conn, whereCondition, param, null, commandTimeout);
				return count;
			}
		}

		/// <summary>
		/// Deletes the rows that match the given condition.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of deleted rows.</returns>
		public async Task<int> DeleteListAsync(IDbTransaction transaction, string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			return await Task.Run(() => DeleteList(transaction, whereCondition, param, commandTimeout));
		}

		/// <summary>
		/// Selects the row with the given key.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="key">The key of the row to select.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The row with the given key.</returns>
		public T Get(IDbTransaction transaction, object key, int commandTimeout = 30)
		{
			T result = Queries.GetKey(transaction.Connection, key, transaction, commandTimeout);
			return result;
		}

		/// <summary>
		/// Selects a row.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="obj">The object to select.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The selected row if it exists; otherwise null.</returns>
		public T Get(IDbTransaction transaction, T obj, int commandTimeout = 30)
		{
			T result = Queries.Get(transaction.Connection, obj, transaction, commandTimeout);
			return result;
		}

		/// <summary>
		/// Selects the row with the given key.
		/// </summary>
		/// <param name="key">The key of the row to select.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The row with the given key.</returns>
		public override T Get(object key, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				T obj = Queries.GetKey(conn, key, null, commandTimeout);
				return obj;
			}
		}

		/// <summary>
		/// Selects a row.
		/// </summary>
		/// <param name="obj">The object to select.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The selected row if it exists; otherwise null.</returns>
		public override T Get(T obj, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				T result = Queries.Get(conn, obj, null, commandTimeout);
				return result;
			}
		}

		/// <summary>
		/// Selects the row with the given key.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="key">The key of the row to select.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The row with the given key.</returns>
		public async Task<T> GetAsync(IDbTransaction transaction, object key, int commandTimeout = 30)
		{
			return await Task.Run(() => Get(transaction, key, commandTimeout));
		}

		/// <summary>
		/// Selects a row.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="obj">The object to select.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The selected row if it exists; otherwise null.</returns>
		public async Task<T> GetAsync(IDbTransaction transaction, T obj, int commandTimeout = 30)
		{
			return await Task.Run(() => Get(transaction, obj, commandTimeout));
		}

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
		public IEnumerable<T> GetDistinct(IDbTransaction transaction, Type columnFilter, string whereCondition = "", object param = null, bool buffered = true, int commandTimeout = 30)
		{
			IEnumerable<T> list = Queries.GetDistinct(transaction.Connection, columnFilter, whereCondition, param, transaction, buffered, commandTimeout);
			return list;
		}

		/// <summary>
		/// Selects the rows that match the given condition.
		/// </summary>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="buffered">This argument is ignored and will always be true.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public override IEnumerable<T> GetDistinct(Type columnFilter, string whereCondition = "", object param = null, bool buffered = true, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				List<T> list = Queries.GetDistinct(conn, columnFilter, whereCondition, param, null, true, commandTimeout).AsList();
				return list;
			}
		}

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
		public async Task<IEnumerable<T>> GetDistinctAsync(IDbTransaction transaction, Type columnFilter, string whereCondition = "", object param = null, bool buffered = true, int commandTimeout = 30)
		{
			return await Task.Run(() => GetDistinct(transaction, columnFilter, whereCondition, param, buffered, commandTimeout));
		}

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
		public IEnumerable<T> GetDistinctLimit(IDbTransaction transaction, int limit, Type columnFilter, string whereCondition = "", object param = null, bool buffered = true, int commandTimeout = 30)
		{
			IEnumerable<T> list = Queries.GetDistinctLimit(transaction.Connection, limit, columnFilter, whereCondition, param, transaction, buffered, commandTimeout);
			return list;
		}

		/// <summary>
		/// Selects the rows that match the given condition.
		/// </summary>
		/// <param name="limit">The maximum number of rows.</param>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="buffered">This argument is ignored and will always be true.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public override IEnumerable<T> GetDistinctLimit(int limit, Type columnFilter, string whereCondition = "", object param = null, bool buffered = true, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				List<T> list = Queries.GetDistinctLimit(conn, limit, columnFilter, whereCondition, param, null, buffered, commandTimeout).AsList();
				return list;
			}
		}

		/// <summary>
		/// Selects the rows that match the given condition asynchronously.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="limit">The maximum number of rows.</param>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public async Task<IEnumerable<T>> GetDistinctLimitAsync(IDbTransaction transaction, int limit, Type columnFilter, string whereCondition = "", object param = null, bool buffered = true, int commandTimeout = 30)
		{
			return await Task.Run(() => GetDistinctLimit(transaction, limit, columnFilter, whereCondition, param, buffered, commandTimeout));
		}

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
		public IEnumerable<KeyType> GetKeys<KeyType>(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int commandTimeout = 30)
		{
			IEnumerable<object> keys = Queries.GetKeysKeys(transaction.Connection, whereCondition, param, transaction, buffered, commandTimeout);
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
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The keys that match the given condition.</returns>
		public IEnumerable<T> GetKeys(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int commandTimeout = 30)
		{
			IEnumerable<T> list = Queries.GetKeys(transaction.Connection, whereCondition, param, transaction, buffered, commandTimeout);
			return list;
		}

		/// <summary>
		/// Selects the rows with the given keys.
		/// </summary>
		/// <typeparam name="KeyType">The key type.</typeparam>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="buffered">This argument is ignored and will always be true.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The keys that match the given condition.</returns>
		public override IEnumerable<KeyType> GetKeys<KeyType>(string whereCondition = "", object param = null, bool buffered = true, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				IEnumerable<object> keys = Queries.GetKeysKeys(conn, whereCondition, param, null, true, commandTimeout);
				List<KeyType> castedKeys = keys.Select(k => (KeyType) k).AsList();
				return castedKeys;
			}
		}

		/// <summary>
		/// Selects the keys that match the given condition.
		/// </summary>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="buffered">This argument is ignored and will always be true.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The keys that match the given condition.</returns>
		public override IEnumerable<T> GetKeys(string whereCondition = "", object param = null, bool buffered = true, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				IEnumerable<T> keys = Queries.GetKeys(conn, whereCondition, param, null, true, commandTimeout);
				return keys;
			}
		}

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
		public async Task<IEnumerable<KeyType>> GetKeysAsync<KeyType>(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int commandTimeout = 30)
		{
			return await Task.Run(() => GetKeys<KeyType>(transaction, whereCondition, param, buffered, commandTimeout));
		}

		/// <summary>
		/// Selects the keys that match the given condition.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The keys that match the given condition.</returns>
		public async Task<IEnumerable<T>> GetKeysAsync(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int commandTimeout = 30)
		{
			return await Task.Run(() => GetKeys(transaction, whereCondition, param, buffered, commandTimeout));
		}

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
		public IEnumerable<T> GetLimit(IDbTransaction transaction, int limit, string whereCondition = "", object param = null, bool buffered = true, int commandTimeout = 30)
		{
			IEnumerable<T> list = Queries.GetLimit(transaction.Connection, limit, whereCondition, param, transaction, buffered, commandTimeout);
			return list;
		}

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
		public IEnumerable<T> GetLimit(IDbTransaction transaction, int limit, Type columnFilter, string whereCondition = "", object param = null, bool buffered = true, int commandTimeout = 30)
		{
			IEnumerable<T> list = Queries.GetFilterLimit(transaction.Connection, limit, columnFilter, whereCondition, param, transaction, buffered, commandTimeout);
			return list;
		}

		/// <summary>
		/// Selects a limited number of rows that match the given condition.
		/// </summary>
		/// <param name="limit">The maximum number of rows.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="buffered">This argument is ignored and will always be true.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The limited number of rows that match the given condition.</returns>
		public override IEnumerable<T> GetLimit(int limit, string whereCondition = "", object param = null, bool buffered = true, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				List<T> list = Queries.GetLimit(conn, limit, whereCondition, param, null, true, commandTimeout).AsList();
				return list;
			}
		}

		/// <summary>
		/// Selects a limited number of rows that match the given condition.
		/// </summary>
		/// <param name="limit">The maximum number of rows.</param>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="buffered">This argument is ignored and will always be true.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>A limited number of rows that match the given condition.</returns>
		public override IEnumerable<T> GetLimit(int limit, Type columnFilter, string whereCondition = "", object param = null, bool buffered = true, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				List<T> list = Queries.GetFilterLimit(conn, limit, columnFilter, whereCondition, param, null, true, commandTimeout).AsList();
				return list;
			}
		}

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
		public async Task<IEnumerable<T>> GetLimitAsync(IDbTransaction transaction, int limit, string whereCondition = "", object param = null, bool buffered = true, int commandTimeout = 30)
		{
			return await Task.Run(() => GetLimit(transaction, limit, whereCondition, param, buffered, commandTimeout));
		}

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
		public async Task<IEnumerable<T>> GetLimitAsync(IDbTransaction transaction, int limit, Type columnFilter, string whereCondition = "", object param = null, bool buffered = true, int commandTimeout = 30)
		{
			return await Task.Run(() => GetLimit(transaction, limit, columnFilter, whereCondition, param, buffered, commandTimeout));
		}

		/// <summary>
		/// Selects the rows that match the given condition.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public IEnumerable<T> GetList(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int commandTimeout = 30)
		{
			IEnumerable<T> list = Queries.GetList(transaction.Connection, whereCondition, param, transaction, buffered, commandTimeout);
			return list;
		}

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
		public IEnumerable<T> GetList(IDbTransaction transaction, Type columnFilter, string whereCondition = "", object param = null, bool buffered = true, int commandTimeout = 30)
		{
			IEnumerable<T> list = Queries.GetFilter(transaction.Connection, columnFilter, whereCondition, param, transaction, buffered, commandTimeout);
			return list;
		}

		/// <summary>
		/// Selects the rows that match the given condition.
		/// </summary>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="buffered">This argument is ignored and will always be true.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public override IEnumerable<T> GetList(string whereCondition = "", object param = null, bool buffered = true, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				List<T> list = Queries.GetList(conn, whereCondition, param, null, true, commandTimeout).AsList();
				return list;
			}
		}

		/// <summary>
		/// Selects the rows that match the given condition.
		/// </summary>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="buffered">This argument is ignored and will always be true.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public override IEnumerable<T> GetList(Type columnFilter, string whereCondition = "", object param = null, bool buffered = true, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				List<T> list = Queries.GetFilter(conn, columnFilter, whereCondition, param, null, true, commandTimeout).AsList();
				return list;
			}
		}

		/// <summary>
		/// Selects the rows that match the given condition.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public async Task<IEnumerable<T>> GetListAsync(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int commandTimeout = 30)
		{
			return await Task.Run(() => GetList(transaction, whereCondition, param, buffered, commandTimeout));
		}

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
		public async Task<IEnumerable<T>> GetListAsync(IDbTransaction transaction, Type columnFilter, string whereCondition = "", object param = null, bool buffered = true, int commandTimeout = 30)
		{
			return await Task.Run(() => GetList(transaction, columnFilter, whereCondition, param, buffered, commandTimeout));
		}

		/// <summary>
		/// Inserts a row.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="obj">The object to insert.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		public void Insert(IDbTransaction transaction, T obj, int commandTimeout = 30)
		{
			Queries.Insert(transaction.Connection, obj, transaction, commandTimeout);
		}

		/// <summary>
		/// Inserts a row.
		/// </summary>
		/// <param name="obj">The object to insert.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		public override void Insert(T obj, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				Queries.Insert(conn, obj, null, commandTimeout);
			}
		}

		/// <summary>
		/// Inserts a row.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="obj">The object to insert.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		public async Task InsertAsync(IDbTransaction transaction, T obj, int commandTimeout = 30)
		{
			await Task.Run(() => Insert(transaction, obj, commandTimeout));
		}

		/// <summary>
		/// Inserts a row if it does not exist.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="obj">The object to insert.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the the row was inserted; false otherwise.</returns>
		public bool InsertIfNotExists(IDbTransaction transaction, T obj, int commandTimeout = 30)
		{
			bool success = Queries.InsertIfNotExists(transaction.Connection, obj, transaction, commandTimeout);
			return success;
		}

		/// <summary>
		/// Inserts a row if it does not exist.
		/// </summary>
		/// <param name="obj">The object to insert.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the the row was inserted; false otherwise.</returns>
		public override bool InsertIfNotExists(T obj, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				bool success = Queries.InsertIfNotExists(conn, obj, null, commandTimeout);
				return success;
			}
		}

		/// <summary>
		/// Inserts a row if it does not exist.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="obj">The object to insert.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the the row was inserted; false otherwise.</returns>
		public async Task<bool> InsertIfNotExistsAsync(IDbTransaction transaction, T obj, int commandTimeout = 30)
		{
			return await Task.Run(() => InsertIfNotExists(transaction, obj, commandTimeout));
		}

		/// <summary>
		/// Counts the number of rows that match the given condition.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of rows that match the given condition.</returns>
		public int RecordCount(IDbTransaction transaction, string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			int count = Queries.RecordCount(transaction.Connection, whereCondition, param, transaction, commandTimeout);
			return count;
		}

		/// <summary>
		/// Counts the number of rows that match the given condition.
		/// </summary>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of rows that match the given condition.</returns>
		public override int RecordCount(string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				int count = Queries.RecordCount(conn, whereCondition, param, null, commandTimeout);
				return count;
			}
		}

		/// <summary>
		/// Counts the number of rows that match the given condition.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of rows that match the given condition.</returns>
		public async Task<int> RecordCountAsync(IDbTransaction transaction, string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			return await Task.Run(() => RecordCount(transaction, whereCondition, param, commandTimeout));
		}

		/// <summary>
		/// Truncates all rows.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		public void Truncate(IDbTransaction transaction, int commandTimeout = 30)
		{
			Queries.Truncate(transaction.Connection, transaction, commandTimeout);
		}

		/// <summary>
		/// Truncates all rows.
		/// </summary>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		public override void Truncate(int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				Queries.Truncate(conn, null, commandTimeout);
			}
		}

		/// <summary>
		/// Truncates all rows.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		public async Task TruncateAsync(IDbTransaction transaction, int commandTimeout = 30)
		{
			await Task.Run(() => Truncate(transaction, commandTimeout));
		}

		/// <summary>
		/// Updates a row.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="obj">The object to update.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the row was updated; false otherwise.</returns>
		public bool Update(IDbTransaction transaction, object obj, int commandTimeout = 30)
		{
			bool success = Queries.UpdateObj(transaction.Connection, obj, transaction, commandTimeout);
			return success;
		}

		/// <summary>
		/// Updates a row.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="obj">The object to update.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the row was updated; false otherwise.</returns>
		public bool Update(IDbTransaction transaction, T obj, int commandTimeout = 30)
		{
			bool success = Queries.Update(transaction.Connection, obj, transaction, commandTimeout);
			return success;
		}

		/// <summary>
		/// Updates a row.
		/// </summary>
		/// <param name="obj">The object to update.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the row was updated; false otherwise.</returns>
		public override bool Update(object obj, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				bool success = Queries.UpdateObj(conn, obj, null, commandTimeout);
				return success;
			}
		}

		/// <summary>
		/// Updates a row.
		/// </summary>
		/// <param name="obj">The object to update.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the row was updated; false otherwise.</returns>
		public override bool Update(T obj, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				bool success = Queries.Update(conn, obj, null, commandTimeout);
				return success;
			}
		}

		/// <summary>
		/// Updates a row.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="obj">The object to update.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the row was updated; false otherwise.</returns>
		public async Task<bool> UpdateAsync(IDbTransaction transaction, object obj, int commandTimeout = 30)
		{
			return await Task.Run(() => Update(transaction, obj, commandTimeout));
		}

		/// <summary>
		/// Updates a row.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="obj">The object to update.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the row was updated; false otherwise.</returns>
		public async Task<bool> UpdateAsync(IDbTransaction transaction, T obj, int commandTimeout = 30)
		{
			return await Task.Run(() => Update(transaction, obj, commandTimeout));
		}

		/// <summary>
		/// Upserts a row.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="obj">The object to upsert.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the object was upserted; false otherwise.</returns>
		public bool Upsert(IDbTransaction transaction, T obj, int commandTimeout = 30)
		{
			bool success = Queries.Upsert(transaction.Connection, obj, transaction, commandTimeout);
			return success;
		}

		/// <summary>
		/// Upserts a row.
		/// </summary>
		/// <param name="obj">The object to upsert.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the object was upserted; false otherwise.</returns>
		public override bool Upsert(T obj, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				bool success = Queries.Upsert(conn, obj, null, commandTimeout);
				return success;
			}
		}

		/// <summary>
		/// Upserts a row.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="obj">The object to upsert.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the object was upserted; false otherwise.</returns>
		public async Task<bool> UpsertAsync(IDbTransaction transaction, T obj, int commandTimeout = 30)
		{
			return await Task.Run(() => Upsert(transaction, obj, commandTimeout));
		}

		#endregion Other Methods

		#region WhereCondition

		/// <summary>
		/// Deletes the rows that match the given condition.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of deleted rows.</returns>
		public int DeleteList(IDbTransaction transaction, Expression<Func<T, bool>> whereExpr, int commandTimeout = 30)
		{
			QueryData<T> data = Queries.Compile(whereExpr);
			int count = DeleteList(transaction, data.WhereCondition, data.Param, commandTimeout);
			return count;
		}

		/// <summary>
		/// Deletes the rows that match the given condition asynchronously.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of deleted rows.</returns>
		public async Task<int> DeleteListAsync(IDbTransaction transaction, Expression<Func<T, bool>> whereExpr, int commandTimeout = 30)
		{
			return await Task.Run(() => DeleteList(transaction, whereExpr, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Selects the rows that match the given condition.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public IEnumerable<T> GetDistinct(IDbTransaction transaction, Type columnFilter, Expression<Func<T, bool>> whereExpr, bool buffered = true, int commandTimeout = 30)
		{
			QueryData<T> data = Queries.Compile(whereExpr);
			IEnumerable<T> list = GetDistinct(transaction, columnFilter, data.WhereCondition, data.Param, buffered, commandTimeout);
			return list;
		}

		/// <summary>
		/// Selects the rows that match the given condition asynchronously.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public async Task<IEnumerable<T>> GetDistinctAsync(IDbTransaction transaction, Type columnFilter, Expression<Func<T, bool>> whereExpr, bool buffered = true, int commandTimeout = 30)
		{
			return await Task.Run(() => GetDistinct(transaction, columnFilter, whereExpr, buffered, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Selects the rows that match the given condition.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="limit">The maximum number of rows.</param>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public IEnumerable<T> GetDistinctLimit(IDbTransaction transaction, int limit, Type columnFilter, Expression<Func<T, bool>> whereExpr, bool buffered = true, int commandTimeout = 30)
		{
			QueryData<T> data = Queries.Compile(whereExpr);
			IEnumerable<T> list = GetDistinctLimit(transaction, limit, columnFilter, data.WhereCondition, data.Param, buffered, commandTimeout);
			return list;
		}

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
		public async Task<IEnumerable<T>> GetDistinctLimitAsync(IDbTransaction transaction, int limit, Type columnFilter, Expression<Func<T, bool>> whereExpr, bool buffered = true, int commandTimeout = 30)
		{
			return await Task.Run(() => GetDistinctLimit(transaction, limit, columnFilter, whereExpr, buffered, commandTimeout)).ConfigureAwait(false);
		}
		/// <summary>
		/// Selects the rows with the given keys.
		/// </summary>
		/// <typeparam name="KeyType">The key type.</typeparam>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The keys that match the given condition.</returns>
		public IEnumerable<KeyType> GetKeys<KeyType>(IDbTransaction transaction, Expression<Func<T, bool>> whereExpr, bool buffered = true, int commandTimeout = 30)
		{
			QueryData<T> data = Queries.Compile(whereExpr);
			IEnumerable<KeyType> keys = GetKeys<KeyType>(transaction, data.WhereCondition, data.Param, buffered, commandTimeout);
			return keys;
		}

		/// <summary>
		/// Selects the keys that match the given condition.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The keys that match the given condition.</returns>
		public IEnumerable<T> GetKeys(IDbTransaction transaction, Expression<Func<T, bool>> whereExpr, bool buffered = true, int commandTimeout = 30)
		{
			QueryData<T> data = Queries.Compile(whereExpr);
			IEnumerable<T> keys = GetKeys(transaction, data.WhereCondition, data.Param, buffered, commandTimeout);
			return keys;
		}

		/// <summary>
		/// Selects the rows with the given keys asynchronously.
		/// </summary>
		/// <typeparam name="KeyType">The key type.</typeparam>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The keys that match the given condition.</returns>
		public async Task<IEnumerable<KeyType>> GetKeysAsync<KeyType>(IDbTransaction transaction, Expression<Func<T, bool>> whereExpr, bool buffered = true, int commandTimeout = 30)
		{
			return await Task.Run(() => GetKeys<KeyType>(transaction, whereExpr, buffered, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Selects the keys that match the given condition asynchronously.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The keys that match the given condition.</returns>
		public async Task<IEnumerable<T>> GetKeysAsync(IDbTransaction transaction, Expression<Func<T, bool>> whereExpr, bool buffered = true, int commandTimeout = 30)
		{
			return await Task.Run(() => GetKeys<T>(transaction, whereExpr, buffered, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Selects a limited number of rows that match the given condition.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="limit">The maximum number of rows.</param>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The limited number of rows that match the given condition.</returns>
		public IEnumerable<T> GetLimit(IDbTransaction transaction, int limit, Expression<Func<T, bool>> whereExpr, bool buffered = true, int commandTimeout = 30)
		{
			QueryData<T> data = Queries.Compile(whereExpr);
			IEnumerable<T> list = GetLimit(transaction, limit, data.WhereCondition, data.Param, buffered, commandTimeout);
			return list;
		}

		/// <summary>
		/// Selects a limited number of rows that match the given condition.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="limit">The maximum number of rows.</param>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>A limited number of rows that match the given condition.</returns>
		public IEnumerable<T> GetLimit(IDbTransaction transaction, int limit, Type columnFilter, Expression<Func<T, bool>> whereExpr, bool buffered = true, int commandTimeout = 30)
		{
			QueryData<T> data = Queries.Compile(whereExpr);
			IEnumerable<T> list = GetLimit(transaction, limit, columnFilter, data.WhereCondition, data.Param, buffered, commandTimeout);
			return list;
		}

		/// <summary>
		/// Selects the rows that match the given condition asynchronously.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="limit">The maximum number of rows.</param>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public async Task<IEnumerable<T>> GetLimitAsync(IDbTransaction transaction, int limit, Expression<Func<T, bool>> whereExpr, bool buffered = true, int commandTimeout = 30)
		{
			return await Task.Run(() => GetLimit(transaction, limit, whereExpr, buffered, commandTimeout)).ConfigureAwait(false);
		}

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
		public async Task<IEnumerable<T>> GetLimitAsync(IDbTransaction transaction, int limit, Type columnFilter, Expression<Func<T, bool>> whereExpr, bool buffered = true, int commandTimeout = 30)
		{
			return await Task.Run(() => GetLimit(transaction, limit, columnFilter, whereExpr, buffered, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Selects the rows that match the given condition.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public IEnumerable<T> GetList(IDbTransaction transaction, Expression<Func<T, bool>> whereExpr, bool buffered = true, int commandTimeout = 30)
		{
			QueryData<T> data = Queries.Compile(whereExpr);
			IEnumerable<T> list = GetList(transaction, data.WhereCondition, data.Param, buffered, commandTimeout);
			return list;
		}

		/// <summary>
		/// Selects the rows that match the given condition.
		/// </summary>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public override IEnumerable<T> GetList(Expression<Func<T, bool>> whereExpr, bool buffered = true, int commandTimeout = 30)
		{
			QueryData<T> data = Queries.Compile(whereExpr);
			IEnumerable<T> list = GetList(data.WhereCondition, data.Param, buffered, commandTimeout);
			return list;
		}

		/// <summary>
		/// Selects the rows that match the given condition.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public IEnumerable<T> GetList(IDbTransaction transaction, Type columnFilter, Expression<Func<T, bool>> whereExpr, bool buffered = true, int commandTimeout = 30)
		{
			QueryData<T> data = Queries.Compile(whereExpr);
			IEnumerable<T> list = GetList(transaction, columnFilter, data.WhereCondition, data.Param, buffered, commandTimeout);
			return list;
		}

		/// <summary>
		/// Selects the rows that match the given condition.
		/// </summary>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public override IEnumerable<T> GetList(Type columnFilter, Expression<Func<T, bool>> whereExpr, bool buffered = true, int commandTimeout = 30)
		{
			QueryData<T> data = Queries.Compile(whereExpr);
			IEnumerable<T> list = GetList(columnFilter, data.WhereCondition, data.Param, buffered, commandTimeout);
			return list;
		}

		/// <summary>
		/// Selects the rows that match the given condition asynchronously.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public async Task<IEnumerable<T>> GetListAsync(IDbTransaction transaction, Expression<Func<T, bool>> whereExpr, bool buffered = true, int commandTimeout = 30)
		{
			return await Task.Run(() => GetList(transaction, whereExpr, buffered, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Selects the rows that match the given condition asynchronously.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public async Task<IEnumerable<T>> GetListAsync(IDbTransaction transaction, Type columnFilter, Expression<Func<T, bool>> whereExpr, bool buffered = true, int commandTimeout = 30)
		{
			return await Task.Run(() => GetList(transaction, columnFilter, whereExpr, buffered, commandTimeout)).ConfigureAwait(false);
		}

		/// <summary>
		/// Counts the number of rows that match the given condition.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of rows that match the given condition.</returns>
		public int RecordCount(IDbTransaction transaction, Expression<Func<T, bool>> whereExpr, int commandTimeout = 30)
		{
			QueryData<T> data = Queries.Compile(whereExpr);
			int count = RecordCount(transaction, data.WhereCondition, data.Param, commandTimeout);
			return count;
		}

		/// <summary>
		/// Counts the number of rows that match the given condition asynchronously.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of rows that match the given condition.</returns>
		public async Task<int> RecordCountAsync(IDbTransaction transaction, Expression<Func<T, bool>> whereExpr, int commandTimeout = 30)
		{
			return await Task.Run(() => RecordCount(transaction, whereExpr, commandTimeout)).ConfigureAwait(false);
		}


		/// <summary>
		/// Deletes the rows that match the given condition.
		/// </summary>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of deleted rows.</returns>
		public override int DeleteList(Expression<Func<T, bool>> whereExpr, int commandTimeout = 30)
		{
			QueryData<T> data = Queries.Compile(whereExpr);
			int count = DeleteList(data.WhereCondition, data.Param, commandTimeout);
			return count;
		}

		/// <summary>
		/// Selects the rows that match the given condition.
		/// </summary>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public override IEnumerable<T> GetDistinct(Type columnFilter, Expression<Func<T, bool>> whereExpr, bool buffered = true, int commandTimeout = 30)
		{
			QueryData<T> data = Queries.Compile(whereExpr);
			IEnumerable<T> list = GetDistinct(columnFilter, data.WhereCondition, data.Param, buffered, commandTimeout);
			return list;
		}

		/// <summary>
		/// Selects the rows that match the given condition.
		/// </summary>
		/// <param name="limit">The maximum number of rows.</param>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public override IEnumerable<T> GetDistinctLimit(int limit, Type columnFilter, Expression<Func<T, bool>> whereExpr, bool buffered = true, int commandTimeout = 30)
		{
			QueryData<T> data = Queries.Compile(whereExpr);
			IEnumerable<T> list = GetDistinctLimit(limit, columnFilter, data.WhereCondition, data.Param, buffered, commandTimeout);
			return list;
		}

		/// <summary>
		/// Selects the rows with the given keys.
		/// </summary>
		/// <typeparam name="KeyType">The key type.</typeparam>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The keys that match the given condition.</returns>
		public override IEnumerable<KeyType> GetKeys<KeyType>(Expression<Func<T, bool>> whereExpr, bool buffered = true, int commandTimeout = 30)
		{
			QueryData<T> data = Queries.Compile(whereExpr);
			IEnumerable<KeyType> keys = GetKeys<KeyType>(data.WhereCondition, data.Param, buffered, commandTimeout);
			return keys;
		}

		/// <summary>
		/// Selects the keys that match the given condition.
		/// </summary>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The keys that match the given condition.</returns>
		public override IEnumerable<T> GetKeys(Expression<Func<T, bool>> whereExpr, bool buffered = true, int commandTimeout = 30)
		{
			QueryData<T> data = Queries.Compile(whereExpr);
			IEnumerable<T> keys = GetKeys<T>(data.WhereCondition, data.Param, buffered, commandTimeout);
			return keys;
		}

		/// <summary>
		/// Selects a limited number of rows that match the given condition.
		/// </summary>
		/// <param name="limit">The maximum number of rows.</param>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The limited number of rows that match the given condition.</returns>
		public override IEnumerable<T> GetLimit(int limit, Expression<Func<T, bool>> whereExpr, bool buffered = true, int commandTimeout = 30)
		{
			QueryData<T> data = Queries.Compile(whereExpr);
			IEnumerable<T> keys = GetLimit(limit, data.WhereCondition, data.Param, buffered, commandTimeout);
			return keys;
		}

		/// <summary>
		/// Selects a limited number of rows that match the given condition.
		/// </summary>
		/// <param name="limit">The maximum number of rows.</param>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>A limited number of rows that match the given condition.</returns>
		public override IEnumerable<T> GetLimit(int limit, Type columnFilter, Expression<Func<T, bool>> whereExpr, bool buffered = true, int commandTimeout = 30)
		{
			QueryData<T> data = Queries.Compile(whereExpr);
			IEnumerable<T> keys = GetLimit(limit, data.WhereCondition, data.Param, buffered, commandTimeout);
			return keys;
		}

		/// <summary>
		/// Counts the number of rows that match the given condition.
		/// </summary>
		/// <param name="whereExpr">The where condition to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of rows that match the given condition.</returns>
		public override int RecordCount(Expression<Func<T, bool>> whereExpr, int commandTimeout = 30)
		{
			QueryData<T> data = Queries.Compile(whereExpr);
			int count = RecordCount(data.WhereCondition, data.Param, commandTimeout);
			return count;
		}

		#endregion WhereCondition
	}
}
