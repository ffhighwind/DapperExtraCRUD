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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Extra.Internal
{
	/// <summary>
	/// Generates specialized commands using a given syntax.
	/// </summary>
	public class SqlAdapter
	{
		/// <summary>
		/// An <see cref="SqlAdapter"/> that generates SQL commands for SQL Server.
		/// </summary>
		public static readonly SqlAdapter SQLServer = new SqlAdapter(SqlSyntax.SQLServer);
		/// <summary>
		/// An <see cref="SqlAdapter"/> that generates SQL commands for PostgreSQL.
		/// </summary>
		public static readonly SqlAdapter PostgreSQL = new SqlAdapter(SqlSyntax.PostgreSQL);
		/// <summary>
		/// An <see cref="SqlAdapter"/> that generates SQL commands for SQLite.
		/// </summary>
		public static readonly SqlAdapter SQLite = new SqlAdapter(SqlSyntax.SQLite);
		/// <summary>
		/// An <see cref="SqlAdapter"/> that generates SQL commands for MySQL.
		/// </summary>
		public static readonly SqlAdapter MySQL = new SqlAdapter(SqlSyntax.MySQL);

		/// <summary>
		/// Gets the <see cref="SqlAdapter"/> that matches a given <see cref="SqlSyntax"/>.
		/// </summary>
		/// <param name="syntax">The syntax of the <see cref="SqlAdapter"/>.</param>
		/// <returns>The <see cref="SqlAdapter"/> that matches a given <see cref="SqlSyntax"/>.</returns>
		public static SqlAdapter GetAdapter(SqlSyntax syntax)
		{
			switch (syntax) {
				case SqlSyntax.MySQL:
					return MySQL;
				case SqlSyntax.PostgreSQL:
					return PostgreSQL;
				case SqlSyntax.SQLite:
					return SQLite;
				case SqlSyntax.SQLServer:
				default:
					return SQLServer;
			}
		}

		private SqlAdapter(SqlSyntax syntax)
		{
			Syntax = syntax;
			switch (syntax) {
				case SqlSyntax.PostgreSQL:
					QuoteLeft = '"';
					QuoteRight = '"';
					EscapeQuoteRight = "\"\"";
					SelectIntIdentityQuery = "SELECT LASTVAL() as \"Id\";";
					SelectIdentityLongQuery = SelectIntIdentityQuery;
					DropTempTableIfExistsQuery = "DROP TEMPORARY TABLE IF EXISTS {0};";
					TruncateTableQuery = "TRUNCATE TABLE ONLY {0};";
					CreateTempTable = @"CREATE TEMPORARY TABLE {0} AS\n";
					TempTableName = "_{0}";
					SelectLimitStart = "";
					SelectLimitEnd = "\nLIMIT {0}";
					break;
				case SqlSyntax.SQLite:
					QuoteLeft = '"';
					QuoteRight = '"';
					EscapeQuoteRight = "\"\"";
					SelectIntIdentityQuery = "SELECT LAST_INSERT_ROWID() as \"Id\";";
					SelectIdentityLongQuery = SelectIntIdentityQuery;
					DropTempTableIfExistsQuery = "DROP TABLE IF EXISTS {0};";
					TruncateTableQuery = "DELETE FROM {0}; DELETE FROM SQLITE_SEQUENCE WHERE name='{0}'"; // resets autoincrement
					CreateTempTable = @"CREATE TEMPORARY TABLE {0} AS\n";
					TempTableName = "_{0}"; // temp.{0}
					SelectLimitStart = "";
					SelectLimitEnd = "\nLIMIT {0}";
					break;
				case SqlSyntax.MySQL:
					QuoteLeft = '`';
					QuoteRight = '`';
					EscapeQuoteRight = "``";
					SelectIntIdentityQuery = "SELECT LAST_INSERT_ID() as `Id`;";
					SelectIdentityLongQuery = SelectIntIdentityQuery;
					DropTempTableIfExistsQuery = "DROP TEMPORARY TABLE IF EXISTS {0};";
					TruncateTableQuery = "TRUNCATE TABLE {0};";
					CreateTempTable = @"CREATE TEMPORARY TABLE {0}\n";
					TempTableName = "_{0}";
					SelectLimitStart = "";
					SelectLimitEnd = "\nLIMIT {0}";
					break;
				/*case SqlSyntax.Oracle:
					QuoteLeft = '\'';
					QuoteRight = '\'';
					EscapeQuoteRight = "''";
					_SelectIdentityQuery = null;
					SelectIdentityLongQuery = null; // SEQUENCE?
					DropTableIfExistsQuery = @"
BEGIN
	EXECUTE IMMEDIATE 'DROP TABLE {0}';
EXCEPTION
	WHEN OTHERS THEN NULL;
END;";
					TruncateTableQuery = "TRUNCATE TABLE {0};";
				break;
				*/
				case SqlSyntax.SQLServer:
				default:
					QuoteLeft = '[';
					QuoteRight = ']';
					EscapeQuoteRight = "[]]";
					SelectIntIdentityQuery = "SELECT CAST(SCOPE_IDENTITY() as INT) as [Id];";
					SelectIdentityLongQuery = "SELECT CAST(SCOPE_IDENTITY() as BIGINT) as [Id];";
					DropTempTableIfExistsQuery = @"
IF OBJECT_ID('tempdb..{0}') IS NOT NULL DROP TABLE {0}";/*@"
IF EXISTS (
	SELECT * from INFORMATION_SCHEMA.TABLES 
WHERE TABLE_NAME = '{0}' 
	AND TABLE_SCHEMA = '{1}'
)
DROP TABLE {1}.{0};";*/
					TruncateTableQuery = "TRUNCATE TABLE {0};";
					TempTableName = "#{0}";
					CreateTempTable = "";
					SelectLimitStart = "TOP({0}) ";
					SelectLimitEnd = "";
					break;
			}
			QuoteRightStr = QuoteRight.ToString();
		}

		/// <summary>
		/// The syntax used to generate SQL commands.
		/// </summary>
		public SqlSyntax Syntax { get; private set; }

		/// <summary>
		/// Creates an SQL command to clone specific columns from a table into a temporary table.
		/// </summary>
		/// <param name="sourceTable">The table to clone.</param>
		/// <param name="tempTable">The temporary to create.</param>
		/// <param name="columns">The columns to copy.</param>
		/// <returns>A command to create a temporary table with specific columns.</returns>
		public string SelectIntoTempTable(string sourceTable, string tempTable, IEnumerable<SqlColumn> columns)
		{
			return string.Format(CreateTempTable, tempTable) + SqlBuilderHelper.SelectIntoTableQuery(sourceTable, tempTable, columns);
		}

		/// <summary>
		/// Creates a name for a temporary table based on an input table name. This currently only affects SQL Server.
		/// </summary>
		/// <param name="tableName">The table name.</param>
		/// <returns>A temporary table name.</returns>
		public string CreateTempTableName(string tableName)
		{
			return string.Format(TempTableName, tableName);
		}

		/// <summary>
		/// Quotes an identifier if necessary. This uses <see cref="IsIdentifier(string)"/> to determine if it needs to be quoted.
		/// </summary>
		/// <param name="identifier">The identifier.</param>
		/// <returns>The original identifier if it does not need to be quoted, otherwise a quoted identifier.</returns>
		public string QuoteIdentifier(string identifier)
		{
			return IsIdentifier(identifier)
				? identifier 
				: QuoteLeft + identifier.Replace(QuoteRightStr, EscapeQuoteRight) + QuoteRight;
		}

		/// <summary>
		/// Checks if the input string qualifies as a basic identifier. This will return false if 
		/// the string does not match the following regular expression: [a-zA-Z_][a-zA-Z0-9_]*
		/// </summary>
		/// <param name="identifier">The identifier.</param>
		/// <returns>True if input qualifies as a basic identifier; otherwise false.</returns>
		public static bool IsIdentifier(string identifier)
		{
			char c = identifier[0];
			if (!((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_'))
				return false;
			for (int i = 1; i < identifier.Length; i++) {
				c = identifier[i];
				if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9' || c == '_')) {
					continue;
				}
				return false;
			}
			return true;
		}

		/// <summary>
		/// Creates an SQL command to drop a temporary table if it exists.
		/// </summary>
		/// <param name="tableName">The temporary table name.</param>
		/// <param name="schema">The schema of the table.</param>
		/// <returns>A command to drop a table if it exists.</returns>
		public string DropTempTableIfExists(string tableName)
		{
			return string.Format(DropTempTableIfExistsQuery, tableName);
		}

		/// <summary>
		/// Creates an SQL command to truncate a table.
		/// </summary>
		/// <param name="tableName">The table name.</param>
		/// <returns>A command to truncate a table.</returns>
		public string TruncateTable(string tableName)
		{
			return string.Format(TruncateTableQuery, tableName);
		}

		/// <summary>
		/// Creates an SQL command to select a generated row's identity (auto-increment).
		/// </summary>
		/// <param name="type">The type of the identity.</param>
		/// <returns>A command to select a generated row's identity.</returns>
		public string SelectIdentityQuery(Type type)
		{
			return type == typeof(long) || type == typeof(ulong) ? SelectIdentityLongQuery : SelectIntIdentityQuery;
		}

		private readonly string TruncateTableQuery;
		private readonly char QuoteLeft;
		private readonly char QuoteRight;
		private readonly string QuoteRightStr;
		private readonly string EscapeQuoteRight;
		private readonly string SelectIntIdentityQuery;
		private readonly string SelectIdentityLongQuery;
		private readonly string DropTempTableIfExistsQuery;
		private readonly string TempTableName;
		private readonly string CreateTempTable;

		public string SelectLimitStart { get; private set; }
		public string SelectLimitEnd { get; private set; }
	}
}
