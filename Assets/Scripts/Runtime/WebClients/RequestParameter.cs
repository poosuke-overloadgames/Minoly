using System.Collections.Generic;
using Minoly.Types;

namespace Minoly
{
	public class RequestParameter
	{
		public RequestMethod Method { get; set; }
		public string ClassName { get; set; }
		public List<GetQuery> GetQueries { get; set; } = new List<GetQuery>();
		public string ApplicationKey { get; set; }
		public Timestamp Timestamp { get; set; }
		
		public string Domain { get; set; } = "mbaas.api.nifcloud.com";
		public string ApiVersion { get; set; } = "2013-09-01";
		public string SignatureMethod { get; set; } = "HmacSHA256";
		public string SignatureVer { get; set; } = "2";
	}
}