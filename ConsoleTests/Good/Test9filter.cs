using System;

namespace ConsoleTests
{
	public class Test9filter : IFilter<Test9>
	{
		public string Name { get; set; }

		public bool IsFiltered(Test9 obj)
		{
			return obj.ID == null && obj.Name != null;
		}
	}
}
