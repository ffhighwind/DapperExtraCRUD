using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper
{
	/// <summary>
	/// Ignores the <see cref="PropertyInfo"/> for inserts.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class IgnoreDeleteAttribute : Attribute
	{
		/// <summary>
		/// Prevents inserts.
		/// </summary>
		public IgnoreDeleteAttribute() { }
	}
}
