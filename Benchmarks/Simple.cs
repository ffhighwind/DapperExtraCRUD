using System;
using Dapper.Extra.Annotations;

namespace Benchmarks
{
	public class Simple
	{
		[Key]
		public int ID { get; set; }
		public string Name { get; set; }

		public static Simple Create(Random random)
		{
			Simple simple = new Simple() {
				Name = random.Next().ToString()
			};
			return simple;
		}

		public static string CreateTableSql()
		{
			return @"
CREATE TABLE [dbo].[Simple](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](150) NOT NULL,
 CONSTRAINT [PK_Simple] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]";
		}
	}
}
