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
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.Extra.Internal;

namespace Dapper.Extra.Utilities
{
	/// <summary>
	/// Automatically connects to the database and performs SQL operations.
	/// </summary>
	/// <typeparam name="T">The table type.</typeparam>
	public class AutoAccessObject<T> : IAccessObject<T>, ITransactionAccessObjectSync<T>
		where T : class
	{
		public AutoAccessObject(string connectionString = null)
		{
			ConnectionString = connectionString;
			SqlBuilder<T> builder = ExtraCrud.Builder<T>();
			Queries = builder.Queries;
			KeyQueries = builder.KeyBuilder?.QueriesObject;
		}

		protected ISqlQueries<T> Queries { get; }
		protected object KeyQueries { get; }

		public string ConnectionString { get; set; }

		#region ITransactionAccessObjectSync<T>
		public int BulkDelete(SqlTransaction transaction, IEnumerable<T> objs, int commandTimeout = 30)
		{
			int count = Queries.BulkDelete(transaction.Connection, objs, transaction, commandTimeout);
			return count;
		}

		public void BulkInsert(SqlTransaction transaction, IEnumerable<T> objs, int commandTimeout = 30)
		{
			Queries.BulkInsert(transaction.Connection, objs, transaction, commandTimeout);
		}

		public int BulkUpdate(SqlTransaction transaction, IEnumerable<T> objs, int commandTimeout = 30)
		{
			int count = Queries.BulkUpdate(transaction.Connection, objs, transaction, commandTimeout);
			return count;
		}

		public int BulkUpsert(SqlTransaction transaction, IEnumerable<T> objs, int commandTimeout = 30)
		{
			int count = Queries.BulkUpsert(transaction.Connection, objs, transaction, commandTimeout);
			return count;
		}

		public int BulkInsertIfNotExists(SqlTransaction transaction, IEnumerable<T> objs, int commandTimeout = 30)
		{
			return Queries.BulkInsertIfNotExists(transaction.Connection, objs, transaction, commandTimeout);
		}

		public bool Delete(IDbTransaction transaction, T obj, int commandTimeout = 30)
		{
			bool success = Queries.Delete(transaction.Connection, obj, transaction, commandTimeout);
			return success;
		}

		public int Delete(IDbTransaction transaction, string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			int count = Queries.DeleteList(transaction.Connection, whereCondition, param, transaction, commandTimeout);
			return count;
		}

		public T Get(IDbTransaction transaction, T obj, int commandTimeout = 30)
		{
			T value = Queries.Get(transaction.Connection, obj, transaction, commandTimeout);
			return value;
		}

		public IEnumerable<T> GetDistinct(IDbTransaction transaction, string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			IEnumerable<T> list = Queries.GetDistinct(transaction.Connection, typeof(T), whereCondition, param, transaction, true, commandTimeout);
			return list;
		}

		public IEnumerable<T> GetDistinct(IDbTransaction transaction, Type columnFilter, string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			IEnumerable<T> list = Queries.GetDistinct(transaction.Connection, columnFilter, whereCondition, param, transaction, true, commandTimeout);
			return list;
		}

		public IEnumerable<T> GetKeys(IDbTransaction transaction, string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			IEnumerable<T> list = Queries.GetKeys(transaction.Connection, whereCondition, param, transaction, true, commandTimeout);
			return list;
		}

		public IEnumerable<T> GetList(IDbTransaction transaction, string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			IEnumerable<T> list = Queries.GetList(transaction.Connection, whereCondition, param, transaction, true, commandTimeout);
			return list;
		}

		public IEnumerable<T> GetList(IDbTransaction transaction, Type columnFilter, string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			IEnumerable<T> list = Queries.GetFilter(transaction.Connection, columnFilter, whereCondition, param, transaction, true, commandTimeout);
			return list;
		}

		public IEnumerable<T> GetLimit(IDbTransaction transaction, int limit, string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			IEnumerable<T> list = Queries.GetLimit(transaction.Connection, limit, whereCondition, param, transaction, true, commandTimeout);
			return list;
		}

		public IEnumerable<T> GetLimit(IDbTransaction transaction, Type columnFilter, int limit, string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			IEnumerable<T> list = Queries.GetFilterLimit(transaction.Connection, columnFilter, limit, whereCondition, param, transaction, true, commandTimeout);
			return list;
		}

		public void Insert(IDbTransaction transaction, T obj, int commandTimeout = 30)
		{
			Queries.Insert(transaction.Connection, obj, transaction, commandTimeout);
		}

		public int RecordCount(IDbTransaction transaction, string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			int count = Queries.RecordCount(transaction.Connection, whereCondition, param, transaction, commandTimeout);
			return count;
		}

		public bool Update(IDbTransaction transaction, object obj, int commandTimeout = 30)
		{
			bool success = Queries.UpdateObj(transaction.Connection, obj, transaction, commandTimeout);
			return success;
		}

		public bool Update(IDbTransaction transaction, T obj, int commandTimeout = 30)
		{
			bool success = Queries.Update(transaction.Connection, obj, transaction, commandTimeout);
			return success;
		}

		public bool Upsert(IDbTransaction transaction, T obj, int commandTimeout = 30)
		{
			bool updated = Queries.Upsert(transaction.Connection, obj, transaction, commandTimeout);
			return updated;
		}

		public bool InsertIfNotExists(IDbTransaction transaction, T obj, int commandTimeout = 30)
		{
			bool updated = Queries.InsertIfNotExists(transaction.Connection, obj, transaction, commandTimeout);
			return updated;
		}

		public IEnumerable<T> GetDistinctLimit(IDbTransaction transaction, int limit, string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			IEnumerable<T> list = Queries.GetDistinctLimit(transaction.Connection, typeof(T), limit, whereCondition, param, transaction, true, commandTimeout);
			return list;
		}

		public IEnumerable<T> GetDistinctLimit(IDbTransaction transaction, Type columnFilter, int limit, string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			IEnumerable<T> list = Queries.GetDistinctLimit(transaction.Connection, columnFilter, limit, whereCondition, param, transaction, true, commandTimeout);
			return list;
		}

		public bool Delete<KeyType>(IDbTransaction transaction, KeyType key, int commandTimeout = 30)
		{
			bool success = ((ISqlQueries<T, KeyType>) KeyQueries).Delete(transaction.Connection, key, transaction, commandTimeout);
			return success;
		}

		public T Get<KeyType>(IDbTransaction transaction, KeyType key, int commandTimeout = 30)
		{
			T obj = ((ISqlQueries<T, KeyType>) KeyQueries).Get(transaction.Connection, key, transaction, commandTimeout);
			return obj;
		}

		public int BulkDelete<KeyType>(SqlTransaction transaction, IEnumerable<KeyType> keys, int commandTimeout = 30)
		{
			int count = ((ISqlQueries<T, KeyType>) KeyQueries).BulkDelete(transaction.Connection, keys, transaction, commandTimeout);
			return count;
		}

		public IEnumerable<KeyType> GetKeys<KeyType>(IDbTransaction transaction, string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			IEnumerable<KeyType> keys = ((ISqlQueries<T, KeyType>) KeyQueries).GetKeys(transaction.Connection, whereCondition, param, transaction, true, commandTimeout);
			return keys;
		}

		public List<T> BulkGet(SqlTransaction transaction, IEnumerable<T> keys, int commandTimeout = 30)
		{
			List<T> list = Queries.BulkGet(transaction.Connection, keys, transaction, commandTimeout).AsList();
			return list;
		}

		public List<T> BulkGet<KeyType>(SqlTransaction transaction, IEnumerable<KeyType> keys, int commandTimeout = 30)
		{
			List<T> list = ((ISqlQueries<T, KeyType>) KeyQueries).BulkGet(transaction.Connection, keys, transaction, commandTimeout);
			return list;
		}
		#endregion  ITransactionAccessObjectSync<T>

		#region ITransactionAccessObjectAsync<T>
		public async Task<int> BulkDeleteAsync(SqlTransaction transaction, IEnumerable<T> objs, int commandTimeout = 30)
		{
			return await Task.Run(() => BulkDelete(transaction, objs, commandTimeout));
		}

		public async Task BulkInsertAsync(SqlTransaction transaction, IEnumerable<T> objs, int commandTimeout = 30)
		{
			await Task.Run(() => BulkInsert(transaction, objs, commandTimeout));
		}

		public async Task<int> BulkUpdateAsync(SqlTransaction transaction, IEnumerable<T> objs, int commandTimeout = 30)
		{
			return await Task.Run(() => BulkUpdate(transaction, objs, commandTimeout));
		}

		public async Task<int> BulkUpsertAsync(SqlTransaction transaction, IEnumerable<T> objs, int commandTimeout = 30)
		{
			return await Task.Run(() => BulkUpsert(transaction, objs, commandTimeout));
		}

		public async Task<int> BulkInsertIfNotExistsAsync(SqlTransaction transaction, IEnumerable<T> objs, int commandTimeout = 30)
		{
			return await Task.Run(() => BulkInsertIfNotExists(transaction, objs, commandTimeout));
		}

		public async Task<bool> DeleteAsync(IDbTransaction transaction, T obj, int commandTimeout = 30)
		{
			return await Task.Run(() => Delete(transaction, obj, commandTimeout));
		}

		public async Task<int> DeleteAsync(IDbTransaction transaction, string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			return await Task.Run(() => Delete(transaction, whereCondition, param, commandTimeout));
		}

		public async Task<T> GetAsync(IDbTransaction transaction, T obj, int commandTimeout = 30)
		{
			return await Task.Run(() => Get(transaction, obj, commandTimeout));
		}

		public async Task<IEnumerable<T>> GetDistinctAsync(IDbTransaction transaction, string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			return await Task.Run(() => GetDistinct(transaction, whereCondition, param, commandTimeout));
		}

		public async Task<IEnumerable<T>> GetKeysAsync(IDbTransaction transaction, string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			return await Task.Run(() => GetKeys(transaction, whereCondition, param, commandTimeout));
		}

		public async Task<IEnumerable<T>> GetListAsync(IDbTransaction transaction, string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			return await Task.Run(() => GetList(transaction, whereCondition, param, commandTimeout));
		}

		public async Task<IEnumerable<T>> GetLimitAsync(IDbTransaction transaction, int limit, string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			return await Task.Run(() => GetLimit(transaction, limit, whereCondition, param, commandTimeout));
		}

		public async Task InsertAsync(IDbTransaction transaction, T obj, int commandTimeout = 30)
		{
			await Task.Run(() => Insert(transaction, obj, commandTimeout));
		}

		public async Task<int> RecordCountAsync(IDbTransaction transaction, string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			return await Task.Run(() => RecordCount(transaction, whereCondition, param, commandTimeout));
		}

		public async Task<bool> UpdateAsync(IDbTransaction transaction, object obj, int commandTimeout = 30)
		{
			return await Task.Run(() => Update(transaction, obj, commandTimeout));
		}

		public async Task<bool> UpdateAsync(IDbTransaction transaction, T obj, int commandTimeout = 30)
		{
			return await Task.Run(() => Update(transaction, obj, commandTimeout));
		}

		public async Task<bool> UpsertAsync(IDbTransaction transaction, T obj, int commandTimeout = 30)
		{
			return await Task.Run(() => Upsert(transaction, obj, commandTimeout));
		}

		public async Task<bool> InsertIfNotExistsAsync(IDbTransaction transaction, T obj, int commandTimeout = 30)
		{
			return await Task.Run(() => InsertIfNotExists(transaction, obj, commandTimeout));
		}

		public async Task<IEnumerable<T>> GetDistinctLimitAsync(IDbTransaction transaction, int limit, string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			return await Task.Run(() => GetDistinctLimit(transaction, limit, whereCondition, param, commandTimeout));
		}

		public async Task<bool> DeleteAsync<KeyType>(IDbTransaction transaction, KeyType key, int commandTimeout = 30)
		{
			return await Task.Run(() => Delete<KeyType>(transaction, key, commandTimeout));
		}

		public async Task<T> GetAsync<KeyType>(IDbTransaction transaction, KeyType key, int commandTimeout = 30)
		{
			return await Task.Run(() => Get<KeyType>(transaction, key, commandTimeout));
		}

		public async Task<int> BulkDeleteAsync<KeyType>(SqlTransaction transaction, IEnumerable<KeyType> keys, int commandTimeout = 30)
		{
			return await Task.Run(() => BulkDelete<KeyType>(transaction, keys, commandTimeout));
		}

		public async Task<IEnumerable<KeyType>> GetKeysAsync<KeyType>(IDbTransaction transaction, string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			return await Task.Run(() => GetKeys<KeyType>(transaction, whereCondition, param, commandTimeout));
		}

		public async Task<List<T>> BulkGetAsync(SqlTransaction transaction, IEnumerable<T> keys, int commandTimeout = 30)
		{
			return await Task.Run(() => BulkGet(transaction, keys, commandTimeout));
		}

		public async Task<List<T>> BulkGetAsync<KeyType>(SqlTransaction transaction, IEnumerable<KeyType> keys, int commandTimeout = 30)
		{
			return await Task.Run(() => BulkGet<KeyType>(transaction, keys, commandTimeout));
		}
		#endregion  ITransactionAccessObjectAsync<T>

		#region IAccessObjectSync<T>
		public override int BulkDelete(IEnumerable<T> objs, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				int count = Queries.BulkDelete(conn, objs, null, commandTimeout);
				return count;
			}
		}

