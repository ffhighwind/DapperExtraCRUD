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
using System.Collections;
using System.Collections.Generic;

namespace Dapper.Extra.Cache.Internal
{
	internal class CacheAutoStorage<T, R> : ICacheStorage<T, R>
		where T : class
		where R : CacheItem<T>, new()
	{
		public readonly Dictionary<T, R> Cache = new Dictionary<T, R>();

		internal CacheAutoStorage()
		{
			SqlBuilder<T> builder = ExtraCrud.Builder<T>();
			if (builder.Info.KeyColumns.Count == 1) {
				ObjectFromKey = builder.ObjectFromKey;
			}
		}

		private Func<object, T> ObjectFromKey { get; }

		public CacheItem<T> this[T key] => Cache[key];

		public int Count => Cache.Count;

		ICollection<T> IDictionary<T, R>.Keys => Cache.Keys;

		ICollection<R> IDictionary<T, R>.Values => Cache.Values;

		public bool IsReadOnly => false;

		R IDictionary<T, R>.this[T key] {
			get => Cache[key];
			set => Cache[key] = value;
		}

		public R Add(T key)
		{
			if(!Cache.TryGetValue(key, out R item)) {
				item = new R();
				Cache.Add(key, item);
			}
			item.CacheValue = key;
			return item;
		}

		public List<R> Add(IEnumerable<T> keys)
		{
			List<R> list = new List<R>();
			foreach (T key in keys) {
				R item = Add(key);
				list.Add(item);
			}
			return list;
		}

		public bool Remove(T value)
		{
			bool success = Cache.Remove(value);
			return success;
		}

		public bool RemoveKey(object key)
		{
			T obj = ObjectFromKey(key);
			bool success = Cache.Remove(obj);
			return success;
		}

		public void Remove(IEnumerable<T> keys)
		{
			foreach (T key in keys) {
				Remove(key);
			}
		}

		public void RemoveKeys(IEnumerable<object> keys)
		{
			foreach (object key in keys) {
				RemoveKey(key);
			}
		}

		public void Clear()
		{
			Cache.Clear();
		}

		public bool Contains(T key)
		{
			bool success = Cache.ContainsKey(key);
			return success;
		}

		public bool ContainsKey(object key)
		{
			T obj = ObjectFromKey(key);
			bool success = Cache.ContainsKey(obj);
			return success;
		}

		public bool ContainsKey(T key)
		{
			bool success = Cache.ContainsKey(key);
			return success;
		}

		public bool TryGetValue(T key, out R value)
		{
			bool success = Cache.TryGetValue(key, out value);
			return success;
		}

		public IEnumerator<KeyValuePair<T, R>> GetEnumerator()
		{
			IEnumerator<KeyValuePair<T, R>> enumerator = Cache.GetEnumerator();
			return enumerator;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			IEnumerator<KeyValuePair<T, R>> enumerator = Cache.GetEnumerator();
			return enumerator;
		}

		public void Add(T key, R value)
		{
			Add(key);
		}

		public void Add(KeyValuePair<T, R> item)
		{
			((IDictionary<T, R>)Cache).Add(item);
		}

		public bool Contains(KeyValuePair<T, R> item)
		{
			bool success = ((IDictionary<T, R>)Cache).Contains(item);
			return success;
		}

		public void CopyTo(KeyValuePair<T, R>[] array, int arrayIndex)
		{
			((IDictionary<T, R>)Cache).CopyTo(array, arrayIndex);
		}

		public bool Remove(KeyValuePair<T, R> item)
		{
			bool success = ((IDictionary<T, R>)Cache).Remove(item);
			return success;
		}
	}
}
