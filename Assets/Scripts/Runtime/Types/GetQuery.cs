namespace Minoly.Types
{
	public readonly struct GetQuery
	{
		public GetQuery(string key, string val)
		{
			Key = key;
			Value = val;
		}

		public string Key { get; }
		public string Value { get; }
	}
}