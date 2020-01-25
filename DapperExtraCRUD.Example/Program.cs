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
