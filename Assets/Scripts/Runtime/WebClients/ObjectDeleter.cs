using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Minoly
{
	public class ObjectDeleter
	{
		private static readonly SignatureGenerator SignatureGenerator = new SignatureGenerator();
		private readonly ICurrentDateTime _current;
		private readonly string _applicationKey;
		private readonly string _clientKey;
		private UnityWebRequest _request;
		private ObjectDeleteResult _result;

		public ObjectDeleter(string applicationKey, string clientKey, ICurrentDateTime current = null)
		{
			_applicationKey = applicationKey;
			_clientKey = clientKey;
			_current = current ?? new CurrentDateTime();
			_result = new ObjectDeleteResult(RequestResultType.Unknown, 0, null);
		}
		
		public UnityWebRequestAsyncOperation DeleteAsync(string className, string objectId)
		{
			_result = new ObjectDeleteResult(RequestResultType.Unknown, 0, null);
			var current = new Timestamp(_current.Get());
			var uri = new Uri($"https://mbaas.api.nifcloud.com/2013-09-01/classes/{className}/{objectId}");
			var signature = SignatureGenerator.Generate(RequestMethod.Delete, uri, _applicationKey, _clientKey, current);
			_request = new UnityWebRequest(uri, RequestMethod.Delete.ToHttpString());
			_request.SetRequestHeader("Content-Type","application/json");
			_request.SetRequestHeader("X-NCMB-Application-Key", _applicationKey);
			_request.SetRequestHeader("X-NCMB-Signature",signature);
			_request.SetRequestHeader("X-NCMB-Timestamp", current.AsString);
			_request.downloadHandler = new DownloadHandlerBuffer();
			return _request.SendWebRequest();
		}

		public ObjectDeleteResult GetResult()
		{
			if (_result.Type != RequestResultType.Unknown && _result.Type != RequestResultType.InProgress) return _result;
			if (_request == null) return _result = new ObjectDeleteResult(RequestResultType.Unknown, 0, null);
			if (_request.result == UnityWebRequest.Result.InProgress) return _result = new ObjectDeleteResult(RequestResultType.InProgress, 0, null);
			if (_request.error == "Request aborted") return _result = new ObjectDeleteResult(RequestResultType.Aborted, 0, null);
			var resultText = _request.downloadHandler.text;
			var error = _request.result == UnityWebRequest.Result.ProtocolError
				? JsonUtility.FromJson<ErrorResponse>(resultText)
				: null;
			var resultType = _request.result.ToRequestResultType();
			var statusCode = (int)_request.responseCode;
			return _result = new ObjectDeleteResult(resultType, statusCode, error);
		}

	}
}