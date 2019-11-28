using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using Fasterflect;

namespace Dapper.Extra.Utilities
{
	/// <summary>
	/// Used with <see cref="SqlBulkCopy.WriteToServer"/> for bulk operations.
	/// </summary>
	public class DataReaderFactory
	{
		public DataReaderFactory(Type type, BindingFlags flags = BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public) 
			: this(type, type.GetProperties(flags))
		{
		}

		public DataReaderFactory(Type type, IEnumerable<PropertyInfo> properties)
		{
			Type = type;
			Properties = properties.ToArray();
			PropertyNames = new string[Properties.Length];
			PropertyTypes = new Type[Properties.Length];
			Getters = new MemberGetter[Properties.Length];
			AllowNull = new BitArray(Properties.Length);
			for (int i = 0; i < Properties.Length; i++) {
				PropertyNames[i] = Properties[i].Name;
				Getters[i] = Fasterflect.PropertyExtensions.DelegateForGetPropertyValue(Type, Properties[i].Name);
				Type propertyType = Properties[i].PropertyType;
				Type underlying = Nullable.GetUnderlyingType(propertyType);
				AllowNull[i] = !propertyType.IsValueType || underlying != null;
				PropertyTypes[i] = underlying ?? propertyType;
				IndexMap.Add(Properties[i].Name, i);
			}
		}

		public DbDataReader Create(IEnumerable<object> list)
		{
			return new DataReader(list.GetEnumerator(), this);
		}

		public Type Type { get; private set; }
		public PropertyInfo[] Properties { get; private set; }
		public MemberGetter[] Getters { get; private set; }
		public BitArray AllowNull { get; private set; }
		public string[] PropertyNames { get; private set; }
		public Type[] PropertyTypes { get; private set; }
		internal Dictionary<string, int> IndexMap = new Dictionary<string, int>();

		/// <summary>
		/// Used with <see cref="SqlBulkCopy.WriteToServer"/> for bulk operations.
		/// </summary>
		/// <remarks>https://github.com/mgravell/fast-member/blob/master/FastMember/ObjectReader.cs</remarks>
		internal class DataReader : DbDataReader
		{
			internal DataReader(IEnumerator enumerator, DataReaderFactory factory)
			{
				Enumerator = enumerator;
				Getters = factory.Getters;
				PropertyNames = factory.PropertyNames;
				PropertyTypes = factory.PropertyTypes;
				AllowNull = factory.AllowNull;
				IndexMap = factory.IndexMap;
			}

			private IEnumerator Enumerator;
			private readonly string[] PropertyNames;
			private readonly Type[] PropertyTypes;
			private readonly BitArray AllowNull;
			private readonly MemberGetter[] Getters;
			private object Current = null;
			private readonly Dictionary<string, int> IndexMap;

			public override object this[int ordinal] => Getters[ordinal](Current) ?? DBNull.Value;

			public override object this[string name] => Getters[IndexMap[name]](Current) ?? DBNull.Value;

			public override int Depth => 0;

			public override int FieldCount => PropertyNames.Length;

			public override bool HasRows => Enumerator != null;

			public override bool IsClosed => Enumerator == null;

			public override int RecordsAffected => 0;

			public override bool GetBoolean(int ordinal)
			{
				return (bool) this[ordinal];
			}

			public override byte GetByte(int ordinal)
			{
				return (byte) this[ordinal];
			}

			public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
			{
				byte[] s = (byte[]) this[ordinal];
				int available = s.Length - (int) dataOffset;
				if (available <= 0)
					return 0;

				int count = Math.Min(length, available);
				Buffer.BlockCopy(s, (int) dataOffset, buffer, bufferOffset, count);
				return count;
			}

			public override char GetChar(int ordinal)
			{
				return (char) this[ordinal];
			}

			public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
			{
				string s = GetString(ordinal);
				int available = s.Length - (int) dataOffset;
				if (available <= 0)
					return 0;

				int count = Math.Min(length, available);
				s.CopyTo((int) dataOffset, buffer, bufferOffset, count);
				return count;
			}

			public override string GetDataTypeName(int ordinal)
			{
				return PropertyTypes[ordinal].Name;
			}

			public override DateTime GetDateTime(int ordinal)
			{
				return (DateTime) this[ordinal];
			}

			public override decimal GetDecimal(int ordinal)
			{
				return (decimal) this[ordinal];
			}

			public override double GetDouble(int ordinal)
			{
				return (double) this[ordinal];
			}

			public override IEnumerator GetEnumerator()
			{
				return new DbEnumerator(this);
			}

			public override Type GetFieldType(int ordinal)
			{
				return PropertyTypes[ordinal];
			}

			public override float GetFloat(int ordinal)
			{
				return (float) this[ordinal];
			}

			public override Guid GetGuid(int ordinal)
			{
				return (Guid) this[ordinal];
			}

			public override short GetInt16(int ordinal)
			{
				return (short) this[ordinal];
			}

			public override int GetInt32(int ordinal)
			{
				return (int) this[ordinal];
			}

			public override long GetInt64(int ordinal)
			{
				return (long) this[ordinal];
			}

			public override string GetName(int ordinal)
			{
				return PropertyNames[ordinal];
			}

			public override int GetOrdinal(string name)
			{
				return IndexMap[name];
			}

			public override string GetString(int ordinal)
			{
				return (string) this[ordinal];
			}

			public override object GetValue(int ordinal)
			{
				return this[ordinal];
			}

			public override int GetValues(object[] values)
			{
				for (int i = 0, count = PropertyNames.Length; i < count; i++) {
					values[i] = Getters[i](Current) ?? DBNull.Value;
				}
				return PropertyNames.Length;
			}

			public override bool IsDBNull(int ordinal)
			{
				return this[ordinal] is DBNull;
			}

			public override bool NextResult()
			{
				return Read();
			}

			public override bool Read()
			{
				if (Enumerator != null) {
					if (Enumerator.MoveNext()) {
						Current = Enumerator.Current;
						return true;
					}
					Enumerator = null;
				}
				return false;
			}

			public override DataTable GetSchemaTable()
			{
				// these are the columns used by DataTable load
				DataTable table = new DataTable
				{
					Columns =
				{
					{ "ColumnOrdinal", typeof(int) },
					{ "ColumnName", typeof(string) },
					{ "DataType", typeof(Type) },
					{ "ColumnSize", typeof(int) },
					{ "AllowDBNull", typeof(bool) }
				}
				};
				object[] rowData = new object[5];
				for (int i = 0; i < PropertyNames.Length; i++) {
					rowData[0] = i;
					rowData[1] = PropertyNames[i];
					rowData[2] = PropertyTypes[i];
					rowData[3] = -1;
					rowData[4] = AllowNull[i];
					table.Rows.Add(rowData);
				}
				return table;
			}

			public override void Close()
			{
				Current = null;
				if (Enumerator is IDisposable d) {
					d.Dispose();
				}
				Enumerator = null;
			}

			protected override void Dispose(bool disposing)
			{
				base.Dispose(disposing);
				if (disposing)
					Close();
			}
		}
	}
}
