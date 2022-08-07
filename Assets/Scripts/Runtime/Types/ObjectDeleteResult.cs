namespace Minoly
{
	public struct ObjectDeleteResult
	{
		public ObjectDeleteResult(
			RequestResultType type, 
			int httpStatusCode, 
			ErrorResponse errorResponse
			)
		{
			Type = type;
			HttpStatusCode = httpStatusCode;
			ErrorResponse = errorResponse;
		}
		public RequestResultType Type { get; }
		public int HttpStatusCode { get; }
		public ErrorResponse ErrorResponse { get; }
	}
}