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

using System.Collections.Generic;
using System.Linq;

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
			while (source.Any()) {
				yield return source.Take(size);
				source = source.Skip(size);
			}
		}
	}
}
