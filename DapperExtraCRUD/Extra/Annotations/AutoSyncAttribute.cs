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
	/// Synchronizes the column with the database on updates/inserts.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class AutoSyncAttribute : Attribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AutoSyncAttribute"/> attribute.
		/// </summary>
		/// <param name="syncInsert">Determines if this column will be automatically selected after an insert.</param>
		/// <param name="syncUpdate">Determines if this column will be automatically selected after an update.</param>
		public AutoSyncAttribute(bool syncInsert = true, bool syncUpdate = true)
		{
			SyncInsert = syncInsert;
			SyncUpdate = syncUpdate;
		}

		/// <summary>
		/// Determines if this column will be automatically selected after an insert.
		/// </summary>
		public bool SyncInsert { get; }

		/// <summary>
		/// Determines if this column will be automatically selected after an update.
		/// </summary>
		public bool SyncUpdate { get; }
	}
}
