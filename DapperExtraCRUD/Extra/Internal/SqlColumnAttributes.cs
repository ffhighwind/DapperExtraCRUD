using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Extra.Internal
{
	public enum SqlColumnAttributes
	{
		None = 0,
		Key = 1,
		AutoKey = 3,
		MatchUpdate = 4,
		MatchDelete = 8,
		IgnoreSelect = 16,
		IgnoreInsert = 32,
		IgnoreUpdate = 64,
		Ignore = IgnoreSelect | IgnoreInsert | IgnoreUpdate,
	}
}
