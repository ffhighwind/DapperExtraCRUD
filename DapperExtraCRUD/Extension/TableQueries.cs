using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dapper.Extension.Interfaces;

namespace Dapper.Extension
{
	public partial class TableQueries<T, KeyType> where T : class
	{
		public TableQueries(TableQueries<T, KeyType>.Data impl)
		{
			BulkDeleteFunc = impl.BulkDeleteFunc;
			DeleteFunc = impl.DeleteKeyFunc;
			DeleteListFunc = impl.DeleteKeysWhereFunc;
			GetFunc = impl.GetKeyFunc;
			GetKeysFunc = impl.GetKeysWhereFunc;
			BulkDeleteListFunc = impl.BulkDeleteListFunc;
		}

		public TableQueries<T, KeyType>.Delegates.SqlKeysKeys BulkDeleteListFunc { get; private set; }
		public TableQueries<T, KeyType>.Delegates.SqlKeysInt BulkDeleteFunc { get; private set; }
		public TableQueries<T, KeyType>.Delegates.SqlKeyBool DeleteFunc { get; private set; }
		public TableQueries<T, KeyType>.Delegates.DbWhereKeys DeleteListFunc { get; private set; }
		public TableQueries<T, KeyType>.Delegates.SqlKeyObj GetFunc { get; private set; }
		public TableQueries<T, KeyType>.Delegates.DbWhereKeys GetKeysFunc { get; private set; }
	}

	public partial class TableQueries<T> where T : class
	{
		public TableQueries(TableQueries<T>.Data impl)
		{
			Properties = impl.Properties;
			KeyProperties = impl.KeyProperties;
			AutoKeyProperties = impl.AutoKeyProperties;
			EqualityProperties = impl.EqualityProperties;
			Columns = impl.Columns;
			KeyColumns = impl.KeyColumns;

			BulkDeleteFunc = impl.BulkDeleteFunc;
			BulkDeleteListFunc = impl.BulkDeleteListFunc;
			BulkInsertFunc = impl.BulkInsertFunc;
			BulkUpdateFunc = impl.BulkUpdateFunc;
			BulkUpdateListFunc = impl.BulkUpdateListFunc;
			BulkUpsertFunc = impl.BulkUpsertFunc;
			BulkUpsertListFunc = impl.BulkUpsertListFunc;
			DeleteDictFunc = impl.DeleteDictFunc;
			DeleteFunc = impl.DeleteFunc;
			DeleteWhereFunc = impl.DeleteWhereFunc;
			DeleteListFunc = impl.DeleteListFunc;
			GetDictFunc = impl.GetDictFunc;
			GetFunc = impl.GetFunc;
			GetKeysFunc = impl.GetKeysFunc;
			GetListFunc = impl.GetListFunc;
			GetTopFunc = impl.GetTopFunc;
			GetDistinctFunc = impl.GetDistinctFunc;
			InsertFunc = impl.InsertFunc;
			RecordCountFunc = impl.RecordCountFunc;
			UpdateFunc = impl.UpdateFunc;
			UpsertFunc = impl.UpsertFunc;
		}

		public PropertyInfo[] Properties { get; private set; }
		public PropertyInfo[] KeyProperties { get; private set; }
		public PropertyInfo[] AutoKeyProperties { get; private set; }
		public PropertyInfo[] EqualityProperties { get; private set; }

		public string[] Columns { get; private set; }
		public string[] KeyColumns { get; private set; }

		public TableQueries<T>.Delegates.SqlListInt BulkDeleteFunc { get; private set; }
		public TableQueries<T>.Delegates.SqlListList BulkDeleteListFunc { get; private set; }
		public TableQueries<T>.Delegates.SqlListList BulkInsertFunc { get; private set; }
		public TableQueries<T>.Delegates.SqlListInt BulkUpdateFunc { get; private set; }
		public TableQueries<T>.Delegates.SqlListList BulkUpdateListFunc { get; private set; }
		public TableQueries<T>.Delegates.SqlListInt BulkUpsertFunc { get; private set; }
		public TableQueries<T>.Delegates.SqlListList BulkUpsertListFunc { get; private set; }
		public TableQueries<T>.Delegates.DbDictBool DeleteDictFunc { get; private set; }
		public TableQueries<T>.Delegates.DbObjBool DeleteFunc { get; private set; }
		public TableQueries<T>.Delegates.DbWhereInt DeleteWhereFunc { get; private set; }
		public TableQueries<T>.Delegates.DbWhereList DeleteListFunc { get; private set; }
		public TableQueries<T>.Delegates.DbDictObj GetDictFunc { get; private set; }
		public TableQueries<T>.Delegates.DbObjObj GetFunc { get; private set; }
		public TableQueries<T>.Delegates.DbWhereList GetKeysFunc { get; private set; }
		public TableQueries<T>.Delegates.DbWhereList GetListFunc { get; private set; }
		public TableQueries<T>.Delegates.DbWhereList GetDistinctFunc { get; private set; }
		public TableQueries<T>.Delegates.DbLimitList GetTopFunc { get; private set; }
		public TableQueries<T>.Delegates.DbObjObj InsertFunc { get; private set; }
		public TableQueries<T>.Delegates.DbWhereInt RecordCountFunc { get; private set; }
		public TableQueries<T>.Delegates.DbObjBool UpdateFunc { get; private set; }
		public TableQueries<T>.Delegates.DbObjObj UpsertFunc { get; private set; }
	}
}
