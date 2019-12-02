﻿#region License
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

namespace Dapper.Extra.Cache.Interfaces
{
	public interface ICacheStorage<T, R> : IEnumerable<KeyValuePair<T, R>>, IReadOnlyDictionary<T, R>
		where T : class
		where R : CacheItem<T>
	{
		CacheItem<T> AddOrUpdate(T value);
		List<CacheItem<T>> AddOrUpdate(IEnumerable<T> values);
		CacheItem<T> Add(T value);
		List<CacheItem<T>> Add(IEnumerable<T> values);
		CacheItem<T> Remove(T value);
		CacheItem<T> RemoveKey(object key);
		void Remove(IEnumerable<T> values);
		void RemoveKeys(IEnumerable<object> keys);
		void Clear();
		bool Contains(T value);
		bool ContainsKey(object key);
	}
}