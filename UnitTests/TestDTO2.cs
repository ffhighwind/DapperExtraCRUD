using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Utilities.UnitTests
{
	[Table("Test2")]
	public class TestDTO2 : IEquatable<TestDTO2>
	{
		public TestDTO2() { }
		public TestDTO2(string str, int i)
		{
			Col1 = i;
			Col2 = str;
		}

		public int Col1 { get; set; }
		public string Col2 { get; set; }
		public float Col3;
		public string Col4;

		public override bool Equals(object obj)
		{
			return Equals(obj as TestDTO2);
		}

		public bool Equals(TestDTO2 other)
		{
			return other != null &&
				   Col1 == other.Col1 &&
				   Col2 == other.Col2 &&
				   Col3 == other.Col3 &&
				   Col4 == other.Col4;
		}

		public override int GetHashCode()
		{
			int hashCode = -1473066521;
			hashCode = hashCode * -1521134295 + Col1.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Col2);
			hashCode = hashCode * -1521134295 + Col3.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Col4);
			return hashCode;
		}
	}
}
