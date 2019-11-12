using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper
{
	/// <summary>
	/// A primary key column.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
	public class KeyAttribute : Attribute
	{
		/// <summary>
		/// A primary key column.
		/// </summary>
		/// <param name="autoIncrement">Determines if the key is auto-incrementing. 
		/// This is only allowed for integer based keys (int, long, short, etc).</param>
		public KeyAttribute(bool autoIncrement = true)
		{
			AutoIncrement = autoIncrement;
		}

		/// <summary>
		/// Determines if the primary key is auto-incrementing.
		/// </summary>
		public bool AutoIncrement { get; private set; }
	}
}
