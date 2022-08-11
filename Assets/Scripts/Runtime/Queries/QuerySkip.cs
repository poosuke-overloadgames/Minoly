namespace Minoly
{
	public class QuerySkip : IQuery
	{
		public QuerySkip(int count)
		{
			Value = count.ToString();
		}

		public string Key => "skip";
		public string Value { get; }
	}
}