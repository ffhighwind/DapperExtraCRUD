using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.Extension.Interfaces;

namespace Dapper
{
	[AttributeUsage(AttributeTargets.Property)]
	public class MatchUpdateAttribute : IDefaultAttribute
	{
		public MatchUpdateAttribute(Func<string> function)
			: base(function) { }

		public MatchUpdateAttribute(string value = null)
			: base(value) { }
	}
}
