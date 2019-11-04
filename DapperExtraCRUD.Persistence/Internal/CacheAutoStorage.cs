using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.Extra.Persistence.Interfaces;

namespace Dapper.Extra.Persistence.Internal
{
	internal class CacheAutoStorage<T> : ICacheStorage<T>
		where T : class
	{
		private readonly IDictionary<T, CacheItem<T>> Cache;

		internal CacheAutoStorage(IDictionary<T, CacheItem<T>> cache)
		{
			Cache = cache;
		}

		#region ICacheStorage<T>
		public CacheItem<T> this[T key] => Cache[key];

		public IEnumerable<T> Keys => Cache.Keys;

		public IEnumerable<CacheItem<T>> Values => Cache.Values;

		public int Count => Cache.Count;

		public CacheItem<T> Add(T value)
		{
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
			CacheItem<T> item;
			if (!Cache.TryGetValue(value, out item)) {
				item = new CacheItem<T>();
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
		}

		public bool Contains(T value)
		{
			bool success = Cache.ContainsKey(value);
			return success;
		}

		public bool ContainsKey(T key)
		{
			bool exists = Cache.ContainsKey(key);
			return exists;
		}

		public bool ContainsKey<KeyType>(KeyType key)
		{
			T obj = TableData<T>.CreateObject<KeyType>(key);
			bool success = Cache.ContainsKey(obj);
			return success;
		}

		public IEnumerator<KeyValuePair<T, CacheItem<T>>> GetEnumerator()
		{
			IEnumerator<KeyValuePair<T, CacheItem<T>>> enumerator = Cache.GetEnumerator();
			return enumerator;
		}

		public CacheItem<T> Remove(T key)
		{
			return RemoveKey(key);
		}

		public void Remove(IEnumerable<T> values)
		{
			RemoveKeys(values);
		}

		public CacheItem<T> RemoveKey(T key)
		{
			if (Cache.TryGetValue(key, out CacheItem<T> item)) {
				item.Item = null;
				Cache.Remove(key);
			}
			return item;
		}

		public CacheItem<T> RemoveKey<KeyType>(KeyType key)
		{
			throw new NotImplementedException();
		}

		public void RemoveKeys(IEnumerable<T> keys)
		{
			foreach (T key in keys) {
				RemoveKey(key);
			}
		}

		public void RemoveKeys<KeyType>(IEnumerable<KeyType> keys)
		{
			throw new NotImplementedException();
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
		#endregion ICacheStorage<T>
	}
}
