﻿// Released under MIT License 
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
	/// The attributes for the class.
	/// </summary>
	public enum SqlTableAttributes
	{
		None = 0,
		DeclaredOnly = 1,
		InheritAttributes = 1 << 1,
		//IgnoreSelect = 1 << 2,
		IgnoreInsert = 1 << 3,
		IgnoreUpdate = 1 << 4,
		IgnoreDelete = 1 << 5
	}
}
