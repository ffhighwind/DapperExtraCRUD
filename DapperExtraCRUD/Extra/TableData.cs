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

namespace Dapper.Extra
{
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
			Type type = typeof(TableData<T>);
			System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(type.TypeHandle);
		}

		/// <summary>
		/// The queries used for the DapperExtraCRUD extension methods.
		/// </summary>
		public static TableQueries<T, KeyType> Queries { get; internal set; }
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
				TableAttribute attr = typeof(T).GetCustomAttribute<TableAttribute>(false);
				SqlSyntax syntax = attr == null ? TableFactory.DefaultSyntax : attr.Syntax;
				TableFactory<T> factory = TableFactory.Create<T>(Syntax);
				TableName = factory.TableName;
				Columns = factory.Columns;
				Properties = factory.Properties;
				KeyProperties = factory.KeyProperties;
				AutoKeyProperty = factory.AutoKeyProperty;
				InsertKeyProperties = factory.InsertKeyProperties;
				UpdateKeyProperties = factory.UpdateKeyProperties;
				AutoKeyColumn = factory.AutoKeyColumn;
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
			catch (Exception ex) { // ignore
				Exception = ex;
			}
		}

		private static void CreateKeyQueries<KeyType>(TableFactory<T> factory)
		{
			try {
				TableQueries<T, KeyType> queries = factory.Create<KeyType>();
				TableData<T, KeyType>.Queries = queries;
			}
			catch (Exception ex) {
				Exception = ex;
			}
		}

		/// <summary>
		/// The queries used for the DapperExtraCRUD extension methods.
		/// </summary>
		public static TableQueries<T> Queries { get; private set; }

		/// <summary>
		/// The exception generated when constructing the queries.
		/// </summary>
		public static Exception Exception { get; private set; }

		/// <summary>
		/// The name of the table.
		/// </summary>
		public static string TableName { get; private set; }

		/// <summary>
		/// The DBMS syntax used for generating queries.
		/// </summary>
		public static SqlSyntax Syntax { get; private set; }

		/// <summary>
		/// The properties of the table.
		/// </summary>
		public static IReadOnlyList<PropertyInfo> Properties { get; private set; }

		/// <summary>
		/// The key properties that determine if two objects are equal.
		/// </summary>
		public static IReadOnlyList<PropertyInfo> KeyProperties { get; private set; }

		/// <summary>
		/// The property that is auto-incremented on insert.
		/// </summary>
		public static PropertyInfo AutoKeyProperty { get; private set; }

		/// <summary>
		/// The name of the the table's column that maps to the AutoKeyProperty.
		/// </summary>
		public static string AutoKeyColumn { get; private set; }

		/// <summary>
		/// The name of the columns based on the ColumnAttributes. These index is the same as Properties.
		/// </summary>
		public static IReadOnlyList<string> Columns { get; private set; }

		public static IReadOnlyList<PropertyInfo> InsertKeyProperties { get; private set; }
		public static IReadOnlyList<PropertyInfo> UpdateKeyProperties { get; private set; }

		/// <summary>
		/// Creates an object and then sets the KeyProperties using an ExpandoObject.
		/// </summary>
		//public static T CreateObject(IDictionary<string, object> key)
		//{
		//	T objKey = (T) System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(T));
		//	SetKey(objKey, key);
		//	return objKey;
		//}

		/// <summary>
		/// Sets the KeyProperties from a dynamic/ExpandoObject.
		/// </summary>
		public static void SetAutoKey(T obj, IDictionary<string, object> key)
		{
			AutoKeyProperty.SetValue(obj, key[AutoKeyProperty.Name]);
		}

		/// <summary>
		/// Sets the KeyProperties from a dynamic/ExpandoObject.
		/// </summary>
		public static void SetKey(T obj, IDictionary<string, object> key)
		{
			for (int i = 0; i < KeyProperties.Count; i++) {
				KeyProperties[i].SetValue(obj, key[KeyProperties[i].Name]);
			}
		}

		/// <summary>
		/// Sets the AutoKeyProperty of an object using another object.
		/// </summary>
		public static void SetAutoKey(T obj, T key)
		{
			AutoKeyProperty.SetValue(obj, AutoKeyProperty.GetValue(key));
		}

		/// <summary>
		/// Sets the KeyProperties of an object using another object.
		/// </summary>
		public static void SetKey(T obj, T key)
		{
			for (int i = 0; i < KeyProperties.Count; i++) {
				KeyProperties[i].SetValue(obj, KeyProperties[i].GetValue(key));
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
			for (int i = 0; i < KeyProperties.Count; i++) {
				key[KeyProperties[i].Name] = KeyProperties[i].GetValue(obj);
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
			TableData<T>.AutoKeyProperty.SetValue(obj, key);
		}

		/// <summary>
		/// Copies a single value to the KeyProperty of an object.
		/// </summary>
		public static void SetKey<KeyType>(T obj, KeyType key)
		{
			TableData<T>.KeyProperties[0].SetValue(obj, key);
		}

		/// <summary>
		/// Creates an ExpandoObject (key) from a single value.
		/// </summary>
		public static IDictionary<string, object> CreateKey<KeyType>(KeyType value)
		{
			IDictionary<string, object> newKey = new ExpandoObject();
			newKey[TableData<T>.KeyProperties[0].Name] = value;
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
			KeyType key = (KeyType) TableData<T>.KeyProperties[0].GetValue(obj);
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
			KeyType key = (KeyType) obj[TableData<T>.KeyProperties[0].Name];
			return key;
		}
		#endregion KeyType

		/// <summary>
		/// Creates a shallow clone of an object.
		/// </summary>
		public static T Clone(T source)
		{
			T dest = (T) System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(T));
			for (int i = 0; i < Properties.Count; i++) {
				Properties[i].SetValue(dest, Properties[i].GetValue(source));
			}
			return dest;
		}

		/// <summary>
		/// Copies the Properties from the source to the destination object and determines any changes were made.
		/// </summary>
		/// <param name="source">The object to copy from.</param>
		/// <param name="dest">The object to set.</param>
		/// <returns>True if the destination was modified, or false if they were identical.</returns>
		public static bool CheckCopy(T source, T dest)
		{
			for (int i = 0; i < Properties.Count; i++) {
				object sourceValue = Properties[i].GetValue(source);
				object destValue = Properties[i].GetValue(dest);
				if (!sourceValue.Equals(destValue)) {
					for (int j = i; j < Properties.Count; j++) {
						Properties[j].SetValue(dest, Properties[j].GetValue(source));
					}
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Copies the Properties from the source object to the destination object.
		/// </summary>
		/// <param name="source">The object to copy from.</param>
		/// <param name="dest">The object to set.</param>
		public static void Copy(T source, T dest)
		{
			for (int i = 0; i < Properties.Count; i++) {
				Properties[i].SetValue(dest, Properties[i].GetValue(source));
			}
		}

		/// <summary>
		/// Determines whether the specified object is equal to the current object.
		/// </summary>
		/// <param name="x">The first object to compare.</param>
		/// <param name="y">The object to compare with the current object.</param>
		/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
		public static bool Equals(T x, T y)
		{
			for (int i = 0; i < KeyProperties.Count; i++) {
				PropertyInfo prop = KeyProperties[i];
				if (!object.Equals(prop.GetValue(x), prop.GetValue(y)))
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
			for (int i = 0; i < KeyProperties.Count; i++) {
				object value = KeyProperties[i].GetValue(obj);
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
}
