using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.Extra.Interfaces;

namespace Dapper.Extra
{
	/// <summary>
	/// Automatically connects to the database and performs SQL operations.
	/// </summary>
	/// <typeparam name="T">The table's type.</typeparam>
	public class AutoAccessObject<T> : IAccessObject<T>
		where T : class
	{
		public AutoAccessObject()
		{
		}

		public AutoAccessObject(string connectionString)
		{
			ConnectionString = connectionString;
		}

		public string ConnectionString { get; set; }

		#region IAccessObjectSync<T>
		public override int BulkDelete(IEnumerable<T> objs, int? commandTimeout = null)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				int count = TableData<T>.Queries.BulkDelete(conn, objs, null, commandTimeout);
				return count;
			}
		}

		public override void BulkInsert(IEnumerable<T> objs, int? commandTimeout = null)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				TableData<T>.Queries.BulkInsert(conn, objs, null, commandTimeout);
			}
		}

		public override int BulkUpdate(IEnumerable<T> objs, int? commandTimeout = null)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				int count = TableData<T>.Queries.BulkUpdate(conn, objs, null, commandTimeout);
				return count;
			}
		}

		public override int BulkUpsert(IEnumerable<T> objs, int? commandTimeout = null)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				int count = TableData<T>.Queries.BulkUpsert(conn, objs, null, commandTimeout);
				return count;
			}
		}

		public override bool Delete(T obj, int? commandTimeout = null)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				bool success = TableData<T>.Queries.Delete(conn, obj, null, commandTimeout);
				return success;
			}
		}

		public override int Delete(string whereCondition = "", object param = null, int? commandTimeout = null)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				int count = TableData<T>.Queries.DeleteWhere(conn, whereCondition, param, null, commandTimeout);
				return count;
			}
		}

		public override IEnumerable<T> DeleteList(string whereCondition = "", object param = null, int? commandTimeout = null)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				conn.Open();
				using (SqlTransaction trans = conn.BeginTransaction()) {
					IEnumerable<T> keys = TableData<T>.Queries.GetKeys(conn, whereCondition, param, null, true, commandTimeout);
					int count = TableData<T>.Queries.DeleteWhere(conn, whereCondition, param, null, commandTimeout);
					trans.Commit();
					return keys;
				}
			}
		}

		public override T Get(T obj, int? commandTimeout = null)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				T value = TableData<T>.Queries.Get(conn, obj, null, commandTimeout);
				return value;
			}
		}

		public override IEnumerable<T> GetDistinct(string whereCondition = "", object param = null, int? commandTimeout = null)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				IEnumerable<T> list = TableData<T>.Queries.GetDistinct(conn, whereCondition, param, null, true, commandTimeout);
				return list;
			}
		}

		public override IEnumerable<T> GetKeys(string whereCondition = "", object param = null, int? commandTimeout = null)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				IEnumerable<T> list = TableData<T>.Queries.GetKeys(conn, whereCondition, param, null, true, commandTimeout);
				return list;
			}
		}

		public override IEnumerable<T> GetList(string whereCondition = "", object param = null, int? commandTimeout = null)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				IEnumerable<T> list = TableData<T>.Queries.GetList(conn, whereCondition, param, null, true, commandTimeout);
				return list;
			}
		}

		public override IEnumerable<T> GetLimit(int limit, string whereCondition = "", object param = null, int? commandTimeout = null)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				IEnumerable<T> list = TableData<T>.Queries.GetLimit(conn, limit, whereCondition, param, null, true, commandTimeout);
				return list;
			}
		}

		public override T Insert(T obj, int? commandTimeout = null)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				T value = TableData<T>.Queries.Insert(conn, obj, null, commandTimeout);
				return value;
			}
		}

		public override int RecordCount(string whereCondition = "", object param = null, int? commandTimeout = null)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				int count = TableData<T>.Queries.RecordCount(conn, whereCondition, param, null, commandTimeout);
				return count;
			}
		}

		public override bool Update(T obj, object filter = null, int? commandTimeout = null)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				bool success = TableData<T>.Queries.UpdateFilter(conn, obj, filter, null, commandTimeout);
				return success;
			}
		}

		public override bool Update(T obj)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				bool success = TableData<T>.Queries.Update(conn, obj);
				return success;
			}
		}

		public override T Upsert(T obj, int? commandTimeout = null)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				T value = TableData<T>.Queries.Upsert(conn, obj, null, commandTimeout);
				return value;
			}
		}

		public override IEnumerable<T> GetDistinctLimit(int limit, string whereCondition = "", object param = null, int? commandTimeout = null)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				IEnumerable<T> list = TableData<T>.Queries.GetDistinctLimit(conn, limit, whereCondition, param, null, true, commandTimeout);
				return list;
			}
		}

		public override bool Delete<KeyType>(KeyType key, int? commandTimeout = null)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				bool success = TableData<T, KeyType>.Queries.Delete(conn, key, null, commandTimeout);
				return success;
			}
		}

		public override T Get<KeyType>(KeyType key, int? commandTimeout = null)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				T obj = TableData<T, KeyType>.Queries.Get(conn, key, null, commandTimeout);
				return obj;
			}
		}

		public override int BulkDelete<KeyType>(IEnumerable<KeyType> keys, int? commandTimeout = null)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				int count = TableData<T, KeyType>.Queries.BulkDelete(conn, keys, null, commandTimeout);
				return count;
			}
		}

		public override IEnumerable<KeyType> GetKeys<KeyType>(string whereCondition = "", object param = null, int? commandTimeout = null)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				IEnumerable<KeyType> keys = TableData<T, KeyType>.Queries.GetKeys(conn, whereCondition, param, null, true, commandTimeout);
				return keys;
			}
		}
		#endregion  IAccessObjectSync<T>
	}
}
