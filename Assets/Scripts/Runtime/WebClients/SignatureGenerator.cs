using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Minoly
{
	public class SignatureGenerator
	{
		private readonly StringBuilder _builder = new StringBuilder();

		private string GetSource(RequestMethod method, Uri url, string applicationKey, IReadOnlyList<GetQuery> queries, Timestamp timestamp)
		{
			_builder.Clear();
			_builder.AppendFormat("{0}\n", method == RequestMethod.Get ? "GET" : "");
			_builder.Append("mbaas.api.nifcloud.com\n");
			_builder.AppendFormat("{0}\n", url.AbsolutePath);
			_builder.Append("SignatureMethod=HmacSHA256&SignatureVersion=2");
			_builder.AppendFormat("&X-NCMB-Application-Key={0}", applicationKey);
			_builder.AppendFormat("&X-NCMB-Timestamp={0}", timestamp.AsString);
			if (queries.Count == 0) return _builder.ToString();
			var q = string.Join(",", queries.Select(q => $"\"{q.Key}\":\"{q.Value}\""));
			_builder.AppendFormat("&where={0}", Uri.EscapeUriString($"{{{q}}}").Replace(":", "%3A"));
			return _builder.ToString();
		}

		public string Generate(string src, string clientKey)
		{
			var binClientKey = Encoding.UTF8.GetBytes(clientKey);
			var binSrc = Encoding.UTF8.GetBytes(src);
			var hmacSha256 = new HMACSHA256();
			hmacSha256.Key = binClientKey;
			var hash = hmacSha256.ComputeHash(binSrc);
			return Convert.ToBase64String(hash);
		}

		public string Generate(RequestMethod method, Uri url, string applicationKey, string clientKey, IReadOnlyList<GetQuery> queries, Timestamp timestamp)
		{
			var src = GetSource(method, url, applicationKey, queries, timestamp);
			return Generate(src, clientKey);
		}
	}
}