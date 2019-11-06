using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dapper.Extra.Interfaces;

namespace Dapper.Extra
{
	public class TableQueries<T, KeyType> 
		where T : class
	{
		internal TableQueries() { }

		public TableDelegates<T, KeyType>.SqlKeysInt BulkDelete { get; internal set; }
		public TableDelegates<T, KeyType>.DbKeyBool Delete { get; internal set; }
		public TableDelegates<T, KeyType>.DbKeyObj Get { get; internal set; }
		public TableDelegates<T, KeyType>.DbWhereKeys GetKeys { get; internal set; }
	}


	public class TableQueries<T> 
		where T : class
	{
		internal TableQueries() { }

		public IReadOnlyList<PropertyInfo> Properties { get; internal set; }
		public IReadOnlyList<PropertyInfo> KeyProperties { get; internal set; }
		public PropertyInfo AutoKeyProperty { get; internal set; }
		public IReadOnlyList<PropertyInfo> InsertKeyProperties { get; internal set; }
		public IReadOnlyList<PropertyInfo> UpdateKeyProperties { get; internal set; }

		public IReadOnlyList<string> Columns { get; internal set; }
		public IReadOnlyList<string> KeyColumns { get; internal set; }

		public TableDelegates<T>.SqlListInt BulkDelete { get; internal set; }
		public TableDelegates<T>.SqlListVoid BulkInsert { get; internal set; }
		public TableDelegates<T>.SqlListInt BulkUpdate { get; internal set; }
		public TableDelegates<T>.SqlListInt BulkUpsert { get; internal set; }
		public TableDelegates<T>.SqlListInt BulkInsertIfNotExists { get; internal set; }
		public TableDelegates<T>.DbObjBool Delete { get; internal set; }
		public TableDelegates<T>.DbWhereInt DeleteWhere { get; internal set; }
		public TableDelegates<T>.DbObjObj Get { get; internal set; }
		public TableDelegates<T>.DbWhereList GetKeys { get; internal set; }
		public TableDelegates<T>.DbWhereList GetList { get; internal set; }
		public TableDelegates<T>.DbWhereList GetDistinct { get; internal set; }
		public TableDelegates<T>.DbLimitList GetLimit { get; internal set; }
		public TableDelegates<T>.DbLimitList GetDistinctLimit { get; internal set; }
		public TableDelegates<T>.DbObjVoid Insert { get; internal set; }
		public TableDelegates<T>.DbWhereInt RecordCount { get; internal set; }
		public TableDelegates<T>.DbObjBool Update { get; internal set; }
		public TableDelegates<T>.DbObjObjBool UpdateFilter { get; internal set; }
		public TableDelegates<T>.DbObjBool Upsert { get; internal set; }
		public TableDelegates<T>.DbObjBool InsertIfNotExists { get; internal set; }

		//public override int RemoveDuplicates(IDbConnection connection, IDbTransaction transaction, int? commandTimeout = null)
	}
}
