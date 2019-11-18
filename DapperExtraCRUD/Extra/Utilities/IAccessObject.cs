// Released under MIT License 
// Copyright(c) 2018 Wesley Hamilton
// License: https://www.mit.edu/~amini/LICENSE.md
// Home page: https://github.com/ffhighwind/DapperExtraCRUD

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dapper.Extra.Internal;
using Dapper.Extra.Utilities;

namespace Dapper.Extra.Utilities
{
	public abstract class IAccessObject<T> : IAccessObjectSync<T>, IAccessObjectAsync<T>
		where T : class
	{
		#region IAccessObjectSync<T>
		public abstract IEnumerable<T> GetKeys(string whereCondition = "", object param = null, int commandTimeout = 30);
		public abstract IEnumerable<KeyType> GetKeys<KeyType>(string whereCondition = "", object param = null, int commandTimeout = 30);

		public abstract bool Delete(T obj, int commandTimeout = 30);
		public abstract int Delete(string whereCondition = "", object param = null, int commandTimeout = 30);
		public abstract bool Delete<KeyType>(KeyType key, int commandTimeout = 30);

		public abstract void Insert(T obj, int commandTimeout = 30);
		public abstract bool Update(object obj, int commandTimeout = 30);
		public abstract bool Update(T obj, int commandTimeout = 30);
		public abstract bool Upsert(T obj, int commandTimeout = 30);
		public abstract bool InsertIfNotExists(T obj, int commandTimeout = 30);

		public abstract T Get(T obj, int commandTimeout = 30);
		public abstract T Get<KeyType>(KeyType key, int commandTimeout = 30);
		public abstract IEnumerable<T> GetList(string whereCondition = "", object param = null, int commandTimeout = 30);
		public abstract IEnumerable<T> GetLimit(int limit, string whereCondition = "", object param = null, int commandTimeout = 30);
		public abstract IEnumerable<T> GetDistinct(string whereCondition = "", object param = null, int commandTimeout = 30);
		public abstract IEnumerable<T> GetDistinctLimit(int limit, string whereCondition = "", object param = null, int commandTimeout = 30);

		public abstract int RecordCount(string whereCondition = "", object param = null, int commandTimeout = 30);

		public abstract List<T> BulkGet(IEnumerable<T> keys, int commandTimeout = 30);
		public abstract List<T> BulkGet<KeyType>(IEnumerable<KeyType> keys, int commandTimeout = 30);
		public abstract void BulkInsert(IEnumerable<T> objs, int commandTimeout = 30);
		public abstract int BulkUpdate(IEnumerable<T> objs, int commandTimeout = 30);
		public abstract int BulkDelete(IEnumerable<T> objs, int commandTimeout = 30);
		public abstract int BulkDelete<KeyType>(IEnumerable<KeyType> keys, int commandTimeout = 30);
		public abstract int BulkUpsert(IEnumerable<T> objs, int commandTimeout = 30);
		public abstract int BulkInsertIfNotExists(IEnumerable<T> objs, int commandTimeout = 30);
		#endregion IAccessObjectSync<T>

		#region IAccessObjectAsync<T>
		public async Task<IEnumerable<T>> GetKeysAsync(string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			return await Task.Run(() => GetKeys(whereCondition, param, commandTimeout));
		}

		public async Task<bool> DeleteAsync(T obj, int commandTimeout = 30)
		{
			return await Task.Run(() => Delete(obj, commandTimeout));
		}

		public async Task<int> DeleteAsync(string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			return await Task.Run(() => Delete(whereCondition, param, commandTimeout));
		}

		public async Task InsertAsync(T obj, int commandTimeout = 30)
		{
			await Task.Run(() => Insert(obj, commandTimeout));
		}

		public async Task<bool> UpdateAsync(object obj, int commandTimeout = 30)
		{
			return await Task.Run(() => Update(obj, commandTimeout));
		}

		public async Task<bool> UpdateAsync(T obj, int commandTimeout = 30)
		{
			return await Task.Run(() => Update(obj, commandTimeout));
		}

		public async Task<bool> UpsertAsync(T obj, int commandTimeout = 30)
		{
			return await Task.Run(() => Upsert(obj, commandTimeout));
		}

		public async Task<bool> InsertIfNotExistsAsync(T obj, int commandTimeout = 30)
		{
			return await Task.Run(() => InsertIfNotExists(obj, commandTimeout));
		}

		public async Task<T> GetAsync(T obj, int commandTimeout = 30)
		{
			return await Task.Run(() => Get(obj, commandTimeout));
		}

		public async Task<IEnumerable<T>> GetListAsync(string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			return await Task.Run(() => GetList(whereCondition, param, commandTimeout));
		}

		public async Task<IEnumerable<T>> GetLimitAsync(int limit, string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			return await Task.Run(() => GetLimit(limit, whereCondition, param, commandTimeout));
		}

		public async Task<IEnumerable<T>> GetDistinctAsync(string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			return await Task.Run(() => GetDistinct(whereCondition, param, commandTimeout));
		}

		public async Task<IEnumerable<T>> GetDistinctLimitAsync(int limit, string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			return await Task.Run(() => GetDistinctLimit(limit, whereCondition, param, commandTimeout));
		}

		public async Task<int> RecordCountAsync(string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			return await Task.Run(() => RecordCount(whereCondition, param, commandTimeout));
		}

		public async Task BulkInsertAsync(IEnumerable<T> objs, int commandTimeout = 30)
		{
			await Task.Run(() => BulkInsert(objs, commandTimeout));
		}

		public async Task<int> BulkUpdateAsync(IEnumerable<T> objs, int commandTimeout = 30)
		{
			return await Task.Run(() => BulkUpdate(objs, commandTimeout));
		}

		public async Task<int> BulkDeleteAsync(IEnumerable<T> objs, int commandTimeout = 30)
		{
			return await Task.Run(() => BulkDelete(objs, commandTimeout));
		}

		public async Task<int> BulkUpsertAsync(IEnumerable<T> objs, int commandTimeout = 30)
		{
			return await Task.Run(() => BulkUpsert(objs, commandTimeout));
		}

		public async Task<bool> DeleteAsync<KeyType>(KeyType key, int commandTimeout = 30)
		{
			return await Task.Run(() => Delete(key, commandTimeout));
		}

		public async Task<T> GetAsync<KeyType>(KeyType key, int commandTimeout = 30)
		{
			return await Task.Run(() => Get(key, commandTimeout));
		}

		public async Task<int> BulkDeleteAsync<KeyType>(IEnumerable<KeyType> keys, int commandTimeout = 30)
		{
			return await Task.Run(() => BulkDelete(keys, commandTimeout));
		}

		public async Task<IEnumerable<KeyType>> GetKeysAsync<KeyType>(string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			return await Task.Run(() => GetKeys<KeyType>(whereCondition, param, commandTimeout));
		}

		public async Task<List<T>> BulkGetAsync(IEnumerable<T> keys, int commandTimeout = 30)
		{
			return await Task.Run(() => BulkGet(keys, commandTimeout));
		}

		public async Task<List<T>> BulkGetAsync<KeyType>(IEnumerable<KeyType> keys, int commandTimeout = 30)
		{
			return await Task.Run(() => BulkGet<KeyType>(keys, commandTimeout));
		}

		public async Task<int> BulkInsertIfNotExistsAsync(IEnumerable<T> objs, int commandTimeout = 30)
		{
			return await Task.Run(() => BulkInsertIfNotExists(objs, commandTimeout));
		}
		#endregion IAccessObjectAsync<T>
	}
}
