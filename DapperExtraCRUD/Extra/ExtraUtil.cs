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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Dapper.Extra.Internal;
using Fasterflect;

namespace Dapper.Extra
{
	/// <summary>
	/// SQL utilities.
	/// </summary>
	public static class ExtraUtil
	{
		internal static Dictionary<Type, DbType> DbTypeMap = new Dictionary<Type, DbType>() {
			[typeof(byte)] = DbType.Byte,
			[typeof(sbyte)] = DbType.SByte,
			[typeof(short)] = DbType.Int16,
			[typeof(ushort)] = DbType.UInt16,
			[typeof(int)] = DbType.Int32,
			[typeof(uint)] = DbType.UInt32,
			[typeof(long)] = DbType.Int64,
			[typeof(ulong)] = DbType.UInt64,
			[typeof(float)] = DbType.Single,
			[typeof(double)] = DbType.Double,
			[typeof(decimal)] = DbType.Decimal,
			[typeof(bool)] = DbType.Boolean,
			[typeof(string)] = DbType.String,
			[typeof(char)] = DbType.StringFixedLength,
			[typeof(Guid)] = DbType.Guid,
			[typeof(DateTime)] = DbType.DateTime,
			[typeof(DateTimeOffset)] = DbType.DateTimeOffset,
			[typeof(TimeSpan)] = DbType.Time,
			[typeof(byte[])] = DbType.Binary,
			[typeof(byte?)] = DbType.Byte,
			[typeof(sbyte?)] = DbType.SByte,
			[typeof(short?)] = DbType.Int16,
			[typeof(ushort?)] = DbType.UInt16,
			[typeof(int?)] = DbType.Int32,
			[typeof(uint?)] = DbType.UInt32,
			[typeof(long?)] = DbType.Int64,
			[typeof(ulong?)] = DbType.UInt64,
			[typeof(float?)] = DbType.Single,
			[typeof(double?)] = DbType.Double,
			[typeof(decimal?)] = DbType.Decimal,
			[typeof(bool?)] = DbType.Boolean,
			[typeof(char?)] = DbType.StringFixedLength,
			[typeof(Guid?)] = DbType.Guid,
			[typeof(DateTime?)] = DbType.DateTime,
			[typeof(DateTimeOffset?)] = DbType.DateTimeOffset,
			[typeof(TimeSpan?)] = DbType.Time,
			[typeof(object)] = DbType.Object
		};

		/// <summary>
		/// Returns the <see cref="DbType"/> that represents the given <see cref="Type"/>. This can be used for switch
		/// statements when handling objects being passed to an SQL database.
		/// </summary>
		/// <param name="type">The type to determine the <see cref="DbType"/> for.</param>
		/// <returns>The <see cref="DbType"/> representing the type if one exists; otherwise <see cref="DbType.Object"/>.</returns>
		public static DbType GetDbType(Type type)
		{
			if (!DbTypeMap.TryGetValue(type, out DbType dbType)) {
				dbType = type.IsEnum ? DbType.Int32 : DbType.Object;
			}
			return dbType;
		}

		/// <summary>
		/// Returns whether the value of an object with the given type should be quoted in SQL.
		/// </summary>
		/// <param name="type">The type of object.</param>
		/// <returns>True if the type should be quoted; false otherwise.</returns>
		public static bool IsQuotedSqlType(Type type)
		{
			TypeCode typeCode = Type.GetTypeCode(type);
			switch (typeCode) {
				case TypeCode.Char:
				case TypeCode.DateTime:
				case TypeCode.String:
					return true;
				case TypeCode.Object:
					return type == typeof(Guid) || type == typeof(DateTimeOffset) || type == typeof(TimeSpan);
			}
			return false;
		}

