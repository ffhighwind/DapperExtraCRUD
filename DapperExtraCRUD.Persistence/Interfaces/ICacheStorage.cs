using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Extra.Persistence.Interfaces
{
	public interface ICacheStorage<T> : IEnumerable<KeyValuePair<T, CacheItem<T>>>, IReadOnlyDictionary<T, CacheItem<T>>
		where T : class
	{
		CacheItem<T> AddOrUpdate(T value);
		List<CacheItem<T>> AddOrUpdate(IEnumerable<T> values);
		CacheItem<T> Add(T value);
		List<CacheItem<T>> Add(IEnumerable<T> values);
		CacheItem<T> Remove(T value);
		CacheItem<T> RemoveKey<KeyType>(KeyType key);
		void Remove(IEnumerable<T> values);
		void RemoveKeys<KeyType>(IEnumerable<KeyType> keys);
		void Clear();
		bool Contains(T value);
		bool ContainsKey<KeyType>(KeyType key);
	}
}
