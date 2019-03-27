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
using Dapper.Extension.Interfaces;

namespace Dapper.Extension
{
	public static class TableData<T, KeyType> where T : class
	{
		public static TableQueries<T, KeyType> Queries { get; internal set; }

		/// <summary>
		/// Creates an object from a single value KeyProperty.
		/// </summary>
		/// <typeparam name="KeyType">The type of the key.</typeparam>
		/// <param name="key">The value of the key.</param>
		/// <returns>A new object with the specified key.</returns>
		public static T CreateObject(KeyType key)
		{
			T objKey = (T) System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(T));
			SetKey(objKey, key);
			return objKey;
		}

		/// <summary>
		/// Copies a single value to the KeyProperty of an object.
		/// </summary>
		public static void SetKey(T obj, KeyType key)
		{
			TableData<T>.Queries.KeyProperties[0].SetValue(obj, key);
		}

		/// <summary>
		/// Creates an ExpandoObject (key) from a single value.
		/// </summary>
		public static IDictionary<string, object> CreateKey(KeyType value)
		{
			IDictionary<string, object> newKey = new ExpandoObject();
			newKey[TableData<T>.Queries.KeyProperties[0].Name] = value;
			return newKey;
		}

		/// <summary>
		/// Gets the value of the first key from an object. This assumes that there is only one KeyAttribute.
		/// </summary>
		/// <typeparam name="KeyType">The type of the key.</typeparam>
		/// <param name="obj">The input object to pull the key from.</param>
		/// <returns>The value of the key.</returns>
		public static KeyType GetKey(T obj)
		{
			return (KeyType) TableData<T>.Queries.KeyProperties[0].GetValue(obj);
		}

		/// <summary>
		/// Gets the value of the first key from an object. This assumes that there is only one KeyAttribute.
		/// </summary>
		/// <typeparam name="KeyType">The type of the key.</typeparam>
		/// <param name="obj">The input object to pull the key from.</param>
		/// <returns>The value of the key.</returns>
		public static KeyType GetKey(object obj)
		{
			dynamic key = new ExpandoObject();
			for (int i = 0; i < TableData<T>.KeyProperties.Length; i++) {
				key[TableData<T>.KeyProperties[i].Name] = TableData<T>.KeyProperties[i].GetValue(obj);
			}
			return key;
		}
	}


	public static class TableData<T> where T : class
	{
		static TableData()
		{
			try {
				TableQueries<T>.Factory factory = new TableQueries<T>.Factory();
				TableName = factory.TableName;
				TableAttribute = factory.TableAttribute;
				TableQueries<T>.Data queries = factory.Create();
				Queries = new TableQueries<T>(queries);
				Columns = Queries.Columns;
				KeyColumns = Queries.KeyColumns;
				Properties = Queries.Properties;
				KeyProperties = Queries.KeyProperties;
				AutoKeyProperties = Queries.AutoKeyProperties;
				EqualityProperties = Queries.EqualityProperties;

				if (factory.KeyProperties.Length == 1) {
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
							if (type.IsArray) {
								Type underlying = type.GetEnumUnderlyingType();
								if (underlying == typeof(byte)) {
									CreateKeyQueries<byte[]>(factory);
								}
							}
							else if (type.IsGenericType && typeof(IEnumerable<>).IsAssignableFrom(type)) {
								if (type.GetGenericArguments()[0] == typeof(byte)) {
									CreateKeyQueries<IEnumerable<Byte>>(factory);
								}
							}
							else if (type == typeof(Guid)) {
								CreateKeyQueries<Guid>(factory);
							}
							else if (type == typeof(DateTimeOffset)) {
								CreateKeyQueries<DateTimeOffset>(factory);
							}
							else if (type == typeof(TimeSpan)) {
								CreateKeyQueries<TimeSpan>(factory);
							}
							break;
					}
				}
			}
			catch { }
		}

		private static void CreateKeyQueries<KeyType>(TableQueries<T>.Factory factory)
		{
			TableData<T, KeyType>.Queries = new TableQueries<T, KeyType>(factory.Create<KeyType>());
		}

		public static TableAttribute TableAttribute { get; private set; }
		public static TableQueries<T> Queries { get; private set; }
		public static string TableName { get; private set; }

		public static PropertyInfo[] Properties { get; private set; }
		public static PropertyInfo[] KeyProperties { get; private set; }
		public static PropertyInfo[] AutoKeyProperties { get; private set; }
		public static PropertyInfo[] EqualityProperties { get; private set; }

		public static string[] Columns { get; private set; }
		public static string[] KeyColumns { get; private set; }


		/// <summary>
		/// Creates an object from a key where the type has identical KeyProperties (e.g. ExpandoObject or typeof(T)).
		/// </summary>
		public static T CreateObject(IDictionary<string, object> key)
		{
			T objKey = (T) System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(T));
			SetKey(objKey, key);
			return objKey;
		}

		/// <summary>
		/// Creates an object from a key where the type has identical KeyProperties (e.g. ExpandoObject or typeof(T)).
		/// </summary>
		public static void SetKey(T obj, IDictionary<string, object> key)
		{
			for (int i = 0; i < KeyProperties.Length; i++) {
				KeyProperties[i].SetValue(obj, key[KeyProperties[i].Name]);
			}
		}

		/// <summary>
		/// Creates an object from a key where the type has identical KeyProperties (e.g. ExpandoObject or typeof(T)).
		/// </summary>
		public static void SetKey(T obj, T key)
		{
			Type type = key.GetType();
			for (int i = 0; i < KeyProperties.Length; i++) {
				KeyProperties[i].SetValue(obj, type.GetProperty(KeyProperties[i].Name, BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance).GetValue(key));
			}
		}


		/// <summary>
		/// Gets an ExpandoObject T with the keys filled in.
		/// </summary>
		/// <param name="obj">The input object to pull keys from.</param>
		/// <returns>An ExpandoObject with keys from the input.</returns>
		public static IDictionary<string, object> GetKey(T obj)
		{
			dynamic key = new ExpandoObject();
			for (int i = 0; i < KeyProperties.Length; i++) {
				key[KeyProperties[i].Name] = KeyProperties[i].GetValue(obj);
			}
			return key;
		}

		/// <summary>
		/// Creates a shallow clone of the object.
		/// </summary>
		public static T Clone(T source)
		{
			T dest = (T) System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(T));
			for (int i = 0; i < Properties.Length; i++) {
				Properties[i].SetValue(dest, Properties[i].GetValue(source));
			}
			return dest;
		}

		/// <summary>
		/// Returns true if the destination was modified, or false if they were identical.
		/// </summary>
		public static bool Copy(T source, T dest)
		{
			for (int i = 0; i < Properties.Length; i++) {
				object sourceValue = Properties[i].GetValue(source);
				object destValue = Properties[i].GetValue(dest);
				if (sourceValue != destValue) {
					for (int j = i; j < Properties.Length; j++) {
						Properties[j].SetValue(dest, Properties[j].GetValue(source));
					}
					return true;
				}
			}
			return false;
		}
	}
}
