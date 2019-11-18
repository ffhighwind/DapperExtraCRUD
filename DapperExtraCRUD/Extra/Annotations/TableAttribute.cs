// Released under MIT License 
// Copyright(c) 2018 Wesley Hamilton
// License: https://www.mit.edu/~amini/LICENSE.md
// Home page: https://github.com/ffhighwind/DapperExtraCRUD

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dapper.Extra;

namespace Dapper.Extra.Annotations
{
	/// <summary>
	/// The name of the database table. If this <see cref="Attribute"/> is not supplied then the table name is assumed to be the same as the <see langword="class"/> name.
	/// </summary>
	/// <see cref="System.ComponentModel.DataAnnotations.Schema.TableAttribute"/>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class TableAttribute : Attribute
	{
		/// <summary>
		/// The table name.
		/// </summary>
		/// <param name="name">The name of the table.</param>
		/// <param name="schema">The schema of the table. This is only for user reference and is completely ignored.</param>
		/// <param name="declaredOnly">Determines if only top-level properties are used. Subclass properties are ignored if this is true.</param>
		/// <param name="inheritAttrs">Determines if attributes are inherited.</param>
		/// <param name="syntax">The syntax used to generate SQL commands.</param>
		public TableAttribute(string name = null, string schema = null, bool declaredOnly = false, bool inheritAttrs = true, SqlSyntax syntax = SqlSyntax.SQLServer)
		{
			Name = name?.Trim();
			Schema = string.IsNullOrWhiteSpace(schema) ? "" : schema.Trim();
			Syntax = syntax;
			InheritAttributes = inheritAttrs;
			DeclaredOnly = declaredOnly;
		}

		/// <summary>
		/// The name of the table.
		/// </summary>
		public string Name { get; }
		/// <summary>
		/// The schema of the table. This is
		/// </summary>
		public string Schema { get; }
		/// <summary>
		/// The syntax used to generate SQL commands.
		/// </summary>
		public SqlSyntax Syntax { get; }
		/// <summary>
		/// Determines if property attributes are inherited.
		/// </summary>
		public bool InheritAttributes { get; }
		/// <summary>
		/// Determines if only top-level properties are used. Subclass properties are ignored if this is true.
		/// </summary>
		public bool DeclaredOnly { get; }
	}
}
