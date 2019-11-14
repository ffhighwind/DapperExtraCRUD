using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Extra.Internal
{
	/// <summary>
	/// Stores metadata for for the given type.
	/// </summary>
	public sealed class SqlTypeInfo
	{
		public SqlTypeInfo(Type type)
		{
			Type = type;
			TableAttribute tableAttr = type.GetCustomAttribute<TableAttribute>(false);
			BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.SetProperty;
			if (tableAttr == null) {
				Adapter = SqlAdapter.GetAdapter(SqlSyntax.SQLServer);
				TableName = type.Name;
			}
			else {
				Adapter = SqlAdapter.GetAdapter(tableAttr.Syntax);
				TableName = string.IsNullOrWhiteSpace(tableAttr.Name) ? type.Name : tableAttr.Name.Trim();
				if (tableAttr.OnlyDeclaredProperties) {
					flags |= BindingFlags.DeclaredOnly;
					Attributes |= SqlTableAttributes.DeclaredOnly;
				}
			}

			PropertyInfo[] properties = type.GetProperties(flags);
			PropertyInfo[] validProperties = properties.Where(SqlInternal.IsValidProperty).ToArray();
			if (!validProperties.Any())
				throw new InvalidOperationException(type.FullName + " does not have any valid properties.");

			List<SqlColumn> keys = new List<SqlColumn>();
			List<SqlColumn> columns = new List<SqlColumn>();
			int autoKeyCount = 0;
			foreach (PropertyInfo prop in validProperties) {
				ColumnAttribute columnAttr = prop.GetCustomAttribute<ColumnAttribute>(false);
				string columnName = columnAttr == null || string.IsNullOrWhiteSpace(columnAttr.Name) ? prop.Name : Adapter.QuoteIdentifier(columnAttr.Name);
				SqlColumn column = new SqlColumn(prop, columnName);
				columns.Add(column);
				KeyAttribute keyAttr = prop.GetCustomAttribute<KeyAttribute>(false);
				if (keyAttr != null) {
					if (keyAttr.AutoIncrement) {
						autoKeyCount++;
					}
					column.Attributes = keyAttr.AutoIncrement ? SqlColumnAttributes.AutoKey : SqlColumnAttributes.Key;
					keys.Add(column);
				}
				else {
					IgnoreAttribute ignoreAttr = prop.GetCustomAttribute<IgnoreAttribute>(false);
					if (ignoreAttr != null)
						column.Attributes = SqlColumnAttributes.Ignore;
					else {
						IgnoreSelectAttribute selectAttr = prop.GetCustomAttribute<IgnoreSelectAttribute>(false);
						if (selectAttr != null)
							column.Attributes |= SqlColumnAttributes.IgnoreSelect;
						IgnoreInsertAttribute insertAttr = prop.GetCustomAttribute<IgnoreInsertAttribute>(false);
						if (insertAttr != null) {
							column.Attributes |= SqlColumnAttributes.IgnoreInsert;
							column.InsertValue = insertAttr.Value;
						}
						IDefaultAttribute updateAttr = prop.GetCustomAttribute<IgnoreUpdateAttribute>(false);
						if (updateAttr != null) {
							column.Attributes |= SqlColumnAttributes.IgnoreUpdate;
							column.UpdateValue = updateAttr.Value;
						}
						else {
							updateAttr = prop.GetCustomAttribute<MatchUpdateAttribute>(false);
							if (updateAttr != null) {
								column.Attributes |= SqlColumnAttributes.MatchUpdate;
								column.UpdateValue = updateAttr.Value;
							}
						}
						MatchDeleteAttribute deleteAttr = prop.GetCustomAttribute<MatchDeleteAttribute>(false);
						if (deleteAttr != null) {
							column.Attributes |= SqlColumnAttributes.MatchDelete;
						}
					}
				}
			}
			Columns = columns.ToArray();
			KeyColumns = keys.Count == 0 ? Array.Empty<SqlColumn>() : keys.ToArray();
			if (KeyColumns.Count == 1 && KeyColumns[0].IsAutoKey && SqlInternal.IsValidAutoIncrementType(keys[0].Type)) {
				AutoKeyColumn = keys[0];
			}
			else if (KeyColumns.Count != 0) {
				if(autoKeyCount != 0 && KeyColumns.Count != autoKeyCount) {
					throw new InvalidOperationException(Type.FullName + " cannot have a both a composite key and an autoincrement key.");
				}
				// remove SqlColumnAttributes.AutoKey from all columns
				foreach (SqlColumn key in keys) {
					key.Attributes = SqlColumnAttributes.Key;
				}
			}
			NoDeletesAttribute noDeletes = type.GetCustomAttribute<NoDeletesAttribute>(false);
			if (noDeletes != null)
				Attributes |= SqlTableAttributes.NoDeletes;
			NoInsertsAttribute noInserts = type.GetCustomAttribute<NoInsertsAttribute>(false);
			if (noInserts != null)
				Attributes |= SqlTableAttributes.NoInserts;
			NoUpdatesAttribute noUpdates = type.GetCustomAttribute<NoUpdatesAttribute>(false);
			if (noUpdates != null)
				Attributes |= SqlTableAttributes.NoUpdates;
			EqualityColumns = KeyColumns.Count == 0 || KeyColumns.Count == Columns.Count ? Columns : KeyColumns;
			UpdateKeyColumns = Columns == EqualityColumns ? Array.Empty<SqlColumn>() : Columns.Where(c => c.IsKey || c.MatchUpdate).ToArray();
			DeleteKeyColumns = Columns == EqualityColumns ? Columns : Columns.Where(c => c.IsKey || c.MatchDelete).ToArray();
			// common case -- reduce memory footprint
			if (UpdateKeyColumns.Count == 0)
				UpdateKeyColumns = Array.Empty<SqlColumn>();
			if (DeleteKeyColumns.Count == 0)
				DeleteKeyColumns = Array.Empty<SqlColumn>();
		}

		public Type Type { get; private set; }

		public SqlTableAttributes Attributes { get; private set; }
		public bool NoDeletes => Attributes.HasFlag(SqlTableAttributes.NoDeletes);
		public bool NoInserts => Attributes.HasFlag(SqlTableAttributes.NoInserts);
		public bool NoUpdates => Attributes.HasFlag(SqlTableAttributes.NoUpdates);

		/// <summary>
		/// The name of the table.
		/// </summary>
		public string TableName { get; private set; }
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
		public IEnumerable<SqlColumn> UpdateColumns {
			get {
				if (NoUpdates || EqualityColumns == Columns)
					return Array.Empty<SqlColumn>();
				return Columns.Where(c => !c.IsKey && (!c.IgnoreUpdate || c.UpdateValue != null));
			}
		}
		/// <summary>
		/// The columns that are inserted on insert commands. If this is an empty list then no inserts are allowed.
		/// </summary>
		public IEnumerable<SqlColumn> InsertColumns {
			get {
				if (NoInserts)
					return Array.Empty<SqlColumn>();
				return Columns.Where(c => !c.IsAutoKey && (!c.IgnoreInsert || c.UpdateValue != null));
			}
		}
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
		public IEnumerable<SqlColumn> BulkUpdateColumns {
			get {
				if (NoUpdates || EqualityColumns == Columns)
					return Array.Empty<SqlColumn>();
				return Columns.Where(c => !c.IgnoreUpdate || c.UpdateValue != null);
			}
		}
	}
}