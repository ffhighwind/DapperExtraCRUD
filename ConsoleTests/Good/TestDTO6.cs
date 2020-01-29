using System;
using System.Collections.Generic;
using Dapper.Extra.Annotations;

namespace ConsoleTests
{
	public class TestDTO6 : IDtoKey<TestDTO6, string>
	{
		private static readonly IEqualityComparer<TestDTO6> Comparer = Dapper.Extra.ExtraCrud.EqualityComparer<TestDTO6>();

		private static readonly string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";


		private static string RandomString(int length, Random random)
		{
			char[] ch = new char[length];
			for(int i = 0; i < length; i++) {
				ch[i] = chars[random.Next() % chars.Length];
			}
			return new string(ch);
		}

		public TestDTO6() { }

		public TestDTO6(Random random)
		{
			ID = RandomString(8 + random.Next() % 15, random);
			Value = random.Next();
		}

		[Key]
		public string ID { get; set; }

		public int Value { get; set; }

		public TestDTO6 Clone()
		{
			return (TestDTO6) MemberwiseClone();
		}

		public int CompareTo(TestDTO6 other)
		{
			return ID.CompareTo(other.ID);
		}

		public string CreateTable()
		{
			return @"
CREATE TABLE [dbo].[TestDTO6](
	[ID] [varchar](24) NOT NULL,
	[Value] [int] NOT NULL,
 CONSTRAINT [PK_TestDTO6] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]";
		}

		public override bool Equals(object other)
		{
			return Equals(other as TestDTO6);
		}

		public bool Equals(TestDTO6 other)
		{
			return Comparer.Equals(this, other);
		}

		public bool Equals(TestDTO6 x, TestDTO6 y)
		{
			return Comparer.Equals(x, y);
		}

		public int GetHashCode(TestDTO6 obj)
		{
			return Comparer.GetHashCode(obj);
		}

		public override int GetHashCode()
		{
			return Comparer.GetHashCode(this);
		}

		public string GetKey()
		{
			return ID;
		}

		public bool IsIdentical(TestDTO6 other)
		{
			return Equals(other) && Value == other.Value;
		}

		public bool IsInserted(TestDTO6 other)
		{
			return IsIdentical(other);
		}

		public bool IsUpdated(TestDTO6 other)
		{
			return IsIdentical(other);
		}

		public TestDTO6 UpdateRandomize(Random random)
		{
			TestDTO6 value = (TestDTO6)MemberwiseClone();
			ID = random.Next() % 2 == 0 ? ID.ToLower() : ID.ToUpper();
			value.Value = random.Next();
			return value;
		}
	}
}
