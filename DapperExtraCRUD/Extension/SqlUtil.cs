using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Dapper.Extension
{
	public static class SqlUtil
	{
		/// <summary>
		/// Escapes an input parameterized string for use with string "like" comparisons in SQL.
		/// </summary>
		/// <param name="sql">The input string.</param>
		/// <returns>An escaped SQL string.</returns>
		public static string EscapeLikeParamString(string sql)
		{
			return sql.Replace("[", "[[]").Replace("%", "[%]");
		}

		public static readonly IReadOnlyCollection<string> Keywords = new HashSet<string>()
		{
			"ABSOLUTE",
			"EXEC",
			"OVERLAPS",
			"ACTION",
			"EXECUTE",
			"PAD",
			"ADA",
			"EXISTS",
			"PARTIAL",
			"ADD",
			"EXTERNAL",
			"PASCAL",
			"ALL",
			"EXTRACT",
			"POSITION",
			"ALLOCATE",
			"FALSE",
			"PRECISION",
			"ALTER",
			"FETCH",
			"PREPARE",
			"AND",
			"FIRST",
			"PRESERVE",
			"ANY",
			"FLOAT",
			"PRIMARY",
			"ARE",
			"FOR",
			"PRIOR",
			"AS",
			"FOREIGN",
			"PRIVILEGES",
			"ASC",
			"FORTRAN",
			"PROCEDURE",
			"ASSERTION",
			"FOUND",
			"PUBLIC",
			"AT",
			"FROM",
			"READ",
			"AUTHORIZATION",
			"FULL",
			"REAL",
			"AVG",
			"GET",
			"REFERENCES",
			"BEGIN",
			"GLOBAL",
			"RELATIVE",
			"BETWEEN",
			"GO",
			"RESTRICT",
			"BIT",
			"GOTO",
			"REVOKE",
			"BIT_LENGTH",
			"GRANT",
			"RIGHT",
			"BOTH",
			"GROUP",
			"ROLLBACK",
			"BY",
			"HAVING",
			"ROWS",
			"CASCADE",
			"HOUR",
			"SCHEMA",
			"CASCADED",
			"IDENTITY",
			"SCROLL",
			"CASE",
			"IMMEDIATE",
			"SECOND",
			"CAST",
			"IN",
			"SECTION",
			"CATALOG",
			"INCLUDE",
			"SELECT",
			"CHAR",
			"INDEX",
			"SESSION",
			"CHAR_LENGTH",
			"INDICATOR",
			"SESSION_USER",
			"CHARACTER",
			"INITIALLY",
			"SET",
			"CHARACTER_LENGTH",
			"INNER",
			"SIZE",
			"CHECK",
			"INPUT",
			"SMALLINT",
			"CLOSE",
			"INSENSITIVE",
			"SOME",
			"COALESCE",
			"INSERT",
			"SPACE",
			"COLLATE",
			"INT",
			"SQL",
			"COLLATION",
			"INTEGER",
			"SQLCA",
			"COLUMN",
			"INTERSECT",
			"SQLCODE",
			"COMMIT",
			"INTERVAL",
			"SQLERROR",
			"CONNECT",
			"INTO",
			"SQLSTATE",
			"CONNECTION",
			"IS",
			"SQLWARNING",
			"CONSTRAINT",
			"ISOLATION",
			"SUBSTRING",
			"CONSTRAINTS",
			"JOIN",
			"SUM",
			"CONTINUE",
			"KEY",
			"SYSTEM_USER",
			"CONVERT",
			"LANGUAGE",
			"TABLE",
			"CORRESPONDING",
			"LAST",
			"TEMPORARY",
			"COUNT",
			"LEADING",
			"THEN",
			"CREATE",
			"LEFT",
			"TIME",
			"CROSS",
			"LEVEL",
			"TIMESTAMP",
			"CURRENT",
			"LIKE",
			"TIMEZONE_HOUR",
			"CURRENT_DATE",
			"LOCAL",
			"TIMEZONE_MINUTE",
			"CURRENT_TIME",
			"LOWER",
			"TO",
			"CURRENT_TIMESTAMP",
			"MATCH",
			"TRAILING",
			"CURRENT_USER",
			"MAX",
			"TRANSACTION",
			"CURSOR",
			"MIN",
			"TRANSLATE",
			"DATE",
			"MINUTE",
			"TRANSLATION",
			"DAY",
			"MODULE",
			"TRIM",
			"DEALLOCATE",
			"MONTH",
			"TRUE",
			"DEC",
			"NAMES",
			"UNION",
			"DECIMAL",
			"NATIONAL",
			"UNIQUE",
			"DECLARE",
			"NATURAL",
			"UNKNOWN",
			"DEFAULT",
			"NCHAR",
			"UPDATE",
			"DEFERRABLE",
			"NEXT",
			"UPPER",
			"DEFERRED",
			"NO",
			"USAGE",
			"DELETE",
			"NONE",
			"USER",
			"DESC",
			"NOT",
			"USING",
			"DESCRIBE",
			"NULL",
			"VALUE",
			"DESCRIPTOR",
			"NULLIF",
			"VALUES",
			"DIAGNOSTICS",
			"NUMERIC",
			"VARCHAR",
			"DISCONNECT",
			"OCTET_LENGTH",
			"VARYING",
			"DISTINCT",
			"OF",
			"VIEW",
			"DOMAIN",
			"ON",
			"WHEN",
			"DOUBLE",
			"ONLY",
			"WHENEVER",
			"DROP",
			"OPEN",
			"WHERE",
			"ELSE",
			"OPTION",
			"WITH",
			"END",
			"OR",
			"WORK",
			"END-EXEC",
			"ORDER",
			"WRITE",
			"ESCAPE",
			"OUTER",
			"YEAR",
			"EXCEPT",
			"OUTPUT",
			"ZONE",
			"EXCEPTION",
			"@@DATEFIRST",
			"@@OPTIONS",
			"@@DBTS",
			"@@REMSERVER",
			"@@LANGID",
			"@@SERVERNAME",
			"@@LANGUAGE",
			"@@SERVICENAME",
			"@@LOCK_TIMEOUT",
			"@@SPID",
			"@@MAX_CONNECTIONS",
			"@@TEXTSIZE",
			"@@MAX_PRECISION",
			"@@VERSION",
			"@@NESTLEVEL",
		};

		/// <summary>
		/// Determines if the input string is an SQL keyword.
		/// </summary>
		/// <param name="str">The input string.</param>
		/// <returns>True if the input string is an SQL keyword; otherwise false.</returns>
		public static bool IsSqlKeyword(string str)
		{
			return Keywords.Contains(str.ToUpper());
		}

		/// <summary>
		/// Determines if the input string is an SQL identifier.
		/// </summary>
		/// <param name="sql">The input string.</param>
		/// <returns>True if the input string is an SQL identifier; otherwise false.</returns>
		public static bool IsSqlIdentifier(string sql)
		{
			sql = sql.Trim();
			if (sql.Length == 0 || sql.Length > 124)
				return false;
			int i = 0;
			char c = sql[0];
			switch (c) {
				case '[':
					if (sql[sql.Length - 1] != ']' || sql.Length < 3)
						return false;
					for (i = 1; i < sql.Length - 2; i++) {
						c = sql[i];
						if (c == ']')
							return false;
						if (c == '[') {
							if (i <= sql.Length - 2 || sql[i + 2] != ']')
								return false;
							i += 2;
						}
					}
					return true;
				case '\'':
					if (sql[sql.Length - 1] != '\'' || sql.Length < 3)
						return false;
					for (i = 1; i < sql.Length - 2; i++) {
						c = sql[i];
						if (c == '\'') {
							if (sql[i + 1] != '\'' || i == sql.Length - 2)
								return false;
							i++;
						}
					}
					return true;
				case '@':
				case '#':
					if (sql.Length == 1)
						return false;
					i++;
					if (sql.Length == 2) {
						if (sql[0] == sql[1])
							return false;
						i++;
					}
					else if (sql[0] == sql[1])
						i++;
					c = sql[i];
					if (!char.IsLetter(c) && c != '_')
						return false;
					i++;
					break;
			}
			for (; i < sql.Length; i++) {
				c = sql[i];
				if (!char.IsLetter(c) && !char.IsDigit(c) && c != '_' && c != '$')
					return false;
			}
			return !Keywords.Contains(sql);
		}

		/// <summary>
		/// Attempts to detect SQL injection. This does not gaurentee that the the value is valid SQL. 
		/// A value is invalid if the SQL includes statement terminators (; or ,), comments (--, /*), or dangling parenthises/brackets/quotes.
		/// </summary>
		/// <param name="sql">The SQL value to validate</param>
		/// <returns>True if the value does not have SQL injection; false otherwise.</returns>
		public static bool IsSqlValue(string sql)
		{
			if (sql == null)
				return false;
			int leftParens = 0;
			for (int i = 0; i < sql.Length; i++) {
				if (sql[i] == '\'') {
					// string value
					for (i = i + 1; i < sql.Length; i++) {
						if (sql[i] == '\'') {
							if (i + 1 >= sql.Length)
								return true;
							if (sql[i + 1] != '\'')
								break; // end of string
							i++; // escaped quote ''
						}
					}
					if (i == sql.Length)
						return false; // failed to find end-quote
					i--; // prevent i++
				}
				else if (sql[i] == ';' || sql[i] == ',' || (sql[i] == '-' && i <= sql.Length && sql[i + 1] == '-') || (sql[i] == '/' && i <= sql.Length && sql[i + 1] == '*')) //prevent comments and statement terminators
					return false;
				else if (sql[i] == '(')
					leftParens++;
				else if (sql[i] == ')') {
					leftParens--;
					if (leftParens < 0)
						return false;
				}
			}
			if (leftParens != 0)
				return false;
			return true;
		}
	}
}
