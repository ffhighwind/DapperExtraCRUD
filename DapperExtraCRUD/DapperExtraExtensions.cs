using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Dapper.Extra;
using Dapper.Extra.Utilities;

namespace Dapper
{
	public static class DapperExtraExtensions
	{
		#region Delegates <T, KeyType> Sync
		public static int BulkDelete<T, KeyType>(this SqlConnection connection, IEnumerable<KeyType> keys, SqlTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			int count = ExtraCrud.Queries<T, KeyType>().BulkDelete(transaction.Connection, keys, transaction, commandTimeout);
			return count;
		}

		public static bool Delete<T, KeyType>(this IDbConnection connection, KeyType key, IDbTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			bool success = ExtraCrud.Queries<T, KeyType>().Delete(transaction.Connection, key, transaction, commandTimeout);
			return success;
		}

		public static T Get<T, KeyType>(this IDbConnection connection, KeyType key, IDbTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			T value = ExtraCrud.Queries<T, KeyType>().Get(transaction.Connection, key, transaction, commandTimeout);
			return value;
		}

		public static IEnumerable<KeyType> GetKeys<T, KeyType>(this IDbConnection connection, string whereCondition = "", object param = null, IDbTransaction transaction = null, bool buffered = true, int commandTimeout = 30)
			where T : class
		{
			IEnumerable<KeyType> keys = ExtraCrud.Queries<T, KeyType>().GetKeys(transaction.Connection, whereCondition, param, transaction, buffered, commandTimeout);
			return keys;
		}
		#endregion Delegates <T, KeyType> Sync

		#region Delegates <T, KeyType> Async
		public static async Task<int> BulkDeleteAsync<T, KeyType>(this SqlConnection connection, IEnumerable<KeyType> keys, SqlTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			return await Task.Run(() => BulkDelete<T, KeyType>(connection, keys, transaction, commandTimeout));
		}

		public static async Task<bool> DeleteAsync<T, KeyType>(this IDbConnection connection, KeyType key, IDbTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			return await Task.Run(() => Delete<T, KeyType>(connection, key, transaction, commandTimeout));
		}

		public static async Task<T> GetAsync<T, KeyType>(this IDbConnection connection, KeyType key, IDbTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			return await Task.Run(() => Get<T, KeyType>(connection, key, transaction, commandTimeout));
		}

		public static async Task<IEnumerable<KeyType>> GetKeysAsync<T, KeyType>(this IDbConnection connection, string whereCondition = "", object param = null, IDbTransaction transaction = null, bool buffered = true, int commandTimeout = 30)
			where T : class
		{
			return await Task.Run(() => GetKeys<T, KeyType>(connection, whereCondition, param, transaction, buffered, commandTimeout));
		}

		#endregion Delegates <T, KeyType> Async

		#region Delegates <T> Sync
		/// <summary>
		/// Returns a sequence of keys that match the given condition.
		/// </summary>
		/// <typeparam name="T">The <see cref="Type"/> that defines the table.</typeparam>
		/// <param name="connection">The connection to query on.</param>
		/// <param name="whereCondition"></param>
		/// <param name="param">The parameters to pass, if any.</param>
		/// <param name="transaction">The transaction to use, if any.</param>
		/// <param name="buffered">Whether to buffer the results in memory.</param>
		/// <param name="commandTimeout">The command timeout (in seconds).</param>
		/// <returns>The keys from a select statement using the given where condition.</returns>
		public static IEnumerable<T> GetKeys<T>(this IDbConnection connection, string whereCondition = "", object param = null, IDbTransaction transaction = null, bool buffered = true, int commandTimeout = 30)
			where T : class
		{
			IEnumerable<T> keys = ExtraCrud.Queries<T>().GetKeys(connection, whereCondition, param, transaction, buffered, commandTimeout);
			return keys;
		}

		public static bool Delete<T>(this IDbConnection connection, T obj, IDbTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			bool success = ExtraCrud.Queries<T>().Delete(connection, obj, transaction, commandTimeout);
			return success;
		}

		public static int BulkDelete<T>(this SqlConnection connection, IEnumerable<T> objs, SqlTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			int count = ExtraCrud.Queries<T>().BulkDelete(connection, objs, transaction, commandTimeout);
			return count;
		}

