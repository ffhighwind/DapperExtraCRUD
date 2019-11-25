#region License
// Released under MIT License 
// License: https://www.mit.edu/~amini/LICENSE.md
// Home page: https://github.com/ffhighwind/DapperExtraCRUD

// Copyright(c) 2018 Wesley Hamilton

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
#endregion

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
