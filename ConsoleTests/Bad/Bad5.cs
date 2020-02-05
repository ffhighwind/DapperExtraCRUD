using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTests
{
	public class Bad5_
	{
		public static int ID { private get; set; }
	}

	public class Bad5 : Bad5_
	{
		public new int ID { get; }
	}
}
