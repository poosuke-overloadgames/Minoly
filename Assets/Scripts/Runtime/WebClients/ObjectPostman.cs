using System;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Minoly
{
	public class ObjectPostman
	{
		[Serializable]
		private class ResultBody
		{
			public string objectId;
			public string createDate;
		}

		private static readonly SignatureGenerator SignatureGenerator = new SignatureGenerator();
		private readonly ICurrentDateTime _current;
		private readonly string _applicationKey;
		private readonly string _clientKey;
		private UnityWebRequest _request;
		private ObjectPostResult _result;

		public ObjectPostman(string applicationKey, string clientKey, ICurrentDateTime current = null)
		{
			_applicationKey = applicationKey;
			_clientKey = clientKey;
			_current = current ?? new CurrentDateTime();
			_result = ObjectPostResult.CreateUnknown();
		}
		
		public UnityWebRequestAsyncOperation PostAsync(string className, string contentInJson)
		{
			if (GetResult().Type == RequestResultType.InProgress) throw new MinolyInProgressException();
			_result = ObjectPostResult.CreateUnknown();
			var current = new Timestamp(_current.Get());
			var uri = new Uri($"https://mbaas.api.nifcloud.com/2013-09-01/classes/{className}");
			var signature = SignatureGenerator.Generate(RequestMethod.Post, uri, _applicationKey, _clientKey, current);
			_request = new UnityWebRequest(uri, RequestMethod.Post.ToHttpString());
			_request.SetRequestHeader("Content-Type","application/json");
			_request.SetRequestHeader("X-NCMB-Application-Key", _applicationKey);
			_request.SetRequestHeader("X-NCMB-Signature",signature);
			_request.SetRequestHeader("X-NCMB-Timestamp", current.AsString);
			var buff = Encoding.UTF8.GetBytes(contentInJson);
			_request.uploadHandler = new UploadHandlerRaw(buff);
			_request.downloadHandler = new DownloadHandlerBuffer();
			return _request.SendWebRequest();
		}
		
		public ObjectPostResult GetResult()
		{
			if (_result.Type != RequestResultType.Unknown && _result.Type != RequestResultType.InProgress) return _result;
			if (_request == null) return _result = ObjectPostResult.CreateUnknown();
			if (_request.result == UnityWebRequest.Result.InProgress) return _result = ObjectPostResult.CreateInProgress();
			if (_request.error == "Request aborted") return _result = ObjectPostResult.CreateAborted();
			var resultText = _request.downloadHandler.text;
			var error = _request.result == UnityWebRequest.Result.ProtocolError
				? JsonUtility.FromJson<ErrorResponse>(resultText)
				: null;
			var resultType = _request.result.ToRequestResultType();
			var body = null != error ? null : JsonUtility.FromJson<ResultBody>(resultText);
			var statusCode = (int)_request.responseCode;
			var objectId = body?.objectId ?? "";
			var createDate = DateTime.TryParse(body?.createDate ?? "", out var d) ? d : new DateTime();
			return _result = new ObjectPostResult(resultType, statusCode, error, objectId, createDate);
		}

	}
}