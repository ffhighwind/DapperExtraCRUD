# Introduction

[Nuget: Dapper.ExtraCRUD](https://www.nuget.org/packages/Dapper.ExtraCRUD/)
<img  align="right" src="https://raw.githubusercontent.com/ffhighwind/DapperExtraCRUD/master/Images/DapperExtraCRUD-200x200.png" alt="ExtraCRUD">

A thread-safe Dapper extension that was inspired by Dapper.SimpleCRUD, Dapper-Plus, and more. Unique additions include Bulk operations, AutoSync, MatchUpdate, MatchDelete, Distinct, Top/Limit, Upsert, and Insert If Not Exists. It also automatically sets auto-increment keys on insert, supports Enum/string based primary keys, and exposes most of the underlying metadata to allow customization and improved performance.

## Extensions

Bulk operations are only supported for Microsoft SQL Server. Update and distinct extensions support custom objects in order to filter the properties.

| Extension | SQL Command |
| --- | --- |
| RecordCount | Select count(*) from [TABLE] where ... |
| Truncate | Truncate table [TABLE] |
| Update | Update [TABLE] set ...  |
| Insert | Insert into [TABLE] ... |
| InsertIfNotExists | If not exists (select * from [TABLE] where ...) insert into [TABLE] ... |
| Upsert | If not exists (select * from [TABLE] where ...) insert into TABLE ... else update [TABLE] set ... |
| Delete | Delete from [TABLE] where ... |
| DeleteList | Delete from [TABLE] where ... |
| Get | Select ... from [TABLE] where ... |
| GetList | Select ... from [TABLE] where ... |
| GetLimit | Select top(N) from [TABLE] where ... |
| GetKeys | Select ... from [TABLE] where ... |
| GetDistinct | Select distinct ... from [TABLE] where ... |
| GetDistinctLimit | Select distinct top(N) from [TABLE] where ... |
| BulkUpdate| Update [TABLE] set ... |
| BulkInsert |Insert into [TABLE] ... |
| BulkInsertIfNotExists | If not exists (select * from [TABLE] where ...) insert into [TABLE] ... |
| BulkUpsert | If not exists (select * from [TABLE] where ...) insert into TABLE ... else update [TABLE] set ... |
| BulkDelete | Delete from [TABLE] where ... |
| BulkGet | Select from [TABLE] where ... |

## Annotations

Annotations map classes to a database tables and properties to table columns.

| Annotation | Description |
| --- | --- |
| Table | Map a class to a table. |
| Column | Map a property to a column. |
| NotMapped | Prevent a property from being mapped. |
| Key | Map a property to a primary key. |
| AutoSync | Automatically selects a column/class after an insert/update. Does not occur in bulk operations. Synchronization is a delayed action, which means that the object may contain inaccurate data without a transaction. |
| IgnoreDelete | Prevents deletes. |
| IgnoreSelect | Ignores a column for selects. This does not work for primary keys. |
| IgnoreInsert | Ignores a column for insertions. A raw SQL string can replace the value. |
| IgnoreUpdate | Ignores a column for updates. A raw SQL string can replace the value. |
| MatchUpdate | Treats a column as a primary key for updates. A raw SQL string can replace the value. |
| MatchDelete | Treats a column as a primary key for deletes. |

#### Priority

NotMapped > Key > ... \
IgnoreSelect > AutoSync \
IgnoreInsert > MatchInsert \
IgnoreUpdate > MatchUpdate

#### Alternatives

Some annotations from System.ComponentModel are supported as replacements for Dapper.Extra.Annotations. 

| System.ComponentModel | Dapper.Extra.Annotations |
| --- | --- |
| \[Table] | \[Table] |
| \[Required] | \[Key(false)] |
| \[Key] | \[Key(true)] |
| \[Column] | \[Column]
| \[Editable(false)] | \[IgnoreInsert][IgnoreUpdate] |
| \[ReadOnly(true)] (property) | \[IgnoreInsert]\[IgnoreUpdate] |
| \[ReadOnly(true)] (class) | \[IgnoreInsert]\[IgnoreUpdate]\[IgnoreDelete] |
| public int Property { set; private get; } | \[IgnoreInsert]\[IgnoreUpdate]\[IgnoreDelete] |
| \[NotMapped] | \[NotMapped] |

## Example

This example shows how to define a "Users" table and perform some operations on it. You may note that
the syntax is similar to other Dapper extensions.

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
	public string UserName { get; set; }

	public UserPermissions Permissions { get; set; }

	[IgnoreInsert(value: null, autoSync: true)]
	[MatchUpdate(value: "getdate()", autoSync: true)]
	[MatchDelete]
	public DateTime Modified { get; set; }

	[IgnoreUpdate]
	[IgnoreInsert("getdate()")]
	public DateTime Created { get; set; }
}

