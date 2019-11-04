using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.Extra.Interfaces;
using System.Reflection;

namespace Dapper
{
	/// <summary>
	/// Turns the <see cref="PropertyInfo"/> into a pseudo key for updates.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class MatchUpdateAttribute : IDefaultAttribute
	{
		/// <summary>
		/// Turns the <see cref="PropertyInfo"/> into a pseudo key for updates and sets the value to the string input if specified.
		/// </summary>
		/// <param name="value">A string that is injected into the update statement as the column's value.
		/// If this is <see langword="null"/> then the column is not modified.</param>
		public MatchUpdateAttribute(string value = null)
			: base(value) { }
	}
}
