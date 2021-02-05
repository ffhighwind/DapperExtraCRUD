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
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;

namespace Dapper.Extra
{
	/// <summary>
	/// Delegate for <see cref="ISqlQueries{T}.DeleteKey"/>.
	/// </summary>
	/// <param name="connection">The connection to query on.</param>
	/// <param name="key">The key of the row.</param>
	/// <param name="transaction">The transaction to use for this query.</param>
	/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
	/// <returns>True a row was affected; false otherwise.</returns>
	public delegate bool DbKeyBool(IDbConnection connection, object key, IDbTransaction transaction = null, int commandTimeout = 30);

	/// <summary>
	/// Delegate for <see cref="ISqlQueries{T}.GetKey"/>.
	/// </summary>
	/// <typeparam name="T">The table type.</typeparam>
	/// <param name="connection">The connection to query on.</param>
	/// <param name="key">The key of the row.</param>
	/// <param name="transaction">The transaction to use for this query.</param>
	/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
	/// <returns>The row with the given key.</returns>
	public delegate T DbKeyObj<T>(IDbConnection connection, object key, IDbTransaction transaction = null, int commandTimeout = 30)
		where T : class;

	/// <summary>
	/// Delegate for <see cref="ISqlQueries{T}.BulkDeleteKeys"/>.
	/// </summary>
	/// <typeparam name="T">The table type.</typeparam>
	/// <param name="connection">The connection to query on.</param>
	/// <param name="keys">The key of the rows.</param>
	/// <param name="transaction">The transaction to use for this query.</param>
	/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
	/// <returns>The number rows affected.</returns>
	public delegate int DbKeysInt<T>(IDbConnection connection, IEnumerable<object> keys, IDbTransaction transaction = null, int commandTimeout = 30)
		where T : class;

	/// <summary>
	/// Delegate for <see cref="ISqlQueries{T}.BulkGetKeys"/>.
	/// </summary>
	/// <typeparam name="T">The table type.</typeparam>
	/// <param name="connection">The connection to query on.</param>
	/// <param name="keys">The keys of the rows.</param>
	/// <param name="transaction">The transaction to use for this query.</param>
	/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
	/// <returns>The rows with the given keys.</returns>
	public delegate List<T> DbKeysList<T>(IDbConnection connection, IEnumerable<object> keys, IDbTransaction transaction = null, int commandTimeout = 30)
		where T : class;

	/// <summary>
	/// Delegate for <see cref="ISqlQueries{T}.GetLimit"/>
	/// </summary>
	/// <typeparam name="T">The table type.</typeparam>
	/// <param name="connection">The connection to query on.</param>
	/// <param name="limit">The maximum number of rows.</param>
	/// <param name="whereCondition">The where condition to use for this query.</param>
	/// <param name="param">The parameters to use for this query.</param>
	/// <param name="transaction">The transaction to use for this query.</param>
	/// <param name="buffered">Whether to buffer the results in memory.</param>
	/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
	/// <returns>The rows that match the given condition.</returns>
	public delegate IEnumerable<T> DbLimitList<T>(IDbConnection connection, int limit, string whereCondition = "", object param = null, IDbTransaction transaction = null, bool buffered = true, int commandTimeout = 30)
		where T : class;

	/// <summary>
	/// Delegate for <see cref="ISqlQueries{T}.BulkUpsert"/>, <see cref="ISqlQueries{T}.BulkDelete"/>, <see cref="ISqlQueries{T}.BulkUpsert"/>, and <see cref="ISqlQueries{T}.BulkInsertIfNotExists"/>.
	/// </summary>
	/// <typeparam name="T">The table type.</typeparam>
	/// <param name="connection">The connection to query on.</param>
	/// <param name="objs">The objects.</param>
	/// <param name="transaction">The transaction to use for this query.</param>
	/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
	/// <returns>The number of rows affected.</returns>
	public delegate int DbListInt<T>(IDbConnection connection, IEnumerable<T> objs, IDbTransaction transaction = null, int commandTimeout = 30)
		where T : class;

	/// <summary>
	/// Delegate for <see cref="ISqlQueries{T}.BulkGet"/>.
	/// </summary>
	/// <typeparam name="T">The table type.</typeparam>
	/// <param name="connection">The connection to query on.</param>
	/// <param name="objs">The objects.</param>
	/// <param name="transaction">The transaction to use for this query.</param>
	/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
	/// <returns>The rows with the given keys.</returns>
	public delegate List<T> DbListList<T>(IDbConnection connection, IEnumerable<T> objs, IDbTransaction transaction = null, int commandTimeout = 30)
		where T : class;

