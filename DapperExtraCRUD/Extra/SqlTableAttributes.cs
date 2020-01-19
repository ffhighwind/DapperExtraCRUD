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
#endregion

namespace Dapper.Extra
{
	/// <summary>
	/// The attributes for the class.
	/// </summary>
	public enum SqlTableAttributes
	{
		/// <summary>
		/// No table attributes.
		/// </summary>
		None = 0,
		/// <summary>
		/// Prevent inherited properties.
		/// </summary>
		DeclaredOnly = 1,
		/// <summary>
		/// Include inherited attributes.
		/// </summary>
		InheritAttributes = 1 << 1,

		//IgnoreSelect = 1 << 2,

		/// <summary>
		/// Prevents insert commands.
		/// </summary>
		IgnoreInsert = 1 << 3,
		/// <summary>
		/// Prevents update commands.
		/// </summary>
		IgnoreUpdate = 1 << 4,
		/// <summary>
		/// Prevents delete commands.
		/// </summary>
		IgnoreDelete = 1 << 5,
		/// <summary>
		/// Determines if objects should be synchronized after inserts.
		/// </summary>
		InsertAutoSync = 1 << 8,
		/// <summary>
		/// Determines if objects should be synchronized after updates.
		/// </summary>
		UpdateAutoSync = 1 << 9,
	}
}
