using System;
using Dapper.Extra.Annotations;

namespace ConsoleTests
{
	public class TestDTO6filter : IFilter<TestDTO6>
	{
		public int Value { get; set; }

		public bool IsFiltered(TestDTO6 obj)
		{
			return obj.ID == null && obj.Value != 0;
		}
	}
}