	/// <summary>
	/// Delegate for <see cref="ISqlQueries{T}.BulkInsert"/>.
	/// </summary>
	/// <typeparam name="T">The table type.</typeparam>
	/// <param name="connection">The connection to query on.</param>
	/// <param name="objs">The objects.</param>
	/// <param name="transaction">The transaction to use for this query.</param>
	/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
	public delegate void DbListVoid<T>(IDbConnection connection, IEnumerable<T> objs, IDbTransaction transaction = null, int commandTimeout = 30)
		where T : class;

	/// <summary>
	/// Delegate for <see cref="ISqlQueries{T}.UpdateObj"/>.
	/// </summary>
	/// <typeparam name="T">The table type.</typeparam>
	/// <param name="connection">The connection to query on.</param>
	/// <param name="obj">The object.</param>
	/// <param name="transaction">The transaction to use for this query.</param>
	/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
	/// <returns>True if a row was affected; false otherwise.</returns>
	public delegate bool DbObjBool<T>(IDbConnection connection, object obj, IDbTransaction transaction = null, int commandTimeout = 30)
		where T : class;

	/// <summary>
	/// Delegate for <see cref="ISqlQueries{T}.Update"/>, <see cref="ISqlQueries{T}.Delete"/>, <see cref="ISqlQueries{T}.Upsert"/>, and <see cref="ISqlQueries{T}.InsertIfNotExists"/>.
	/// </summary>
	/// <typeparam name="T">The table type.</typeparam>
	/// <param name="connection">The connection to query on.</param>
	/// <param name="obj">The object.</param>
	/// <param name="transaction">The transaction to use for this query.</param>
	/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
	/// <returns>True if an object was affected; false otherwise.</returns>
	public delegate bool DbTBool<T>(IDbConnection connection, T obj, IDbTransaction transaction = null, int commandTimeout = 30)
		where T : class;

	/// <summary>
	/// Delegate for <see cref="ISqlQueries{T}.Get"/>.
	/// </summary>
	/// <typeparam name="T">The table type.</typeparam>
	/// <param name="connection">The connection to query on.</param>
	/// <param name="obj">The object.</param>
	/// <param name="transaction">The transaction to use for this query.</param>
	/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
	/// <returns>The row if it exists; otherwise null.</returns>
	public delegate T DbTT<T>(IDbConnection connection, T obj, IDbTransaction transaction = null, int commandTimeout = 30)
		where T : class;

	/// <summary>
	/// Delegate for <see cref="ISqlQueries{T}.Insert"/>, <see cref="ISqlQueries{T}.InsertAutoSync"/>, and <see cref="ISqlQueries{T}.UpdateAutoSync"/>.
	/// </summary>
	/// <typeparam name="T">The table type.</typeparam>
	/// <param name="connection">The connection to query on.</param>
	/// <param name="obj">The object.</param>
	/// <param name="transaction">The transaction to use for this query.</param>
	/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
	public delegate void DbTVoid<T>(IDbConnection connection, T obj, IDbTransaction transaction = null, int commandTimeout = 30)
		where T : class;

	/// <summary>
	/// Delegate for <see cref="ISqlQueries{T}.GetFilterLimit"/> and <see cref="ISqlQueries{T}.GetDistinctLimit"/>.
	/// </summary>
	/// <typeparam name="T">The table type.</typeparam>
	/// <param name="connection">The connection to query on.</param>
	/// <param name="limit">The maximum number of rows.</param>
	/// <param name="columnFilter">The type whose properties will filter the result.</param>
	/// <param name="whereCondition">The where condition to use for this query.</param>
	/// <param name="param">The parameters to use for this query.</param>
	/// <param name="transaction">The transaction to use for this query.</param>
	/// <param name="buffered">Whether to buffer the results in memory.</param>
	/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
	/// <returns>The rows that match the given condition.</returns>
	public delegate IEnumerable<T> DbTypeLimitList<T>(IDbConnection connection, int limit, Type columnFilter, string whereCondition = "", object param = null, IDbTransaction transaction = null, bool buffered = true, int commandTimeout = 30)
		where T : class;

