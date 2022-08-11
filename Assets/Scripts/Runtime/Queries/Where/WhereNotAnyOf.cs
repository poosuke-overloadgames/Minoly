using System.Collections.Generic;
using System.Linq;

namespace Minoly
{
	public class WhereNotAnyOf : IWhereCondition
	{
		public WhereNotAnyOf(string fieldName, IEnumerable<string> values)
		{
			ToJson = CreateJson(fieldName, values.Select(v => $"\"{v}\""));
		}

		public WhereNotAnyOf(string fieldName, IEnumerable<int> values)
		{
			ToJson = CreateJson(fieldName, values.Select(v => v.ToString()));
		}

		private string CreateJson(string key, IEnumerable<string> values)
			=> $"\"{key}\":{{\"$nin\":[{string.Join(",", values)}]}}";

		public string ToJson { get; }
	}
}