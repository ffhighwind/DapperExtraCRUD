# Description:

A thread-safe Dapper extension that includes the functionality of Dapper-Plus, Dapper.SimpleCRUD, and more. 
Unique additions include new attributes (Versioning keys: MatchUpdate, MatchDelete), AutoSync, Distinct, 
Top/Limit, Upsert, Insert If Not Exists, and Bulk operations. It also exposes most of the underlying
metadata to allow customization and improved performance.

# TODO

* AutoSync, ITypeHandler tests, SqlSyntax or other RDBMS tests

# Future Plans:

* Getters/Setters, MultiMap, Joins

# Installation:

This project requires [Visual Studio](https://visualstudio.microsoft.com/) to compile.
Once installed to install the required NuGet packages. Right click on the project in the
Solution Explorer and select "Manage NuGet Packages...". From here Visual Studio will
prompt you to install the missing packages.

# NuGet Packages:

* [Dapper](https://github.com/StackExchange/Dapper)
   For querying SQL Databases in a type-safe way.
* [FastMember](https://github.com/mgravell/fast-member)
   For the DataReader used in Bulk operations. This may be replaced by a custom IL generator (or Fasterflect) in the future.

# Example:

### Note: 
The example is not a recommendation on how to implement a table. It is often better 
to use triggers, default values, and stored procedures when faced with certain database
constraints.

```
public enum UserPermissions
{
	None = 0,
	Basic = 1,
	Admin = 2,
	SuperAdmin = 3
};

[IgnoreDelete]
[Table("Users", declaredOnly: true, inheritAttrs: true, syntax: SqlSyntax.SQLServer)]
public class User
{
	[Key(autoIncrement: false)]
	public int UserID { get; set; }

	public string FirstName { get; set; }
	public string LastName { get; set; }
	[Column("Account Name")]
	public decimal UserName { get; set; }

	public UserPermissions Permissions { get; set; }

	[IgnoreInsert(autoSync: true] // Defaults to getdate()
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

The above example describes the following table.
```
CREATE TABLE [dbo].[Users](
	\[UserID] [int] NOT NULL,
	\[FirstName] \[varchar](100) NOT NULL,
	\[LastName] \[varchar](100) NOT NULL,
	\[Account Name] \[varchar](255) NOT NULL,
	\[Permissions] [tinyint] NOT NULL,
	\[Modified] \[datetime2](7) NOT NULL,
	\[Created] \[datetime2](7) NOT NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY];
```
* The User class maps to the table [Users]. This table does not allow deletes. If a delete method is invoked then nothing will occur.
* The UserID property maps to the primary key of the table. This column is not an identity (not auto-incrementing), so it will not be 
ignored during inserts.
* The Modified property is treated as a versioning key for updates/deletes. This row will receive the default value getdate() on inserts. 
Autosync ensures that the property is automatically updated to match the database on insert/update. Updates and deletes will fail unless 
this column matches what is in the database.
* The UserName property maps to the column [Account Name] in the database.
* The Created property cannot be updated and will receive getdate() on insert instead of the property's value.
* The rest of the columns are ignored for various reasons: _Friends, Friends, NotUsed, BestFriendID, Points, and IsDirty. 
Only properties with public get/set methods are allowed. These properties are only accepted if they are standard SQL types, enums, 
or classes that implement Dapper.SqlMapper.ITypeHandler.

# Accessing Metadata:

using Dapper.Extra;

public static Program {
public static void Main(string[] args)
{
	// change the default syntax
	ExtraCrud.Syntax = SqlSyntax.PostgreSQL; // change the default syntax

	// contains class/property information such as attributes that are used internally
	SqlTypeInfo typeInfo = ExtraCrud.TypeInfo<User>();

	// contains delegates and other metadata for accessing this table/type
	SqlBuilder<User> builder = ExtraCrud.Builder<User>(); 

	// Access query delegates
	SqlQueries<User> queries = ExtraCrud.Queries<User>();
	SqlQueries<User, int> keyQueries = ExtraCrud.Queries<User, int>();

	// Clear metadata to allow garbage collection. This will increase memory usage if keep a reference to purged metadata.
	// I recommend only doing this if are having problems with OutOfMemoryExceptions.
	ExtraCrud.Purge<User>();
	ExtraCrud.Purge();

	using (SqlConnection conn = new SqlConnection(ConnString)) {
		conn.Open();
		// Get all users created within the last month
		List<User> users = queries.GetList("WHERE Created >= @minDate, new { minDate = DateTime.Today.AddDays(-30) });

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

		// IEqualityComparer which can be used by Dictionary<User, User>
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
}

# Utilities:

Some utilities are located in Dapper.Extra.Utilities and Dapper.Extra.Internal.Extensions.

#### AutoAccessObject and DataAccessObject 

These include all of the same functionality as the extension methods without needing to
pass an SqlConnection or SqlTransaction every method call. They also store the queries and therefore perform a little better than
the extension methods.

#### WhereConditionGenerator

This can will generate WHERE conditions from a Linq.Expression<Predicate<T>>. This can be somewhat expensive, so it is recommended that you cache the results when possible.
This is not extremely well tested, so do not use it in a production environment. It can be useful if you want a type-safe query or you have a cache of items and
you need some way to determine which ones match a condition. Specifically, I have used this to remove items that I have deleted from a cache.

# Tips:

If you need joins in your queries then you can create a view. If this is not sufficient then you can use 
Dapper's multi-mapping queries or manually map the results.

# Maximizing Performance:

All metadata is stored in concurrent dictionaries. This was to follow Dapper's design and allow the user the ability to purge metadata.
The extension methods in Dapper.DapperExtraExtensions perform a lookup and a cast every time they are called. You can prevent this 
by using the AutoAccessObject/DataAccessObject utilities or storing the results of Dapper.Extra.ExtraCrud.Queries<T>() and accessing the delegates directly. 
Less-commonly used delegates such as bulk operations have lazy initialization. These delegates will only be created the first time they are accessed and have a 
very small performance hit every time they are accessed. In order to prevent synchronized access to queries you will need to store a reference
to each delegate.

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