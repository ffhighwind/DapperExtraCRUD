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
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using Dapper;
using System.Linq;
using Dapper.Extra.Internal;

namespace Benchmarks
{
	[MemoryDiagnoser]
	public class Inserts
	{
		private const string ConnString = @"Data Source=DESKTOP-V0JVTST\SQLEXPRESS;Initial Catalog=Test;Integrated Security=True;";
		private const int Count = 10000;


		static Inserts()
		{
			_ = Dapper.Extra.ExtraCrud.Builder<Employee>();
		}

		[Benchmark(Description = "InsertMany (ExtraCRUD)", OperationsPerInvoke = 1)]
		public bool InsertManyExtra()
		{
			Random random = new Random(125125);
            var info = Dapper.Extra.ExtraCrud.TypeInfo<Employee>();
            List<SqlColumn> columns = info.Columns.Where(c => !c.IsAutoKey).ToList();
            int max = (2050 / columns.Count) * columns.Count;
            StringBuilder sb = new StringBuilder("INSERT INTO [dbo].[Employees] (" + string.Join(",", columns.Select(c => c.ColumnName)) + @") VALUES 
");
            DynamicParameters dynParams = new DynamicParameters();
            for (int j = 0; j < max;) {
                sb.Append('(');
                Employee employee = Employee.Create(random);
                for (int k = 0; k < columns.Count; k++, j++) {
                    string name = "@p" + j;
                    sb.Append(name).Append(',');
                    dynParams.Add(name, columns[k].Getter(employee));
                }
                sb.Remove(sb.Length - 1, 1).Append(@"),");
            }
            string cmd = sb.Remove(sb.Length - 1, 1).ToString();

            using (SqlConnection conn = new SqlConnection(ConnString)) {
                for (int i = 0; i < Count; i += max) {
                    conn.Execute(cmd, dynParams);
                }
                try {
                    conn.Truncate<Employee>();
                }
                catch {
                    conn.DeleteList<Employee>();
                }
            }
            return true;
		}


		[Benchmark(Description = "InsertMany (Dapper)", OperationsPerInvoke = 1)]
		public bool InsertMany()
		{
			Random random = new Random(125125);
			using (SqlConnection conn = new SqlConnection(ConnString)) {
				//conn.Open();
				int numberPer = Count;
				for (int i = 0; i < Count / numberPer; i++) {
					List<Employee> employees = new List<Employee>();
					for (int j = 0; j < numberPer; j++) {
						Employee employee = Employee.Create(random);
						employees.Add(employee);
					}
					#region SQL
					conn.Execute(@"
INSERT INTO [dbo].[Employees]
        ([FirstName]
        ,[LastName]
        ,[FormerLastName]
        ,[PermissionID]
        ,[Nickname]
        ,[Birthday]
        ,[HireDate]
        ,[Email]
        ,[Title]
        ,[Division]
        ,[Department]
        ,[ManagerID]
        ,[Username]
        ,[Salary]
        ,[Status]
        ,[Weight]
        ,[Height]
        ,[GlobalID]
        ,[LastLogin]
        ,[FailedLoginAttempts]
        ,[Sex]
        ,[PayType]
        ,[PtoHours])
    VALUES
        (@FirstName
        ,@LastName
        ,@FormerLastName
        ,@PermissionID
        ,@Nickname
        ,@Birthday
        ,@HireDate
        ,@Email
        ,@Title
        ,@Division
        ,@Department
        ,@ManagerID
        ,@Username
        ,@Salary
        ,@Status
        ,@Weight
        ,@Height
        ,@GlobalID
        ,@LastLogin
        ,@FailedLoginAttempts
        ,@Sex
        ,@PayType
        ,@PtoHours)
", employees);
					#endregion SQL
				}
				try {
					conn.Truncate<Employee>();
				}
				catch {
					conn.DeleteList<Employee>();
				}
			}
			return true;
		}

		/*
		[Benchmark(Description = "BulkInsert", OperationsPerInvoke = 1)]
		public bool BulkInsert()
		{
            Random random = new Random(125125);
            List<Employee> employees = new List<Employee>();
            for (int i = 0; i < Count; i++) {
                employees.Add(Employee.Create(random));
            }
            using (SqlConnection conn = new SqlConnection(ConnString)) {
				conn.Open();
				conn.BulkInsert(employees);
                try {
                    conn.Truncate<Employee>();
                }
                catch {
                    conn.DeleteList<Employee>();
                }
                conn.Close();
			}
			return true;
		}

		[Benchmark(Description = "Insert (Dapper)", OperationsPerInvoke = 1)]
		public bool Insert()
		{
            Random random = new Random(125125);
            using (SqlConnection conn = new SqlConnection(ConnString)) {
				//conn.Open();
                for (int i = 0; i < Count; i++) {
                    Employee employee = Employee.Create(random);
					#region SQL
					conn.Execute(@"
INSERT INTO [dbo].[Employees]
        ([FirstName]
        ,[LastName]
        ,[FormerLastName]
        ,[PermissionID]
        ,[Nickname]
        ,[Birthday]
        ,[HireDate]
        ,[Email]
        ,[Title]
        ,[Division]
        ,[Department]
        ,[ManagerID]
        ,[Username]
        ,[Salary]
        ,[Status]
        ,[Weight]
        ,[Height]
        ,[GlobalID]
        ,[LastLogin]
        ,[FailedLoginAttempts]
        ,[Sex]
        ,[PayType]
        ,[PtoHours])
    VALUES
        (@FirstName
        ,@LastName
        ,@FormerLastName
        ,@PermissionID
        ,@Nickname
        ,@Birthday
        ,@HireDate
        ,@Email
        ,@Title
        ,@Division
        ,@Department
        ,@ManagerID
        ,@Username
        ,@Salary
        ,@Status
        ,@Weight
        ,@Height
        ,@GlobalID
        ,@LastLogin
        ,@FailedLoginAttempts
        ,@Sex
        ,@PayType
        ,@PtoHours)
", employee);
					#endregion SQL
				}
                try {
                    conn.Truncate<Employee>();
                }
                catch {
                    conn.DeleteList<Employee>();
                }
            }
			return true;
		}
        */

        [Benchmark(Description = "Insert (ExtraCRUD)", OperationsPerInvoke = 1, Baseline = true)]
        public bool InsertExtraCRUD()
        {
            Random random = new Random(125125);
            using (SqlConnection conn = new SqlConnection(ConnString)) {
                //conn.Open();
                for (int i = 0; i < Count; i++) {
                    conn.Insert(Employee.Create(random));
                }
                try {
                    conn.Truncate<Employee>();
                }
                catch {
                    conn.DeleteList<Employee>();
                }
            }
            return true;
        }
	}
}