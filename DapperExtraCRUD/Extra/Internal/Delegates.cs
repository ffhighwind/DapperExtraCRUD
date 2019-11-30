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
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Dapper.Extra.Internal
{
	#region Keys
	public delegate List<T> DbKeysList<T>(IDbConnection connection, IEnumerable<object> keys, IDbTransaction transaction = null, int commandTimeout = 30)
		where T : class;
	public delegate List<T> SqlKeysList<T>(SqlConnection connection, IEnumerable<object> keys, SqlTransaction transaction = null, int commandTimeout = 30)
		where T : class;
	public delegate IEnumerable<object> SqlKeysKeys<T>(SqlConnection connection, IEnumerable<object> keys, SqlTransaction transaction = null, bool buffered = true, int commandTimeout = 30)
		where T : class;
	public delegate int SqlKeysInt<T>(SqlConnection connection, IEnumerable<object> keys, SqlTransaction transaction = null, int commandTimeout = 30)
		where T : class;
	public delegate bool DbKeyBool(IDbConnection connection, object key, IDbTransaction transaction = null, int commandTimeout = 30);
	public delegate T DbKeyObj<T>(IDbConnection connection, object key, IDbTransaction transaction = null, int commandTimeout = 30)
		where T : class;
	public delegate IEnumerable<object> DbWhereKeys(IDbConnection connection, string whereCondition = "", object param = null, IDbTransaction transaction = null, bool buffered = true, int commandTimeout = 30);
	#endregion Keys

	#region T
	public delegate void DbVoid(IDbConnection connection, IDbTransaction transaction = null, int commandTimeout = 30);
	public delegate int SqlListInt<T>(SqlConnection connection, IEnumerable<T> objs, SqlTransaction transaction = null, int commandTimeout = 30)
		where T : class;
	public delegate void SqlListVoid<T>(SqlConnection connection, IEnumerable<T> objs, SqlTransaction transaction = null, int commandTimeout = 30)
		where T : class;
	public delegate IEnumerable<T> SqlListList<T>(SqlConnection connection, IEnumerable<T> objs, SqlTransaction transaction = null, int commandTimeout = 30)
		where T : class;
	public delegate bool DbObjBool<T>(IDbConnection connection, object obj, IDbTransaction transaction = null, int commandTimeout = 30)
		where T : class;
	public delegate void DbTVoid<T>(IDbConnection connection, T obj, IDbTransaction transaction = null, int commandTimeout = 30)
		where T : class;
	public delegate bool DbTBool<T>(IDbConnection connection, T obj, IDbTransaction transaction = null, int commandTimeout = 30)
		where T : class;
	public delegate int DbWhereInt<T>(IDbConnection connection, string whereCondition = "", object param = null, IDbTransaction transaction = null, int commandTimeout = 30)
		where T : class;
	public delegate int DbWhereBufferedInt(IDbConnection connection, string whereCondition = "", object param = null, IDbTransaction transaction = null, bool buffered = true, int commandTimeout = 30);
	public delegate IEnumerable<T> DbTypeWhereList<T>(IDbConnection connection, Type columnFilter, string whereCondition = "", object param = null, IDbTransaction transaction = null, bool buffered = true, int commandTimeout = 30)
		where T : class;
	public delegate IEnumerable<T> DbWhereList<T>(IDbConnection connection, string whereCondition = "", object param = null, IDbTransaction transaction = null, bool buffered = true, int commandTimeout = 30)
		where T : class;
	public delegate IEnumerable<T> DbLimitList<T>(IDbConnection connection, int limit, string whereCondition = "", object param = null, IDbTransaction transaction = null, bool buffered = true, int commandTimeout = 30)
		where T : class;
	public delegate IEnumerable<T> DbTypeLimitList<T>(IDbConnection connection, Type columnFilter, int limit, string whereCondition = "", object param = null, IDbTransaction transaction = null, bool buffered = true, int commandTimeout = 30)
		where T : class;
	public delegate T DbTT<T>(IDbConnection connection, T obj, IDbTransaction transaction = null, int commandTimeout = 30)
		where T : class;
	#endregion T
}
