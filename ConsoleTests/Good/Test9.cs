using System;
using System.Collections.Generic;
using Dapper.Extra.Annotations;

namespace ConsoleTests
{
	public class Test9 : IDtoKey<Test9, byte[]>
	{
		private static readonly IEqualityComparer<Test9> Comparer = Dapper.Extra.ExtraCrud.EqualityComparer<Test9>();

		public Test9() { }
		public Test9(Random random)
		{
			ID = new byte[random.Next() % 15 + 20];
			random.NextBytes(ID);
			Name = random.Next().ToString();
		}

		[Key(false)]
		public byte[] ID { get; set; }
		public string Name { get; set; }

		public Test9 Clone()
		{
			Test9 clone = (Test9) MemberwiseClone();
			clone.ID = new byte[ID.Length];
			Array.Copy(ID, 0, clone.ID, 0, ID.Length);
			return clone;
		}

		public int CompareTo(Test9 other)
		{
			int cmp = Name.CompareTo(other.Name);
			if (cmp == 0) {
				cmp = ID.Length.CompareTo(other.ID.Length);
				if (cmp == 0) {
					for (int i = 0; i < ID.Length; i++) {
						cmp = ID[i].CompareTo(other.ID[i]);
						if (cmp != 0)
							break;
					}
				}
			}
			return cmp;
		}

		public string CreateTable()
		{
			return @"
CREATE TABLE [dbo].[Test9](
	[ID] [varbinary](35) NOT NULL,
	[Name] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Test9] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]";
		}

		public override int GetHashCode()
		{
			return Comparer.GetHashCode(this);
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as Test9);
		}

		public bool Equals(Test9 other)
		{
			return other != null && Comparer.Equals(this, other);
		}

		public bool Equals(Test9 x, Test9 y)
		{
			return Comparer.Equals(x, y);
		}

		public int GetHashCode(Test9 obj)
		{
			return Comparer.GetHashCode(obj);
		}

		public byte[] GetKey()
		{
			return ID;
		}

		public bool IsIdentical(Test9 other)
		{
			if (Name != other.Name)
				return false;
			if (other.ID != ID) {
				if (other.ID == null || ID == null || other.ID.Length != ID.Length)
					return false;
				for (int i = 0; i < ID.Length; i++) {
					if (ID[i] != other.ID[i])
						return false;
				}
			}
			return true;
		}

		public bool IsInserted(Test9 other)
		{
			return IsIdentical(other);
		}

		public bool IsUpdated(Test9 other)
		{
			return IsIdentical(other);
		}

		public Test9 UpdateRandomize(Random random)
		{
			Test9 clone = Clone();
			clone.Name = random.Next().ToString();
			return clone;
		}
	}
}
