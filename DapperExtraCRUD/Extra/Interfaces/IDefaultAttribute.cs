using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Extra.Interfaces
{
	/// <summary>
	/// Base Attribute for MatchUpdate, IgnoreInsert, and IgnoreUpdate
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class IDefaultAttribute : Attribute
	{
		protected IDefaultAttribute() { }

		protected IDefaultAttribute(string value)
		{
			Value = string.IsNullOrWhiteSpace(value) ? null : "(" + value.Trim() + ")";
		}

		public string Value { get; protected set; }
		public bool HasValue => Value != null;

		public override string ToString()
		{
			return Value ?? "";
		}
	}
}
