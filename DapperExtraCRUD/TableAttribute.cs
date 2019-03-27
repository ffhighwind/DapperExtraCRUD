using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dapper
{
	/// <summary>
	/// The name of the database table. If this <see cref="Attribute"/> is not supplied then the table name is assumed to be the same as the <see langword="class"/>.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class TableAttribute : Attribute
	{
		/// <summary>
		/// Stores the table name and the type of <see cref="BindingFlags"/> to search for.
		/// </summary>
		/// <param name="name">The name of the database table.</param>
		/// <param name="flags">The binding flags for properties.</param>
		public TableAttribute(string name, BindingFlags flags = BindingFlags.Instance | BindingFlags.Public)
		{
			BindingFlags = flags;
			Name = name;
		}

		/// <summary>
		/// Stores the table name and the type of <see cref="BindingFlags"/> to search for.
		/// </summary>
		/// <param name="name">The name of the database table.</param>
		/// <param name="flags">The binding flags for properties.</param>
		public TableAttribute(BindingFlags flags = BindingFlags.Instance | BindingFlags.Public)
			: this(null, flags) { }

		/// <summary>
		/// The name of the database table.
		/// </summary>
		public string Name { get; private set; }

		public BindingFlags BindingFlags { get; private set; }
	}
}
