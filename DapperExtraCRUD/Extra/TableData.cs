using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Dapper.Extra.Interfaces;
using Dapper.Extra.Internal;

namespace Dapper.Extra
{
	/*
	/// <summary>
	/// Stores metadata and queries for DapperExtraCRUD.
	/// </summary>
	/// <typeparam name="T">The <see cref="Type"/> to store metadata for.</typeparam>
	/// <typeparam name="KeyType">The <see cref="Type"/> of the table's key.</typeparam>
	public static class TableData<T, KeyType>
		where T : class
	{
		static TableData()
		{
			System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(TableData<T>).TypeHandle);
		}

		/// <summary>
		/// The queries used for the DapperExtraCRUD extension methods.
		/// </summary>
		public static SqlQueries<T, KeyType> Queries { get; internal set; }
	}


	/// <summary>
	/// Stores metadata and queries for DapperExtraCRUD.
	/// </summary>
	/// <typeparam name="T">The <see cref="Type"/> to store metadata for.</typeparam>
	public static class TableData<T>
		where T : class
	{
		static TableData()
		{
			try {
				Initialize();
			}
			catch (Exception ex) { // ignore
				Exception = ex;
			}
		}

		private static void Initialize()
		{
			Info = new SqlTypeInfo(typeof(T));
			TableFactory<T> factory = TableFactory.Create<T>(Info.Syntax);
			Queries = factory.Create();
			if (factory.KeyProperties.Count == 1) {
				Type type = factory.KeyProperties[0].PropertyType;
				type = Nullable.GetUnderlyingType(type) ?? type;
				TypeCode typeCode = Type.GetTypeCode(type);
				switch (typeCode) {
					case TypeCode.Int16:
						CreateKeyQueries<short>(factory);
						break;
					case TypeCode.Int32:
						CreateKeyQueries<int>(factory);
						break;
					case TypeCode.Int64:
						CreateKeyQueries<long>(factory);
						break;
					case TypeCode.SByte:
						CreateKeyQueries<sbyte>(factory);
						break;
					case TypeCode.Single:
						CreateKeyQueries<float>(factory);
						break;
					case TypeCode.String:
						CreateKeyQueries<string>(factory);
						break;
					case TypeCode.UInt16:
						CreateKeyQueries<ushort>(factory);
						break;
					case TypeCode.Double:
						CreateKeyQueries<double>(factory);
						break;
					case TypeCode.UInt32:
						CreateKeyQueries<uint>(factory);
						break;
					case TypeCode.UInt64:
						CreateKeyQueries<ulong>(factory);
						break;
					case TypeCode.Byte:
						CreateKeyQueries<byte>(factory);
						break;
					case TypeCode.Char:
						CreateKeyQueries<char>(factory);
						break;
					case TypeCode.DateTime:
						CreateKeyQueries<DateTime>(factory);
						break;
					case TypeCode.Decimal:
						CreateKeyQueries<decimal>(factory);
						break;
					default:
						if (type == typeof(Guid))
							CreateKeyQueries<Guid>(factory);
						else if (type == typeof(DateTimeOffset))
							CreateKeyQueries<DateTimeOffset>(factory);
						else if (type.IsArray) {
							Type underlying = type.GetEnumUnderlyingType();
							if (underlying == typeof(byte)) {
								CreateKeyQueries<byte[]>(factory);
							}
						}
						else if (type.IsGenericType && typeof(IEnumerable<>).IsAssignableFrom(type)) {
							if (type.GetGenericArguments()[0] == typeof(byte)) {
								CreateKeyQueries<IEnumerable<byte>>(factory);
							}
						}
						else if (type == typeof(TimeSpan))
							CreateKeyQueries<TimeSpan>(factory);
						break;
				}
			}
		}

		private static void CreateKeyQueries<KeyType>(TableFactory<T> factory)
		{
			SqlQueries<T, KeyType> queries = factory.Create<KeyType>();
			TableData<T, KeyType>.Queries = queries;
		}

		/// <summary>
		/// The queries used for the DapperExtraCRUD extension methods.
		/// </summary>
		public static SqlQueries<T> Queries { get; private set; }

		/// <summary>
		/// The exception generated when constructing the queries.
		/// </summary>
		public static Exception Exception { get; private set; }

		public static SqlTypeInfo Info { get; private set; }

		/// <summary>
		/// The name of the table.
		/// </summary>
		public static string TableName => Info.TableName;

		/// <summary>
		/// The DBMS syntax used for generating queries.
		/// </summary>
		public static SqlSyntax Syntax => Info.Syntax;

		/// <summary>
		/// Sets the KeyProperties from a dynamic/ExpandoObject.
		/// </summary>
		public static void SetAutoKey(T obj, IDictionary<string, object> key)
		{
			PropertyInfo property = Info.AutoKeyColumn.Property;
			property.SetValue(obj, key[property.Name]);
		}

		/// <summary>
		/// Sets the KeyProperties from a dynamic/ExpandoObject.
		/// </summary>
		public static void SetKey(T obj, IDictionary<string, object> key)
		{
			foreach (var column in Info.KeyColumns) {
				var property = column.Property;
				property.SetValue(obj, key[property.Name]);
			}
		}

		/// <summary>
		/// Sets the AutoKeyProperty of an object using another object.
		/// </summary>
		public static void SetAutoKey(T obj, T key)
		{
			PropertyInfo property = Info.AutoKeyColumn.Property;
			property.SetValue(obj, property.GetValue(key));
		}

		/// <summary>
		/// Sets the KeyProperties of an object using another object.
		/// </summary>
		public static void SetKey(T obj, T key)
		{
			foreach (var column in Info.KeyColumns) {
				var property = column.Property;
				property.SetValue(obj, property.GetValue(key));
			}
		}

		/// <summary>
		/// Creates an ExpandoObject and copies the KeyProperties from an object to it.
		/// </summary>
		/// <param name="obj">The input object to copy the KeyProperties from.</param>
		/// <returns>An ExpandoObject with copied KeyProperties from another object.</returns>
		public static ExpandoObject GetKey(T obj)
		{
			dynamic key = new ExpandoObject();
			foreach (var column in Info.KeyColumns) {
				PropertyInfo property = column.Property;
				key[property.Name] = property.GetValue(obj);
			}
			return key;
		}

		#region KeyType
		/// <summary>
		/// Creates an object from a single value KeyProperty.
		/// </summary>
		/// <typeparam name="KeyType">The type of the key.</typeparam>
		/// <param name="key">The value of the key.</param>
		/// <returns>A new object with the specified key.</returns>
		public static T CreateObject<KeyType>(KeyType key)
		{
			T objKey = (T) System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(T));
			SetKey(objKey, key);
			return objKey;
		}

		/// <summary>
		/// Copies the AutoKeyProperty of an object.
		/// </summary>
		public static void SetAutoKey<KeyType>(T obj, KeyType key)
		{
			Info.AutoKeyColumn.Property.SetValue(obj, key);
		}

		/// <summary>
		/// Copies a single value to the KeyProperty of an object.
		/// </summary>
		public static void SetKey<KeyType>(T obj, KeyType key)
		{
			Info.KeyColumns[0].Property.SetValue(obj, key);
		}

		/// <summary>
		/// Creates an ExpandoObject (key) from a single value.
		/// </summary>
		public static IDictionary<string, object> CreateKey<KeyType>(KeyType value)
		{
			IDictionary<string, object> newKey = new ExpandoObject();
			newKey[Info.KeyColumns[0].Property.Name] = value;
			return newKey;
		}

		/// <summary>
		/// Gets the value of the first key from an object. This assumes that there is only one KeyAttribute.
		/// </summary>
		/// <typeparam name="KeyType">The type of the key.</typeparam>
		/// <param name="obj">The input object to pull the key from.</param>
		/// <returns>The value of the key.</returns>
		public static KeyType GetKey<KeyType>(T obj)
		{
			KeyType key = (KeyType) Info.KeyColumns[0].Property.GetValue(obj);
			return key;
		}

		/// <summary>
		/// Gets the value of the first key from an object. This assumes that there is only one KeyAttribute.
		/// </summary>
		/// <typeparam name="KeyType">The type of the key.</typeparam>
		/// <param name="obj">The input object to pull the key from.</param>
		/// <returns>The value of the key.</returns>
		public static KeyType GetKey<KeyType>(IDictionary<string, object> obj)
		{
			KeyType key = (KeyType) obj[Info.KeyColumns[0].Property.Name];
			return key;
		}
		#endregion KeyType

		/// <summary>
		/// Determines whether the specified object is equal to the current object.
		/// </summary>
		/// <param name="x">The first object to compare.</param>
		/// <param name="y">The object to compare with the current object.</param>
		/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
		public static bool Equals(T x, T y)
		{
			foreach (var column in Info.KeyColumns) {
				var property = column.Property;
				if (!object.Equals(property.GetValue(x), property.GetValue(y)))
					return false;
			}
			return true;
		}

		private static readonly int InitialHash = TableName.GetHashCode();
		/// <summary>
		/// Generates a hash code for the current object.
		/// </summary>
		/// <param name="obj">The <see cref="object"/> for which a hash code is to be returned..</param>
		/// <returns>A hash code for the current object.</returns>
		public static int GetHashCode(T obj)
		{
			int hashCode = InitialHash;
			foreach (var column in Info.KeyColumns) {
				object value = column.Property.GetValue(obj);
				hashCode = hashCode * 397;
				if (value != null) {
					hashCode ^= value.GetHashCode();
				}
			}
			return hashCode;
		}

		/// <summary>
		/// Removes duplicates from a list of objects.
		/// </summary>
		/// <param name="list">The list of objects.</param>
		/// <returns>A new list where duplicates are removed.</returns>
		public static IEnumerable<T> MakeDistinct(IEnumerable<T> list)
		{
			return list.Distinct(EqualityComparer);
		}

		/// <summary>
		/// IEqualityComparer for type T.
		/// </summary>
		/// <typeparam name="T">The <see cref="Type"/> to compare.</typeparam>
		private class TableEqualityComparer : IEqualityComparer<T>
		{
			public bool Equals(T x, T y)
			{
				return TableData<T>.Equals(x, y);
			}

			public int GetHashCode(T obj)
			{
				return TableData<T>.GetHashCode(obj);
			}
		}

		/// <summary>
		/// <see cref="IEqualityComparer{T}"/> for type T.
		/// </summary>
		public static IEqualityComparer<T> EqualityComparer = new TableEqualityComparer();
	}
	*/
}
