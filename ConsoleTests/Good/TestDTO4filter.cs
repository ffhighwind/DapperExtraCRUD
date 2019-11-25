using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
	public class TestDTO4filter : IFilter<TestDTO4>
	{
		//public int ID { get; set; }

		//public string FirstName { get; set; }
		public string LastName { get; set; }

		public bool IsFiltered(TestDTO4 obj)
		{
			return obj.ID == default(int)
				&& obj.LastName != default(string)
				&& obj.FirstName == default(string);
		}
	}
}
