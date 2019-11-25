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
