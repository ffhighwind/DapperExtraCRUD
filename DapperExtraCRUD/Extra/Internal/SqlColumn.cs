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
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dapper.Extra.Annotations;

namespace Dapper.Extra.Internal
{
	/// <summary>
	/// Stores metadata for a property/column.
	/// </summary>
	public sealed class SqlColumn
	{
		/// <summary>
		/// Constructs an <see cref="SqlColumn"/>.
		/// </summary>
		internal SqlColumn(PropertyInfo property, string columnName, int ordinal)
		{
			Property = property;
			ColumnName = columnName;
		}

		/// <summary>
		/// The <see cref="PropertyInfo"/> that this column represents.
		/// </summary>
		public PropertyInfo Property { get; private set; }

		/// <summary>
		/// The <see cref="PropertyInfo"/> type.
		/// </summary>
		public Type Type => Property.PropertyType;

		/// <summary>
		/// The <see cref="DbType"/> that represents this column's type.
		/// </summary>
		public DbType DbType => SqlInternal.TryGetDbType(Property.PropertyType, out DbType dbType) ? dbType : DbType.Object;
		/// <summary>
		/// The name of the column. This is quoted using the <see cref="SqlAdapter"/> if necessary.
		/// </summary>
		public string ColumnName { get; private set; }
		/// <summary>
		/// The zero-based ordinal of the column.
		/// </summary>
		public int Ordinal { get; internal set; }
		/// <summary>
		/// The value stored in the <see cref="IgnoreInsertAttribute"/> if one exists.
		/// </summary>
		public string InsertValue { get; internal set; }
		/// <summary>
		/// The value stored in the <see cref="IgnoreUpdateAttribute"/> or <see cref="MatchUpdateAttribute"/> if one exists. 
		/// </summary>
		public string UpdateValue { get; internal set; }
		public bool IsKey => (Attributes & SqlColumnAttributes.Key) != 0;
		public bool IsAutoKey => (Attributes & SqlColumnAttributes.AutoKey) == SqlColumnAttributes.AutoKey;
		public bool IgnoreSelect => (Attributes & SqlColumnAttributes.IgnoreSelect) != 0;
		public bool IgnoreInsert => (Attributes & SqlColumnAttributes.IgnoreInsert) != 0;
		public bool IgnoreUpdate => (Attributes & SqlColumnAttributes.IgnoreUpdate) != 0;
		public bool IgnoreDelete => (Attributes & SqlColumnAttributes.IgnoreDelete) != 0;
		public bool NotMapped => (Attributes & SqlColumnAttributes.NotMapped) == SqlColumnAttributes.NotMapped;
		public bool MatchUpdate => (Attributes & SqlColumnAttributes.MatchUpdate) != 0;
		public bool MatchDelete => (Attributes & SqlColumnAttributes.MatchDelete) != 0;
		public bool InsertAutoSync => (Attributes & SqlColumnAttributes.InsertAutoSync) != 0;
		public bool UpdateAutoSync => (Attributes & SqlColumnAttributes.UpdateAutoSync) != 0;
		public SqlColumnAttributes Attributes { get; internal set; }

		// Getter
		// Setter
		// TypeHandler
	}
}
