using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Extension.Interfaces
{
	/// <summary>
	/// Base Attribute for MatchUpdate, IgnoreInsert, and IgnoreUpdate
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class IDefaultAttribute : Attribute
	{
		public IDefaultAttribute(Func<string> function)
		{
			if (function != null) {
				Value = () =>
				{
					string value = function();
					return "(" + value + ")";
				};
			}
		}

		public IDefaultAttribute(string value = null)
		{
			value = value?.Trim();
			if (value != null && value.Length != 0) {
				value = "(" + value + ")";
				Value = () => value;
				IsConstant = true;
			}
		}

		private static readonly Func<string> NullValue = () => null;
		public readonly Func<string> Value = NullValue;
		public bool HasValue => Value != NullValue;
		public bool IsConstant { get; private set; }

		public override string ToString()
		{
			return Value();
		}
	}
}
