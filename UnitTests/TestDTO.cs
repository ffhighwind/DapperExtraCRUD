using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Extra.UnitTests
{
	[Table("Test")]
	public class TestDTO
	{
		[Key]
		public int ID { get; set; }

		[Column("FirstName")]
		public string Name { get; set; }
		[IgnoreInsert("getdate()")]
		[IgnoreUpdate("getdate()")]
		public DateTime? CreatedDt { get; set; }

		public TestDTO Test { get; set; }
	}
}
