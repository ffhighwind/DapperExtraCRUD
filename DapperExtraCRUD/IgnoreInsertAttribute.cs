using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.Extra.Internal;

namespace Dapper
{
	/// <summary>
	/// Ignores the <see cref="PropertyInfo"/> for inserts.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
	public class IgnoreInsertAttribute : IDefaultAttribute
	{
		/// <summary>
		/// Ignores the <see cref="PropertyInfo"/> for inserts.
		/// </summary>
		/// <param name="value">A string that is injected into the insert statement as the column's value.
		/// If this is <see langword="null"/> then the column is not modified.</param>
		public IgnoreInsertAttribute(string value = null) 
			: base(value) { }
	}
}
