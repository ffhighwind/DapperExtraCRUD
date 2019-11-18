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
	/// A primary key column.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
	public class KeyAttribute : Attribute
	{
		/// <summary>
		/// A primary key column.
		/// </summary>
		/// <param name="autoIncrement">Determines if the key is an identity (auto-incrementing). 
		/// This is ignored if it is not an integral property type (int, long, short, etc).</param>
		public KeyAttribute(bool autoIncrement = true)
		{
			AutoIncrement = autoIncrement;
		}

		/// <summary>
		/// Determines if the primary key is an identity (auto-incrementing).
		/// This is ignored if it is not an integral property type (int, long, short, etc).
		/// </summary>
		public bool AutoIncrement { get; private set; }
	}
}
