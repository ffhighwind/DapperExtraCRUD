using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Dapper.Extra.Utilities;

namespace DapperExtraCRUD.Example
{
	public static class Program
	{
		private const string ConnString = "Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=myPassword;";

		public static void Main()
		{
			using (SqlConnection conn = new SqlConnection(ConnString)) {
				DateTime minHireDate = DateTime.Today.AddDays(-30);
				IEnumerable<User> users = conn.GetList<User>("where HireDate >= @minHireDate ", new { minHireDate });
				User user = new User() {
					FirstName = "Jason",
					LastName = "Borne",
					UserName = "jborne",
					Permissions = UserPermissions.Admin,
				};
				conn.Insert(user);

				string condition = WhereConditionGenerator.Create<User>((u) => u.UserName == "jborne"
					&& (u.FirstName != null || u.Permissions == UserPermissions.Basic) 
					&& new string[] { "Jason", "Chris", "Zack" }.Contains(u.FirstName),
					out IDictionary<string, object> param);
				// condition = "(((Test.Col1 = 11) AND ((Test.Col2 is not NULL) OR (Test.Col4 is NULL))) AND Test.Col2 in @P0)"
				// param = List<object>() { "aa", "bb", "cc" }
				IEnumerable<User> result4 = conn.Query<User>("SELECT * FROM Users WHERE " + condition, param);
			}
		}
	}
}
