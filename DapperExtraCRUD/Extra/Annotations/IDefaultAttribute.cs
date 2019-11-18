// Released under MIT License 
// Copyright(c) 2018 Wesley Hamilton
// License: https://www.mit.edu/~amini/LICENSE.md
// Home page: https://github.com/ffhighwind/DapperExtraCRUD

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Extra.Annotations
{
	/// <summary>
	/// Base Attribute for MatchUpdate, IgnoreInsert, and IgnoreUpdate.
	/// </summary>
	public interface IDefaultAttribute
	{
		string Value { get; }
		bool HasValue { get; }
		bool AutoSync { get; }
	}
}
