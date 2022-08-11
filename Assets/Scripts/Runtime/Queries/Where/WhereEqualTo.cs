
namespace Minoly
{
	public class WhereEqualTo : IWhereCondition
	{
		public WhereEqualTo(string fieldName, string val)
		{
			ToJson = CreateJson(fieldName, val);
		}
		public WhereEqualTo(string fieldName, int val)
		{
			ToJson = CreateJson(fieldName, val.ToString());
		}
		public WhereEqualTo(string fieldName, bool val)
		{
			ToJson = CreateJson(fieldName, val ? "true" : "false");
		}

		private string CreateJson(string key, string val) => $"\"{key}\":\"{val}\"";

		public string ToJson { get; }
	}
}