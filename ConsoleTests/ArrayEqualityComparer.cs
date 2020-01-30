using System;
using System.Collections.Generic;

namespace Dapper.Extra.Utilities
{
	public sealed class ArrayEqualityComparer<T> : IEqualityComparer<T[]> where T : struct
	{
		public static readonly IEqualityComparer<T[]> Default = new ArrayEqualityComparer<T>();

		private ArrayEqualityComparer()
		{
			InitialHashCode = typeof(T).FullName.GetHashCode();
		}

		private int InitialHashCode { get; }

		public bool Equals(T[] x, T[] y)
		{
			if (x != y) {
				if (x == null || y == null || x.Length != y.Length)
					return false;
				for (int i = 0; i < x.Length; i++) {
					if (!x[i].Equals(y[i]))
						return false;
				}
			}
			return true;
		}

		public int GetHashCode(T[] obj)
		{
			int hashCode = InitialHashCode;
			for (int i = 0; i < obj.Length; i++) {
				hashCode = hashCode * 67236819 + obj[i].GetHashCode();
			}
			return hashCode;

		}
	}
}
