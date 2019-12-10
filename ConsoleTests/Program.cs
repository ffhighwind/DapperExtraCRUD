#region License
// Released under MIT License 
// License: https://www.mit.edu/~amini/LICENSE.md
// Home page: https://github.com/ffhighwind/DapperExtraCRUD

// Copyright(c) 2018 Wesley Hamilton

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
#endregion

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using Dapper;
using Dapper.Extra;
using Dapper.Extra.Utilities;
using Dapper.Extra.Cache;

namespace UnitTests
{
	public class Program
	{
		private const string ConnString = @"Data Source=DESKTOP-V0JVTST\SQLEXPRESS;Initial Catalog=Test;Integrated Security=True;";

		public static void Main()
		{
			Random random = new Random(123512);
			using (SqlConnection conn = new SqlConnection(ConnString)) {
				conn.Open();
				Recreate<TestDTO>(conn, null);
				Recreate<TestDTO2>(conn, null);
				Recreate<Test3>(conn, null);
				Recreate<TestDTO4>(conn, null);
				/*
				DoTests<TestDTO>(() => new TestDTO(random), (t) => t.UpdateRandomize(random), new TestDTOfilter());
				DoTests<TestDTO2>(() => new TestDTO2(random), (t) => t.UpdateRandomize(random), new TestDTO2filter());
				DoTests<Test3>(() => new Test3(random), (t) => t.UpdateRandomize(random), new Test3filter());
				DoTests<TestDTO4>(() => new TestDTO4(random), (t) => t.UpdateRandomize(random), new TestDTO4filter());

				DoTests<TestDTO, int>(conn);
				DoTests<TestDTO4, int>(conn);
				*/

				DoCacheTests<TestDTO>(() => new TestDTO(random));
				DoCacheTests<TestDTO2>(() => new TestDTO2(random));
				DoCacheTests<Test3>(() => new Test3(random));
				DoCacheTests<TestDTO4>(() => new TestDTO4(random));

				DropTable<TestDTO>(conn);
				DropTable<TestDTO2>(conn);
				DropTable<Test3>(conn);
				DropTable<TestDTO4>(conn);
			}
		}

		public static List<T> CreateList<T>(int count, Func<T> create) where T : class, IDto<T>
		{
			Dictionary<T, T> map = new Dictionary<T, T>(create());
			for (int i = 0; i < count; i++) {
				T created;
				int fails = 0;
				while (true) {
					created = create();
					if (!map.ContainsKey(created))
						break;
					fails++;
					if (fails > 3)
						break; // assumes only autokeys
				}
				map.Add(created, created);
			}
			List<T> list = map.Values.ToList();
			list.Sort((x, y) => x.CompareTo(y));
			return list;
		}

		public static List<T> CloneList<T>(List<T> list) where T : class, IDto<T>
		{
			List<T> list2 = new List<T>();
			for (int i = 0; i < list.Count; i++) {
				list2.Add(list[i].Clone());
			}
			return list2;
		}

		public static void DoCacheTests<T>(Func<T> constructor) where T : class, IDto<T>, new()
		{
			DbCache cache = new DbCache(ConnString);
			DbCacheTable<T, CacheItem<T>> table = cache.CreateTable<T>();
			Random random = new Random(512851);
			List<T> list = CreateList<T>(50, () => constructor());

			CacheInsert(table, list);
			CacheDelete(table, list);
		}

		public static void CacheInsert<T>(DbCacheTable<T, CacheItem<T>> table, List<T> list) where T : class, IDto<T>, new()
		{
			list = CloneList(list);
			for (int i = 0; i < list.Count; i++) {
				table.Insert(list[i]);
				if (!table.Access.Get(list[i]).IsInserted(list[i])) {
					throw new InvalidOperationException();
				}
				using (DbCacheTransaction trans = table.BeginTransaction()) {
					for (int j = i + 1; j < i + 5 && j < list.Count; j++) {
						table.Insert(list[j]);
						if (table.Items.Count != table.RecordCount()) {
							throw new InvalidOperationException();
						}
					}
				}
				if (table.Items.Count != table.RecordCount()) {
					throw new InvalidOperationException();
				}
			}
			table.DeleteList();
		}

