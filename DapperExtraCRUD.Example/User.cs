using System;
using Dapper.Extra.Annotations;

namespace DapperExtraCRUD.Example
{
	[IgnoreDelete]
	[Table(name: "Users", declaredOnly: true, inheritAttrs: true)]
	public class User
	{
		[Key]
		public int UserID { get; private set; }

		public string FirstName { get; set; }

		public string LastName { get; set; }

		[Column("Account Name")]
		public string UserName { get; set; }

		public UserPermissions Permissions { get; set; }

		[IgnoreInsert(value: null, autoSync: true)]
		[MatchUpdate(value: "getdate()", autoSync: true)]
		[MatchDelete]
		public DateTime Modified { get; protected set; }

		[IgnoreUpdate]
		[IgnoreInsert("getdate()")]
		public DateTime Created { get; private set; }
	}
}
