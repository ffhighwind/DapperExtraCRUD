using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Extension.Interfaces
{
	public interface IDataAccessObjectSync<T> : IDataAccessObjectSync<T, T> where T : class { }

	public interface IDataAccessObjectSync<T, Ret> where T : class
	{
		#region IDbConnection
		IEnumerable<T> GetKeys(string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);

		bool Delete(IDictionary<string, object> key, int? commandTimeout = null);
		bool Delete(T obj, int? commandTimeout = null);
		int Delete(string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);
		IEnumerable<T> DeleteList(string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);

		Ret Insert(T obj, int? commandTimeout = null);
		bool Update(T obj, int? commandTimeout = null);
		Ret Upsert(T obj, int? commandTimeout = null);

		Ret Get(IDictionary<string, object> key, int? commandTimeout = null);
		Ret Get(T obj, int? commandTimeout = null);
		IEnumerable<Ret> GetList(string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);
		IEnumerable<Ret> GetTop(int limit, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);
		IEnumerable<Ret> GetDistinct(string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);

		int RecordCount(string whereCondition = "", object param = null, int? commandTimeout = null);

		IEnumerable<Ret> BulkInsert(IEnumerable<T> objs, int? commandTimeout = null);
		int BulkUpdate(IEnumerable<T> objs, int? commandTimeout = null);
		int BulkDelete(IEnumerable<T> objs, int? commandTimeout = null);
		int BulkUpsert(IEnumerable<T> objs, int? commandTimeout = null);

		IEnumerable<Ret> BulkUpdateList(IEnumerable<T> objs, int? commandTimeout = null);
		IEnumerable<T> BulkDeleteList(IEnumerable<T> objs, int? commandTimeout = null);
		IEnumerable<Ret> BulkUpsertList(IEnumerable<T> objs, int? commandTimeout = null);
		#endregion IDbConnection

		#region IDbTransaction
		bool Delete(IDbTransaction transaction, IDictionary<string, object> key, int? commandTimeout = null);
		bool Delete(IDbTransaction transaction, T obj, int? commandTimeout = null);
		int Delete(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);
		IEnumerable<T> DeleteList(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);

		Ret Insert(IDbTransaction transaction, T obj, int? commandTimeout = null);
		bool Update(IDbTransaction transaction, T obj, int? commandTimeout = null);
		//int Update(IDbTransaction transaction, string whereCondition = "", object param = null, int? commandTimeout = null);
		Ret Upsert(IDbTransaction transaction, T obj, int? commandTimeout = null);

		IEnumerable<T> GetKeys(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);
		Ret Get(IDbTransaction transaction, IDictionary<string, object> key, int? commandTimeout = null);
		Ret Get(IDbTransaction transaction, T obj, int? commandTimeout = null);
		IEnumerable<Ret> GetList(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);
		IEnumerable<Ret> GetTop(IDbTransaction transaction, int limit, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);
		IEnumerable<Ret> GetDistinct(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);

		int RecordCount(IDbTransaction transaction, string whereCondition = "", object param = null, int? commandTimeout = null);

		IEnumerable<Ret> BulkInsert(SqlTransaction transaction, IEnumerable<T> objs, int? commandTimeout = null);
		int BulkUpdate(SqlTransaction transaction, IEnumerable<T> objs, int? commandTimeout = null);
		int BulkDelete(SqlTransaction transaction, IEnumerable<T> objs, int? commandTimeout = null);
		int BulkUpsert(SqlTransaction transaction, IEnumerable<T> objs, int? commandTimeout = null);

		IEnumerable<Ret> BulkUpdateList(SqlTransaction transaction, IEnumerable<T> objs, int? commandTimeout = null);
		IEnumerable<T> BulkDeleteList(SqlTransaction transaction, IEnumerable<T> objs, int? commandTimeout = null);
		IEnumerable<Ret> BulkUpsertList(SqlTransaction transaction, IEnumerable<T> objs, int? commandTimeout = null);
		#endregion IDbTransaction
	}



	public interface IDataAccessObjectSync<T, KeyType, Ret> where T : class
	{
		#region IDbConnection
		IEnumerable<KeyType> GetKeys(string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);

		bool Delete(KeyType key, int? commandTimeout = null);
		bool Delete(IDictionary<string, object> key, int? commandTimeout = null);
		bool Delete(T obj, int? commandTimeout = null);
		int Delete(string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);
		IEnumerable<KeyType> DeleteList(string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);

		Ret Insert(T obj, int? commandTimeout = null);
		bool Update(T obj, int? commandTimeout = null);
		Ret Upsert(T obj, int? commandTimeout = null);

		Ret Get(KeyType key, int? commandTimeout = null);
		Ret Get(IDictionary<string, object> key, int? commandTimeout = null);
		Ret Get(T obj, int? commandTimeout = null);
		IEnumerable<Ret> GetList(string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);
		IEnumerable<Ret> GetTop(int limit, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);
		IEnumerable<Ret> GetDistinct(string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);

		int RecordCount(string whereCondition = "", object param = null, int? commandTimeout = null);

		IEnumerable<Ret> BulkInsert(IEnumerable<T> objs, int? commandTimeout = null);
		int BulkUpdate(IEnumerable<T> objs, int? commandTimeout = null);
		int BulkDelete(IEnumerable<KeyType> keys, int? commandTimeout = null);
		int BulkDelete(IEnumerable<T> objs, int? commandTimeout = null);
		int BulkUpsert(IEnumerable<T> objs, int? commandTimeout = null);

		IEnumerable<Ret> BulkUpdateList(IEnumerable<T> objs, int? commandTimeout = null);
		IEnumerable<KeyType> BulkDeleteList(IEnumerable<T> objs, int? commandTimeout = null);
		IEnumerable<Ret> BulkUpsertList(IEnumerable<T> objs, int? commandTimeout = null);
		#endregion IDbConnection

		#region IDbTransaction
		bool Delete(IDbTransaction transaction, KeyType key, int? commandTimeout = null);
		bool Delete(IDbTransaction transaction, IDictionary<string, object> key, int? commandTimeout = null);
		bool Delete(IDbTransaction transaction, T obj, int? commandTimeout = null);
		int Delete(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);
		IEnumerable<KeyType> DeleteList(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);

		Ret Insert(IDbTransaction transaction, T obj, int? commandTimeout = null);
		bool Update(IDbTransaction transaction, T obj, int? commandTimeout = null);
		//int Update(IDbTransaction transaction, string whereCondition = "", object param = null, int? commandTimeout = null);
		Ret Upsert(IDbTransaction transaction, T obj, int? commandTimeout = null);

		IEnumerable<KeyType> GetKeys(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);
		Ret Get(IDbTransaction transaction, KeyType key, int? commandTimeout = null);
		Ret Get(IDbTransaction transaction, IDictionary<string, object> key, int? commandTimeout = null);
		Ret Get(IDbTransaction transaction, T obj, int? commandTimeout = null);
		IEnumerable<Ret> GetList(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);
		IEnumerable<Ret> GetTop(IDbTransaction transaction, int limit, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);
		IEnumerable<Ret> GetDistinct(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);

		int RecordCount(IDbTransaction transaction, string whereCondition = "", object param = null, int? commandTimeout = null);

		IEnumerable<Ret> BulkInsert(SqlTransaction transaction, IEnumerable<T> objs, int? commandTimeout = null);
		int BulkUpdate(SqlTransaction transaction, IEnumerable<T> objs, int? commandTimeout = null);
		int BulkDelete(SqlTransaction transaction, IEnumerable<T> objs, int? commandTimeout = null);
		int BulkDelete(SqlTransaction transaction, IEnumerable<KeyType> keys, int? commandTimeout = null);
		int BulkUpsert(SqlTransaction transaction, IEnumerable<T> objs, int? commandTimeout = null);

		IEnumerable<Ret> BulkUpdateList(SqlTransaction transaction, IEnumerable<T> objs, int? commandTimeout = null);
		IEnumerable<KeyType> BulkDeleteList(SqlTransaction transaction, IEnumerable<T> objs, int? commandTimeout = null);
		IEnumerable<Ret> BulkUpsertList(SqlTransaction transaction, IEnumerable<T> objs, int? commandTimeout = null);
		#endregion IDbTransaction
	}
}