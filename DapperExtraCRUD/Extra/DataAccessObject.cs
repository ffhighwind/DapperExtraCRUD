using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.Extra.Interfaces;

namespace Dapper.Extra
{
	/// <summary>
	/// Connects to the database and performs SQL operations. This is not thread-safe because the 
	/// <see cref="SqlConnection"/> and <see cref="SqlTransaction"/> are reused.
	/// </summary>
	/// <typeparam name="T">The table's type.</typeparam>
	public class DataAccessObject<T> : IAccessObject<T>, IDataAccessConnection
		where T : class
	{
		public DataAccessObject(bool buffered = true)
		{
			Buffered = buffered;
		}

		public DataAccessObject(string connectionString, bool buffered = true)
		{
			Connection = new SqlConnection(connectionString);
			Buffered = buffered;
		}

		/// <summary>
		/// The connection used for queries. This will be temporarily opened it if is closed. 
		/// This connection is not thread-safe because it is reused for all queries.
		/// </summary>
		public SqlConnection Connection { get; set; }
		/// <summary>
		/// The transaction used for queries.
		/// </summary>
		public SqlTransaction Transaction { get; set; }
		/// <summary>
		/// Determines if the queries are buffered.
		/// </summary>
		public bool Buffered { get; set; }

		#region IAccessObjectSync<T>
		public override int BulkDelete(IEnumerable<T> objs, int? commandTimeout = null)
		{
			int count = TableData<T>.Queries.BulkDelete(Connection, objs, Transaction, commandTimeout);
			return count;
		}

		public override void BulkInsert(IEnumerable<T> objs, int? commandTimeout = null)
		{
			TableData<T>.Queries.BulkInsert(Connection, objs, Transaction, commandTimeout);
		}

		public override int BulkUpdate(IEnumerable<T> objs, int? commandTimeout = null)
		{
			int count = TableData<T>.Queries.BulkUpdate(Connection, objs, Transaction, commandTimeout);
			return count;
		}

		public override int BulkUpsert(IEnumerable<T> objs, int? commandTimeout = null)
		{
			int count = TableData<T>.Queries.BulkUpsert(Connection, objs, Transaction, commandTimeout);
			return count;
		}

		public override bool Delete(T obj, int? commandTimeout = null)
		{
			bool success = TableData<T>.Queries.Delete(Connection, obj, Transaction, commandTimeout);
			return success;
		}

		public override int Delete(string whereCondition = "", object param = null, int? commandTimeout = null)
		{
			int count = TableData<T>.Queries.DeleteWhere(Connection, whereCondition, param, Transaction, commandTimeout);
			return count;
		}

		public override T Get(T obj, int? commandTimeout = null)
		{
			T value = TableData<T>.Queries.Get(Connection, obj, Transaction, commandTimeout);
			return value;
		}

		public override IEnumerable<T> GetDistinct(string whereCondition = "", object param = null, int? commandTimeout = null)
		{
			IEnumerable<T> list = TableData<T>.Queries.GetDistinct(Connection, whereCondition, param, Transaction, Buffered, commandTimeout);
			return list;
		}

		public override IEnumerable<T> GetKeys(string whereCondition = "", object param = null, int? commandTimeout = null)
		{
			IEnumerable<T> list = TableData<T>.Queries.GetKeys(Connection, whereCondition, param, Transaction, Buffered, commandTimeout);
			return list;
		}

		public override IEnumerable<T> GetList(string whereCondition = "", object param = null, int? commandTimeout = null)
		{
			IEnumerable<T> list = TableData<T>.Queries.GetList(Connection, whereCondition, param, Transaction, Buffered, commandTimeout);
			return list;
		}

		public override IEnumerable<T> GetLimit(int limit, string whereCondition = "", object param = null, int? commandTimeout = null)
		{
			IEnumerable<T> list = TableData<T>.Queries.GetLimit(Connection, limit, whereCondition, param, Transaction, Buffered, commandTimeout);
			return list;
		}

		public override T Insert(T obj, int? commandTimeout = null)
		{
			T value = TableData<T>.Queries.Insert(Connection, obj, Transaction, commandTimeout);
			return value;
		}

		public override int RecordCount(string whereCondition = "", object param = null, int? commandTimeout = null)
		{
			int count = TableData<T>.Queries.RecordCount(Connection, whereCondition, param, Transaction, commandTimeout);
			return count;
		}

		public override bool Update(T obj)
		{
			bool success = TableData<T>.Queries.Update(Connection, obj, Transaction);
			return success;
		}

		public override bool Update(T obj, object filter = null, int? commandTimeout = null)
		{
			bool success = TableData<T>.Queries.UpdateFilter(Connection, obj, filter, Transaction, commandTimeout);
			return success;
		}

		public override bool Upsert(T obj, int? commandTimeout = null)
		{
			bool updated = TableData<T>.Queries.Upsert(Connection, obj, Transaction, commandTimeout);
			return updated;
		}

		public override IEnumerable<T> GetDistinctLimit(int limit, string whereCondition = "", object param = null, int? commandTimeout = null)
		{
			IEnumerable<T> list = TableData<T>.Queries.GetDistinctLimit(Connection, limit, whereCondition, param, Transaction, Buffered, commandTimeout);
			return list;
		}

		public override IEnumerable<T> DeleteList(string whereCondition = "", object param = null, int? commandTimeout = null)
		{
			if (Transaction == null) {
				bool wasClosed = Connection.State == ConnectionState.Closed;
				if (wasClosed)
					Connection.Open();
				using (SqlTransaction trans = Connection.BeginTransaction()) {
					IEnumerable<T> keys = TableData<T>.Queries.GetKeys(Connection, whereCondition, param, trans, true, commandTimeout);
					int count = TableData<T>.Queries.DeleteWhere(Connection, whereCondition, param, trans, commandTimeout);
					trans.Commit();
					if (wasClosed)
						Connection.Close();
					return keys;
				}
			}
			else {
				IEnumerable<T> keys = TableData<T>.Queries.GetKeys(Transaction.Connection, whereCondition, param, Transaction, true, commandTimeout);
				int count = TableData<T>.Queries.DeleteWhere(Transaction.Connection, whereCondition, param, Transaction, commandTimeout);
				return keys;
			}
		}

		public override bool Delete<KeyType>(KeyType key, int? commandTimeout = null)
		{
			bool success = TableData<T, KeyType>.Queries.Delete(Connection, key, Transaction, commandTimeout);
			return success;
		}

		public override T Get<KeyType>(KeyType key, int? commandTimeout = null)
		{
			T obj = TableData<T, KeyType>.Queries.Get(Connection, key, Transaction, commandTimeout);
			return obj;
		}

		public override int BulkDelete<KeyType>(IEnumerable<KeyType> keys, int? commandTimeout = null)
		{
			int count = TableData<T, KeyType>.Queries.BulkDelete(Connection, keys, Transaction, commandTimeout);
			return count;
		}

		public override IEnumerable<KeyType> GetKeys<KeyType>(string whereCondition = "", object param = null, int? commandTimeout = null)
		{
			IEnumerable<KeyType> keys = TableData<T, KeyType>.Queries.GetKeys(Connection, whereCondition, param, Transaction, Buffered, commandTimeout);
			return keys;
		}
		#endregion  IAccessObjectSync<T>
	}
}
