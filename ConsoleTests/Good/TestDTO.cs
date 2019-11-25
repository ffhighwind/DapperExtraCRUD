#region License
// Released under MIT License 
// License: https://www.mit.edu/~amini/LICENSE.md
// Home page: https://github.com/ffhighwind/DapperExtraCRUD

// Copyright(c) 2018 Wesley Hamilton

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
#endregion

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
		[IgnoreInsert("getdate()", false)]
		[IgnoreUpdate("getdate()", false)]
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
