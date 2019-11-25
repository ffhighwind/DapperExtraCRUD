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
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dapper.Extra.Annotations;

namespace Dapper.Extra.Internal
{
	/// <summary>
	/// Stores metadata for for the given table type.
	/// </summary>
	public sealed class SqlTypeInfo
	{
		/// <summary>
		/// Constructs a <see cref="SqlTypeInfo"/> for the given table type.
		/// </summary>
		/// <param name="type">The table type.</param>
		public SqlTypeInfo(Type type)
		{
			Type = type;
			TableAttribute tableAttr = type.GetCustomAttribute<TableAttribute>(false);
			BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty;
			bool inherit = false;
			if (tableAttr == null) {
				var tableAttr2 = type.GetCustomAttribute<System.ComponentModel.DataAnnotations.Schema.TableAttribute>(false);
				Adapter = SqlAdapter.GetAdapter(SqlSyntax.SQLServer);
				TableName = tableAttr2 != null ? tableAttr2.Name : type.Name;
			}
			else {
				Adapter = SqlAdapter.GetAdapter(tableAttr.Syntax);
				TableName = string.IsNullOrWhiteSpace(tableAttr.Name) ? type.Name : tableAttr.Name;
				if (tableAttr.DeclaredOnly) {
					flags |= BindingFlags.DeclaredOnly;
					Attributes |= SqlTableAttributes.DeclaredOnly;
				}
				if (tableAttr.InheritAttributes) {
					inherit = true;
					Attributes |= SqlTableAttributes.InheritAttributes;
				}
			}
			PropertyInfo[] properties = type.GetProperties(flags);
			PropertyInfo[] validProperties = properties.Where(SqlInternal.IsValidProperty).ToArray();
			if (!validProperties.Any())
				throw new InvalidOperationException(type.FullName + " does not have any valid properties.");
			ReadOnlyAttribute readOnlyAttr = type.GetCustomAttribute<ReadOnlyAttribute>(false);
			if (readOnlyAttr != null)
				Attributes |= SqlTableAttributes.IgnoreInsert | SqlTableAttributes.IgnoreUpdate | SqlTableAttributes.IgnoreDelete;
			else {
				var editableAttr = type.GetCustomAttribute<System.ComponentModel.DataAnnotations.EditableAttribute>(false);
				if (editableAttr != null) {
					if (!editableAttr.AllowEdit)
						Attributes |= SqlTableAttributes.IgnoreUpdate;
					if (editableAttr.AllowInitialValue)
						Attributes |= SqlTableAttributes.IgnoreInsert;
				}
				else {
					IgnoreInsertAttribute noInserts = type.GetCustomAttribute<IgnoreInsertAttribute>(false);
					if (noInserts != null)
						Attributes |= SqlTableAttributes.IgnoreInsert;
					IgnoreUpdateAttribute noUpdates = type.GetCustomAttribute<IgnoreUpdateAttribute>(false);
					if (noUpdates != null)
						Attributes |= SqlTableAttributes.IgnoreUpdate;
				}
				IgnoreDeleteAttribute noDeletes = type.GetCustomAttribute<IgnoreDeleteAttribute>(false);
				if (noDeletes != null)
					Attributes |= SqlTableAttributes.IgnoreDelete;
			}

			int ordinal = 0;
			List<SqlColumn> keys = new List<SqlColumn>();
			List<SqlColumn> columns = new List<SqlColumn>();
			int autoKeyCount = 0;
			for (int i = 0; i < validProperties.Length; i++) {
				// Check for unreadable keys before assigning columns
				PropertyInfo prop = validProperties[i];
				if (prop.GetCustomAttribute<NotMappedAttribute>(inherit) != null || prop.GetCustomAttribute<System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute>(inherit) != null)
					continue;
				string columnName = null;
				ColumnAttribute columnAttr = prop.GetCustomAttribute<ColumnAttribute>(inherit);
				if (columnAttr != null) {
					columnName = columnAttr.Name;
					ordinal = columnAttr.Ordinal;
				}
				else {
					var columnAttr2 = prop.GetCustomAttribute<System.ComponentModel.DataAnnotations.Schema.ColumnAttribute>(inherit);
					if (columnAttr2 != null) {
						columnName = columnAttr2.Name;
						ordinal = columnAttr2.Order;
					}
				}
				SqlColumn column = new SqlColumn(prop, string.IsNullOrWhiteSpace(columnName) ? prop.Name : Adapter.QuoteIdentifier(columnName), ordinal);
				columns.Add(column);
				KeyAttribute keyAttr = prop.GetCustomAttribute<KeyAttribute>(inherit);
				if (keyAttr != null) {
					if (keyAttr.AutoIncrement)
						autoKeyCount++;
					column.Attributes = keyAttr.AutoIncrement ? SqlColumnAttributes.AutoKey : SqlColumnAttributes.Key;
				}
				else {
					var requiredAttr = prop.GetCustomAttribute<System.ComponentModel.DataAnnotations.RequiredAttribute>(inherit);
					if (requiredAttr != null)
						column.Attributes |= SqlColumnAttributes.Key;
					else {
						var keyAttr2 = prop.GetCustomAttribute<System.ComponentModel.DataAnnotations.KeyAttribute>(inherit);
						if (keyAttr2 != null) {
							column.Attributes |= SqlColumnAttributes.AutoKey;
							autoKeyCount++;
						}
					}
				}
				if (column.IsKey) {
					keys.Add(column);
					if (!prop.CanRead) {
						Attributes |= SqlTableAttributes.IgnoreInsert | SqlTableAttributes.IgnoreUpdate | SqlTableAttributes.IgnoreDelete;
					}
				}
			}

			if (keys.Count == 0) {
				KeyColumns = Array.Empty<SqlColumn>();
			}
			else {
				KeyColumns = keys.ToArray();
				if (KeyColumns.Count == 1 && KeyColumns[0].IsAutoKey && SqlInternal.IsValidAutoIncrementType(keys[0].Type)) {
					AutoKeyColumn = keys[0];
				}
				else if (autoKeyCount != 0 && KeyColumns.Count != autoKeyCount) {
					throw new InvalidOperationException(Type.FullName + " cannot have a both a composite key and an autoincrement key.");
				}
				else {
					// remove SqlColumnAttributes.AutoKey from all columns
					foreach (SqlColumn key in keys) {
						key.Attributes = SqlColumnAttributes.Key;
					}
				}
			}

			foreach (SqlColumn column in columns.Where(c => !c.IsKey)) {
				PropertyInfo prop = column.Property;

				// Deletes
				MatchDeleteAttribute deleteAttr = prop.GetCustomAttribute<MatchDeleteAttribute>(inherit);
				if (deleteAttr != null || IgnoreDelete)
					column.Attributes |= SqlColumnAttributes.IgnoreDelete;

				readOnlyAttr = prop.GetCustomAttribute<ReadOnlyAttribute>(inherit);
				if (readOnlyAttr != null) {
					column.Attributes |= SqlColumnAttributes.IgnoreInsert | SqlColumnAttributes.IgnoreUpdate;
					continue;
				}
				var editableAttr = prop.GetCustomAttribute<System.ComponentModel.DataAnnotations.EditableAttribute>(inherit);
				if (editableAttr != null) {
					if (!editableAttr.AllowInitialValue)
						column.Attributes |= SqlColumnAttributes.IgnoreUpdate;
					if (!editableAttr.AllowEdit)
						column.Attributes |= SqlColumnAttributes.IgnoreUpdate;
				}

				// Inserts
				if (IgnoreInsert)
					column.Attributes |= SqlColumnAttributes.IgnoreInsert;
				else {
					IgnoreInsertAttribute insertAttr = prop.GetCustomAttribute<IgnoreInsertAttribute>(inherit);
					if (insertAttr != null) {
						column.InsertValue = insertAttr.Value;
						if (insertAttr.AutoSync)
							column.Attributes |= SqlColumnAttributes.InsertAutoSync;
					}
				}

				// Updates
				if (IgnoreUpdate)
					column.Attributes = SqlColumnAttributes.IgnoreUpdate;
				else {
					IDefaultAttribute updateAttr = prop.GetCustomAttribute<IgnoreUpdateAttribute>(inherit);
					if (updateAttr != null) {
						column.Attributes |= SqlColumnAttributes.IgnoreUpdate;
						column.UpdateValue = updateAttr.Value;
						if (updateAttr.AutoSync)
							column.Attributes |= SqlColumnAttributes.UpdateAutoSync;
					}
					else {
						updateAttr = prop.GetCustomAttribute<MatchUpdateAttribute>(inherit); //NOTE: MatchUpdate != IgnoreUpdate
						if (updateAttr != null) {
							column.Attributes |= SqlColumnAttributes.MatchUpdate;
							column.UpdateValue = updateAttr.Value;
							if (updateAttr.AutoSync)
								column.Attributes |= SqlColumnAttributes.UpdateAutoSync;
						}
					}
				}
			}

			Columns = columns.Where(c => !c.NotMapped).ToArray();
			EqualityColumns = KeyColumns.Count == 0 || KeyColumns.Count == Columns.Count ? Columns : KeyColumns;
			UpdateKeyColumns = Columns == EqualityColumns ? Array.Empty<SqlColumn>() : Columns.Where(c => c.IsKey || c.MatchUpdate).ToArray();
			DeleteKeyColumns = Columns == EqualityColumns ? Columns : Columns.Where(c => c.IsKey || c.MatchDelete).ToArray();
		}