		public override void BulkInsert(IEnumerable<T> objs, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				Queries.BulkInsert(conn, objs, null, commandTimeout);
			}
		}

		public override int BulkUpdate(IEnumerable<T> objs, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				int count = Queries.BulkUpdate(conn, objs, null, commandTimeout);
				return count;
			}
		}

		public override int BulkUpsert(IEnumerable<T> objs, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				int count = Queries.BulkUpsert(conn, objs, null, commandTimeout);
				return count;
			}
		}

		public override int BulkInsertIfNotExists(IEnumerable<T> objs, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				return Queries.BulkInsertIfNotExists(conn, objs, null, commandTimeout);
			}
		}

		public override bool Delete(T obj, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				bool success = Queries.Delete(conn, obj, null, commandTimeout);
				return success;
			}
		}

		public override int Delete(string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				int count = Queries.DeleteList(conn, whereCondition, param, null, commandTimeout);
				return count;
			}
		}

		public override T Get(T obj, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				T value = Queries.Get(conn, obj, null, commandTimeout);
				return value;
			}
		}

		public override IEnumerable<T> GetDistinct(string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			return GetDistinct(typeof(T), whereCondition, param, commandTimeout);
		}

		public override IEnumerable<T> GetDistinct(Type columnFilter, string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				IEnumerable<T> list = Queries.GetDistinct(conn, columnFilter, whereCondition, param, null, true, commandTimeout);
				return list;
			}
		}

		public override IEnumerable<T> GetKeys(string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				IEnumerable<T> list = Queries.GetKeys(conn, whereCondition, param, null, true, commandTimeout);
				return list;
			}
		}

		public override IEnumerable<T> GetList(string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				IEnumerable<T> list = Queries.GetList(conn, whereCondition, param, null, true, commandTimeout);
				return list;
			}
		}

		public override IEnumerable<T> GetList(Type columnFilter, string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				IEnumerable<T> list = Queries.GetFilter(conn, columnFilter, whereCondition, param, null, true, commandTimeout);
				return list;
			}
		}

		public override IEnumerable<T> GetLimit(Type columnFilter, int limit, string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				IEnumerable<T> list = Queries.GetFilterLimit(conn, columnFilter, limit, whereCondition, param, null, true, commandTimeout);
				return list;
			}
		}

		public override IEnumerable<T> GetLimit(int limit, string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				IEnumerable<T> list = Queries.GetLimit(conn, limit, whereCondition, param, null, true, commandTimeout);
				return list;
			}
		}

		public override void Insert(T obj, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				Queries.Insert(conn, obj, null, commandTimeout);
			}
		}

		public override int RecordCount(string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				int count = Queries.RecordCount(conn, whereCondition, param, null, commandTimeout);
				return count;
			}
		}

		public override bool Update(object obj, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				bool success = Queries.UpdateObj(conn, obj, null, commandTimeout);
				return success;
			}
		}

		public override bool Update(T obj, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				bool success = Queries.Update(conn, obj, null, commandTimeout);
				return success;
			}
		}

		public override bool Upsert(T obj, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				bool updated = Queries.Upsert(conn, obj, null, commandTimeout);
				return updated;
			}
		}

		public override bool InsertIfNotExists(T obj, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				bool updated = Queries.InsertIfNotExists(conn, obj, null, commandTimeout);
				return updated;
			}
		}

		public override IEnumerable<T> GetDistinctLimit(int limit, string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			return GetDistinctLimit(typeof(T), limit, whereCondition, param, commandTimeout);
		}

		public override IEnumerable<T> GetDistinctLimit(Type columnFilter, int limit, string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				IEnumerable<T> list = Queries.GetDistinctLimit(conn, columnFilter, limit, whereCondition, param, null, true, commandTimeout);
				return list;
			}
		}

		public override bool Delete<KeyType>(KeyType key, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				bool success = ((ISqlQueries<T, KeyType>) KeyQueries).Delete(conn, key, null, commandTimeout);
				return success;
			}
		}

		public override T Get<KeyType>(KeyType key, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				T obj = ((ISqlQueries<T, KeyType>) KeyQueries).Get(conn, key, null, commandTimeout);
				return obj;
			}
		}

		public override int BulkDelete<KeyType>(IEnumerable<KeyType> keys, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				int count = ((ISqlQueries<T, KeyType>) KeyQueries).BulkDelete(conn, keys, null, commandTimeout);
				return count;
			}
		}

		public override IEnumerable<KeyType> GetKeys<KeyType>(string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				IEnumerable<KeyType> keys = ((ISqlQueries<T, KeyType>) KeyQueries).GetKeys(conn, whereCondition, param, null, true, commandTimeout);
				return keys;
			}
		}

		public override List<T> BulkGet(IEnumerable<T> keys, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				List<T> list = Queries.BulkGet(conn, keys, null, commandTimeout).AsList();
				return list;
			}
		}

		public override List<T> BulkGet<KeyType>(IEnumerable<KeyType> keys, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				List<T> list = ((ISqlQueries<T, KeyType>) KeyQueries).BulkGet(conn, keys, null, commandTimeout);
				return list;
			}
		}
		#endregion  IAccessObjectSync<T>
	}
}
