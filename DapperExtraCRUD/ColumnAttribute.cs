using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper
{
	/// <summary>
	/// The name of the column.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
	public class ColumnAttribute : Attribute
	{
		/// <summary>
		/// The name of the column.
		/// </summary>
		/// <param name="name">The name of the column.</param>
		public ColumnAttribute(string name)
		{
			Name = name;
		}

		/// <summary>
		/// The name of the column.
		/// </summary>
		public string Name { get; private set; }
	}
}
