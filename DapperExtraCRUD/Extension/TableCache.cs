using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Dapper.Extension.Interfaces;

namespace Dapper.Extension
{
	public class TableCache<T, KeyType, Ret> : IDataAccessObject<T, KeyType, Ret>, IEnumerable<Ret>
		where T : class
		where Ret : class
	{
		public TableCache(DataAccessObject<T, KeyType> dao, Func<T, Ret> constructor, Func<T, Ret, Ret> update)
		{
			Dictionary<KeyType, Ret> map = new Dictionary<KeyType, Ret>();
			Map = map;
			DAO = dao;
			Items = map;
			UpdateRet = update;
			Constructor = constructor;
		}

		protected readonly IDictionary<KeyType, Ret> Map;
		protected readonly Func<T, Ret, Ret> UpdateRet;
		protected readonly Func<T, Ret> Constructor;

		public readonly IReadOnlyDictionary<KeyType, Ret> Items;
		public readonly DataAccessObject<T, KeyType> DAO;

		public IEnumerator<Ret> GetEnumerator()
		{
			return Map.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return Map.Values.GetEnumerator();
		}

		protected Ret UpsertItem(T obj)
		{
			KeyType key = TableData<T, KeyType>.GetKey(obj);
			Ret output = Map.TryGetValue(key, out Ret value) ? UpdateRet(obj, value) : Constructor(obj);
			Map[key] = output;
			return output;
		}

		protected List<Ret> UpsertItems(IEnumerable<T> objs)
		{
			List<Ret> list = new List<Ret>();
			foreach (T obj in objs) {
				Ret item = UpsertItem(obj);
				list.Add(item);
			}
			return list;
		}

		public Ret Find(KeyType key, int? commandTimeout = null)
		{
			return Map.TryGetValue(key, out Ret value) ? value : Get(key, commandTimeout);
		}

		public Ret Find(T obj, int? commandTimeout = null)
		{
			KeyType key = TableData<T, KeyType>.GetKey(obj);
			return Find(key, commandTimeout);
		}

		#region IDataAccessObjectSync<T, KeyType, Ret>
		public override IEnumerable<KeyType> GetKeys(string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			return DAO.GetKeys(whereCondition, param, buffered, commandTimeout);
		}

		public override bool Delete(KeyType key, int? commandTimeout = null)
		{
			bool deleted = DAO.Delete(key, commandTimeout);
			Map.Remove(key);
			return deleted;
		}

		public override int BulkDelete(IEnumerable<KeyType> keys, int? commandTimeout = null)
		{
			int deleted = DAO.BulkDelete(keys, commandTimeout);
			foreach (KeyType key in keys) {
				Map.Remove(key);
			}
			return deleted;
		}

		public override IEnumerable<KeyType> DeleteList(string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			IEnumerable<KeyType> keys = DAO.DeleteList(whereCondition, param, buffered, commandTimeout);
			foreach (KeyType key in keys) {
				Map.Remove(key);
			}
			return keys;
		}

		public override Ret Get(KeyType key, int? commandTimeout = null)
		{
			T obj = DAO.Get(key, commandTimeout);
			return obj == null ? null : UpsertItem(obj);
		}

		public override bool Delete(IDictionary<string, object> key, int? commandTimeout = null)
		{
			KeyType keyVal = TableData<T, KeyType>.GetKey(key);
			return Delete(keyVal, commandTimeout);
		}

		public override bool Delete(T obj, int? commandTimeout = null)
		{
			KeyType keyVal = TableData<T, KeyType>.GetKey(obj);
			return Delete(keyVal, commandTimeout);
		}

		public override int BulkDelete(IEnumerable<T> objs, int? commandTimeout = null)
		{
			return BulkDelete(objs.Select(obj => TableData<T, KeyType>.GetKey(obj)), commandTimeout);
		}

		public override int Delete(string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			return DeleteList(whereCondition, param, buffered, commandTimeout).Count();
		}

		public override Ret Insert(T obj, int? commandTimeout = null)
		{
			T item = DAO.Insert(obj, commandTimeout);
			return UpsertItem(item);
		}

		public override IEnumerable<Ret> BulkInsert(IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null)
		{
			IEnumerable<T> list = DAO.BulkInsert(objs, buffered, commandTimeout);
			return UpsertItems(list);
		}

		public override bool Update(T obj, int? commandTimeout = null)
		{
			if(DAO.Update(obj, commandTimeout)) {
				UpsertItem(obj);
				return true;
			}
			return false;
		}

		public override int BulkUpdate(IEnumerable<T> objs, int? commandTimeout = null)
		{
			IEnumerable<T> list = DAO.BulkUpdateList(objs, true, commandTimeout);
			return UpsertItems(list).Count;
		}

		public override Ret Upsert(T obj, int? commandTimeout = null)
		{
			obj = DAO.Upsert(obj, commandTimeout);
			return obj == null ? null : UpsertItem(obj);
		}

		public override int BulkUpsert(IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null)
		{
			IEnumerable<T> list = DAO.BulkUpsertList(objs, buffered, commandTimeout);
			return UpsertItems(list).Count;
		}

		public override Ret Get(IDictionary<string, object> key, int? commandTimeout = null)
		{
			KeyType keyVal = TableData<T, KeyType>.GetKey(key);
			return Get(keyVal, commandTimeout);
		}

		public override Ret Get(T obj, int? commandTimeout = null)
		{
			KeyType keyVal = TableData<T, KeyType>.GetKey(obj);
			return Get(keyVal, commandTimeout);
		}

		public override IEnumerable<Ret> GetList(string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			IEnumerable<T> list = DAO.GetList(whereCondition, param, buffered, commandTimeout);
			return UpsertItems(list);
		}

		public override IEnumerable<Ret> GetTop(int limit, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			IEnumerable<T> list = DAO.GetTop(limit, whereCondition, param, buffered, commandTimeout);
			return UpsertItems(list);
		}

		public override IEnumerable<Ret> GetDistinct(string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			IEnumerable<T> list = DAO.GetDistinct(whereCondition, param, buffered, commandTimeout);
			return UpsertItems(list);
		}

		public override int RecordCount(string whereCondition = "", object param = null, int? commandTimeout = null)
		{
			return DAO.RecordCount(whereCondition, param, commandTimeout);
		}

		public override IEnumerable<Ret> BulkUpdateList(IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null)
		{
			IEnumerable<T> list = DAO.BulkUpdateList(objs, buffered, commandTimeout);
			return UpsertItems(list);
		}

		public override IEnumerable<Ret> BulkUpsertList(IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null)
		{
			IEnumerable<T> list = DAO.BulkUpsertList(objs, buffered, commandTimeout);
			return UpsertItems(list);
		}
		#endregion IDataAccessObjectSync<T, KeyType, Ret>


		#region ITransactionQueriesSync<T, KeyType, Ret>
		public override IEnumerable<KeyType> GetKeys(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			return DAO.GetKeys(transaction, whereCondition, param, buffered, commandTimeout);
		}

		public override bool Delete(IDbTransaction transaction, KeyType key, int? commandTimeout = null)
		{
			bool deleted = DAO.Delete(transaction, key, commandTimeout);
			Map.Remove(key);
			return deleted;
		}

		public override int BulkDelete(SqlTransaction transaction, IEnumerable<KeyType> keys, int? commandTimeout = null)
		{
			int deleted = DAO.BulkDelete(transaction, keys, commandTimeout);
			foreach (KeyType key in keys) {
				Map.Remove(key);
			}
			return deleted;
		}

		public override Ret Get(IDbTransaction transaction, KeyType key, int? commandTimeout = null)
		{
			return UpsertItem(DAO.Get(transaction, key, commandTimeout));
		}

		public override IEnumerable<KeyType> DeleteList(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			IEnumerable<KeyType> keys = DAO.DeleteList(transaction, whereCondition, param, buffered, commandTimeout);
			foreach (KeyType key in keys) {
				Map.Remove(key);
			}
			return keys;
		}

		public override bool Delete(IDbTransaction transaction, IDictionary<string, object> key, int? commandTimeout = null)
		{
			KeyType keyVal = TableData<T, KeyType>.GetKey(key);
			return Delete(transaction, keyVal, commandTimeout);
		}

		public override bool Delete(IDbTransaction transaction, T obj, int? commandTimeout = null)
		{
			KeyType key = TableData<T, KeyType>.GetKey(obj);
			return Delete(transaction, key, commandTimeout);
		}

		public override int BulkDelete(SqlTransaction transaction, IEnumerable<T> objs, int? commandTimeout = null)
		{
			return BulkDelete(transaction, objs.Select(obj => TableData<T, KeyType>.GetKey(obj)), commandTimeout);
		}

		public override int Delete(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			return DeleteList(transaction, whereCondition, param, buffered, commandTimeout).Count();
		}

		public override Ret Insert(IDbTransaction transaction, T obj, int? commandTimeout = null)
		{
			return UpsertItem(DAO.Insert(transaction, obj, commandTimeout));
		}

		public override IEnumerable<Ret> BulkInsert(SqlTransaction transaction, IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null)
		{
			return UpsertItems(DAO.BulkInsert(transaction, objs, buffered, commandTimeout));
		}

		public override bool Update(IDbTransaction transaction, T obj, int? commandTimeout = null)
		{
			if(DAO.Update(transaction, obj, commandTimeout)) {
				UpsertItem(obj);
				return true;
			}
			return false;
		}

		public override int BulkUpdate(SqlTransaction transaction, IEnumerable<T> objs, int? commandTimeout = null)
		{
			IEnumerable<T> list = DAO.BulkUpdateList(transaction, objs, true, commandTimeout);
			return UpsertItems(list).Count;
		}

		public override Ret Upsert(IDbTransaction transaction, T obj, int? commandTimeout = null)
		{
			return UpsertItem(DAO.Upsert(transaction, obj, commandTimeout));
		}

		public override int BulkUpsert(SqlTransaction transaction, IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null)
		{
			IEnumerable<T> list = DAO.BulkUpsertList(transaction, objs, buffered, commandTimeout);
			return UpsertItems(list).Count;
		}

		public override Ret Get(IDbTransaction transaction, IDictionary<string, object> key, int? commandTimeout = null)
		{
			return UpsertItem(DAO.Get(transaction, key, commandTimeout));
		}

		public override Ret Get(IDbTransaction transaction, T obj, int? commandTimeout = null)
		{
			return UpsertItem(DAO.Get(transaction, obj, commandTimeout));
		}

		public override IEnumerable<Ret> GetList(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			return UpsertItems(DAO.GetList(transaction, whereCondition, param, buffered, commandTimeout));
		}

		public override IEnumerable<Ret> GetTop(IDbTransaction transaction, int limit, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			IEnumerable<T> list = DAO.GetTop(transaction, limit, whereCondition, param, buffered, commandTimeout);
			return UpsertItems(list);
		}

		public override IEnumerable<Ret> GetDistinct(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			IEnumerable<T> list = DAO.GetDistinct(transaction, whereCondition, param, buffered, commandTimeout);
			return UpsertItems(list);
		}

		public override int RecordCount(IDbTransaction transaction, string whereCondition = "", object param = null, int? commandTimeout = null)
		{
			return DAO.RecordCount(transaction, whereCondition, param, commandTimeout);
		}

		public override IEnumerable<KeyType> BulkDeleteList(IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null)
		{
			IEnumerable<KeyType> keys = DAO.BulkDeleteList(objs, buffered, commandTimeout);
			foreach (KeyType key in keys) {
				Map.Remove(key);
			}
			return keys;
		}

		public override IEnumerable<KeyType> BulkDeleteList(SqlTransaction transaction, IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null)
		{
			IEnumerable<KeyType> keys = DAO.BulkDeleteList(transaction, objs, buffered, commandTimeout);
			foreach (KeyType key in keys) {
				Map.Remove(key);
			}
			return keys;
		}

		public override IEnumerable<Ret> BulkUpdateList(SqlTransaction transaction, IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null)
		{
			IEnumerable<T> list = DAO.BulkUpdateList(transaction, objs, buffered, commandTimeout);
			return UpsertItems(list);
		}

		public override IEnumerable<Ret> BulkUpsertList(SqlTransaction transaction, IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null)
		{
			IEnumerable<T> list = DAO.BulkUpsertList(transaction, objs, buffered, commandTimeout);
			return UpsertItems(list);
		}
		#endregion ITransactionQueriesSync<T, KeyType, Ret>
	}



