using System;
using System.Collections.Generic;
using System.Linq;
using Dapper.Extra.Annotations;

namespace ConsoleTests
{
	public class Test8 : IDtoKey<Test8, long>
	{
		private static readonly IEqualityComparer<Test8> Comparer = Dapper.Extra.ExtraCrud.EqualityComparer<Test8>();
		private static readonly string Chars = "$%#@!*abcdefghijklmnopqrstuvwxyz1234567890?;:ABCDEFGHIJKLMNOPQRSTUVWXYZ^&";
		private static readonly int Odds = 4;

		private static readonly long MinSmallDateTime = new DateTime(1900, 1, 1).Ticks;
		private static readonly long MaxSmallDateTime = new DateTime(2079, 6, 6).Ticks - 1;
		private static readonly decimal MaxMoney = 922337203685477.5807m;
		private static readonly decimal MinMoney = -922337203685477.5808m;
		private static readonly decimal MaxSmallMoney = 214748.3647m;
		private static readonly decimal MinSmallMoney = -214748.3648m;

		public Test8() { }
		public Test8(Random random)
		{
			ID = random.Next();
			Varchar = random.Next().ToString();
			Quantity = random.Next();
			Int = random.Next() % Odds == 0 ? null : (int?)random.Next();
			Small = random.Next() % Odds == 0 ? null : (short?)random.Next();
			Tiny = random.Next() % Odds == 0 ? null : (byte?)random.Next();
			Bit = random.Next() % Odds == 0 ? null : (bool?)(random.Next() % 2 == 0);
			Guid = random.Next() % Odds == 0 ? null : (Guid?)System.Guid.NewGuid();
			Money = random.Next() % Odds == 0 ? null : (decimal?)decimal.Truncate(NextDecimal(random, MinMoney * 10000, MaxMoney * 10000)) / 10000m;
			if (Money != null) {
				if (Money.Value > MaxMoney)
					throw new InvalidOperationException();
				else if (Money.Value < MinMoney)
					throw new InvalidOperationException();
			}
			Real = random.Next() % Odds == 0 ? null : (float?)random.NextDouble();
			Char = Chars[random.Next() % Chars.Length];
			Float = random.Next() % Odds == 0 ? null : (float?)random.NextDouble();
			Decimal16_3 = random.Next() % Odds == 0 ? null : (decimal?)decimal.Truncate(NextDecimal(random, -9999999999999999m, 9999999999999999m)) / 1000m;
			DateTimeOffset = new DateTimeOffset(System.DateTime.FromOADate(random.NextDouble()));
			DateTimeOffset.Value.ToOffset(new TimeSpan((random.Next() % 23) - 11, 0, 0));
			if (random.Next() % Odds == 0)
				DateTimeOffset = null;
			Date = (DateTime?)System.DateTime.FromOADate(random.NextDouble()).Date;
			Char12 = random.Next() % Odds == 0 ? null : random.Next(100000, 999999).ToString() + random.Next(100000, 999999).ToString();
			Numeric13_5 = random.Next() % Odds == 0 ? null : (decimal?)decimal.Truncate(NextDecimal(random, -9999999999999m, 9999999999999m)) / 100000m;
			DateTime2_7 = random.Next() % Odds == 0 ? null : (DateTime?)System.DateTime.FromOADate(random.NextDouble());
			DateTime = random.Next() % Odds == 0 ? null : (DateTime?)System.DateTime.FromOADate(random.NextDouble());
			Binary35 = new byte[35];
			random.NextBytes(Binary35);
			if (random.Next() % Odds == 0)
				Binary35 = null;
			SmallDateTime = random.Next() % Odds == 0 ? null : (DateTime?)new System.DateTime(NextLong(random, MinSmallDateTime, MaxSmallDateTime)).Date;
			SmallMoney = random.Next() % Odds == 0 ? null : (decimal?)decimal.Truncate(NextDecimal(random, MinSmallMoney * 10000, MaxSmallMoney * 10000)) / 10000m;
			if (SmallMoney != null) {
				if (SmallMoney.Value > MaxSmallMoney)
					throw new InvalidOperationException();
				else if (SmallMoney.Value < MinSmallMoney)
					throw new InvalidOperationException();
			}
			VarBinary25 = new byte[random.Next() % 20 + 5];
			random.NextBytes(VarBinary25);
			if (random.Next() % Odds == 0)
				VarBinary25 = null;
			Time = new DateTime(random.Next()).TimeOfDay;
		}

