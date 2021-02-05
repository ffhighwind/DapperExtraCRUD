using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Dapper.Extra.Utilities;

namespace Dapper.Extra
{
	public class QueryData<T> where T : class
	{
		public QueryData(Expression<Func<T, bool>> expr)
		{
			WhereCondition = "WHERE " + WhereConditionGenerator.Create(expr, out IDictionary<string, object> param);
			Param = param;
			Predicate = expr;
		}

		public Expression<Func<T, bool>> Predicate { get; protected set; }
		public string WhereCondition { get; protected set; }
		public IDictionary<string, object> Param { get; protected set; }
	}
}
