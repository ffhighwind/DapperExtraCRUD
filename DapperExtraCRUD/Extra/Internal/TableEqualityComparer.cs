// Released under MIT License 
// Copyright(c) 2018 Wesley Hamilton
// License: https://www.mit.edu/~amini/LICENSE.md
// Home page: https://github.com/ffhighwind/DapperExtraCRUD

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Extra.Internal
{
	internal class TableKeyEqualityComparer<T> : IEqualityComparer<T>
	{
		public TableKeyEqualityComparer(string tableName, SqlColumn equalityColumn)
		{
			EqualityColumn = equalityColumn;
			InitialHash = tableName.GetHashCode() * 31619117;
		}

		private SqlColumn EqualityColumn { get; set; }
		private int InitialHash { get; set; }

		/// <summary>
		/// Determines whether the specified object is equal to the current object.
		/// </summary>
		/// <param name="x">The first object to compare.</param>
		/// <param name="y">The object to compare with the current object.</param>
		/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
		public bool Equals(T x, T y)
		{
			PropertyInfo property = EqualityColumn.Property;
			bool success = object.Equals(property.GetValue(x), property.GetValue(y));
			return success;
		}

		/// <summary>
		/// Generates a hash code for the current object.
		/// </summary>
		/// <param name="obj">The <see cref="object"/> for which a hash code is to be returned.</param>
		/// <returns>A hash code for the current object.</returns>
		public int GetHashCode(T obj)
		{
			int hashCode = InitialHash;
			object value = EqualityColumn.Property.GetValue(obj);
			if (value != null) {
				hashCode ^= value.GetHashCode();
			}
			return hashCode;
		}
	}


	internal class TableEqualityComparer<T> : IEqualityComparer<T>
	{
		public TableEqualityComparer(string tableName, IReadOnlyList<SqlColumn> equalityColumns)
		{
			EqualityColumns = equalityColumns;
			InitialHash = tableName.GetHashCode();
		}

		private IReadOnlyList<SqlColumn> EqualityColumns { get; set; }
		private int InitialHash { get; set; }

		/// <summary>
		/// Determines whether the specified object is equal to the current object.
		/// </summary>
		/// <param name="x">The first object to compare.</param>
		/// <param name="y">The object to compare with the current object.</param>
		/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
		public bool Equals(T x, T y)
		{
			foreach (SqlColumn column in EqualityColumns) {
				PropertyInfo property = column.Property;
				if (!object.Equals(property.GetValue(x), property.GetValue(y)))
					return false;
			}
			return true;
		}

		/// <summary>
		/// Generates a hash code for the current object.
		/// </summary>
		/// <param name="obj">The <see cref="object"/> for which a hash code is to be returned.</param>
		/// <returns>A hash code for the current object.</returns>
		public int GetHashCode(T obj)
		{
			int hashCode = InitialHash;
			foreach (SqlColumn column in EqualityColumns) {
				object value = column.Property.GetValue(obj);
				hashCode = hashCode * 31619117;
				if (value != null) {
					hashCode ^= value.GetHashCode();
				}
			}
			return hashCode;
		}
	}
}
