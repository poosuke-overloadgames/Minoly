using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace Minoly
{
	public class ObjectFinder
	{
		private static readonly SignatureGenerator SignatureGenerator = new SignatureGenerator();
		private readonly ICurrentDateTime _current;
		private readonly string _applicationKey;
		private readonly string _clientKey;
		private UnityWebRequest _request;
		private ObjectFindResult _result;
		
		public ObjectFinder(string applicationKey, string clientKey, ICurrentDateTime current = null)
		{
			_applicationKey = applicationKey;
			_clientKey = clientKey;
			_current = current ?? new CurrentDateTime();
			_result = ObjectFindResult.CreateUnknown();
		}

		private class QueryComparer : IEqualityComparer<IQuery>
		{
			public bool Equals(IQuery x, IQuery y)
			{
				if (ReferenceEquals(x, y)) return true;
				if (ReferenceEquals(x, null)) return false;
				if (ReferenceEquals(y, null)) return false;
				if (x.GetType() != y.GetType()) return false;
				return x.Key == y.Key;
			}

			public int GetHashCode(IQuery obj)
			{
				return (obj.Key != null ? obj.Key.GetHashCode() : 0);
			}
		}

		private static readonly QueryComparer Comparer = new QueryComparer();

		private void ThrowExceptionIfDuplicationQueryFound(IReadOnlyList<IQuery> queries)
		{
			if (queries.Count == queries.Distinct(Comparer).Count()) return;
			throw new MinolyDuplicateQueryException();
		}

		public UnityWebRequestAsyncOperation FindAsync(string className, IEnumerable<IQuery> queries)
		{
			var queryArray = queries as IQuery[] ?? queries.ToArray();
			ThrowExceptionIfDuplicationQueryFound(queryArray);
			
			_result = ObjectFindResult.CreateUnknown();
			var current = new Timestamp(_current.Get());
			var uri = new Uri($"https://mbaas.api.nifcloud.com/2013-09-01/classes/{className}?{queryArray.ToEscapedString()}");
			var signature = SignatureGenerator.Generate(RequestMethod.Get, uri, _applicationKey, _clientKey, current, queryArray);
			_request = UnityWebRequest.Get(uri);
			_request.SetRequestHeader("Content-Type","application/json");
			_request.SetRequestHeader("X-NCMB-Application-Key", _applicationKey);
			_request.SetRequestHeader("X-NCMB-Signature",signature);
			_request.SetRequestHeader("X-NCMB-Timestamp", current.AsString);
			return _request.SendWebRequest();
		}

		public ObjectFindResult GetResult()
		{
			if (_result.Type != RequestResultType.Unknown && _result.Type != RequestResultType.InProgress) return _result;
			if (_request == null) return _result = ObjectFindResult.CreateUnknown();
			if (_request.result == UnityWebRequest.Result.InProgress) return _result = ObjectFindResult.CreateInProgress();
			if (_request.error == "Request aborted") return _result = ObjectFindResult.CreateAborted();
			var resultText = _request.downloadHandler.text;
			var error = _request.result == UnityWebRequest.Result.ProtocolError
				? JsonUtility.FromJson<ErrorResponse>(resultText)
				: null;
			var resultType = _request.result.ToRequestResultType();
			return _result = new ObjectFindResult(resultType, (int)_request.responseCode, error, resultText);
		}

	}
}