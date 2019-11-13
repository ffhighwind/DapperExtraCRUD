using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Extra.Internal.Extensions
{
	public static class UtilExtensions
	{
		/// <summary>
		/// Partitions a list into multiple lists of the given size.
		/// </summary>
		/// <typeparam name="T">The type of list.</typeparam>
		/// <param name="source">The list to partition.</param>
		/// <param name="size">The maximum objects per partition.</param>
		/// <returns>Partitions of the list.</returns>
		public static IEnumerable<List<T>> Partition<T>(this IList<T> source, int size)
		{
			for (int i = 0; i < source.Count; i += size)
				yield return new List<T>(source.Skip(i).Take(size));
		}

		/// <summary>
		/// Partitions a list into multiple lists of the given size.
		/// </summary>
		/// <typeparam name="T">The type of list.</typeparam>
		/// <param name="source">The list to partition.</param>
		/// <param name="size">The maximum objects per partition.</param>
		/// <returns>Partitions of the list.</returns>
		public static IEnumerable<IEnumerable<T>> Partition<T>(this IEnumerable<T> source, int size)
		{
			while(source.Any()) {
				yield return source.Take(size);
				source.Skip(size);
			}
		}
	}
}
