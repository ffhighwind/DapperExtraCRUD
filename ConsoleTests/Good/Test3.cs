#region License
// Released under MIT License 
// License: https://opensource.org/licenses/MIT
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
using Dapper.Extra.Annotations;

namespace ConsoleTests
{
	public class Test3 : IDto<Test3>
	{
		public Test3() { }
		public Test3(Random random)
		{
			Col1 = random.Next();
			Col2 = random.Next().ToString();
			Col3 = (float)random.NextDouble();
			Col4 = random.Next() >= 0 ? random.Next() : (int?)null;
		}
		[Key]
		public int Col1 { get; set; }
		[Key]
		public string Col2 { get; set; }
		[Key]
		public float Col3 { get; set; }
		public int? Col4 { get; set; }

		public string CreateTable()
		{
			return @"
CREATE TABLE [dbo].[Test3](
	[Col1] [int] NOT NULL,
	[Col2] [nvarchar](50) NOT NULL,
	[Col3] [float] NOT NULL,
	[Col4] [int] NULL,
 CONSTRAINT [PK_Test3] PRIMARY KEY CLUSTERED 
(
	[Col1] ASC,
	[Col2] ASC,
	[Col3] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_Test3] UNIQUE NONCLUSTERED 
(
	[Col1] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]";
		}

		public bool Equals(Test3 other)
		{
			return other.Col1 == Col1
				&& other.Col2 == Col2
				&& other.Col3 == Col3;
		}

		public bool Equals(Test3 x, Test3 y)
		{
			return x.Equals(y);
		}

		public int GetHashCode(Test3 obj)
		{
			return obj.GetHashCode();
		}

		public int CompareTo(Test3 other)
		{
			int ret = Col1.CompareTo(other.Col1);
			if (ret == 0) {
				ret = Col2.CompareTo(other.Col2);
				if (ret == 0) {
					ret = Col3.CompareTo(other.Col3);
				}
			}
			return ret;
		}

		public bool IsInserted(Test3 other)
		{
			return Equals(other);
		}

		public bool IsIdentical(Test3 other)
		{
			return other.Col1 == Col1
				&& other.Col2 == Col2
				&& other.Col3 == Col3
				&& other.Col4 == Col4;
		}

		public override int GetHashCode()
		{
			int hashCode = -1473066521;
			hashCode = hashCode * -1521134295 + Col1.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Col2);
			hashCode = hashCode * -1521134295 + Col3.GetHashCode();
			return hashCode;
		}

		public Test3 UpdateRandomize(Random random)
		{
			Test3 clone = (Test3)MemberwiseClone();
			clone.Col4 = random.Next();
			return clone;
		}

		public bool IsUpdated(Test3 other)
		{
			return Equals(other) && Col4 == other.Col4;
		}

		public Test3 Clone()
		{
			return (Test3) MemberwiseClone();
		}
	}
}