		public static int Delete<T>(this IDbConnection connection, string whereCondition = "", object param = null, IDbTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			int count = ExtraCrud.Queries<T>().DeleteWhere(connection, whereCondition, param, transaction, commandTimeout);
			return count;
		}

		public static void Insert<T>(this IDbConnection connection, T obj, IDbTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			ExtraCrud.Queries<T>().Insert(connection, obj, transaction, commandTimeout);
		}

		public static void BulkInsert<T>(this SqlConnection connection, IEnumerable<T> objs, SqlTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			ExtraCrud.Queries<T>().BulkInsert(connection, objs, transaction, commandTimeout);
		}

		public static bool Update<T>(this IDbConnection connection, object obj, IDbTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			bool success = ExtraCrud.Queries<T>().UpdateObj(connection, obj, transaction, commandTimeout);
			return success;
		}

		public static bool Update<T>(this IDbConnection connection, T obj)
			where T : class
		{
			bool success = ExtraCrud.Queries<T>().Update(connection, obj);
			return success;
		}

		public static int BulkUpdate<T>(this SqlConnection connection, IEnumerable<T> objs, SqlTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			int count = ExtraCrud.Queries<T>().BulkUpdate(connection, objs, transaction, commandTimeout);
			return count;
		}

		public static bool Upsert<T>(this IDbConnection connection, T obj, IDbTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			bool updated = ExtraCrud.Queries<T>().Upsert(connection, obj, transaction, commandTimeout);
			return updated;
		}

		public static int BulkUpsert<T>(this SqlConnection connection, IEnumerable<T> objs, SqlTransaction transaction = null, bool buffered = true, int commandTimeout = 30)
			where T : class
		{
			int count = ExtraCrud.Queries<T>().BulkUpsert(connection, objs, transaction, commandTimeout);
			return count;
		}

		public static T Get<T>(this IDbConnection connection, T obj, IDbTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			T result = ExtraCrud.Queries<T>().Get(connection, obj, transaction, commandTimeout);
			return result;
		}

		public static IEnumerable<T> GetList<T>(this IDbConnection connection, string whereCondition = "", object param = null, IDbTransaction transaction = null, bool buffered = true, int commandTimeout = 30)
			where T : class
		{
			IEnumerable<T> list = ExtraCrud.Queries<T>().GetList(connection, whereCondition, param, transaction, buffered, commandTimeout);
			return list;
		}

		public static IEnumerable<T> GetLimit<T>(this IDbConnection connection, int limit, string whereCondition = "", object param = null, IDbTransaction transaction = null, bool buffered = true, int commandTimeout = 30)
			where T : class
		{
			IEnumerable<T> list = ExtraCrud.Queries<T>().GetLimit(connection, limit, whereCondition, param, transaction, buffered, commandTimeout);
			return list;
		}

		public static IEnumerable<T> GetDistinct<T>(this IDbConnection connection, string whereCondition = "", object param = null, IDbTransaction transaction = null, bool buffered = true, int commandTimeout = 30)
			where T : class
		{
			IEnumerable<T> list = ExtraCrud.Queries<T>().GetDistinct(connection, whereCondition, param, transaction, buffered, commandTimeout);
			return list;
		}

		public static IEnumerable<T> GetDistinctLimit<T>(this IDbConnection connection, int limit, string whereCondition = "", object param = null, IDbTransaction transaction = null, bool buffered = true, int commandTimeout = 30)
			where T : class
		{
			IEnumerable<T> list = ExtraCrud.Queries<T>().GetDistinctLimit(connection, limit, whereCondition, param, transaction, buffered, commandTimeout);
			return list;
		}

		public static int RecordCount<T>(this IDbConnection connection, string whereCondition = "", object param = null, IDbTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			int count = ExtraCrud.Queries<T>().RecordCount(connection, whereCondition, param, transaction, commandTimeout);
			return count;
		}
		#endregion Delegates <T> Sync

