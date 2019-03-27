using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper
{
	[AttributeUsage(AttributeTargets.Property)]
	public class KeyAttribute : Attribute
	{
		/// <param name="required">Determines if the key is auto-generated.
		/// False is equivilent to [Key][IgnoreInsert][IgnoreUpdate] while true is just [Key].</param>
		public KeyAttribute(bool required = false)
		{
			Required = required;
		}

		/// <summary>
		/// Determines if the key is auto-generated.
		/// False is equivilent to [Key][IgnoreInsert][IgnoreUpdate] while true is just [Key].</param>
		/// </summary>
		public bool Required { get; private set; }
	}
}
