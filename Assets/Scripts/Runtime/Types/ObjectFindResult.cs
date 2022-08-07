namespace Minoly
{
	public struct ObjectFindResult
	{
		public ObjectFindResult(
			RequestResultType type, 
			int httpStatusCode, 
			ErrorResponse errorResponse,
			string body
		)
		{
			Type = type;
			HttpStatusCode = httpStatusCode;
			ErrorResponse = errorResponse;
			Body = body;
		}
		public RequestResultType Type { get; }
		public int HttpStatusCode { get; }
		public ErrorResponse ErrorResponse { get; }
		public string Body { get; }

		public static ObjectFindResult CreateUnknown() => new ObjectFindResult(RequestResultType.Unknown, 0, null, "");
		public static ObjectFindResult CreateInProgress() => new ObjectFindResult(RequestResultType.InProgress, 0, null, "");
		public static ObjectFindResult CreateAborted() => new ObjectFindResult(RequestResultType.Aborted, 0, null, "");
	}
}