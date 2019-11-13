using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Extra.Utilities
{
	public interface IAccessObjectSync<T>
		where T : class
	{
		IEnumerable<T> GetKeys(string whereCondition = "", object param = null, int commandTimeout = 30);
		IEnumerable<KeyType> GetKeys<KeyType>(string whereCondition = "", object param = null, int commandTimeout = 30);

		bool Delete(T obj, int commandTimeout = 30);
		bool Delete<KeyType>(KeyType key, int commandTimeout = 30);
		int Delete(string whereCondition = "", object param = null, int commandTimeout = 30);
		IEnumerable<T> DeleteList(string whereCondition = "", object param = null, int commandTimeout = 30);

		void Insert(T obj, int commandTimeout = 30);
		bool Update(object obj, int commandTimeout = 30);
		bool Update(T obj, int commandTimeout = 30);
		bool Upsert(T obj, int commandTimeout = 30);

		T Get(T obj, int commandTimeout = 30);
		T Get<KeyType>(KeyType key, int commandTimeout = 30);
		IEnumerable<T> GetList(string whereCondition = "", object param = null, int commandTimeout = 30);
		IEnumerable<T> GetLimit(int limit, string whereCondition = "", object param = null, int commandTimeout = 30);
		IEnumerable<T> GetDistinct(string whereCondition = "", object param = null, int commandTimeout = 30);
		IEnumerable<T> GetDistinctLimit(int limit, string whereCondition = "", object param = null, int commandTimeout = 30);

		int RecordCount(string whereCondition = "", object param = null, int commandTimeout = 30);

		List<T> BulkGet(IEnumerable<T> keys, int commandTimeout = 30);
		List<T> BulkGet<KeyType>(IEnumerable<KeyType> keys, int commandTimeout = 30);
		void BulkInsert(IEnumerable<T> objs, int commandTimeout = 30);
		int BulkUpdate(IEnumerable<T> objs, int commandTimeout = 30);
		int BulkDelete(IEnumerable<T> objs, int commandTimeout = 30);
		int BulkDelete<KeyType>(IEnumerable<KeyType> keys, int commandTimeout = 30);
		int BulkUpsert(IEnumerable<T> objs, int commandTimeout = 30);
	}
}