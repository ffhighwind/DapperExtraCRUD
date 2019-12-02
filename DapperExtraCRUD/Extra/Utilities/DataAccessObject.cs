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
using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper.Extra.Internal;
using System.Linq;

namespace Dapper.Extra.Utilities
{
	/// <summary>
	/// Connects to the database and performs SQL operations. This is not thread-safe because the 
	/// <see cref="SqlConnection"/> and <see cref="SqlTransaction"/> are reused.
	/// </summary>
	/// <typeparam name="T">The table type.</typeparam>
	public class DataAccessObject<T> : IAccessObject<T>, IDataAccessConnection
		where T : class
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DataAccessObject{T}"/> class.
		/// </summary>
		/// <param name="buffered">The buffered<see cref="bool"/></param>
		public DataAccessObject(bool buffered = true) : this(null, buffered)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DataAccessObject{T}"/> class.
		/// </summary>
		/// <param name="connectionString">The connectionString<see cref="string"/></param>
		/// <param name="buffered">The buffered<see cref="bool"/></param>
		public DataAccessObject(string connectionString, bool buffered = true)
		{
			Connection = new SqlConnection(connectionString);
			Buffered = buffered;
			Queries = ExtraCrud.Queries<T>();
		}

		/// <summary>
		/// Determines if the queries are buffered.
		/// </summary>
		public bool Buffered { get; set; }

		/// <summary>
		/// The connection used for queries. This will be temporarily opened it if is closed. 
		/// This connection is not thread-safe because it is reused for all queries.
		/// </summary>
		public SqlConnection Connection { get; set; }

		/// <summary>
		/// The transaction used for queries.
		/// </summary>
		public SqlTransaction Transaction { get; set; }

		/// <summary>
		/// The SQL commands for a given type.
		/// </summary>
		protected ISqlQueries<T> Queries { get; private set; }

		#region IAccessObjectSync<T>

		/// <summary>
		/// Deletes the rows with the given keys.
		/// </summary>
		/// <param name="keys">The keys for the rows to delete.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of deleted rows.</returns>
		public override int BulkDelete(IEnumerable<object> keys, int commandTimeout = 30)
		{
			int count = Queries.BulkDeleteKeys(Connection, keys, Transaction, commandTimeout);
			return count;
		}

		/// <summary>
		/// Deletes the given rows.
		/// </summary>
		/// <param name="objs">The objects to delete.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of deleted rows.</returns>
		public override int BulkDelete(IEnumerable<T> objs, int commandTimeout = 30)
		{
			int count = Queries.BulkDelete(Connection, objs, Transaction, commandTimeout);
			return count;
		}

		/// <summary>
		/// Selects the rows with the given keys.
		/// </summary>
		/// <param name="keys">The keys of the rows to select.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows with the given keys.</returns>
		public override IEnumerable<T> BulkGet(IEnumerable<object> keys, int commandTimeout = 30)
		{
			List<T> list = Queries.BulkGetKeys(Connection, keys, Transaction, commandTimeout).AsList();
			return list;
		}

		/// <summary>
		/// Selects the rows with the given keys.
		/// </summary>
		/// <param name="objs">The objects to select.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given keys.</returns>
		public override IEnumerable<T> BulkGet(IEnumerable<T> objs, int commandTimeout = 30)
		{
			List<T> list = Queries.BulkGet(Connection, objs, Transaction, commandTimeout).AsList();
			return list;
		}

		/// <summary>
		/// Inserts the given rows.
		/// </summary>
		/// <param name="objs">The objects to insert.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		public override void BulkInsert(IEnumerable<T> objs, int commandTimeout = 30)
		{
			Queries.BulkInsert(Connection, objs, Transaction, commandTimeout);
		}

		/// <summary>
		/// Inserts the given rows if they do not exist.
		/// </summary>
		/// <param name="objs">The objects to insert.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of rows inserted.</returns>
		public override int BulkInsertIfNotExists(IEnumerable<T> objs, int commandTimeout = 30)
		{
			int count = Queries.BulkInsertIfNotExists(Connection, objs, Transaction, commandTimeout);
			return count;
		}

		/// <summary>
		/// Updates the given rows.
		/// </summary>
		/// <param name="objs">The objects to update.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of updated rows.</returns>
		public override int BulkUpdate(IEnumerable<T> objs, int commandTimeout = 30)
		{
			int count = Queries.BulkUpdate(Connection, objs, Transaction, commandTimeout);
			return count;
		}

