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

using System;
using System.Linq.Expressions;
using Dapper.Extra.Utilities;

namespace Dapper.Extra
{
	/// <summary>
	/// The SQL commands for a given type.
	/// </summary>
	/// <typeparam name="T">The table type.</typeparam>
	public interface ISqlQueries<T> where T : class
	{
		/// <summary>
		/// Compiles an expression using <see cref="WhereConditionGenerator"/> and stores the result.
		/// This may use a cache to prevent compilation.
		/// </summary>
		/// <param name="predicate">The predicate to use for the query.</param>
		/// <returns>The compiled result from passing the expression to <see cref="WhereConditionGenerator"/>.</returns>
		WhereConditionData<T> Compile(Expression<Func<T, bool>> predicate);

		/// <summary>
		/// Bulk delete command for any key type.
		/// </summary>
		DbListInt<T> BulkDelete { get; }

		/// <summary>
		/// Bulk delete command for single-value keys.
		/// </summary>
		DbKeysInt<T> BulkDeleteKeys { get; }

		/// <summary>
		/// Bulk select command for any key type.
		/// </summary>
		DbListList<T> BulkGet { get; }

		/// <summary>
		/// Bulk select command for single-value keys.
		/// </summary>
		DbKeysList<T> BulkGetKeys { get; }

		/// <summary>
		/// Bulk insert command for any key type.
		/// </summary>
		DbListVoid<T> BulkInsert { get; }

		/// <summary>
		/// Bulk insert-if-not-exists command.
		/// </summary>
		DbListInt<T> BulkInsertIfNotExists { get; }

		/// <summary>
		/// Bulk update command for any key type.
		/// </summary>
		DbListInt<T> BulkUpdate { get; }

		/// <summary>
		/// Bulk upsert (insert or update) command.
		/// </summary>
		DbListInt<T> BulkUpsert { get; }

		/// <summary>
		/// Delete command for any key type.
		/// </summary>
		DbTBool<T> Delete { get; }

		/// <summary>
		/// Delete command for a single key.
		/// </summary>
		DbKeyBool DeleteKey { get; }

		/// <summary>
		/// Delete command for a given condition.
		/// </summary>
		DbWhereInt<T> DeleteList { get; }

		/// <summary>
		/// Select command for any key type.
		/// </summary>
		DbTT<T> Get { get; }

		/// <summary>
		/// Select distinct command which is filtered to a subset of columns.
		/// </summary>
		DbTypeWhereList<T> GetDistinct { get; }

		/// <summary>
		/// Select distinct command which is filtered to a subset of columns and limited to a certain number of rows.
		/// </summary>
		DbTypeLimitList<T> GetDistinctLimit { get; }

		/// <summary>
		/// Select command which is filtered to a subset of columns.
		/// </summary>
		DbTypeWhereList<T> GetFilter { get; }

		/// <summary>
		/// Select command which is filtered to a subset of columns.
		/// </summary>
		DbTypeLimitList<T> GetFilterLimit { get; }

		/// <summary>
		/// Select command for a single key.
		/// </summary>
		DbKeyObj<T> GetKey { get; }

		/// <summary>
		/// Select command for a given condition.
		/// </summary>
		DbWhereList<T> GetKeys { get; }

		/// <summary>
		/// Select command for a given condition which returns single-value keys.
		/// </summary>
		DbWhereKeys GetKeysKeys { get; }

		/// <summary>
		/// Select command which is limited to a certain number of rows.
		/// </summary>
		DbLimitList<T> GetLimit { get; }

		/// <summary>
		/// Select command for a given condition.
		/// </summary>
		DbWhereList<T> GetList { get; }

		/// <summary>
		/// Insert command.
		/// </summary>
		DbTVoid<T> Insert { get; }

		/// <summary>
		/// Auto-sync command for inserts.
		/// </summary>
		DbTVoid<T> InsertAutoSync { get; }

		/// <summary>
		/// Insert-if-not-exists command.
		/// </summary>
		DbTBool<T> InsertIfNotExists { get; }

		/// <summary>
		/// Count command for a given condition.
		/// </summary>
		DbWhereInt<T> RecordCount { get; }

		/// <summary>
		/// Truncate command.
		/// </summary>
		DbVoid Truncate { get; }

		/// <summary>
		/// Update command.
		/// </summary>
		DbTBool<T> Update { get; }

		/// <summary>
		/// Auto-sync command for updates.
		/// </summary>
		DbTVoid<T> UpdateAutoSync { get; }

		/// <summary>
		/// Update command for a subset of columns.
		/// </summary>
		DbObjBool<T> UpdateObj { get; }

		/// <summary>
		/// Upsert (insert or update) command.
		/// </summary>
		DbTBool<T> Upsert { get; }
	}
}
