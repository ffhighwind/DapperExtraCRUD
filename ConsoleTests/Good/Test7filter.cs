using System;

namespace ConsoleTests
{
	public class Test7filter : IFilter<Test7>
	{
		public Test7Type ID { get; set; }

		public bool IsFiltered(Test7 obj)
		{
			return true;
		}
	}
}