		/// <summary>
		/// Upserts the given rows.
		/// </summary>
		/// <param name="objs">The objects to upsert.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of upserted rows.</returns>
		public override int BulkUpsert(IEnumerable<T> objs, int commandTimeout = 30)
		{
			int count = Queries.BulkUpsert(Connection, objs, Transaction, commandTimeout);
			return count;
		}

		/// <summary>
		/// Deletes the row with the given key.
		/// </summary>
		/// <param name="key">The key of the row to delete.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the row was deleted; false otherwise.</returns>
		public override bool Delete(object key, int commandTimeout = 30)
		{
			bool success = Queries.DeleteKey(Connection, key, Transaction, commandTimeout);
			return success;
		}

		/// <summary>
		/// Deletes the given row.
		/// </summary>
		/// <param name="obj">The object to delete.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the row was deleted; false otherwise.</returns>
		public override bool Delete(T obj, int commandTimeout = 30)
		{
			bool success = Queries.Delete(Connection, obj, Transaction, commandTimeout);
			return success;
		}

		/// <summary>
		/// Deletes the rows that match the given condition.
		/// </summary>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of deleted rows.</returns>
		public override int DeleteList(string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			int count = Queries.DeleteList(Connection, whereCondition, param, Transaction, commandTimeout);
			return count;
		}

		/// <summary>
		/// Selects the row with the given key.
		/// </summary>
		/// <param name="key">The key of the row to select.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The row with the given key.</returns>
		public override T Get(object key, int commandTimeout = 30)
		{
			T obj = Queries.GetKey(Connection, key, Transaction, commandTimeout);
			return obj;
		}

		/// <summary>
		/// Selects a row.
		/// </summary>
		/// <param name="obj">The object to select.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The selected row if it exists; otherwise null.</returns>
		public override T Get(T obj, int commandTimeout = 30)
		{
			T result = Queries.Get(Connection, obj, Transaction, commandTimeout);
			return result;
		}

		/// <summary>
		/// Selects the rows that match the given condition.
		/// </summary>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="buffered">This argument is ignored and will always be true.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public override IEnumerable<T> GetDistinct(string whereCondition = "", object param = null, bool buffered = true, int commandTimeout = 30)
		{
			List<T> list = Queries.GetDistinct(Connection, typeof(T), whereCondition, param, Transaction, true, commandTimeout).AsList();
			return list;
		}

		/// <summary>
		/// Selects the rows that match the given condition.
		/// </summary>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="buffered">This argument is ignored and will always be true.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public override IEnumerable<T> GetDistinct(Type columnFilter, string whereCondition = "", object param = null, bool buffered = true, int commandTimeout = 30)
		{
			List<T> list = Queries.GetDistinct(Connection, columnFilter, whereCondition, param, Transaction, true, commandTimeout).AsList();
			return list;
		}

		/// <summary>
		/// Selects the rows that match the given condition.
		/// </summary>
		/// <param name="limit">The maximum number of rows.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="buffered">This argument is ignored and will always be true.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public override IEnumerable<T> GetDistinctLimit(int limit, string whereCondition = "", object param = null, bool buffered = true, int commandTimeout = 30)
		{
			List<T> list = Queries.GetDistinctLimit(Connection, typeof(T), limit, whereCondition, param, Transaction, true, commandTimeout).AsList();
			return list;
		}

		/// <summary>
		/// Selects the rows that match the given condition.
		/// </summary>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="limit">The maximum number of rows.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="buffered">This argument is ignored and will always be true.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public override IEnumerable<T> GetDistinctLimit(Type columnFilter, int limit, string whereCondition = "", object param = null, bool buffered = true, int commandTimeout = 30)
		{
			List<T> list = Queries.GetDistinctLimit(Connection, columnFilter, limit, whereCondition, param, Transaction, buffered, commandTimeout).AsList();
			return list;
		}

		/// <summary>
		/// Selects the rows with the given keys.
		/// </summary>
		/// <typeparam name="KeyType">The key type.</typeparam>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="buffered">This argument is ignored and will always be true.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The keys that match the given condition.</returns>
		public override IEnumerable<KeyType> GetKeys<KeyType>(string whereCondition = "", object param = null, bool buffered = true, int commandTimeout = 30)
		{
			IEnumerable<object> keys = Queries.GetKeysKeys(Connection, whereCondition, param, Transaction, true, commandTimeout);
			List<KeyType> castedKeys = keys.Select(k => (KeyType)k).AsList();
			return castedKeys;
		}

		/// <summary>
		/// Selects the keys that match the given condition.
		/// </summary>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="buffered">This argument is ignored and will always be true.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The keys that match the given condition.</returns>
		public override IEnumerable<T> GetKeys(string whereCondition = "", object param = null, bool buffered = true, int commandTimeout = 30)
		{
			IEnumerable<T> keys = Queries.GetKeys(Connection, whereCondition, param, Transaction, true, commandTimeout);
			return keys;
		}

