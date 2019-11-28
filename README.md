# Description:

A thread-safe Dapper extension that includes the functionality of Dapper-Plus, Dapper.SimpleCRUD, and more. 
Unique additions include new attributes (Versioning keys: MatchUpdate, MatchDelete), AutoSync, Distinct, 
Top/Limit, Upsert, Insert If Not Exists, and Bulk operations. It also exposes most of the underlying
metadata to allow customization and improved performance.

# Example:

#### Note: 
This example is not a recommendation on how to design a database table. It is usually better 
to use triggers, default values, and stored procedures when faced with certain constraints.

```sql
CREATE TABLE [dbo].[Users](
	[UserID] [int] NOT NULL,
	[FirstName] [varchar](100) NOT NULL,
	[LastName] [varchar](100) NOT NULL,
	[Account Name] [varchar](255) NOT NULL,
	[Permissions] [tinyint] NOT NULL,
	[Modified] [datetime2](7) NOT NULL,
	[Created] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, 
	ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY];

ALTER TABLE [dbo].[Users] ADD CONSTRAINT [DF_Users_Created]  DEFAULT (getdate()) FOR [Created];

ALTER TABLE [dbo].[Users] ADD CONSTRAINT [DF_Users_Modified]  DEFAULT (getdate()) FOR [Modified];
```

```csharp

// Represents and RDBMS syntax used for generating queries.
public enum SqlSyntax
{
	SQLServer,
	PostgreSQL,
	MySQL,
	SQLite,
}

public enum UserPermissions
{
	None = 0,
	Basic = 1,
	Admin = 2,
	SuperAdmin = 3
};

[IgnoreDelete] // prevents deletes
[Table("Users", declaredOnly: true, inheritAttrs: true, syntax: SqlSyntax.SQLServer)]
public class User
{
	// Primary Key
	[Key(autoIncrement: false)]
	public int UserID { get; set; }

	public string FirstName { get; set; }
	public string LastName { get; set; }
	
	// UserName maps to the column 'Account Name'
	[Column("Account Name")]
	public decimal UserName { get; set; }

	public UserPermissions Permissions { get; set; }

	[IgnoreInsert(autoSync: true)] // Defaults to getdate()
	[MatchUpdate(value: "getdate()", autoSync: true)]
	[MatchDelete]
	public DateTime Modified { get; set; }

	[IgnoreUpdate]
	[IgnoreInsert("getdate()")]
	public DateTime Created { get; set; }

	private List<User> _Friends { get; set; } = new List<User>();
	public IReadOnlyList<User> Friends => Friends;
	public void AddFriend(User friend)
	{
		_Friends.Add(friend);
	}
	public int NotUsed { get; }
	private int BestFriendID { get; set; }
	public decimal Points;
	[NotMapped]
	public bool IsDirty { get; set; }
}
```

* User objects represent rows in the [Users] table. 
* [IgnoreDelete] means nothing will occur when delete methods are invoked.
* [Key(autoIncrement: false)] means the property is a primary key of the table. It is also not an identity (not auto-incrementing) key.
* [IgnoreInsert(autoSync: true)] means that the property will receive the default value 'getdate()' on inserts. Autosync means that the property 
will be automatically updated using a select after an insert.
* [MatchUpdate(value: "getdate()", autoSync: true)] means that the property acts as a versioning key for updates. This means that updates will 
fail unless the property's value matches what is in the database. After a successful update the value will be set to 'getdate()'.
* [MatchDelete] means that the property acts as a pseudo-key for deletes. The row will only be deleted if the property's value matches 
what is in the database.
* [NotMapped] means that the property should be ignored completely. This should be used on properties that do not represent a column
in the table.
* The following columns are ignored for various reasons: _Friends, Friends, NotUsed, BestFriendID, Points, and IsDirty. 
A valid property must have a public set method and its type must be a standard SQL type, enum, or implement Dapper.SqlMapper.ITypeHandler.
* A [Key] property without a get method is equivalent to adding \[IgnoreInsert]\[IgnoreUpdate]\[IgnoreDelete] to the class.

# Alternative Annotations