		public static void CacheDelete<T>(DbCacheTable<T, CacheItem<T>> table, List<T> list) where T : class, IDto<T>, new()
		{
			list = CloneList(list);
			for (int i = 0; i < list.Count; i++) {
				table.Insert(list[i]);
			}
			if (table.Items.Count != table.RecordCount()) {
				throw new InvalidOperationException();
			}
			for (int i = 0; i < list.Count; i++) {
				table.Delete(list[i]);
				using (DbCacheTransaction trans = table.BeginTransaction()) {
					for (int j = i + 1; j < i + 5 && j < list.Count; j++) {
						table.Delete(list[j]);
						if (table.Items.Count != table.RecordCount()) {
							throw new InvalidOperationException();
						}
						foreach(var item in table.Access.GetList()) {
							if(!table.Items.Contains(item)) {
								throw new InvalidOperationException();
							}
						}
					}
				}
				if (table.Items.Count != table.RecordCount()) {
					throw new InvalidOperationException();
				}
			}
			table.DeleteList();
		}

		public static void DoTests<T>(Func<T> constructor, Func<T, T> randomize, IFilter<T> filter) where T : class, IDto<T>, new()
		{
			Random random = new Random(512851);
			using (SqlConnection conn = new SqlConnection(ConnString)) {
				conn.Open();
				using (SqlTransaction trans = conn.BeginTransaction()) {
					Recreate<T>(conn, trans);
					Insert<T>(conn, trans, random.Next() % 50 + 25, constructor);
					trans.Commit();
				}
				List<T> list = conn.GetList<T>().ToList();
				list.Sort((x, y) => x.CompareTo(y));
				using (SqlTransaction trans = conn.BeginTransaction()) {
					RecordCount<T>(conn, trans, list);
				}
				using (SqlTransaction trans = conn.BeginTransaction()) {
					GetList(conn, trans, list);
				}
				using (SqlTransaction trans = conn.BeginTransaction()) {
					InsertGet(conn, trans, constructor);
				}
				using (SqlTransaction trans = conn.BeginTransaction()) {
					Delete(conn, trans, list);
				}
				using (SqlTransaction trans = conn.BeginTransaction()) {
					GetKeys(conn, trans, list);
				}
				using (SqlTransaction trans = conn.BeginTransaction()) {
					DeleteList(conn, trans, list);
				}
				using (SqlTransaction trans = conn.BeginTransaction()) {
					GetLimit(conn, trans, list);
				}
				using (SqlTransaction trans = conn.BeginTransaction()) {
					DeleteAll<T>(conn, trans);
				}
				using (SqlTransaction trans = conn.BeginTransaction()) {
					Update<T>(conn, trans, list, randomize);
				}
				using (SqlTransaction trans = conn.BeginTransaction()) {
					Upsert<T>(conn, trans);
				}
				using (SqlTransaction trans = conn.BeginTransaction()) {
					GetDistinct(conn, trans, list);
				}
				using (SqlTransaction trans = conn.BeginTransaction()) {
					GetDistinctLimit(conn, trans, list);
				}
				//
				// Bulk
				//
				using (SqlTransaction trans = conn.BeginTransaction()) {
					BulkGet(conn, trans, list);
				}
				using (SqlTransaction trans = conn.BeginTransaction()) {
					BulkInsert(conn, trans, list);
				}
				using (SqlTransaction trans = conn.BeginTransaction()) {
					BulkUpdate(conn, trans, list, (t) => randomize(t));
				}
				using (SqlTransaction trans = conn.BeginTransaction()) {
					BulkDelete<T>(conn, trans);
				}
				using (SqlTransaction trans = conn.BeginTransaction()) {
					BulkUpsert<T>(conn, trans);
				}
				using (SqlTransaction trans = conn.BeginTransaction()) {
					InsertIfNotExists<T>(conn, trans);
				}
				using (SqlTransaction trans = conn.BeginTransaction()) {
					BulkInsertIfNotExists<T>(conn, trans, list);
				}
				//
				// Filters
				//
				using (SqlTransaction trans = conn.BeginTransaction()) {
					GetFilter(conn, trans, list, filter);
				}
				using (SqlTransaction trans = conn.BeginTransaction()) {
					GetFilterLimit(conn, trans, list, filter);
				}
				using (SqlTransaction trans = conn.BeginTransaction()) {
					GetFilterDistinctLimit(conn, trans, list, filter);
				}
			}
		}

