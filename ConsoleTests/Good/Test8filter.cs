using System;

namespace ConsoleTests
{
	public class Test8filter : IFilter<Test8>
	{
		//public long ID { get; set; }
		public string Varchar { get; set; } // [varchar] (25) NOT NULL
											//public long? Quantity { get; set; } // [Quantity] [bigint] NULL,
		public int? Int { get; set; } // [Int] [int] NULL
		public short? Small { get; set; } // [Small] [smallint] NULL
										  //public byte? Tiny { get; set; } // [Tiny] [tinyint] NULL
		public bool? Bit { get; set; } // [Bit] [bit] NULL
		public Guid? Guid { get; set; } // [Guid] [uniqueidentifier] NULL
		public decimal? Money { get; set; } // [Money] [money] NULL
		public float? Real { get; set; } // [Real] [real] NULL
		public char? Char { get; set; } // [Char] [char](1) NULL
		public float? Float { get; set; } // [Float] [float] NULL
		public decimal? Decimal16_3 { get; set; } // [Decimal16_3] [decimal](16, 3) NULL
		public DateTimeOffset? DateTimeOffset { get; set; } // [DateTimeOffset] [datetimeoffset] (7) NULL
															//public DateTime? Date { get; set; } // [Date] [date] NULL
		public string Char12 { get; set; } // [Char12] [char](12) NULL
		public decimal? Numeric13_5 { get; set; } // [Numeric13_5] [numeric] (13, 5) NULL
		public DateTime? DateTime2_7 { get; set; } // [DateTime2_7] [datetime2] (7) NULL
												   //public DateTime? DateTime { get; set; } // [DateTime] [datetime] NULL
		public byte[] Binary35 { get; set; } // [Binary35] [binary] (35) NULL
		public DateTime? SmallDateTime { get; set; } // [SmallDateTime] [smalldatetime] NULL
		//public byte[] VarBinary25 { get; set; }
		public decimal SmallMoney { get; set; }
		public DateTime? Date { get; set; }

		public TimeSpan Time { get; set; }

		public bool IsFiltered(Test8 obj)
		{
			return obj.ID == 0
				&& obj.Quantity == null
				&& obj.Date != null
				&& obj.DateTime == null
				&& obj.Char != null
				&& obj.VarBinary25 == null
				&& obj.Time != default(TimeSpan)
				&& obj.TimeStamp == null;
		}
	}
}
