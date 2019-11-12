using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Dapper
{
	/// <summary>
	/// Ignores the <see cref="PropertyInfo"/> for selects.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
	public class IgnoreSelectAttribute : Attribute
	{
	}
}
