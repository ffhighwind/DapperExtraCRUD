// Released under MIT License 
// Copyright(c) 2018 Wesley Hamilton
// License: https://www.mit.edu/~amini/LICENSE.md
// Home page: https://github.com/ffhighwind/DapperExtraCRUD

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Extra.Internal
{
	/// <summary>
	/// The accepted attributes for the column.
	/// </summary>
	public enum SqlColumnAttributes
	{
		None = 0,
		Key = 1,
		AutoKey = (1 << 1) | Key,
		IgnoreSelect = 1 << 2,
		IgnoreInsert = 1 << 3,
		IgnoreUpdate = 1 << 4,
		IgnoreDelete = 1 << 5,
		MatchDelete = 1 << 6,
		MatchUpdate = 1 << 7,
		NotMapped = IgnoreSelect | IgnoreInsert | IgnoreUpdate | IgnoreDelete, 
	}
}
