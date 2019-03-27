using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Extension
{
	public class TableEqualityComparer<T> : IEqualityComparer<T> where T : class
	{
		private PropertyInfo[] Properties = TableData<T>.EqualityProperties;
		private readonly int InitialHash = TableData<T>.TableName.GetHashCode();

		bool IEqualityComparer<T>.Equals(T x, T y)
		{
			for (int i = 0; i < Properties.Length; i++) {
				PropertyInfo prop = Properties[i];
				if (!prop.GetValue(x).Equals(prop.GetValue(y)))
					return false;
			}
			return true;
		}

		int IEqualityComparer<T>.GetHashCode(T obj)
		{
			int hashCode = InitialHash;
			for (int i = 0; i < Properties.Length; i++) {
				object value = Properties[i].GetValue(obj);
				hashCode = hashCode * 397;
				if (value != null) {
					hashCode ^= value.GetHashCode();
				}
			}
			return hashCode;
		}

		public static TableEqualityComparer<T> Default = new TableEqualityComparer<T>();
	}
}
