using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Extra.Utilities
{
	public interface IAccessObjectAsync<T>
		where T : class
	{
		Task<IEnumerable<T>> GetKeysAsync(string whereCondition = "", object param = null, int commandTimeout = 30);
		Task<IEnumerable<KeyType>> GetKeysAsync<KeyType>(string whereCondition = "", object param = null, int commandTimeout = 30);

		Task<bool> DeleteAsync(T obj, int commandTimeout = 30);
		Task<bool> DeleteAsync<KeyType>(KeyType key, int commandTimeout = 30);
		Task<int> DeleteAsync(string whereCondition = "", object param = null, int commandTimeout = 30);
		Task<IEnumerable<T>> DeleteListAsync(string whereCondition = "", object param = null, int commandTimeout = 30);

		Task InsertAsync(T obj, int commandTimeout = 30);
		Task<bool> UpdateAsync(object obj, int commandTimeout = 30);
		Task<bool> UpdateAsync(T obj, int commandTimeout = 30);
		Task<bool> UpsertAsync(T obj, int commandTimeout = 30);

		Task<T> GetAsync(T obj, int commandTimeout = 30);
		Task<T> GetAsync<KeyType>(KeyType key, int commandTimeout = 30);
		Task<IEnumerable<T>> GetListAsync(string whereCondition = "", object param = null, int commandTimeout = 30);
		Task<IEnumerable<T>> GetLimitAsync(int limit, string whereCondition = "", object param = null, int commandTimeout = 30);
		Task<IEnumerable<T>> GetDistinctAsync(string whereCondition = "", object param = null, int commandTimeout = 30);
		Task<IEnumerable<T>> GetDistinctLimitAsync(int limit, string whereCondition = "", object param = null, int commandTimeout = 30);

		Task<int> RecordCountAsync(string whereCondition = "", object param = null, int commandTimeout = 30);

		Task<List<T>> BulkGetAsync(IEnumerable<T> keys, int commandTimeout = 30);
		Task<List<T>> BulkGetAsync<KeyType>(IEnumerable<KeyType> keys, int commandTimeout = 30);
		Task BulkInsertAsync(IEnumerable<T> objs, int commandTimeout = 30);
		Task<int> BulkUpdateAsync(IEnumerable<T> objs, int commandTimeout = 30);
		Task<int> BulkDeleteAsync(IEnumerable<T> objs, int commandTimeout = 30);
		Task<int> BulkDeleteAsync<KeyType>(IEnumerable<KeyType> keys, int commandTimeout = 30);
		Task<int> BulkUpsertAsync(IEnumerable<T> objs, int commandTimeout = 30);
	}
}
