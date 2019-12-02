using Dapper.Extra;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
	[TestClass]
	public class BadUnitTests
	{
		[TestMethod]
		[ExpectedException(typeof(System.InvalidOperationException))]
		public void Bad1()
		{
			ExtraCrud.Builder<Bad1>();
		}

		[TestMethod]
		[ExpectedException(typeof(System.InvalidOperationException))]
		public void Bad2()
		{
			ExtraCrud.Builder<Bad2>();
		}
	}
}
