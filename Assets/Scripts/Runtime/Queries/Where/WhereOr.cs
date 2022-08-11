using System.Collections.Generic;
using System.Linq;

namespace Minoly
{
	public class WhereOr : IWhereCondition
	{
		public WhereOr(IEnumerable<IWhereCondition> conditions)
		{
			ToJson = $"\"$or\":[{string.Join(",", conditions.Select(c => $"{{{c.ToJson}}}"))}]" ;
		}
		public string ToJson { get; }
	}
}