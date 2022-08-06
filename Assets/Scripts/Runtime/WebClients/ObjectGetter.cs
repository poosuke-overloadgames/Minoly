using System;
using System.Collections;
using Minoly.Types;
using UnityEngine.Networking;

namespace Minoly
{
	public class ObjectGetter
	{
		private static readonly SignatureGenerator SignatureGenerator = new SignatureGenerator();
		private readonly ICurrentDateTime _current;
		private readonly string _applicationKey;
		private readonly string _clientKey;
		private UnityWebRequest _request; 

		public ObjectGetter(string applicationKey, string clientKey, ICurrentDateTime current = null)
		{
			_applicationKey = applicationKey;
			_clientKey = clientKey;
			_current = current ?? new CurrentDateTime();
		}

		public UnityWebRequestAsyncOperation Fetch(string className, string objectId)
		{
			var current = new Timestamp(_current.Get());
			var uri = new Uri($"https://mbaas.api.nifcloud.com/2013-09-01/classes/{className}/{objectId}");
			var signature = SignatureGenerator.Generate(RequestMethod.Get, uri, _applicationKey, _clientKey, Array.Empty<GetQuery>(), current);
			_request = UnityWebRequest.Get(uri);
			_request.SetRequestHeader("Content-Type","application/json");
			_request.SetRequestHeader("X-NCMB-Application-Key", _applicationKey);
			_request.SetRequestHeader("X-NCMB-Signature",signature);
			_request.SetRequestHeader("X-NCMB-Timestamp", current.AsString);
			return _request.SendWebRequest();
		}

		public string Response => _request.downloadHandler.text;

	}
}