using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.Extra.Interfaces;
using Dapper.Extra.Internal;

namespace Dapper.Extra.Utilities
{
	/// <summary>
	/// Automatically connects to the database and performs SQL operations.
	/// </summary>
	/// <typeparam name="T">The table's type.</typeparam>
	public class AutoAccessObject<T> : IAccessObject<T>
		where T : class
	{
		public AutoAccessObject(string connectionString = null)
		{
			ConnectionString = connectionString;
		}

		public string ConnectionString { get; set; }

		#region IAccessObjectSync<T>
		public override int BulkDelete(IEnumerable<T> objs, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				int count = ExtraCrud.Queries<T>().BulkDelete(conn, objs, null, commandTimeout);
				return count;
			}
		}

		public override void BulkInsert(IEnumerable<T> objs, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				ExtraCrud.Queries<T>().BulkInsert(conn, objs, null, commandTimeout);
			}
		}

		public override int BulkUpdate(IEnumerable<T> objs, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				int count = ExtraCrud.Queries<T>().BulkUpdate(conn, objs, null, commandTimeout);
				return count;
			}
		}

		public override int BulkUpsert(IEnumerable<T> objs, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				int count = ExtraCrud.Queries<T>().BulkUpsert(conn, objs, null, commandTimeout);
				return count;
			}
		}

		public override bool Delete(T obj, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				bool success = ExtraCrud.Queries<T>().Delete(conn, obj, null, commandTimeout);
				return success;
			}
		}

		public override int Delete(string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				int count = ExtraCrud.Queries<T>().DeleteWhere(conn, whereCondition, param, null, commandTimeout);
				return count;
			}
		}

		public override IEnumerable<T> DeleteList(string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			SqlQueries<T> queries = ExtraCrud.Queries<T>();
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				conn.Open();
				using (SqlTransaction trans = conn.BeginTransaction()) {
					IEnumerable<T> keys = queries.GetKeys(conn, whereCondition, param, null, true, commandTimeout);
					int count = queries.DeleteWhere(conn, whereCondition, param, null, commandTimeout);
					trans.Commit();
					return keys;
				}
			}
		}

		public override T Get(T obj, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				T value = ExtraCrud.Queries<T>().Get(conn, obj, null, commandTimeout);
				return value;
			}
		}

		public override IEnumerable<T> GetDistinct(string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				IEnumerable<T> list = ExtraCrud.Queries<T>().GetDistinct(conn, whereCondition, param, null, true, commandTimeout);
				return list;
			}
		}

		public override IEnumerable<T> GetKeys(string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				IEnumerable<T> list = ExtraCrud.Queries<T>().GetKeys(conn, whereCondition, param, null, true, commandTimeout);
				return list;
			}
		}

		public override IEnumerable<T> GetList(string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				IEnumerable<T> list = ExtraCrud.Queries<T>().GetList(conn, whereCondition, param, null, true, commandTimeout);
				return list;
			}
		}

		public override IEnumerable<T> GetLimit(int limit, string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				IEnumerable<T> list = ExtraCrud.Queries<T>().GetLimit(conn, limit, whereCondition, param, null, true, commandTimeout);
				return list;
			}
		}

		public override void Insert(T obj, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				ExtraCrud.Queries<T>().Insert(conn, obj, null, commandTimeout);
			}
		}

		public override int RecordCount(string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				int count = ExtraCrud.Queries<T>().RecordCount(conn, whereCondition, param, null, commandTimeout);
				return count;
			}
		}

		public override bool Update(object obj, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				bool success = ExtraCrud.Queries<T>().UpdateObj(conn, obj, null, commandTimeout);
				return success;
			}
		}

		public override bool Update(T obj, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				bool success = ExtraCrud.Queries<T>().Update(conn, obj, null, commandTimeout);
				return success;
			}
		}

		public override bool Upsert(T obj, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				bool updated = ExtraCrud.Queries<T>().Upsert(conn, obj, null, commandTimeout);
				return updated;
			}
		}

		public override IEnumerable<T> GetDistinctLimit(int limit, string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				IEnumerable<T> list = ExtraCrud.Queries<T>().GetDistinctLimit(conn, limit, whereCondition, param, null, true, commandTimeout);
				return list;
			}
		}

		public override bool Delete<KeyType>(KeyType key, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				bool success = ExtraCrud.Queries<T, KeyType>().Delete(conn, key, null, commandTimeout);
				return success;
			}
		}

		public override T Get<KeyType>(KeyType key, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				T obj = ExtraCrud.Queries<T, KeyType>().Get(conn, key, null, commandTimeout);
				return obj;
			}
		}

		public override int BulkDelete<KeyType>(IEnumerable<KeyType> keys, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				int count = ExtraCrud.Queries<T, KeyType>().BulkDelete(conn, keys, null, commandTimeout);
				return count;
			}
		}

		public override IEnumerable<KeyType> GetKeys<KeyType>(string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				IEnumerable<KeyType> keys = ExtraCrud.Queries<T, KeyType>().GetKeys(conn, whereCondition, param, null, true, commandTimeout);
				return keys;
			}
		}

		public override List<T> BulkGet(IEnumerable<T> keys, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				List<T> list = ExtraCrud.Queries<T>().BulkGet(conn, keys, null, commandTimeout).AsList();
				return list;
			}
		}

		public override List<T> BulkGet<KeyType>(IEnumerable<KeyType> keys, int commandTimeout = 30)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				List<T> list = ExtraCrud.Queries<T, KeyType>().BulkGet(conn, keys, null, commandTimeout);
				return list;
			}
		}
		#endregion  IAccessObjectSync<T>
	}
}
