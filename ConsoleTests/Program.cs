#region License
// Released under MIT License 
// License: https://opensource.org/licenses/MIT
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
using DapperExtraCRUD.Example;

namespace ConsoleTests
{
	public class Program
	{
		private const string ConnString = @"Data Source=DESKTOP-V0JVTST\SQLEXPRESS;Initial Catalog=Test;Integrated Security=True;";
		private const int TestAmount = 300;

		public static void Main()
		{
			Random random = new Random(125125);
			TestStringComparer(random);
			DoBadTests();
			using (SqlConnection conn = new SqlConnection(ConnString)) {
				conn.Open();

				Recreate<TestDTO>(conn, null);
				Recreate<TestDTO2>(conn, null);
				Recreate<Test3>(conn, null);
				Recreate<TestDTO4>(conn, null);
				Recreate<TestDTO5>(conn, null);
				Recreate<TestDTO6>(conn, null);
				Recreate<Test7>(conn, null);
				Recreate<Test8>(conn, null);
				Recreate<Test9>(conn, null);

				TestEnums(conn);
				DoWhereConditionGenTest();

				DoCacheTests<TestDTO>(() => new TestDTO(random));
				DoTests<TestDTO>(() => new TestDTO(random), (t) => t.UpdateRandomize(random), new TestDTOfilter());
				DoTests<TestDTO, int>(conn);

				DoCacheTests<TestDTO2>(() => new TestDTO2(random));
				DoTests<TestDTO2>(() => new TestDTO2(random), (t) => t.UpdateRandomize(random), new TestDTO2filter());

				DoCacheTests<Test3>(() => new Test3(random));
				DoTests<Test3>(() => new Test3(random), (t) => t.UpdateRandomize(random), new Test3filter());

				DoCacheTests<TestDTO4>(() => new TestDTO4(random));
				DoTests<TestDTO4>(() => new TestDTO4(random), (t) => t.UpdateRandomize(random), new TestDTO4filter());
				DoTests<TestDTO4, int>(conn);

				DoCacheTests<TestDTO5>(() => new TestDTO5(random));
				DoTests<TestDTO5>(() => new TestDTO5(random), (t) => t.UpdateRandomize(random), new TestDTO5filter());
				DoTests<TestDTO5, int>(conn);

				DoCacheTests<TestDTO6>(() => new TestDTO6(random));
				DoTests<TestDTO6>(() => new TestDTO6(random), (t) => t.UpdateRandomize(random), new TestDTO6filter());
				DoTests<TestDTO6, string>(conn);

				DoCacheTests<Test7>(() => new Test7(random));
				DoTests<Test7>(() => new Test7(random), (t) => t.UpdateRandomize(random), new Test7filter());
				DoTests<Test7, Test7Type>(conn);

				DoCacheTests<Test8>(() => new Test8(random));
				DoTests<Test8>(() => new Test8(random), (t) => t.UpdateRandomize(random), new Test8filter());
				DoTests<Test8, long>(conn);

				DoCacheTests<Test9>(() => new Test9(random));
				DoTests<Test9>(() => new Test9(random), (t) => t.UpdateRandomize(random), new Test9filter());
				DoTests<Test9, byte[]>(conn);

				DoMultiCacheTest<TestDTO, TestDTO2>(() => new TestDTO(random), () => new TestDTO2(random));
				DoMultiCacheTest<TestDTO, Test3>(() => new TestDTO(random), () => new Test3(random));
				DoMultiCacheTest<TestDTO, TestDTO4>(() => new TestDTO(random), () => new TestDTO4(random));
				DoMultiCacheTest<TestDTO, TestDTO5>(() => new TestDTO(random), () => new TestDTO5(random));
				DoMultiCacheTest<TestDTO2, TestDTO5>(() => new TestDTO2(random), () => new TestDTO5(random));
				DoMultiCacheTest<Test3, TestDTO4>(() => new Test3(random), () => new TestDTO4(random));
				DoMultiCacheTest<TestDTO6, Test3>(() => new TestDTO6(random), () => new Test3(random));
				DoMultiCacheTest<Test7, TestDTO2>(() => new Test7(random), () => new TestDTO2(random));
				DoMultiCacheTest<Test8, TestDTO6>(() => new Test8(random), () => new TestDTO6(random));
				DoMultiCacheTest<Test9, TestDTO4>(() => new Test9(random), () => new TestDTO4(random));

				DropTable<TestDTO>(conn);
				DropTable<TestDTO2>(conn);
				DropTable<Test3>(conn);
				DropTable<TestDTO4>(conn);
				DropTable<TestDTO5>(conn);
				DropTable<TestDTO6>(conn);
				DropTable<Test7>(conn);
				DropTable<Test8>(conn);
				DropTable<Test9>(conn);
			}
		}

