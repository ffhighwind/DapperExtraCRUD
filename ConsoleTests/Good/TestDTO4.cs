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
