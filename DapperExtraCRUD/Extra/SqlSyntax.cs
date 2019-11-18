// Released under MIT License 
// Copyright(c) 2018 Wesley Hamilton
// License: https://www.mit.edu/~amini/LICENSE.md
// Home page: https://github.com/ffhighwind/DapperExtraCRUD

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Extra
{
	/// <summary>
	/// Represents and RDBMS syntax used for generating queries.
	/// </summary>
	public enum SqlSyntax
	{
		SQLServer,
		PostgreSQL,
		MySQL,
		SQLite,
		//Oracle,
	}
}
