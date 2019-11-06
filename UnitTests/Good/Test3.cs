using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
	public class Test3
	{
		public int Col1 { get; set; }
		public string Col2 { get; set; }
		public float Col3 { get; set; }
		public int? Col4 { get; set; }

		public static string CreateTable()
		{
			return @"
CREATE TABLE [dbo].[Test3](
	[Col1] [int] NOT NULL,
	[Col2] [nvarchar](MAX) NOT NULL,
	[Col3] [float] NOT NULL,
	[Col4] [int] NULL
) ON [PRIMARY]";
		}
	}
}
