using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fasterflect;

namespace Dapper.Extra.Internal.Adapters
{
	internal class SqlServerAdapter : SqlAdapterImpl
	{
		public SqlServerAdapter() : base (SqlSyntax.SQLServer)
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

		protected string SelectIdentityLongQuery { get; set; }

		/// <summary>
		/// Creates an SQL command to select a generated row's identity (auto-increment).
		/// </summary>
		/// <param name="type">The type of the identity.</param>
		/// <returns>A command to select a generated row's identity.</returns>
		public override string SelectIdentityQuery(Type type)
		{
			return type == typeof(long) || type == typeof(ulong) ? SelectIdentityLongQuery : SelectIntIdentityQuery;
		}

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
	}
}