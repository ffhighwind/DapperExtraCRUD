# Description:

A thread-safe Dapper extension that was inspired by Dapper.SimpleCRUD, Dapper-Plus, and more. Unique additions
include Bulk operations, AutoSync, MatchUpdate, MatchDelete, Distinct, 
Top/Limit, Upsert, and Insert If Not Exists. It also exposes most of the underlying metadata to 
allow customization and improved performance.

# Extensions:

All extension methods can be seen [here](https://github.com/ffhighwind/DapperExtraCRUD/blob/master/DapperExtraCRUD/DapperExtraExtensions.cs). Bulk operations are only supported for Microsoft SQL Server.
Update and distinct extensions support custom objects in order to filter the properties.

| Extension | SQL Command |
| --- | --- |
| RecordCount | Select count(*) from TABLE where ... |
| Truncate | Truncate table TABLE |
| Update | Update TABLE T set ...  |
| Insert | Insert into TABLE ... |
| InsertIfNotExists | If not exists (select * from TABLE where ...) insert into TABLE ... |
| Upsert | If not exists (select * from TABLE where ...) insert into TABLE ... else update TABLE set ... |
| Delete | Delete from TABLE where ... |
| DeleteList | Delete from TABLE where ... |
| Get | Select ... from TABLE where ... |
| GetList | Select ... from TABLE where ... |
| GetLimit | Select top(N) from TABLE where ... |
| GetKeys | Select ... from TABLE where ... |
| GetDistinct | Select distinct ... from TABLE where ... |
| GetDistinctLimit | Select distinct top(N) from TABLE where ... |
| BulkUpdate| Update TABLE set ... |
| BulkInsert |Insert into TABLE ... |
| BulkInsertIfNotExists | If not exists (select * from TABLE where ...) insert into TABLE ... |
| BulkUpsert | If not exists (select * from TABLE where ...) insert into TABLE ... else update TABLE set ... |
| BulkDelete | Delete from TABLE where ... |
| BulkGet | Select from TABLE where ... |

# Annotations:

These map properties/classes to a database tables and columns.

| Annotation | Description |
| --- | --- |
| Table | Map a class to a table. |
| Column | Map a property to a column. |
| Key | Map a property to a primary key. |
| NotMapped | Prevent a property from being mapped. |
| AutoSync | Automatically selects a column after an insert/update. This does not affect bulk operations. |
| IgnoreDelete | Prevents deletes. |
| IgnoreSelect | Ignores a column for selects. This does not work for primary keys. |
| IgnoreInsert | Ignores a column for insertions. A raw SQL string can replace the value (e.g. 'getdate()'). This does not work for primary keys. |
| IgnoreUpdate | Ignores a column for updates. A raw SQL string can replace the value. This does not work for primary keys. |
| MatchUpdate | Treats a column as a primary key for updates. A raw SQL string can replace the value. |
| MatchDelete | Treats a column as a primary key for deletes. |

# Annotation Priority:

[NotMapped] > [Key] > ... \
[IgnoreInsert] > [MatchInsert] \
[IgnoreUpdate] > [MatchUpdate]

# Example:

```csharp
[IgnoreDelete]
[Table(name: "Users", declaredOnly: true, inheritAttrs: true)]
public class User
{
	[Key]
	public int UserID { get; set; }

	public string FirstName { get; set; }

	public string LastName { get; set; }

	[Column("Account Name")]
	public decimal UserName { get; set; }

	public UserPermissions Permissions { get; set; }

	[IgnoreInsert(autoSync: true)]
	[MatchUpdate(value: "getdate()", autoSync: true)]
	[MatchDelete]
	public DateTime Modified { get; set; }

	[IgnoreUpdate]
	[IgnoreInsert("getdate()")]
	public DateTime Created { get; set; }
}
```

# Alternative Annotations

Some annotations from System.ComponentModel are supported as replacements for Dapper.Extra.Annotations. 

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
| \[NotMapped] | \[NotMapped] (property) |

# Utilities:

#### AutoAccessObject<T> / DataAccessObject<T>

These include the same the functionality as the extension methods but require fewer parameters per call because they store an SqlConnection or SqlTransaction. 
They also perform a slightly better than the extension methods because they store a reference to the ISqlQueries.

#### WhereConditionGenerator

This generates SQL WHERE conditions from a Linq.Expression<Predicate<T>>. It can be somewhat expensive to generate, so I recommend caching the results when possible.
The main reason to use this utility is if you need a type-safe query or need to map a predicate to SQL command. Specifically, I have used this to remove items 
from a dictionary after deleting rows from a database. It is not well tested, so I do not recommend using it in a production environment.

# Tip:

Use a view if you need joins. If this is not sufficient then you can use Dapper's multi-mapping queries or manually map the results.

# Performance:

The Dapper.DapperExtraExtensions methods perform lookups on a ConcurrentDictionary and a cast the results every time they are called. This is
negligible, but it can prevented by storing the ISqlQueries object and accessing the delegates directly (e.g. AutoAccessObject/DataAccessObject). 
Also, less frequently used delegates such as bulk operations have lazy initialization. There is a small synchronization cost every time 
these are accessed. This can be prevented by storing a reference to each delegate outside of the ISqlQueries object.

# Accessing Metadata:

```csharp
public static void Main(string[] args)
{
	// Changes the default dialect
	ExtraCrud.Dialect = SqlDialect.PostgreSQL;

	// Contains class/property information such as attributes that are used internally
	SqlTypeInfo typeInfo = ExtraCrud.TypeInfo<User>();

	// Contains delegates and other metadata for accessing this table/type
	SqlBuilder<User> builder = ExtraCrud.Builder<User>(); 

	// Access query delegates
	SqlQueries<User> queries = ExtraCrud.Queries<User>();

	// Clear metadata to allow garbage collection. This will increase memory usage if you keep a reference 
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

# Future Plans

* ITypeHandler tests
* Other RDBMS tests
* Bulk operations for other RDBMS.
* Multi-Mapping/Joins
* Paged results

# License:

*MIT License*

Copyright (c) 2018 Wesley Hamilton

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