		public static void DoTests<T, KeyType>(SqlConnection conn) where T : class, IDtoKey<T, KeyType>
		{
			List<T> list = conn.GetList<T>().AsList();
			list.Sort((x, y) => x.CompareTo(y));
			using (SqlTransaction trans = conn.BeginTransaction()) {
				Get_Key<T, KeyType>(conn, trans, list);
			}
			using (SqlTransaction trans = conn.BeginTransaction()) {
				GetKeys_Key<T, KeyType>(conn, trans, list);
			}
			using (SqlTransaction trans = conn.BeginTransaction()) {
				Delete_Key<T, KeyType>(conn, trans, list);
			}
			using (SqlTransaction trans = conn.BeginTransaction()) {
				BulkGet_Key<T, KeyType>(conn, trans, list);
			}
			using (SqlTransaction trans = conn.BeginTransaction()) {
				BulkDelete_Key<T, KeyType>(conn, trans);
			}
		}

		private static Dictionary<T, T> CreateMap<T>(List<T> list) where T : class, IDto<T>
		{
			Dictionary<T, T> map = new Dictionary<T, T>(list[0]);
			foreach (T item in list) {
				map.Add(item, item);
			}
			return map;
		}

		private static Dictionary<KeyType, T> CreateMap<T, KeyType>(List<T> list) where T : class, IDtoKey<T, KeyType>
		{
			Dictionary<KeyType, T> map = new Dictionary<KeyType, T>();
			foreach (T item in list) {
				map.Add(item.GetKey(), item);
			}
			return map;
		}

		private static void DropTable<T>(SqlConnection conn, SqlTransaction trans = null) where T : class
		{
			string tableName = ExtraCrud.TypeInfo<T>().TableName;
			string str = $@"
IF EXISTS (
	SELECT * from INFORMATION_SCHEMA.TABLES 
WHERE TABLE_NAME = '{tableName}' 
	AND TABLE_SCHEMA = 'dbo'
) 
DROP TABLE dbo.{tableName};";
			conn.Execute(str, null, trans);
		}

		private static void CreateTable<T>(SqlConnection conn, SqlTransaction trans = null) where T : class, IDto<T>, new()
		{
			conn.Execute(new T().CreateTable(), null, trans);
		}

		private static void Recreate<T>(SqlConnection conn, SqlTransaction trans) where T : class, IDto<T>, new()
		{
			DropTable<T>(conn, trans);
			CreateTable<T>(conn, trans);
		}

		#region Tested
		private static void Insert<T>(SqlConnection conn, SqlTransaction trans, int count, Func<T> constructor) where T : class, IDto<T>
		{
			if (count <= 0)
				count = 1;
			List<T> list = CreateList<T>(count, constructor);
			for (int i = 0; i < list.Count; i++) {
				conn.Insert<T>(list[i], trans);
			}
		}

		public static void RecordCount<T>(SqlConnection conn, SqlTransaction trans, List<T> list) where T : class, IDto<T>
		{
			int count = conn.RecordCount<T>(trans);
			if (list.Count != count)
				throw new InvalidOperationException();
		}

		public static void WhereConditionGen<T>(SqlConnection conn, SqlTransaction trans, List<T> list, Expression<Predicate<T>> predicateExpr) where T : class, IDto<T>
		{
			string condition = WhereConditionGenerator<T>.Create(predicateExpr, out IDictionary<string, object> param);
			Predicate<T> predicate = predicateExpr.Compile();
			List<T> filtered = list.Where(d => predicate(d)).ToList();
			int count = conn.RecordCount<T>("WHERE " + condition, param, trans);
			if (filtered.Count != count)
				throw new InvalidOperationException();
		}

		public static void InsertGet<T>(SqlConnection conn, SqlTransaction trans, Func<T> constructor) where T : class, IDto<T>
		{
			T constructed = constructor();
			T get = conn.Get(constructed, trans);
			if (get != null)
				throw new InvalidOperationException();
			conn.Insert(constructed, trans);
			get = conn.Get(constructed, trans);
			if (get == null)
				throw new InvalidOperationException();
			if (!get.Equals(constructed))
				throw new InvalidOperationException();
		}

		public static void GetList<T>(SqlConnection conn, SqlTransaction trans, List<T> list) where T : class, IDto<T>
		{
			Dictionary<T, T> map = CreateMap<T>(list);
			var list2 = conn.GetList<T>(trans).AsList();
			if (map.Count != list2.Count)
				throw new InvalidOperationException();
			foreach (T item in list2) {
				if (!map.Remove(item)) {
					throw new InvalidOperationException();
				}
			}
		}

		public static void GetFilter<T>(SqlConnection conn, SqlTransaction trans, List<T> list, IFilter<T> filter) where T : class, IDto<T>
		{
			var list2 = conn.GetList<T>(filter.GetType(), "", null, trans).AsList();
			if (list.Count != list2.Count)
				throw new InvalidOperationException();
			foreach (T item in list2) {
				if (!filter.IsFiltered(item))
					throw new InvalidOperationException();
			}
		}

