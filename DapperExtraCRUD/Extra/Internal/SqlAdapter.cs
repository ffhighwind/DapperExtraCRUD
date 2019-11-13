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
		public static readonly SqlAdapter SQLServer = new SqlAdapter(SqlSyntax.SQLServer);
		public static readonly SqlAdapter PostgreSQL = new SqlAdapter(SqlSyntax.PostgreSQL);
		public static readonly SqlAdapter SQLite = new SqlAdapter(SqlSyntax.SQLite);
		public static readonly SqlAdapter MySQL = new SqlAdapter(SqlSyntax.MySQL);

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
					_SelectIdentityQuery = "SELECT LASTVAL() as \"Id\";";
					SelectIdentityLongQuery = _SelectIdentityQuery;
					DropTableIfExistsQuery = "DROP TABLE IF EXISTS {0};";
					TruncateTableQuery = "TRUNCATE TABLE ONLY {0};";
					break;
				case SqlSyntax.SQLite:
					QuoteLeft = '"';
					QuoteRight = '"';
					EscapeQuoteRight = "\"\"";
					_SelectIdentityQuery = "SELECT LAST_INSERT_ROWID() as \"Id\";";
					SelectIdentityLongQuery = _SelectIdentityQuery;
					DropTableIfExistsQuery = "DROP TABLE IF EXISTS {0};";
					TruncateTableQuery = "DELETE FROM {0};";
					break;
				case SqlSyntax.MySQL:
					QuoteLeft = '`';
					QuoteRight = '`';
					EscapeQuoteRight = "``";
					_SelectIdentityQuery = "SELECT LAST_INSERT_ID() as `Id`;";
					SelectIdentityLongQuery = _SelectIdentityQuery;
					DropTableIfExistsQuery = "DROP TABLE IF EXISTS {0};";
					TruncateTableQuery = "TRUNCATE TABLE {0};";
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
					_SelectIdentityQuery = "SELECT CAST(SCOPE_IDENTITY() as INT) as [Id];";
					SelectIdentityLongQuery = "SELECT CAST(SCOPE_IDENTITY() as BIGINT) as [Id];";
					DropTableIfExistsQuery = @"
IF EXISTS (
	SELECT * from INFORMATION_SCHEMA.TABLES 
WHERE TABLE_NAME = '{0}' 
	AND TABLE_SCHEMA = '{1}'
)
DROP TABLE {1}.{0};";
					TruncateTableQuery = "TRUNCATE TABLE {0};";
					break;
			}
			QuoteRightStr = QuoteRight.ToString();
		}

		public SqlSyntax Syntax { get; private set; }

		public string QuoteIdentifier(string identifier)
		{
			return IsIdentifier(identifier)
				? identifier 
				: QuoteLeft + identifier.Replace(QuoteRightStr, EscapeQuoteRight) + QuoteRight;
		}

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

		public string DropTableIfExists(string tableName, string schema = "dbo")
		{
			return string.Format(DropTableIfExistsQuery, tableName, schema);
		}

		public string TruncateTable(string tableName)
		{
			return string.Format(TruncateTableQuery, tableName);
		}

		public static string MakeString(string str)
		{
			return "'" + str.Replace("'", "''") + "'";
		}

		public string SelectIdentityQuery(Type type)
		{
			return type == typeof(long) || type == typeof(ulong) ? SelectIdentityLongQuery : _SelectIdentityQuery;
		}

		private readonly string TruncateTableQuery;
		private readonly char QuoteLeft;
		private readonly char QuoteRight;
		private readonly string QuoteRightStr;
		private readonly string EscapeQuoteRight;
		private readonly string _SelectIdentityQuery;
		private readonly string SelectIdentityLongQuery;
		private readonly string DropTableIfExistsQuery;
	}
}
