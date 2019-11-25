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
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Extra.Utilities
{
	public interface IAccessObjectAsync<T>
		where T : class
	{
		Task<IEnumerable<T>> GetKeysAsync(string whereCondition = "", object param = null, int commandTimeout = 30);
		Task<IEnumerable<KeyType>> GetKeysAsync<KeyType>(string whereCondition = "", object param = null, int commandTimeout = 30);

		Task<bool> DeleteAsync(T obj, int commandTimeout = 30);
		Task<bool> DeleteAsync<KeyType>(KeyType key, int commandTimeout = 30);
		Task<int> DeleteAsync(string whereCondition = "", object param = null, int commandTimeout = 30);

		Task InsertAsync(T obj, int commandTimeout = 30);
		Task<bool> UpdateAsync(object obj, int commandTimeout = 30);
		Task<bool> UpdateAsync(T obj, int commandTimeout = 30);
		Task<bool> UpsertAsync(T obj, int commandTimeout = 30);
		Task<bool> InsertIfNotExistsAsync(T obj, int commandTimeout = 30);

		Task<T> GetAsync(T obj, int commandTimeout = 30);
		Task<T> GetAsync<KeyType>(KeyType key, int commandTimeout = 30);
		Task<IEnumerable<T>> GetListAsync(string whereCondition = "", object param = null, int commandTimeout = 30);
		Task<IEnumerable<T>> GetListAsync(Type columnFilter, string whereCondition = "", object param = null, int commandTimeout = 30);
		Task<IEnumerable<T>> GetLimitAsync(int limit, string whereCondition = "", object param = null, int commandTimeout = 30);
		Task<IEnumerable<T>> GetLimitAsync(Type columnFilter, int limit, string whereCondition = "", object param = null, int commandTimeout = 30);
		Task<IEnumerable<T>> GetDistinctAsync(string whereCondition = "", object param = null, int commandTimeout = 30);
		Task<IEnumerable<T>> GetDistinctAsync(Type columnFilter, string whereCondition = "", object param = null, int commandTimeout = 30);
		Task<IEnumerable<T>> GetDistinctLimitAsync(int limit, string whereCondition = "", object param = null, int commandTimeout = 30);
		Task<IEnumerable<T>> GetDistinctLimitAsync(Type columnFilter, int limit, string whereCondition = "", object param = null, int commandTimeout = 30);

		Task<int> RecordCountAsync(string whereCondition = "", object param = null, int commandTimeout = 30);

		Task<List<T>> BulkGetAsync(IEnumerable<T> keys, int commandTimeout = 30);
		Task<List<T>> BulkGetAsync<KeyType>(IEnumerable<KeyType> keys, int commandTimeout = 30);
		Task BulkInsertAsync(IEnumerable<T> objs, int commandTimeout = 30);
		Task<int> BulkUpdateAsync(IEnumerable<T> objs, int commandTimeout = 30);
		Task<int> BulkDeleteAsync(IEnumerable<T> objs, int commandTimeout = 30);
		Task<int> BulkDeleteAsync<KeyType>(IEnumerable<KeyType> keys, int commandTimeout = 30);
		Task<int> BulkUpsertAsync(IEnumerable<T> objs, int commandTimeout = 30);
		Task<int> BulkInsertIfNotExistsAsync(IEnumerable<T> objs, int commandTimeout = 30);
	}
}
