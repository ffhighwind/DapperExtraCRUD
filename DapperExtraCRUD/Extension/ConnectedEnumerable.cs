using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Extension
{
	internal class ConnectedEnumerable<T> : IEnumerable<T>
	{
		public IEnumerable<T> List { get; private set; }
		public IDbConnection Conn { get; private set; }

		public ConnectedEnumerable(IEnumerable<T> list, IDbConnection connection)
		{
			this.List = list;
			Conn = connection;
		}

		public IEnumerator<T> GetEnumerator()
		{
			return new Enumerator(Conn, List.GetEnumerator());
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new Enumerator(Conn, List.GetEnumerator());
		}

		internal class Enumerator : IEnumerator<T>
		{
			private IDbConnection conn;
			private IEnumerator<T> enumerator;

			public Enumerator(IDbConnection conn, IEnumerator<T> enumerator)
			{
				this.conn = conn;
				this.enumerator = enumerator;
			}

			public T Current => enumerator.Current;

			object IEnumerator.Current => enumerator.Current;

			public bool MoveNext()
			{
				if (!enumerator.MoveNext()) {
					conn.Close();
					return false;
				}
				return true;
			}

			public void Reset()
			{
				throw new InvalidOperationException();
			}

			#region IDisposable Support
			private bool disposedValue = false; // To detect redundant calls

			protected virtual void Dispose(bool disposing)
			{
				if (!disposedValue) {
					if (disposing) {
						// TODO: dispose managed state (managed objects).
					}
					conn.Dispose();
					conn = null;
					disposedValue = true;
				}
			}

			// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
			// ~Enumerator() {
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
}
