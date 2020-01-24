#region License
// Released under MIT License 
// License: https://opensource.org/licenses/MIT
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
using System.Data;
using System.Data.SqlClient;
using Fasterflect;

namespace Dapper.Extra.Adapters
{
	/// <summary>
	/// An <see cref="SqlAdapter"/> that changes the default behavior of bulk SQL commands.
	/// </summary>
	public class BulkOptionsAdapter : ISqlAdapter
	{
		private readonly ISqlAdapter Adapter;

		private readonly SqlBulkCopyOptions OptionsMask;

		/// <summary>
		/// Constructs an <see cref="ISqlAdapter"/> that 
		/// </summary>
		/// <param name="adapter">The adapter to copy. By default this is the SQL Server adapter.</param>
		/// <param name="optionsMask">The <see cref="SqlBulkCopyOptions"/> mask that limits the options 
		/// allowed for all bulk operations. By default this prevents triggers from being fired.</param>
		public BulkOptionsAdapter(ISqlAdapter adapter = null, SqlBulkCopyOptions optionsMask = ~SqlBulkCopyOptions.FireTriggers)
		{
			Adapter = adapter ?? SqlAdapter.SQLServer;
			OptionsMask = optionsMask;
		}

		public string LimitQuery => Adapter.LimitQuery;

		public SqlDialect Dialect => Adapter.Dialect;

		public string CurrentDate => Adapter.CurrentDate;

		public string CurrentDateTime => Adapter.CurrentDateTime;

		public string CurrentDateTimeUtc => Adapter.CurrentDateTimeUtc;

		public string CurrentDateUtc => Adapter.CurrentDateUtc;

		public void BulkInsert<T>(IDbConnection connection, IEnumerable<T> objs, IDbTransaction transaction, string tableName, DataReaderFactory factory, IEnumerable<SqlColumn> columns, int commandTimeout = 30, SqlBulkCopyOptions options = SqlBulkCopyOptions.Default) where T : class
		{
			Adapter.BulkInsert(connection, objs, transaction, tableName, factory, columns, commandTimeout, options & OptionsMask);
		}

		public string CreateTempTableName(string tableName)
		{
			return Adapter.CreateTempTableName(tableName);
		}

		public string DropTempTableIfExists(string tableName)
		{
			return Adapter.DropTempTableIfExists(tableName);
		}

		public string QuoteIdentifier(string identifier)
		{
			return Adapter.QuoteIdentifier(identifier);
		}

		public string SelectIdentityQuery(Type type)
		{
			return Adapter.SelectIdentityQuery(type);
		}

		public string SelectIntoTempTable(string sourceTable, string tempTable, IEnumerable<SqlColumn> columns)
		{
			return Adapter.SelectIntoTempTable(sourceTable, tempTable, columns);
		}

		public string TruncateTable(string tableName)
		{
			return Adapter.TruncateTable(tableName);
		}
	}
}
