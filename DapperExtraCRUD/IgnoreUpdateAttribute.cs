using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.Extra;
using Dapper.Extra.Internal;

namespace Dapper
{
	/// <summary>
	/// Ignores the <see cref="PropertyInfo"/> for updates.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
	public class IgnoreUpdateAttribute : IDefaultAttribute
	{
		/// <summary>
		/// Ignores the <see cref="PropertyInfo"/> for updates.
		/// </summary>
		/// <param name="value">A string that is injected into the update statement as the column's value. 
		/// If this is <see langword="null"/> then the column is not modified.</param>
		public IgnoreUpdateAttribute(string value = null)
			: base(value) { }
	}
}
