using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dapper.Extra;

namespace Dapper
{
	/// <summary>
	/// The name of the database table. If this <see cref="Attribute"/> is not supplied then the table name is assumed to be the same as the <see langword="class"/> name.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class TableAttribute : Attribute
	{
		/// <summary>
		/// The database table name.
		/// </summary>
		/// <param name="name">The name of the database table.</param>
		public TableAttribute(string name = null, bool onlyDeclaredProperties = true, SqlSyntax syntax = SqlSyntax.SQLServer)
		{
			Name = name;
			Syntax = syntax;
			OnlyDeclaredProperties = onlyDeclaredProperties;
		}

		/// <summary>
		/// The name of the database table.
		/// </summary>
		public string Name { get; private set; }
		public SqlSyntax Syntax { get; private set; }
		public bool OnlyDeclaredProperties { get; private set;}
	}
}
