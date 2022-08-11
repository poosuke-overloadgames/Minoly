using System;
using Minoly.ApiTypes;
using UnityEngine;

namespace Minoly
{
	public class WhereLessThan : IWhereCondition
	{
		public WhereLessThan(string fieldName, string val, bool includeEqual = true)
		{
			ToJson = CreateJson(fieldName, $"\"{val}\"", includeEqual);
		}
		public WhereLessThan(string fieldName, int val, bool includeEqual = true)
		{
			ToJson = CreateJson(fieldName, val.ToString(), includeEqual);
		}
		public WhereLessThan(string fieldName, DateTime val, bool includeEqual = true)
		{
			var d = new ApiDateTime(val);
			ToJson = CreateJson(fieldName, JsonUtility.ToJson(d), includeEqual);
		}
		private string CreateJson(string key, string val, bool includeEqual) 
			=> $"\"{key}\":{{\"${(includeEqual ? "lte" : "lt")}\":{val}}}";

		public string ToJson { get; }
	}
}