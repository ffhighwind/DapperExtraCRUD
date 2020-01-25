using System;
using Dapper.Extra.Annotations;

namespace ConsoleTests
{
	public class TestDTO5 : IDtoKey<TestDTO5, int>
	{
		public TestDTO5() { }

		public TestDTO5(Random random)
		{
			Name = random.Next().ToString();
			Created = DateTime.FromOADate(random.NextDouble());
			Modified = DateTime.FromOADate(random.NextDouble());
			Modified2 = DateTime.FromOADate(random.NextDouble());
		}

		public int ID { get; set; }

		[Column("Full Name")]
		public string Name { get; set; }

		[IgnoreInsert]
		[IgnoreUpdate]
		public DateTime Created { get; set; }

		[MatchUpdate("getdate()", true)]
		[IgnoreInsert("getdate()")]
		public DateTime Modified { get; set; }


		[MatchUpdate(null, true)]
		[IgnoreInsert(null)]
		public DateTime Modified2 { get; set; }

		public TestDTO5 Clone()
		{
			return (TestDTO5)MemberwiseClone();
		}

		public int CompareTo(TestDTO5 other)
		{
			return ID.CompareTo(other.ID);
		}

		public string CreateTable()
		{
			return @"
CREATE TABLE [dbo].[TestDTO5](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Full Name] [varchar](255) NOT NULL,
	[Created] [datetime2](7) NOT NULL,
	[Modified] [datetime] NOT NULL,
	[Modified2] [datetime2](2) NOT NULL,
 CONSTRAINT [PK_TestDTO5] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY];

ALTER TABLE [dbo].[TestDTO5] ADD  CONSTRAINT [DF_TestDTO5_Created]  DEFAULT (getdate()) FOR [Created];

ALTER TABLE [dbo].[TestDTO5] ADD  CONSTRAINT [DF_TestDTO5_Modified]  DEFAULT (getdate()) FOR [Modified];

ALTER TABLE [dbo].[TestDTO5] ADD  CONSTRAINT [DF_TestDTO5_Modified2]  DEFAULT (getdate()) FOR [Modified2];";
		}

		public bool Equals(TestDTO5 other)
		{
			return ID.Equals(other.ID);
		}

		public bool Equals(TestDTO5 x, TestDTO5 y)
		{
			return x.Equals(y);
		}

		public override int GetHashCode()
		{
			return -1213502048 + ID.GetHashCode();
		}

		public int GetHashCode(TestDTO5 obj)
		{
			return obj.GetHashCode();
		}

		public int GetKey()
		{
			return ID;
		}

		public bool IsIdentical(TestDTO5 other)
		{
			bool a1 = ID == other.ID;
			bool a2 = Name == other.Name;
			bool a3 = Created == other.Created;
			//bool a4 = Modified == other.Modified; // auto-syncd
			return a1 && a2 && a3;
		}

		public bool IsInserted(TestDTO5 other)
		{
			return Equals(other) && ID != 0;
		}

		public bool IsUpdated(TestDTO5 other)
		{
			return Equals(other) && Name == other.Name;
		}

		public TestDTO5 UpdateRandomize(Random random)
		{
			TestDTO5 clone = (TestDTO5)MemberwiseClone();
			clone.Name = random.Next().ToString();
			return clone;
		}
	}
}
