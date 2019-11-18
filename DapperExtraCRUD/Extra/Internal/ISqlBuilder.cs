using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Extra.Internal
{
	public interface ISqlBuilder
	{
		/// <summary>
		/// The internal <see cref="ISqlQueries{T}"/> or <see cref="ISqlQueries{T, KeyType}"/> object.
		/// </summary>
		object QueriesObject { get; }
		/// <summary>
		/// The quoted table name or the class name.
		/// </summary>
		string TableName { get; }
		/// <summary>
		/// The temporary table name for bulk operations.
		/// </summary>
		string BulkStagingTable { get; }
		/// <summary>
		/// The syntax used to generate SQL commands.
		/// </summary>
		SqlSyntax Syntax { get; }
		/// <summary>
		/// Stores metadata for for the given type.
		/// </summary>
		SqlTypeInfo Info { get; }
		/// <summary>
		///  All valid columns for the given type.
		/// </summary>
		IReadOnlyList<SqlColumn> Columns { get; }
	}
}
