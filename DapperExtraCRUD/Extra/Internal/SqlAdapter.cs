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
using Dapper.Extra.Internal.Adapters;

namespace Dapper.Extra.Internal
{
	/// <summary>
	/// Generates specialized commands using a given syntax.
	/// </summary>
	public abstract class SqlAdapter
	{
		/// <summary>
		/// An <see cref="SqlAdapter"/> that generates SQL commands for Microsoft SQL Server.
		/// </summary>
		public static readonly ISqlAdapter SQLServer = new SqlServerAdapter();
		/// <summary>
		/// An <see cref="SqlAdapter"/> that generates SQL commands for PostgreSQL.
		/// </summary>
		public static readonly ISqlAdapter PostgreSQL = new PostgreSqlAdapter();
		/// <summary>
		/// An <see cref="SqlAdapter"/> that generates SQL commands for SQLite.
		/// </summary>
		public static readonly ISqlAdapter SQLite = new SqlLiteAdapter();
		/// <summary>
		/// An <see cref="SqlAdapter"/> that generates SQL commands for MySQL.
		/// </summary>
		public static readonly ISqlAdapter MySQL = new MySqlAdapter();

		/// <summary>
		/// Gets the <see cref="SqlAdapter"/> that matches a given <see cref="SqlSyntax"/>.
		/// </summary>
		/// <param name="syntax">The syntax of the <see cref="SqlAdapter"/>.</param>
		/// <returns>The <see cref="SqlAdapter"/> that matches a given <see cref="SqlSyntax"/>.</returns>
		public static ISqlAdapter GetAdapter(SqlSyntax syntax)
		{
			switch (syntax) {
				case SqlSyntax.MySQL:
					return MySQL;
				case SqlSyntax.PostgreSQL:
					return PostgreSQL;
				case SqlSyntax.SQLite:
					return SQLite;
				case SqlSyntax.SQLServer:
				default:
					return SQLServer;
			}
		}
	}
}
