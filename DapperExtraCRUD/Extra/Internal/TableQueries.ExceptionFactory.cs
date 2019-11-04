using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Extra
{
	public partial class TableQueries<T>
		where T : class
	{
		internal class ExceptionFactory
		{
			private readonly Exception e;

			internal ExceptionFactory(Exception ex)
			{
				e = ex;
			}

			public TableQueries<T> Create()
			{
				TableQueries<T> queries = new TableQueries<T>()
				{
					BulkDelete = SqlListInt,
					BulkInsert = SqlListVoid,
					BulkUpdate = SqlListInt,
					BulkUpsert = SqlListInt,
					Delete = DbObjBool,
					DeleteWhere = DbWhereInt,
					GetDistinct = DbWhereList,
					Get = DbObjObj,
					GetKeys = DbWhereList,
					GetList = DbWhereList,
					GetLimit = DbLimitList,
					Insert = DbObjObj,
					RecordCount = DbWhereInt,
					Update = DbObjBool,
					Upsert = DbObjObj,
					GetDistinctLimit = DbLimitList,
				};
				return queries;
			}

			private int SqlListInt(SqlConnection connection, IEnumerable<T> objs, SqlTransaction transaction = null, int? commandTimeout = null)
			{
				DoException();
				throw e;
			}

			private void SqlListVoid(SqlConnection connection, IEnumerable<T> objs, SqlTransaction transaction = null, int? commandTimeout = null)
			{
				DoException();
				throw e;
			}

			private bool DbObjBool(IDbConnection connection, T obj, IDbTransaction transaction = null, int? commandTimeout = null)
			{
				DoException();
				throw e;
			}

			private int DbWhereInt(IDbConnection connection, string whereCondition = "", object param = null, IDbTransaction transaction = null, int? commandTimeout = null)
			{
				DoException();
				throw e;
			}

			private IEnumerable<T> DbWhereList(IDbConnection connection, string whereCondition = "", object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null)
			{
				DoException();
				throw e;
			}

			private IEnumerable<T> DbLimitList(IDbConnection connection, int limit, string whereCondition = "", object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null)
			{
				DoException();
				throw e;
			}

			private T DbObjObj(IDbConnection connection, T obj, IDbTransaction transaction = null, int? commandTimeout = null)
			{
				DoException();
				throw e;
			}

			private void DoException()
			{
				TableFactory<T> factory = TableFactory.Create<T>();
				factory.Create();
			}
		}
	}


	public partial class TableQueries<T, KeyType>
		where T : class
	{
		internal class ExceptionFactory
		{
			private readonly Exception e;

			public ExceptionFactory(Exception ex)
			{
				e = ex;
			}

			public TableQueries<T, KeyType> Create()
			{
				TableQueries<T, KeyType> queries = new TableQueries<T, KeyType>()
				{
					BulkDelete = SqlKeysInt,
					GetKeys = DbWhereKeys,
					Delete = DbKeyBool,
					Get = DbKeyObj,
				};
				return queries;
			}

			private int SqlKeysInt(SqlConnection connection, IEnumerable<KeyType> keys, SqlTransaction transaction = null, int? commandTimeout = null)
			{
				DoException();
				throw e;
			}

			private bool DbKeyBool(IDbConnection connection, KeyType key, IDbTransaction transaction = null, int? commandTimeout = null)
			{
				DoException();
				throw e;
			}

			private T DbKeyObj(IDbConnection connection, KeyType key, IDbTransaction transaction = null, int? commandTimeout = null)
			{
				DoException();
				throw e;
			}

			private IEnumerable<KeyType> DbWhereKeys(IDbConnection connection, string whereCondition = "", object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null)
			{
				DoException();
				throw e;
			}

			private void DoException()
			{
				TableFactory<T> factory = TableFactory.Create<T>();
				factory.Create();
				factory.Create<KeyType>();
			}
		}
	}
}