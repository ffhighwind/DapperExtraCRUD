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
using System.Collections.Generic;

namespace Dapper.Extra.Utilities
{
	public interface IAccessObjectSync<T>
		where T : class
	{
		IEnumerable<T> GetKeys(string whereCondition = "", object param = null, int commandTimeout = 30);
		IEnumerable<KeyType> GetKeys<KeyType>(string whereCondition = "", object param = null, int commandTimeout = 30);

		bool Delete(T obj, int commandTimeout = 30);
		bool Delete(object key, int commandTimeout = 30);
		int Delete(string whereCondition = "", object param = null, int commandTimeout = 30);

		void Insert(T obj, int commandTimeout = 30);
		bool Update(object obj, int commandTimeout = 30);
		bool Update(T obj, int commandTimeout = 30);
		bool Upsert(T obj, int commandTimeout = 30);
		bool InsertIfNotExists(T obj, int commandTimeout = 30);

		T Get(T obj, int commandTimeout = 30);
		T Get(object key, int commandTimeout = 30);
		IEnumerable<T> GetList(string whereCondition = "", object param = null, int commandTimeout = 30);
		IEnumerable<T> GetList(Type columnFilter, string whereCondition = "", object param = null, int commandTimeout = 30);
		IEnumerable<T> GetLimit(int limit, string whereCondition = "", object param = null, int commandTimeout = 30);
		IEnumerable<T> GetLimit(Type columnFilter, int limit, string whereCondition = "", object param = null, int commandTimeout = 30);
		IEnumerable<T> GetDistinct(Type columnFilter, string whereCondition = "", object param = null, int commandTimeout = 30);
		IEnumerable<T> GetDistinctLimit(int limit, string whereCondition = "", object param = null, int commandTimeout = 30);
		IEnumerable<T> GetDistinctLimit(Type columnFilter, int limit, string whereCondition = "", object param = null, int commandTimeout = 30);

		int RecordCount(string whereCondition = "", object param = null, int commandTimeout = 30);

		List<T> BulkGet(IEnumerable<T> keys, int commandTimeout = 30);
		List<T> BulkGet(IEnumerable<object> keys, int commandTimeout = 30);
		void BulkInsert(IEnumerable<T> objs, int commandTimeout = 30);
		int BulkUpdate(IEnumerable<T> objs, int commandTimeout = 30);
		int BulkDelete(IEnumerable<T> objs, int commandTimeout = 30);
		int BulkDelete(IEnumerable<object> keys, int commandTimeout = 30);
		int BulkUpsert(IEnumerable<T> objs, int commandTimeout = 30);
		int BulkInsertIfNotExists(IEnumerable<T> objs, int commandTimeout = 30);
	}
}