		/// <summary>
		/// The class type that represents the table.
		/// </summary>
		public Type Type { get; private set; }
		/// <summary>
		/// The attributes of the table.
		/// </summary>
		public SqlTableAttributes Attributes { get; private set; }
		public bool DeclaredOnly => Attributes.HasFlag(SqlTableAttributes.DeclaredOnly);
		public bool IgnoreDelete => Attributes.HasFlag(SqlTableAttributes.IgnoreDelete);
		public bool IgnoreInsert => Attributes.HasFlag(SqlTableAttributes.IgnoreInsert);
		public bool IgnoreUpdate => Attributes.HasFlag(SqlTableAttributes.IgnoreUpdate);

		/// <summary>
		/// The name of the table.
		/// </summary>
		public string TableName { get; private set; }
		/// <summary>
		/// The schema of the table.
		/// </summary>
		public string Schema { get; private set; }
		/// <summary>
		/// The syntax used to generate SQL commands.
		/// </summary>
		public SqlSyntax Syntax => Adapter.Syntax;
		/// <summary>
		/// Generates SQL commands using a given syntax.
		/// </summary>
		public SqlAdapter Adapter { get; private set; }
		/// <summary>
		///  All valid columns for the given type.
		/// </summary>
		public IReadOnlyList<SqlColumn> Columns { get; private set; }
		/// <summary>
		/// The columns that determine uniqueness. This is every column if there are no keys.
		/// </summary>
		public IReadOnlyList<SqlColumn> EqualityColumns { get; private set; }
		/// <summary>
		/// The columns that determine uniqueness.
		/// </summary>
		public IReadOnlyList<SqlColumn> KeyColumns { get; private set; }
		/// <summary>
		/// The column auto increment key if one exists.
		/// </summary>
		public SqlColumn AutoKeyColumn { get; private set; }
		/// <summary>
		/// The columns that are returned on select commands.
		/// </summary>
		public IEnumerable<SqlColumn> SelectColumns => Columns.Where(c => !c.IgnoreSelect);
		/// <summary>
		/// The columns that are modified on update commands. If this is an empty list then no updates are allowed.
		/// </summary>
		public IEnumerable<SqlColumn> UpdateColumns => Columns.Where(c => !c.IsKey && (!c.IgnoreUpdate || c.UpdateValue != null));
		/// <summary>
		/// The columns that need to be synchronized after inserts.
		/// </summary>
		public IEnumerable<SqlColumn> InsertAutoSyncColumns => Columns.Where(c => c.InsertAutoSync);
		/// <summary>
		/// The columns that need to be synchronized after updates.
		/// </summary>
		public IEnumerable<SqlColumn> UpdateAutoSyncColumns => Columns.Where(c => c.UpdateAutoSync);
		/// <summary>
		/// The columns that are inserted on insert commands. If this is an empty list then no inserts are allowed.
		/// </summary>
		public IEnumerable<SqlColumn> InsertColumns => Columns.Where(c => !c.IsAutoKey && (!c.IgnoreInsert || c.InsertValue != null));
		/// <summary>
		/// The columns that are inserted on insert commands. If this is an empty list then no inserts are allowed.
		/// </summary>
		public IEnumerable<SqlColumn> UpsertColumns => Columns.Where(c => !c.IgnoreInsert || !c.IgnoreUpdate || c.InsertValue != null || c.UpdateValue != null);
		/// <summary>
		/// The columns that determine equality when performing updates.
		/// </summary>
		public IReadOnlyList<SqlColumn> UpdateKeyColumns { get; private set; }
		/// <summary>
		/// The columns that determine equality when performing deletes.
		/// </summary>
		public IReadOnlyList<SqlColumn> DeleteKeyColumns { get; private set; }
		/// <summary>
		/// The columns that should be copied to a temporary staging table when performing bulk updates.
		/// </summary>
		public IEnumerable<SqlColumn> BulkUpdateColumns => Columns.Where(c => !c.IgnoreUpdate || c.UpdateValue != null);
		/// <summary>
		/// The columns that are inserted on insert-if-not-exists commands. If this is an empty list then no inserts are allowed.
		/// </summary>
		public IEnumerable<SqlColumn> BulkInsertIfNotExistsColumns => Columns.Where(c => !c.IgnoreInsert || c.InsertValue != null);
	}
}