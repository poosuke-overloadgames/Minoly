namespace Minoly
{
	public struct RequestResult
	{
		public RequestResult(RequestResultType type, int httpStatusCode, ErrorResponse errorResponse, string body)
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

		public static RequestResult CreateUnknown() => new RequestResult(RequestResultType.Unknown, 0, null, "");
		public static RequestResult CreateInProgress() => new RequestResult(RequestResultType.InProgress, 0, null, "");
	}
}