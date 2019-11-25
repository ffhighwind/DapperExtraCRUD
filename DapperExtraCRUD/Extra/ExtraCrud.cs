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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.Extra.Internal;

namespace Dapper.Extra
{
	public static class ExtraCrud
	{
		public static SqlSyntax Syntax { get; set; } = SqlSyntax.SQLServer;
		private static ConcurrentDictionary<Type, object> TypeInfoCache = new ConcurrentDictionary<Type, object>();
		private static ConcurrentDictionary<Type, object> BuilderCache = new ConcurrentDictionary<Type, object>();
		private static ConcurrentDictionary<Type, object> QueriesCache = new ConcurrentDictionary<Type, object>();
		private static ConcurrentDictionary<Type, object> KeyQueriesCache = new ConcurrentDictionary<Type, object>();

		public static SqlTypeInfo TypeInfo<T>() where T : class
		{
			Type type = typeof(T);
			if (TypeInfoCache.TryGetValue(type, out object obj)) {
				return (SqlTypeInfo) obj;
			}
			SqlTypeInfo typeInfo = new SqlTypeInfo(type);
			return (SqlTypeInfo) TypeInfoCache.GetOrAdd(type, typeInfo);
		}

		/// <summary>
		/// Creates or gets a cached <see cref="SqlBuilder{T}"/>.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <returns>The <see cref="SqlBuilder{T}"/>.</returns>
		public static SqlBuilder<T> Builder<T>() where T : class
		{
			Type type = typeof(T);
			if (BuilderCache.TryGetValue(type, out object obj)) {
				return (SqlBuilder<T>) obj;
			}
			SqlTypeInfo typeInfo = TypeInfo<T>();
			SqlBuilder<T> builder = new SqlBuilder<T>(typeInfo);
			return (SqlBuilder<T>) BuilderCache.GetOrAdd(type, builder);
		}

		/// <summary>
		/// Creates or gets a cached <see cref="SqlBuilder{T, KeyType}"/>.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <typeparam name="KeyType">The key type.</typeparam>
		/// <returns>The <see cref="SqlBuilder{T, KeyType}"/>.</returns>
		public static SqlBuilder<T, KeyType> Builder<T, KeyType>() where T : class
		{
			SqlBuilder<T> builder = Builder<T>();
			return builder.Create<KeyType>();
		}

		/// <summary>
		/// Creates or gets the queries and commands for a given type.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <returns>The queries for the given type.</returns>
		public static SqlQueries<T> Queries<T>() where T : class
		{
			Type type = typeof(T);
			if (QueriesCache.TryGetValue(type, out object obj)) {
				return (SqlQueries<T>) obj;
			}
			ISqlQueries<T> queries = Builder<T>().Queries;
			return (SqlQueries<T>) QueriesCache.GetOrAdd(type, queries);
		}

		/// <summary>
		/// Creates or gets the queries and commands for a given type.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		/// <typeparam name="KeyType">The key type.</typeparam>
		/// <returns>The queries for the given type.</returns>
		public static SqlQueries<T, KeyType> Queries<T, KeyType>() where T : class
		{
			Type type = typeof(T);
			if (KeyQueriesCache.TryGetValue(type, out object obj)) {
				return (SqlQueries<T, KeyType>) obj;
			}
			ISqlQueries<T, KeyType> queries = Builder<T, KeyType>().Queries;
			return (SqlQueries<T, KeyType>) KeyQueriesCache.GetOrAdd(type, queries);
		}

		/// <summary>
		/// Compares two objects of the given type and determines if they are equal.
		/// </summary>
		/// <typeparam name="T">The table type.</typeparam>
		public static IEqualityComparer<T> EqualityComparer<T>() where T : class
		{
			return Builder<T>().EqualityComparer;
		}

		/// <summary>
		/// Clears the cache of queries and builders. This is not recommended unless you run out of memory.
		/// </summary>
		public static void PurgeCache()
		{
			TypeInfoCache.Clear();
			BuilderCache.Clear();
			QueriesCache.Clear();
			KeyQueriesCache.Clear();
		}

		/// <summary>
		/// Clears the cache of queries and builders for the given type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		public static void Purge<T>() where T : class
		{
			Type type = typeof(T);
			TypeInfoCache.TryRemove(type, out object obj);
			BuilderCache.TryRemove(type, out obj);
			QueriesCache.TryRemove(type, out obj);
			KeyQueriesCache.TryRemove(type, out obj);
		}
	}
}
