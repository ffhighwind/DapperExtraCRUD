using System;
using Dapper.Extra.Annotations;

namespace Benchmarks
{
	public enum Permission
	{
		None = 0,
		Basic,
		Admin,
		SuperAdmin,
		Count,
	}

	public enum PayType
	{
		None = 0,
		Contractor,
		Salary,
		Hourly,
		Count,
	}

	[Table("Employees")]
	public class Employee
	{
		[Key]
		public int EmployeeID { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public Permission PermissionID { get; set; }
		public DateTime? Birthday { get; set; }
		public DateTime HireDate { get; set; }
		public string Division { get; set; }
		public string Department { get; set; }
		public string Title { get; set; }
		public int ManagerID { get; set; }
		public string Email { get; set; }
		public string Username { get; set; }
		public decimal Salary { get; set; }
		public string Status { get; set; }
		public string FormerLastName { get; set; }
		public string Nickname { get; set; }
		public double Weight { get; set; }
		public double Height { get; set; }
		public Guid GlobalID { get; set; }
		public DateTime LastLogin { get; set; }
		public int FailedLoginAttempts { get; set; }
		public char Sex { get; set; }
		public PayType PayType { get; set; }
		public decimal PtoHours { get; set; }

		public static Employee Create(Random random)
		{
			string firstName = random.Next().ToString();
			string lastName = random.Next().ToString();
			char sex = random.Next() % 100 > 60 ? 'M' : (random.Next() > 30 ? 'F' : ' ');
			string division = random.Next().ToString();
			Employee emp = new Employee() {
				FirstName = firstName,
				LastName = lastName,
				PermissionID = (Permission)(random.Next() % (int)Permission.Count),
				Birthday = random.Next() % 5 == 0 ? null : (DateTime?)new DateTime(1960, 1, 1).AddDays(random.Next() % (45 * 365)).AddTicks(random.Next() % TimeSpan.TicksPerDay),
				HireDate = DateTime.Today.AddDays(0 - random.Next() % (365 * 15)),
				Division = division,
				Department = random.Next().ToString(),
				Title = random.Next().ToString(),
				ManagerID = 1,
				Email = firstName + "." + lastName + "@" + division + ".com",
				Username = firstName[0] + lastName,
				Salary = (40000 + random.Next() % 210000) + (random.Next() % 100 / 100m),
				Status = random.Next().ToString(),
				Sex = sex,
				FormerLastName = (random.Next() % 100 > 40 && sex == 'F') ? random.Next().ToString() : null,
				Nickname = random.Next() % 100 > 25 ? null : firstName.Substring(0, firstName.Length / 2),
				Weight = random.NextDouble() * 200 + 120,
				Height = random.NextDouble() * 2.7 + 5.3,
				GlobalID = Guid.NewGuid(),
				LastLogin = random.Next() % 100 <= 5 ? new DateTime(1753, 1, 1) : DateTime.Now.AddDays(random.NextDouble() * 8),
				FailedLoginAttempts = random.Next() % 100 > 50 ? random.Next() % 3 : 0,
				PayType = (PayType)(random.Next() % (int)PayType.Count),
				PtoHours = (decimal)random.NextDouble() * 250,
			};
			return emp;
		}

		public static string CreateTableSql()
		{
			return @"
CREATE TABLE [dbo].[Employees](
	[EmployeeID] [int] IDENTITY(1,1) NOT NULL,
	[FirstName] [varchar](100) NOT NULL,
	[LastName] [varchar](100) NOT NULL,
	[FormerLastName] [varchar](150) NULL,
	[PermissionID] [int] NOT NULL,
	[Nickname] [varchar](150) NULL,
	[Birthday] [datetime2](7) NULL,
	[HireDate] [date] NOT NULL,
	[Email] [varchar](280) NOT NULL,
	[Title] [varchar](150) NULL,
	[Division] [varchar](150) NULL,
	[Department] [varchar](150) NULL,
	[ManagerID] [int] NOT NULL,
	[Username] [varchar](280) NOT NULL,
	[Salary] [decimal](18, 2) NOT NULL,
	[Status] [varchar](100) NOT NULL,
	[Weight] [decimal](9, 2) NULL,
	[Height] [decimal](9, 2) NULL,
	[GlobalID] [uniqueidentifier] NOT NULL,
	[LastLogin] [datetime2](7) NULL,
	[FailedLoginAttempts] [int] NOT NULL,
	[Sex] [char](1) NOT NULL,
	[PayType] [int] NOT NULL,
	[PtoHours] [decimal](9, 2) NOT NULL,
 CONSTRAINT [PK_Employees] PRIMARY KEY CLUSTERED 
(
	[EmployeeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]";
		}
	}
}
