using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace UnitTests
{
	public class Bad2
	{
		[Key]
		public int AutoId { get; set; }
		[Key(false)]
		public string CompositeKey { get; set; }
	}
}
