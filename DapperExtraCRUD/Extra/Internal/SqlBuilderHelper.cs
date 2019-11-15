using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Extra.Internal
{
	/// <summary>
	/// Creates partial SQL commands that are used by <see cref="SqlBuilder"/>.
	/// </summary>
	public static class SqlBuilderHelper
	{
		public static string SelectIntoTableQuery(string sourceTable, string destinationTable, IEnumerable<SqlColumn> columns, string whereCondition = "WHERE 1=0")
		{
			string sql = $"\nSELECT {string.Join(",", columns.Select(c => c.ColumnName))} INTO {destinationTable} FROM {sourceTable} " + whereCondition;
			return sql;
		}

		/// <summary>
		/// Creates the WHERE section for bulk operations.<para></para>
		/// Source.[x] = TableName.[x] AND Source.[y] = TableName.[y]
		/// </summary>
		/// <param name="tableSource">The source table that is used to modify the destination.</param>
		/// <param name="tableDestination">The destination table that is modified.</param>
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

		/// <summary>
		/// Creates the WHERE section for commands.<para></para>
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
		/// Creates the column list for select commands.<para></para>
		/// [x] as X, [y], [z] as Z
		/// </summary>
		/// <param name="columns">The columns to select.</param>
		public static string SelectedColumns(IEnumerable<SqlColumn> columns)
		{
			StringBuilder sb = new StringBuilder();
			foreach (SqlColumn column in columns) {
				sb.Append(column.ColumnName);
				if (column.Property.Name != column.ColumnName) {
					sb.Append(" as ").Append(column.Property.Name);
				}
				sb.Append(',');
			}
			sb.Remove(sb.Length - 1, 1); // remove ','
			return sb.ToString();
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
		/// Creates the SET section for update commands.<para></para>
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
		/// Creates the VALUES section for insert commands.<para></para>
		/// VALUES(@a,@b,getdate()) 
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
	}
}
