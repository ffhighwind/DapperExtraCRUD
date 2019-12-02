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
using System.Reflection;
using Dapper.Extra.Annotations;
using Fasterflect;

namespace Dapper.Extra.Internal
{
	/// <summary>
	/// Stores metadata for a property/column.
	/// </summary>
	public sealed class SqlColumn
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SqlColumn"/> class.
		/// </summary>
		/// <param name="property">The property<see cref="PropertyInfo"/></param>
		/// <param name="columnName">The columnName<see cref="string"/></param>
		/// <param name="ordinal">The ordinal<see cref="int"/></param>
		internal SqlColumn(PropertyInfo property, string columnName, int ordinal)
		{
			Property = property;
			ColumnName = columnName;
			Ordinal = ordinal;
		}

		/// <summary>
		/// The column attributes.
		/// </summary>
		public SqlColumnAttributes Attributes { get; internal set; }

		/// <summary>
		/// The name of the column. This is quoted using the <see cref="SqlAdapter"/> if necessary.
		/// </summary>
		public string ColumnName { get; private set; }

		/// <summary>
		/// Gets the value of the property for an object.
		/// </summary>
		public MemberGetter Getter => Property.CanRead ? Reflect.PropertyGetter(Property) : null;

		/// <summary>
		/// Ignores the column for inserts.
		/// </summary>
		public bool IgnoreDelete => (Attributes & SqlColumnAttributes.IgnoreDelete) != 0;

		/// <summary>
		/// Ignores the column for inserts.
		/// </summary>
		public bool IgnoreInsert => (Attributes & SqlColumnAttributes.IgnoreInsert) != 0;

		/// <summary>
		/// Ignores the column for selects.
		/// </summary>
		public bool IgnoreSelect => (Attributes & SqlColumnAttributes.IgnoreSelect) != 0;

		/// <summary>
		/// Ignores the column for updates.
		/// </summary>
		public bool IgnoreUpdate => (Attributes & SqlColumnAttributes.IgnoreUpdate) != 0;

		/// <summary>
		/// Determines if the column should be synchronized after inserts.
		/// </summary>
		public bool InsertAutoSync => (Attributes & SqlColumnAttributes.InsertAutoSync) != 0;

		/// <summary>
		/// The value from the <see cref="IgnoreInsertAttribute"/> if it exists.
		/// </summary>
		public string InsertValue { get; internal set; }

		/// <summary>
		/// The column is an auto-increment key.
		/// </summary>
		public bool IsAutoKey => (Attributes & SqlColumnAttributes.AutoKey) == SqlColumnAttributes.AutoKey;

		/// <summary>
		/// The column is part of the primary key.
		/// </summary>
		public bool IsKey => (Attributes & SqlColumnAttributes.Key) != 0;

		/// <summary>
		/// Determines if the column must match the database on deletes.
		/// </summary>
		public bool MatchDelete => (Attributes & SqlColumnAttributes.MatchDelete) != 0;

		/// <summary>
		/// Determines if the column must matched the database on updates.
		/// </summary>
		public bool MatchUpdate => (Attributes & SqlColumnAttributes.MatchUpdate) != 0;

		/// <summary>
		/// Prevents the column from being included in commands.
		/// </summary>
		public bool NotMapped => (Attributes & SqlColumnAttributes.NotMapped) == SqlColumnAttributes.NotMapped;

		/// <summary>
		/// The zero-based ordinal of the column.
		/// </summary>
		public int Ordinal { get; internal set; }

		/// <summary>
		/// The <see cref="System.Reflection.PropertyInfo"/> that this column represents.
		/// </summary>
		public PropertyInfo Property { get; private set; }

		/// <summary>
		/// Sets the value of athe property for an object.
		/// </summary>
		public MemberSetter Setter => Property.CanWrite ? Reflect.PropertySetter(Property) : null;

		/// <summary>
		/// The <see cref="System.Reflection.PropertyInfo"/> type.
		/// </summary>
		public Type Type => Property.PropertyType;

		/// <summary>
		/// Determines if the column should be synchronized after updates.
		/// </summary>
		public bool UpdateAutoSync => (Attributes & SqlColumnAttributes.UpdateAutoSync) != 0;

		/// <summary>
		/// The value from the <see cref="IgnoreUpdateAttribute"/> or <see cref="MatchUpdateAttribute"/> if it exists. 
		/// </summary>
		public string UpdateValue { get; internal set; }
	}
}
