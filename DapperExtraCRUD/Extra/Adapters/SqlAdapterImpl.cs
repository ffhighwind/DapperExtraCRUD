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

namespace Dapper.Extra.Internal.Adapters
{
	internal abstract class SqlAdapterImpl : ISqlAdapter
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SqlAdapterImpl"/> class.
		/// </summary>
		/// <param name="dialect">The dialect<see cref="SqlDialect"/></param>
		internal SqlAdapterImpl(SqlDialect dialect)
		{
			Dialect = dialect;
		}

		#region Properties

		/// <summary>
		/// A format string where {1} is the query excluding the SELECT portion and {0} is the limit.
		/// </summary>
		public string LimitQuery { get; protected set; }

		/// <summary>
		/// The dialect used to generate SQL commands.
		/// </summary>
		public SqlDialect Dialect { get; private set; }

		/// <summary>
		/// The 'CREATE TABLE AS' part of a 'SELECT INTO' command.
		/// </summary>
		protected string CreateTempTable { get; set; }

		protected string DropTempTableIfExistsQuery { get; set; }

		protected string EscapeQuote { get; set; }

		protected string QuoteLeft { get; set; }

		protected string QuoteRight { get; set; }

		protected string SelectIntIdentityQuery { get; set; }

		/// <summary>
		/// Converts a table name to a temporary table name.
		/// <code>"#{0}"</code>
		/// <code>"{0}"</code>
		/// </summary>
		protected string TempTableName { get; set; }

		protected string TruncateTableQuery { get; set; }

		/// <summary>
		/// The SQL command that returns the current date.
		/// </summary>
		public string CurrentDate { get; protected set; }

		/// <summary>
		/// The SQL command that returns the current datetime.
		/// </summary>
		public string CurrentDateTime { get; protected set; }

		/// <summary>
		/// The SQL command that returns the current UTC datetime.
		/// </summary>
		public string CurrentDateTimeUtc { get; protected set; }

		/// <summary>
		/// The SQL command that returns the current UTC date.
		/// </summary>
		public string CurrentDateUtc { get; protected set; }


		/// <summary>
		/// The <see cref="System.StringComparer"/> equivalent to the database's string collation. 
		/// This is used to generate the <see cref="IEqualityComparer{T}"/>.
		/// By default this is <see cref="StringComparer.OrdinalIgnoreCase"/>.
		/// </summary>
		public IEqualityComparer<string> StringComparer => System.StringComparer.OrdinalIgnoreCase;

		#endregion

		#region Methods

		/// <summary>
		/// Inserts data into a table using <see cref="SqlBulkCopy"/>.
		/// </summary>
		/// <typeparam name="T">The type of object to insert.</typeparam>
		/// <param name="connection">The database connection.</param>
		/// <param name="objs">The objects to insert into the table.</param>
		/// <param name="transaction">The transaction for the connection, or null if an internal transaction should be used.</param>
		/// <param name="tableName">The name of the table.</param>
		/// <param name="factory">The factory from an <see cref="SqlBuilder{T}"/>.</param>
		/// <param name="columns">The column mappings for the table.</param>
		/// <param name="commandTimeout">The command timeout in seconds. 0 or null prevent a timeout.</param>
		/// <param name="options">The bulk copy options used when transfering data.</param>
		public virtual void BulkInsert<T>(IDbConnection connection, IEnumerable<T> objs, IDbTransaction transaction, string tableName,
			DataReaderFactory factory, IEnumerable<SqlColumn> columns, int commandTimeout = 30, SqlBulkCopyOptions options = SqlBulkCopyOptions.Default) where T : class
		{
			// TODO -- CSV mass import etc
			throw new NotSupportedException(nameof(SqlAdapterImpl.BulkInsert) + " is not supported for " + Dialect);
		}

		/// <summary>
		/// Creates a name for a temporary table based on an input table name. This currently only affects SQL Server.
		/// </summary>
		/// <param name="tableName">The table name.</param>
		/// <returns>A temporary table name.</returns>
		public string CreateTempTableName(string tableName)
		{
			return string.Format(TempTableName, tableName);
		}

		/// <summary>
		/// Creates an SQL command to drop a temporary table if it exists.
		/// </summary>
		/// <param name="tableName">The temporary table name.</param>
		/// <returns>A command to drop a table if it exists.</returns>
		public string DropTempTableIfExists(string tableName)
		{
			return string.Format(DropTempTableIfExistsQuery, tableName);
		}

		/// <summary>
		/// Quotes an identifier if necessary. This uses <see cref="Dapper.Extra.ExtraUtil.IsSqlIdentifier(string)"/> to determine if it needs to be quoted.
		/// </summary>
		/// <param name="identifier">The identifier.</param>
		/// <returns>The original identifier if it does not need to be quoted, otherwise a quoted identifier.</returns>
		public string QuoteIdentifier(string identifier)
		{
			return ExtraUtil.IsSqlIdentifier(identifier)
				? identifier
				: QuoteLeft + identifier.Replace(QuoteRight, EscapeQuote) + QuoteRight;
		}

		/// <summary>
		/// Creates an SQL command to select a generated row's identity (auto-increment).
		/// </summary>
		/// <param name="type">The type of the identity.</param>
		/// <returns>A command to select a generated row's identity.</returns>
		public virtual string SelectIdentityQuery(Type type)
		{
			return SelectIntIdentityQuery;
		}

		/// <summary>
		/// Creates an SQL command to clone specific columns from a table into a temporary table.
		/// </summary>
		/// <param name="sourceTable">The table to clone.</param>
		/// <param name="tempTable">The temporary to create.</param>
		/// <param name="columns">The columns to copy.</param>
		/// <returns>A command to create a temporary table with specific columns.</returns>
		public string SelectIntoTempTable(string sourceTable, string tempTable, IEnumerable<SqlColumn> columns)
		{
			return string.Format(CreateTempTable, tempTable) + SqlBuilderHelper.SelectIntoTableQuery(sourceTable, tempTable, columns);
		}

		/// <summary>
		/// Creates an SQL command to truncate a table.
		/// </summary>
		/// <param name="tableName">The table name.</param>
		/// <returns>A command to truncate a table.</returns>
		public string TruncateTable(string tableName)
		{
			return string.Format(TruncateTableQuery, tableName);
		}

		#endregion
	}
}
