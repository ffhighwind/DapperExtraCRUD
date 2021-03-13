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
using Fasterflect;

namespace Dapper.Extra
{
	internal class TableEqualityComparer<T> : IEqualityComparer<T>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TableEqualityComparer{T}"/> class.
		/// </summary>
		/// <param name="typeInfo">The information about the table.</param>
		public TableEqualityComparer(SqlTypeInfo typeInfo)
		{
			InitialHash = typeInfo.FullyQualifiedTableName.GetHashCode() * -1977;
			StringComparer = typeInfo.Adapter.StringComparer;
			List<MemberGetter> stringGetters = new List<MemberGetter>();
			List<MemberGetter> otherGetters = new List<MemberGetter>();
			List<MemberGetter> byteGetters = new List<MemberGetter>();
			foreach (SqlColumn column in typeInfo.EqualityColumns) {
				MemberGetter getter = column.Getter;
				if (column.Type == typeof(byte[]))
					byteGetters.Add(getter);
				else if (column.Type == typeof(string) && StringComparer != System.StringComparer.Ordinal)
					stringGetters.Add(getter);
				else
					otherGetters.Add(getter);
			}
			StringGetters = stringGetters.Count == 0 ? Array.Empty<MemberGetter>() : stringGetters.ToArray();
			Getters = otherGetters.Count == 0 ? Array.Empty<MemberGetter>() : otherGetters.ToArray();
			ByteArrayGetters = byteGetters.Count == 0 ? Array.Empty<MemberGetter>() : byteGetters.ToArray();
		}

		private readonly MemberGetter[] Getters;
		private readonly MemberGetter[] StringGetters;
		private readonly MemberGetter[] ByteArrayGetters;
		private readonly IEqualityComparer<string> StringComparer;

		private readonly int InitialHash;

		/// <summary>
		/// Determines whether the specified object is equal to the current object.
		/// </summary>
		/// <param name="x">The first object to compare.</param>
		/// <param name="y">The object to compare with the current object.</param>
		/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
		public bool Equals(T x, T y)
		{
			foreach (MemberGetter getter in Getters) {
				if (!Equals(getter(x), getter(y)))
					return false;
			}
			foreach (MemberGetter getter in StringGetters) {
				if (!StringComparer.Equals(getter(x) as string, getter(y) as string))
					return false;
			}
			foreach (MemberGetter getter in ByteArrayGetters) {
				byte[] xVal = getter(x) as byte[];
				byte[] yVal = getter(y) as byte[];
				if (xVal != yVal) {
					if (xVal == null || yVal == null || xVal.Length != yVal.Length)
						return false;
					if (!xVal.SequenceEqual(yVal))
						return false;
				}
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
			foreach (MemberGetter getter in Getters) {
				hashCode *= 125235693;
				object value = getter(obj);
				if (value != null)
					hashCode += value.GetHashCode();
			}
			foreach (MemberGetter getter in StringGetters) {
				hashCode *= 126247697;
				if (getter(obj) is string value)
					hashCode += StringComparer.GetHashCode(value);
			}
			// byte[] is an awful key choice
			foreach (MemberGetter getter in ByteArrayGetters) {
				hashCode *= 126147695;
				if (getter(obj) is byte[] value) {
					for (int i = 0; i < value.Length; i++) {
						hashCode = hashCode * 5315137 + value[i].GetHashCode();
					}
				}
			}
			return hashCode;
		}
	}

	internal class TableKeyEqualityComparer<T> : IEqualityComparer<T>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TableKeyEqualityComparer{T}"/> class.
		/// </summary>
		/// <param name="tableName">The tableName<see cref="string"/></param>
		/// <param name="equalityColumn">The equalityColumn<see cref="SqlColumn"/></param>
		public TableKeyEqualityComparer(string tableName, SqlColumn equalityColumn)
		{
			Getter = equalityColumn.Getter;
			InitialHash = tableName.GetHashCode() * -1977;
		}

		private readonly MemberGetter Getter;

		private readonly int InitialHash;

		/// <summary>
		/// Determines whether the specified object is equal to the current object.
		/// </summary>
		/// <param name="x">The first object to compare.</param>
		/// <param name="y">The object to compare with the current object.</param>
		/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
		public bool Equals(T x, T y)
		{
			bool success = Equals(Getter(x), Getter(y));
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
			object value = Getter(obj);
			if (value != null) {
				hashCode = hashCode * 126247697 + value.GetHashCode();
			}
			return hashCode;
		}
	}
}