		public static void Get_Key<T, KeyType>(SqlConnection conn, SqlTransaction trans, List<T> list) where T : class, IDtoKey<T, KeyType>
		{
			foreach (T item in list) {
				T get = conn.Get<T>(item.GetKey(), trans);
				if (!item.IsIdentical(get))
					throw new InvalidOperationException();
			}
		}

		public static void Delete<T>(SqlConnection conn, SqlTransaction trans, List<T> list) where T : class, IDto<T>
		{
			for (int i = 0; i < list.Count; i++) {
				T t = conn.Get(list[i], trans);
				if (t == null)
					throw new InvalidOperationException();
				if (!conn.Delete(list[i], trans))
					throw new InvalidOperationException();
				int count1 = conn.RecordCount<T>(trans);
				if (count1 != (list.Count - i - 1))
					throw new InvalidOperationException();
				t = conn.Get(list[i], trans);
				if (t != null)
					throw new InvalidOperationException();
			}
		}

		public static void GetKeys<T>(SqlConnection conn, SqlTransaction trans, List<T> list) where T : class, IDto<T>
		{
			Dictionary<T, T> map = CreateMap<T>(list);
			IEnumerable<T> keys = conn.GetKeys<T>(trans);
			foreach (T key in keys) {
				if (!map.Remove(key)) {
					throw new InvalidOperationException();
				}
			}
		}

		public static void GetKeys_Key<T, KeyType>(SqlConnection conn, SqlTransaction trans, List<T> list) where T : class, IDtoKey<T, KeyType>
		{
			Dictionary<KeyType, T> map = CreateMap<T, KeyType>(list);
			IEnumerable<KeyType> keys = conn.GetKeys<T, KeyType>(trans);
			foreach (KeyType key in keys) {
				if (!map.Remove(key))
					throw new InvalidOperationException();
			}
		}

		public static void DeleteList<T>(SqlConnection conn, SqlTransaction trans, List<T> list) where T : class, IDto<T>
		{
			int count = conn.DeleteList<T>(trans);
			if (count != list.Count)
				throw new InvalidOperationException();
		}

		public static void Delete_Key<T, KeyType>(SqlConnection conn, SqlTransaction trans, List<T> list) where T : class, IDtoKey<T, KeyType>
		{
			//bool Delete<KeyType>(KeyType key, int commandTimeout = 30);
			for (int i = 0; i < list.Count; i++) {
				var key = list[i].GetKey();
				T t = conn.Get<T>(key, trans);
				if (t == null)
					throw new InvalidOperationException();
				if (!conn.Delete<T>(key, trans))
					throw new InvalidOperationException();
				int count1 = conn.RecordCount<T>(trans);
				if (count1 != (list.Count - i - 1))
					throw new InvalidOperationException();
				t = conn.Get<T>(key, trans);
				if (t != null)
					throw new InvalidOperationException();
			}
		}

		public static void GetLimit<T>(SqlConnection conn, SqlTransaction trans, List<T> list) where T : class, IDto<T>
		{
			//IEnumerable<T> GetLimit(int limit, string whereCondition = "", object param = null, int commandTimeout = 30);
			int max = Math.Min(list.Count, 10);
			for (int i = 0; i < max; i++) {
				List<T> items = conn.GetLimit<T>(i, trans).AsList();
				if (items.Count != i)
					throw new InvalidOperationException();
			}
		}

		public static void GetFilterLimit<T>(SqlConnection conn, SqlTransaction trans, List<T> list, IFilter<T> filter) where T : class, IDto<T>
		{
			int max = Math.Min(list.Count, 10);
			for (int i = 0; i < max; i++) {
				List<T> items = conn.GetLimit<T>(i, filter.GetType(), "", null, trans).AsList();
				if (items.Count != i)
					throw new InvalidOperationException();
			}
		}

		public static void DeleteAll<T>(SqlConnection conn, SqlTransaction trans) where T : class, IDto<T>
		{
			int count = conn.RecordCount<T>(trans);
			if (count == 0)
				throw new InvalidOperationException();
			conn.Truncate<T>(trans);
			count = conn.RecordCount<T>(trans);
			if (count != 0)
				throw new InvalidOperationException();
		}

