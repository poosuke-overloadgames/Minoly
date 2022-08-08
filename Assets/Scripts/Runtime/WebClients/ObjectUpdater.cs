using System;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Minoly
{
	public class ObjectUpdater
	{
		[Serializable]
		private class ResultBody
		{
			public string updateDate;
		}

		private static readonly SignatureGenerator SignatureGenerator = new SignatureGenerator();
		private readonly ICurrentDateTime _current;
		private readonly string _applicationKey;
		private readonly string _clientKey;
		private UnityWebRequest _request;
		private ObjectUpdateResult _result;

		public ObjectUpdater(string applicationKey, string clientKey, ICurrentDateTime current = null)
		{
			_applicationKey = applicationKey;
			_clientKey = clientKey;
			_current = current ?? new CurrentDateTime();
			_result = ObjectUpdateResult.CreateUnknown();
		}
		public UnityWebRequestAsyncOperation UpdateAsync(string className, string objectId, string contentInJson)
		{
			_result = ObjectUpdateResult.CreateUnknown();
			var current = new Timestamp(_current.Get());
			var uri = new Uri($"https://mbaas.api.nifcloud.com/2013-09-01/classes/{className}/{objectId}");
			var signature = SignatureGenerator.Generate(RequestMethod.Put, uri, _applicationKey, _clientKey, current);
			_request = new UnityWebRequest(uri, RequestMethod.Put.ToHttpString());
			_request.SetRequestHeader("Content-Type","application/json");
			_request.SetRequestHeader("X-NCMB-Application-Key", _applicationKey);
			_request.SetRequestHeader("X-NCMB-Signature",signature);
			_request.SetRequestHeader("X-NCMB-Timestamp", current.AsString);
			var buff = Encoding.UTF8.GetBytes(contentInJson);
			_request.uploadHandler = new UploadHandlerRaw(buff);
			_request.downloadHandler = new DownloadHandlerBuffer();
			return _request.SendWebRequest();
		}
		
		public ObjectUpdateResult GetResult()
		{
			if (_result.Type != RequestResultType.Unknown && _result.Type != RequestResultType.InProgress) return _result;
			if (_request == null) return _result = ObjectUpdateResult.CreateUnknown();
			if (_request.result == UnityWebRequest.Result.InProgress) return _result = ObjectUpdateResult.CreateInProgress();
			if (_request.error == "Request aborted") return _result = ObjectUpdateResult.CreateAborted();
			var resultText = _request.downloadHandler.text;
			var error = _request.result == UnityWebRequest.Result.ProtocolError
				? JsonUtility.FromJson<ErrorResponse>(resultText)
				: null;
			var resultType = _request.result.ToRequestResultType();
			var body = error != null ? null : JsonUtility.FromJson<ResultBody>(resultText);
			var createDate = DateTime.TryParse(body?.updateDate ?? "", out var d) ? d : new DateTime();
			return _result = new ObjectUpdateResult(resultType, (int)_request.responseCode, error, createDate);
		}


	}
}