using UnityEngine.Networking;

namespace Minoly
{
	public static class UnityWebRequestExtension
	{
		public static RequestResultType ToRequestResultType(this UnityWebRequest.Result result) => result switch
		{
			UnityWebRequest.Result.InProgress => RequestResultType.InProgress,
			UnityWebRequest.Result.Success => RequestResultType.Success,
			UnityWebRequest.Result.ConnectionError => RequestResultType.NetworkError,
			UnityWebRequest.Result.ProtocolError => RequestResultType.ProtocolError,
			UnityWebRequest.Result.DataProcessingError => RequestResultType.DataError,
			_ => RequestResultType.Unknown
		};
	}
}