		#region Delegates <T> Async
		public static async Task<IEnumerable<T>> GetKeysAsync<T>(this IDbConnection connection, string whereCondition = "", object param = null, IDbTransaction transaction = null, bool buffered = true, int commandTimeout = 30)
			where T : class
		{
			return await Task.Run(() => GetKeys<T>(connection, whereCondition, param, transaction, buffered, commandTimeout));
		}

		public static async Task<bool> DeleteAsync<T>(this IDbConnection connection, T obj, IDbTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			return await Task.Run(() => Delete<T>(connection, obj, transaction, commandTimeout));
		}

		public static async Task<int> BulkDeleteAsync<T>(this SqlConnection connection, IEnumerable<T> objs, SqlTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			return await Task.Run(() => BulkDelete<T>(connection, objs, transaction, commandTimeout));
		}

		public static async Task<int> DeleteAsync<T>(this IDbConnection connection, string whereCondition = "", object param = null, IDbTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			return await Task.Run(() => Delete<T>(connection, whereCondition, param, transaction, commandTimeout));
		}

		public static async Task InsertAsync<T>(this IDbConnection connection, T obj, IDbTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			await Task.Run(() => Insert(connection, obj, transaction, commandTimeout));
		}

		public static async Task InsertAsync<T>(this IDbConnection connection, IEnumerable<T> objs, IDbTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			await Task.Run(() => Insert(connection, objs, transaction, commandTimeout));
		}

		public static async Task<bool> UpdateAsync<T>(this IDbConnection connection, object obj, IDbTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			return await Task.Run(() => Update<T>(connection, obj, transaction, commandTimeout));
		}

		public static async Task<int> BulkUpdateAsync<T>(this SqlConnection connection, IEnumerable<T> objs, SqlTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			return await Task.Run(() => BulkUpdate(connection, objs, transaction, commandTimeout));
		}

		public static async Task UpsertAsync<T>(this IDbConnection connection, T obj, IDbTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			await Task.Run(() => Upsert(connection, obj, transaction, commandTimeout));
		}

		public static async Task UpsertAsync<T>(this IDbConnection connection, IEnumerable<T> objs, IDbTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			await Task.Run(() => Upsert(connection, objs, transaction, commandTimeout));
		}

		public static async Task<T> GetAsync<T>(this IDbConnection connection, T obj, IDbTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			return await Task.Run(() => Get(connection, obj, transaction, commandTimeout));
		}

		public static async Task<IEnumerable<T>> GetListAsync<T>(this IDbConnection connection, string whereCondition = "", object param = null, IDbTransaction transaction = null, bool buffered = true, int commandTimeout = 30)
			where T : class
		{
			return await Task.Run(() => GetList<T>(connection, whereCondition, param, transaction, buffered, commandTimeout));
		}

		public static async Task<IEnumerable<T>> GetLimitAsync<T>(this IDbConnection connection, int limit, string whereCondition = "", object param = null, IDbTransaction transaction = null, bool buffered = true, int commandTimeout = 30)
			where T : class
		{
			return await Task.Run(() => GetLimit<T>(connection, limit, whereCondition, param, transaction, buffered, commandTimeout));
		}

		public static async Task<IEnumerable<T>> GetDistinctAsync<T>(this IDbConnection connection, string whereCondition = "", object param = null, IDbTransaction transaction = null, bool buffered = true, int commandTimeout = 30)
			where T : class
		{
			return await Task.Run(() => GetDistinct<T>(connection, whereCondition, param, transaction, buffered, commandTimeout));
		}

		public static async Task<IEnumerable<T>> GetDistinctLimitAsync<T>(this IDbConnection connection, int limit, string whereCondition = "", object param = null, IDbTransaction transaction = null, bool buffered = true, int commandTimeout = 30)
			where T : class
		{
			return await Task.Run(() => GetDistinctLimit<T>(connection, limit, whereCondition, param, transaction, buffered, commandTimeout));
		}

		public static async Task<int> RecordCountAsync<T>(this IDbConnection connection, string whereCondition = "", object param = null, IDbTransaction transaction = null, int commandTimeout = 30)
			where T : class
		{
			return await Task.Run(() => RecordCount<T>(connection, whereCondition, param, transaction, commandTimeout));
		}
		#endregion Delegates <T> Async
	}
}