	/// <summary>
	/// Delegate for <see cref="ISqlQueries{T}.GetFilter"/> and <see cref="ISqlQueries{T}.GetDistinct"/>.
	/// </summary>
	/// <typeparam name="T">The table type.</typeparam>
	/// <param name="connection">The connection to query on.</param>
	/// <param name="columnFilter">The type whose properties will filter the result.</param>
	/// <param name="whereCondition">The where condition to use for this query.</param>
	/// <param name="param">The parameters to use for this query.</param>
	/// <param name="transaction">The transaction to use for this query.</param>
	/// <param name="buffered">Whether to buffer the results in memory.</param>
	/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
	/// <returns>The rows that match the given condition.</returns>
	public delegate IEnumerable<T> DbTypeWhereList<T>(IDbConnection connection, Type columnFilter, string whereCondition = "", object param = null, IDbTransaction transaction = null, bool buffered = true, int commandTimeout = 30)
		where T : class;

	/// <summary>
	/// Delegate for <see cref="ISqlQueries{T}.Truncate"/>.
	/// </summary>
	/// <param name="connection">The connection to query on.</param>
	/// <param name="transaction">The transaction to use for this query.</param>
	/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
	public delegate void DbVoid(IDbConnection connection, IDbTransaction transaction = null, int commandTimeout = 30);

	/// <summary>
	/// Delegate for <see cref="ISqlQueries{T}.DeleteList"/> and <see cref="ISqlQueries{T}.RecordCount"/>.
	/// </summary>
	/// <typeparam name="T">The table type.</typeparam>
	/// <param name="connection">The connection to query on.</param>
	/// <param name="whereCondition">The where condition to use for this query.</param>
	/// <param name="param">The parameters to use for this query.</param>
	/// <param name="transaction">The transaction to use for this query.</param>
	/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
	/// <returns>The number of deleted rows.</returns>
	public delegate int DbWhereInt<T>(IDbConnection connection, string whereCondition = "", object param = null, IDbTransaction transaction = null, int commandTimeout = 30)
		where T : class;

	/// <summary>
	/// Delegate for <see cref="ISqlQueries{T}.GetKeysKeys"/>.
	/// </summary>
	/// <param name="connection">The connection to query on.</param>
	/// <param name="whereCondition">The where condition to use for this query.</param>
	/// <param name="param">The parameters to use for this query.</param>
	/// <param name="transaction">The transaction to use for this query.</param>
	/// <param name="buffered">Whether to buffer the results in memory.</param>
	/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
	/// <returns>The keys matching the condition.</returns>
	public delegate IEnumerable<object> DbWhereKeys(IDbConnection connection, string whereCondition = "", object param = null, IDbTransaction transaction = null, bool buffered = true, int commandTimeout = 30);

	/// <summary>
	/// Delegate for <see cref="ISqlQueries{T}.GetKeys"/> and <see cref="ISqlQueries{T}.GetList"/>.
	/// </summary>
	/// <typeparam name="T">The table type.</typeparam>
	/// <param name="connection">The connection to query on.</param>
	/// <param name="whereCondition">The where condition to use for this query.</param>
	/// <param name="param">The parameters to use for this query.</param>
	/// <param name="transaction">The transaction to use for this query.</param>
	/// <param name="buffered">Whether to buffer the results in memory.</param>
	/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
	/// <returns>The rows that match the given condition.</returns>
	public delegate IEnumerable<T> DbWhereList<T>(IDbConnection connection, string whereCondition = "", object param = null, IDbTransaction transaction = null, bool buffered = true, int commandTimeout = 30)
		where T : class;


	/// <summary>
	/// Delegate for <see cref="ISqlQueries{T}.DeleteListExpr"/> and <see cref="ISqlQueries{T}.RecordCountExpr"/>.
	/// </summary>
	/// <typeparam name="T">The table type.</typeparam>
	/// <param name="connection">The connection to query on.</param>
	/// <param name="whereExpr">The where condition to use for this query.</param>
	/// <param name="transaction">The transaction to use for this query.</param>
	/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
	/// <returns>The number of deleted rows.</returns>
	public delegate int DbWhereIntExpr<T>(IDbConnection connection, Expression<Func<T, bool>> whereExpr, IDbTransaction transaction = null, int commandTimeout = 30)
		where T : class;
}
