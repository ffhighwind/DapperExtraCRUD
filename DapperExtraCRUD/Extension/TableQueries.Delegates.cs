using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.Extension.Interfaces;

namespace Dapper.Extension
{
	public partial class TableQueries<T, KeyType> where T : class
	{
		public static class Delegates
		{
			public delegate IEnumerable<KeyType> SqlKeysKeys(SqlConnection connection, IEnumerable<KeyType> keys, SqlTransaction transaction = null, bool buffered = true, int? commandTimeout = null);
			public delegate int SqlKeysInt(SqlConnection connection, IEnumerable<KeyType> keys, SqlTransaction transaction = null, int? commandTimeout = null);

			public delegate bool SqlKeyBool(IDbConnection connection, KeyType key, IDbTransaction transaction = null, int? commandTimeout = null);
			public delegate T SqlKeyObj(IDbConnection connection, KeyType key, IDbTransaction transaction = null, int? commandTimeout = null);
			public delegate IEnumerable<KeyType> DbWhereKeys(IDbConnection connection, string whereCondition = "", object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null);
		}
	}

	public partial class TableQueries<T> where T : class
	{
		public static class Delegates
		{
			public delegate int SqlListInt(SqlConnection connection, IEnumerable<T> objs, SqlTransaction transaction = null, int? commandTimeout = null);
			public delegate IEnumerable<T> SqlListList(SqlConnection connection, IEnumerable<T> objs, SqlTransaction transaction = null, bool buffered = true, int? commandTimeout = null);

			public delegate bool DbDictBool(IDbConnection connection, IDictionary<string, object> key, IDbTransaction transaction = null, int? commandTimeout = null);
			public delegate bool DbObjBool(IDbConnection connection, T obj, IDbTransaction transaction = null, int? commandTimeout = null);
			public delegate int DbWhereInt(IDbConnection connection, string whereCondition = "", object param = null, IDbTransaction transaction = null, int? commandTimeout = null);
			public delegate int DbWhereBufferedInt(IDbConnection connection, string whereCondition = "", object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null);
			public delegate IEnumerable<T> DbWhereList(IDbConnection connection, string whereCondition = "", object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null);
			public delegate IEnumerable<T> DbLimitList(IDbConnection connection, int limit, string whereCondition = "", object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null);
			public delegate T DbDictObj(IDbConnection connection, IDictionary<string, object> key, IDbTransaction transaction = null, int? commandTimeout = null);
			public delegate T DbObjObj(IDbConnection connection, T obj, IDbTransaction transaction = null, int? commandTimeout = null);
		}
	}
}
