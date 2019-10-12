using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.Extension;

namespace Dapper
{
	public static class DapperExtensions
	{
		#region ITableQueries<T>
		public static IEnumerable<T> GetKeys<T>(this IDbConnection connection, string whereCondition = "", object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null)
			where T : class
		{
			return TableData<T>.Queries.GetKeysFunc(connection, whereCondition, param, transaction, buffered, commandTimeout);
		}

		public static bool Delete<T>(this IDbConnection connection, IDictionary<string, object> key, IDbTransaction transaction = null, int? commandTimeout = null)
			where T : class
		{
			return TableData<T>.Queries.DeleteDictFunc(connection, key, transaction, commandTimeout);
		}

		public static bool Delete<T>(this IDbConnection connection, T obj, IDbTransaction transaction = null, int? commandTimeout = null)
			where T : class
		{
			return TableData<T>.Queries.DeleteFunc(connection, obj, transaction, commandTimeout);
		}

		public static int BulkDelete<T>(this SqlConnection connection, IEnumerable<T> objs, SqlTransaction transaction = null, int? commandTimeout = null)
			where T : class
		{
			return TableData<T>.Queries.BulkDeleteFunc(connection, objs, transaction, commandTimeout);
		}

		public static int Delete<T>(this IDbConnection connection, string whereCondition = "", object param = null, IDbTransaction transaction = null, int? commandTimeout = null)
			where T : class
		{
			return TableData<T>.Queries.DeleteWhereFunc(connection, whereCondition, param, transaction, commandTimeout);
		}

		public static IEnumerable<T> DeleteList<T>(this IDbConnection connection, string whereCondition = "", object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null)
			where T : class
		{
			return TableData<T>.Queries.DeleteListFunc(connection, whereCondition, param, transaction, buffered, commandTimeout);
		}

		public static void Insert<T>(this IDbConnection connection, T obj, IDbTransaction transaction = null, int? commandTimeout = null)
			where T : class
		{
			TableData<T>.Queries.InsertFunc(connection, obj, transaction, commandTimeout);
		}

		public static IEnumerable<T> BulkInsert<T>(this SqlConnection connection, IEnumerable<T> objs, SqlTransaction transaction = null, int? commandTimeout = null)
			where T : class
		{
			return TableData<T>.Queries.BulkInsertFunc(connection, objs, transaction, commandTimeout);
		}

		public static bool Update<T>(this IDbConnection connection, T obj, IDbTransaction transaction = null, int? commandTimeout = null)
			where T : class
		{
			return TableData<T>.Queries.UpdateFunc(connection, obj, transaction, commandTimeout);
		}

		//public static async int Update<T>(this IDbConnection connection, string whereCondition = "", object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null)
		//{
		//	return TableData<T>.Queries.Update(connection, whereCondition, param, transaction, buffered, commandTimeout);
		//}

		public static int BulkUpdate<T>(this SqlConnection connection, IEnumerable<T> objs, SqlTransaction transaction = null, int? commandTimeout = null)
			where T : class
		{
			return TableData<T>.Queries.BulkUpdateFunc(connection, objs, transaction, commandTimeout);
		}

		public static void Upsert<T>(this IDbConnection connection, T obj, IDbTransaction transaction = null, int? commandTimeout = null)
			where T : class
		{
			TableData<T>.Queries.UpsertFunc(connection, obj, transaction, commandTimeout);
		}

		public static int BulkUpsert<T>(this SqlConnection connection, IEnumerable<T> objs, SqlTransaction transaction = null, bool buffered = true, int? commandTimeout = null)
			where T : class
		{
			return TableData<T>.Queries.BulkUpsertFunc(connection, objs, transaction, commandTimeout);
		}

		public static T Get<T>(this IDbConnection connection, IDictionary<string, object> key, IDbTransaction transaction = null, int? commandTimeout = null)
			where T : class
		{
			return TableData<T>.Queries.GetDictFunc(connection, key, transaction, commandTimeout);
		}

		public static T Get<T>(this IDbConnection connection, T obj, IDbTransaction transaction = null, int? commandTimeout = null)
			where T : class
		{
			return TableData<T>.Queries.GetFunc(connection, obj, transaction, commandTimeout);
		}

		public static IEnumerable<T> GetList<T>(this IDbConnection connection, string whereCondition = "", object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null)
			where T : class
		{
			return TableData<T>.Queries.GetListFunc(connection, whereCondition, param, transaction, buffered, commandTimeout);
		}

		public static IEnumerable<T> GetTop<T>(this IDbConnection connection, int limit, string whereCondition = "", object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null)
			where T : class
		{
			return TableData<T>.Queries.GetTopFunc(connection, limit, whereCondition, param, transaction, buffered, commandTimeout);
		}

		public static IEnumerable<T> GetDistinct<T>(this IDbConnection connection, string whereCondition = "", object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null)
			where T : class
		{
			return TableData<T>.Queries.GetDistinctFunc(connection, whereCondition, param, transaction, buffered, commandTimeout);
		}

		public static int RecordCount<T>(this IDbConnection connection, string whereCondition = "", object param = null, IDbTransaction transaction = null, int? commandTimeout = null)
			where T : class
		{
			return TableData<T>.Queries.RecordCountFunc(connection, whereCondition, param, transaction, commandTimeout);
		}
		#endregion ITableQueries<T>

