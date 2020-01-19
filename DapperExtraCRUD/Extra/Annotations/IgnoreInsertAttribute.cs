#region License
// Released under MIT License 
// License: https://opensource.org/licenses/MIT
// Home page: https://github.com/ffhighwind/DapperExtraCRUD

// Copyright(c) 2018 Wesley Hamilton

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
#endregion License

using System;

namespace Dapper.Extra.Annotations
{
	/// <summary>
	/// Ignores the property for inserts.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class IgnoreInsertAttribute : Attribute, IDefaultAttribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="IgnoreInsertAttribute"/> class.
		/// </summary>
		public IgnoreInsertAttribute()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="IgnoreInsertAttribute"/> class.
		/// </summary>
		/// <param name="value">A string that is substituted for the column's value on insert.
		/// If this is <see langword="null"/> then the database's default value will be inserted instead.</param>
		/// <param name="autoSync">Determines if this column will be automatically selected after an insert.</param>
		public IgnoreInsertAttribute(string value, bool autoSync = false)
		{
			AutoSync = autoSync;
			if (!string.IsNullOrWhiteSpace(value)) {
				Value = "(" + value.Trim() + ")";
			}
		}

		/// <summary>
		/// Determines if this column will be automatically selected after an insert.
		/// </summary>
		public bool AutoSync { get; }

		/// <summary>
		/// Checks if the value is <see langword="null"/>.
		/// </summary>
		public bool HasValue => Value != null;

		/// <summary>
		/// A string that is substituted for the column's value on insert.
		/// If this is <see langword="null"/> then the database's default value will be inserted instead.
		/// </summary>
		public string Value { get; }
	}
}
