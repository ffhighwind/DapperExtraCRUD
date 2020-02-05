using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.Extra.Annotations;

namespace ConsoleTests
{
	public class Bad3
	{
		public int ID;

		protected int Num;
		[Key]
		private bool Bool { get; }
	}
}