		private static decimal NextDecimal(Random random, decimal min, decimal max)
		{
			return min + (decimal)(random.NextDouble() * (double)(max - min));
			// 29 decimal places
		}

		private static long NextLong(Random random)
		{
			long tmp = ((long)random.Next()) << 32;
			return tmp | (long)random.Next();
		}

		private static long NextLong(Random random, long min, long max)
		{
			long result = random.Next((int)(min >> 32), (int)(max >> 32));
			result <<= 32;
			int minI = (int)min;
			int maxI = (int)max;
			if (minI < maxI) {
				result |= (long)random.Next(minI, maxI);
			}
			return result;
		}

		[Key(false)]
		public long ID { get; set; }
		public string Varchar { get; set; } // [varchar] (25) NOT NULL
		public long? Quantity { get; set; } // [Quantity] [bigint] NULL,
		public int? Int { get; set; } // [Int] [int] NULL
		public short? Small { get; set; } // [Small] [smallint] NULL
		public byte? Tiny { get; set; } // [Tiny] [tinyint] NULL
		public bool? Bit { get; set; } // [Bit] [bit] NULL
		public Guid? Guid { get; set; } // [Guid] [uniqueidentifier] NULL
		public decimal? Money { get; set; } // [Money] [money] NULL
		public float? Real { get; set; } // [Real] [real] NULL
		public char? Char { get; set; } // [Char] [char](1) NULL
		public float? Float { get; set; } // [Float] [float] NULL
		public decimal? Decimal16_3 { get; set; } // [Decimal16_3] [decimal](16, 3) NULL
		public DateTimeOffset? DateTimeOffset { get; set; } // [DateTimeOffset] [datetimeoffset] (7) NULL
		public DateTime? Date { get; set; } // [Date] [date] NULL
		public string Char12 { get; set; } // [Char12] [char](12) NULL
		public decimal? Numeric13_5 { get; set; } // [Numeric13_5] [numeric] (13, 5) NULL
		[AutoSync(true, true)]
		public DateTime? DateTime2_7 { get; set; } // [DateTime2_7] [datetime2] (7) NULL
		[AutoSync]
		public DateTime? DateTime { get; set; } // [DateTime] [datetime] NULL
		public byte[] Binary35 { get; set; } // [Binary35] [binary] (35) NULL
		public DateTime? SmallDateTime { get; set; } // [SmallDateTime] [smalldatetime] NULL
		public decimal? SmallMoney { get; set; } // [SmallMoney] [smallmoney] NULL,
		public byte[] VarBinary25 { get; set; } // [VarBinary25] [varbinary](25) NULL,
		public TimeSpan Time { get; set; }// [Time] [time] (7) NOT NULL,
		[IgnoreInsert(null, true)]
		[IgnoreUpdate]
		public byte[] TimeStamp { get; set; } // [TimeStamp] [timestamp] NOT NULL,

