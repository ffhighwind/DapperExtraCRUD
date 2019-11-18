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
	/// Ignores the <see cref="PropertyInfo"/> for deletes.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class IgnoreDeleteAttribute : Attribute
	{
		/// <summary>
		/// Prevents deletion.
		/// </summary>
		public IgnoreDeleteAttribute() { }
	}
}
