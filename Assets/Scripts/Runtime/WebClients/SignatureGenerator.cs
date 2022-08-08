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

		
		
		private string GetSource(RequestMethod method, Uri url, string applicationKey, Timestamp timestamp, IEnumerable<IQuery> queries = null)
		{
			_builder.Clear();
			_builder.AppendFormat("{0}\n", method.ToHttpString());
			_builder.Append("mbaas.api.nifcloud.com\n");
			_builder.AppendFormat("{0}\n", url.AbsolutePath);
			_builder.Append("SignatureMethod=HmacSHA256&SignatureVersion=2");
			_builder.AppendFormat("&X-NCMB-Application-Key={0}", applicationKey);
			_builder.AppendFormat("&X-NCMB-Timestamp={0}", timestamp.AsString);
			if (null == queries) return _builder.ToString();
			//var q = string.Join(",", queries.Select(q => $"\"{q.Key}\":\"{q.Value}\""));
			//_builder.AppendFormat("&limit=2&where={0}", Uri.EscapeUriString($"{{{q}}}").Replace(":", "%3A"));
			_builder.AppendFormat("&{0}", queries.ToEscapedString());
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

		public string Generate(RequestMethod method, Uri url, string applicationKey, string clientKey, Timestamp timestamp, IEnumerable<IQuery> queries = null)
		{
			var src = GetSource(method, url, applicationKey, timestamp, queries);
			return Generate(src, clientKey);
		}
	}
}