#region License
// Released under MIT License 
// License: https://www.mit.edu/~amini/LICENSE.md
// Home page: https://github.com/ffhighwind/DapperExtraCRUD

// Copyright(c) 2018 Wesley Hamilton

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
#endregion

using System;

namespace Dapper.Extra.Internal
{
	internal class SqlQueries<T> : ISqlQueries<T> where T : class
	{
		public DbTBool<T> Delete { get; internal set; }
		public DbTT<T> Get { get; internal set; }
		public DbWhereList<T> GetList { get; internal set; }
		public DbTVoid<T> Insert { get; internal set; }
		public DbTBool<T> Update { get; internal set; }

		public DbKeyObj<T> GetKey { get; internal set; }
		public DbKeyBool DeleteKey { get; internal set; }
		public DbKeysList<T> BulkGetKeys => LazyBulkGetKeys.Value;
		public DbKeysInt<T> BulkDeleteKeys => LazyBulkDeleteKeys.Value;
		public DbWhereKeys GetKeysKeys => LazyGetKeysKeys.Value;

		public DbListList<T> BulkGet => LazyBulkGet.Value;
		public DbListInt<T> BulkDelete => LazyBulkDelete.Value;
		public DbListVoid<T> BulkInsert => LazyBulkInsert.Value;
		public DbListInt<T> BulkUpdate => LazyBulkUpdate.Value;
		public DbListInt<T> BulkUpsert => LazyBulkUpsert.Value;
		public DbListInt<T> BulkInsertIfNotExists => LazyBulkInsertIfNotExists.Value;
		public DbWhereInt<T> DeleteList => LazyDeleteList.Value;
		public DbVoid Truncate => LazyTruncate.Value;
		public DbWhereList<T> GetKeys => LazyGetKeys.Value;
		public DbTypeWhereList<T> GetDistinct => LazyGetDistinct.Value;
		public DbLimitList<T> GetLimit => LazyGetLimit.Value;
		public DbTypeLimitList<T> GetDistinctLimit => LazyGetDistinctLimit.Value;
		public DbWhereInt<T> RecordCount => LazyRecordCount.Value;
		public DbObjBool<T> UpdateObj => LazyUpdateObj.Value;
		public DbTBool<T> Upsert => LazyUpsert.Value;
		public DbTBool<T> InsertIfNotExists => LazyInsertIfNotExists.Value;
		public DbTypeWhereList<T> GetFilter => LazyGetFilter.Value;
		public DbTypeLimitList<T> GetFilterLimit => LazyGetFilterLimit.Value;
		public DbTVoid<T> InsertAutoSync { get; internal set; }
		public DbTVoid<T> UpdateAutoSync { get; internal set; }

		#region Lazy Internal
		internal Lazy<DbListList<T>> LazyBulkGet { get; set; }
		internal Lazy<DbListInt<T>> LazyBulkDelete { get; set; }
		internal Lazy<DbListVoid<T>> LazyBulkInsert { get; set; }
		internal Lazy<DbListInt<T>> LazyBulkUpdate { get; set; }
		internal Lazy<DbListInt<T>> LazyBulkUpsert { get; set; }
		internal Lazy<DbListInt<T>> LazyBulkInsertIfNotExists { get; set; }
		internal Lazy<DbWhereInt<T>> LazyDeleteList { get; set; }
		internal Lazy<DbTypeWhereList<T>> LazyGetFilter { get; set; }
		internal Lazy<DbTypeLimitList<T>> LazyGetFilterLimit { get; set; }
		internal Lazy<DbVoid> LazyTruncate { get; set; }
		internal Lazy<DbWhereList<T>> LazyGetKeys { get; set; }
		internal Lazy<DbTypeWhereList<T>> LazyGetDistinct { get; set; }
		internal Lazy<DbLimitList<T>> LazyGetLimit { get; set; }
		internal Lazy<DbTypeLimitList<T>> LazyGetDistinctLimit { get; set; }
		internal Lazy<DbWhereInt<T>> LazyRecordCount { get; set; }
		internal Lazy<DbObjBool<T>> LazyUpdateObj { get; set; }
		internal Lazy<DbTBool<T>> LazyUpsert { get; set; }
		internal Lazy<DbTBool<T>> LazyInsertIfNotExists { get; set; }
		// Keys
		internal Lazy<DbKeysList<T>> LazyBulkGetKeys { get; set; }
		internal Lazy<DbKeysInt<T>> LazyBulkDeleteKeys { get; set; }
		internal Lazy<DbWhereKeys> LazyGetKeysKeys { get; set; }
		#endregion Lazy Internal
	}
}