| System.ComponentModel | Dapper.Extra.Annotations |
| --- | --- |
| \[Table("name")] | \[Table("name")] |
| \[Required] | \[Key(false)] |
| \[Key] | \[Key(true)] |
| \[Column("name")] | \[Column("name")]
| \[Editable(false)] | \[IgnoreInsert][IgnoreUpdate] |
| \[ReadOnly(true)] (property) | \[IgnoreInsert]\[IgnoreUpdate] |
| \[ReadOnly(true)] (class) | \[IgnoreInsert]\[IgnoreUpdate]\[IgnoreDelete] |
| public int Property { set; private get; } | \[IgnoreInsert]\[IgnoreUpdate]\[IgnoreDelete] |
| \[NotMapped] | \[NotMapped] |
| - | \[IgnoreInsert] (class) |
| - | \[IgnoreUpdate] (class) |
| - | \[IgnoreDelete] (class) |
| - | \[MatchInsert] |
| - | \[MatchUpdate] |

# Annotation Priority:

[NotMapped] > [Required] > [Key] > ... \
[IgnoreInsert] > [MatchInsert] \
[IgnoreUpdate] > [MatchUpdate]

# Accessing Metadata:

```csharp
public static void Main(string[] args)
{
	// Changes the default syntax
	ExtraCrud.Syntax = SqlSyntax.PostgreSQL;

	// Contains class/property information such as attributes that are used internally
	SqlTypeInfo typeInfo = ExtraCrud.TypeInfo<User>();

	// Contains delegates and other metadata for accessing this table/type
	SqlBuilder<User> builder = ExtraCrud.Builder<User>(); 

	// Access query delegates
	SqlQueries<User> queries = ExtraCrud.Queries<User>();
	SqlQueries<User, int> keyQueries = ExtraCrud.Queries<User, int>();

	// Clear metadata to allow garbage collection. This will increase memory usage if keep a reference 
	// to purged metadata. I recommend only doing this if are having problems with OutOfMemoryExceptions.
	ExtraCrud.Purge<User>();
	ExtraCrud.Purge();

	using (SqlConnection conn = new SqlConnection(ConnString)) {
		conn.Open();

		// Get all users created within the last month
		DateTime minDate = DateTime.Today.AddDays(-30);
		List<User> users = queries.GetList("WHERE Created >= @minDate", new { minDate });

		User johnDoe = new User()
		{
			UserID = 1,
			FirstName = "John",
			LastName = "Doe",
			Permissions = UserPermissions.Basic,
		};

		using (SqlTransaction trans = conn.BeginTransaction()) {
			if(!conn.InsertIfNotExists(user, trans)) {
				Console.WriteLine("User already exists!"); // based on the UserID
			}
			else
				trans.Commit();
		}

		johnDoe = conn.Get(johnDoe.UserID, trans);

		// Compares <User> for equality based on the keys
		IEqualityComparer<User> comparer = ExtraCrud.EqualityComparer<User>();

		Dictionary<User, User> map = new Dictionary<User, User>(comparer);
		foreach (User user in users) {
			if(!comparer.Equals(user, johnDoe)) {
				// Prevent self friendship
				user.AddFriend(johnDoe);
			}
			map.Add(user, user);
		}
	}
}
```

# Utilities:

#### AutoAccessObject<T> / DataAccessObject<T>

These include the same the functionality as the extension methods but require fewer parameters per call because they store an SqlConnection or SqlTransaction. 
They also perform a slightly better than the extension methods because they store a reference to the ISqlQueries.

#### WhereConditionGenerator

This generates SQL WHERE conditions from a Linq.Expression<Predicate<T>>. It can be somewhat expensive to generate, so I recommend caching the results when possible.
The main reason to use this utility is if you need a type-safe query or need to map a predicate to SQL command. Specifically, I have used this to remove items 
from a dictionary after deleting rows from a database. It is not well tested, so I do not recommend using it in a production environment.

#### Partition<T>(this IEnumerable<T> source, int size)

This extension is used internally by the KeyType queries in order to get around Dapper's limitation of 2100 parameters. You should not need to use this with any DapperExtraCRUD methods, but you may 
need it for Dapper queries.

# Tip:

Use a view if you need joins. If this is not sufficient then you can use Dapper's multi-mapping queries or manually map the results.

# Performance:

The Dapper.DapperExtraExtensions methods perform lookups on a ConcurrentDictionary and a cast the results every time they are called. You can prevent this 
by storing the ISqlQueries object and accessing the delegates directly (e.g. AutoAccessObject/DataAccessObject). Also, 
less frequently used delegates such as bulk operations have lazy initialization. There is a small synchronization cost every time 
these are accessed. This can be prevented by storing a reference to each delegate outside of the ISqlQueries object.

# Future Plans

* ITypeHandler tests
* Other RDBMS tests (SqlSyntax)
* Thread-safe bulk operations. These are thread safe for SQL Server, but not others.
* Multi-Mapping/Joins
* Paged results

# License:

*MIT License*

Copyright (c) 2018 Wesley Hamilton

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