	public class TableCache<T, Ret> : IDataAccessObject<T, Ret>, IEnumerable<Ret>
		where T : class
		where Ret : class
	{
		public TableCache(DataAccessObject<T> dao, Func<T, Ret> constructor, Func<T, Ret, Ret> update)
		{
			if (TableData<T>.KeyProperties.Length == 0) {
				throw new InvalidOperationException("Cannot cache objects without a KeyAttribute.");
			}
			Dictionary<T, Ret> map = new Dictionary<T, Ret>(TableEqualityComparer<T>.Default);
			Map = map;
			DAO = dao;
			Items = map;
			UpdateRet = update;
			Constructor = constructor;
		}

		protected readonly IDictionary<T, Ret> Map;
		protected readonly Func<T, Ret, Ret> UpdateRet;
		protected readonly Func<T, Ret> Constructor;

		public readonly DataAccessObject<T> DAO;
		public readonly IReadOnlyDictionary<T, Ret> Items;

		public IEnumerator<Ret> GetEnumerator()
		{
			return Map.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return Map.Values.GetEnumerator();
		}

		public SqlConnection Connection()
		{
			return DAO.Connection();
		}

		protected Ret UpsertItem(T obj)
		{
			Ret output = Map.TryGetValue(obj, out Ret value) ? UpdateRet(obj, value) : Constructor(obj);
			Map[obj] = output;
			return output;
		}

		protected List<Ret> UpsertItems(IEnumerable<T> objs)
		{
			List<Ret> list = new List<Ret>();
			foreach (T obj in objs) {
				Ret val = UpsertItem(obj);
				list.Add(val);
			}
			return list;
		}

		#region IDataAccessObjectSync<T, T, Ret>
		public override IEnumerable<T> GetKeys(string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			return DAO.GetKeys(whereCondition, param, buffered, commandTimeout);
		}

		public override IEnumerable<T> DeleteList(string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			IEnumerable<T> objs = DAO.DeleteList(whereCondition, param, buffered, commandTimeout);
			foreach (T obj in objs) {
				Map.Remove(obj);
			}
			return objs;
		}

		public override bool Delete(IDictionary<string, object> key, int? commandTimeout = null)
		{
			T keyVal = TableData<T>.CreateObject(key);
			return Delete(keyVal, commandTimeout);
		}

		public override bool Delete(T obj, int? commandTimeout = null)
		{
			bool deleted = DAO.Delete(obj, commandTimeout);
			Map.Remove(obj);
			return deleted;
		}

		public override int BulkDelete(IEnumerable<T> objs, int? commandTimeout = null)
		{
			int deleted = DAO.BulkDelete(objs, commandTimeout);
			foreach (T obj in objs) {
				Map.Remove(obj);
			}
			return deleted;
		}

		public override int Delete(string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			return DeleteList(whereCondition, param, buffered, commandTimeout).Count();
		}

		public override Ret Insert(T obj, int? commandTimeout = null)
		{
			T item = DAO.Insert(obj, commandTimeout);
			return UpsertItem(item);
		}

		public override IEnumerable<Ret> BulkInsert(IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null)
		{
			IEnumerable<T> list = DAO.BulkInsert(objs, buffered, commandTimeout);
			return UpsertItems(list);
		}

		public override bool Update(T obj, int? commandTimeout = null)
		{
			if(DAO.Update(obj, commandTimeout)) {
				UpsertItem(obj);
				return true;
			}
			return false;
		}

		public override int BulkUpdate(IEnumerable<T> objs, int? commandTimeout = null)
		{
			IEnumerable<T> list = DAO.BulkUpdateList(objs, true, commandTimeout);
			return UpsertItems(list).Count;
		}

		public override Ret Upsert(T obj, int? commandTimeout = null)
		{
			T item = DAO.Upsert(obj, commandTimeout);
			return item == null ? null : UpsertItem(DAO.Upsert(obj, commandTimeout));
		}

		public override int BulkUpsert(IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null)
		{
			IEnumerable<T> list = DAO.BulkUpsertList(objs, buffered, commandTimeout);
			return UpsertItems(list).Count;
		}

		public override Ret Get(IDictionary<string, object> key, int? commandTimeout = null)
		{
			T val = DAO.Get(key, commandTimeout);
			return UpsertItem(val);
		}

		public override Ret Get(T obj, int? commandTimeout = null)
		{
			T item = DAO.Get(obj, commandTimeout);
			return item == null ? null : UpsertItem(item);
		}

		public override IEnumerable<Ret> GetList(string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			IEnumerable<T> list = DAO.GetList(whereCondition, param, buffered, commandTimeout);
			return UpsertItems(list);
		}

		public override IEnumerable<Ret> GetTop(int limit, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			IEnumerable<T> list = DAO.GetTop(limit, whereCondition, param, buffered, commandTimeout);
			return UpsertItems(list);
		}

		public override IEnumerable<Ret> GetDistinct(string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			IEnumerable<T> list = DAO.GetDistinct(whereCondition, param, buffered, commandTimeout);
			return UpsertItems(list);
		}

		public override int RecordCount(string whereCondition = "", object param = null, int? commandTimeout = null)
		{
			return DAO.RecordCount(whereCondition, param, commandTimeout);
		}

		public override IEnumerable<Ret> BulkUpdateList(IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null)
		{
			IEnumerable<T> list = DAO.BulkUpdateList(objs, buffered, commandTimeout);
			return UpsertItems(list);
		}

		public override IEnumerable<Ret> BulkUpsertList(IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null)
		{
			IEnumerable<T> list = DAO.BulkUpsertList(objs, buffered, commandTimeout);
			return UpsertItems(list);
		}
		#endregion IDataAccessObjectSync<T, T, Ret>


		#region ITransactionQueriesSync<T, T, Ret>
		public override IEnumerable<T> GetKeys(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			return DAO.GetKeys(transaction, whereCondition, param, buffered, commandTimeout);
		}

		public override IEnumerable<T> DeleteList(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			IEnumerable<T> objs = DAO.DeleteList(transaction, whereCondition, param, buffered, commandTimeout);
			foreach (T obj in objs) {
				Map.Remove(obj);
			}
			return objs;
		}

		public override bool Delete(IDbTransaction transaction, IDictionary<string, object> key, int? commandTimeout = null)
		{
			T obj = TableData<T>.CreateObject(key);
			return Delete(transaction, obj, commandTimeout);
		}

		public override bool Delete(IDbTransaction transaction, T obj, int? commandTimeout = null)
		{
			bool deleted = DAO.Delete(transaction, obj, commandTimeout);
			Map.Remove(obj);
			return deleted;
		}

		public override int BulkDelete(SqlTransaction transaction, IEnumerable<T> objs, int? commandTimeout = null)
		{
			int deleted = DAO.BulkDelete(transaction, objs, commandTimeout);
			foreach (T obj in objs) {
				Map.Remove(obj);
			}
			return deleted;
		}

		public override int Delete(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			return DeleteList(transaction, whereCondition, param, buffered, commandTimeout).Count();
		}

		public override Ret Insert(IDbTransaction transaction, T obj, int? commandTimeout = null)
		{
			T val = DAO.Insert(transaction, obj, commandTimeout);
			return UpsertItem(val);
		}

		public override IEnumerable<Ret> BulkInsert(SqlTransaction transaction, IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null)
		{
			IEnumerable<T> list = DAO.BulkInsert(transaction, objs, buffered, commandTimeout);
			return UpsertItems(list);
		}

		public override bool Update(IDbTransaction transaction, T obj, int? commandTimeout = null)
		{
			if(DAO.Update(transaction, obj, commandTimeout)) {
				UpsertItem(obj);
				return true;
			}
			return false;
		}

		public override int BulkUpdate(SqlTransaction transaction, IEnumerable<T> objs, int? commandTimeout = null)
		{
			IEnumerable<T> list = DAO.BulkUpdateList(transaction, objs, true, commandTimeout);
			return UpsertItems(list).Count;
		}

		public override Ret Upsert(IDbTransaction transaction, T obj, int? commandTimeout = null)
		{
			T val = DAO.Upsert(transaction, obj, commandTimeout);
			return UpsertItem(val);
		}

		public override int BulkUpsert(SqlTransaction transaction, IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null)
		{
			IEnumerable<T> list = DAO.BulkUpsertList(transaction, objs, buffered, commandTimeout);
			return UpsertItems(list).Count;
		}

		public override Ret Get(IDbTransaction transaction, IDictionary<string, object> key, int? commandTimeout = null)
		{
			T val = DAO.Get(transaction, key, commandTimeout);
			return val == null ? null : UpsertItem(val);
		}

		public override Ret Get(IDbTransaction transaction, T obj, int? commandTimeout = null)
		{
			T val = DAO.Get(transaction, obj, commandTimeout);
			return val == null ? null : UpsertItem(val);
		}

		public override IEnumerable<Ret> GetList(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			IEnumerable<T> list = DAO.GetList(transaction, whereCondition, param, buffered, commandTimeout);
			return UpsertItems(list);
		}

		public override IEnumerable<Ret> GetTop(IDbTransaction transaction, int limit, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			IEnumerable<T> list = DAO.GetTop(transaction, limit, whereCondition, param, buffered, commandTimeout);
			return UpsertItems(list);
		}

		public override IEnumerable<Ret> GetDistinct(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int? commandTimeout = null)
		{
			IEnumerable<T> list = DAO.GetDistinct(transaction, whereCondition, param, buffered, commandTimeout);
			return UpsertItems(list);
		}

		public override int RecordCount(IDbTransaction transaction, string whereCondition = "", object param = null, int? commandTimeout = null)
		{
			return DAO.RecordCount(transaction, whereCondition, param, commandTimeout);
		}

		public override IEnumerable<T> BulkDeleteList(IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null)
		{
			IEnumerable<T> keys = DAO.BulkDeleteList(objs, buffered, commandTimeout);
			foreach (T key in keys) {
				Map.Remove(key);
			}
			return keys;
		}

		public override IEnumerable<T> BulkDeleteList(SqlTransaction transaction, IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null)
		{
			IEnumerable<T> keys = DAO.BulkDeleteList(transaction, objs, buffered, commandTimeout);
			foreach (T key in keys) {
				Map.Remove(key);
			}
			return keys;
		}

		public override IEnumerable<Ret> BulkUpdateList(SqlTransaction transaction, IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null)
		{
			IEnumerable<T> list = DAO.BulkUpdateList(transaction, objs, buffered, commandTimeout);
			return UpsertItems(list);
		}

		public override IEnumerable<Ret> BulkUpsertList(SqlTransaction transaction, IEnumerable<T> objs, bool buffered = true, int? commandTimeout = null)
		{
			IEnumerable<T> list = DAO.BulkUpsertList(transaction, objs, buffered, commandTimeout);
			return UpsertItems(list);
		}
		#endregion ITransactionQueriesSync<T, T, Ret>
	}
}