using System;

namespace Minoly
{
	public struct ObjectUpdateResult
	{
		public ObjectUpdateResult(
			RequestResultType type, 
			int httpStatusCode, 
			ErrorResponse errorResponse,
			DateTime updateDate
		)
		{
			Type = type;
			HttpStatusCode = httpStatusCode;
			ErrorResponse = errorResponse;
			UpdateDate = updateDate;
		}
		public RequestResultType Type { get; }
		public int HttpStatusCode { get; }
		public ErrorResponse ErrorResponse { get; }
		public DateTime UpdateDate { get; }
		public static ObjectUpdateResult CreateUnknown() => new ObjectUpdateResult(RequestResultType.Unknown, 0, null, new DateTime());
		public static ObjectUpdateResult CreateInProgress() => new ObjectUpdateResult(RequestResultType.InProgress, 0, null, new DateTime());
		public static ObjectUpdateResult CreateAborted() => new ObjectUpdateResult(RequestResultType.Aborted, 0, null, new DateTime());

	}
}