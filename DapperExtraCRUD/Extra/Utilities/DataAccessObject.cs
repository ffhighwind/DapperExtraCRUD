using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Extra.Utilities
{
	/// <summary>
	/// Connects to the database and performs SQL operations. This is not thread-safe because the 
	/// <see cref="SqlConnection"/> and <see cref="SqlTransaction"/> are reused.
	/// </summary>
	/// <typeparam name="T">The table type.</typeparam>
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
		public override int BulkDelete(IEnumerable<T> objs, int commandTimeout = 30)
		{
			int count = ExtraCrud.Queries<T>().BulkDelete(Connection, objs, Transaction, commandTimeout);
			return count;
		}

		public override void BulkInsert(IEnumerable<T> objs, int commandTimeout = 30)
		{
			ExtraCrud.Queries<T>().BulkInsert(Connection, objs, Transaction, commandTimeout);
		}

		public override int BulkUpdate(IEnumerable<T> objs, int commandTimeout = 30)
		{
			int count = ExtraCrud.Queries<T>().BulkUpdate(Connection, objs, Transaction, commandTimeout);
			return count;
		}

		public override int BulkUpsert(IEnumerable<T> objs, int commandTimeout = 30)
		{
			int count = ExtraCrud.Queries<T>().BulkUpsert(Connection, objs, Transaction, commandTimeout);
			return count;
		}

		public override bool Delete(T obj, int commandTimeout = 30)
		{
			bool success = ExtraCrud.Queries<T>().Delete(Connection, obj, Transaction, commandTimeout);
			return success;
		}

		public override int Delete(string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			int count = ExtraCrud.Queries<T>().DeleteList(Connection, whereCondition, param, Transaction, commandTimeout);
			return count;
		}

		public override T Get(T obj, int commandTimeout = 30)
		{
			T value = ExtraCrud.Queries<T>().Get(Connection, obj, Transaction, commandTimeout);
			return value;
		}

		public override IEnumerable<T> GetDistinct(string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			IEnumerable<T> list = ExtraCrud.Queries<T>().GetDistinct(Connection, whereCondition, param, Transaction, Buffered, commandTimeout);
			return list;
		}

		public override IEnumerable<T> GetKeys(string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			IEnumerable<T> list = ExtraCrud.Queries<T>().GetKeys(Connection, whereCondition, param, Transaction, Buffered, commandTimeout);
			return list;
		}

		public override IEnumerable<T> GetList(string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			IEnumerable<T> list = ExtraCrud.Queries<T>().GetList(Connection, whereCondition, param, Transaction, Buffered, commandTimeout);
			return list;
		}

		public override IEnumerable<T> GetLimit(int limit, string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			IEnumerable<T> list = ExtraCrud.Queries<T>().GetLimit(Connection, limit, whereCondition, param, Transaction, Buffered, commandTimeout);
			return list;
		}

		public override void Insert(T obj, int commandTimeout = 30)
		{
			ExtraCrud.Queries<T>().Insert(Connection, obj, Transaction, commandTimeout);
		}

		public override int RecordCount(string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			int count = ExtraCrud.Queries<T>().RecordCount(Connection, whereCondition, param, Transaction, commandTimeout);
			return count;
		}

		public override bool Update(object obj, int commandTimeout = 30)
		{
			bool success = ExtraCrud.Queries<T>().UpdateObj(Connection, obj, Transaction, commandTimeout);
			return success;
		}
	
		public override bool Update(T obj, int commandTimeout = 30)
		{
			bool success = ExtraCrud.Queries<T>().Update(Connection, obj, Transaction, commandTimeout);
			return success;
		}

		public override bool Upsert(T obj, int commandTimeout = 30)
		{
			bool updated = ExtraCrud.Queries<T>().Upsert(Connection, obj, Transaction, commandTimeout);
			return updated;
		}

		public override IEnumerable<T> GetDistinctLimit(int limit, string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			IEnumerable<T> list = ExtraCrud.Queries<T>().GetDistinctLimit(Connection, limit, whereCondition, param, Transaction, Buffered, commandTimeout);
			return list;
		}

		public override IEnumerable<T> DeleteList(string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			Internal.SqlQueries<T> queries = ExtraCrud.Queries<T>();
			if (Transaction == null) {
				bool wasClosed = Connection.State == ConnectionState.Closed;
				if (wasClosed)
					Connection.Open();
				using (SqlTransaction trans = Connection.BeginTransaction()) {
					IEnumerable<T> keys = queries.GetKeys(Connection, whereCondition, param, trans, true, commandTimeout);
					int count = queries.DeleteList(Connection, whereCondition, param, trans, commandTimeout);
					trans.Commit();
					if (wasClosed)
						Connection.Close();
					return keys;
				}
			}
			else {
				IEnumerable<T> keys = queries.GetKeys(Transaction.Connection, whereCondition, param, Transaction, true, commandTimeout);
				int count = queries.DeleteList(Transaction.Connection, whereCondition, param, Transaction, commandTimeout);
				return keys;
			}
		}

		public override bool Delete<KeyType>(KeyType key, int commandTimeout = 30)
		{
			bool success = ExtraCrud.Queries<T, KeyType>().Delete(Connection, key, Transaction, commandTimeout);
			return success;
		}

		public override T Get<KeyType>(KeyType key, int commandTimeout = 30)
		{
			T obj = ExtraCrud.Queries<T, KeyType>().Get(Connection, key, Transaction, commandTimeout);
			return obj;
		}

		public override int BulkDelete<KeyType>(IEnumerable<KeyType> keys, int commandTimeout = 30)
		{
			int count = ExtraCrud.Queries<T, KeyType>().BulkDelete(Connection, keys, Transaction, commandTimeout);
			return count;
		}

		public override IEnumerable<KeyType> GetKeys<KeyType>(string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			IEnumerable<KeyType> keys = ExtraCrud.Queries<T, KeyType>().GetKeys(Connection, whereCondition, param, Transaction, Buffered, commandTimeout);
			return keys;
		}

		public override List<T> BulkGet(IEnumerable<T> keys, int commandTimeout = 30)
		{
			List<T> list = ExtraCrud.Queries<T>().BulkGet(Connection, keys, Transaction, commandTimeout).AsList();
			return list;
		}

		public override List<T> BulkGet<KeyType>(IEnumerable<KeyType> keys, int commandTimeout = 30)
		{
			List<T> list = ExtraCrud.Queries<T, KeyType>().BulkGet(Connection, keys, Transaction, commandTimeout);
			return list;
		}
		#endregion  IAccessObjectSync<T>
	}
}
