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
using System.Collections.Generic;

namespace Dapper.Extra.Cache
{
	/// <summary>
	/// A factory for database caches.
	/// </summary>
	public class DbCache
	{
		/// <summary>
		/// A cache of <see cref="DbCacheTable{T, R}"/>.
		/// </summary>
		private readonly Dictionary<Type, object> Map = new Dictionary<Type, object>();

		private readonly string ConnectionString;

		/// <summary>
		/// Initializes a new instance of the <see cref="DbCache"/> class.
		/// </summary>
		/// <param name="connectionString">The connection string</param>
		public DbCache(string connectionString)
		{
			ConnectionString = connectionString;
		}

		/// <summary>
		/// Constructs a <see cref="DbCacheTable{T, R}"/> that stores the default <see cref="CacheItem{T}"/>.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <returns>A cache for a database table.</returns>
		public DbCacheTable<T, CacheItem<T>> CreateTable<T>()
			where T : class
		{
			return CreateTable<T, CacheItem<T>>();
		}

		/// <summary>
		/// Constructs a <see cref="DbCacheTable{T, R}"/> that stores a specific <see cref="CacheItem{T}"/> type.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <typeparam name="R">The <see cref="CacheItem{T}"/> type.</typeparam>
		/// <returns>A cache for a database table.</returns>
		public DbCacheTable<T, R> CreateTable<T, R>()
			where T : class
			where R : CacheItem<T>, new()
		{
			DbCacheTable<T, R> table;
			if (Map.TryGetValue(typeof(T), out object obj)) {
				table = (DbCacheTable<T, R>)obj;
			}
			else {
				table = new DbCacheTable<T, R>(ConnectionString);
				Map.Add(typeof(T), table);
			}
			return table;
		}
	}
}