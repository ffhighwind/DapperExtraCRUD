using System;

namespace Dapper.Extra.Internal
{
	/// <summary>
	/// Framework specific constants.
	/// </summary>
	internal static class Constants
	{
#if NET45
		public static readonly SqlColumn[] SqlColumnsEmpty = new SqlColumn[0];
#else
		public static SqlColumn[] SqlColumnsEmpty => Array.Empty<SqlColumn>();
#endif
	}
}
