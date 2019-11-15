using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace UnitTests
{
	[Table("Test4")]
	public class TestDTO4 : IEquatable<TestDTO4>, IDto<TestDTO4>, IEqualityComparer<TestDTO4>
	{
		public TestDTO4() { }
		public TestDTO4(Random random)
		{
			//ID = random.Next();
			FirstName = random.Next().ToString();
			LastName = random.Next().ToString();
		}

		[Key]
		public int ID { get; set; }

		[MatchDelete]
		public string FirstName { get; set; }
		[MatchUpdate]
		public string LastName { get; set; }

		public static string CreateTable()
		{
			return @"
CREATE TABLE [dbo].[Test4](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[FirstName] [varchar](max) NOT NULL,
	[LastName] [varchar](max) NOT NULL,
 CONSTRAINT [PK_Test4] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";
		}

		public bool Equals(TestDTO4 other)
		{
			return other.ID == ID
				&& other.FirstName == FirstName
				&& other.LastName == LastName;
		}

		public bool Equals(TestDTO4 x, TestDTO4 y)
		{
			return x.Equals(y);
		}

		public int GetHashCode(TestDTO4 obj)
		{
			return obj.GetHashCode();
		}

		public bool IsInserted()
		{
			return ID != 0;
		}

		public bool IsKeyEqual(TestDTO4 other)
		{
			return other.ID == ID;
		}
	}
}
