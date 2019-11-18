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
	/// The name of the column.
	/// </summary>
	public class ColumnAttribute : Attribute
	{
		public ColumnAttribute(string name, int ordinal = 0)
		{
			Name = name?.Trim();
			Ordinal = ordinal;
		}

		/// <summary>
		/// The name of the column.
		/// </summary>
		public string Name { get; } 

		/// <summary>
		/// The zero-based ordinal of the column.
		/// </summary>
		public int Ordinal { get; }
	}
}
