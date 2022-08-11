namespace Minoly
{
	public class QueryWhere : IQuery
	{
		public QueryWhere(string conditionInJson)
		{
			Value = $"{{{conditionInJson}}}";
		}

		public string Key => "where";
		public string Value { get; }

		public static QueryWhere Create(IWhereCondition condition) => new QueryWhere(condition.ToJson);
	}
}