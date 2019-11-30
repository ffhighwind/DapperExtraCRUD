using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Dapper.Extra.Utilities
{
	public interface ITransactionAccessObjectSync<T> where T : class
	{
		int BulkDelete(SqlTransaction transaction, IEnumerable<T> objs, int commandTimeout = 30);
		void BulkInsert(SqlTransaction transaction, IEnumerable<T> objs, int commandTimeout = 30);
		int BulkUpdate(SqlTransaction transaction, IEnumerable<T> objs, int commandTimeout = 30);
		int BulkUpsert(SqlTransaction transaction, IEnumerable<T> objs, int commandTimeout = 30);
		int BulkInsertIfNotExists(SqlTransaction transaction, IEnumerable<T> objs, int commandTimeout = 30);
		int BulkDelete(SqlTransaction transaction, IEnumerable<object> keys, int commandTimeout = 30);

		bool Delete(IDbTransaction transaction, T obj, int commandTimeout = 30);
		int Delete(IDbTransaction transaction, string whereCondition = "", object param = null, int commandTimeout = 30);
		T Get(IDbTransaction transaction, T obj, int commandTimeout = 30);
		IEnumerable<T> GetDistinct(IDbTransaction transaction, string whereCondition = "", object param = null, int commandTimeout = 30);
		IEnumerable<T> GetDistinct(IDbTransaction transaction, Type columnFilter, string whereCondition = "", object param = null, int commandTimeout = 30);
		IEnumerable<T> GetKeys(IDbTransaction transaction, string whereCondition = "", object param = null, int commandTimeout = 30);
		IEnumerable<T> GetList(IDbTransaction transaction, string whereCondition = "", object param = null, int commandTimeout = 30);
		IEnumerable<T> GetList(IDbTransaction transaction, Type columnFilter, string whereCondition = "", object param = null, int commandTimeout = 30);
		IEnumerable<T> GetLimit(IDbTransaction transaction, int limit, string whereCondition = "", object param = null, int commandTimeout = 30);
		IEnumerable<T> GetLimit(IDbTransaction transaction, Type columnFilter, int limit, string whereCondition = "", object param = null, int commandTimeout = 30);

		void Insert(IDbTransaction transaction, T obj, int commandTimeout = 30);
		int RecordCount(IDbTransaction transaction, string whereCondition = "", object param = null, int commandTimeout = 30);
		bool Update(IDbTransaction transaction, object obj, int commandTimeout = 30);
		bool Update(IDbTransaction transaction, T obj, int commandTimeout = 30);
		bool Upsert(IDbTransaction transaction, T obj, int commandTimeout = 30);
		bool InsertIfNotExists(IDbTransaction transaction, T obj, int commandTimeout = 30);
		IEnumerable<T> GetDistinctLimit(IDbTransaction transaction, int limit, string whereCondition = "", object param = null, int commandTimeout = 30);
		IEnumerable<T> GetDistinctLimit(IDbTransaction transaction, Type columnFilter, int limit, string whereCondition = "", object param = null, int commandTimeout = 30);

		T Get(IDbTransaction transaction, object key, int commandTimeout = 30);
		bool Delete(IDbTransaction transaction, object key, int commandTimeout = 30);
		IEnumerable<KeyType> GetKeys<KeyType>(IDbTransaction transaction, string whereCondition = "", object param = null, int commandTimeout = 30);
		List<T> BulkGet(SqlTransaction transaction, IEnumerable<T> keys, int commandTimeout = 30);
		List<T> BulkGet(SqlTransaction transaction, IEnumerable<object> keys, int commandTimeout = 30);
	}
}
