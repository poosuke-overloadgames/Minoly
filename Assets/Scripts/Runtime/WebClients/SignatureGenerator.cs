using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Minoly
{
	public class SignatureGenerator
	{
		private readonly StringBuilder _builder = new StringBuilder();
		private string GetSource(RequestParameter parameter)
		{
			_builder.AppendFormat("{0}\n", parameter.Method == RequestMethod.Get ? "GET" : "");
			_builder.Append("mbaas.api.nifcloud.com\n");
			_builder.AppendFormat("/2013-09-01/classes/{0}\n", parameter.ClassName);
			_builder.Append("SignatureMethod=HmacSHA256&SignatureVersion=2");
			_builder.AppendFormat("&X-NCMB-Application-Key={0}", parameter.ApplicationKey);
			_builder.AppendFormat("&X-NCMB-Timestamp={0}", parameter.Timestamp.AsString);
			if (parameter.GetQueries.Count == 0) return _builder.ToString();
			var q = string.Join(",", parameter.GetQueries.Select(q => $"\"{q.Key}\":\"{q.Value}\""));
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

		public string Generate(RequestParameter parameter, string clientKey)
		{
			var src = GetSource(parameter);
			return Generate(src, clientKey);
		}
	}
}