		public static void Update<T>(SqlConnection conn, SqlTransaction trans, List<T> list, Func<T, T> randomize) where T : class, IDto<T>
		{
			//bool Update(T obj, int commandTimeout = 30);
			foreach (T item in list) {
				T get = conn.Get<T>(item, trans);
				if (get == null)
					throw new InvalidOperationException();
				T updated = randomize(item);
				if (!conn.Update<T>(updated, trans) && !updated.IsIdentical(get))
					throw new InvalidOperationException();
				get = conn.Get<T>(updated, trans);
				if (!updated.IsUpdated(get))
					throw new InvalidOperationException();
			}
		}

		public static void Upsert<T>(SqlConnection conn, SqlTransaction trans) where T : class, IDto<T>
		{
			List<T> list = conn.GetList<T>(trans).AsList();
			for (int i = 0; i < list.Count; i++) {
				bool doDelete = i % 2 == 1;
				if (doDelete) {
					if (!conn.Delete(list[i], trans))
						throw new InvalidOperationException();
					if (!conn.Upsert<T>(list[i], trans))
						throw new InvalidOperationException();
					T get = conn.Get<T>(list[i], trans);
					if (get == null)
						throw new InvalidOperationException();
					if (!list[i].IsInserted(get))
						throw new InvalidOperationException();
				}
				else {
					T get = conn.Get<T>(list[i], trans);
					if (get == null)
						throw new InvalidOperationException();
					if (conn.Upsert<T>(list[i], trans))
						throw new InvalidOperationException();
				}
			}
		}

		public static void GetDistinct<T>(SqlConnection conn, SqlTransaction trans, List<T> list) where T : class, IDto<T>
		{
			//requires IgnoreSelect to be useful
			Dictionary<T, T> map = CreateMap<T>(list);
			List<T> list2 = conn.GetDistinct<T>(typeof(T), trans).AsList();
			if (map.Count != list2.Count)
				throw new InvalidOperationException();
			foreach (T item in list2) {
				if (!map.Remove(item)) {
					throw new InvalidOperationException();
				}
			}
		}

		public static void GetDistinctLimit<T>(SqlConnection conn, SqlTransaction trans, List<T> list) where T : class, IDto<T>
		{
			int max = Math.Min(list.Count, 10);
			for (int i = 2; i < max; i++) {
				Dictionary<T, T> map = CreateMap<T>(list);
				List<T> list2 = conn.GetDistinctLimit<T>(i, typeof(T), trans).AsList();
				if (list2.Count != i)
					throw new InvalidOperationException();
				foreach (T item in list2) {
					if (!map.Remove(item)) {
						throw new InvalidOperationException();
					}
				}
			}
		}

		public static void GetFilterDistinctLimit<T>(SqlConnection conn, SqlTransaction trans, List<T> list, IFilter<T> filter) where T : class, IDto<T>
		{
			int max = Math.Min(list.Count, 10);
			for (int i = 2; i < max; i++) {
				Dictionary<T, T> map = CreateMap<T>(list);
				List<T> list2 = conn.GetDistinctLimit<T>(i, filter.GetType(), trans).AsList();
				if (list2.Count == 0)
					throw new InvalidOperationException();
			}
		}
		#endregion Tested

		#region Tested Bulk
		public static void BulkGet<T>(SqlConnection conn, SqlTransaction trans, List<T> list) where T : class, IDto<T>
		{
			//List<T> BulkGet(IEnumerable<T> keys, int commandTimeout = 30);
			int max = Math.Min(list.Count, 10);
			for (int i = 2; i < max; i++) {
				List<T> limited = list.Take(i).ToList();
				List<T> bulk = conn.BulkGet<T>(limited, trans).AsList();
				for (int j = 0; j < i; j++) {
					if (!limited[j].IsIdentical(bulk[j]))
						throw new InvalidOperationException();
				}
			}
		}

		public static void BulkGet_Key<T, KeyType>(SqlConnection conn, SqlTransaction trans, List<T> list) where T : class, IDtoKey<T, KeyType>
		{
			//public static List<T> BulkGet<KeyType>(IEnumerable<KeyType> keys, int commandTimeout = 30);
			int max = Math.Min(list.Count, 10);
			for (int i = 2; i < max; i++) {
				List<T> limited = list.Take(i).ToList();
				List<T> bulk = conn.BulkGet<T>(limited.Select(c => (object)c.GetKey()), trans).AsList();
				for (int j = 0; j < i; j++) {
					if (!limited[j].IsIdentical(bulk[j]))
						throw new InvalidOperationException();
				}
			}
		}

