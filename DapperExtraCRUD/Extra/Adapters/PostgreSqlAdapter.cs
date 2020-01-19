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

namespace Dapper.Extra.Internal.Adapters
{
	/// <summary>
	/// An <see cref="SqlAdapter"/> that generates SQL commands for PostgreSQL.
	/// </summary>
	internal class PostgreSqlAdapter : SqlAdapterImpl
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PostgreSqlAdapter"/> class.
		/// </summary>
		internal PostgreSqlAdapter() : base(SqlDialect.PostgreSQL)
		{
			QuoteLeft = "\"";
			QuoteRight = "\"";
			EscapeQuote = "\"\"";
			SelectIntIdentityQuery = "SELECT LASTVAL() as \"Id\";";
			DropTempTableIfExistsQuery = "DROP TEMPORARY TABLE IF EXISTS {0};";
			TruncateTableQuery = "TRUNCATE TABLE ONLY {0};";
			CreateTempTable = @"CREATE TEMPORARY TABLE {0} AS
";
			TempTableName = "_{0}";
			LimitQuery = @"{1}
LIMIT {0}";
			CurrentDate = "CURRENT_DATE";
			CurrentDateTime = "NOW()";
			CurrentDateUtc = "(CURRENT_DATE AT TIME ZONE 'UTC')";
			CurrentDateTimeUtc = "(NOW() AT TIME ZONE 'UTC')";
		}

		//public override void BulkInsert<T>(IDbConnection connection, IEnumerable<T> objs, IDbTransaction transaction, string tableName, DataReaderFactory factory, 
		//	IEnumerable<SqlColumn> columns, int commandTimeout = 30, SqlBulkCopyOptions options = SqlBulkCopyOptions.Default)
		//{
		//	https://www.postgresql.org/docs/9.1/sql-copy.html
		//	COPY table FROM file [CSV|BINARY]
		//	COPY [BINARY] table FROM file
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
			string opStr = amount < 0 ? "-" : "+";
			string intervalStr;
			switch (interval) {
				case TimeInterval.MICROSECOND:
					amount /= 1000;
					goto case TimeInterval.MILLISECOND;
				case TimeInterval.MILLISECOND:
					intervalStr = new TimeSpan(0, 0, 0, amount).ToString("'hh:mm:ss.fff'");
					break;
				case TimeInterval.SECOND:
					intervalStr = amount + " SECONDS";
					break;
				case TimeInterval.MINUTE:
					intervalStr = amount + " MINUTES";
					break;
				case TimeInterval.HOUR:
					intervalStr = amount + " HOURS";
					break;
				case TimeInterval.DAY:
					intervalStr = amount + " DAYS";
					break;
				case TimeInterval.WEEK:
					amount *= 7;
					intervalStr = amount + " DAYS";
					break;
				case TimeInterval.MONTH:
					intervalStr = amount + " MONTHS";
					break;
				case TimeInterval.QUARTER:
					intervalStr = amount + " MONTHS";
					break;
				case TimeInterval.YEAR:
					intervalStr = amount + " YEARS";
					break;
				default:
					throw new InvalidOperationException(interval.ToString());
			}
			string columnStr = column == null ? "@" + column.ColumnName : CurrDateTime;
			return string.Format("({0} {1} INTERVAL {2}))", columnStr, opStr, intervalStr);
		}
		*/
	}
}
