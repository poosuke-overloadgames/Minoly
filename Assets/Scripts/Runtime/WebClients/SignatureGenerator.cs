using System;
using System.Security.Cryptography;
using System.Text;

namespace Minoly
{
	public class SignatureGenerator
	{
		public string Generate(string src, string clientKey)
		{
			var binClientKey = Encoding.UTF8.GetBytes(clientKey);
			var binSrc = Encoding.UTF8.GetBytes(src);
			var hmacSha256 = new HMACSHA256();
			hmacSha256.Key = binClientKey;
			var hash = hmacSha256.ComputeHash(binSrc);
			return Convert.ToBase64String(hash);
		}
	}
}