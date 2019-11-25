using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
	public class Test3filter : IFilter<Test3>
	{
		public int Col1 { get; set; }
		//public string Col2 { get; set; }
		//public float Col3 { get; set; }
		public int? Col4 { get; set; }

		public bool IsFiltered(Test3 obj)
		{
			return obj.Col1 != default(int)
				&& obj.Col2 == default(string)
				&& obj.Col3 == default(float)
				&& obj.Col4 != default(int?);
		}
	}
}
