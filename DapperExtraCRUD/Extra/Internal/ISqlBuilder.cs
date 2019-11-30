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

using System.Collections.Generic;

namespace Dapper.Extra.Internal
{
	public interface ISqlBuilder
	{
		/// <summary>
		/// The quoted table name or the class name.
		/// </summary>
		string TableName { get; }
		/// <summary>
		/// The temporary table name for bulk operations.
		/// </summary>
		string BulkStagingTable { get; }
		/// <summary>
		/// The syntax used to generate SQL commands.
		/// </summary>
		SqlSyntax Syntax { get; }
		/// <summary>
		/// Stores metadata for for the given type.
		/// </summary>
		SqlTypeInfo Info { get; }
		/// <summary>
		///  All valid columns for the given type.
		/// </summary>
		IReadOnlyList<SqlColumn> Columns { get; }
	}
}
