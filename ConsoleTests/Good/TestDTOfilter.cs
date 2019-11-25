using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
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
				&& obj.Name == default(string);
		}
	}
}
