using System;

namespace ConsoleTests
{
	public class TestDTO5filter : IFilter<TestDTO5>
	{
		public string Name { get; set; }

		public bool IsFiltered(TestDTO5 obj)
		{
			return obj.ID == default(int)
				&& obj.Modified == default(DateTime)
				&& obj.Modified2 == default(DateTime)
				&& obj.Created == default(DateTime)
				&& obj.Name != default(string);
		}
	}
}
