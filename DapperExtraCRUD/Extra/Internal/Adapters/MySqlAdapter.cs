using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Extra.Internal.Adapters
{
	internal class MySqlAdapter : SqlAdapterImpl
	{
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
	}
}
