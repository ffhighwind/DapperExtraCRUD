
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
		private const string ConnString = @"Data Source=DESKTOP-V0JVTST\SQLEXPRESS;Initial Catalog=Test;Integrated Security=True;";
		private static readonly Random random = new Random(123512);

		#region Initialize
		[TestInitialize]
		public void TestInit()
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
					//conn.Execute(TestDTO.CreateTable(), null, trans);
					//conn.Execute(TestDTO2.CreateTable(), null, trans);
					//conn.Execute(Test3.CreateTable(), null, trans);
					//conn.Execute(TestDTO4.CreateTable(), null, trans);
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
		#endregion Initialize

		[TestMethod]
		public void InsertGet()
		{
			using (SqlConnection conn = new SqlConnection(ConnString)) {
				conn.Open();
				using (SqlTransaction trans = conn.BeginTransaction()) {

				}
			}
		}
	}
}
