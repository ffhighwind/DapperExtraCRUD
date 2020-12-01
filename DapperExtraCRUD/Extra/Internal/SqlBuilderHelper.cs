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
using System.Linq;
using System.Text;

namespace Dapper.Extra.Internal
{
	/// <summary>
	/// Creates partial SQL commands that are used by <see cref="SqlBuilder{T}"/>.
	/// </summary>
	public static class SqlBuilderHelper
	{
		/// <summary>
		/// Quotes and escapes a string with single quotes.
		/// </summary>
		/// <param name="str">The content of the string.</param>
		/// <returns>A quoted and escaped string.</returns>
		public static string CreateString(string str)
		{
			return "'" + str.Replace("'", "''") + "'";
		}

		/// <summary>
		/// Creates the VALUES section for insert commands.<para />
		/// VALUES (@a,@b,getdate())
		/// </summary>
		/// <param name="columns">The columns to insert.</param>
		public static string InsertedValues(IEnumerable<SqlColumn> columns)
		{
			StringBuilder sb = new StringBuilder("VALUES (");
			foreach (var column in columns) {
				if (column.InsertValue != null)
					sb.Append(column.InsertValue).Append(',');
				else if (!column.Attributes.HasFlag(SqlColumnAttributes.IgnoreInsert))
					sb.Append($"@{column.Property.Name},");
			}
			sb.Remove(sb.Length - 1, 1); // remove ','
			string result = sb.Append(')').ToString();
			return result;
		}

		/// <summary>
		/// Creates the column list for select commands.<para />
		/// [x] as X, [y], [z] as Z
		/// </summary>
		/// <param name="columns">The columns to select.</param>
		public static string SelectedColumns(IEnumerable<SqlColumn> columns)
		{
			StringBuilder sb = new StringBuilder();
			foreach (SqlColumn column in columns) {
				sb.Append(column.ColumnName);
				if (column.PropertyName != column.ColumnName) {
					sb.Append(" as ").Append(column.PropertyName);
				}
				sb.Append(',');
			}
			sb.Remove(sb.Length - 1, 1); // remove ','
			return sb.ToString();
		}

		/// <summary>
		/// Creates the column list for select commands.<para />
		/// TableName.[x] as X, TableName.[y], TableName.[z] as Z
		/// </summary>
		/// <param name="columns">The columns to select.</param>
		/// <param name="tableName">The table name.</param>
		public static string SelectedColumns(IEnumerable<SqlColumn> columns, string tableName)
		{
			StringBuilder sb = new StringBuilder();
			foreach (SqlColumn column in columns) {
				sb.Append(tableName);
				sb.Append('.');
				sb.Append(column.ColumnName);
				if (column.PropertyName != column.ColumnName) {
					sb.Append(" as ").Append(column.PropertyName);
				}
				sb.Append(',');
			}
			sb.Remove(sb.Length - 1, 1); // remove ','
			return sb.ToString();
		}

		/// <summary>
		/// Creates a SELECT INTO command.
		/// </summary>
		/// <param name="sourceTable">The source table.</param>
		/// <param name="destinationTable">The name of the table to create.</param>
		/// <param name="columns">The columns to copy from the source table.</param>
		/// <param name="whereCondition">The condition that determines which rows to copy.</param>
		public static string SelectIntoTableQuery(string sourceTable, string destinationTable, IEnumerable<SqlColumn> columns, string whereCondition = "WHERE 1=0")
		{
			string sql = $"SELECT {string.Join(",", columns.Select(c => c.ColumnName))} INTO {destinationTable} FROM {sourceTable} " + whereCondition;
			return sql;
		}

		/// <summary>
		/// Creates the SET section for update commands.<para />
		/// SET [x] = @x, [y] = getdate()
		/// </summary>
		/// <param name="columns">The columns to set.</param>
		public static string UpdateSet(IEnumerable<SqlColumn> columns)
		{
			StringBuilder sb = new StringBuilder("\nSET \t");
			foreach (SqlColumn column in columns) {
				if (column.UpdateValue != null)
					sb.Append($"{column.ColumnName} = {column.UpdateValue},\n\t");
				else if (!column.Attributes.HasFlag(SqlColumnAttributes.IgnoreUpdate | SqlColumnAttributes.MatchUpdate))
					sb.Append($"{column.ColumnName} = @{column.Property.Name},\n\t");
			}
			sb.Remove(sb.Length - 3, 3); // remove ",\n\t"
			string result = sb.ToString();
			return result;
		}

		/// <summary>
		/// Creates the SET section for bulk update commands.<para></para>
		/// TableName.[x] = Source.[x], TableName.[y] = getdate()
		/// </summary>
		/// <param name="tableSource">The source table that is used to modify the destination.</param>
		/// <param name="tableDestination">The destination table that is modified.</param>
		/// <param name="columns">The columns to set.</param>
		public static string UpdateSetTables(string tableSource, string tableDestination, IEnumerable<SqlColumn> columns)
		{
			StringBuilder sb = new StringBuilder("\nSET \t");
			foreach (SqlColumn column in columns) {
				if (column.UpdateValue != null)
					sb.Append($"{tableDestination}.{column.ColumnName} = {column.UpdateValue},\n\t");
				else if (!column.Attributes.HasFlag(SqlColumnAttributes.IgnoreUpdate | SqlColumnAttributes.MatchUpdate))
					sb.Append($"{tableDestination}.{column.ColumnName} = {tableSource}.{column.ColumnName},\n\t");
			}
			sb.Remove(sb.Length - 3, 3); // remove ",\n\t"
			string result = sb.ToString();
			return result;
		}

		/// <summary>
		/// Creates the WHERE section for commands.<para />
		/// x = @x AND y = @y
		/// </summary>
		/// <param name="columns">The columns to compare.</param>
		public static string WhereEquals(IEnumerable<SqlColumn> columns)
		{
			StringBuilder sb = new StringBuilder();
			foreach (SqlColumn column in columns) {
				sb.Append($"{column.ColumnName} = @{column.Property.Name}\n\tAND ");
			}
			string result = sb.Remove(sb.Length - 6, 6).ToString(); // remove "\n\tAND "
			return result;
		}

		/// <summary>
		/// Creates the WHERE section for bulk operations.<para></para>
		/// Source.[x] = TableName.[x] AND Source.[y] = TableName.[y]
		/// </summary>
		/// <param name="sourceTable">The source table that is used to modify the destination.</param>
		/// <param name="destinationTable">The destination table that is modified.</param>
		/// <param name="columns">The columns to set.</param>
		public static string WhereEqualsTables(string sourceTable, string destinationTable, IEnumerable<SqlColumn> columns)
		{
			StringBuilder sb = new StringBuilder();
			foreach (SqlColumn column in columns) {
				sb.Append($"{sourceTable}.{column.ColumnName} = {destinationTable}.{column.ColumnName}\n\tAND ");
			}
			sb.Remove(sb.Length - 6, 6); // remove "\n\tAND "
			string result = sb.ToString();
			return result;
		}
	}
}
