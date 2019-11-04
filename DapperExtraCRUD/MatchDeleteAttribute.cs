using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper
{
	/// <summary>
	/// Turns a <see cref="PropertyInfo"/> into a pseudo key for deletions.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class MatchDeleteAttribute : Attribute
	{
	}
}
