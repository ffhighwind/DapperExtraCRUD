using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Extra.Internal
{
	public enum TimeInterval
	{
		/// <summary>
		/// MySQL only
		/// </summary>
		MICROSECOND,
		MILLISECOND,
		SECOND,
		MINUTE,
		HOUR,
		DAY,
		WEEK,
		MONTH,
		QUARTER,
		YEAR,
	}
}
