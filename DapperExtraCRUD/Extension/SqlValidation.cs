using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Dapper.Extension
{
	public static class SqlValidation
	{
		/// <summary>
		/// Attempts to detect SQL injection. This does not gaurentee that the the value is valid SQL. 
		/// A value is invalid if the SQL includes statement terminators (; or ,), comments (--, /*), or dangling parenthises/brackets/quotes.
		/// </summary>
		/// <param name="sql">The SQL value to validate</param>
		/// <returns>True if the value does not have SQL injection; false otherwise.</returns>
		public static bool IsValidSqlValue(string sql)
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
