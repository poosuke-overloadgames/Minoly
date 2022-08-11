using System;
using Minoly.ApiTypes;
using UnityEngine;

namespace Minoly
{
	public class WhereGreaterThan : IWhereCondition
	{
		public WhereGreaterThan(string fieldName, string val, bool includeEqual = true)
		{
			ToJson = CreateJson(fieldName, $"\"{val}\"", includeEqual);
		}
		public WhereGreaterThan(string fieldName, int val, bool includeEqual = true)
		{
			ToJson = CreateJson(fieldName, val.ToString(), includeEqual);
		}
		public WhereGreaterThan(string fieldName, DateTime val, bool includeEqual = true)
		{
			var d = new ApiDateTime(val);
			ToJson = CreateJson(fieldName, JsonUtility.ToJson(d), includeEqual);
		}
		private string CreateJson(string key, string val, bool includeEqual) 
			=> $"\"{key}\":{{\"${(includeEqual ? "gte" : "gt")}\":{val}}}";

		public string ToJson { get; }
	}
}