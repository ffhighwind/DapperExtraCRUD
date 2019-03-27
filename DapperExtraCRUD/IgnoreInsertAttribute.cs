using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.Extension.Interfaces;

namespace Dapper
{
	[AttributeUsage(AttributeTargets.Property)]
	public class IgnoreInsertAttribute : IDefaultAttribute
	{
		public IgnoreInsertAttribute(Func<string> function) 
			: base(function) { }

		public IgnoreInsertAttribute(string value = null) 
			: base(value) { }
	}
}
