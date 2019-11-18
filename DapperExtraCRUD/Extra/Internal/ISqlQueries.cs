// Released under MIT License 
// Copyright(c) 2018 Wesley Hamilton
// License: https://www.mit.edu/~amini/LICENSE.md
// Home page: https://github.com/ffhighwind/DapperExtraCRUD

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Extra.Internal
{
	public interface ISqlQueries<T, KeyType> where T : class
	{
		DbKeysList<T, KeyType> BulkGet { get; }
		SqlKeysInt<T, KeyType> BulkDelete { get; }
		DbKeyBool<KeyType> Delete { get; }
		DbKeyObj<T, KeyType> Get { get; }
		DbWhereKeys<KeyType> GetKeys { get; }
	}


	public interface ISqlQueries<T> where T : class
	{
		SqlListList<T> BulkGet { get; }
		SqlListInt<T> BulkDelete { get; }
		SqlListVoid<T> BulkInsert { get; }
		SqlListInt<T> BulkUpdate { get; }
		SqlListInt<T> BulkUpsert { get; }
		SqlListInt<T> BulkInsertIfNotExists { get; }
		DbTBool<T> Delete { get; }
		DbWhereInt<T> DeleteList { get; }
		DbVoid DeleteAll { get; }
		DbTT<T> Get { get; }
		DbWhereList<T> GetKeys { get; }
		DbWhereList<T> GetList { get; }
		DbWhereList<T> GetDistinct { get; }
		DbLimitList<T> GetLimit { get; }
		DbLimitList<T> GetDistinctLimit { get; }
		DbTVoid<T> Insert { get; }
		DbWhereInt<T> RecordCount { get; }
		DbTBool<T> Update { get; }
		DbObjBool<T> UpdateObj { get; }
		DbTBool<T> Upsert { get; }
		DbTBool<T> InsertIfNotExists { get; }

		//public override int RemoveDuplicates(IDbConnection connection, IDbTransaction transaction, int commandTimeout = 30)
	}
}
