#region License
// Released under MIT License 
// License: https://www.mit.edu/~amini/LICENSE.md
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
#endregion

using System;

namespace Dapper.Extra.Annotations
{
	/// <summary>
	/// A named column in a database table.
	/// </summary>
	public class ColumnAttribute : Attribute
	{
		/// <summary>
		/// A named column in a database table.
		/// </summary>
		/// <param name="name">The name of the column.</param>
		/// <param name="ordinal">The zero-based ordinal of the column.</param>
		public ColumnAttribute(string name, int ordinal = 0)
		{
			Name = name;
			Ordinal = ordinal;
		}

		/// <summary>
		/// The name of the column.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// The zero-based ordinal of the column.
		/// </summary>
		public int Ordinal { get; }
	}
}
