// Released under MIT License 
// Copyright(c) 2018 Wesley Hamilton
// License: https://www.mit.edu/~amini/LICENSE.md
// Home page: https://github.com/ffhighwind/DapperExtraCRUD

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.Extra.Internal;

namespace Dapper.Extra.Annotations
{
	/// <summary>
	/// Ignores the <see cref="PropertyInfo"/> for inserts.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class IgnoreInsertAttribute : Attribute, IDefaultAttribute
	{
		/// <summary>
		/// Ignores the <see cref="PropertyInfo"/> for inserts.
		/// </summary>
		public IgnoreInsertAttribute()
		{
		}

		/// <summary>
		/// Ignores the <see cref="PropertyInfo"/> for inserts.
		/// </summary>
		/// <param name="value">A string that is injected into the insert statement as the column's value.
		/// If this is <see langword="null"/> then the default value will be inserted instead.</param>
		/// <param name="autoSync">Determines if the property should be selected to match the database after an insert.</param>
		public IgnoreInsertAttribute(string value, bool autoSync = false)
		{
			AutoSync = autoSync;
			if (!string.IsNullOrWhiteSpace(value)) {
				Value = "(" + value.Trim() + ")";
			}
		}

		/// <summary>
		/// A string that is injected into the insert statement as the column's value.
		/// If this is <see langword="null"/> then the default value will be inserted instead.
		/// </summary>
		public string Value { get; }
		/// <summary>
		/// Checks if the value is null.
		/// </summary>
		public bool HasValue => Value != null;
		/// <summary>
		/// Determines if this column will be automatically selected after an insert.
		/// </summary>
		public bool AutoSync { get; }
	}
}
