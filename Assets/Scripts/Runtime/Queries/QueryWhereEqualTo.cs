namespace Minoly
{
	public class QueryWhereEqualTo : IQuery
	{
		public QueryWhereEqualTo(string key, string val)
		{
			Value = $"{{\"{key}\":\"{val}\"}}";
		}

		public string Key => "where";
		public string Value { get; }
	}
}