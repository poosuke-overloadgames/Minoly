using System;
using System.Collections.Generic;
using System.Linq;

namespace Minoly
{
	public interface IQuery
	{
		public string Key { get; }
		public string Value { get; }
	}
	
	public static class QueryExtension
	{
		public static string ToEscapedString(this IEnumerable<IQuery> queries) => Uri
			.EscapeUriString(string.Join("&", queries.OrderBy(q => q.Key).Select(q => $"{q.Key}={q.Value}")))
			.Replace(":", "%3A")
			.Replace("[", "%5B")
			.Replace("]", "%5D");
	}
}