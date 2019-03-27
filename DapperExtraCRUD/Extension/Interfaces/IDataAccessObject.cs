using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Extension.Interfaces
{
	public abstract class IDataAccessObject<T> : IDataAccessObject<T, T> where T : class { }


	public abstract class IDataAccessObject<T, Ret> : IDataAccessObjectSync<T, Ret>, IDataAccessObjectAsync<T, Ret> where T : class
	{
		public IDataAccessObject()
		{
			Queries = TableData<T>.Queries;
			if (Queries == null) {
				TableQueries<T>.Data queryData = new TableQueries<T>.Factory().Create();
				throw new InvalidOperationException("Uncreachable");
			}
			TableName = TableData<T>.TableName;
		}

		public readonly string TableName;
		protected readonly TableQueries<T> Queries;

		#region IDataAccessObjectSync<T>

		public abstract bool Delete(IDictionary<string, object> key, int? commandTimeout = null);
		public abstract bool Delete(T obj, int? commandTimeout = null);
		public abstract int Delete(string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);
		public abstract IEnumerable<T> DeleteList(string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);
		public abstract Ret Get(IDictionary<string, object> key, int? commandTimeout = null);
		public abstract Ret Get(T obj, int? commandTimeout = null);
		public abstract IEnumerable<T> GetKeys(string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);
		public abstract IEnumerable<Ret> GetList(string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);
		public abstract IEnumerable<Ret> GetTop(int limit, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);
		public abstract IEnumerable<Ret> GetDistinct(string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);

		public abstract Ret Insert(T obj, int? commandTimeout = null);
		public abstract bool Update(T obj, int? commandTimeout = null);
		public abstract Ret Upsert(T obj, int? commandTimeout = null);
		public abstract int RecordCount(string whereCondition = "", object param = null, int? commandTimeout = null);

		public abstract IEnumerable<Ret> BulkInsert(IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null);
		public abstract int BulkUpdate(IEnumerable<T> objs, int? commandTimeout = null);
		public abstract int BulkDelete(IEnumerable<T> objs, int? commandTimeout = null);
		public abstract int BulkUpsert(IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null);

		public abstract IEnumerable<Ret> BulkUpdateList(IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null);
		public abstract IEnumerable<T> BulkDeleteList(IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null);
		public abstract IEnumerable<Ret> BulkUpsertList(IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null);
		#endregion IDataAccessObjectSync<T>

		#region IDataAccessObjectAsync<T>
		public async Task<bool> DeleteAsync(IDictionary<string, object> key, int? commandTimeout = null)
		{
			return await Task.Run(() => Delete(key, commandTimeout));
		}

		public async Task<bool> DeleteAsync(T obj, int? commandTimeout = null)
		{
			return await Task.Run(() => Delete(obj, commandTimeout));
		}

		public async Task<int> BulkDeleteAsync(IEnumerable<T> objs, int? commandTimeout = null)
		{
			return await Task.Run(() => BulkDelete(objs, commandTimeout));
		}

		public async Task<int> DeleteAsync(string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			return await Task.Run(() => Delete(whereCondition, param, buffered, commandTimeout));
		}

		public async Task<IEnumerable<T>> DeleteListAsync(string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			return await Task.Run(() => DeleteList(whereCondition, param, buffered, commandTimeout));
		}

		public async Task<Ret> GetAsync(IDictionary<string, object> key, int? commandTimeout = null)
		{
			return await Task.Run(() => Get(key, commandTimeout));
		}

		public async Task<Ret> GetAsync(T obj, int? commandTimeout = null)
		{
			return await Task.Run(() => Get(obj, commandTimeout));
		}

		public async Task<IEnumerable<T>> GetKeysAsync(string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			return await Task.Run(() => GetKeys(whereCondition, param, buffered, commandTimeout));
		}

		public async Task<IEnumerable<Ret>> GetListAsync(string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			return await Task.Run(() => GetList(whereCondition, param, buffered, commandTimeout));
		}

		public async Task<IEnumerable<Ret>> GetTopAsync(int limit, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			return await Task.Run(() => GetTop(limit, whereCondition, param, buffered, commandTimeout));
		}

		public async Task<IEnumerable<Ret>> GetDistinctAsync(string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			return await Task.Run(() => GetDistinct(whereCondition, param, buffered, commandTimeout));
		}

		public async Task<Ret> InsertAsync(T obj, int? commandTimeout = null)
		{
			return await Task.Run(() => Insert(obj, commandTimeout));
		}

		public async Task<IEnumerable<Ret>> BulkInsertAsync(IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null)
		{
			return await Task.Run(() => BulkInsert(objs, buffered, commandTimeout));
		}

		public async Task<int> RecordCountAsync(string whereCondition = "", object param = null, int? commandTimeout = null)
		{
			return await Task.Run(() => RecordCount(whereCondition, param, commandTimeout));
		}

		public async Task<bool> UpdateAsync(T obj, int? commandTimeout = null)
		{
			return await Task.Run(() => Update(obj, commandTimeout));
		}

		public async Task<int> BulkUpdateAsync(IEnumerable<T> objs, int? commandTimeout = null)
		{
			return await Task.Run(() => BulkUpdate(objs, commandTimeout));
		}

		public async Task<IEnumerable<Ret>> BulkUpdateListAsync(IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null)
		{
			return await Task.Run(() => BulkUpdateList(objs, buffered, commandTimeout));
		}


		public async Task<Ret> UpsertAsync(T obj, int? commandTimeout = null)
		{
			return await Task.Run(() => Upsert(obj, commandTimeout));
		}

		public async Task<IEnumerable<Ret>> BulkUpsertListAsync(IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null)
		{
			return await Task.Run(() => BulkUpsertList(objs, buffered, commandTimeout));
		}

		public async Task<int> BulkUpsertAsync(IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null)
		{
			return await Task.Run(() => BulkUpsert(objs, buffered, commandTimeout));
		}

		public async Task<IEnumerable<T>> BulkDeleteListAsync(IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null)
		{
			return await Task.Run(() => BulkDeleteList(objs, buffered, commandTimeout));
		}
		#endregion IDataAccessObjectAsync<T>

		#region ITransactionQueriesSync<T>
		public abstract bool Delete(IDbTransaction transaction, IDictionary<string, object> key, int? commandTimeout = null);
		public abstract bool Delete(IDbTransaction transaction, T obj, int? commandTimeout = null);
		public abstract int Delete(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);
		public abstract IEnumerable<T> DeleteList(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);
		public abstract Ret Get(IDbTransaction transaction, IDictionary<string, object> key, int? commandTimeout = null);
		public abstract Ret Get(IDbTransaction transaction, T obj, int? commandTimeout = null);
		public abstract IEnumerable<T> GetKeys(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);
		public abstract IEnumerable<Ret> GetList(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);
		public abstract IEnumerable<Ret> GetTop(IDbTransaction transaction, int limit, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);
		public abstract IEnumerable<Ret> GetDistinct(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);

		public abstract Ret Insert(IDbTransaction transaction, T obj, int? commandTimeout = null);
		public abstract bool Update(IDbTransaction transaction, T obj, int? commandTimeout = null);
		//public abstract int Update(IDbTransaction transaction, string whereCondition = "", object param = null, int? commandTimeout = null);
		public abstract Ret Upsert(IDbTransaction transaction, T obj, int? commandTimeout = null);
		public abstract int RecordCount(IDbTransaction transaction, string whereCondition = "", object param = null, int? commandTimeout = null);

		public abstract IEnumerable<Ret> BulkInsert(SqlTransaction transaction, IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null);
		public abstract int BulkUpdate(SqlTransaction transaction, IEnumerable<T> objs, int? commandTimeout = null);
		public abstract int BulkDelete(SqlTransaction transaction, IEnumerable<T> objs, int? commandTimeout = null);
		public abstract int BulkUpsert(SqlTransaction transaction, IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null);

		public abstract IEnumerable<Ret> BulkUpdateList(SqlTransaction transaction, IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null);
		public abstract IEnumerable<T> BulkDeleteList(SqlTransaction transaction, IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null);
		public abstract IEnumerable<Ret> BulkUpsertList(SqlTransaction transaction, IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null);
		#endregion ITransactionQueriesSync<T>

		#region ITransactionQueriesAsync<T>
		public async Task<bool> DeleteAsync(IDbTransaction transaction, IDictionary<string, object> key, int? commandTimeout = null)
		{
			return await Task.Run(() => Delete(transaction, key, commandTimeout));
		}

		public async Task<bool> DeleteAsync(IDbTransaction transaction, T obj, int? commandTimeout = null)
		{
			return await Task.Run(() => Delete(transaction, obj, commandTimeout));
		}

		public async Task<int> DeleteAsync(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			return await Task.Run(() => Delete(transaction, whereCondition, param, buffered, commandTimeout));
		}

		public async Task<IEnumerable<T>> DeleteListAsync(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			return await Task.Run(() => DeleteList(transaction, whereCondition, param, buffered, commandTimeout));
		}

		public async Task<Ret> GetAsync(IDbTransaction transaction, IDictionary<string, object> key, int? commandTimeout = null)
		{
			return await Task.Run(() => Get(transaction, key, commandTimeout));
		}

		public async Task<Ret> GetAsync(IDbTransaction transaction, T obj, int? commandTimeout = null)
		{
			return await Task.Run(() => Get(transaction, obj, commandTimeout));
		}

		public async Task<IEnumerable<T>> GetKeysAsync(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			return await Task.Run(() => GetKeys(transaction, whereCondition, param, buffered, commandTimeout));
		}

		public async Task<IEnumerable<Ret>> GetListAsync(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			return await Task.Run(() => GetList(transaction, whereCondition, param, buffered, commandTimeout));
		}

		public async Task<IEnumerable<Ret>> GetTopAsync(IDbTransaction transaction, int limit, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			return await Task.Run(() => GetTop(transaction, limit, whereCondition, param, buffered, commandTimeout));
		}

		public async Task<IEnumerable<Ret>> GetDistinctAsync(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			return await Task.Run(() => GetDistinct(transaction, whereCondition, param, buffered, commandTimeout));
		}

		public async Task<Ret> InsertAsync(IDbTransaction transaction, T obj, int? commandTimeout = null)
		{
			return await Task.Run(() => Insert(transaction, obj, commandTimeout));
		}

		public async Task<int> RecordCountAsync(IDbTransaction transaction, string whereCondition = "", object param = null, int? commandTimeout = null)
		{
			return await Task.Run(() => RecordCount(transaction, whereCondition, param, commandTimeout));
		}

		public async Task<bool> UpdateAsync(IDbTransaction transaction, T obj, int? commandTimeout = null)
		{
			return await Task.Run(() => Update(transaction, obj, commandTimeout));
		}

		//public async Task<int> UpdateAsync(IDbTransaction transaction, string whereCondition = "", object param = null, int? commandTimeout = null)
		//{
		//	return await Task.Run(() => Update(transaction, whereCondition, param, commandTimeout));
		//}

		public async Task<Ret> UpsertAsync(IDbTransaction transaction, T obj, int? commandTimeout = null)
		{
			return await Task.Run(() => Upsert(transaction, obj, commandTimeout));
		}

		public async Task<IEnumerable<Ret>> BulkInsertAsync(SqlTransaction transaction, IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null)
		{
			return await Task.Run(() => BulkInsert(transaction, objs, buffered, commandTimeout));
		}

		public async Task<int> BulkUpdateAsync(SqlTransaction transaction, IEnumerable<T> objs, int? commandTimeout = null)
		{
			return await Task.Run(() => BulkUpdate(transaction, objs, commandTimeout));
		}

		public async Task<int> BulkDeleteAsync(SqlTransaction transaction, IEnumerable<T> objs, int? commandTimeout = null)
		{
			return await Task.Run(() => BulkDelete(transaction, objs, commandTimeout));
		}

		public async Task<int> BulkUpsertAsync(SqlTransaction transaction, IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null)
		{
			return await Task.Run(() => BulkUpsert(transaction, objs, buffered, commandTimeout));
		}

		public async Task<IEnumerable<T>> BulkDeleteListAsync(SqlTransaction transaction, IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null)
		{
			return await Task.Run(() => BulkDeleteList(transaction, objs, buffered, commandTimeout));
		}

		public async Task<IEnumerable<Ret>> BulkUpdateListAsync(SqlTransaction transaction, IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null)
		{
			return await Task.Run(() => BulkUpdateList(transaction, objs, buffered, commandTimeout));
		}

		public async Task<IEnumerable<Ret>> BulkUpsertListAsync(SqlTransaction transaction, IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null)
		{
			return await Task.Run(() => BulkUpsertList(transaction, objs, buffered, commandTimeout));
		}

		#endregion ITransactionQueriesAsync<T>
	}


	public abstract class IDataAccessObject<T, KeyType, Ret> where T : class
	{
		public IDataAccessObject()
		{
			if (TableData<T>.KeyProperties.Length != 1) {
				string type = typeof(T).Name;
				throw new InvalidOperationException(type + " must have one KeyAttribute.");
			}
			Queries = TableData<T>.Queries;
			if(Queries == null) {
				TableQueries<T>.Data queryData = new TableQueries<T>.Factory().Create();
				throw new InvalidOperationException("Uncreachable");
			}
			QueriesKey = TableData<T, KeyType>.Queries;
			if (QueriesKey == null) {
				string keyType = TableData<T>.KeyProperties[0].PropertyType.Name;
				throw new InvalidOperationException("Invalid or unsupported Key type: " + keyType);
			}
			TableName = TableData<T>.TableName;
		}

		public readonly string TableName;
		protected readonly TableQueries<T, KeyType> QueriesKey;
		protected readonly TableQueries<T> Queries;

		#region IDataAccessObjectSync<T, KeyType, Ret>
		public abstract bool Delete(KeyType key, int? commandTimeout = null);
		public abstract bool Delete(IDictionary<string, object> key, int? commandTimeout = null);
		public abstract bool Delete(T obj, int? commandTimeout = null);
		public abstract int Delete(string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);
		public abstract IEnumerable<KeyType> DeleteList(string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);
		public abstract Ret Get(KeyType key, int? commandTimeout = null);
		public abstract Ret Get(IDictionary<string, object> key, int? commandTimeout = null);
		public abstract Ret Get(T obj, int? commandTimeout = null);
		public abstract IEnumerable<KeyType> GetKeys(string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);
		public abstract IEnumerable<Ret> GetList(string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);
		public abstract IEnumerable<Ret> GetTop(int limit, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);
		public abstract IEnumerable<Ret> GetDistinct(string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);
		public abstract Ret Insert(T obj, int? commandTimeout = null);
		public abstract bool Update(T obj, int? commandTimeout = null);
		public abstract Ret Upsert(T obj, int? commandTimeout = null);
		public abstract int RecordCount(string whereCondition = "", object param = null, int? commandTimeout = null);

		public abstract IEnumerable<Ret> BulkInsert(IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null);
		public abstract int BulkUpdate(IEnumerable<T> objs, int? commandTimeout = null);
		public abstract int BulkDelete(IEnumerable<KeyType> keys, int? commandTimeout = null);
		public abstract int BulkDelete(IEnumerable<T> objs, int? commandTimeout = null);
		public abstract int BulkUpsert(IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null);

		public abstract IEnumerable<Ret> BulkUpdateList(IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null);
		public abstract IEnumerable<KeyType> BulkDeleteList(IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null);
		public abstract IEnumerable<Ret> BulkUpsertList(IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null);
		#endregion IDataAccessObjectSync<T, KeyType, Ret>

		#region IDataAccessObjectAsync<T, KeyType, Ret>
		public async Task<bool> DeleteAsync(KeyType key, int? commandTimeout = null)
		{
			return await Task.Run(() => Delete(key, commandTimeout));
		}

		public async Task<int> BulkDeleteAsync(IEnumerable<KeyType> keys, int? commandTimeout = null)
		{
			return await Task.Run(() => BulkDelete(keys, commandTimeout));
		}

		public async Task<bool> DeleteAsync(IDictionary<string, object> key, int? commandTimeout = null)
		{
			return await Task.Run(() => Delete(key, commandTimeout));
		}

		public async Task<bool> DeleteAsync(T obj, int? commandTimeout = null)
		{
			return await Task.Run(() => Delete(obj, commandTimeout));
		}

		public async Task<int> BulkDeleteAsync(IEnumerable<T> objs, int? commandTimeout = null)
		{
			return await Task.Run(() => BulkDelete(objs, commandTimeout));
		}

		public async Task<int> DeleteAsync(string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			return await Task.Run(() => Delete(whereCondition, param, buffered, commandTimeout));
		}

		public async Task<IEnumerable<KeyType>> DeleteListAsync(string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			return await Task.Run(() => DeleteList(whereCondition, param, buffered, commandTimeout));
		}

		public async Task<Ret> GetAsync(KeyType key, int? commandTimeout = null)
		{
			return await Task.Run(() => Get(key, commandTimeout));
		}

		public async Task<Ret> GetAsync(IDictionary<string, object> key, int? commandTimeout = null)
		{
			return await Task.Run(() => Get(key, commandTimeout));
		}

		public async Task<Ret> GetAsync(T obj, int? commandTimeout = null)
		{
			return await Task.Run(() => Get(obj, commandTimeout));
		}

		public async Task<IEnumerable<KeyType>> GetKeysAsync(string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			return await Task.Run(() => GetKeys(whereCondition, param, buffered, commandTimeout));
		}

		public async Task<IEnumerable<Ret>> GetListAsync(string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			return await Task.Run(() => GetList(whereCondition, param, buffered, commandTimeout));
		}

		public async Task<IEnumerable<Ret>> GetTopAsync(int limit, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			return await Task.Run(() => GetTop(limit, whereCondition, param, buffered, commandTimeout));
		}

		public async Task<IEnumerable<Ret>> GetDistinctAsync(string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			return await Task.Run(() => GetDistinct(whereCondition, param, buffered, commandTimeout));
		}

		public async Task<Ret> InsertAsync(T obj, int? commandTimeout = null)
		{
			return await Task.Run(() => Insert(obj, commandTimeout));
		}

		public async Task<IEnumerable<Ret>> BulkInsertAsync(IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null)
		{
			return await Task.Run(() => BulkInsert(objs, buffered, commandTimeout));
		}

		public async Task<int> RecordCountAsync(string whereCondition = "", object param = null, int? commandTimeout = null)
		{
			return await Task.Run(() => RecordCount(whereCondition, param, commandTimeout));
		}

		public async Task<bool> UpdateAsync(T obj, int? commandTimeout = null)
		{
			return await Task.Run(() => Update(obj, commandTimeout));
		}

		public async Task<int> BulkUpdateAsync(IEnumerable<T> objs, int? commandTimeout = null)
		{
			return await Task.Run(() => BulkUpdate(objs, commandTimeout));
		}

		public async Task<IEnumerable<Ret>> BulkUpdateListAsync(IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null)
		{
			return await Task.Run(() => BulkUpdateList(objs, buffered, commandTimeout));
		}

		public async Task<Ret> UpsertAsync(T obj, int? commandTimeout = null)
		{
			return await Task.Run(() => Upsert(obj, commandTimeout));
		}

		public async Task<IEnumerable<Ret>> BulkUpsertListAsync(IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null)
		{
			return await Task.Run(() => BulkUpsertList(objs, buffered, commandTimeout));
		}

		public async Task<int> BulkUpsertAsync(IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null)
		{
			return await Task.Run(() => BulkUpsert(objs, buffered, commandTimeout));
		}

		public async Task<IEnumerable<KeyType>> BulkDeleteListAsync(IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null)
		{
			return await Task.Run(() => BulkDeleteList(objs, buffered, commandTimeout));
		}
		#endregion IDataAccessObjectAsync<T, KeyType, Ret>

		#region ITransactionQueriesSync<T, KeyType, Ret>
		public abstract bool Delete(IDbTransaction transaction, KeyType key, int? commandTimeout = null);
		public abstract bool Delete(IDbTransaction transaction, IDictionary<string, object> key, int? commandTimeout = null);
		public abstract bool Delete(IDbTransaction transaction, T obj, int? commandTimeout = null);
		public abstract int Delete(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);
		public abstract IEnumerable<KeyType> DeleteList(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);
		public abstract Ret Insert(IDbTransaction transaction, T obj, int? commandTimeout = null);
		public abstract bool Update(IDbTransaction transaction, T obj, int? commandTimeout = null);
		//public abstract int Update(IDbTransaction transaction, string whereCondition = "", object param = null, int? commandTimeout = null);
		public abstract Ret Upsert(IDbTransaction transaction, T obj, int? commandTimeout = null);
		public abstract IEnumerable<KeyType> GetKeys(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);
		public abstract Ret Get(IDbTransaction transaction, IDictionary<string, object> key, int? commandTimeout = null);
		public abstract Ret Get(IDbTransaction transaction, KeyType key, int? commandTimeout = null);
		public abstract Ret Get(IDbTransaction transaction, T obj, int? commandTimeout = null);
		public abstract IEnumerable<Ret> GetList(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);
		public abstract IEnumerable<Ret> GetTop(IDbTransaction transaction, int limit, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);
		public abstract IEnumerable<Ret> GetDistinct(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null);

		public abstract int RecordCount(IDbTransaction transaction, string whereCondition = "", object param = null, int? commandTimeout = null);

		public abstract IEnumerable<Ret> BulkInsert(SqlTransaction transaction, IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null);
		public abstract int BulkUpdate(SqlTransaction transaction, IEnumerable<T> objs, int? commandTimeout = null);
		public abstract int BulkDelete(SqlTransaction transaction, IEnumerable<T> objs, int? commandTimeout = null);
		public abstract int BulkDelete(SqlTransaction transaction, IEnumerable<KeyType> objs, int? commandTimeout = null);
		public abstract int BulkUpsert(SqlTransaction transaction, IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null);

		public abstract IEnumerable<KeyType> BulkDeleteList(SqlTransaction transaction, IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null);
		public abstract IEnumerable<Ret> BulkUpdateList(SqlTransaction transaction, IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null);
		public abstract IEnumerable<Ret> BulkUpsertList(SqlTransaction transaction, IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null);
		#endregion ITransactionQueriesSync<T, KeyType, Ret>

		#region ITransactionQueriesAsync<T, KeyType, Ret>
		public async Task<bool> DeleteAsync(IDbTransaction transaction, IDictionary<string, object> key, int? commandTimeout = null)
		{
			return await Task.Run(() => Delete(transaction, key, commandTimeout));
		}

		public async Task<bool> DeleteAsync(IDbTransaction transaction, T obj, int? commandTimeout = null)
		{
			return await Task.Run(() => Delete(transaction, obj, commandTimeout));
		}

		public async Task<int> DeleteAsync(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			return await Task.Run(() => Delete(transaction, whereCondition, param, buffered, commandTimeout));
		}

		public async Task<Ret> GetAsync(IDbTransaction transaction, IDictionary<string, object> key, int? commandTimeout = null)
		{
			return await Task.Run(() => Get(transaction, key, commandTimeout));
		}

		public async Task<Ret> GetAsync(IDbTransaction transaction, T obj, int? commandTimeout = null)
		{
			return await Task.Run(() => Get(transaction, obj, commandTimeout));
		}

		public async Task<IEnumerable<Ret>> GetListAsync(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			return await Task.Run(() => GetList(transaction, whereCondition, param, buffered, commandTimeout));
		}

		public async Task<IEnumerable<Ret>> GetTopAsync(IDbTransaction transaction, int limit, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			return await Task.Run(() => GetTop(transaction, limit, whereCondition, param, buffered, commandTimeout));
		}

		public async Task<IEnumerable<Ret>> GetDistinctAsync(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			return await Task.Run(() => GetDistinct(transaction, whereCondition, param, buffered, commandTimeout));
		}

		public async Task<Ret> InsertAsync(IDbTransaction transaction, T obj, int? commandTimeout = null)
		{
			return await Task.Run(() => Insert(transaction, obj, commandTimeout));
		}

		public async Task<int> RecordCountAsync(IDbTransaction transaction, string whereCondition = "", object param = null, int? commandTimeout = null)
		{
			return await Task.Run(() => RecordCount(transaction, whereCondition, param, commandTimeout));
		}

		public async Task<bool> UpdateAsync(IDbTransaction transaction, T obj, int? commandTimeout = null)
		{
			return await Task.Run(() => Update(transaction, obj, commandTimeout));
		}

		//public async Task<int> UpdateAsync(IDbTransaction transaction, string whereCondition = "", object param = null, int? commandTimeout = null)
		//{
		//	return await Task.Run(() => Update(transaction, whereCondition, param, commandTimeout));
		//}

		public async Task<Ret> UpsertAsync(IDbTransaction transaction, T obj, int? commandTimeout = null)
		{
			return await Task.Run(() => Upsert(transaction, obj, commandTimeout));
		}

		public async Task<IEnumerable<KeyType>> GetKeysAsync(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			return await Task.Run(() => GetKeys(transaction, whereCondition, param, buffered, commandTimeout));
		}

		public async Task<bool> DeleteAsync(IDbTransaction transaction, KeyType key, int? commandTimeout = null)
		{
			return await Task.Run(() => Delete(transaction, key, commandTimeout));
		}

		public async Task<Ret> GetAsync(IDbTransaction transaction, KeyType key, int? commandTimeout = null)
		{
			return await Task.Run(() => Get(transaction, key, commandTimeout));
		}

		public async Task<IEnumerable<KeyType>> DeleteListAsync(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			return await Task.Run(() => DeleteList(transaction, whereCondition, param, buffered, commandTimeout));
		}

		public async Task<int> BulkDeleteAsync(SqlTransaction transaction, IEnumerable<T> objs, int? commandTimeout = null)
		{
			return await Task.Run(() => BulkDelete(transaction, objs, commandTimeout));
		}

		public async Task<IEnumerable<Ret>> BulkInsertAsync(SqlTransaction transaction, IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null)
		{
			return await Task.Run(() => BulkInsert(transaction, objs, buffered, commandTimeout));
		}

		public async Task<int> BulkUpdateAsync(SqlTransaction transaction, IEnumerable<T> objs, int? commandTimeout = null)
		{
			return await Task.Run(() => BulkUpdate(transaction, objs, commandTimeout));
		}

		public async Task<int> BulkUpsertAsync(SqlTransaction transaction, IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null)
		{
			return await Task.Run(() => BulkUpsert(transaction, objs, buffered, commandTimeout));
		}

		public async Task<int> BulkDeleteAsync(SqlTransaction transaction, IEnumerable<KeyType> keys, int? commandTimeout = null)
		{
			return await Task.Run(() => BulkDelete(transaction, keys, commandTimeout));
		}

		public async Task<IEnumerable<KeyType>> BulkDeleteListAsync(SqlTransaction transaction, IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null)
		{
			return await Task.Run(() => BulkDeleteList(transaction, objs, buffered, commandTimeout));
		}

		public async Task<IEnumerable<Ret>> BulkUpdateListAsync(SqlTransaction transaction, IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null)
		{
			return await Task.Run(() => BulkUpdateList(transaction, objs, buffered, commandTimeout));
		}

		public async Task<IEnumerable<Ret>> BulkUpsertListAsync(SqlTransaction transaction, IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null)
		{
			return await Task.Run(() => BulkUpsertList(transaction, objs, buffered, commandTimeout));
		}
		#endregion ITransactionQueriesAsync<T, KeyType Ret>
	}
}
