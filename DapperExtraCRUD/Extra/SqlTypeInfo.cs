#region License
// Released under MIT License 
// License: https://opensource.org/licenses/MIT
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
using Dapper.Extra.Annotations;
using Dapper.Extra.Internal;

namespace Dapper.Extra
{
	/// <summary>
	/// Stores metadata for for the given table type.
	/// </summary>
	public sealed class SqlTypeInfo
	{
		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlTypeInfo"/> class.
		/// </summary>
		/// <param name="type">The table type.</param>
		/// <param name="dialect">The dialect used to generate SQL commands.</param>
		public SqlTypeInfo(Type type, SqlDialect dialect = SqlDialect.SQLServer)
			: this(type, SqlAdapter.GetAdapter(dialect))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlTypeInfo"/> class.
		/// </summary>
		/// <param name="type">The table type.</param>
		/// <param name="adapter">The adapter used to generate SQL commands.</param>
		public SqlTypeInfo(Type type, ISqlAdapter adapter)
		{
			Type = type;
			Adapter = adapter ?? SqlAdapter.SQLServer;
			TableAttribute tableAttr = type.GetCustomAttribute<TableAttribute>(false);
			BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.NonPublic;
			bool inherit = false;
			if (tableAttr == null) {
				var tableAttr2 = type.GetCustomAttribute<System.ComponentModel.DataAnnotations.Schema.TableAttribute>(false);
				Adapter = SqlAdapter.GetAdapter(SqlDialect.SQLServer);
				TableName = tableAttr2 != null ? tableAttr2.Name : type.Name;
			}
			else {
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
			PropertyInfo[] validProperties = properties.Where(ExtraCrud.IsValidProperty).ToArray();
			if (!validProperties.Any())
				throw new InvalidOperationException(type.FullName + " does not have any valid properties.");
			ReadOnlyAttribute readOnlyAttr = type.GetCustomAttribute<ReadOnlyAttribute>(false);
			if (readOnlyAttr != null)
				Attributes |= SqlTableAttributes.IgnoreInsert | SqlTableAttributes.IgnoreUpdate | SqlTableAttributes.IgnoreDelete;
			else {
				var editableAttr = type.GetCustomAttribute<System.ComponentModel.DataAnnotations.EditableAttribute>(false);
				AutoSyncAttribute autoSyncAttr = type.GetCustomAttribute<AutoSyncAttribute>(false);
				if (autoSyncAttr != null) {
					if (autoSyncAttr.SyncUpdate)
						Attributes |= SqlTableAttributes.UpdateAutoSync;
					if (autoSyncAttr.SyncInsert)
						Attributes |= SqlTableAttributes.InsertAutoSync;
				}
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
				}
				else {
					var columnAttr2 = prop.GetCustomAttribute<System.ComponentModel.DataAnnotations.Schema.ColumnAttribute>(inherit);
					if (columnAttr2 != null) {
						columnName = columnAttr2.Name;
					}
				}
				string propName = Adapter.QuoteIdentifier(prop.Name);
				SqlColumn column = new SqlColumn(prop, string.IsNullOrWhiteSpace(columnName) ? propName : Adapter.QuoteIdentifier(columnName), propName, i);
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
						var keyAttrCm = prop.GetCustomAttribute<System.ComponentModel.DataAnnotations.KeyAttribute>(inherit);
						if (keyAttrCm != null) {
							column.Attributes |= SqlColumnAttributes.AutoKey;
							autoKeyCount++;
						}
					}
				}
				if (column.IsKey) {
					keys.Add(column);
					if (!prop.CanRead || !prop.GetMethod.IsPublic || prop.GetMethod.IsStatic) {
						Attributes |= SqlTableAttributes.IgnoreInsert | SqlTableAttributes.IgnoreUpdate | SqlTableAttributes.IgnoreDelete;
					}
				}
				columns.Add(column);
			}
			if (keys.Count == 0) {
				SqlColumn key = columns.FirstOrDefault(c => string.Equals(c.ColumnName, "ID", StringComparison.OrdinalIgnoreCase));
				if (key == null) {
					int len = type.Name.Length + 2;
					key = columns.FirstOrDefault(c => c.ColumnName.Length == len && c.ColumnName.StartsWith(type.Name) && c.ColumnName.EndsWith("ID", StringComparison.OrdinalIgnoreCase));
				}
				if (key != null) {
					key.Attributes |= SqlColumnAttributes.AutoKey;
					keys.Add(key);
				}
				else
					KeyColumns = Constants.SqlColumnsEmpty;
			}
			if (keys.Count != 0) {
				KeyColumns = keys.ToArray();
				if (KeyColumns.Count == 1 && KeyColumns[0].IsAutoKey && ExtraCrud.IsValidAutoIncrementType(KeyColumns[0].Type)) {
					AutoKeyColumn = KeyColumns[0];
				}
				else if (autoKeyCount != 0 && KeyColumns.Count != autoKeyCount) {
					throw new InvalidOperationException(Type.FullName + " cannot have a both a composite key and an autoincrement key.");
				}
				else {
					// remove SqlColumnAttributes.AutoKey from all key columns
					SqlColumnAttributes invertedAutoKey = ~(SqlColumnAttributes.AutoKey ^ SqlColumnAttributes.Key);
					foreach (SqlColumn key in KeyColumns) {
						key.Attributes &= invertedAutoKey;
					}
				}
			}

