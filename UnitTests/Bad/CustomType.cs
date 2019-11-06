using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace UnitTests
{
	public class CustomType : SqlMapper.TypeHandler<TestDTO>
	{
		public object Value { get; set; }

		public override TestDTO Parse(object value)
		{
			return null;
		}

		public override void SetValue(IDbDataParameter parameter, TestDTO value)
		{			
		}
	}
}
