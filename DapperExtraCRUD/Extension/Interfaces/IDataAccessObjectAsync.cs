using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Extension.Interfaces
{
	public interface IDataAccessObjectAsync<T> : IDataAccessObjectAsync<T, T> where T : class {	}

	public interface IDataAccessObjectAsync<T, Ret> where T : class
	{
		#region IDbConnection
		Task<IEnumerable<T>> GetKeysAsync(string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);

		Task<bool> DeleteAsync(IDictionary<string, object> key, int? commandTimeout = null);
		Task<bool> DeleteAsync(T obj, int? commandTimeout = null);
		Task<int> DeleteAsync(string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);
		Task<IEnumerable<T>> DeleteListAsync(string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);

		Task<Ret> InsertAsync(T obj, int? commandTimeout = null);
		Task<bool> UpdateAsync(T obj, int? commandTimeout = null);
		Task<Ret> UpsertAsync(T obj, int? commandTimeout = null);

		Task<Ret> GetAsync(IDictionary<string, object> key, int? commandTimeout = null);
		Task<Ret> GetAsync(T obj, int? commandTimeout = null);
		Task<IEnumerable<Ret>> GetListAsync(string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);
		Task<IEnumerable<Ret>> GetTopAsync(int limit, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);
		Task<IEnumerable<Ret>> GetDistinctAsync(string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);

		Task<int> RecordCountAsync(string whereCondition = "", object param = null, int? commandTimeout = null);

		Task<IEnumerable<Ret>> BulkInsertAsync(IEnumerable<T> objs, int? commandTimeout = null);
		Task<int> BulkUpdateAsync(IEnumerable<T> objs, int? commandTimeout = null);
		Task<int> BulkDeleteAsync(IEnumerable<T> objs, int? commandTimeout = null);
		Task<int> BulkUpsertAsync(IEnumerable<T> objs, int? commandTimeout = null);

		Task<IEnumerable<Ret>> BulkUpdateListAsync(IEnumerable<T> objs, int? commandTimeout = null);
		Task<IEnumerable<T>> BulkDeleteListAsync(IEnumerable<T> objs, int? commandTimeout = null);
		Task<IEnumerable<Ret>> BulkUpsertListAsync(IEnumerable<T> objs, int? commandTimeout = null);
		#endregion IDbConnection

		#region IDbTransaction
		Task<bool> DeleteAsync(IDbTransaction transaction, IDictionary<string, object> key, int? commandTimeout = null);
		Task<bool> DeleteAsync(IDbTransaction transaction, T obj, int? commandTimeout = null);
		Task<int> DeleteAsync(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);
		Task<IEnumerable<T>> DeleteListAsync(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);

		Task<Ret> InsertAsync(IDbTransaction transaction, T obj, int? commandTimeout = null);
		Task<bool> UpdateAsync(IDbTransaction transaction, T obj, int? commandTimeout = null);
		//Task<int> UpdateAsync(IDbTransaction transaction, string whereCondition = "", object param = null, int? commandTimeout = null);

		Task<Ret> UpsertAsync(IDbTransaction transaction, T obj, int? commandTimeout = null);

		Task<IEnumerable<T>> GetKeysAsync(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);
		Task<Ret> GetAsync(IDbTransaction transaction, IDictionary<string, object> key, int? commandTimeout = null);
		Task<Ret> GetAsync(IDbTransaction transaction, T obj, int? commandTimeout = null);
		Task<IEnumerable<Ret>> GetListAsync(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);
		Task<IEnumerable<Ret>> GetTopAsync(IDbTransaction transaction, int limit, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);
		Task<IEnumerable<Ret>> GetDistinctAsync(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);

		Task<int> RecordCountAsync(IDbTransaction transaction, string whereCondition = "", object param = null, int? commandTimeout = null);

		Task<IEnumerable<Ret>> BulkInsertAsync(SqlTransaction transaction, IEnumerable<T> objs, int? commandTimeout = null);
		Task<int> BulkUpdateAsync(SqlTransaction transaction, IEnumerable<T> objs, int? commandTimeout = null);
		Task<int> BulkDeleteAsync(SqlTransaction transaction, IEnumerable<T> objs, int? commandTimeout = null);
		Task<int> BulkUpsertAsync(SqlTransaction transaction, IEnumerable<T> objs, int? commandTimeout = null);

		Task<IEnumerable<Ret>> BulkUpdateListAsync(SqlTransaction transaction, IEnumerable<T> objs, int? commandTimeout = null);
		Task<IEnumerable<T>> BulkDeleteListAsync(SqlTransaction transaction, IEnumerable<T> objs, int? commandTimeout = null);
		Task<IEnumerable<Ret>> BulkUpsertListAsync(SqlTransaction transaction, IEnumerable<T> objs, int? commandTimeout = null);
		#endregion IDbTransaction
	}


	public interface IDataAccessObjectAsync<T, KeyType, Ret> where T : class
	{
		#region IDbConnection
		Task<IEnumerable<KeyType>> GetKeysAsync(string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);

		Task<bool> DeleteAsync(KeyType key, int? commandTimeout = null);
		Task<bool> DeleteAsync(IDictionary<string, object> key, int? commandTimeout = null);
		Task<bool> DeleteAsync(T obj, int? commandTimeout = null);
		Task<int> DeleteAsync(string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);
		Task<IEnumerable<KeyType>> DeleteListAsync(string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);

		Task<Ret> InsertAsync(T obj, int? commandTimeout = null);
		Task<bool> UpdateAsync(T obj, int? commandTimeout = null);
		Task<Ret> UpsertAsync(T obj, int? commandTimeout = null);

		Task<Ret> GetAsync(KeyType key, int? commandTimeout = null);
		Task<Ret> GetAsync(IDictionary<string, object> key, int? commandTimeout = null);
		Task<Ret> GetAsync(T obj, int? commandTimeout = null);
		Task<IEnumerable<Ret>> GetListAsync(string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);
		Task<IEnumerable<Ret>> GetTopAsync(int limit, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);
		Task<IEnumerable<Ret>> GetDistinctAsync(string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);

		Task<int> RecordCountAsync(string whereCondition = "", object param = null, int? commandTimeout = null);

		Task<IEnumerable<Ret>> BulkInsertAsync(IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null);
		Task<int> BulkUpdateAsync(IEnumerable<T> objs, int? commandTimeout = null);
		Task<int> BulkDeleteAsync(IEnumerable<KeyType> keys, int? commandTimeout = null);
		Task<int> BulkDeleteAsync(IEnumerable<T> objs, int? commandTimeout = null);
		Task<int> BulkUpsertAsync(IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null);

		Task<IEnumerable<Ret>> BulkUpdateListAsync(IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null);
		Task<IEnumerable<KeyType>> BulkDeleteListAsync(IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null);
		Task<IEnumerable<Ret>> BulkUpsertListAsync(IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null);
		#endregion IDbConnection

		#region IDbTransaction
		Task<bool> DeleteAsync(IDbTransaction transaction, KeyType key, int? commandTimeout = null);
		Task<bool> DeleteAsync(IDbTransaction transaction, IDictionary<string, object> key, int? commandTimeout = null);
		Task<bool> DeleteAsync(IDbTransaction transaction, T obj, int? commandTimeout = null);
		Task<int> DeleteAsync(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);
		Task<IEnumerable<KeyType>> DeleteListAsync(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);

		Task<Ret> InsertAsync(IDbTransaction transaction, T obj, int? commandTimeout = null);
		Task<bool> UpdateAsync(IDbTransaction transaction, T obj, int? commandTimeout = null);
		//Task<int> UpdateAsync(IDbTransaction transaction, string whereCondition = "", object param = null, int? commandTimeout = null);
		Task<Ret> UpsertAsync(IDbTransaction transaction, T obj, int? commandTimeout = null);

		Task<IEnumerable<KeyType>> GetKeysAsync(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);
		Task<Ret> GetAsync(IDbTransaction transaction, KeyType key, int? commandTimeout = null);
		Task<Ret> GetAsync(IDbTransaction transaction, IDictionary<string, object> key, int? commandTimeout = null);
		Task<Ret> GetAsync(IDbTransaction transaction, T obj, int? commandTimeout = null);
		Task<IEnumerable<Ret>> GetListAsync(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);
		Task<IEnumerable<Ret>> GetTopAsync(IDbTransaction transaction, int limit, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);
		Task<IEnumerable<Ret>> GetDistinctAsync(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);

		Task<int> RecordCountAsync(IDbTransaction transaction, string whereCondition = "", object param = null, int? commandTimeout = null);

		Task<IEnumerable<Ret>> BulkInsertAsync(SqlTransaction transaction, IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null);
		Task<int> BulkUpdateAsync(SqlTransaction transaction, IEnumerable<T> objs, int? commandTimeout = null);
		Task<int> BulkDeleteAsync(SqlTransaction transaction, IEnumerable<KeyType> keys, int? commandTimeout = null);
		Task<int> BulkDeleteAsync(SqlTransaction transaction, IEnumerable<T> objs, int? commandTimeout = null);
		Task<int> BulkUpsertAsync(SqlTransaction transaction, IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null);

		Task<IEnumerable<Ret>> BulkUpdateListAsync(SqlTransaction transaction, IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null);
		Task<IEnumerable<KeyType>> BulkDeleteListAsync(SqlTransaction transaction, IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null);
		Task<IEnumerable<Ret>> BulkUpsertListAsync(SqlTransaction transaction, IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null);
		#endregion IDbTransaction
	}
}
