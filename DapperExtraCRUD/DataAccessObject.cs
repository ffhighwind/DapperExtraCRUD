using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dapper.Extension;
using Dapper.Extension.Interfaces;

namespace Dapper
{
	public class DataAccessObject<T> : IDataAccessObject<T> where T : class
	{
		public DataAccessObject(string connectionString)
		{
			ConnectionString = connectionString;
			Properties = Queries.Properties;
			KeyProperties = Queries.KeyProperties;
			Columns = Queries.Columns;
		}

		public PropertyInfo[] Properties { get; private set; }
		public PropertyInfo[] KeyProperties { get; private set; }
		public string[] Columns { get; private set; }
		protected readonly string ConnectionString;

		public SqlConnection Connection()
		{
			SqlConnection conn = new SqlConnection(ConnectionString);
			conn.Open();
			return conn;
		}

		#region IDataAccessObjectSync<T>
		public override bool Delete(IDictionary<string, object> key, int? commandTimeout = null)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				return Queries.DeleteDictFunc(conn, key, null, commandTimeout);
			}
		}

		public override bool Delete(T obj, int? commandTimeout = null)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				return Queries.DeleteFunc(conn, obj, null, commandTimeout);
			}
		}

		public override int BulkDelete(IEnumerable<T> objs, int? commandTimeout = null)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				return Queries.BulkDeleteFunc(conn, objs, null, commandTimeout);
			}
		}

		public override int Delete(string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				return Queries.DeleteWhereFunc(conn, whereCondition, param, null, commandTimeout);
			}
		}

		public override IEnumerable<T> DeleteList(string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			SqlConnection conn = new SqlConnection(ConnectionString);
			IEnumerable<T> list = Queries.DeleteListFunc(conn, whereCondition, param, null, buffered, commandTimeout);
			if (buffered) {
				conn.Dispose(); 
				return list;
			}
			return new ConnectedEnumerable<T>(list, conn);
		}

		public override T Get(IDictionary<string, object> key, int? commandTimeout = null)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				return Queries.GetDictFunc(conn, key, null, commandTimeout);
			}
		}

		public override T Get(T obj, int? commandTimeout = null)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				return Queries.GetFunc(conn, obj, null, commandTimeout);
			}
		}

		public override IEnumerable<T> GetKeys(string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			SqlConnection conn = new SqlConnection(ConnectionString);
			IEnumerable<T> list = Queries.GetKeysFunc(conn, whereCondition, param, null, buffered, commandTimeout);
			if (buffered) {
				conn.Dispose(); 
				return list;
			}
			return new ConnectedEnumerable<T>(list, conn);
		}

		public override IEnumerable<T> GetList(string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			SqlConnection conn = new SqlConnection(ConnectionString);
			IEnumerable<T> list = Queries.GetListFunc(conn, whereCondition, param, null, buffered, commandTimeout);
			if (buffered) {
				conn.Dispose(); 
				return list;
			}
			return new ConnectedEnumerable<T>(list, conn);
		}

		public override IEnumerable<T> GetTop(int limit, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			SqlConnection conn = new SqlConnection(ConnectionString);
			IEnumerable<T> list = Queries.GetTopFunc(conn, limit, whereCondition, param, null, buffered, commandTimeout);
			if (buffered) {
				conn.Dispose(); 
				return list;
			}
			return new ConnectedEnumerable<T>(list, conn);
		}

		public override IEnumerable<T> GetDistinct(string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			SqlConnection conn = new SqlConnection(ConnectionString);
			IEnumerable<T> list = Queries.GetDistinctFunc(conn, whereCondition, param, null, buffered, commandTimeout);
			if (buffered) {
				conn.Dispose(); 
				return list;
			}
			return new ConnectedEnumerable<T>(list, conn);
		}

		public override T Insert(T obj, int? commandTimeout = null)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				return Queries.InsertFunc(conn, obj, null, commandTimeout);
			}
		}

		public override IEnumerable<T> BulkInsert(IEnumerable<T> objs, int? commandTimeout = null)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				conn.Open();
				using (SqlTransaction trans = conn.BeginTransaction()) {
					IEnumerable<T> list = Queries.BulkInsertFunc(conn, objs, null, commandTimeout);
					trans.Commit();
					return list;
				}
			}
		}

		public override int RecordCount(string whereCondition = "", object param = null, int? commandTimeout = null)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				return Queries.RecordCountFunc(conn, whereCondition, param, null, commandTimeout);
			}
		}

		public override bool Update(T obj, int? commandTimeout = null)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				return Queries.UpdateFunc(conn, obj, null, commandTimeout);
			}
		}

		public override int BulkUpdate(IEnumerable<T> objs, int? commandTimeout = null)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				return Queries.BulkUpdateFunc(conn, objs, null, commandTimeout);
			}
		}

		public override T Upsert(T obj, int? commandTimeout = null)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				return Queries.UpsertFunc(conn, obj, null, commandTimeout);
			}
		}

		public override int BulkUpsert(IEnumerable<T> objs, int? commandTimeout = null)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				conn.Open();
				using (SqlTransaction trans = conn.BeginTransaction()) {
					int count = Queries.BulkUpsertFunc(conn, objs, null, commandTimeout);
					trans.Commit();
					return count;
				}
			}
		}

		public override IEnumerable<T> BulkDeleteList(IEnumerable<T> objs, int? commandTimeout = null)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				conn.Open();
				using (SqlTransaction trans = conn.BeginTransaction()) {
					IEnumerable<T> list = Queries.BulkDeleteListFunc(conn, objs, null, commandTimeout);
					trans.Commit();
					return list;
				}
			}
		}

		public override IEnumerable<T> BulkUpdateList(IEnumerable<T> objs, int? commandTimeout = null)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				conn.Open();
				using (SqlTransaction trans = conn.BeginTransaction()) {
					IEnumerable<T> list = Queries.BulkUpdateListFunc(conn, objs, null, commandTimeout);
					trans.Commit();
					return list;
				}
			}
		}

		public override IEnumerable<T> BulkUpsertList(IEnumerable<T> objs, int? commandTimeout = null)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				conn.Open();
				using (SqlTransaction trans = conn.BeginTransaction()) {
					IEnumerable<T> list = Queries.BulkUpsertListFunc(conn, objs, null, commandTimeout);
					trans.Commit();
					return list;
				}
			}
		}
		#endregion IDataAccessObjectSync<T>
		
		#region ITransactionQueriesSync<T>
		public override bool Delete(IDbTransaction transaction, IDictionary<string, object> key, int? commandTimeout = null)
		{
			return Queries.DeleteDictFunc(transaction.Connection, key, transaction, commandTimeout);
		}

		public override bool Delete(IDbTransaction transaction, T obj, int? commandTimeout = null)
		{
			return Queries.DeleteFunc(transaction.Connection, obj, transaction, commandTimeout);
		}

		public override int BulkDelete(SqlTransaction transaction, IEnumerable<T> objs, int? commandTimeout = null)
		{
			return Queries.BulkDeleteFunc(transaction.Connection, objs, transaction, commandTimeout);
		}

		public override int Delete(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			return Queries.DeleteWhereFunc(transaction.Connection, whereCondition, param, transaction, commandTimeout);
		}

		public override IEnumerable<T> DeleteList(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			return Queries.DeleteListFunc(transaction.Connection, whereCondition, param, transaction, buffered, commandTimeout);
		}

		public override T Get(IDbTransaction transaction, IDictionary<string, object> key, int? commandTimeout = null)
		{
			return Queries.GetDictFunc(transaction.Connection, key, transaction, commandTimeout);
		}

		public override T Get(IDbTransaction transaction, T obj, int? commandTimeout = null)
		{
			return Queries.GetFunc(transaction.Connection, obj, transaction, commandTimeout);
		}

		public override IEnumerable<T> GetKeys(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			return Queries.GetKeysFunc(transaction.Connection, whereCondition, param, transaction, buffered, commandTimeout);
		}

		public override IEnumerable<T> GetList(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			return Queries.GetListFunc(transaction.Connection, whereCondition, param, transaction, buffered, commandTimeout);
		}

		public override IEnumerable<T> GetTop(IDbTransaction transaction, int limit, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			return Queries.GetTopFunc(transaction.Connection, limit, whereCondition, param, transaction, buffered, commandTimeout);
		}

		public override IEnumerable<T> GetDistinct(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			return Queries.GetDistinctFunc(transaction.Connection, whereCondition, param, transaction, buffered, commandTimeout);
		}

		public override T Insert(IDbTransaction transaction, T obj, int? commandTimeout = null)
		{
			return Queries.InsertFunc(transaction.Connection, obj, transaction, commandTimeout);
		}

		public override IEnumerable<T> BulkInsert(SqlTransaction transaction, IEnumerable<T> objs, int? commandTimeout = null)
		{
			return Queries.BulkInsertFunc(transaction.Connection, objs, transaction, commandTimeout);
		}

		public override int RecordCount(IDbTransaction transaction, string whereCondition = "", object param = null, int? commandTimeout = null)
		{
			return Queries.RecordCountFunc(transaction.Connection, whereCondition, param, transaction, commandTimeout);
		}

		public override bool Update(IDbTransaction transaction, T obj, int? commandTimeout = null)
		{
			return Queries.UpdateFunc(transaction.Connection, obj, transaction, commandTimeout);
		}

		public override int BulkUpdate(SqlTransaction transaction, IEnumerable<T> objs, int? commandTimeout = null)
		{
			return Queries.BulkUpdateFunc(transaction.Connection, objs, transaction, commandTimeout);
		}

		public override T Upsert(IDbTransaction transaction, T obj, int? commandTimeout = null)
		{
			return Queries.UpsertFunc(transaction.Connection, obj, transaction, commandTimeout);
		}

		public override int BulkUpsert(SqlTransaction transaction, IEnumerable<T> objs, int? commandTimeout = null)
		{
			return Queries.BulkUpsertFunc(transaction.Connection, objs, transaction, commandTimeout);
		}

		public override IEnumerable<T> BulkDeleteList(SqlTransaction transaction, IEnumerable<T> objs, int? commandTimeout = null)
		{
			return Queries.BulkDeleteListFunc(transaction.Connection, objs, transaction, commandTimeout);
		}

		public override IEnumerable<T> BulkUpdateList(SqlTransaction transaction, IEnumerable<T> objs, int? commandTimeout = null)
		{
			return Queries.BulkUpdateListFunc(transaction.Connection, objs, transaction, commandTimeout);
		}

		public override IEnumerable<T> BulkUpsertList(SqlTransaction transaction, IEnumerable<T> objs, int? commandTimeout = null)
		{
			return Queries.BulkUpsertListFunc(transaction.Connection, objs, transaction, commandTimeout);
		}
		#endregion ITransactionQueriesSync<T>
	}



	public class DataAccessObject<T, KeyType> : IDataAccessObject<T, KeyType, T>
		where T : class
	{
		public DataAccessObject(string connectionString)
		{
			ConnectionString = connectionString;
			Properties = Queries.Properties;
			KeyProperties = Queries.KeyProperties;
			Columns = Queries.Columns;
		}

		public PropertyInfo[] Properties { get; private set; }
		public PropertyInfo[] KeyProperties { get; private set; }
		public string[] Columns { get; private set; }
		protected readonly string ConnectionString;

		public SqlConnection Connection()
		{
			SqlConnection conn = new SqlConnection(ConnectionString);
			conn.Open();
			return conn;
		}

		#region IDataAccessObjectSync<T, KeyType, Ret>
		public override bool Delete(KeyType key, int? commandTimeout = null)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				return QueriesKey.DeleteFunc(conn, key, null, commandTimeout);
			}
		}

		public override int BulkDelete(IEnumerable<KeyType> keys, int? commandTimeout = null)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				return QueriesKey.BulkDeleteFunc(conn, keys, null, commandTimeout);
			}
		}

		public override bool Delete(IDictionary<string, object> key, int? commandTimeout = null)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				return Queries.DeleteDictFunc(conn, key, null, commandTimeout);
			}
		}

		public override bool Delete(T obj, int? commandTimeout = null)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				return Queries.DeleteFunc(conn, obj, null, commandTimeout);
			}
		}

		public override int BulkDelete(IEnumerable<T> objs, int? commandTimeout = null)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				return Queries.BulkDeleteFunc(conn, objs, null, commandTimeout);
			}
		}

		public override int Delete(string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				return Queries.DeleteWhereFunc(conn, whereCondition, param, null, commandTimeout);
			}
		}

		public override IEnumerable<KeyType> DeleteList(string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			SqlConnection conn = new SqlConnection(ConnectionString);
			IEnumerable<KeyType> keys = QueriesKey.DeleteListFunc(conn, whereCondition, param, null, buffered, commandTimeout);
			if (buffered) {
				conn.Dispose(); 
				return keys;
			}
			return new ConnectedEnumerable<KeyType>(keys, conn);
		}

		public override T Get(KeyType key, int? commandTimeout = null)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				return QueriesKey.GetFunc(conn, key, null, commandTimeout);
			}
		}

		public override T Get(IDictionary<string, object> key, int? commandTimeout = null)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				return Queries.GetDictFunc(conn, key, null, commandTimeout);
			}
		}

		public override T Get(T obj, int? commandTimeout = null)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				return Queries.GetFunc(conn, obj, null, commandTimeout);
			}
		}

		public override IEnumerable<KeyType> GetKeys(string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			SqlConnection conn = new SqlConnection(ConnectionString);
			IEnumerable<KeyType> keys = QueriesKey.GetKeysFunc(conn, whereCondition, param, null, buffered, commandTimeout);
			if(buffered) {
				conn.Dispose(); 
				return keys;
			}
			return new ConnectedEnumerable<KeyType>(keys, conn);
		}

		public override IEnumerable<T> GetList(string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			SqlConnection conn = new SqlConnection(ConnectionString);
			IEnumerable<T> objs = Queries.GetListFunc(conn, whereCondition, param, null, buffered, commandTimeout);
			if (buffered) {
				conn.Dispose(); 
				return objs;
			}
			return new ConnectedEnumerable<T>(objs, conn);
		}

		public override IEnumerable<T> GetTop(int limit, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			SqlConnection conn = new SqlConnection(ConnectionString);
			IEnumerable<T> objs = Queries.GetTopFunc(conn, limit, whereCondition, param, null, buffered, commandTimeout);
			if (buffered) {
				conn.Dispose(); 
				return objs;
			}
			return new ConnectedEnumerable<T>(objs, conn);
		}

		public override IEnumerable<T> GetDistinct(string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			SqlConnection conn = new SqlConnection(ConnectionString);
			IEnumerable<T> objs = Queries.GetDistinctFunc(conn, whereCondition, param, null, buffered, commandTimeout);
			if (buffered) {
				conn.Dispose(); 
				return objs;
			}
			return new ConnectedEnumerable<T>(objs, conn);
		}

		public override T Insert(T obj, int? commandTimeout = null)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				return Queries.InsertFunc(conn, obj, null, commandTimeout);
			}
		}

		public override IEnumerable<T> BulkInsert(IEnumerable<T> objs, int? commandTimeout = null)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				conn.Open();
				using (SqlTransaction trans = conn.BeginTransaction()) {
					IEnumerable<T> list = Queries.BulkInsertFunc(conn, objs, null, commandTimeout);
					trans.Commit();
					return list;
				}
			}
		}

		public override int RecordCount(string whereCondition = "", object param = null, int? commandTimeout = null)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				return Queries.RecordCountFunc(conn, whereCondition, param, null, commandTimeout);
			}
		}

		public override bool Update(T obj, int? commandTimeout = null)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				return Queries.UpdateFunc(conn, obj, null, commandTimeout);
			}
		}

		public override int BulkUpdate(IEnumerable<T> objs, int? commandTimeout = null)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				return Queries.BulkUpdateFunc(conn, objs, null, commandTimeout);
			}
		}

		public override T Upsert(T obj, int? commandTimeout = null)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				return Queries.UpsertFunc(conn, obj, null, commandTimeout);
			}
		}

		public override int BulkUpsert(IEnumerable<T> objs, int? commandTimeout = null)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				conn.Open();
				using (SqlTransaction trans = conn.BeginTransaction()) {
					int count = Queries.BulkUpsertFunc(conn, objs, null, commandTimeout);
					trans.Commit();
					return count;
				}
			}
		}

		public override IEnumerable<KeyType> BulkDeleteList(IEnumerable<T> objs, int? commandTimeout = null)
		{
			PropertyInfo prop = KeyProperties[0];
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				conn.Open();
				using (SqlTransaction trans = conn.BeginTransaction()) {
					IEnumerable<KeyType> keys = QueriesKey.BulkDeleteListFunc(conn, objs.Select(obj => (KeyType) prop.GetValue(obj)), null, true, commandTimeout);
					trans.Commit();
					return keys;
				}
			}
		}


		public override IEnumerable<T> BulkUpdateList(IEnumerable<T> objs, int? commandTimeout = null)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				conn.Open();
				using (SqlTransaction trans = conn.BeginTransaction()) {
					IEnumerable<T> list = Queries.BulkUpdateListFunc(conn, objs, null, commandTimeout);
					trans.Commit();
					return list;
				}
			}
		}

		public override IEnumerable<T> BulkUpsertList(IEnumerable<T> objs, int? commandTimeout = null)
		{
			using (SqlConnection conn = new SqlConnection(ConnectionString)) {
				conn.Open();
				using (SqlTransaction trans = conn.BeginTransaction()) {
					IEnumerable<T> list = Queries.BulkUpsertListFunc(conn, objs, null, commandTimeout);
					trans.Commit();
					return list;
				}
			}
		}
		#endregion IDataAccessObjectSync<T, KeyType, Ret>


		#region ITransactionQueriesSync<T, KeyType, Ret>
		public override IEnumerable<KeyType> GetKeys(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			return QueriesKey.GetKeysFunc(transaction.Connection, whereCondition, param, transaction, buffered, commandTimeout);
		}

		public override bool Delete(IDbTransaction transaction, KeyType key, int? commandTimeout = null)
		{
			return QueriesKey.DeleteFunc(transaction.Connection, key, transaction, commandTimeout);
		}

		public override int BulkDelete(SqlTransaction transaction, IEnumerable<KeyType> objs, int? commandTimeout = null)
		{
			return QueriesKey.BulkDeleteFunc(transaction.Connection, objs, transaction, commandTimeout);
		}

		public override T Get(IDbTransaction transaction, KeyType key, int? commandTimeout = null)
		{
			return QueriesKey.GetFunc(transaction.Connection, key, transaction, commandTimeout);
		}

		public override IEnumerable<KeyType> DeleteList(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			return QueriesKey.DeleteListFunc(transaction.Connection, whereCondition, param, transaction, buffered, commandTimeout);
		}

		public override bool Delete(IDbTransaction transaction, IDictionary<string, object> key, int? commandTimeout = null)
		{
			return Queries.DeleteDictFunc(transaction.Connection, key, transaction, commandTimeout);
		}

		public override bool Delete(IDbTransaction transaction, T obj, int? commandTimeout = null)
		{
			return Queries.DeleteFunc(transaction.Connection, obj, transaction, commandTimeout);
		}

		public override int BulkDelete(SqlTransaction transaction, IEnumerable<T> objs, int? commandTimeout = null)
		{
			return Queries.BulkDeleteFunc(transaction.Connection, objs, transaction, commandTimeout);
		}

		public override int Delete(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			return Queries.DeleteWhereFunc(transaction.Connection, whereCondition, param, transaction, commandTimeout);
		}

		public override T Insert(IDbTransaction transaction, T obj, int? commandTimeout = null)
		{
			return Queries.InsertFunc(transaction.Connection, obj, transaction, commandTimeout);
		}

		public override IEnumerable<T> BulkInsert(SqlTransaction transaction, IEnumerable<T> objs, int? commandTimeout = null)
		{
			return Queries.BulkInsertFunc(transaction.Connection, objs, transaction, commandTimeout);
		}

		public override bool Update(IDbTransaction transaction, T obj, int? commandTimeout = null)
		{
			return Queries.UpdateFunc(transaction.Connection, obj, transaction, commandTimeout);
		}

		public override int BulkUpdate(SqlTransaction transaction, IEnumerable<T> objs, int? commandTimeout = null)
		{
			return Queries.BulkUpdateFunc(transaction.Connection, objs, transaction, commandTimeout);
		}

		public override T Upsert(IDbTransaction transaction, T obj, int? commandTimeout = null)
		{
			return Queries.UpsertFunc(transaction.Connection, obj, transaction, commandTimeout);
		}

		public override int BulkUpsert(SqlTransaction transaction, IEnumerable<T> objs, int? commandTimeout = null)
		{
			return Queries.BulkUpsertFunc(transaction.Connection, objs, transaction, commandTimeout);
		}

		public override T Get(IDbTransaction transaction, IDictionary<string, object> key, int? commandTimeout = null)
		{
			return Queries.GetDictFunc(transaction.Connection, key, transaction, commandTimeout);
		}

		public override T Get(IDbTransaction transaction, T obj, int? commandTimeout = null)
		{
			return Queries.GetFunc(transaction.Connection, obj, transaction, commandTimeout);
		}

		public override IEnumerable<T> GetList(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			return Queries.GetListFunc(transaction.Connection, whereCondition, param, transaction, buffered, commandTimeout);
		}

		public override IEnumerable<T> GetTop(IDbTransaction transaction, int limit, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			return Queries.GetTopFunc(transaction.Connection, limit, whereCondition, param, transaction, buffered, commandTimeout);
		}

		public override IEnumerable<T> GetDistinct(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			return Queries.GetDistinctFunc(transaction.Connection, whereCondition, param, transaction, buffered, commandTimeout);
		}

		public override int RecordCount(IDbTransaction transaction, string whereCondition = "", object param = null, int? commandTimeout = null)
		{
			return Queries.RecordCountFunc(transaction.Connection, whereCondition, param, transaction, commandTimeout);
		}

		public override IEnumerable<KeyType> BulkDeleteList(SqlTransaction transaction, IEnumerable<T> objs, int? commandTimeout = null)
		{
			PropertyInfo prop = KeyProperties[0];
			return QueriesKey.BulkDeleteListFunc(transaction.Connection, objs.Select(obj => (KeyType)prop.GetValue(obj)), transaction, true, commandTimeout);
		}

		public override IEnumerable<T> BulkUpdateList(SqlTransaction transaction, IEnumerable<T> objs, int? commandTimeout = null)
		{
			return Queries.BulkUpdateListFunc(transaction.Connection, objs, transaction, commandTimeout);
		}

		public override IEnumerable<T> BulkUpsertList(SqlTransaction transaction, IEnumerable<T> objs, int? commandTimeout = null)
		{
			return Queries.BulkUpsertListFunc(transaction.Connection, objs, transaction, commandTimeout);
		}
		#endregion ITransactionQueriesSync<T, KeyType, Ret>
	}
}
