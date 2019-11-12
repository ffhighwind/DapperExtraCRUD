﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dapper.Extra.Interfaces;
using Dapper.Extra.Internal;

namespace Dapper.Extra.Interfaces
{
	public interface ISqlQueries<T, KeyType> where T : class
	{
		SqlKeysList<T, KeyType> BulkGet { get; }
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
		DbWhereInt<T> DeleteWhere { get; }
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
		DbTBool<T> Upsert { get; }
		DbTBool<T> InsertIfNotExists { get; }

		//public override int RemoveDuplicates(IDbConnection connection, IDbTransaction transaction, int commandTimeout = 30)
	}
}