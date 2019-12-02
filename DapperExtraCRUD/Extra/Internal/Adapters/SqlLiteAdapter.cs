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

namespace Dapper.Extra.Internal.Adapters
{
	internal class SqlLiteAdapter : SqlAdapterImpl
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SqlLiteAdapter"/> class.
		/// </summary>
		internal SqlLiteAdapter() : base(SqlSyntax.SQLite)
		{
			QuoteLeft = "\"";
			QuoteRight = "\"";
			EscapeQuote = "\"\"";
			SelectIntIdentityQuery = "SELECT LAST_INSERT_ROWID() as \"Id\";";
			DropTempTableIfExistsQuery = "DROP TABLE IF EXISTS {0};";
			TruncateTableQuery = @"DELETE FROM {0};
DELETE FROM SQLITE_SEQUENCE WHERE name='{0}'"; // resets autoincrement
			CreateTempTable = @"CREATE TEMPORARY TABLE {0} AS
";
			TempTableName = "_{0}"; // temp.{0}
			LimitQuery = @"{1}
LIMIT {0}";
		}
	}
}
