using System;

namespace Dapper.Extra.Annotations
{
	/// <summary>
	/// Ignores the property for selects.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class IgnoreSelectAttribute : Attribute
	{
	}
}
