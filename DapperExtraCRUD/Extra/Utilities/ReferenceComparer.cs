using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Extra.Utilities
{
	public class ReferenceComparer : IEqualityComparer<object>
	{
		private ReferenceComparer() { }

		public new bool Equals(object x, object y)
		{

			return object.ReferenceEquals(x, y);
		}

		public int GetHashCode(object obj)
		{
			return System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(obj);
		}

		public static readonly ReferenceComparer Default = new ReferenceComparer();
	}
}