		public static void TestEnums(SqlConnection conn)
		{
			IEqualityComparer<Test7> comparer = ExtraCrud.EqualityComparer<Test7>();
			Dictionary<Test7, Test7> map = new Dictionary<Test7, Test7>(comparer);
			Dictionary<Test7Type, Test7> map2 = new Dictionary<Test7Type, Test7>();
			if (conn.RecordCount<Test7>() != 0)
				throw new InvalidOperationException();
			using (SqlTransaction trans = conn.BeginTransaction()) {
				for (int i = (int)Test7Type.ID0; i <= (int)Test7Type.ID19; i += 2) {
					Test7 test = new Test7();
					test.ID = (Test7Type)i;
					map.Add(test, test);
					map2.Add(test.ID, test);
					conn.Insert(test, trans);
				}
				List<Test7> list = conn.GetList<Test7>(trans).AsList();
				if (list.Count != map.Count)
					throw new InvalidOperationException();
				foreach (var item in list) {
					if (!map.Remove(item)) {
						throw new InvalidOperationException();
					}
					if (!map2.Remove(item.ID))
						throw new InvalidOperationException();
				}
			}
			if (conn.RecordCount<Test7>() != 0)
				throw new InvalidOperationException();
		}

		public static void TestStringComparer(Random random)
		{
			IEqualityComparer<TestDTO> comparer = ExtraCrud.EqualityComparer<TestDTO>();
			TestDTO test1a = new TestDTO(random);
			test1a.Name = "ABCDEFG";
			TestDTO test1b = test1a.Clone();
			test1b.Name = test1a.Name.ToLower();
			if (!comparer.Equals(test1a, test1b))
				throw new InvalidOperationException();

			IEqualityComparer<TestDTO2> comparer2 = ExtraCrud.EqualityComparer<TestDTO2>();
			TestDTO2 test2a = new TestDTO2(random);
			test2a.Col2 = "ABCDEFG";
			TestDTO2 test2b = test2a.Clone();
			test2b.Col2 = test2b.Col2.ToLower();
			if (!comparer2.Equals(test2a, test2b))
				throw new InvalidOperationException();

			IEqualityComparer<TestDTO2> comparer6 = ExtraCrud.EqualityComparer<TestDTO2>();
			TestDTO6 test6a = new TestDTO6(random);
			test6a.ID = "ABCDEFG";
			TestDTO6 test6b = test6a.Clone();
			test6b.ID = test6b.ID.ToLower();
			if (!comparer2.Equals(test2a, test2b))
				throw new InvalidOperationException();
		}