public static class Program {
	private const string ConnString = "Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=myPassword;";
	
	public static void Main()
	{
		using (SqlConnection conn = new SqlConnection(ConnString)) {
			DateTime minHireDate = DateTime.Today.AddDays(-30);
			IEnumerable<User> users = conn.GetList<User>("where HireDate >= @minHireDate ", new { minHireDate });
			User user = new User() {
				FirstName = "Jason",
				LastName = "Borne",
				UserName = "jborne",
				Permissions = UserPermissions.Admin,
			};
			conn.Insert(user);
		}
	}
}
```
## Utilities

#### AutoAccessObject<T> / DataAccessObject<T>

These include the same the functionality as the extension methods but require fewer parameters per call because they store an SqlConnection or SqlTransaction. 
They also perform a slightly better than the extension methods because they store a reference to the ISqlQueries.

#### WhereConditionGenerator

This generates SQL WHERE conditions from a Linq.Expression<Predicate<T>>. It can be somewhat expensive to generate, so I recommend caching the results when possible.

```csharp
string condition = WhereConditionGenerator.Create<User>((u) => 
	u.UserName == "jborne"
	&& (u.FirstName != null || u.Permissions == UserPermissions.Basic) 
	&& new string[] { "Jason", "Chris", "Zack" }.Contains(u.FirstName),
	out IDictionary<string, object> param);
/*	(((Users.[Account Name] = 'jborne') 
	AND ((Users.FirstName is not NULL) OR (Users.Permissions = 1))) 
	AND Users.FirstName in @P0)"
 param = { "P0" => List<object>() { "Jason", "Chris", "Zack" } }
*/
IEnumerable<User> result = conn.Query<User>("SELECT * FROM Test WHERE " + condition, param);
```

#### ExtraUtil

This static class contains a few helper functions such as IsQuotedSqlType, IsSqlIdentifier, SqlValue, etc.

## Accessing Metadata

```csharp
public static void Main(string[] args)
{
	// Changes the default dialect
	ExtraCrud.Dialect = SqlDialect.PostgreSQL;

	// Contains class/property information such as attributes that are used internally
	SqlTypeInfo typeInfo = ExtraCrud.TypeInfo<User>();

	// Contains delegates and other metadata for accessing this table/type
	SqlBuilder<User> builder = ExtraCrud.Builder<User>(); 

	// Contains delegates for SQL commands
	SqlQueries<User> queries = ExtraCrud.Queries<User>();

	// Clear metadata to allow garbage collection. This will increase memory usage if you keep a reference 
	// to purged metadata. 
	ExtraCrud.Purge<User>();
	ExtraCrud.Purge();

	// Compares User objects for equality based on the primary keys
	IEqualityComparer<User> comparer = ExtraCrud.EqualityComparer<User>();
	Dictionary<User, User> map = new Dictionary<User, User>(comparer);
}
```

## Tips

* MatchUpdate and MatchDelete on DateTime only work on datetime2 for up to 2 decimal places (e.g. datetime2(2)). This is because of differences 
in precision for datetime2 vs C# DateTime.
* Use a view if you need joins. Alternatively you can use Dapper's multi-mapping queries or manually map the results.

## Performance

The extension methods in DapperExtraExtensions perform lookups on a ConcurrentDictionary and a cast the results every time they are called. This is
negligible, but it can prevented by storing the ISqlQueries object and accessing the delegates directly (e.g. AutoAccessObject/DataAccessObject). 
Also, less frequently used delegates such as bulk operations have lazy initialization. There is a small synchronization cost every time 
these are accessed. This can be prevented by storing a reference to each delegate outside of the ISqlQueries object.

## Customization

Changing the SqlAdapter using ExtraCrud.SetAdapter<T>() will change the default behavior the methods. 
This is mainly used to support multiple RDBMS syntaxes such as MySQL and PostgreSQL. However, it could also be used for other purposes. 
For example, you could use [BulkOptionsAdapter](https://github.com/ffhighwind/DapperExtraCRUD/blob/master/DapperExtraCRUD.Example/BulkOptionsAdapter.cs)
in order to prevent bulk insert from firing triggers or checking constraints. Note that this will *only* work for bulk insert as other bulk operations
use a temporary table. You can also use [CollateAdapter](https://github.com/ffhighwind/DapperExtraCRUD/blob/master/DapperExtraCRUD.Example/CollateAdapter.cs)
to change the case sensitivity for primary key comparisons.

## Future Plans

* ITypeHandler tests
* Other RDBMS tests
* Bulk operations for other RDBMS.
* Multi-Mapping/Joins
* Paged results

# License

*MIT License*

Copyright (c) 2018 Wesley Hamilton

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
