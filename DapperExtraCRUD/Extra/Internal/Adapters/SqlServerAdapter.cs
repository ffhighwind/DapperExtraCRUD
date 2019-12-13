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
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Fasterflect;

namespace Dapper.Extra.Internal.Adapters
{
	/// <summary>
	/// An <see cref="SqlAdapter"/> that generates SQL commands for Microsoft SQL Server.
	/// </summary>
	internal class SqlServerAdapter : SqlAdapterImpl
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SqlServerAdapter"/> class.
		/// </summary>
		internal SqlServerAdapter() : base(SqlDialect.SQLServer)
		{
			QuoteLeft = "[";
			QuoteRight = "]";
			EscapeQuote = "[]]";
			SelectIntIdentityQuery = "SELECT CAST(SCOPE_IDENTITY() as INT) as [Id];";
			SelectIdentityLongQuery = "SELECT CAST(SCOPE_IDENTITY() as BIGINT) as [Id];";
			DropTempTableIfExistsQuery = @"IF OBJECT_ID('tempdb..{0}') IS NOT NULL 
	DROP TABLE {0}";
			TruncateTableQuery = "TRUNCATE TABLE {0};";
			TempTableName = "#{0}";
			CreateTempTable = "";
			LimitQuery = @"TOP({0})
{1}";
		}

		/// <summary>
		/// Gets or sets the SelectIdentityLongQuery
		/// </summary>
		protected string SelectIdentityLongQuery { get; set; }

		/// <summary>
		/// Bulk copies rows into a table.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="connection">The connection<see cref="IDbConnection"/></param>
		/// <param name="objs">The objs<see cref="IEnumerable{T}"/></param>
		/// <param name="transaction">The transaction<see cref="IDbTransaction"/></param>
		/// <param name="tableName">The tableName<see cref="string"/></param>
		/// <param name="factory">The factory<see cref="DataReaderFactory"/></param>
		/// <param name="columns">The columns<see cref="IEnumerable{SqlColumn}"/></param>
		/// <param name="commandTimeout">The commandTimeout<see cref="int"/></param>
		/// <param name="options">The options<see cref="SqlBulkCopyOptions"/></param>
		public override void BulkInsert<T>(IDbConnection connection, IEnumerable<T> objs, IDbTransaction transaction, string tableName,
			DataReaderFactory factory, IEnumerable<SqlColumn> columns, int commandTimeout = 30, SqlBulkCopyOptions options = SqlBulkCopyOptions.Default)
		{
			DbDataReader dataReader = factory.Create(objs);
			using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection as SqlConnection, options, transaction as SqlTransaction)) {
				bulkCopy.DestinationTableName = tableName;
				bulkCopy.BulkCopyTimeout = commandTimeout;
				foreach (SqlColumn column in columns) {
					bulkCopy.ColumnMappings.Add(column.Property.Name, column.ColumnName);
				}
				bulkCopy.WriteToServer(dataReader);
			}
		}

		/// <summary>
		/// Creates an SQL command to select a generated row's identity (auto-increment).
		/// </summary>
		/// <param name="type">The type of the identity.</param>
		/// <returns>A command to select a generated row's identity.</returns>
		public override string SelectIdentityQuery(Type type)
		{
			return type == typeof(long) || type == typeof(ulong) ? SelectIdentityLongQuery : SelectIntIdentityQuery;
		}
	}
}
