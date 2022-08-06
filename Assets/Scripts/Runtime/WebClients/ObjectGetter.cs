using System;
using UnityEngine;
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
		private RequestResult _result;

		private RequestResultType ToRequestResultType(UnityWebRequest.Result result) => result switch
		{
			UnityWebRequest.Result.InProgress => RequestResultType.InProgress,
			UnityWebRequest.Result.Success => RequestResultType.Success,
			UnityWebRequest.Result.ConnectionError => RequestResultType.NetworkError,
			UnityWebRequest.Result.ProtocolError => RequestResultType.ProtocolError,
			UnityWebRequest.Result.DataProcessingError => RequestResultType.DataError,
			_ => RequestResultType.Unknown
		};
		

		public ObjectGetter(string applicationKey, string clientKey, ICurrentDateTime current = null)
		{
			_applicationKey = applicationKey;
			_clientKey = clientKey;
			_current = current ?? new CurrentDateTime();
			_result = RequestResult.CreateUnknown();
		}

		public UnityWebRequestAsyncOperation FetchAsync(string className, string objectId)
		{
			_result = RequestResult.CreateUnknown();
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
		
		public RequestResult GetResult()
		{
			if (_result.Type != RequestResultType.Unknown && _result.Type != RequestResultType.InProgress) return _result;
			if (_request == null) return _result = RequestResult.CreateUnknown();
			if (_request.result == UnityWebRequest.Result.InProgress) return _result = RequestResult.CreateInProgress();
			var resultText = _request.downloadHandler.text;
			var error = _request.result == UnityWebRequest.Result.ProtocolError
				? JsonUtility.FromJson<ErrorResponse>(resultText)
				: null;
			var resultType = ToRequestResultType(_request.result);
			return _result = new RequestResult(resultType, (int)_request.responseCode, error, resultText);
		}

	}
}