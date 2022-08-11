using System.Collections.Generic;
using System.Linq;

namespace Minoly
{
	public class WhereAnd : IWhereCondition
	{
		public WhereAnd(IEnumerable<IWhereCondition> conditions)
		{
			ToJson = string.Join(",", conditions.Select(c => c.ToJson));
		}

		public string ToJson { get; }
	}
}