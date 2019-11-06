using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Extra.Interfaces
{
	public interface IAccessObjectSync<T>
		where T : class
	{
		IEnumerable<T> GetKeys(string whereCondition = "", object param = null, int? commandTimeout = null);
		IEnumerable<KeyType> GetKeys<KeyType>(string whereCondition = "", object param = null, int? commandTimeout = null);

		bool Delete(T obj, int? commandTimeout = null);
		bool Delete<KeyType>(KeyType key, int? commandTimeout = null);
		int Delete(string whereCondition = "", object param = null, int? commandTimeout = null);
		IEnumerable<T> DeleteList(string whereCondition = "", object param = null, int? commandTimeout = null);

		void Insert(T obj, int? commandTimeout = null);
		bool Update(T obj, object filter = null, int? commandTimeout = null);
		bool Update(T obj);
		bool Upsert(T obj, int? commandTimeout = null);

		T Get(T obj, int? commandTimeout = null);
		T Get<KeyType>(KeyType key, int? commandTimeout = null);
		IEnumerable<T> GetList(string whereCondition = "", object param = null, int? commandTimeout = null);
		IEnumerable<T> GetLimit(int limit, string whereCondition = "", object param = null, int? commandTimeout = null);
		IEnumerable<T> GetDistinct(string whereCondition = "", object param = null, int? commandTimeout = null);
		IEnumerable<T> GetDistinctLimit(int limit, string whereCondition = "", object param = null, int? commandTimeout = null);

		int RecordCount(string whereCondition = "", object param = null, int? commandTimeout = null);

		void BulkInsert(IEnumerable<T> objs, int? commandTimeout = null);
		int BulkUpdate(IEnumerable<T> objs, int? commandTimeout = null);
		int BulkDelete(IEnumerable<T> objs, int? commandTimeout = null);
		int BulkDelete<KeyType>(IEnumerable<KeyType> keys, int? commandTimeout = null);
		int BulkUpsert(IEnumerable<T> objs, int? commandTimeout = null);
	}
}