		public static void BulkInsert<T>(SqlConnection conn, SqlTransaction trans, List<T> list) where T : class, IDto<T>
		{
			conn.Truncate<T>(trans);
			if (conn.RecordCount<T>(trans) != 0)
				throw new InvalidOperationException();
			conn.BulkInsert<T>(list, trans);
			conn.GetList<T>(trans);
			if (conn.RecordCount<T>(trans) != list.Count)
				throw new InvalidOperationException();
		}

		public static void BulkUpdate<T>(SqlConnection conn, SqlTransaction trans, List<T> list, Func<T, T> randomize) where T : class, IDto<T>
		{
			List<T> updateList = list.Select(i => randomize(i)).ToList();
			int count = conn.BulkUpdate<T>(updateList, trans);
			if (count != updateList.Count) {
				for (int i = 0; i < updateList.Count; i++) {
					T get = conn.Get<T>(updateList[i], trans);
					if (!get.IsIdentical(updateList[i]))
						throw new InvalidOperationException();
				}
			}
		}

		public static void BulkDelete<T>(SqlConnection conn, SqlTransaction trans) where T : class, IDto<T>
		{
			List<T> list = conn.GetList<T>(trans).AsList();
			int count = conn.BulkDelete(list, trans);
			if (count != list.Count)
				throw new InvalidOperationException();
			count = conn.RecordCount<T>(trans);
			if (count != 0)
				throw new InvalidOperationException();
		}

		public static void BulkDelete_Key<T, KeyType>(SqlConnection conn, SqlTransaction trans) where T : class, IDtoKey<T, KeyType>
		{
			//int BulkDelete<KeyType>(IEnumerable<KeyType> keys, int commandTimeout = 30);
			List<KeyType> keys = conn.GetList<T>(trans).Select(t => t.GetKey()).AsList();
			int count = conn.BulkDelete<T>(keys.Select(k => (object)k), trans);
			if (count != keys.Count)
				throw new InvalidOperationException();
			count = conn.RecordCount<T>(trans);
			if (count != 0)
				throw new InvalidOperationException();
		}
		#endregion Tested Bulk

		public static void Update_Obj<T>(SqlConnection conn, SqlTransaction trans, List<T> list) where T : class, IDto<T>
		{
			//bool Update(object obj, int commandTimeout = 30);
		}

		public static void BulkUpsert<T>(SqlConnection conn, SqlTransaction trans) where T : class, IDto<T>
		{
			//int BulkUpsert(IEnumerable<T> objs, int commandTimeout = 30);
			List<T> list = conn.GetList<T>(trans).AsList();
			List<T> updates = new List<T>();
			List<T> inserts = new List<T>();
			for (int i = 0; i < list.Count; i += 2) {
				updates.Add(list[i]);
			}
			for (int i = 1; i < list.Count; i += 2) {
				inserts.Add(list[i]);
				if (!conn.Delete(list[i], trans))
					throw new InvalidOperationException();
			}
			int count = conn.BulkUpsert<T>(list, trans);
			if (count != inserts.Count)
				throw new InvalidOperationException();
		}

		public static void BulkInsertIfNotExists<T>(SqlConnection conn, SqlTransaction trans, List<T> list) where T : class, IDto<T>
		{
			List<T> insert = list.Take(list.Count / 2).ToList();
			List<T> fails = list.Skip(list.Count / 2).ToList();
			int count = conn.RecordCount<T>(trans);
			foreach (T item in insert) {
				if (!conn.Delete(item, trans))
					throw new InvalidOperationException();
			}
			if (0 != conn.BulkInsertIfNotExists(fails, trans))
				throw new InvalidOperationException();
			int insertedCount = conn.BulkInsertIfNotExists(insert, trans);
			if (insertedCount != insert.Count)
				throw new InvalidOperationException();
			int count2 = conn.RecordCount<T>(trans);
			if (count2 != count)
				throw new InvalidOperationException();
		}

		public static void InsertIfNotExists<T>(SqlConnection conn, SqlTransaction trans) where T : class, IDto<T>
		{
			List<T> list = conn.GetList<T>(trans).AsList();
			for (int i = 0; i < list.Count; i++) {
				T item = list[i];
				if (conn.InsertIfNotExists(item, trans))
					throw new InvalidOperationException();
				if (!conn.Delete(item, trans))
					throw new InvalidOperationException();
				if (!conn.InsertIfNotExists(item, trans))
					throw new InvalidOperationException();
			}
			int count = conn.RecordCount<T>(trans);
			if (count != conn.RecordCount<T>(trans))
				throw new InvalidOperationException();
		}
	}
}