		#region ITableQueriesAsync<T>
		public static async Task<IEnumerable<T>> GetKeysAsync<T>(this IDbConnection connection, string whereCondition = "", object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null)
			where T : class
		{
			return await Task.Run(() => GetKeys<T>(connection, whereCondition, param, transaction, buffered, commandTimeout));
		}

		public static async Task<bool> DeleteAsync<T>(this IDbConnection connection, IDictionary<string, object> key, IDbTransaction transaction = null, int? commandTimeout = null)
			where T : class
		{
			return await Task.Run(() => Delete<T>(connection, key, transaction, commandTimeout));
		}

		public static async Task<bool> DeleteAsync<T>(this IDbConnection connection, T obj, IDbTransaction transaction = null, int? commandTimeout = null)
			where T : class
		{
			return await Task.Run(() => Delete<T>(connection, obj, transaction, commandTimeout));
		}

		public static async Task<int> BulkDeleteAsync<T>(this SqlConnection connection, IEnumerable<T> objs, SqlTransaction transaction = null, int? commandTimeout = null)
			where T : class
		{
			return await Task.Run(() => BulkDelete<T>(connection, objs, transaction, commandTimeout));
		}

		public static async Task<int> DeleteAsync<T>(this IDbConnection connection, string whereCondition = "", object param = null, IDbTransaction transaction = null, int? commandTimeout = null)
			where T : class
		{
			return await Task.Run(() => Delete<T>(connection, whereCondition, param, transaction, commandTimeout));
		}

		public static async Task<IEnumerable<T>> DeleteListAsync<T>(this IDbConnection connection, string whereCondition = "", object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null)
			where T : class
		{
			return await Task.Run(() => DeleteList<T>(connection, whereCondition, param, transaction, buffered, commandTimeout));
		}

		public static async Task InsertAsync<T>(this IDbConnection connection, T obj, IDbTransaction transaction = null, int? commandTimeout = null)
			where T : class
		{
			await Task.Run(() => Insert(connection, obj, transaction, commandTimeout));
		}

		public static async Task InsertAsync<T>(this IDbConnection connection, IEnumerable<T> objs, IDbTransaction transaction = null, int? commandTimeout = null)
			where T : class
		{
			await Task.Run(() => Insert(connection, objs, transaction, commandTimeout));
		}

		public static async Task<bool> UpdateAsync<T>(this IDbConnection connection, T obj, IDbTransaction transaction = null, int? commandTimeout = null)
			where T : class
		{
			return await Task.Run(() => Update(connection, obj, transaction, commandTimeout));
		}

		//public static async Task<int> UpdateAsync<T>(this IDbConnection connection, string whereCondition = "", object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null)
		//{
		//	return await Task.Run(() => Update<T>(connection, whereCondition, param, transaction, buffered, commandTimeout));
		//}

		public static async Task<int> BulkUpdateAsync<T>(this SqlConnection connection, IEnumerable<T> objs, SqlTransaction transaction = null, int? commandTimeout = null)
			where T : class
		{
			return await Task.Run(() => BulkUpdate(connection, objs, transaction, commandTimeout));
		}

		public static async Task UpsertAsync<T>(this IDbConnection connection, T obj, IDbTransaction transaction = null, int? commandTimeout = null)
			where T : class
		{
			await Task.Run(() => Upsert(connection, obj, transaction, commandTimeout));
		}

		public static async Task UpsertAsync<T>(this IDbConnection connection, IEnumerable<T> objs, IDbTransaction transaction = null, int? commandTimeout = null)
			where T : class
		{
			await Task.Run(() => Upsert(connection, objs, transaction, commandTimeout));
		}

		public static async Task<T> GetAsync<T>(this IDbConnection connection, IDictionary<string, object> key, IDbTransaction transaction = null, int? commandTimeout = null)
			where T : class
		{
			return await Task.Run(() => Get<T>(connection, key, transaction, commandTimeout));
		}

		public static async Task<T> GetAsync<T>(this IDbConnection connection, T obj, IDbTransaction transaction = null, int? commandTimeout = null)
			where T : class
		{
			return await Task.Run(() => Get(connection, obj, transaction, commandTimeout));
		}

		public static async Task<IEnumerable<T>> GetListAsync<T>(this IDbConnection connection, string whereCondition = "", object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null)
			where T : class
		{
			return await Task.Run(() => GetList<T>(connection, whereCondition, param, transaction, buffered, commandTimeout));
		}

		public static async Task<IEnumerable<T>> GetTopAsync<T>(this IDbConnection connection, int limit, string whereCondition = "", object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null)
			where T : class
		{
			return await Task.Run(() => GetTop<T>(connection, limit, whereCondition, param, transaction, buffered, commandTimeout));
		}

		public static async Task<IEnumerable<T>> GetDistinctAsync<T>(this IDbConnection connection, string whereCondition = "", object param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null)
			where T : class
		{
			return await Task.Run(() => GetDistinct<T>(connection, whereCondition, param, transaction, buffered, commandTimeout));
		}

		public static async Task<int> RecordCountAsync<T>(this IDbConnection connection, string whereCondition = "", object param = null, IDbTransaction transaction = null, int? commandTimeout = null)
			where T : class
		{
			return await Task.Run(() => RecordCount<T>(connection, whereCondition, param, transaction, commandTimeout));
		}
		#endregion ITableQueriesAsync<T>
	}
}