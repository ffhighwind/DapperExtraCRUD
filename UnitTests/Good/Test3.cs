using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace UnitTests
{
	public class Test3 : IEquatable<Test3>, IDto<Test3>
	{
		public Test3() { }
		public Test3(Random random)
		{
			Col1 = random.Next();
			Col2 = random.Next().ToString();
			Col3 = (float) random.NextDouble();
			Col4 = random.Next() >= 0 ? random.Next() : (int?) null;
		}
		[Key]
		public int Col1 { get; set; }
		[Key]
		public string Col2 { get; set; }
		[Key]
		public float Col3 { get; set; }
		public int? Col4 { get; set; }

		public static string CreateTable()
		{
			return @"
CREATE TABLE [dbo].[Test3](
	[Col1] [int] NOT NULL,
	[Col2] [nvarchar](50) NOT NULL,
	[Col3] [float] NOT NULL,
	[Col4] [int] NULL,
 CONSTRAINT [PK_Test3] PRIMARY KEY CLUSTERED 
(
	[Col1] ASC,
	[Col2] ASC,
	[Col3] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_Test3] UNIQUE NONCLUSTERED 
(
	[Col1] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]";
		}

		public bool Equals(Test3 other)
		{
			return other.Col1 == Col1
				&& other.Col2 == Col2
				&& other.Col3 == Col3
				&& other.Col4 == Col4;
		}

		public bool IsKeyEqual(Test3 other)
		{
			return other.Col1 == Col1;
		}
	}
}
