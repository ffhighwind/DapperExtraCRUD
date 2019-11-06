using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace UnitTests
{
	[Table("Test4")]
	public class TestDTO4
	{
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
	}
}
