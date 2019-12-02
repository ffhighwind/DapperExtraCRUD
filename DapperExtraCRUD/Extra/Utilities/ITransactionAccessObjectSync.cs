using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Dapper.Extra.Utilities
{
	public interface ITransactionAccessObjectSync<T> where T : class
	{
		/// <summary>
		/// Deletes the rows with the given keys.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="keys">The keys for the rows to delete.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of deleted rows.</returns>
		int BulkDelete(IDbTransaction transaction, IEnumerable<object> keys, int commandTimeout = 30);

		/// <summary>
		/// Deletes the row with the given key.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="key">The key of the row to delete.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the row was deleted; false otherwise.</returns>
		bool Delete(IDbTransaction transaction, object key, int commandTimeout = 30);

		/// <summary>
		/// Selects the rows with the given keys.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="keys">The keys of the rows to select.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows with the given keys.</returns>
		IEnumerable<T> BulkGet(IDbTransaction transaction, IEnumerable<object> keys, int commandTimeout = 30);

		/// <summary>
		/// Selects the row with the given key.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="key">The key of the row to select.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The row with the given key.</returns>
		T Get(IDbTransaction transaction, object key, int commandTimeout = 30);

		/// <summary>
		/// Selects the rows with the given keys.
		/// </summary>
		/// <typeparam name="KeyType">The key type.</typeparam>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The keys that match the given condition.</returns>
		IEnumerable<KeyType> GetKeys<KeyType>(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int commandTimeout = 30);

		/// <summary>
		/// Selects the keys that match the given condition.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The keys that match the given condition.</returns>
		IEnumerable<T> GetKeys(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int commandTimeout = 30);

		/// <summary>
		/// Deletes the given row.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="obj">The object to delete.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the row was deleted; false otherwise.</returns>
		bool Delete(IDbTransaction transaction, T obj, int commandTimeout = 30);

		/// <summary>
		/// Deletes the given rows.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="objs">The objects to delete.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of deleted rows.</returns>
		int BulkDelete(IDbTransaction transaction, IEnumerable<T> objs, int commandTimeout = 30);

		/// <summary>
		/// Deletes the rows that match the given condition.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of deleted rows.</returns>
		int DeleteList(IDbTransaction transaction, string whereCondition = "", object param = null, int commandTimeout = 30);

		/// <summary>
		/// Inserts a row.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="obj">The object to insert.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		void Insert(IDbTransaction transaction, T obj, int commandTimeout = 30);

		/// <summary>
		/// Inserts the given rows.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="objs">The objects to insert.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		void BulkInsert(IDbTransaction transaction, IEnumerable<T> objs, int commandTimeout = 30);

		/// <summary>
		/// Inserts a row if it does not exist.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="obj">The object to insert.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the the row was inserted; false otherwise.</returns>
		bool InsertIfNotExists(IDbTransaction transaction, T obj, int commandTimeout = 30);

		/// <summary>
		/// Inserts the given rows if they do not exist.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="objs">The objects to insert.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of rows inserted.</returns>
		int BulkInsertIfNotExists(IDbTransaction transaction, IEnumerable<T> objs, int commandTimeout = 30);

		/// <summary>
		/// Updates a row.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="obj">The object to update.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the row was updated; false otherwise.</returns>
		bool Update(IDbTransaction transaction, object obj, int commandTimeout = 30);

		/// <summary>
		/// Updates a row.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="obj">The object to update.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the row was updated; false otherwise.</returns>
		bool Update(IDbTransaction transaction, T obj, int commandTimeout = 30);

		/// <summary>
		/// Updates the given rows.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="objs">The objects to update.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of updated rows.</returns>
		int BulkUpdate(IDbTransaction transaction, IEnumerable<T> objs, int commandTimeout = 30);

		/// <summary>
		/// Upserts a row.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="obj">The object to upsert.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the object was upserted; false otherwise.</returns>
		bool Upsert(IDbTransaction transaction, T obj, int commandTimeout = 30);

		/// <summary>
		/// Upserts the given rows.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="objs">The objects to upsert.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of upserted rows.</returns>
		int BulkUpsert(IDbTransaction transaction, IEnumerable<T> objs, int commandTimeout = 30);

		/// <summary>
		/// Selects a row.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="obj">The object to select.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The selected row if it exists; otherwise null.</returns>
		T Get(IDbTransaction transaction, T obj, int commandTimeout = 30);

		/// <summary>
		/// Selects the rows with the given keys.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="objs">The objects to select.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given keys.</returns>
		IEnumerable<T> BulkGet(IDbTransaction transaction, IEnumerable<T> objs, int commandTimeout = 30);

		/// <summary>
		/// Selects the rows that match the given condition.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		IEnumerable<T> GetList(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int commandTimeout = 30);

		/// <summary>
		/// Selects the rows that match the given condition.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		IEnumerable<T> GetList(IDbTransaction transaction, Type columnFilter, string whereCondition = "", object param = null, bool buffered = true, int commandTimeout = 30);

		/// <summary>
		/// Selects a limited number of rows that match the given condition.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="limit">The maximum number of rows.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The limited number of rows that match the given condition.</returns>
		IEnumerable<T> GetLimit(IDbTransaction transaction, int limit, string whereCondition = "", object param = null, bool buffered = true, int commandTimeout = 30);

		/// <summary>
		/// Selects a limited number of rows that match the given condition.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="limit">The maximum number of rows.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>A limited number of rows that match the given condition.</returns>
		IEnumerable<T> GetLimit(IDbTransaction transaction, Type columnFilter, int limit, string whereCondition = "", object param = null, bool buffered = true, int commandTimeout = 30);

		/// <summary>
		/// Selects the rows that match the given condition.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		IEnumerable<T> GetDistinct(IDbTransaction transaction, string whereCondition = "", object param = null, bool buffered = true, int commandTimeout = 30);

		/// <summary>
		/// Selects the rows that match the given condition.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		IEnumerable<T> GetDistinct(IDbTransaction transaction, Type columnFilter, string whereCondition = "", object param = null, bool buffered = true, int commandTimeout = 30);

		/// <summary>
		/// Selects the rows that match the given condition.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="limit">The maximum number of rows.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		IEnumerable<T> GetDistinctLimit(IDbTransaction transaction, int limit, string whereCondition = "", object param = null, bool buffered = true, int commandTimeout = 30);

		/// <summary>
		/// Selects the rows that match the given condition.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="limit">The maximum number of rows.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		IEnumerable<T> GetDistinctLimit(IDbTransaction transaction, Type columnFilter, int limit, string whereCondition = "", object param = null, bool buffered = true, int commandTimeout = 30);

		/// <summary>
		/// Counts the number of rows that match the given condition.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of rows that match the given condition.</returns>
		int RecordCount(IDbTransaction transaction, string whereCondition = "", object param = null, int commandTimeout = 30);

		/// <summary>
		/// Truncates all rows.
		/// </summary>
		/// <param name="transaction">The transaction to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		void Truncate(IDbTransaction transaction, int commandTimeout = 30);
	}
}