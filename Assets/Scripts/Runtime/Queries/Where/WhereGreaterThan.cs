using System;
using Minoly.ApiTypes;
using UnityEngine;

namespace Minoly
{
	public class WhereGreaterThan : IWhereCondition
	{
		public WhereGreaterThan(string fieldName, string val)
		{
			ToJson = CreateJson(fieldName, $"\"{val}\"");
		}
		public WhereGreaterThan(string fieldName, int val)
		{
			ToJson = CreateJson(fieldName, val.ToString());
		}
		public WhereGreaterThan(string fieldName, DateTime val)
		{
			var d = new ApiDateTime(val);
			ToJson = CreateJson(fieldName, JsonUtility.ToJson(d));
		}
		private string CreateJson(string key, string val) => $"\"{key}\":{{\"$gte\":{val}}}";

		public string ToJson { get; }
	}
}