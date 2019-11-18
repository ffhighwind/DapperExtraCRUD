// Released under MIT License 
// Copyright(c) 2018 Wesley Hamilton
// License: https://www.mit.edu/~amini/LICENSE.md
// Home page: https://github.com/ffhighwind/DapperExtraCRUD

using System;
using System.Collections.Generic;
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
	public class AutoAccessObject<T> : IAccessObject<T>
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
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				IEnumerable<T> list = Queries.GetDistinct(conn, whereCondition, param, null, true, commandTimeout);
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
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				IEnumerable<T> list = Queries.GetDistinctLimit(conn, limit, whereCondition, param, null, true, commandTimeout);
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
