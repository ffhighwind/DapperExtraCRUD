using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace Dapper.Extra.Persistence
{
	public sealed class CacheItem<T>
		where T : class
	{
		internal CacheItem()
		{
		}

		/// <summary>
		/// Null if deleted.
		/// </summary>
		public T Item { get; internal set; }
		public void Delete() => Item = null;
	}
}
