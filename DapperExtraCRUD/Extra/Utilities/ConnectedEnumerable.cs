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

using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace Dapper.Extra.Utilities
{
	/// <summary>
	/// Allows an <see cref="IDbConnection"/> to stay open until the enumeration is completely traversed or it is disposed.
	/// This allows unbuffered queries th be returned without being wrapped in a using statement.
	/// However, it should be wrapped in a using statement or try catch block before it is traversed.
	/// </summary>
	/// <typeparam name="T">The type returned by the query.</typeparam>
	public class ConnectedEnumerable<T> : IEnumerable<T>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ConnectedEnumerable{T}"/> class.
		/// </summary>
		/// <param name="list">The list<see cref="IEnumerable{T}"/></param>
		/// <param name="connection">The connection<see cref="IDbConnection"/></param>
		public ConnectedEnumerable(IEnumerable<T> list, IDbConnection connection)
		{
			List = list;
			Connection = connection;
		}

		/// <summary>
		/// The connection that should be closed once <see cref="List"/> 
		/// <see cref="GetEnumerator"/> has been traversed completely.
		/// </summary>
		public IDbConnection Connection { get; private set; }

		/// <summary>
		/// The unbuffered result from the query.
		/// </summary>
		public IEnumerable<T> List { get; private set; }

		/// <summary>
		/// The enumerator for <see cref="List"/>.
		/// </summary>
		public IEnumerator<T> GetEnumerator()
		{
			return new Enumerator(Connection, List.GetEnumerator());
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new Enumerator(Connection, List.GetEnumerator());
		}

		internal class Enumerator : IEnumerator<T>
		{
			private readonly IEnumerator<T> enumerator;

			private IDbConnection conn;

			private bool disposedValue = false;// To detect redundant calls

			/// <summary>
			/// Initializes a new instance of the <see cref="Enumerator"/> class.
			/// </summary>
			/// <param name="conn">The conn<see cref="IDbConnection"/></param>
			/// <param name="enumerator">The enumerator<see cref="IEnumerator{T}"/></param>
			public Enumerator(IDbConnection conn, IEnumerator<T> enumerator)
			{
				this.conn = conn;
				this.enumerator = enumerator;
			}

			public T Current => enumerator.Current;

			object IEnumerator.Current => enumerator.Current;


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
			}

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
				enumerator.Reset();
			}

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
		}
	}
}
