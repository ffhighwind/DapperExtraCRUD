# Introduction

[Nuget: Dapper.ExtraCRUD.Cache](https://www.nuget.org/packages/Dapper.ExtraCRUD.Cache/)
<img  align="right" src="https://raw.githubusercontent.com/ffhighwind/DapperExtraCRUD/master/Images/DapperExtraCRUD-200x200.png" alt="ExtraCRUD">

A cache framework for Dapper.ExtraCRUD. This combines the functionality of DataAccessObjects/AutoAccessObject and a ConcurrentDictionary with support for transactions and rollbacks.

#### Notes

The cache is not thread-safe. It is recommended that you keep a separate cache per thread or do not use caching at all when multi-threading.

Using caching on databases where multiple users will simultaneously update/delete data is not recommended. If you are going to do this then you should be very careful. Refresh caches for tables that are modified often. Always refresh or double-check for changes before performing updates and deletes.

## Example

```csharp
using System;
using System.Data.SqlClient;
using Dapper.Extra.Annotations;
using Dapper.Extra.Cache;
using Dapper;

namespace Example
{
public class Employee
{
	[Key]
	public int EmployeeID { get; set; }
	public string UserName { get; set; }
	public string FirstName { get; set; }
	public string LastName { get; set; }
	public DateTime HireDate { get; set; }
	public int ManagerID { get; set; }
	public DateTime DateOfBirth { get; set; }
	[IgnoreInsert]
	[MatchUpdate]
	[AutoSync(syncInsert: true, syncUpdate: true)]
	public DateTime ModifiedDate { get; set; }
	[IgnoreInsert(value: "getdate()", autoSync: true)]
	[IgnoreUpdate]
	public DateTime CreatedDate { get; set; }
}

public class EmployeeItem : CacheItem<Employee>
{
	public int ID => CacheValue.EmployeeID;
	public string UserName => CacheValue.UserName;
	public string Name => CacheValue.FirstName + " " + CacheValue.LastName;
	public DateTime HireDate => CacheValue.HireDate;
	public EmployeeItem Manager { get; private set; }
	public DateTime DOB => CacheValue.DateOfBirth;
	public double Age => (DateTime.Today - CacheValue.DateOfBirth).TotalDays / 365.0;
	public DateTime ModifiedDate => CacheValue.ModifiedDate;
	public DateTime CreatedDate => CacheValue.CreatedDate;
	protected override void ValueChanged()
	{
		Manager = DB.Employees[CacheValue.ManagerID];
	}

	public bool Save()
	{
		return DB.Employees.Update(CacheValue);
	}

	public bool Load()
	{
		EmployeeItem value = DB.Employees.Get(CacheValue);
		return value == this;
	}
}

public static class DB
{
	private const string ConnString = "Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=myPassword;";
	private static readonly DbCache Cache = new DbCache(ConnString);
	public static readonly DbCacheTable<Employee, EmployeeItem> Employees = Cache.CreateTable<Employee, EmployeeItem>();

	public static DateTime GetDate()
	{
		using (SqlConnection conn = new SqlConnection(ConnString)) {
			return conn.QueryFirst<DateTime>("select getdate()");
		}
	}
}

public static class Program
{
	public static void Main()
	{
		DB.Employees.GetList(); // get all rows in the database

		using (DbCacheTransaction transaction = DB.Employees.BeginTransaction()) {
			Employee emp = new Employee() {
				DateOfBirth = new DateTime(2000, 2, 15),
				FirstName = "Jack",
				LastName = "Black",
				HireDate = DB.GetDate(),
				UserName = "blackj",
				ManagerID = 2,
			};
			EmployeeItem empItem = DB.Employees.Insert(emp); // automatically uses the transaction
			Console.WriteLine("Manager: " + empItem.Manager.Name + "\nAge: " + empItem.Manager.Age);
			transaction.Commit();
		}
	}
}
}
```
# License:

*MIT License*

Copyright (c) 2018 Wesley Hamilton

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
