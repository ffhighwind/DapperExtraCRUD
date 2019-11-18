// Released under MIT License 
// Copyright(c) 2018 Wesley Hamilton
// License: https://www.mit.edu/~amini/LICENSE.md
// Home page: https://github.com/ffhighwind/DapperExtraCRUD

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.Extra;
using Dapper.Extra.Internal;

namespace Dapper.Extra.Annotations
{
	/// <summary>
	/// Ignores the <see cref="PropertyInfo"/> for updates.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class IgnoreUpdateAttribute : Attribute, IDefaultAttribute
	{
		/// <summary>
		/// Ignores the <see cref="PropertyInfo"/> for updates.
		/// </summary>
		/// <param name="value">A string that is injected into the update statement as the column's value. 
		/// If this is <see langword="null"/> then the column cannot be updated.</param>
		public IgnoreUpdateAttribute(string value = null, bool autoSync = false)
		{
			AutoSync = autoSync;
			Value = string.IsNullOrWhiteSpace(value) ? null : "(" + value.Trim() + ")";
		}
		/// <summary>
		/// A string that is injected into the update statement as the column's value.
		/// If this is <see langword="null"/> then the column cannot be updated.
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
