using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
	[Table("Test")]
	public class TestDTO
	{
		[Key]
		public int ID { get; set; }

		[Column("FirstName")]
		public string Name { get; set; }
		[IgnoreInsert("getdate()")]
		[IgnoreUpdate("getdate()")]
		public DateTime? CreatedDt { get; set; }

		public TestDTO Test { get; set; }

		public static string CreateTable()
		{
			return @"
CREATE TABLE [dbo].[Test](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[FirstName] [varchar](MAX) NOT NULL,
	[CreatedDt] [datetime2](7) NULL,
 CONSTRAINT [PK_Test_1] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]";
		}
	}
}
