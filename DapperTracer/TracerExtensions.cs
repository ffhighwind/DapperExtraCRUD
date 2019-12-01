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
using System.Data.Common;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace Dapper
{
	public static class TracerExtensions
	{
		public static IEnumerable<object> Query(this IDbConnection cnn, Type type, string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
		{
			TRACE(sql);
			return SqlMapper.Query(cnn, type, sql, param, transaction, buffered, commandTimeout, commandType);
		}

		public static IEnumerable<T> Query<T>(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
		{
			TRACE(sql);
			return SqlMapper.Query<T>(cnn, sql, param, transaction, buffered, commandTimeout, commandType);
		}

		public static IEnumerable<TReturn> Query<TReturn>(this IDbConnection cnn, string sql, Type[] types, Func<object[], TReturn> map, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
		{
			TRACE(sql);
			return SqlMapper.Query<TReturn>(cnn, sql, types, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
		}

		public static IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(this IDbConnection cnn, string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
		{
			TRACE(sql);
			return SqlMapper.Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(cnn, sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
		}

		public static IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(this IDbConnection cnn, string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
		{
			TRACE(sql);
			return SqlMapper.Query<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(cnn, sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
		}

		public static IEnumerable<TReturn> Query<TFirst, TSecond, TReturn>(this IDbConnection cnn, string sql, Func<TFirst, TSecond, TReturn> map, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
		{
			TRACE(sql);
			return SqlMapper.Query<TFirst, TSecond, TReturn>(cnn, sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
		}

		public static IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(this IDbConnection cnn, string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
		{
			TRACE(sql);
			return SqlMapper.Query<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(cnn, sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
		}

		public static IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TFourth, TReturn>(this IDbConnection cnn, string sql, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
		{
			TRACE(sql);
			return SqlMapper.Query<TFirst, TSecond, TThird, TFourth, TReturn>(cnn, sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
		}

		//[return: Dynamic(new[] { false, true })]
		public static IEnumerable<dynamic> Query(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
		{
			TRACE(sql);
			return SqlMapper.Query(cnn, sql, param, transaction, buffered, commandTimeout, commandType);
		}

		public static IEnumerable<TReturn> Query<TFirst, TSecond, TThird, TReturn>(this IDbConnection cnn, string sql, Func<TFirst, TSecond, TThird, TReturn> map, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
		{
			TRACE(sql);
			return SqlMapper.Query<TFirst, TSecond, TThird, TReturn>(cnn, sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
		}

		public static Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TReturn>(this IDbConnection cnn, string sql, Func<TFirst, TSecond, TReturn> map, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
		{
			TRACE(sql);
			return SqlMapper.QueryAsync<TFirst, TSecond, TReturn>(cnn, sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
		}

		//[return: Dynamic(new[] { false, false, true })]
		public static Task<IEnumerable<dynamic>> QueryAsync(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			TRACE(sql);
			return SqlMapper.QueryAsync(cnn, sql, param, transaction, commandTimeout, commandType);
		}

		public static Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TReturn>(this IDbConnection cnn, string sql, Func<TFirst, TSecond, TThird, TReturn> map, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
		{
			TRACE(sql);
			return SqlMapper.QueryAsync<TFirst, TSecond, TThird, TReturn>(cnn, sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
		}

		public static Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TReturn>(this IDbConnection cnn, string sql, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
		{
			TRACE(sql);
			return SqlMapper.QueryAsync<TFirst, TSecond, TThird, TFourth, TReturn>(cnn, sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
		}

		public static Task<IEnumerable<T>> QueryAsync<T>(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			TRACE(sql);
			return SqlMapper.QueryAsync<T>(cnn, sql, param, transaction, commandTimeout, commandType);
		}

		public static Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(this IDbConnection cnn, string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
		{
			TRACE(sql);
			return SqlMapper.QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(cnn, sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
		}

		public static Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(this IDbConnection cnn, string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
		{
			TRACE(sql);
			return SqlMapper.QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(cnn, sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
		}

		public static Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(this IDbConnection cnn, string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
		{
			TRACE(sql);
			return SqlMapper.QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(cnn, sql, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
		}

		public static Task<IEnumerable<TReturn>> QueryAsync<TReturn>(this IDbConnection cnn, string sql, Type[] types, Func<object[], TReturn> map, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, CommandType? commandType = null)
		{
			TRACE(sql);
			return SqlMapper.QueryAsync<TReturn>(cnn, sql, types, map, param, transaction, buffered, splitOn, commandTimeout, commandType);
		}

		public static Task<IEnumerable<object>> QueryAsync(this IDbConnection cnn, Type type, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			TRACE(sql);
			return SqlMapper.QueryAsync(cnn, type, sql, param, transaction = null, commandTimeout, commandType);
		}

		public static object QueryFirst(this IDbConnection cnn, Type type, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			TRACE(sql);
			return SqlMapper.QueryFirst(cnn, type, sql, param, transaction, commandTimeout, commandType);
		}

		public static T QueryFirst<T>(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			TRACE(sql);
			return SqlMapper.QueryFirst<T>(cnn, sql, param, transaction, commandTimeout, commandType);
		}

		//[return: Dynamic]
		public static dynamic QueryFirst(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			TRACE(sql);
			return SqlMapper.QueryFirst(cnn, sql, param, transaction, commandTimeout, commandType);
		}

		//[return: Dynamic(new[] { false, true })]
		public static Task<dynamic> QueryFirstAsync(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			TRACE(sql);
			return SqlMapper.QueryFirstAsync(cnn, sql, param, transaction, commandTimeout, commandType);
		}

		public static Task<T> QueryFirstAsync<T>(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			TRACE(sql);
			return SqlMapper.QueryFirstAsync<T>(cnn, sql, param, transaction, commandTimeout, commandType);
		}

		public static Task<object> QueryFirstAsync(this IDbConnection cnn, Type type, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			TRACE(sql);
			return SqlMapper.QueryFirstAsync(cnn, type, sql, param, transaction, commandTimeout, commandType);
		}

		public static T QueryFirstOrDefault<T>(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			TRACE(sql);
			return SqlMapper.QueryFirstOrDefault<T>(cnn, sql, param, transaction, commandTimeout, commandType);
		}

		//[return: Dynamic]
		public static dynamic QueryFirstOrDefault(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			TRACE(sql);
			return SqlMapper.QueryFirstOrDefault(cnn, sql, param, transaction, commandTimeout, commandType);
		}

		public static object QueryFirstOrDefault(this IDbConnection cnn, Type type, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			TRACE(sql);
			return SqlMapper.QueryFirstOrDefault(cnn, type, sql, param, transaction, commandTimeout, commandType);
		}

		//[return: Dynamic(new[] { false, true })]
		public static Task<dynamic> QueryFirstOrDefaultAsync(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			TRACE(sql);
			return SqlMapper.QueryFirstOrDefaultAsync(cnn, sql, param, transaction, commandTimeout, commandType);
		}

		public static Task<T> QueryFirstOrDefaultAsync<T>(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			TRACE(sql);
			return SqlMapper.QueryFirstOrDefaultAsync<T>(cnn, sql, param, transaction, commandTimeout, commandType);
		}

		public static Task<object> QueryFirstOrDefaultAsync(this IDbConnection cnn, Type type, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			TRACE(sql);
			return SqlMapper.QueryFirstOrDefaultAsync(cnn, type, sql, param, transaction, commandTimeout, commandType);
		}

		public static GridReader QueryMultiple(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			TRACE(sql);
			return SqlMapper.QueryMultiple(cnn, sql, param, transaction, commandTimeout, commandType);
		}

		public static Task<GridReader> QueryMultipleAsync(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			TRACE(sql);
			return SqlMapper.QueryMultipleAsync(cnn, sql, param, transaction, commandTimeout, commandType);
		}

		public static object QuerySingle(this IDbConnection cnn, Type type, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			TRACE(sql);
			return SqlMapper.QuerySingle(cnn, type, sql, param, transaction, commandTimeout, commandType);
		}

		public static T QuerySingle<T>(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			TRACE(sql);
			return SqlMapper.QuerySingle<T>(cnn, sql, param, transaction, commandTimeout, commandType);
		}

		//[return: Dynamic]
		public static dynamic QuerySingle(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			TRACE(sql);
			return SqlMapper.QuerySingle(cnn, sql, param, transaction, commandTimeout, commandType);
		}

		//[return: Dynamic(new[] { false, true })]
		public static Task<dynamic> QuerySingleAsync(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			TRACE(sql);
			return SqlMapper.QuerySingleAsync(cnn, sql, param, transaction, commandTimeout, commandType);
		}

		public static Task<object> QuerySingleAsync(this IDbConnection cnn, Type type, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			TRACE(sql);
			return SqlMapper.QuerySingleAsync(cnn, type, sql, param, transaction, commandTimeout, commandType);
		}

		public static Task<T> QuerySingleAsync<T>(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			TRACE(sql);
			return SqlMapper.QuerySingleAsync<T>(cnn, sql, param, transaction, commandTimeout, commandType);
		}

		//[return: Dynamic]
		public static dynamic QuerySingleOrDefault(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			TRACE(sql);
			return SqlMapper.QuerySingleOrDefault(cnn, sql, param, transaction, commandTimeout, commandType);
		}

		public static T QuerySingleOrDefault<T>(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			TRACE(sql);
			return SqlMapper.QuerySingleOrDefault<T>(cnn, sql, param, transaction, commandTimeout, commandType);
		}

		public static object QuerySingleOrDefault(this IDbConnection cnn, Type type, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			TRACE(sql);
			return SqlMapper.QuerySingleOrDefault(cnn, type, sql, param, transaction, commandTimeout, commandType);
		}

		public static Task<object> QuerySingleOrDefaultAsync(this IDbConnection cnn, Type type, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			TRACE(sql);
			return SqlMapper.QuerySingleOrDefaultAsync(cnn, type, sql, param, transaction, commandTimeout, commandType);
		}

		//[return: Dynamic(new[] { false, true })]
		public static Task<dynamic> QuerySingleOrDefaultAsync(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			TRACE(sql);
			return SqlMapper.QuerySingleOrDefaultAsync(cnn, sql, param, transaction, commandTimeout, commandType);
		}

		public static Task<T> QuerySingleOrDefaultAsync<T>(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			TRACE(sql);
			return SqlMapper.QuerySingleOrDefaultAsync<T>(cnn, sql, param, transaction, commandTimeout, commandType);
		}

		public static int Execute(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			TRACE(sql);
			return SqlMapper.Execute(cnn, sql, param, transaction, commandTimeout, commandType);
		}

		public static Task<int> ExecuteAsync(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			TRACE(sql);
			return SqlMapper.ExecuteAsync(cnn, sql, param, transaction, commandTimeout, commandType);
		}

		public static IDataReader ExecuteReader(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			TRACE(sql);
			return SqlMapper.ExecuteReader(cnn, sql, param, transaction, commandTimeout, commandType);
		}

		public static Task<DbDataReader> ExecuteReaderAsync(this DbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			TRACE(sql);
			return SqlMapper.ExecuteReaderAsync(cnn, sql, param, transaction, commandTimeout, commandType);
		}

		public static Task<IDataReader> ExecuteReaderAsync(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			TRACE(sql);
			return SqlMapper.ExecuteReaderAsync(cnn, sql, param, transaction, commandTimeout, commandType);
		}

		public static object ExecuteScalar(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			TRACE(sql);
			return SqlMapper.ExecuteScalar(cnn, sql, param, transaction, commandTimeout, commandType);
		}

		public static T ExecuteScalar<T>(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			TRACE(sql);
			return SqlMapper.ExecuteScalar<T>(cnn, sql, param, transaction, commandTimeout, commandType);
		}

		public static Task<T> ExecuteScalarAsync<T>(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			TRACE(sql);
			return SqlMapper.ExecuteScalarAsync<T>(cnn, sql, param, transaction, commandTimeout, commandType);
		}

		public static Task<object> ExecuteScalarAsync(this IDbConnection cnn, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
		{
			TRACE(sql);
			return SqlMapper.ExecuteScalarAsync(cnn, sql, param, transaction, commandTimeout, commandType);
		}

		public static List<T> AsList<T>(this IEnumerable<T> source)
		{
			return SqlMapper.AsList(source);
		}

		private static void TRACE(string sql)
		{
#if DEBUG
			Console.WriteLine(sql);
			Console.WriteLine();
			// add an extra newline if there isn't one at the end of sql
			for (int i = sql.Length - 1; i >= 0; i--) {
				if (sql[i] == ' ')
					continue;
				if (sql[i] == '\r' || sql[i] == '\n')
					break;
				Console.WriteLine();
				break;
			}
#endif
		}
	}
}
