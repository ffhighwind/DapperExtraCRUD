using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Extra.Interfaces
{
	public interface IAccessObjectAsync<T>
		where T : class
	{
		Task<IEnumerable<T>> GetKeysAsync(string whereCondition = "", object param = null, int? commandTimeout = null);
		Task<IEnumerable<KeyType>> GetKeysAsync<KeyType>(string whereCondition = "", object param = null, int? commandTimeout = null);

		Task<bool> DeleteAsync(T obj, int? commandTimeout = null);
		Task<bool> DeleteAsync<KeyType>(KeyType key, int? commandTimeout = null);
		Task<int> DeleteAsync(string whereCondition = "", object param = null, int? commandTimeout = null);
		Task<IEnumerable<T>> DeleteListAsync(string whereCondition = "", object param = null, int? commandTimeout = null);

		Task InsertAsync(T obj, int? commandTimeout = null);
		Task<bool> UpdateAsync(T obj, object filter = null, int? commandTimeout = null);
		Task<bool> UpdateAsync(T obj);
		Task<bool> UpsertAsync(T obj, int? commandTimeout = null);

		Task<T> GetAsync(T obj, int? commandTimeout = null);
		Task<T> GetAsync<KeyType>(KeyType key, int? commandTimeout = null);
		Task<IEnumerable<T>> GetListAsync(string whereCondition = "", object param = null, int? commandTimeout = null);
		Task<IEnumerable<T>> GetLimitAsync(int limit, string whereCondition = "", object param = null, int? commandTimeout = null);
		Task<IEnumerable<T>> GetDistinctAsync(string whereCondition = "", object param = null, int? commandTimeout = null);
		Task<IEnumerable<T>> GetDistinctLimitAsync(int limit, string whereCondition = "", object param = null, int? commandTimeout = null);

		Task<int> RecordCountAsync(string whereCondition = "", object param = null, int? commandTimeout = null);

		Task<List<T>> BulkGetAsync(IEnumerable<T> keys, int? commandTimeout = null);
		Task<List<T>> BulkGetAsync<KeyType>(IEnumerable<KeyType> keys, int? commandTimeout = null);
		Task BulkInsertAsync(IEnumerable<T> objs, int? commandTimeout = null);
		Task<int> BulkUpdateAsync(IEnumerable<T> objs, int? commandTimeout = null);
		Task<int> BulkDeleteAsync(IEnumerable<T> objs, int? commandTimeout = null);
		Task<int> BulkDeleteAsync<KeyType>(IEnumerable<KeyType> keys, int? commandTimeout = null);
		Task<int> BulkUpsertAsync(IEnumerable<T> objs, int? commandTimeout = null);
	}
}
