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

using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Dapper.Extra.Cache
{
	/// <summary>
	/// The interface for the storage of a <see cref="DbCacheTable{T, R}"/>.
	/// </summary>
	/// <typeparam name="T">The table type.</typeparam>
	/// <typeparam name="R">The cached item type.</typeparam>
	public interface ICacheStorage<T, R> : IDictionary<T, R>
		where T : class
		where R : CacheItem<T>, new()
	{
		/// <summary>
		/// Adds or updates an object in the storage.
		/// </summary>
		/// <param name="value">The object to add to the storage.</param>
		/// <returns>The cache item referencing the added object.</returns>
		R Add(T value);

		/// <summary>
		/// Adds or updates the objects in the storage.
		/// </summary>
		/// <param name="values">The objects to add to the storage.</param>
		/// <returns>The cache items referencing the added objects.</returns>
		List<R> Add(IEnumerable<T> values);

		/// <summary>
		/// Attempts to remove an object matching a key.
		/// </summary>
		/// <param name="key">The key to remove.</param>
		/// <returns>True if the object was removed; false otherwise.</returns>
		bool RemoveKey(object key);

		/// <summary>
		/// Removes the objects from the storage.
		/// </summary>
		/// <param name="values">The objects to remove.</param>
		void Remove(IEnumerable<T> values);

		/// <summary>
		/// Removes the objects matching the specified keys from the storage.
		/// </summary>
		/// <param name="keys">The keys of the objects to remove.</param>
		void RemoveKeys(IEnumerable<object> keys);
		
		/// <summary>
		/// Determines whether the storage contains the specified object.
		/// </summary>
		/// <param name="value">The object to locate.</param>
		/// <returns>True if the object is in the storage; false otherwise.</returns>
		bool Contains(T value);

		/// <summary>
		/// Determines whether the storage contains the specified key.
		/// </summary>
		/// <param name="key">The key to locate.</param>
		/// <returns>True if the key is in the storage; false otherwise.</returns>
		bool ContainsKey(object key);

		/// <summary>
		/// Attempts to add the specified object to the storage.
		/// </summary>
		/// <param name="value">The element to add.</param>
		/// <returns>True if the element was added; false otherwise.</returns>
		bool TryAdd(T value);
	}
}
