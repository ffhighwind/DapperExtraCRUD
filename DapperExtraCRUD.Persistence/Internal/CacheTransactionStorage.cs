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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dapper.Extra.Persistence.Interfaces;

namespace Dapper.Extra.Persistence.Internal
{
	internal class CacheTransactionStorage<T> : ICacheStorage<T>, ITransactionStorage
		where T : class
	{
		private IDictionary<T, CacheItem<T>> Cache;
		private readonly List<Dictionary<T, CacheItem<T>>> SavePoints = new List<Dictionary<T, CacheItem<T>>>();
		private readonly List<string> SavePointNames = new List<string>();
		private Dictionary<T, CacheItem<T>> SavePoint;
		private readonly Action OnClose;

		internal CacheTransactionStorage(IDictionary<T, CacheItem<T>> cache, Action onClose)
		{
			Cache = cache;
			OnClose = onClose;
			Save(null);
		}

		#region ITransactionStorage
		public void Commit()
		{
			if (Cache != null) {
				foreach (var savePoint in SavePoints) {
					savePoint.Clear();
				}
				SavePoint = null;
				Cache = null;
				OnClose();
			}
		}

		public void Rollback()
		{
			Rollback(null);
		}

		public void Rollback(string savePointName)
		{
			int index = SavePointNames.IndexOf(savePointName);
			if (index < 0)
				throw new InvalidOperationException("Unknown save point: " + savePointName);
			for (int i = SavePointNames.Count; i >= index; i--) {
				SavePoint = SavePoints[i - 1];
				foreach (var kv in SavePoint) {
					if (kv.Value.Item == null)
						Cache.Remove(kv.Key);
					else if (Cache.TryGetValue(kv.Value.Item, out CacheItem<T> item))
						item.Item = kv.Value.Item;
					else
						Cache.Add(kv.Key, kv.Value);
				}
				SavePoint.Clear();
			}
			SavePoints.RemoveRange(index, SavePoints.Count - index);
			SavePointNames.RemoveRange(index, SavePoints.Count - index);
		}

		public void Save(string savePointName)
		{
			int index = SavePointNames.IndexOf(savePointName);
			if (index >= 0)
				throw new InvalidOperationException("Save point already exists: " + savePointName);
			SavePoint = new Dictionary<T, CacheItem<T>>(ExtraCrud.EqualityComparer<T>());
			SavePointNames.Add(savePointName);
			SavePoints.Add(SavePoint);
		}
		#endregion ITransactionStorage

		#region IReadOnlyDictionary<Key, T>
		public CacheItem<T> this[T key] => Cache[key];

		public IEnumerable<T> Keys => Cache.Keys;

		public IEnumerable<CacheItem<T>> Values => Cache.Values;

		public int Count => Cache.Count;

		public CacheItem<T> Add(T key, T value)
		{
			CacheItem<T> item = Add(value);
			return item;
		}

		public bool TryGetValue(T key, out CacheItem<T> value)
		{
			bool success = Cache.TryGetValue(key, out value);
			return success;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			IEnumerator<KeyValuePair<T, CacheItem<T>>> enumerator = GetEnumerator();
			return enumerator;
		}
		#endregion IReadOnlyDictionary<Key, T>

		#region ICacheStorage<KeyType, T>
		public CacheItem<T> Add(T value)
		{
			if (SavePoint.ContainsKey(value))
				SavePoint.Add(value, null); // force exception to be thrown
			CacheItem<T> item = new CacheItem<T>();
			item.Item = value;
			Cache.Add(value, item);
			return item;
		}

		public List<CacheItem<T>> Add(IEnumerable<T> values)
		{
			List<CacheItem<T>> list = new List<CacheItem<T>>();
			foreach (T value in values) {
				CacheItem<T> item = Add(value);
				list.Add(item);
			}
			return list;
		}

		public CacheItem<T> AddOrUpdate(T value)
		{
			if (!Cache.TryGetValue(value, out CacheItem<T> item)) {
				// keep item.Item as null for SavePoint
				item = new CacheItem<T>();
				Cache.Add(value, item);
			}
			if (!SavePoint.ContainsKey(value)) {
				CacheItem<T> removed = new CacheItem<T>();
				removed.Item = item.Item;
				SavePoint.Add(removed.Item, removed);
			}
			item.Item = value;
			return item;
		}

		public List<CacheItem<T>> AddOrUpdate(IEnumerable<T> values)
		{
			List<CacheItem<T>> list = new List<CacheItem<T>>();
			foreach (T value in values) {
				CacheItem<T> item = AddOrUpdate(value);
				list.Add(item);
			}
			return list;
		}

		public void Clear()
		{
			foreach (CacheItem<T> value in Cache.Values) {
				value.Item = null;
			}
			Cache.Clear();
			foreach (Dictionary<T, CacheItem<T>> savePoint in SavePoints) {
				savePoint.Clear();
			}
			SavePoint = SavePoints[0];
			SavePointNames.Clear();
			SavePoints.Clear();
			SavePoints.Add(SavePoint);
			SavePointNames.Add(null);
		}

		public bool Contains(T value)
		{
			bool success = Cache.ContainsKey(value);
			return success;
		}

		public bool ContainsKey<KeyType>(KeyType key)
		{
			T obj = TableData<T>.CreateObject<KeyType>(key);
			bool success = Cache.ContainsKey(obj);
			return success;
		}

		public bool ContainsKey(T key)
		{
			bool success = Cache.ContainsKey(key);
			return success;
		}

		public IEnumerator<KeyValuePair<T, CacheItem<T>>> GetEnumerator()
		{
			IEnumerator<KeyValuePair<T, CacheItem<T>>> enumerator = Cache.AsEnumerable().GetEnumerator();
			return enumerator;
		}

		public CacheItem<T> Remove(T value)
		{
			if (Cache.TryGetValue(value, out CacheItem<T> item)) {
				if (!SavePoint.ContainsKey(value)) {
					CacheItem<T> removed = new CacheItem<T>();
					removed.Item = item.Item;
					SavePoint.Add(value, removed);
				}
				Cache.Remove(item.Item);
				item.Item = null;
			}
			return item;
		}

		public void Remove(IEnumerable<T> values)
		{
			foreach (T value in values) {
				CacheItem<T> item = RemoveKey(value);
			}
		}

		public CacheItem<T> RemoveKey<KeyType>(KeyType key)
		{
			T obj = TableData<T>.CreateObject<KeyType>(key);
			CacheItem<T> item = Remove(obj);
			return item;
		}

		public void RemoveKeys<KeyType>(IEnumerable<KeyType> keys)
		{
			foreach (KeyType key in keys) {
				CacheItem<T> item = RemoveKey(key);
			}
		}
		#endregion ICacheStorage<KeyType, T>

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue) {
				if (disposing) {
					// TODO: dispose managed state (managed objects).
					OnClose();
					Rollback();
					Cache = null;
				}
				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.
				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~CacheTransactionStorage() {
		//   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		//   Dispose(false);
		// }

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// TODO: uncomment the following line if the finalizer is overridden above.
			// GC.SuppressFinalize(this);
		}
		#endregion
	}
}