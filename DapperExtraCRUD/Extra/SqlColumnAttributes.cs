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
	/// The accepted attributes for the column.
	/// </summary>
	public enum SqlColumnAttributes
	{
		/// <summary>
		/// No column attributes.
		/// </summary>
		None = 0,
		/// <summary>
		/// The column is part of the primary key.
		/// </summary>
		Key = 1,
		/// <summary>
		/// The column is an auto-increment key.
		/// </summary>
		AutoKey = (1 << 1) | Key,
		/// <summary>
		/// Ignores the column for selects.
		/// </summary>
		IgnoreSelect = 1 << 2,
		/// <summary>
		/// Ignores the column for inserts.
		/// </summary>
		IgnoreInsert = 1 << 3,
		/// <summary>
		/// Ignores the column for updates.
		/// </summary>
		IgnoreUpdate = 1 << 4,
		/// <summary>
		/// Ignores the column for inserts.
		/// </summary>
		IgnoreDelete = 1 << 5,
		/// <summary>
		/// Determines if the column must match the database on deletes.
		/// </summary>
		MatchDelete = 1 << 6,
		/// <summary>
		/// Determines if the column must matched the database on updates.
		/// </summary>
		MatchUpdate = 1 << 7,
		/// <summary>
		/// Prevents the column from being included in commands.
		/// </summary>
		NotMapped = IgnoreSelect | IgnoreInsert | IgnoreUpdate | IgnoreDelete,
		/// <summary>
		/// Determines if the column should be synchronized after inserts.
		/// </summary>
		InsertAutoSync = 1 << 8,
		/// <summary>
		/// Determines if the column should be synchronized after updates.
		/// </summary>
		UpdateAutoSync = 1 << 9,
	}
}
