// Released under MIT License 
// Copyright(c) 2018 Wesley Hamilton
// License: https://www.mit.edu/~amini/LICENSE.md
// Home page: https://github.com/ffhighwind/DapperExtraCRUD

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Extra.Internal
{
	public class SqlQueries<T, KeyType> : ISqlQueries<T, KeyType> where T : class
	{
		public DbKeyObj<T, KeyType> Get { get; internal set; }

		public DbKeysList<T, KeyType> BulkGet => LazyBulkGet.Value;
		public SqlKeysInt<T, KeyType> BulkDelete => LazyBulkDelete.Value;
		public DbKeyBool<KeyType> Delete => LazyDelete.Value;
		public DbWhereKeys<KeyType> GetKeys => LazyGetKeys.Value;

		#region Lazy Internal
		internal Lazy<DbKeysList<T, KeyType>> LazyBulkGet { get; set; }
		internal Lazy<SqlKeysInt<T, KeyType>> LazyBulkDelete { get; set; }
		internal Lazy<DbKeyBool<KeyType>> LazyDelete { get; set; }
		internal Lazy<DbWhereKeys<KeyType>> LazyGetKeys { get; set; }
		#endregion Lazy Internal
	}

	public class SqlQueries<T> : ISqlQueries<T> where T : class
	{
		public DbTBool<T> Delete { get; internal set; }
		public DbTT<T> Get { get; internal set; }
		public DbWhereList<T> GetList { get; internal set; }
		public DbTVoid<T> Insert { get; internal set; }
		public DbTBool<T> Update { get; internal set; }

		public SqlListList<T> BulkGet => LazyBulkGet.Value;
		public SqlListInt<T> BulkDelete => LazyBulkDelete.Value;
		public SqlListVoid<T> BulkInsert => LazyBulkInsert.Value;
		public SqlListInt<T> BulkUpdate => LazyBulkUpdate.Value;
		public SqlListInt<T> BulkUpsert => LazyBulkUpsert.Value;
		public SqlListInt<T> BulkInsertIfNotExists => LazyBulkInsertIfNotExists.Value;
		public DbWhereInt<T> DeleteList => LazyDeleteList.Value;
		public DbVoid DeleteAll => LazyDeleteAll.Value;
		public DbWhereList<T> GetKeys => LazyGetKeys.Value;
		public DbWhereList<T> GetDistinct => LazyGetDistinct.Value;
		public DbLimitList<T> GetLimit => LazyGetLimit.Value;
		public DbLimitList<T> GetDistinctLimit => LazyGetDistinctLimit.Value;
		public DbWhereInt<T> RecordCount => LazyRecordCount.Value;
		public DbObjBool<T> UpdateObj => LazyUpdateObj.Value;
		public DbTBool<T> Upsert => LazyUpsert.Value;
		public DbTBool<T> InsertIfNotExists => LazyInsertIfNotExists.Value;

		#region Lazy Internal
		internal Lazy<SqlListList<T>> LazyBulkGet { get; set; }
		internal Lazy<SqlListInt<T>> LazyBulkDelete { get; set; }
		internal Lazy<SqlListVoid<T>> LazyBulkInsert { get; set; }
		internal Lazy<SqlListInt<T>> LazyBulkUpdate { get; set; }
		internal Lazy<SqlListInt<T>> LazyBulkUpsert { get; set; }
		internal Lazy<SqlListInt<T>> LazyBulkInsertIfNotExists { get; set; }
		internal Lazy<DbWhereInt<T>> LazyDeleteList { get; set; }
		internal Lazy<DbVoid> LazyDeleteAll { get; set; }
		internal Lazy<DbWhereList<T>> LazyGetKeys { get; set; }
		internal Lazy<DbWhereList<T>> LazyGetDistinct { get; set; }
		internal Lazy<DbLimitList<T>> LazyGetLimit { get; set; }
		internal Lazy<DbLimitList<T>> LazyGetDistinctLimit { get; set; }
		internal Lazy<DbWhereInt<T>> LazyRecordCount { get; set; }
		internal Lazy<DbObjBool<T>> LazyUpdateObj { get; set; }
		internal Lazy<DbTBool<T>> LazyUpsert { get; set; }
		internal Lazy<DbTBool<T>> LazyInsertIfNotExists { get; set; }
		#endregion Lazy Internal
	}
}
