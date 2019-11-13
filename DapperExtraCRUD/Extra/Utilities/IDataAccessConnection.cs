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
