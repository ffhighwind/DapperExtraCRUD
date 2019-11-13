using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Extra.Internal
{
	/// <summary>
	/// The attributes for the class.
	/// </summary>
	public enum SqlTableAttributes
	{
		None = 0,
		NoDeletes = 1,
		NoInserts = 2,
		NoUpdates = 4
	}
}
