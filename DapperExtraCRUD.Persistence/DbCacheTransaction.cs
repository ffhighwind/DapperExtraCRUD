using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.Extra.Interfaces;
using Dapper.Extra.Persistence.Interfaces;
using Dapper.Extra.Persistence.Internal;

namespace Dapper.Extra.Persistence
{
	public class DbCacheTransaction : IDbTransaction
	{
		internal DbCacheTransaction(SqlTransaction transaction)
		{
			Transaction = transaction;
		}

		internal readonly List<ITransactionStorage> TransactionStorage = new List<ITransactionStorage>();

		public void Add(params ICacheTable[] tables)
		{
			foreach (ICacheTable table in tables) {
				table.BeginTransaction(this);
			}
		}

		internal readonly SqlTransaction Transaction;
		public IDbConnection Connection => Transaction.Connection;
		public IsolationLevel IsolationLevel => Transaction.IsolationLevel;

		public void Commit()
		{
			Transaction.Commit();
			foreach (ITransactionStorage storage in TransactionStorage) {
				storage.Commit();
				storage.Dispose();
			}
			TransactionStorage.Clear();
		}

		public void Save(string savePointName)
		{
			Transaction.Save(savePointName);
			foreach (ITransactionStorage storage in TransactionStorage) {
				storage.Save(savePointName);
			}
		}

		public void Rollback()
		{
			Transaction.Rollback();
			foreach (ITransactionStorage storage in TransactionStorage) {
				storage.Rollback();
			}
		}

		public void Rollback(string savePointName)
		{
			Transaction.Rollback(savePointName);
			foreach (ITransactionStorage storage in TransactionStorage) {
				storage.Rollback(savePointName);
			}
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
