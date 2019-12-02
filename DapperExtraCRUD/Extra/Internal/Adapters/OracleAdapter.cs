using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Extra.Internal.Adapters
{
	internal class OracleAdapter : SqlAdapterImpl
	{
		public OracleAdapter() : base(SqlSyntax.Oracle)
		{
			QuoteLeft = "'";
			QuoteRight = "'";
			EscapeQuoteRight = "''";
			SelectIntIdentityQuery = ""; // SEQUENCE?
			DropTableIfExistsQuery = @"
BEGIN
	EXECUTE IMMEDIATE 'DROP TABLE {0}';
	EXCEPTION
	WHEN OTHERS THEN NULL;
END;";
			TruncateTableQuery = "TRUNCATE TABLE {0};";
			TempTableName = "{0}";
			CreateTempTable = "";
			LimitQuery = @"TOP({0})
{1}";
		}
	}
}
