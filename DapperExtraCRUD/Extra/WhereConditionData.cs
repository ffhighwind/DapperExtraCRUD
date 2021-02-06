using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Dapper.Extra.Utilities;

namespace Dapper.Extra
{
	/// <summary>
	/// Stores the result of an expression using <see cref="WhereConditionGenerator"/>.
	/// </summary>
	/// <typeparam name="T">The table type.</typeparam>
	public class WhereConditionData<T> where T : class
	{
		/// <summary>
		/// Compiles an expression using <see cref="WhereConditionGenerator"/> and stores the result.
		/// </summary>
		/// <param name="predicate">The predicate to use for the query.</param>
		public WhereConditionData(Expression<Func<T, bool>> predicate)
		{
			WhereCondition = "WHERE " + WhereConditionGenerator.Create(predicate, out IDictionary<string, object> param);
			Param = param;
			Predicate = predicate;
		}

		/// <summary>
		/// The predicate expression that was created.
		/// </summary>
		public Expression<Func<T, bool>> Predicate { get; protected set; }

		/// <summary>
		/// The WHERE expression in SQL that the predicate represents.
		/// </summary>
		public string WhereCondition { get; protected set; }

		/// <summary>
		/// The Dapper parameters for the WHERE condition.
		/// </summary>
		public IDictionary<string, object> Param { get; protected set; }
	}
}
