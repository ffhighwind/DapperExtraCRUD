using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Extension
{
	public partial class TableQueries<T, KeyType> where T : class
	{
		public class Data
		{
			public TableQueries<T, KeyType>.Delegates.SqlKeysKeys BulkDeleteListFunc { get; set; }
			public TableQueries<T, KeyType>.Delegates.SqlKeysInt BulkDeleteFunc { get; set; }
			public TableQueries<T, KeyType>.Delegates.SqlKeyBool DeleteKeyFunc { get; set; }
			public TableQueries<T, KeyType>.Delegates.DbWhereKeys DeleteKeysWhereFunc { get; set; }
			public TableQueries<T, KeyType>.Delegates.SqlKeyObj GetKeyFunc { get; set; }
			public TableQueries<T, KeyType>.Delegates.DbWhereKeys GetKeysWhereFunc { get; set; }
		}
	}

	public partial class TableQueries<T> where T : class
	{
		public class Data
		{
			public PropertyInfo[] Properties { get; set; }
			public PropertyInfo[] KeyProperties { get; set; }
			public PropertyInfo[] AutoKeyProperties { get; set; }
			public PropertyInfo[] EqualityProperties { get; set; }

			public string[] Columns { get; set; }
			public string[] KeyColumns { get; set; }

			public TableQueries<T>.Delegates.SqlListInt BulkDeleteFunc { get; set; }
			public TableQueries<T>.Delegates.SqlListList BulkDeleteListFunc { get; set; }
			public TableQueries<T>.Delegates.SqlListList BulkInsertFunc { get; set; }
			public TableQueries<T>.Delegates.SqlListInt BulkUpdateFunc { get; set; }
			public TableQueries<T>.Delegates.SqlListList BulkUpdateListFunc { get; set; }
			public TableQueries<T>.Delegates.SqlListInt BulkUpsertFunc { get; set; }
			public TableQueries<T>.Delegates.SqlListList BulkUpsertListFunc { get; set; }
			public TableQueries<T>.Delegates.DbDictBool DeleteDictFunc { get; set; }
			public TableQueries<T>.Delegates.DbObjBool DeleteFunc { get; set; }
			public TableQueries<T>.Delegates.DbWhereInt DeleteWhereFunc { get; set; }
			public TableQueries<T>.Delegates.DbWhereList DeleteListFunc { get; set; }
			public TableQueries<T>.Delegates.DbDictObj GetDictFunc { get; set; }
			public TableQueries<T>.Delegates.DbObjObj GetFunc { get; set; }
			public TableQueries<T>.Delegates.DbWhereList GetKeysFunc { get; set; }
			public TableQueries<T>.Delegates.DbWhereList GetListFunc { get; set; }
			public TableQueries<T>.Delegates.DbWhereList GetDistinctFunc { get; set; }
			public TableQueries<T>.Delegates.DbLimitList GetTopFunc { get; set; }
			public TableQueries<T>.Delegates.DbObjObj InsertFunc { get; set; }
			public TableQueries<T>.Delegates.DbWhereInt RecordCountFunc { get; set; }
			public TableQueries<T>.Delegates.DbObjBool UpdateFunc { get; set; }
			public TableQueries<T>.Delegates.DbObjObj UpsertFunc { get; set; }
		}
	}
}
