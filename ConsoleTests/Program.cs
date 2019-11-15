
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Dapper.Extra;
using Dapper.Extra.Utilities;

namespace UnitTests
{
	public class Program
	{

		public static void Main(string[] args)
		{
			TestInit();
			//InsertGet();
			//RecordCount();
			Delete();
		}

		//private const string ConnString = @"DESKTOP-V0JVTST\SQLEXPRESS";
		//private const string ConnString = @"Server=DESKTOP-V0JVTST\SQLEXPRESS; Database=Test; Trusted_Connection=True;";
		//private const string ConnString = @"Server=(localdb)\MyInstance;Integrated Security=true;"

		//private const string ConnString = @"Data Source=C:\Program Files\Microsoft SQL Server\MSSQL14.SQLEXPRESS\MSSQL\DATA\Test.mdf; Initial Catalog=Test; Integrated Security=true; User Instance=True;";
		//private const string ConnString = @"Data Source=.; Initial Catalog=Test; Integrated Security=true;";

		private const string ConnString = @"Data Source=DESKTOP-V0JVTST\SQLEXPRESS;Initial Catalog=Test;Integrated Security=True;";
		private static Random random = new Random(123512);

		//Dictionary<int, TestDTO> map1 = new Dictionary<int, TestDTO>();
		//Dictionary<TestDTO2, TestDTO2> map2 = new Dictionary<TestDTO2, TestDTO2>();
		//Dictionary<Test3, Test3> map3 = new Dictionary<Test3, Test3>();
		//Dictionary<int, TestDTO4> map4 = new Dictionary<int, TestDTO4>();


		public static void TestInit()
		{
			string[] tables = new string[] {
				ExtraCrud.Builder<TestDTO>().TableName, ExtraCrud.Builder<TestDTO2>().TableName, ExtraCrud.Builder<Test3>().TableName, ExtraCrud.Builder<TestDTO4>().TableName,
			};
			using (SqlConnection conn = new SqlConnection(ConnString)) {
				conn.Open();
				using (SqlTransaction trans = conn.BeginTransaction()) {
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


		public static void InsertGet()
		{
			using (SqlConnection conn = new SqlConnection(ConnString)) {
				conn.Open();
				using (SqlTransaction trans = conn.BeginTransaction()) {
					TestDTO dto1 = new TestDTO(random);
					TestDTO2 dto2 = new TestDTO2(random);
					Test3 dto3 = new Test3(random);
					TestDTO4 dto4 = new TestDTO4(random);
					conn.Insert(dto1, trans);
					conn.Insert(dto2, trans);
					conn.Insert(dto3, trans);
					conn.Insert(dto4, trans);
					if (dto1.ID == 0)
						throw new InvalidOperationException();
					if (dto4.ID == 0)
						throw new InvalidOperationException();

					TestDTO dto1_ = conn.Get(dto1, trans);
					if (dto1.CreatedDt == dto1_.CreatedDt || !dto1.IsKeyEqual(dto1_))
						throw new InvalidOperationException();
					TestDTO2 dto2_ = conn.Get(dto2, trans);
					if (!dto2.IsKeyEqual(dto2))
						throw new InvalidOperationException();
					Test3 dto3_ = conn.Get(dto3, trans);
					if (!dto3.IsKeyEqual(dto3_))
						throw new InvalidOperationException();
					TestDTO4 dto4_ = conn.Get(dto4, trans);
					if (!dto4.IsKeyEqual(dto4_))
						throw new InvalidOperationException();
				}
			}
		}

		public static void RecordCount()
		{
			using (SqlConnection conn = new SqlConnection(ConnString)) {
				conn.Open();
				using (SqlTransaction trans = conn.BeginTransaction()) {
					int count1 = random.Next() % 35 + 25;
					int count2 = random.Next() % 15 + 10;
					int count3 = random.Next() % 10 + 30;
					int count4 = random.Next() % 22 + 11;
					for (int i = 0; i < count1; i++) {
						conn.Insert(new TestDTO(random), trans);
					}
					for (int i = 0; i < count2; i++) {
						conn.Insert(new TestDTO2(random), trans);
					}
					for (int i = 0; i < count3; i++) {
						conn.Insert(new Test3(random), trans);
					}
					for (int i = 0; i < count4; i++) {
						conn.Insert(new TestDTO4(random), trans);
					}
					if (count1 != conn.RecordCount<TestDTO>("", null, trans))
						throw new InvalidOperationException();
					if (count2 != conn.RecordCount<TestDTO2>("", null, trans))
						throw new InvalidOperationException();
					if (count3 != conn.RecordCount<Test3>("", null, trans))
						throw new InvalidOperationException();
					if (count4 != conn.RecordCount<TestDTO4>("", null, trans))
						throw new InvalidOperationException();
				}
			}
		}

		public static void Delete()
		{
			using (SqlConnection conn = new SqlConnection(ConnString)) {
				conn.Open();
				using (SqlTransaction trans = conn.BeginTransaction()) {
					TestDTO dto1 = new TestDTO(random);
					TestDTO2 dto2 = new TestDTO2(random);
					Test3 dto3 = new Test3(random);
					TestDTO4 dto4 = new TestDTO4(random);

					conn.Insert(dto1, trans);
					conn.Insert(dto2, trans);
					conn.Insert(dto3, trans);
					conn.Insert(dto4, trans);

					int count1 = conn.RecordCount<TestDTO>();
					if (count1 != 1)
						throw new InvalidOperationException();
					int count2 = conn.RecordCount<TestDTO2>();
					if (count2 != 1)
						throw new InvalidOperationException();
					int count3 = conn.RecordCount<Test3>();
					if (count3 != 1)
						throw new InvalidOperationException();
					int count4 = conn.RecordCount<TestDTO4>();
					if (count4 != 1)
						throw new InvalidOperationException();

					if (!conn.Delete(dto1, trans))
						throw new InvalidOperationException();
					if (!conn.Delete(dto2, trans))
						throw new InvalidOperationException();
					if (!conn.Delete(dto3, trans))
						throw new InvalidOperationException();
					if (!conn.Delete(dto4, trans))
						throw new InvalidOperationException();

					count1 = conn.RecordCount<TestDTO>();
					if (count1 != 0)
						throw new InvalidOperationException();
					count2 = conn.RecordCount<TestDTO2>();
					if (count2 != 0)
						throw new InvalidOperationException();
					count3 = conn.RecordCount<Test3>();
					if (count3 != 0)
						throw new InvalidOperationException();
					count4 = conn.RecordCount<TestDTO4>();
					if (count4 != 0)
						throw new InvalidOperationException();

					TestDTO dto1_ = conn.Get(dto1, trans);
					TestDTO2 dto2_ = conn.Get(dto2, trans);
					Test3 dto3_ = conn.Get(dto3, trans);
					TestDTO4 dto4_ = conn.Get(dto4, trans);

					if (dto1_ != null)
						throw new InvalidOperationException();
					if (dto2_ != null)
						throw new InvalidOperationException();
					if (dto3_ != null)
						throw new InvalidOperationException();
					if (dto4_ != null)
						throw new InvalidOperationException();
				}
			}
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
