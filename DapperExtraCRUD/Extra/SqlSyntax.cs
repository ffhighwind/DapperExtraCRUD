using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Extra
{
	/// <summary>
	/// Determines the RDBMS syntax used for generating queries.
	/// </summary>
	public enum SqlSyntax
	{
		SQLServer,
		PostgreSQL,
		MySQL,
		SQLite,
		Oracle,
	}
}
