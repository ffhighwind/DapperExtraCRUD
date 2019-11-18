// Released under MIT License 
// Copyright(c) 2018 Wesley Hamilton
// License: https://www.mit.edu/~amini/LICENSE.md
// Home page: https://github.com/ffhighwind/DapperExtraCRUD

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Extra.Utilities
{
	public interface IDataAccessConnection
	{
		SqlConnection Connection { get; set; }
		SqlTransaction Transaction { get; set; }
		bool Buffered { get; set; }
	}
}
