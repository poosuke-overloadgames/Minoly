using System;

namespace Minoly
{
	public struct ObjectPostResult
	{
		public ObjectPostResult(
			RequestResultType type, 
			int httpStatusCode, 
			ErrorResponse errorResponse,
			string objectId, 
			DateTime createDate)
		{
			Type = type;
			HttpStatusCode = httpStatusCode;
			ErrorResponse = errorResponse;
			ObjectId = objectId;
			CreateDate = createDate;
		}

		public RequestResultType Type { get; }
		public int HttpStatusCode { get; }
		public ErrorResponse ErrorResponse { get; }
		public string ObjectId { get; }
		public DateTime CreateDate { get; }

		public static ObjectPostResult CreateUnknown() 
			=> new ObjectPostResult(RequestResultType.Unknown, 0, null, "", new DateTime());
		public static ObjectPostResult CreateInProgress() 
			=> new ObjectPostResult(RequestResultType.InProgress, 0, null, "", new DateTime());
		public static ObjectPostResult CreateAborted() 
			=> new ObjectPostResult(RequestResultType.Aborted, 0, null, "", new DateTime());
	}
}