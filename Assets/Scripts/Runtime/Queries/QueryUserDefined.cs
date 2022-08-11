namespace Minoly
{
	public class QueryUserDefined : IQuery
	{
		public QueryUserDefined(string key, string val)
		{
			Key = key;
			Value = val;
		}

		public string Key { get; }
		public string Value { get; }
	}
}