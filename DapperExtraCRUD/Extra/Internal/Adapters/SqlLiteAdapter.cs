using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fasterflect;

namespace Dapper.Extra.Internal.Adapters
{
	internal class SqlLiteAdapter : SqlAdapterImpl
	{
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
