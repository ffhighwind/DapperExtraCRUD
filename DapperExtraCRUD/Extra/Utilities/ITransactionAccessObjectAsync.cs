using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Dapper.Extra.Utilities
{
	public interface ITransactionAccessObjectAsync<T> where T : class
	{
		Task<int> BulkDeleteAsync(SqlTransaction transaction, IEnumerable<T> objs, int commandTimeout = 30);
		Task BulkInsertAsync(SqlTransaction transaction, IEnumerable<T> objs, int commandTimeout = 30);
		Task<int> BulkUpdateAsync(SqlTransaction transaction, IEnumerable<T> objs, int commandTimeout = 30);
		Task<int> BulkUpsertAsync(SqlTransaction transaction, IEnumerable<T> objs, int commandTimeout = 30);
		Task<int> BulkInsertIfNotExistsAsync(SqlTransaction transaction, IEnumerable<T> objs, int commandTimeout = 30);
		Task<int> BulkDeleteAsync(SqlTransaction transaction, IEnumerable<object> keys, int commandTimeout = 30);

		Task<bool> DeleteAsync(IDbTransaction transaction, T obj, int commandTimeout = 30);
		Task<int> DeleteAsync(IDbTransaction transaction, string whereCondition = "", object param = null, int commandTimeout = 30);
		Task<T> GetAsync(IDbTransaction transaction, T obj, int commandTimeout = 30);
		Task<IEnumerable<T>> GetDistinctAsync(IDbTransaction transaction, string whereCondition = "", object param = null, int commandTimeout = 30);
		Task<IEnumerable<T>> GetDistinctAsync(IDbTransaction transaction, Type columnFilter, string whereCondition = "", object param = null, int commandTimeout = 30);
		Task<IEnumerable<T>> GetKeysAsync(IDbTransaction transaction, string whereCondition = "", object param = null, int commandTimeout = 30);
		Task<IEnumerable<T>> GetListAsync(IDbTransaction transaction, string whereCondition = "", object param = null, int commandTimeout = 30);
		Task<IEnumerable<T>> GetListAsync(IDbTransaction transaction, Type columnFilter, string whereCondition = "", object param = null, int commandTimeout = 30);
		Task<IEnumerable<T>> GetLimitAsync(IDbTransaction transaction, int limit, string whereCondition = "", object param = null, int commandTimeout = 30);
		Task<IEnumerable<T>> GetLimitAsync(IDbTransaction transaction, Type columnFilter, int limit, string whereCondition = "", object param = null, int commandTimeout = 30);

		Task InsertAsync(IDbTransaction transaction, T obj, int commandTimeout = 30);
		Task<int> RecordCountAsync(IDbTransaction transaction, string whereCondition = "", object param = null, int commandTimeout = 30);
		Task<bool> UpdateAsync(IDbTransaction transaction, object obj, int commandTimeout = 30);
		Task<bool> UpdateAsync(IDbTransaction transaction, T obj, int commandTimeout = 30);
		Task<bool> UpsertAsync(IDbTransaction transaction, T obj, int commandTimeout = 30);
		Task<bool> InsertIfNotExistsAsync(IDbTransaction transaction, T obj, int commandTimeout = 30);
		Task<IEnumerable<T>> GetDistinctLimitAsync(IDbTransaction transaction, int limit, string whereCondition = "", object param = null, int commandTimeout = 30);
		Task<IEnumerable<T>> GetDistinctLimitAsync(IDbTransaction transaction, Type columnFilter, int limit, string whereCondition = "", object param = null, int commandTimeout = 30);

		Task<T> Get(IDbTransaction transaction, object key, int commandTimeout = 30);
		Task<bool> Delete(IDbTransaction transaction, object key, int commandTimeout = 30);
		Task<IEnumerable<KeyType>> GetKeys<KeyType>(IDbTransaction transaction, string whereCondition = "", object param = null, int commandTimeout = 30);
		Task<List<T>> BulkGet(SqlTransaction transaction, IEnumerable<T> keys, int commandTimeout = 30);
		Task<List<T>> BulkGet(SqlTransaction transaction, IEnumerable<object> keys, int commandTimeout = 30);
	}
}
