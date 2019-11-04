using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper
{
	/// <summary>
	/// Makes deletes to do nothing.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class NoDeletesAttribute : System.Attribute
	{
	}
}
