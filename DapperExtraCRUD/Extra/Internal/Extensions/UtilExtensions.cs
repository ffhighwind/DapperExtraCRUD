using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Extra.Internal.Extensions
{
	public static class UtilExtensions
	{
		public static IEnumerable<List<T>> Partition<T>(this IList<T> source, int size)
		{
			for (int i = 0; i < source.Count; i += size)
				yield return new List<T>(source.Skip(i).Take(size));
		}

		public static IEnumerable<IEnumerable<T>> Partition<T>(this IEnumerable<T> source, int size)
		{
			while(source.Any()) {
				yield return source.Take(size);
				source.Skip(size);
			}
		}
	}
}
