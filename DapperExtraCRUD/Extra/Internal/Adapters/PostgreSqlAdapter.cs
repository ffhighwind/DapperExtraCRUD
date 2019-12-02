using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Extra.Internal.Adapters
{
	internal class PostgreSqlAdapter : SqlAdapterImpl
	{
		internal PostgreSqlAdapter() : base(SqlSyntax.PostgreSQL)
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
		}
	}
}
