using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Dapper.Extra.Annotations;

namespace UnitTests
{
	[Table("Test")]
	public class TestDTO : IDtoKey<TestDTO, int>
	{
		public TestDTO() { }
		public TestDTO(Random random)
		{
			ID = random.Next();
			Name = random.Next().ToString();
			CreatedDt = DateTime.FromOADate(random.NextDouble());
		}

		[Key]
		public int ID { get; set; }

		[Column("FirstName")]
		public string Name { get; set; }
		[IgnoreInsert("getdate()")]
		[IgnoreUpdate("getdate()")]
		public DateTime? CreatedDt { get; set; }

		public TestDTO Test { get; set; }

		public string CreateTable()
		{
			return @"
CREATE TABLE [dbo].[Test](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[FirstName] [varchar](max) NOT NULL,
	[CreatedDt] [datetime2](7) NULL,
 CONSTRAINT [PK_Test_1] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";
		}

		public int CompareTo(TestDTO other)
		{
			return ID.CompareTo(other.ID);
		}

		public bool Equals(TestDTO other)
		{
			return other.ID == ID;
		}

		public bool Equals(TestDTO x, TestDTO y)
		{
			return x.Equals(y);
		}

		public int GetHashCode(TestDTO obj)
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

		public bool IsIdentical(TestDTO other)
		{
			return other.ID == ID
				&& other.Name == Name
				&& other.CreatedDt == CreatedDt;
		}

		public bool IsInserted(TestDTO other)
		{
			return ID == other.ID 
				&& Name == other.Name 
				&& CreatedDt != other.CreatedDt
				&& ID != 0;
		}

		public TestDTO UpdateRandomize(Random random)
		{
			TestDTO clone = (TestDTO) MemberwiseClone();
			clone.Name = random.Next().ToString();
			return clone;
		}

		public bool IsUpdated(TestDTO other)
		{
			return Equals(other) && Name == other.Name;
		}
	}
}
