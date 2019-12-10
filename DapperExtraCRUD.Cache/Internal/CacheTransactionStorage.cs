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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;

namespace Dapper.Extra.Cache.Internal
{
	internal class CacheTransactionStorage<T, R> : ICacheStorage<T, R>, IDbTransaction
		where T : class
		where R : CacheItem<T>, new()
	{
		/// <summary>
		/// The current state.
		/// </summary>
		private ConcurrentDictionary<T, R> Cache { get; set; }
		/// <summary>
		///  A backup of what was changed in the Cache in case a rollback is needed. A CacheValue will be null if an item was added
		///  during the transaction and needs to be removed on a rollback.
		/// </summary>
		private ConcurrentDictionary<T, R> SavePoint { get; set; } = new ConcurrentDictionary<T, R>();
		/// <summary>
		/// Resets the state of the DbCacheTable that created this object. The AccessObject will be changed back to the 
		/// <see cref="Utilities.AutoAccessObject{T}"/> and the <see cref="ICacheStorage{T, R}"/> will be changed back to 
		/// the <see cref="CacheAutoStorage{T, R}"/>.
		/// </summary>
		private readonly Action OnClose;
		/// <summary>
		/// The transaction for the current connection.
		/// </summary>
		private readonly IDbTransaction Transaction;
		/// <summary>
		/// Creates an object from a single value.
		/// </summary>
		private readonly Func<object, T> CreateFromKey;

		public CacheTransactionStorage(ConcurrentDictionary<T, R> cache, IDbTransaction transaction, Func<object, T> createFromKey, Action onClose)
		{
			Cache = cache;
			Transaction = transaction;
			OnClose = onClose;
			CreateFromKey = createFromKey;
		}

		private R UpdateValueFactory(T t, R r)
		{
			r.CacheValue = t;
			return r;
		}

		private R AddValueFactory(T t)
		{
			R item = new R();
			item.CacheValue = t;
			return item;
		}

		public IDbConnection Connection => Transaction.Connection;

		public IsolationLevel IsolationLevel => Transaction.IsolationLevel;

		public ICollection<T> Keys => Cache.Keys;

		public ICollection<R> Values => Cache.Values;

		public int Count => Cache.Count;

		public bool IsReadOnly => false;

		public R this[T key] {
			get => Cache[key];
			set {
				if (value == null)
					_ = Remove(key);
				else
					_ = Add(key);
			}
		}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue) {
				if (disposing) {
					// dispose managed state (managed objects).
					OnClose();
					Rollback();
				}
				// free unmanaged resources (unmanaged objects) and override a finalizer below.
				// set large fields to null.
				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~CacheTransactionStorage()
		// {
		//   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		//   Dispose(false);
		// }

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// uncomment the following line if the finalizer is overridden above.
			// GC.SuppressFinalize(this);
		}
		#endregion IDisposable

		public void Commit()
		{
			if (SavePoint != null) {
				SavePoint = null;
				Cache = null;
				OnClose();
			}
		}

		public void Rollback()
		{
			if (SavePoint == null)
				return;
			foreach (var kv in SavePoint) {
				if (kv.Value.CacheValue == null)
					Cache.TryRemove(kv.Key, out R item);
				else
					Cache.AddOrUpdate(kv.Key, AddValueFactory, UpdateValueFactory);
			}
			SavePoint.Clear();
			SavePoint = null;
			Cache = null;
		}

		public R Add(T value)
		{
			if (!Cache.TryGetValue(value, out R item))
				item = new R();
			SavePoint.TryAdd(value, item);
			item = Cache.AddOrUpdate(value, AddValueFactory, UpdateValueFactory);
			return item;
		}

		public List<R> Add(IEnumerable<T> values)
		{
			List<R> list = new List<R>();
			foreach (T value in values) {
				R item = Add(value);
				list.Add(item);
			}
			return list;
		}

		public void Remove(IEnumerable<T> values)
		{
			foreach (T value in values) {
				_ = RemoveKey(value);
			}
		}

		public bool RemoveKey(object key)
		{
			T obj = CreateFromKey(key);
			bool success = Remove(obj);
			return success;
		}

		public void RemoveKeys(IEnumerable<object> keys)
		{
			foreach (object key in keys) {
				_ = RemoveKey(key);
			}
		}

		public bool Contains(T value)
		{
			bool success = Cache.ContainsKey(value);
			return success;
		}

		public bool ContainsKey(object key)
		{
			T obj = CreateFromKey(key);
			bool success = Contains(obj);
			return success;
		}

		public bool ContainsKey(T key)
		{
			bool success = Cache.ContainsKey(key);
			return success;
		}

		public void Add(T key, R value)
		{
			if (!Cache.TryGetValue(key, out R item))
				item = new R();
			SavePoint.TryAdd(key, item);
			item = Cache.AddOrUpdate(key, value, UpdateValueFactory);
		}

		public bool Remove(T key)
		{
			if (Cache.TryRemove(key, out R item)) {
				SavePoint.TryAdd(key, item);
				return true;
			}
			return false;
		}

		public bool TryGetValue(T key, out R value)
		{
			bool success = Cache.TryGetValue(key, out value);
			return success;
		}

		public void Add(KeyValuePair<T, R> item)
		{
			((IDictionary<T, R>)Cache).Add(item);
		}

		public void Clear()
		{
			Cache.Clear();
			SavePoint.Clear();
		}

		public bool Contains(KeyValuePair<T, R> item)
		{
			return ((IDictionary<T, R>)Cache).Contains(item);
		}

		public void CopyTo(KeyValuePair<T, R>[] array, int arrayIndex)
		{
			((IDictionary<T, R>)Cache).CopyTo(array, arrayIndex);
		}

		public bool Remove(KeyValuePair<T, R> item)
		{
			return ((IDictionary<T, R>)Cache).Remove(item);
		}

		public IEnumerator<KeyValuePair<T, R>> GetEnumerator()
		{
			return Cache.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return Cache.GetEnumerator();
		}

		public bool TryAdd(T value)
		{
			bool success = GetOrAdd(value).CacheValue == value;
			return success;
		}

		public R GetOrAdd(T value)
		{
			if (Cache.TryGetValue(value, out R item)) {
				return item;
			}
			item = Cache.GetOrAdd(value, AddValueFactory);
			return item;
		}
	}
}