		public static List<T> CreateList<T>(int count, Func<T> create) where T : class, IDto<T>
		{
			List<T> list;
			if (ExtraCrud.TypeInfo<T>().AutoKeyColumn != null) {
				list = new List<T>();
				for (int i = 0; i < count; ++i) {
					T created = create();
					list.Add(created);
				}
			}
			else {
				Dictionary<T, T> map = new Dictionary<T, T>(ExtraCrud.EqualityComparer<T>());
				T created = null;
				for (int i = 0; i < count; i++) {
					for (int fails = 0; fails < 4; ++fails) {
						created = create();
						if (!map.ContainsKey(created))
							break;
					}
					map.Add(created, created);
				}
				list = map.Values.ToList();
			}
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

		internal class BadException : Exception
		{
		}

		public static void DoWhereConditionGenTest()
		{
			using (SqlConnection conn = new SqlConnection(ConnString)) {

				string str = WhereConditionGenerator.Create<TestDTO>((t) => t.CreatedDt == new DateTime(1900, 5, 9), out IDictionary<string, object> param);
				if (str != "(Test.CreatedDt = @P0)")
					throw new InvalidOperationException();
				if (param.Any() && (DateTime)param["P0"] != new DateTime(1900, 5, 9))
					throw new InvalidOperationException();
				var result = conn.Query<TestDTO>("SELECT * FROM Test WHERE " + str, param);

				str = WhereConditionGenerator.Create<TestDTO>((t) => t.ID > 5 && t.Name == null, out param);
				if (str != "((Test.ID > 5) AND (Test.FirstName is NULL))")
					throw new InvalidOperationException();
				if (param.Any())
					throw new InvalidOperationException();
				var result2 = conn.Query<TestDTO>("SELECT * FROM Test WHERE " + str, param);

				str = WhereConditionGenerator.Create<TestDTO2>((t) => t.Col3 <= 5.5f && (t.Col1 == 5 || t.Col2.Equals("ab")), out param);
				if (str != "((Test2.Col3 <= @P0) AND ((Test2.Col1 = 5) OR (Test2.Col2 = 'ab')))")
					throw new InvalidOperationException();
				if (param.Any() && (float)param["P0"] != 5.5f)
					throw new InvalidOperationException();
				var result3 = conn.Query<TestDTO2>("SELECT * FROM Test2 WHERE " + str, param);

				str = WhereConditionGenerator.Create<Test3>((t) => t.Col1 == (-15253 & 155) && (t.Col2 != null || t.Col4 == null) && new string[] { "aa", "bb", "cc" }.Contains(t.Col2), out param);
				if (str != "(((Test3.Col1 = 11) AND ((Test3.Col2 is not NULL) OR (Test3.Col4 is NULL))) AND Test3.Col2 in @P0)")
					throw new InvalidOperationException();
				if (param.Any() && ((List<object>)param["P0"]).Any(s => !new string[] { "aa", "bb", "cc" }.Contains(s)))
					throw new InvalidOperationException();
				var result4 = conn.Query<Test3>("SELECT * FROM Test3 WHERE " + str, param);

				str = WhereConditionGenerator.Create<User>((u) => u.UserName == "jborne"
					&& (u.FirstName != null || u.Permissions == UserPermissions.Basic) && new string[] { "Jason", "Chris", "Zack" }.Contains(u.FirstName),
					out param);
				if (str != "(((Users.[Account Name] = 'jborne') AND ((Users.FirstName is not NULL) OR (Users.Permissions = 1))) AND Users.FirstName in @P0)")
					throw new InvalidOperationException();
				// condition = "(((Users.[Account Name] = 'jborne') AND ((Users.FirstName is not NULL) OR (Users.Permissions = 1))) AND Users.FirstName in @P0)"
				// param = List<object>() { "Jason", "Chris", "Zack" }
				str = WhereConditionGenerator.Create<TestDTO>((t) => t.IsActive != true && t.IsActive, out param);
				if (str != "((Test.IsActive <> 1) AND Test.IsActive = 1)")
					throw new InvalidOperationException();
				var result5 = conn.Query<TestDTO>("SELECT * FROM Test WHERE " + str, param);
				str = WhereConditionGenerator.Create<TestDTO>((t) => t.IsActive != true && !t.IsActive, out param);
				if (str != "((Test.IsActive <> 1) AND NOT Test.IsActive = 1)")
					throw new InvalidOperationException();
				var result6 = conn.Query<TestDTO>("SELECT * FROM Test WHERE " + str, param);
				str = WhereConditionGenerator.Create<TestDTO>((t) => t.IsActive, out param);
				if (str != "Test.IsActive = 1")
					throw new InvalidOperationException();
				var result7 = conn.Query<TestDTO>("SELECT * FROM Test WHERE " + str, param);
				bool tmp = false;
				str = WhereConditionGenerator.Create<TestDTO2, TestDTO>((t2, t1) => t2.Col3 <= 5.5f && (t2.Col1 == 5 || t2.Col2.Equals("ab")) && t1.IsActive == tmp, out param);
				if (str != "(((Test2.Col3 <= @P0) AND ((Test2.Col1 = 5) OR (Test2.Col2 = 'ab'))) AND (Test.IsActive = 0))")
					throw new InvalidOperationException();
				var result8 = conn.Query<TestDTO2>("SELECT * FROM Test2 left join Test on Test.ID = Test2.Col1 WHERE\n" + str, param);
				str = WhereConditionGenerator.Create<TestDTO>((t) => t.IsActive.Equals(true) || t.CreatedDt == new DateTime(2019, 1, 5) && (t.ID > 2), out param);
				var result9 = conn.Query<TestDTO>("SELECT * FROM Test WHERE\n" + str, param);
			}
		}

		public static void DoMultiCacheTest<T1, T2>(Func<T1> constructor1, Func<T2> constructor2)
			where T1 : class, IDto<T1>
			where T2 : class, IDto<T2>
		{
			Random random = new Random(512851);
			DbCache cache = new DbCache(ConnString);
			DbCacheTable<T1, CacheItem<T1>> table1 = cache.CreateTable<T1>();
			List<T1> list1 = CreateList<T1>(TestAmount + (int)((random.Next() % TestAmount) * 0.1), () => constructor1());
			DbCacheTable<T2, CacheItem<T2>> table2 = cache.CreateTable<T2>();
			List<T2> list2 = CreateList<T2>(TestAmount + (int)((random.Next() % TestAmount) * 0.1), () => constructor2());

			table1.Truncate();
			table2.Truncate();

			if (table1.GetList().Count() != 0) {
				throw new InvalidOperationException();
			}
			if (table1.Items.Count != table1.RecordCount()) {
				throw new InvalidOperationException();
			}
			if (table2.GetList().Count() != 0) {
				throw new InvalidOperationException();
			}
			if (table2.Items.Count != table2.RecordCount()) {
				throw new InvalidOperationException();
			}
			table1.BulkInsert(list1.Take(list1.Count / 2));
			table2.BulkInsert(list2.Take(list2.Count / 2));

			using (DbCacheTransaction trans = table1.BeginTransaction().Add(table2)) {
				if (table1.Items.Count != table1.RecordCount()) {
					throw new InvalidOperationException();
				}
				if (table2.Items.Count != table2.RecordCount()) {
					throw new InvalidOperationException();
				}
				table1.BulkInsert(list1.Skip(list1.Count / 2).Take(list1.Count / 3));
				table2.BulkInsert(list2.Skip(list2.Count / 2).Take(list2.Count / 4));
				if (table1.Items.Count != table1.RecordCount()) {
					throw new InvalidOperationException();
				}
				if (table2.Items.Count != table2.RecordCount()) {
					throw new InvalidOperationException();
				}
				if (table1.GetList().Count() != (list1.Count / 2) + (list1.Count / 3)) {
					throw new InvalidOperationException();
				}
				if (table2.GetList().Count() != (list2.Count / 2) + (list2.Count / 4)) {
					throw new InvalidOperationException();
				}
				for (int i = 0; i < Math.Min(Math.Min(list1.Count, list2.Count), 15); i++) {
					table1.Delete(list1[i]);
					table2.Delete(list2[i]);
					if (table1.Items.Count != table1.RecordCount()) {
						throw new InvalidOperationException();
					}
					if (table2.Items.Count != table2.RecordCount()) {
						throw new InvalidOperationException();
					}
				}
				trans.Commit();
			}
			if (table1.Items.Count != table1.RecordCount()) {
				throw new InvalidOperationException();
			}
			if (table2.Items.Count != table2.RecordCount()) {
				throw new InvalidOperationException();
			}
			table1.Truncate();
			table2.Truncate();
			if (table1.GetList().Count() != 0) {
				throw new InvalidOperationException();
			}
			if (table1.Items.Count != table1.RecordCount()) {
				throw new InvalidOperationException();
			}
			if (table2.GetList().Count() != 0) {
				throw new InvalidOperationException();
			}
			if (table2.Items.Count != table2.RecordCount()) {
				throw new InvalidOperationException();
			}
		}

		public static void DoBadTests()
		{
			try {
				_ = ExtraCrud.Builder<Bad1>();
				throw new BadException();
			}
			catch (BadException ex) {
				throw ex;
			}
			catch {
				// ignore
			}
			try {
				_ = ExtraCrud.Builder<Bad2>();
				throw new BadException();
			}
			catch (BadException ex) {
				throw ex;
			}
			catch {
				// ignore
			}
			try {
				_ = ExtraCrud.Builder<Bad3>();
				throw new BadException();
			}
			catch (BadException ex) {
				throw ex;
			}
			catch {
				// ignore
			}
			try {
				_ = ExtraCrud.Builder<Bad4>();
				throw new BadException();
			}
			catch (BadException ex) {
				throw ex;
			}
			catch {
				// ignore
			}
			try {
				_ = ExtraCrud.Builder<Bad5>();
				throw new BadException();
			}
			catch (BadException ex) {
				throw ex;
			}
			catch {
				// ignore
			}
		}

		public static void DoCacheTests<T>(Func<T> constructor) where T : class, IDto<T>, new()
		{
			DbCache cache = new DbCache(ConnString);
			DbCacheTable<T, CacheItem<T>> table = cache.CreateTable<T>();
			Random random = new Random(512851);
			List<T> list = CreateList<T>(TestAmount + (int)((random.Next() % TestAmount) * 0.1), () => constructor());

			CacheInsert(table, list);
			CacheDelete(table, list);
			CacheBulkInsert(table, list);
			CacheBulkInsertIfNotExists(table, list);
			CacheBulkUpdate(table, list, random);
			CacheBulkGet(table, list);
			CacheTransactions(table, list, random);
		}

		public static void CacheInsert<T>(DbCacheTable<T, CacheItem<T>> table, List<T> list) where T : class, IDto<T>, new()
		{
			list = CloneList(list).Take(200).ToList();
			for (int i = 0; i < list.Count; i++) {
				table.Insert(list[i]);
				if (!table.Access.Get(list[i]).IsInserted(list[i])) {
					throw new InvalidOperationException();
				}
				int count = table.RecordCount();
				if (table.Items.Count != count) {
					throw new InvalidOperationException();
				}
				using (DbCacheTransaction trans = table.BeginTransaction()) {
					for (int j = i + 1; j < i + 5 && j < list.Count; j += list.Count / 2) {
						table.Insert(list[j]);
						count = table.RecordCount();
						if (table.Items.Count != count) {
							throw new InvalidOperationException();
						}
					}
				}
				count = table.RecordCount();
				if (table.Items.Count != count) {
					throw new InvalidOperationException();
				}
			}
			table.DeleteList();
		}

		public static void CacheBulkInsert<T>(DbCacheTable<T, CacheItem<T>> table, List<T> list) where T : class, IDto<T>, new()
		{
			list = CloneList(list);
			table.Insert(list[0]);
			if (1 != table.RecordCount()) {
				throw new InvalidOperationException();
			}
			using (DbCacheTransaction trans = table.BeginTransaction()) {
				table.BulkInsert(list.Skip(1));
				if (table.Items.Count != table.RecordCount()) {
					throw new InvalidOperationException();
				}
			}
			if (1 != table.RecordCount()) {
				throw new InvalidOperationException();
			}
			table.DeleteList();
		}

		public static void CacheBulkInsertIfNotExists<T>(DbCacheTable<T, CacheItem<T>> table, List<T> list) where T : class, IDto<T>, new()
		{
			bool hasAutoKey = ExtraCrud.TypeInfo<T>().AutoKeyColumn != null;
			list = CloneList(list);
			int count = list.Count / 2;
			table.BulkInsertIfNotExists(list.Take(count));
			using (DbCacheTransaction trans = table.BeginTransaction()) {
				if (count != table.RecordCount()) {
					throw new InvalidOperationException();
				}
				table.BulkInsertIfNotExists(list);
				int count2 = table.RecordCount();
				if (list.Count != count2) {
					if (count + list.Count != count2) {
						throw new InvalidOperationException();
					}
					if (!hasAutoKey) {
						throw new InvalidOperationException();
					}
				}
			}
			if (count != table.RecordCount()) {
				throw new InvalidOperationException();
			}
			table.DeleteList();
		}

		public static void CacheBulkUpdate<T>(DbCacheTable<T, CacheItem<T>> table, List<T> list, Random random) where T : class, IDto<T>, new()
		{
			list = CloneList(list);
			table.BulkInsert(list);
			if (list.Count != table.RecordCount()) {
				throw new InvalidOperationException();
			}
			list = table.Access.GetList().ToList();
			using (DbCacheTransaction trans = table.BeginTransaction()) {
				List<T> updatedList = new List<T>();
				List<T> originalList = new List<T>();
				foreach (var item in table.Items.Values.Select(c => c.CacheValue)) {
					originalList.Add(item);
					T updated = item.UpdateRandomize(random);
					updatedList.Add(updated);
				}
				table.BulkUpdate(updatedList);
				for (int i = 0; i < updatedList.Count; i++) {
					T get = table.Access.Get(originalList[i]);
					T updated = updatedList[i];
					if (!get.IsUpdated(updated)) {
						throw new InvalidOperationException();
					}
				}
			}
			if (list.Count != table.RecordCount()) {
				throw new InvalidOperationException();
			}
			for (int i = 0; i < list.Count; i++) {
				T original = list[i];
				CacheItem<T> get = table.Get(original);
				if (!get.CacheValue.IsIdentical(list[i])) {
					throw new InvalidOperationException();
				}
			}
			table.DeleteList();
		}

		public static void CacheBulkGet<T>(DbCacheTable<T, CacheItem<T>> table, List<T> list) where T : class, IDto<T>, new()
		{
			list = CloneList(list);
			table.Access.BulkInsert(list);
			if (list.Count != table.RecordCount()) {
				throw new InvalidOperationException();
			}
			if (table.Items.Count != 0) {
				throw new InvalidOperationException();
			}
			list = table.Items.Values.Select(c => c.CacheValue).AsList();
			using (DbCacheTransaction trans = table.BeginTransaction()) {
				IEnumerable<T> tmp = list;
				int total = 0;
				while (tmp.Any()) {
					var tmp20 = tmp.Take(20).AsList();
					tmp = tmp.Skip(20);
					List<T> getList = table.BulkGet(tmp20).Select(c => c.CacheValue).AsList();
					total += getList.Count;
					if (getList.Count != tmp20.Count)
						throw new InvalidCastException();
					if (table.Items.Count != total) {
						throw new InvalidOperationException();
					}
				}
			}
			if (table.Items.Count != 0) {
				throw new InvalidOperationException();
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
					for (int j = i + 1; j < i + 5 && j < list.Count; j += list.Count / 2) {
						table.Delete(list[j]);
						if (table.Items.Count != table.RecordCount()) {
							throw new InvalidOperationException();
						}
						foreach (var item in table.Access.GetList()) {
							if (!table.Items.Contains(item)) {
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

		public static void CacheTransactions<T>(DbCacheTable<T, CacheItem<T>> table, List<T> list, Random random) where T : class, IDto<T>, new()
		{
			list = CloneList(list);
			if (table.GetList().Count() != 0) {
				throw new InvalidOperationException();
			}
			if (table.Items.Count != table.RecordCount()) {
				throw new InvalidOperationException();
			}
			table.BulkInsert(list.Take(list.Count / 2));
			if (table.Items.Count != table.RecordCount()) {
				throw new InvalidOperationException();
			}
			List<T> list2 = table.GetList().Select(c => c.CacheValue.Clone()).AsList();
			using (DbCacheTransaction trans = table.BeginTransaction()) {
				if (list2.Count != table.RecordCount()) {
					throw new InvalidOperationException();
				}
				table.BulkInsert(list.Skip(list.Count / 2));
				for (int i = 0; i < list.Count; i++) {
					list[i] = list[i].UpdateRandomize(random);
				}
				table.BulkUpdate(list);
				if (table.Items.Count != table.RecordCount()) {
					throw new InvalidOperationException();
				}
				for (int i = 0; i < list.Count; i++) {
					list[i] = list[i].UpdateRandomize(random);
				}
				table.BulkUpdate(list);
				if (table.Items.Count != table.RecordCount()) {
					throw new InvalidOperationException();
				}
				List<T> list3 = table.Items.Values.Select(c => c.CacheValue).Skip(list.Count / 4).Take(list.Count / 2).ToList();
				int count = table.BulkDelete(list3);
				if (count != list3.Count)
					throw new InvalidCastException();
				if (table.Items.Count != table.RecordCount()) {
					throw new InvalidOperationException();
				}
			}
			if (table.Items.Count != list.Count / 2) {
				throw new InvalidOperationException();
			}
			if (table.GetList().Count() != list.Count / 2) {
				throw new InvalidOperationException();
			}
			for (int i = 0; i < list2.Count; i++) {
				if (!table.Items.TryGetValue(list2[i], out CacheItem<T> cache))
					throw new InvalidOperationException();
				if (!cache.CacheValue.IsIdentical(list2[i]))
					throw new InvalidOperationException();
			}
		}

		public static void DoTests<T>(Func<T> constructor, Func<T, T> randomize, IFilter<T> filter) where T : class, IDto<T>, new()
		{
			Random random = new Random(512851);
			using (SqlConnection conn = new SqlConnection(ConnString)) {
				conn.Open();
				using (SqlTransaction trans = conn.BeginTransaction()) {
					Recreate<T>(conn, trans);
					Insert<T>(conn, trans, TestAmount + (int)((random.Next() % TestAmount) * 0.1), constructor);
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
					Update_Obj<T>(conn, trans, list);
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
			Dictionary<T, T> map = new Dictionary<T, T>(ExtraCrud.EqualityComparer<T>());
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

		public static void WhereConditionGen<T>(SqlConnection conn, SqlTransaction trans, List<T> list, Expression<Func<T, bool>> predicateExpr) where T : class, IDto<T>
		{
			string condition = WhereConditionGenerator.Create<T>(predicateExpr, out IDictionary<string, object> param);
			Func<T, bool> predicate = predicateExpr.Compile();
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
			if (conn.RecordCount<T>(trans) != list.Count) {
				throw new InvalidOperationException();
			}
			Dictionary<T, T> map = CreateMap<T>(list);
			list = conn.GetList<T>(trans).AsList();
			if (map.Count != list.Count)
				throw new InvalidOperationException();
			foreach (T item in list) {
				if (!map.Remove(item)) {
					throw new InvalidOperationException();
				}
			}
		}

		public static void GetFilter<T>(SqlConnection conn, SqlTransaction trans, List<T> list, IFilter<T> filter) where T : class, IDto<T>
		{
			if (conn.RecordCount<T>(trans) != list.Count) {
				throw new InvalidOperationException();
			}
			list = conn.GetList<T>(filter.GetType(), "", null, trans).AsList();
			foreach (T item in list) {
				if (!filter.IsFiltered(item))
					throw new InvalidOperationException();
			}
		}

		public static void Get_Key<T, KeyType>(SqlConnection conn, SqlTransaction trans, List<T> list) where T : class, IDtoKey<T, KeyType>
		{
			if (conn.RecordCount<T>(trans) != list.Count) {
				throw new InvalidOperationException();
			}
			list = CloneList(list);
			foreach (T item in list) {
				T get = conn.Get<T>(item.GetKey(), trans);
				if (!item.IsIdentical(get))
					throw new InvalidOperationException();
			}
		}

		public static void Delete<T>(SqlConnection conn, SqlTransaction trans, List<T> list) where T : class, IDto<T>
		{
			if (conn.RecordCount<T>(trans) != list.Count) {
				throw new InvalidOperationException();
			}
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
			Dictionary<KeyType, T> map;
			if (typeof(KeyType) == typeof(byte[])) {
				map = new Dictionary<KeyType, T>((IEqualityComparer<KeyType>)(object)ArrayEqualityComparer<byte>.Default);
				foreach (var item in list) {
					map.Add(item.GetKey(), item);
				}
			}
			else
				map = CreateMap<T, KeyType>(list);
			IEnumerable<KeyType> keys = conn.GetKeys<T, KeyType>(trans);
			foreach (KeyType key in keys) {
				if (!map.Remove(key))
					throw new InvalidOperationException();
			}
		}

		public static void DeleteList<T>(SqlConnection conn, SqlTransaction trans, List<T> list) where T : class, IDto<T>
		{
			if (conn.RecordCount<T>(trans) != list.Count) {
				throw new InvalidOperationException();
			}
			int count = conn.DeleteList<T>(trans);
			if (count != list.Count)
				throw new InvalidOperationException();
		}

		public static void Delete_Key<T, KeyType>(SqlConnection conn, SqlTransaction trans, List<T> list) where T : class, IDtoKey<T, KeyType>
		{
			if (conn.RecordCount<T>(trans) != list.Count) {
				throw new InvalidOperationException();
			}
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
			if (conn.RecordCount<T>(trans) != list.Count) {
				throw new InvalidOperationException();
			}
			list = CloneList(list);
			int max = Math.Min(list.Count, 20);
			for (int i = 0; i < max; i++) {
				List<T> items = conn.GetLimit<T>(i, trans).AsList();
				if (items.Count != i)
					throw new InvalidOperationException();
			}
		}

		public static void GetFilterLimit<T>(SqlConnection conn, SqlTransaction trans, List<T> list, IFilter<T> filter) where T : class, IDto<T>
		{
			if (conn.RecordCount<T>(trans) != list.Count) {
				throw new InvalidOperationException();
			}
			int max = Math.Min(list.Count, 20);
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
			if (conn.RecordCount<T>(trans) != list.Count) {
				throw new InvalidOperationException();
			}
			list = CloneList(list);
			//bool Update(T obj, int commandTimeout = 30);
			List<T> failures = new List<T>();
			foreach (T item in list) {
				T get = conn.Get<T>(item, trans);
				if (get == null)
					throw new InvalidOperationException();
				T updated = randomize(get);
				if (!conn.Update<T>(updated, trans)) {
					if (!updated.IsIdentical(get)) {
						failures.Add(get);
						continue;
					}
				}
				get = conn.Get<T>(updated, trans);
				if (!updated.IsUpdated(get)) {
					throw new InvalidOperationException();
				}
			}
			if (failures.Count > 0) {
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
			if (conn.RecordCount<T>(trans) != list.Count) {
				throw new InvalidOperationException();
			}
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
			if (conn.RecordCount<T>(trans) != list.Count) {
				throw new InvalidOperationException();
			}
			list = CloneList(list);
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
			if (conn.RecordCount<T>(trans) != list.Count) {
				throw new InvalidOperationException();
			}
			list = CloneList(list);
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
			if (conn.RecordCount<T>(trans) != list.Count) {
				throw new InvalidOperationException();
			}
			list = CloneList(list);
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
			if (conn.RecordCount<T>(trans) != list.Count) {
				throw new InvalidOperationException();
			}
			list = CloneList(list);
			int max = Math.Min(list.Count, 10);
			for (int i = 2; i < max; i++) {
				List<T> limited = list.Take(i).OrderBy(x => x).ToList();
				List<T> bulk = conn.BulkGet<T>(limited.Select(c => (object)c.GetKey()), trans).OrderBy(x => x).AsList();
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
			if (conn.RecordCount<T>(trans) == 0) {
				throw new InvalidOperationException();
			}
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
			var updateObj = Dapper.Extra.ExtraCrud.Builder<T>().Queries.UpdateObj;
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
