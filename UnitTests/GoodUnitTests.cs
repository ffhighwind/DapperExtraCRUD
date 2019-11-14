﻿
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Dapper.Extra;
using Dapper.Extra.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
	[TestClass]
	public class GoodUnitTests
	{
		//private const string ConnString = @"DESKTOP-V0JVTST\SQLEXPRESS";
		//private const string ConnString = @"Server=DESKTOP-V0JVTST\SQLEXPRESS; Database=Test; Trusted_Connection=True;";
		//private const string ConnString = @"Server=(localdb)\MyInstance;Integrated Security=true;"

		//private const string ConnString = @"Data Source=C:\Program Files\Microsoft SQL Server\MSSQL14.SQLEXPRESS\MSSQL\DATA\Test.mdf; Initial Catalog=Test; Integrated Security=true; User Instance=True;";
		//private const string ConnString = @"Data Source=.; Initial Catalog=Test; Integrated Security=true;";

		private const string ConnString = @"Data Source=DESKTOP-V0JVTST\SQLEXPRESS;Initial Catalog=Test;Integrated Security=True;";
		private static Random random = new Random(123512);

		List<TestDTO> list1 = new List<TestDTO>();
		List<TestDTO2> list2 = new List<TestDTO2>();
		List<Test3> list3 = new List<Test3>();
		List<TestDTO4> list4 = new List<TestDTO4>();

		#region Initialize
		[TestInitialize]
		public void TestInit()
		{
			string[] tables = new string[] {
				ExtraCrud.Builder<TestDTO>().TableName, ExtraCrud.Builder<TestDTO2>().TableName, ExtraCrud.Builder<Test3>().TableName, ExtraCrud.Builder<TestDTO4>().TableName,
			};
			using(SqlConnection conn = new SqlConnection(ConnString)) {
				conn.Open();
				using(trans = conn.BeginTransaction()) {
					foreach (string table in tables) {
						string cmd = DropTable(table);
						conn.Execute(cmd, null, trans);
					}
					conn.Execute(TestDTO.CreateTable(), null, trans);
					conn.Execute(TestDTO2.CreateTable(), null, trans);
					conn.Execute(Test3.CreateTable(), null, trans);
					conn.Execute(TestDTO4.CreateTable(), null, trans);
					trans.Commit();
				}
			}
			 list1 = new List<TestDTO>();
			for (int i = 0, max = random.Next() % 50 + 25; i < max; i++) {
				list1.Add(new TestDTO(random));
			}
			list2 = new List<TestDTO2>();
			for (int i = 0, max = random.Next() % 50 + 25; i < max; i++) {
				list2.Add(new TestDTO2(random));
			}
			list3 = new List<Test3>();
			for (int i = 0, max = random.Next() % 50 + 25; i < max; i++) {
				list3.Add(new Test3(random));
			}
			list4 = new List<TestDTO4>();
			for (int i = 0, max = random.Next() % 50 + 25; i < max; i++) {
				list4.Add(new TestDTO4(random));
			}
		}

		private static string DropTable(string tableName)
		{
			string str = $@"
IF EXISTS (
	SELECT * from INFORMATION_SCHEMA.TABLES 
WHERE TABLE_NAME = '{tableName}' 
	AND TABLE_SCHEMA = 'dbo'
) 
DROP TABLE dbo.{tableName};";
			return str;
		}
		#endregion Initialize

		[TestMethod]
		public void Insert()
		{
			using (SqlConnection conn = new SqlConnection(ConnString)) {
				conn.Open();
				using (SqlTransaction trans = conn.BeginTransaction()) {
					for(int i = 0; i < 4; i++) {
						conn.Insert(list2[i]);
						TestDTO2 obj = conn.Get(list2[i]);
					}
					/*
					int deleted1 = conn.Delete<TestDTO>("", null, trans);
					int deleted4 = conn.Delete<TestDTO4>("", null, trans);
					// Bulk Insert
					list = conn.BulkInsert(list, trans).OrderBy(x => x.ID).ToList();
					for (int i = 0; i < list.Count; i++) {
						if (list[i].ID == -1) {
							throw new InvalidOperationException();
						}
					}
					// BulkDelete
					int count = conn.BulkDelete(list, trans);

					list = conn.BulkInsert(list, trans).OrderBy(x => x.ID).ToList();
					// Update
					for (int i = 0; i < list.Count; i++) {
						list[i].Name = "Update " + list[i].Name;
						if (!conn.Update(list[i], trans)) {
							throw new InvalidOperationException();
						}
					}
					// Get
					for (int i = 0; i < list.Count; i++) {
						TestDTO tmp = conn.Get(list[i], trans);
						if (!tmp.Name.StartsWith("Update")) {
							throw new InvalidOperationException();
						}
					}

					// GetList
					List<TestDTO> newList = conn.GetList<TestDTO>("", null, trans).OrderBy(x => x.ID).ToList();
					if (newList.Count != list.Count) {
						throw new InvalidOperationException();
					}
					for (int i = 0; i < list.Count; i++) {
						if (newList[i].ID != list[i].ID) {
							throw new InvalidOperationException();
						}
					}

					// BulkUpsert
					list.Add(dto);
					for (int i = 0; i < list.Count; i++) {
						list[i].CreatedDt = list[i].CreatedDt.Value.AddDays(5);
					}

					//upsert
					for (int i = 0; i < list.Count; i++) {
						conn.Upsert(list[i], trans);
					}
					//conn.BulkUpsert(list, trans);
					newList = conn.GetList<TestDTO>("", null, trans).OrderBy(x => x.ID).ToList();
					if (newList.Count != list.Count) {
						throw new InvalidOperationException();
					}
					for (int i = 0; i < list.Count; i++) {
						if (newList[i].CreatedDt == list[i].CreatedDt) {
							throw new InvalidOperationException();
						}
					}

					// RecordCount
					count = conn.RecordCount<TestDTO>("", null, trans);
					if (count != 4)
						throw new InvalidOperationException();

					// BulkDelete
					count = conn.BulkDelete(list, trans);
					if (count != 4)
						throw new InvalidOperationException();

					// RecordCount
					count = conn.RecordCount<TestDTO>("", null, trans);
					if (count != 0)
						throw new InvalidOperationException();

					// Upsert
					conn.Upsert(list[0], trans);
					newList = conn.GetList<TestDTO>("", null, trans).OrderBy(x => x.ID).ToList();
					if (newList.Count != 1)
						throw new InvalidOperationException();
					if (list[0].ID != newList[0].ID)
						throw new InvalidOperationException();

					// Insert
					for (int i = 1; i < list.Count; i++) {
						conn.Insert(list[i], trans);
					}

					// DeleteList
					newList = conn.DeleteList<TestDTO>("", null, trans).OrderBy(x => x.ID).ToList();
					if (newList.Count != 4)
						throw new InvalidOperationException();
					for (int i = 0; i < newList.Count; i++) {
						if (list[i].ID != newList[i].ID)
							throw new InvalidOperationException();
					}

					conn.Insert(list2[0], trans);
					conn.Insert(list2[1], trans);
					list2[0].LastName = "Hamilton2"; // MatchUpdate
					list2[1].FirstName = "Wesley"; // MatchDelete
					if (conn.Update(list2[0], trans)) {
						throw new InvalidOperationException();
					}
					if (!conn.Delete(list2[0], trans)) {
						throw new InvalidOperationException();
					}
					if (!conn.Update(list2[1], trans)) {
						throw new InvalidOperationException();
					}
					if (!conn.Delete(list2[1], trans)) {
						throw new InvalidOperationException();
					}
					*/
				}
			}
		}
	}
}
