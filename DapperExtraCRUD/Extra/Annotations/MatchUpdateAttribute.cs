// Released under MIT License 
// Copyright(c) 2018 Wesley Hamilton
// License: https://www.mit.edu/~amini/LICENSE.md
// Home page: https://github.com/ffhighwind/DapperExtraCRUD

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Dapper.Extra.Internal;

namespace Dapper.Extra.Annotations
{
	/// <summary>
	/// Turns the <see cref="PropertyInfo"/> into a pseudo key for updates.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class MatchUpdateAttribute : Attribute, IDefaultAttribute
	{
		/// <summary>
		/// Turns the <see cref="PropertyInfo"/> into a pseudo key for updates and sets the value to the string input if specified.
		/// </summary>
		/// <param name="value">A string that is injected into the update statement as the column's value.
		/// If this is <see langword="null"/> then the column is not modified.</param>
		/// <param name="autoSync"></param>
		public MatchUpdateAttribute(string value = null, bool autoSync = false)
		{
			AutoSync = autoSync;
			Value = string.IsNullOrWhiteSpace(value) ? null : "(" + value.Trim() + ")";
		}
		/// <summary>
		/// A string that is injected into the update statement as the column's new value for updates.
		/// If this is <see langword="null"/> then the column's value will not change on updates.
		/// </summary>
		public string Value { get; }
		/// <summary>
		/// Checks if the value is null.
		/// </summary>
		public bool HasValue => Value != null;
		/// <summary>
		/// Determines if this column will be automatically selected after an update.
		/// </summary>
		public bool AutoSync { get; }
	}
}
