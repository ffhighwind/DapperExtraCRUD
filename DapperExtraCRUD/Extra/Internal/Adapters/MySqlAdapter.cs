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

namespace Dapper.Extra.Internal.Adapters
{
	/// <summary>
	/// An <see cref="SqlAdapter"/> that generates SQL commands for MySQL.
	/// </summary>
	internal class MySqlAdapter : SqlAdapterImpl
	{
		/// <summary>
		/// An <see cref="SqlAdapter"/> that generates SQL commands for MySQL.
		/// </summary>
		internal MySqlAdapter() : base(SqlDialect.MySQL)
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

			CurrentDate = "CURDATE()";
			CurrentDateTime = "NOW()";
			CurrentDateUtc = "CAST(UTC_TIMESTAMP() as DATE)";
			CurrentDateTimeUtc = "UTC_TIMESTAMP()";
		}

		//public override void BulkInsert<T>(IDbConnection connection, IEnumerable<T> objs, IDbTransaction transaction, string tableName, DataReaderFactory factory, 
		//	IEnumerable<SqlColumn> columns, int commandTimeout = 30, SqlBulkCopyOptions options = SqlBulkCopyOptions.Default)
		//{
		//	https://dev.mysql.com/doc/refman/8.0/en/load-data.html
		//	@"C:\Program Files (x86)\MySQL\MySQL Server 5.0\bin\mysql.exe",
		//	LOAD DATA INFILE 'file.txt' INTO TABLE table;
		//	FIELDS TERMINATED BY '\t' ENCLOSED BY '' ESCAPED BY '\\'
		//	LINES TERMINATED BY '\n' STARTING BY ''
		//}

		/*
		/// <summary>
		/// Creates an SQL command to call the DATEADD function.
		/// </summary>
		/// <param name="interval">The interval type</param>
		/// <param name="amount">The amount to add.</param>
		/// <param name="column">The datetime column. If this is null then it will be replaced by the current date.</param>
		/// <returns>A command to call the DATEADD function.</returns>
		public override string DateAdd(TimeInterval interval, int amount, SqlColumn column = null)
		{
			string intervalStr;
			switch (interval) {
				case TimeInterval.MILLISECOND:
					amount *= 1000;
					goto case TimeInterval.MICROSECOND;
				case TimeInterval.MICROSECOND:
					intervalStr = "MICROSECOND";
					break;
				case TimeInterval.SECOND:
					intervalStr = "SECOND";
					break;
				case TimeInterval.MINUTE:
					intervalStr = "MINUTE";
					break;
				case TimeInterval.HOUR:
					intervalStr = "HOUR";
					break;
				case TimeInterval.DAY:
					intervalStr = "DAY";
					break;
				case TimeInterval.WEEK:
					intervalStr = "WEEK";
					break;
				case TimeInterval.MONTH:
					intervalStr = "MONTH";
					break;
				case TimeInterval.QUARTER:
					intervalStr = "QUARTER";
					break;
				case TimeInterval.YEAR:
					intervalStr = "YEAR";
					break;
				default:
					throw new InvalidOperationException(interval.ToString());
			}
			string columnStr = column == null ? "@" + column.ColumnName : CurrDateTime;
			return string.Format("DATE_ADD({0}, INTERVAL {1} {2})", columnStr, intervalStr, amount);
		}
		*/
	}
}
