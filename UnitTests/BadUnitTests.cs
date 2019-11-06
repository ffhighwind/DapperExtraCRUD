
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Dapper.Extra;
using Dapper.Extra.Utilities;
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
			TableFactory.Create<Bad1>();
		}

		[TestMethod]
		[ExpectedException(typeof(System.InvalidOperationException))]
		public void Bad2()
		{
			TableFactory.Create<Bad2>();
		}
	}
}
