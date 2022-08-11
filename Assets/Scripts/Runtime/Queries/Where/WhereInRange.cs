using System;
using Minoly.ApiTypes;
using UnityEngine;

namespace Minoly
{
	public class WhereInRange : IWhereCondition
	{
		public WhereInRange(string fieldName, string min, string max, bool includeEqualMin = true, bool includeEqualMax = true)
		{
			ToJson = CreateJson(fieldName, $"\"{min}\"", $"\"{max}\"", includeEqualMin, includeEqualMax);
		}
		public WhereInRange(string fieldName, int min, int max, bool includeEqualMin = true, bool includeEqualMax = true)
		{
			ToJson = CreateJson(fieldName, min.ToString(), max.ToString(), includeEqualMin, includeEqualMax);
		}
		public WhereInRange(string fieldName, DateTime min, DateTime max, bool includeEqualMin = true, bool includeEqualMax = true)
		{
			var minDate = new ApiDateTime(min);
			var maxDate = new ApiDateTime(max);
			ToJson = CreateJson(
				fieldName,
				JsonUtility.ToJson(minDate),
				JsonUtility.ToJson(maxDate),
				includeEqualMin,
				includeEqualMax
			);
		}
		private string CreateJson(string key, string min, string max, bool includeEqualMin, bool includeEqualMax)
		{
			var greater = includeEqualMin ? "$gte" : "$gt";
			var less = includeEqualMax ? "$lte" : "$lt";
			return $"\"{key}\":{{\"{greater}\":{min},\"{less}\":{max}}}";
		}

		public string ToJson { get; }

	}
}