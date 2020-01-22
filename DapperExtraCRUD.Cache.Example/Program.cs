using System;
using System.Data.SqlClient;
using Dapper.Extra.Annotations;
using Dapper.Extra.Cache;
using Dapper;

namespace Example
{
	[Table("Employees")]
	public class Employee : IEquatable<Employee>
	{
		[Key]
		public int EmployeeID { get; set; }
		public string UserName { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public DateTime HireDate { get; set; }
		public int ManagerID { get; set; }
		public DateTime DateOfBirth { get; set; }
		[IgnoreInsert("getdate()")]
		[MatchUpdate("getdate()")]
		[AutoSync(syncInsert: true, syncUpdate: true)]
		public DateTime ModifiedDate { get; set; }
		[IgnoreInsert("getdate()", true)]
		[IgnoreUpdate]
		public DateTime CreatedDate { get; set; }

		public override bool Equals(object obj)
		{
			// Equals() is NOT necessary for proper caching
			return Equals(obj as Employee);
		}

		public bool Equals(Employee other)
		{
			return other != null &&	EmployeeID == other.EmployeeID;
		}

		public override int GetHashCode()
		{
			// overriding GetHashCode() is recommended for proper caching
			return -1708179596 * EmployeeID.GetHashCode();
		}
	}

	public class EmployeeItem : CacheItem<Employee>, IEquatable<CacheItem<Employee>>
	{
		public int ID => CacheValue.EmployeeID;
		public string UserName => CacheValue.UserName;
		public string Name => CacheValue.FirstName + " " + CacheValue.LastName;
		public DateTime HireDate => CacheValue.HireDate;
		public EmployeeItem Manager => LazyManager.Value;
		public DateTime DOB => CacheValue.DateOfBirth;
		public double Age => (DateTime.Today - CacheValue.DateOfBirth).TotalDays / 365.0;
		public DateTime ModifiedDate => CacheValue.ModifiedDate;
		public DateTime CreatedDate => CacheValue.CreatedDate;
		private Lazy<EmployeeItem> LazyManager;
		protected override void OnValueChanged()
		{
			// Lazy is required here or this could be costly
			LazyManager = new Lazy<EmployeeItem>(() => DB.Employees[CacheValue.ManagerID], false);
		}

		public bool Save()
		{
			// returns false if deleted or the row was not modified
			return DB.Employees.Update(CacheValue);
		}

		public bool Load()
		{
			EmployeeItem value = DB.Employees.Get(CacheValue);
			return value == this;
		}

		public bool Equals(CacheItem<Employee> other)
		{
			return other != null && other.CacheValue.EmployeeID == CacheValue.EmployeeID;
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as CacheItem<Employee>);
		}

		public override int GetHashCode()
		{
			return CacheValue.GetHashCode();
		}
	}

	public static class DB
	{
		// Please do not hard code your passwords like this ;)
		private const string ConnString = "Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=myPassword;";
		private static readonly DbCache Cache = new DbCache(ConnString);
		public static readonly DbCacheTable<Employee, EmployeeItem> Employees = Cache.CreateTable<Employee, EmployeeItem>();

		public static DateTime GetDate()
		{
			using (SqlConnection conn = new SqlConnection(ConnString)) {
				// getdate() could be replaced by SqlAdapter.CurrentDateTime
				return conn.QueryFirst<DateTime>("select getdate()");
			}
		}
	}

	public static class Program
	{
		public static void Main()
		{
			DB.Employees.GetList(); // caches all employees in the database

			try {
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
			catch (Exception ex) {
				// roll back cache to before the transaction
				Console.WriteLine(ex.Message);
				Console.WriteLine(ex.StackTrace);
			}
			// etc...
		}
	}
}