			foreach (SqlColumn column in columns.Where(c => !c.IsKey)) {
				PropertyInfo prop = column.Property;

				bool selectOnly = !prop.CanRead || !prop.GetMethod.IsPublic || prop.GetMethod.IsStatic;
				if (selectOnly)
					column.Attributes |= SqlColumnAttributes.IgnoreInsert | SqlColumnAttributes.IgnoreUpdate | SqlColumnAttributes.IgnoreDelete;

				// Selects
				IgnoreSelectAttribute selectAttr = prop.GetCustomAttribute<IgnoreSelectAttribute>(inherit);
				if (selectAttr != null) {
					column.Attributes |= SqlColumnAttributes.IgnoreSelect;
					if (selectOnly)
						continue;
				}
				else if (selectOnly)
					continue;
				else {
					AutoSyncAttribute autoSyncAttr = prop.GetCustomAttribute<AutoSyncAttribute>(inherit);
					if (autoSyncAttr != null) {
						if (autoSyncAttr.SyncInsert)
							column.Attributes |= SqlColumnAttributes.InsertAutoSync;
						if (autoSyncAttr.SyncUpdate)
							column.Attributes |= SqlColumnAttributes.UpdateAutoSync;
					}
					if (InsertAutoSync)
						column.Attributes |= SqlColumnAttributes.InsertAutoSync;
					if (UpdateAutoSync)
						column.Attributes |= SqlColumnAttributes.UpdateAutoSync;
				}

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
				IgnoreInsertAttribute insertAttr = prop.GetCustomAttribute<IgnoreInsertAttribute>(inherit);
				if (insertAttr != null) {
					column.Attributes |= SqlColumnAttributes.IgnoreInsert;
					column.InsertValue = insertAttr.Value;
					if (insertAttr.AutoSync)
						column.Attributes |= SqlColumnAttributes.InsertAutoSync;
				}
				else if (IgnoreInsert)
					column.Attributes |= SqlColumnAttributes.IgnoreInsert;

				// Updates
				IgnoreUpdateAttribute ignoreUpdateAttr = prop.GetCustomAttribute<IgnoreUpdateAttribute>(inherit);
				if (ignoreUpdateAttr != null) {
					column.Attributes |= SqlColumnAttributes.IgnoreUpdate;
					column.UpdateValue = ignoreUpdateAttr.Value;
					if (ignoreUpdateAttr.AutoSync)
						column.Attributes |= SqlColumnAttributes.UpdateAutoSync;
				}
				else if (IgnoreUpdate)
					column.Attributes = SqlColumnAttributes.IgnoreUpdate;
				else {
					MatchUpdateAttribute matchUpdateAttr = prop.GetCustomAttribute<MatchUpdateAttribute>(inherit); //NOTE: MatchUpdate != IgnoreUpdate
					if (matchUpdateAttr != null) {
						column.Attributes |= SqlColumnAttributes.MatchUpdate;
						column.UpdateValue = matchUpdateAttr.Value;
						if (matchUpdateAttr.AutoSync)
							column.Attributes |= SqlColumnAttributes.UpdateAutoSync;
					}
				}
			}

