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


namespace Dapper.Extra.Internal
{
	public interface ISqlQueries<T> where T : class
	{
		DbKeyObj<T> GetKey { get; }
		DbKeysList<T> BulkGetKeys { get; }
		SqlKeysInt<T> BulkDeleteKeys { get; }
		DbKeyBool DeleteKey { get; }
		DbWhereKeys GetKeysKeys { get; }

		SqlListList<T> BulkGet { get; }
		SqlListInt<T> BulkDelete { get; }
		SqlListVoid<T> BulkInsert { get; }
		SqlListInt<T> BulkUpdate { get; }
		SqlListInt<T> BulkUpsert { get; }
		SqlListInt<T> BulkInsertIfNotExists { get; }
		DbTBool<T> Delete { get; }
		DbWhereInt<T> DeleteList { get; }
		DbVoid DeleteAll { get; }
		DbTT<T> Get { get; }
		DbWhereList<T> GetKeys { get; }
		DbWhereList<T> GetList { get; }
		DbTypeWhereList<T> GetFilter { get; }
		DbTypeWhereList<T> GetDistinct { get; }
		DbLimitList<T> GetLimit { get; }
		DbTypeLimitList<T> GetFilterLimit { get; }
		DbTypeLimitList<T> GetDistinctLimit { get; }
		DbTVoid<T> Insert { get; }
		DbWhereInt<T> RecordCount { get; }
		DbTBool<T> Update { get; }
		DbObjBool<T> UpdateObj { get; }
		DbTBool<T> Upsert { get; }
		DbTBool<T> InsertIfNotExists { get; }
		DbTVoid<T> InsertAutoSync { get; }
		DbTVoid<T> UpdateAutoSync { get; }

		//public override int RemoveDuplicates(IDbConnection connection, IDbTransaction transaction, int commandTimeout = 30)
	}
}
