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
	/// An <see cref="SqlAdapter"/> that generates SQL commands for Sqlite.
	/// </summary>
	internal class SqlLiteAdapter : SqlAdapterImpl
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SqlLiteAdapter"/> class.
		/// </summary>
		internal SqlLiteAdapter() : base(SqlDialect.SQLite)
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
			CurrentDate = "DATE('now', 'localtime')";
			CurrentDateTime = "DATETIME('now', 'localtime')";
			CurrentDateUtc = "DATE('now')";
			CurrentDateTimeUtc = "DATETIME('now')";
		}

		//public override void BulkInsert<T>(IDbConnection connection, IEnumerable<T> objs, IDbTransaction transaction, string tableName, DataReaderFactory factory, 
		//	IEnumerable<SqlColumn> columns, int commandTimeout = 30, SqlBulkCopyOptions options = SqlBulkCopyOptions.Default)
		//{
		//	EXEC sqlite3.exe csvfile table
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
			object amountObj = amount;
			string addSubStr = amount < 0 ? "-" : "+";
			string columnStr = column == null ? "@" + column.ColumnName : "'localtime'";
			string intervalStr;
			switch (interval) {
				case TimeInterval.MICROSECOND:
					amountObj = (amount / 1000000.0).ToString("0.0#####");
					intervalStr = "second";
					break;
				case TimeInterval.MILLISECOND:
					amountObj = (amount / 1000000.0).ToString("0.0##");
					intervalStr = "second";
					break;
				case TimeInterval.SECOND:
					intervalStr = "second";
					break;
				case TimeInterval.MINUTE:
					intervalStr = "minute";
					break;
				case TimeInterval.HOUR:
					intervalStr = "hour";
					break;
				case TimeInterval.WEEK:
					amountObj = amount * 7;
					intervalStr = "day";
					break;
				case TimeInterval.DAY:
					intervalStr = "day";
					break;
				case TimeInterval.QUARTER:
					amountObj = amount * 3;
					intervalStr = "month";
					break;
				case TimeInterval.MONTH:
					intervalStr = "month";
					break;
				case TimeInterval.YEAR:
					intervalStr = "year";
					break;
				default:
					throw new InvalidOperationException(interval.ToString());
			}
			return string.Format("DATETIME({0}, {1}{2} {3})", columnStr, addSubStr, intervalStr, amountObj);
		}
		*/
	}
}
