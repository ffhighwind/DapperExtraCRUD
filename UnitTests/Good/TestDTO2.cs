using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace UnitTests
{
	[Table("Test2")]
	public class TestDTO2 : IEquatable<TestDTO2>, IDto<TestDTO2>
	{
		public TestDTO2() { }
		public TestDTO2(Random random)
		{
			Col1 = random.Next();
			Col2 = random.Next().ToString();
			Col3 = (float) random.NextDouble();
		}

		[Key(false)]
		public int Col1 { get; set; }
		[Key(false)]
		public string Col2 { get; set; }
		[Key(false)]
		public float Col3 { get; set; }

		public override bool Equals(object obj)
		{
			return Equals(obj as TestDTO2);
		}

		public bool Equals(TestDTO2 other)
		{
			return other != null &&
				   Col1 == other.Col1 &&
				   Col2 == other.Col2 &&
				   Col3 == other.Col3;
		}

		public override int GetHashCode()
		{
			int hashCode = -1473066521;
			hashCode = hashCode * -1521134295 + Col1.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Col2);
			hashCode = hashCode * -1521134295 + Col3.GetHashCode();
			return hashCode;
		}

		public static string CreateTable()
		{
			return @"
CREATE TABLE [dbo].[Test2](
	[Col1] [int] NOT NULL,
	[Col2] [nvarchar](50) NOT NULL,
	[Col3] [float] NOT NULL,
 CONSTRAINT [PK_Test2] PRIMARY KEY CLUSTERED 
(
	[Col1] ASC,
	[Col2] ASC,
	[Col3] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]";
		}

		public bool IsKeyEqual(TestDTO2 other)
		{
			return Equals(other);
		}
	}
}