		/// <summary>
		/// Selects a limited number of rows that match the given condition.
		/// </summary>
		/// <param name="limit">The maximum number of rows.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="buffered">This argument is ignored and will always be true.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The limited number of rows that match the given condition.</returns>
		public override IEnumerable<T> GetLimit(int limit, string whereCondition = "", object param = null, bool buffered = true, int commandTimeout = 30)
		{
			List<T> list = Queries.GetLimit(Connection, limit, whereCondition, param, Transaction, true, commandTimeout).AsList();
			return list;
		}

		/// <summary>
		/// Selects a limited number of rows that match the given condition.
		/// </summary>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="limit">The maximum number of rows.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="buffered">This argument is ignored and will always be true.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>A limited number of rows that match the given condition.</returns>
		public override IEnumerable<T> GetLimit(Type columnFilter, int limit, string whereCondition = "", object param = null, bool buffered = true, int commandTimeout = 30)
		{
			List<T> list = Queries.GetFilterLimit(Connection, columnFilter, limit, whereCondition, param, Transaction, true, commandTimeout).AsList();
			return list;
		}

		/// <summary>
		/// Selects the rows that match the given condition.
		/// </summary>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="buffered">This argument is ignored and will always be true.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public override IEnumerable<T> GetList(string whereCondition = "", object param = null, bool buffered = true, int commandTimeout = 30)
		{
			List<T> list = Queries.GetList(Connection, whereCondition, param, Transaction, true, commandTimeout).AsList();
			return list;
		}

		/// <summary>
		/// Selects the rows that match the given condition.
		/// </summary>
		/// <param name="columnFilter">The type whose properties will filter the result.</param>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="buffered">This argument is ignored and will always be true.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The rows that match the given condition.</returns>
		public override IEnumerable<T> GetList(Type columnFilter, string whereCondition = "", object param = null, bool buffered = true, int commandTimeout = 30)
		{
			List<T> list = Queries.GetFilter(Connection, columnFilter, whereCondition, param, Transaction, true, commandTimeout).AsList();
			return list;
		}

		/// <summary>
		/// Inserts a row.
		/// </summary>
		/// <param name="obj">The object to insert.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		public override void Insert(T obj, int commandTimeout = 30)
		{
			Queries.Insert(Connection, obj, Transaction, commandTimeout);
		}

		/// <summary>
		/// Inserts a row if it does not exist.
		/// </summary>
		/// <param name="obj">The object to insert.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the the row was inserted; false otherwise.</returns>
		public override bool InsertIfNotExists(T obj, int commandTimeout = 30)
		{
			bool success = Queries.InsertIfNotExists(Connection, obj, Transaction, commandTimeout);
			return success;
		}

		/// <summary>
		/// Counts the number of rows that match the given condition.
		/// </summary>
		/// <param name="whereCondition">The where condition to use for this query.</param>
		/// <param name="param">The parameters to use for this query.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>The number of rows that match the given condition.</returns>
		public override int RecordCount(string whereCondition = "", object param = null, int commandTimeout = 30)
		{
			int count = Queries.RecordCount(Connection, whereCondition, param, Transaction, commandTimeout);
			return count;
		}

		/// <summary>
		/// Truncates all rows.
		/// </summary>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		public override void Truncate(int commandTimeout = 30)
		{
			Queries.Truncate(Connection, Transaction, commandTimeout);
		}

		/// <summary>
		/// Updates a row.
		/// </summary>
		/// <param name="obj">The object to update.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the row was updated; false otherwise.</returns>
		public override bool Update(object obj, int commandTimeout = 30)
		{
			bool success = Queries.UpdateObj(Connection, obj, Transaction, commandTimeout);
			return success;
		}

		/// <summary>
		/// Updates a row.
		/// </summary>
		/// <param name="obj">The object to update.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the row was updated; false otherwise.</returns>
		public override bool Update(T obj, int commandTimeout = 30)
		{
			bool success = Queries.Update(Connection, obj, Transaction, commandTimeout);
			return success;
		}

		/// <summary>
		/// Upserts a row.
		/// </summary>
		/// <param name="obj">The object to upsert.</param>
		/// <param name="commandTimeout">Number of seconds before command execution timeout.</param>
		/// <returns>True if the object was upserted; false otherwise.</returns>
		public override bool Upsert(T obj, int commandTimeout = 30)
		{
			bool success = Queries.Upsert(Connection, obj, Transaction, commandTimeout);
			return success;
		}

		#endregion IAccessObjectSync<T>
	}
}
