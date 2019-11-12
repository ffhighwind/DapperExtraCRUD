using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dapper.Extra.Interfaces;

namespace Dapper.Extra.Internal
{
	public sealed class SqlColumn
	{
		/// <summary>
		/// Constructs an SqlColumn.
		/// </summary>
		internal SqlColumn(PropertyInfo property, string columnName)
		{
			Property = property;
			ColumnName = columnName;
		}

		/// <summary>
		/// The property that this column represents.
		/// </summary>
		public PropertyInfo Property { get; private set; }

		public Type Type => Property.PropertyType;
		/// <summary>
		/// The name of the column. This will be quoted using the <see cref="SqlSyntax"/> from the constructor if necessary.
		/// </summary>
		public string ColumnName { get; private set; }
		/// <summary>
		/// This is the value stored in the <see cref="IgnoreInsertAttribute"/> if one exists. If it is <see langword="null"/> then it will be ignored. 
		/// It will be the name of the property e.g. @Property if the attribute does not exist.
		/// </summary>
		public string InsertValue { get; internal set; }
		/// <summary>
		/// This is the value stored in the <see cref="IgnoreUpdateAttribute"/> if one exists. If it is <see langword="null"/> then it will be ignored. 
		/// It will be the name of the property e.g. @Property if the attribute does not exist.
		/// </summary>
		public string UpdateValue { get; internal set; }
		public bool IsKey => (Attributes & SqlColumnAttributes.Key) != 0;
		public bool IsAutoKey => (Attributes & SqlColumnAttributes.AutoKey) == SqlColumnAttributes.AutoKey;
		public bool IgnoreSelect => (Attributes & SqlColumnAttributes.IgnoreSelect) != 0;
		public bool IgnoreInsert => (Attributes & SqlColumnAttributes.IgnoreInsert) != 0;
		public bool IgnoreUpdate => (Attributes & SqlColumnAttributes.IgnoreUpdate) != 0;
		public bool Ignore => (Attributes & SqlColumnAttributes.Ignore) == SqlColumnAttributes.Ignore;
		public bool MatchUpdate => (Attributes & SqlColumnAttributes.MatchUpdate) != 0;
		public bool MatchDelete => (Attributes & SqlColumnAttributes.MatchDelete) != 0;
		public SqlColumnAttributes Attributes { get; internal set; }

		// Getter
		// Setter
		// TypeHandler
	}
}
