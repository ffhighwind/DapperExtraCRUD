using System;
using Dapper.Extra.Annotations;

namespace ConsoleTests
{
	public class Dto13
	{
		private int id;
		private int dto13ID { get; set; }
		[Column("Dto13ID")]
		public int MyId { get; set; }
		private int Default { get; set; }
	}
}
