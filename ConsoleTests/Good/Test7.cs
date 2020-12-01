using System;
using System.Collections.Generic;
using Dapper.Extra.Annotations;

namespace ConsoleTests
{
	public enum Test7Type
	{
		ID0,
		ID1,
		ID2,
		ID3,
		ID4,
		ID5,
		ID6,
		ID7,
		ID8,
		ID9,
		ID10,
		ID11,
		ID12,
		ID13,
		ID14,
		ID15,
		ID16,
		ID17,
		ID18,
		ID19,
	}

	public class Test7 : IDtoKey<Test7, Test7Type>
	{
		private static readonly IEqualityComparer<Test7> Comparer = Dapper.Extra.ExtraCrud.EqualityComparer<Test7>();

		public Test7() { }

		public Test7(Random random) 
		{
			ID = (Test7Type)random.Next();
			Value = (random.Next() % 100) > 35 ? (Test7Type?) (random.Next() % (int) Test7Type.ID19) : null;
		}

		[Key(false)]
		public Test7Type ID { get; set; }

		public Test7Type? Value { get; set; }

		public bool? Default { get; set; }

		public Test7 Clone()
		{
			return (Test7) MemberwiseClone();
		}

		public int CompareTo(Test7 other)
		{
			return ID.CompareTo(other.ID);
		}

		public string CreateTable()
		{
			return @"
CREATE TABLE [dbo].[Test7](
	[ID] [int] NOT NULL,
	[Value] [int] NULL,
	[Default] [bit] NULL,
 CONSTRAINT [PK_Test7] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]";
		}

		public override bool Equals(object other)
		{
			return Equals(other as Test7);
		}

		public bool Equals(Test7 other)
		{
			return Comparer.Equals(this, other);
		}

		public bool Equals(Test7 x, Test7 y)
		{
			return Comparer.Equals(x, y);
		}

		public int GetHashCode(Test7 obj)
		{
			return Comparer.GetHashCode(obj);
		}

		public override int GetHashCode()
		{
			return Comparer.GetHashCode(this);
		}

		public Test7Type GetKey()
		{
			return ID;
		}

		public bool IsIdentical(Test7 other)
		{
			return Equals(other);
		}

		public bool IsInserted(Test7 other)
		{
			return Equals(other);
		}

		public bool IsUpdated(Test7 other)
		{
			return Equals(other);
		}

		public Test7 UpdateRandomize(Random random)
		{
			return Clone();
		}
	}
}