		public string CreateTable()
		{
			return @"
CREATE TABLE [dbo].[Test8](
	[ID] [bigint] NOT NULL,
	[Varchar] [varchar](25) NOT NULL,
	[Quantity] [bigint] NULL,
	[Int] [int] NULL,
	[Small] [smallint] NULL,
	[Tiny] [tinyint] NULL,
	[Bit] [bit] NULL,
	[Guid] [uniqueidentifier] NULL,
	[Money] [money] NULL,
	[Real] [real] NULL,
	[Char] [char](1) NULL,
	[Float] [float] NULL,
	[Decimal16_3] [decimal](16, 3) NULL,
	[DateTimeOffset] [datetimeoffset](7) NULL,
	[Date] [date] NULL,
	[Char12] [char](12) NULL,
	[Numeric13_5] [numeric](13, 5) NULL,
	[DateTime2_7] [datetime2](7) NULL,
	[DateTime] [datetime] NULL,
	[Binary35] [binary](35) NULL,
	[SmallDateTime] [smalldatetime] NULL,
	[SmallMoney] [smallmoney] NULL,
	[VarBinary25] [varbinary](25) NULL,
	[Time] [time](7) NOT NULL,
	[TimeStamp] [timestamp] NOT NULL,
 CONSTRAINT [PK_Test8] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]";
		}

		public Test8 Clone()
		{
			return (Test8)MemberwiseClone();
		}

		public int CompareTo(Test8 other)
		{
			return ID.CompareTo(other.ID);
		}

		public bool Equals(Test8 other)
		{
			return Comparer.Equals(this, other);
		}

		public bool Equals(Test8 x, Test8 y)
		{
			return Comparer.Equals(x, y);
		}

		public int GetHashCode(Test8 obj)
		{
			return Comparer.GetHashCode(obj);
		}

		public long GetKey()
		{
			return ID;
		}

		public bool IsIdentical(Test8 other)
		{
			if (!IsUpdated(other))
				return false;
			if (!Enumerable.SequenceEqual(TimeStamp, other.TimeStamp))
				return false;
			return true;
		}

		public override bool Equals(object other)
		{
			return Equals(other as Test8);
		}

		public override int GetHashCode()
		{
			return Comparer.GetHashCode(this);
		}

		public bool IsInserted(Test8 other)
		{
			return IsIdentical(other);
		}

		public bool IsUpdated(Test8 other)
		{
			if (ID != other.ID)
				return false;
			if (!StringComparer.OrdinalIgnoreCase.Equals(Varchar, other.Varchar))
				return false;
			if (Quantity != other.Quantity)
				return false;
			if (Int != other.Int)
				return false;
			if (Small != other.Small)
				return false;
			if (Tiny != other.Tiny)
				return false;
			if (Bit != other.Bit)
				return false;
			if (Guid != other.Guid)
				return false;
			if (Money != other.Money)
				return false;
			if (Real != other.Real)
				return false;
			if (Char != other.Char)
				return false;
			if (Float != other.Float)
				return false;
			if (Decimal16_3 != other.Decimal16_3)
				return false;
			if (DateTimeOffset != other.DateTimeOffset)
				return false;
			if (Date != other.Date)
				return false;
			if (Char12 != other.Char12)
				return false;
			if (Numeric13_5 != other.Numeric13_5)
				return false;
			if (DateTime2_7 != other.DateTime2_7 && (DateTime2_7.Value.AddTicks(-100000) > other.DateTime2_7 || DateTime2_7.Value.AddTicks(100000) < other.DateTime2_7))
				return false;
			if (DateTime != other.DateTime && (DateTime.Value.AddTicks(-100000) > other.DateTime || DateTime.Value.AddTicks(100000) < other.DateTime))
				return false;
			if (Binary35 != other.Binary35) {
				if (Binary35 == null || other.Binary35 == null)
					return false;
				if (!Enumerable.SequenceEqual(Binary35, other.Binary35))
					return false;
			}
			if (SmallDateTime != other.SmallDateTime)
				return false;
			if (VarBinary25 != other.VarBinary25) {
				if (VarBinary25 == null || other.VarBinary25 == null)
					return false;
				if (!Enumerable.SequenceEqual(VarBinary25, other.VarBinary25))
					return false;
			}
			if (SmallMoney != other.SmallMoney)
				return false;
			if (Time != other.Time)
				return false;
			return true;
		}

		public Test8 UpdateRandomize(Random random)
		{
			Test8 clone = new Test8(random);
			clone.ID = ID;
			clone.TimeStamp = TimeStamp;
			return clone;
		}
	}
}
