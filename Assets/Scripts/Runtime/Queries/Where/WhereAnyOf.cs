using System.Collections.Generic;
using System.Linq;

namespace Minoly
{
	public class WhereAnyOf : IWhereCondition
	{
		public WhereAnyOf(string fieldName, IEnumerable<string> values)
		{
			ToJson = CreateJson(fieldName, values.Select(v => $"\"{v}\""));
		}
		public WhereAnyOf(string fieldName, IEnumerable<int> values)
		{
			ToJson = CreateJson(fieldName, values.Select(v => v.ToString()));
		}
		private string CreateJson(string key, IEnumerable<string> values) 
			=> $"\"{key}\":{{\"$in\":[{string.Join(",", values)}]}}";

		public string ToJson { get; }
	}
}