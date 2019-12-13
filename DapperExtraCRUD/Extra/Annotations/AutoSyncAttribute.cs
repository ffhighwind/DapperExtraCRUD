using System;

namespace Dapper.Extra.Annotations
{
	/// <summary>
	/// Synchronizes the column with the database on updates/inserts.
	/// </summary>
	public class AutoSyncAttribute : Attribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AutoSyncAttribute"/> attribute.
		/// </summary>
		/// <param name="syncInsert">Determines if this column will be automatically selected after an insert.</param>
		/// <param name="syncUpdate">Determines if this column will be automatically selected after an update.</param>
		public AutoSyncAttribute(bool syncInsert = true, bool syncUpdate = true)
		{
			SyncInsert = syncInsert;
			SyncUpdate = syncUpdate;
		}

		/// <summary>
		/// Determines if this column will be automatically selected after an insert.
		/// </summary>
		public bool SyncInsert { get; }

		/// <summary>
		/// Determines if this column will be automatically selected after an update.
		/// </summary>
		public bool SyncUpdate { get; }
	}
}
