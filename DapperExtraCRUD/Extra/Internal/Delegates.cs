using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Extra.Internal
{
	#region Keys
	public delegate List<T> DbKeysList<T, KeyType>(IDbConnection connection, IEnumerable<KeyType> keys, IDbTransaction transaction = null, int commandTimeout = 30)
		where T : class;
	public delegate List<T> SqlKeysList<T, KeyType>(SqlConnection connection, IEnumerable<KeyType> keys, SqlTransaction transaction = null, int commandTimeout = 30)
		where T : class;
	public delegate IEnumerable<KeyType> SqlKeysKeys<T, KeyType>(SqlConnection connection, IEnumerable<KeyType> keys, SqlTransaction transaction = null, bool buffered = true, int commandTimeout = 30)
		where T : class;
	public delegate int SqlKeysInt<T, KeyType>(SqlConnection connection, IEnumerable<KeyType> keys, SqlTransaction transaction = null, int commandTimeout = 30)
		where T : class;
	public delegate bool DbKeyBool<KeyType>(IDbConnection connection, KeyType key, IDbTransaction transaction = null, int commandTimeout = 30);
	public delegate T DbKeyObj<T, KeyType>(IDbConnection connection, KeyType key, IDbTransaction transaction = null, int commandTimeout = 30)
		where T : class;
	public delegate IEnumerable<KeyType> DbWhereKeys<KeyType>(IDbConnection connection, string whereCondition = "", object param = null, IDbTransaction transaction = null, bool buffered = true, int commandTimeout = 30);
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
	public delegate IEnumerable<T> DbWhereList<T>(IDbConnection connection, string whereCondition = "", object param = null, IDbTransaction transaction = null, bool buffered = true, int commandTimeout = 30)
		where T : class;
	public delegate IEnumerable<T> DbLimitList<T>(IDbConnection connection, int limit, string whereCondition = "", object param = null, IDbTransaction transaction = null, bool buffered = true, int commandTimeout = 30)
		where T : class;
	public delegate T DbTT<T>(IDbConnection connection, T obj, IDbTransaction transaction = null, int commandTimeout = 30)
		where T : class;
	#endregion T
}
