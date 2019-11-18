// Released under MIT License 
// Copyright(c) 2018 Wesley Hamilton
// License: https://www.mit.edu/~amini/LICENSE.md
// Home page: https://github.com/ffhighwind/DapperExtraCRUD

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
	/// Connects to the database and performs SQL operations. This is not thread-safe because the 
	/// <see cref="SqlConnection"/> and <see cref="SqlTransaction"/> are reused.
	/// </summary>
	/// <typeparam name="T">The table type.</typeparam>
	public class DataAccessObject<T> : IAccessObject<T>, IDataAccessConnection
		where T : class
	{
		public DataAccessObject(bool buffered = true) : this(null, buffered)
		{
		}

		public DataAccessObject(string connectionString, bool buffered = true)
		{
			Connection = new SqlConnection(connectionString);
			Buffered = buffered;
			SqlBuilder<T> builder = ExtraCrud.Builder<T>();
			Queries = builder.Queries;
			KeyQueries = builder.KeyBuilder?.QueriesObject;
		}

		protected ISqlQueries<T> Queries { get; }
		protected object KeyQueries { get; }

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
			int count = Queries.BulkDelete(Connection, objs, Transaction, commandTimeout);
			return count;
		}

		public override void BulkInsert(IEnumerable<T> objs, int commandTimeout = 30)
		{
			Queries.BulkInsert(Connection, objs, Transaction, commandTimeout);
		}

		public override int BulkUpdate(IEnumerable<T> objs, int commandTimeout = 30)
		{
			int count = Queries.BulkUpdate(Connection, objs, Transaction, commandTimeout);
			return count;
		}

		public override int BulkUpsert(IEnumerable<T> objs, int commandTimeout = 30)
		{
			int count = Queries.BulkUpsert(Connection, objs, Transaction, commandTimeout);
			return count;
		}

		public override bool Delete(T obj, int commandTimeout = 30)
		{
			bool success = Queries.Delete(Connection, obj, Transaction, commandTimeout);
			return success;
		}

		public override int Delete(string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			int count = Queries.DeleteList(Connection, whereCondition, param, Transaction, commandTimeout);
			return count;
		}

		public override T Get(T obj, int commandTimeout = 30)
		{
			T value = Queries.Get(Connection, obj, Transaction, commandTimeout);
			return value;
		}

		public override IEnumerable<T> GetDistinct(string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			IEnumerable<T> list = Queries.GetDistinct(Connection, whereCondition, param, Transaction, Buffered, commandTimeout);
			return list;
		}

		public override IEnumerable<T> GetKeys(string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			IEnumerable<T> list = Queries.GetKeys(Connection, whereCondition, param, Transaction, Buffered, commandTimeout);
			return list;
		}

		public override IEnumerable<T> GetList(string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			IEnumerable<T> list = Queries.GetList(Connection, whereCondition, param, Transaction, Buffered, commandTimeout);
			return list;
		}

		public override IEnumerable<T> GetLimit(int limit, string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			IEnumerable<T> list = Queries.GetLimit(Connection, limit, whereCondition, param, Transaction, Buffered, commandTimeout);
			return list;
		}

		public override void Insert(T obj, int commandTimeout = 30)
		{
			Queries.Insert(Connection, obj, Transaction, commandTimeout);
		}

		public override int RecordCount(string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			int count = Queries.RecordCount(Connection, whereCondition, param, Transaction, commandTimeout);
			return count;
		}

		public override bool Update(object obj, int commandTimeout = 30)
		{
			bool success = Queries.UpdateObj(Connection, obj, Transaction, commandTimeout);
			return success;
		}
	
		public override bool Update(T obj, int commandTimeout = 30)
		{
			bool success = Queries.Update(Connection, obj, Transaction, commandTimeout);
			return success;
		}

		public override bool Upsert(T obj, int commandTimeout = 30)
		{
			bool updated = Queries.Upsert(Connection, obj, Transaction, commandTimeout);
			return updated;
		}

		public override bool InsertIfNotExists(T obj, int commandTimeout = 30)
		{
			bool updated = Queries.InsertIfNotExists(Connection, obj, Transaction, commandTimeout);
			return updated;
		}

		public override IEnumerable<T> GetDistinctLimit(int limit, string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			IEnumerable<T> list = Queries.GetDistinctLimit(Connection, limit, whereCondition, param, Transaction, Buffered, commandTimeout);
			return list;
		}

		public override bool Delete<KeyType>(KeyType key, int commandTimeout = 30)
		{
			bool success = ((ISqlQueries<T, KeyType>) KeyQueries).Delete(Connection, key, Transaction, commandTimeout);
			return success;
		}

		public override T Get<KeyType>(KeyType key, int commandTimeout = 30)
		{
			T obj = ((ISqlQueries<T, KeyType>) KeyQueries).Get(Connection, key, Transaction, commandTimeout);
			return obj;
		}

		public override int BulkDelete<KeyType>(IEnumerable<KeyType> keys, int commandTimeout = 30)
		{
			int count = ((ISqlQueries<T, KeyType>) KeyQueries).BulkDelete(Connection, keys, Transaction, commandTimeout);
			return count;
		}

		public override IEnumerable<KeyType> GetKeys<KeyType>(string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			IEnumerable<KeyType> keys = ((ISqlQueries<T, KeyType>) KeyQueries).GetKeys(Connection, whereCondition, param, Transaction, Buffered, commandTimeout);
			return keys;
		}

		public override List<T> BulkGet(IEnumerable<T> keys, int commandTimeout = 30)
		{
			List<T> list = Queries.BulkGet(Connection, keys, Transaction, commandTimeout).AsList();
			return list;
		}

		public override List<T> BulkGet<KeyType>(IEnumerable<KeyType> keys, int commandTimeout = 30)
		{
			List<T> list = ((ISqlQueries<T, KeyType>) KeyQueries).BulkGet(Connection, keys, Transaction, commandTimeout);
			return list;
		}

		public override int BulkInsertIfNotExists(IEnumerable<T> objs, int commandTimeout = 30)
		{
			int count = Queries.BulkInsertIfNotExists(Connection, objs, Transaction, commandTimeout);
			return count;
		}
		#endregion  IAccessObjectSync<T>
	}
}
