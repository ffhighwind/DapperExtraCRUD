using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper
{
	/// <summary>
	/// Makes inserts to do nothing and changes upserts into updates.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class NoInsertsAttribute : System.Attribute
	{
	}
}
