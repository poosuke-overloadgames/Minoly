namespace Minoly
{
	public class QueryLimit : IQuery
	{
		public QueryLimit(int count)
		{
			Value = count.ToString();
		}

		public string Key => "limit";
		public string Value { get; }
	}
}