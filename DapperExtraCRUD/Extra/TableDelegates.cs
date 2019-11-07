using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.Extra.Interfaces;

namespace Dapper.Extra
{
	public class TableDelegates<T, KeyType> 
		where T : class
	{
		public delegate List<T> SqlKeysList(SqlConnection connection, IEnumerable<KeyType> keys, SqlTransaction transaction = null, int? commandTimeout = null);
		public delegate IEnumerable<KeyType> SqlKeysKeys(SqlConnection connection, IEnumerable<KeyType> keys, SqlTransaction transaction = null, bool buffered = true, int? commandTimeout = null);
		public delegate int SqlKeysInt(SqlConnection connection, IEnumerable<KeyType> keys, SqlTransaction transaction = null, int? commandTimeout = null);
		public delegate bool DbKeyBool(IDbConnection connection, KeyType key, IDbTransaction transaction = null, int? commandTimeout = null);
		public delegate T DbKeyObj(IDbConnection connection, KeyType key, IDbTransaction transaction = null, int? commandTimeout = null);
		public delegate IEnumerable<KeyType> DbWhereKeys(IDbConnection connection, string whereCondition = "", object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null);
	}

	public class TableDelegates<T>
		where T : class
	{
		public delegate int SqlListInt(SqlConnection connection, IEnumerable<T> objs, SqlTransaction transaction = null, int? commandTimeout = null);
		public delegate void SqlListVoid(SqlConnection connection, IEnumerable<T> objs, SqlTransaction transaction = null, int? commandTimeout = null);
		public delegate List<T> SqlListList(SqlConnection connection, IEnumerable<T> objs, SqlTransaction transaction = null, int? commandTimeout = null);
		//public delegate bool DbDictBool(IDbConnection connection, IDictionary<string, object> key, IDbTransaction transaction = null, int? commandTimeout = null);
		public delegate void DbObjVoid(IDbConnection connection, T obj, IDbTransaction transaction = null, int? commandTimeout = null);
		public delegate bool DbObjBool(IDbConnection connection, T obj, IDbTransaction transaction = null, int? commandTimeout = null);
		public delegate bool DbObjObjBool(IDbConnection connection, T obj, object filter = null, IDbTransaction transaction = null, int? commandTimeout = null);
		public delegate int DbWhereInt(IDbConnection connection, string whereCondition = "", object param = null, IDbTransaction transaction = null, int? commandTimeout = null);
		public delegate int DbWhereBufferedInt(IDbConnection connection, string whereCondition = "", object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null);
		public delegate IEnumerable<T> DbWhereList(IDbConnection connection, string whereCondition = "", object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null);
		public delegate IEnumerable<T> DbLimitList(IDbConnection connection, int limit, string whereCondition = "", object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null);
		//public delegate T DbDictObj(IDbConnection connection, IDictionary<string, object> key, IDbTransaction transaction = null, int? commandTimeout = null);
		public delegate T DbObjObj(IDbConnection connection, T obj, IDbTransaction transaction = null, int? commandTimeout = null);
	}
}
