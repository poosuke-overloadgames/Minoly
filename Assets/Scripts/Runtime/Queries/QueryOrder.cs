namespace Minoly
{
	public class QueryOrder : IQuery
	{
		public QueryOrder(string fieldName, bool isAscend)
		{
			Value = isAscend ? fieldName : $"-{fieldName}";
		}

		public string Key => "order";
		public string Value { get; }
	}
}