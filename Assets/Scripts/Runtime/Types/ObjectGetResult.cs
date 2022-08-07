namespace Minoly
{
	public struct ObjectGetResult
	{
		public ObjectGetResult(RequestResultType type, int httpStatusCode, ErrorResponse errorResponse, string body)
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

		public static ObjectGetResult CreateUnknown() => new ObjectGetResult(RequestResultType.Unknown, 0, null, "");
		public static ObjectGetResult CreateInProgress() => new ObjectGetResult(RequestResultType.InProgress, 0, null, "");
		public static ObjectGetResult CreateAborted() => new ObjectGetResult(RequestResultType.Aborted, 0, null, "");
	}
}