		/// <summary>
		/// Checks if the input string qualifies as a basic identifier. This will return false if 
		/// the string does not match the following regular expression: [a-zA-Z_][a-zA-Z0-9_]*
		/// </summary>
		/// <param name="identifier">The identifier.</param>
		/// <returns>True if input qualifies as a basic identifier; otherwise false.</returns>
		public static bool IsSqlIdentifier(string identifier)
		{
			char c = identifier[0];
			if (!((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_'))
				return false;
			for (int i = 1; i < identifier.Length; i++) {
				c = identifier[i];
				if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9' || c == '_')) {
					continue;
				}
				return false;
			}
			return true;
		}

		/// <summary>
		/// Returns the SQL string and <see cref="DbType"/> that represent an object.
		/// </summary>
		/// <param name="value">The object to obtain the SQL value of.</param>
		/// <param name="str">The SQL string representation of the value. This will be <see langword="null"/> if the value cannot be determined.</param>
		/// <returns>The <see cref="DbType"/> that represents the object's <see cref="Type"/>.</returns>
		public static DbType SqlValue(object value, out string str)
		{
			if (value == null) {
				str = "NULL";
				return DbType.Object;
			}
			DbType dbType = GetDbType(value.GetType());
			switch (dbType) {
				case DbType.AnsiStringFixedLength:
				case DbType.StringFixedLength:
					if (value is char ch) {
						str = ch == '\'' ? "''''" : "'" + value.ToString() + "'";
						break;
					}
					goto case DbType.Xml;
				case DbType.AnsiString:
				case DbType.String:
				case DbType.Xml:
					str = "'" + value.ToString().Replace("'", "''") + "'";
					break;
				case DbType.Boolean:
					str = ((bool)value) ? "TRUE" : "FALSE";
					break;
				case DbType.Byte:
				case DbType.Int16:
				case DbType.Int32:
				case DbType.Int64:
				case DbType.SByte:
				case DbType.UInt16:
				case DbType.UInt32:
				case DbType.UInt64:
					str = value.ToString();
					break;
				case DbType.Guid:
					str = "'" + value.ToString() + "'";
					break;
				case DbType.Binary:
				case DbType.Object:
					str = null;
					break;
				case DbType.Time:
					str = ((TimeSpan)value).ToString("'HH:mm:ss:fff'");
					break;
				case DbType.Single:
				case DbType.Double:
				case DbType.VarNumeric:
				case DbType.Decimal:
				case DbType.Currency:
					str = ((decimal)value).ToString();
					break;
				case DbType.Date:
					str = ((DateTime)value).ToString("'yyyy-MM-dd'");
					break;
				case DbType.DateTime:
				case DbType.DateTime2:
					str = ((DateTime)value).ToString("'yyyy-MM-dd HH:mm:ss:fff'");
					break;
				case DbType.DateTimeOffset:
					str = ((DateTimeOffset)value).ToString("'dddd, MMM dd yyyy HH:mm:ss:fff zzz'", CultureInfo.InvariantCulture);
					break;
				default:
					throw new InvalidOperationException("Unknown DbType: " + dbType.ToString());
			}
			return dbType;
		}

		/// <summary>
		/// Converts a list of objects to a list of CSV rows.
		/// </summary>
		/// <typeparam name="T">The type of object.</typeparam>
		/// <param name="list">A list of objects to convert to a csv.</param>
		/// <param name="separater">The separater between each value.</param>
		/// <param name="includeColumnNames">Determines if the column names should be printed.</param>
		/// <returns>A list of CSV rows.</returns>
		public static IEnumerable<string> ToCsv<T>(IEnumerable<T> list, string separater = ",", bool printColumnNames = true) where T : class
		{
			SqlTypeInfo typeInfo = ExtraCrud.TypeInfo<T>();
			IReadOnlyList<SqlColumn> columns = typeInfo.AutoKeyColumn == null ? typeInfo.Columns : typeInfo.Columns.Where(c => !c.IsAutoKey).ToArray();
			if (columns.Count != 0) {
				MemberGetter[] getters = new MemberGetter[columns.Count];
				StringBuilder sb = new StringBuilder();
				for (int i = 0; i < columns.Count; i++) {
					SqlColumn column = columns[i];
					getters[i] = columns[i].Getter;
					sb.Append('\"').Append(column.ColumnName.Replace("\"", "\"\"")).Append('\"').Append(separater);
				}
				sb.Remove(sb.Length - separater.Length, separater.Length);
				if (printColumnNames) {
					yield return sb.ToString();
				}
				foreach (T obj in list) {
					sb.Clear();
					foreach (MemberGetter getter in getters) {
						object value = getter(obj);
						if (value == null)
							sb.Append("NULL");
						else
							sb.Append('\"').Append(value.ToString().Replace("\"", "\"\"")).Append('\"');
						sb.Append(separater);
					}
					sb.Remove(sb.Length - separater.Length, separater.Length);
					yield return sb.ToString();
				}
			}
		}
	}
}
