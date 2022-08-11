namespace Minoly
{
	public class WhereNotEqualTo : IWhereCondition
	{
		public WhereNotEqualTo(string fieldName, string val)
		{
			ToJson = CreateJson(fieldName, $"\"{val}\"");
		}
		public WhereNotEqualTo(string fieldName, int val)
		{
			ToJson = CreateJson(fieldName, val.ToString());
		}
		public WhereNotEqualTo(string fieldName, bool val)
		{
			ToJson = CreateJson(fieldName, val ? "\"true\"" : "\"false\"");
		}

		private string CreateJson(string key, string val) => $"\"{key}\":{{\"$ne\":{val}}}";

		public string ToJson { get; }
	}
}