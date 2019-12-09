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

using System.Collections.Generic;
using System.Data;
namespace Dapper.Extra.Cache
{
	public class DbCacheTransaction : IDbTransaction
	{
		internal DbCacheTransaction(IDbTransaction transaction)
		{
			Transaction = transaction;
		}

		internal readonly List<IDbTransaction> TransactionStorage = new List<IDbTransaction>();

		public DbCacheTransaction Add(params ICacheTable[] tables)
		{
			foreach (ICacheTable table in tables) {
				table.BeginTransaction(this);
			}
			return this;
		}

		internal readonly IDbTransaction Transaction;
		public IDbConnection Connection => Transaction.Connection;
		public IsolationLevel IsolationLevel => Transaction.IsolationLevel;

		public void Commit()
		{
			Transaction.Commit();
			foreach (IDbTransaction storage in TransactionStorage) {
				storage.Commit();
				storage.Dispose();
			}
			TransactionStorage.Clear();
		}

		public void Rollback()
		{
			foreach (IDbTransaction storage in TransactionStorage) {
				storage.Rollback();
			}
			Transaction.Rollback();
		}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue) {
				if (disposing) {
					// TODO: dispose managed state (managed objects).
					Transaction.Dispose();
					Rollback();
				}
				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.
				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~CacheTransaction() {
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
