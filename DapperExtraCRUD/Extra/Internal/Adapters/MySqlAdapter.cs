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

using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Fasterflect;

namespace Dapper.Extra.Internal.Adapters
{
	internal class MySqlAdapter : SqlAdapterImpl
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MySqlAdapter"/> class.
		/// </summary>
		public MySqlAdapter() : base(SqlSyntax.MySQL)
		{
			QuoteLeft = "`";
			QuoteRight = "`";
			EscapeQuote = "``";
			SelectIntIdentityQuery = "SELECT LAST_INSERT_ID() as `Id`;";
			DropTempTableIfExistsQuery = "DROP TEMPORARY TABLE IF EXISTS {0};";
			TruncateTableQuery = "TRUNCATE TABLE {0};";
			CreateTempTable = @"CREATE TEMPORARY TABLE {0}
";
			TempTableName = "_{0}";
			LimitQuery = @"{1}
LIMIT {0}";
		}

		//public override void BulkInsert<T>(IDbConnection connection, IEnumerable<T> objs, IDbTransaction transaction, string tableName, DataReaderFactory factory, IEnumerable<SqlColumn> columns, int commandTimeout = 30, SqlBulkCopyOptions options = SqlBulkCopyOptions.Default)
		//{
		//	https://dev.mysql.com/doc/refman/8.0/en/load-data.html
		//	@"C:\Program Files (x86)\MySQL\MySQL Server 5.0\bin\mysql.exe",
		//	LOAD DATA INFILE 'file.txt' INTO TABLE table;
		//	FIELDS TERMINATED BY '\t' ENCLOSED BY '' ESCAPED BY '\\'
		//	LINES TERMINATED BY '\n' STARTING BY ''
		//}
	}
}
