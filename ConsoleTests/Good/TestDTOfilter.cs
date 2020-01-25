using System;

namespace ConsoleTests
{
	public class TestDTOfilter : IFilter<TestDTO>
	{
		//public int ID { get; set; }

		//public string Name { get; set; }
		public DateTime? CreatedDt { get; set; }

		public bool IsFiltered(TestDTO obj)
		{
			return obj.CreatedDt != null
				&& obj.CreatedDt != default(DateTime)
				&& obj.ID == default(int)
				&& obj.Name == default(string)
				&& obj.IsActive == default(bool);
		}
	}
}
