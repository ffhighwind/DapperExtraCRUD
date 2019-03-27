using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.Extension;
using Dapper.Extension.Interfaces;

namespace Dapper
{
	[AttributeUsage(AttributeTargets.Property)]
	public class IgnoreUpdateAttribute : IDefaultAttribute
	{
		public IgnoreUpdateAttribute(Func<string> function)
			: base(function) { }

		public IgnoreUpdateAttribute(string value = null)
			: base(value) { }
	}
}
