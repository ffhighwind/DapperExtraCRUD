using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ConsoleTests
{
	public class Dto15
	{
		[Required]
		public int MyID { get; set; }
		public string Name { get; set; }
	}
}