			Columns = columns.Where(c => !c.NotMapped).ToArray();
			EqualityColumns = KeyColumns.Count == 0 || KeyColumns.Count == Columns.Count ? Columns : KeyColumns;
			UpdateKeyColumns = Columns == EqualityColumns ? Constants.SqlColumnsEmpty : Columns.Where(c => c.IsKey || c.MatchUpdate).ToArray();
			DeleteKeyColumns = Columns == EqualityColumns ? Columns : Columns.Where(c => c.IsKey || c.MatchDelete).ToArray();
		}

		#endregion

		#region Properties

		/// <summary>
		/// Generates SQL commands using a given dialect.
		/// </summary>
		public ISqlAdapter Adapter { get; private set; }

		/// <summary>
		/// The table attributes.
		/// </summary>
		public SqlTableAttributes Attributes { get; private set; }

		/// <summary>
		/// The column auto increment key if one exists.
		/// </summary>
		public SqlColumn AutoKeyColumn { get; private set; }

		/// <summary>
		/// The columns that are inserted on insert-if-not-exists commands. If this is an empty list then no inserts are allowed.
		/// </summary>
		public IEnumerable<SqlColumn> BulkInsertIfNotExistsColumns => Columns.Where(c => !c.IgnoreInsert || c.InsertValue != null);

		/// <summary>
		/// The columns that should be copied to a temporary staging table when performing bulk updates.
		/// </summary>
		public IEnumerable<SqlColumn> BulkUpdateColumns => Columns.Where(c => !c.IgnoreUpdate || c.UpdateValue != null);

		/// <summary>
		///  All valid columns for the given type.
		/// </summary>
		public IReadOnlyList<SqlColumn> Columns { get; private set; }

		/// <summary>
		/// Determines if the properties are declared-only (top-level).
		/// </summary>
		public bool DeclaredOnly => Attributes.HasFlag(SqlTableAttributes.DeclaredOnly);

		/// <summary>
		/// The columns that determine equality when performing deletes.
		/// </summary>
		public IReadOnlyList<SqlColumn> DeleteKeyColumns { get; private set; }

		/// <summary>
		/// The columns that determine uniqueness. This is every column if there are no keys.
		/// </summary>
		public IReadOnlyList<SqlColumn> EqualityColumns { get; private set; }

		/// <summary>
		/// Determines if deletes are ignored.
		/// </summary>
		public bool IgnoreDelete => Attributes.HasFlag(SqlTableAttributes.IgnoreDelete);

		/// <summary>
		/// Determines if inserts are ignored.
		/// </summary>
		public bool IgnoreInsert => Attributes.HasFlag(SqlTableAttributes.IgnoreInsert);

		/// <summary>
		/// Determines if objects should be synchronized after insert.
		/// </summary>
		public bool InsertAutoSync => Attributes.HasFlag(SqlTableAttributes.InsertAutoSync);

		/// <summary>
		/// Determines if objects should be synchronized after updates.
		/// </summary>
		public bool UpdateAutoSync => Attributes.HasFlag(SqlTableAttributes.UpdateAutoSync);

		/// <summary>
		/// Determines if updates are ignored.
		/// </summary>
		public bool IgnoreUpdate => Attributes.HasFlag(SqlTableAttributes.IgnoreUpdate);

		/// <summary>
		/// The columns that need to be synchronized after inserts.
		/// </summary>
		public IEnumerable<SqlColumn> InsertAutoSyncColumns => Columns.Where(c => c.InsertAutoSync);

		/// <summary>
		/// The columns that are inserted on insert commands. If this is an empty list then no inserts are allowed.
		/// </summary>
		public IEnumerable<SqlColumn> InsertColumns => Columns.Where(c => !c.IsAutoKey && (!c.IgnoreInsert || c.InsertValue != null));

		/// <summary>
		/// The columns that determine uniqueness.
		/// </summary>
		public IReadOnlyList<SqlColumn> KeyColumns { get; private set; }

		/// <summary>
		/// The schema of the table.
		/// </summary>
		public string Schema { get; private set; }

		/// <summary>
		/// The columns that are returned on select commands.
		/// </summary>
		public IEnumerable<SqlColumn> SelectColumns => Columns.Where(c => !c.IgnoreSelect);

		/// <summary>
		/// The dialect used to generate SQL commands.
		/// </summary>
		public SqlDialect Dialect => Adapter.Dialect;

		/// <summary>
		/// The name of the table.
		/// </summary>
		public string TableName { get; private set; }

		/// <summary>
		/// The class type that represents the table.
		/// </summary>
		public Type Type { get; private set; }

		/// <summary>
		/// The columns that need to be synchronized after updates.
		/// </summary>
		public IEnumerable<SqlColumn> UpdateAutoSyncColumns => Columns.Where(c => c.UpdateAutoSync);

		/// <summary>
		/// The columns that are modified on update commands. If this is an empty list then no updates are allowed.
		/// </summary>
		public IEnumerable<SqlColumn> UpdateColumns => Columns.Where(c => !c.IsKey && ((!c.IgnoreUpdate && !c.MatchUpdate) || c.UpdateValue != null));

		/// <summary>
		/// The columns that determine equality when performing updates.
		/// </summary>
		public IReadOnlyList<SqlColumn> UpdateKeyColumns { get; private set; }

		/// <summary>
		/// The columns that are inserted on insert commands. If this is an empty list then no inserts are allowed.
		/// </summary>
		public IEnumerable<SqlColumn> UpsertColumns => Columns.Where(c => !c.IgnoreInsert || !c.IgnoreUpdate || c.InsertValue != null || c.UpdateValue != null);

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		public override string ToString()
		{
			return "(SqlTypeInfo " + Type.FullName + ")";
		}

		#endregion
	}
}
