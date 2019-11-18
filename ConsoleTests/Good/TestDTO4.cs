using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Dapper.Extra.Annotations;

namespace UnitTests
{
	[Table("Test4")]
	public class TestDTO4 : IDtoKey<TestDTO4, int>
	{
		public TestDTO4() { }
		public TestDTO4(Random random)
		{
			ID = random.Next();
			FirstName = random.Next().ToString();
			LastName = random.Next().ToString();
		}

		[Key]
		public int ID { get; set; }

		[MatchDelete]
		public string FirstName { get; set; }
		[MatchUpdate]
		public string LastName { get; set; }

		public string CreateTable()
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

		public int CompareTo(TestDTO4 other)
		{
			return ID.CompareTo(other.ID);
		}

		public bool Equals(TestDTO4 other)
		{
			return other.ID == ID;
		}

		public bool Equals(TestDTO4 x, TestDTO4 y)
		{
			return x.Equals(y);
		}

		public int GetHashCode(TestDTO4 obj)
		{
			return obj.GetHashCode();
		}

		public override int GetHashCode()
		{
			return 1213502048 + ID.GetHashCode();
		}

		public int GetKey()
		{
			return ID;
		}

		public bool IsIdentical(TestDTO4 other)
		{
			return other.ID == ID
				&& other.FirstName == FirstName
				&& other.LastName == LastName;
		}

		public bool IsInserted(TestDTO4 other)
		{
			return Equals(other) && ID != 0;
		}


		public bool IsUpdated(TestDTO4 other)
		{
			return Equals(other) && FirstName == other.FirstName;
		}

		public TestDTO4 UpdateRandomize(Random random)
		{
			TestDTO4 clone = (TestDTO4) MemberwiseClone();
			clone.FirstName = random.Next().ToString();
			return clone;
		}
	}
}
