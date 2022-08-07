using System;

namespace Minoly
{
	public enum RequestMethod
	{
		Get,
		Post,
		Put,
		Delete
	}

	public static class RequestMethodExtension
	{
		public static string ToHttpString(this RequestMethod method) => method switch
		{
			RequestMethod.Get => "GET",
			RequestMethod.Post => "POST",
			RequestMethod.Put => "PUT",
			RequestMethod.Delete => "DELETE",
			_ => throw new NotImplementedException()
		};
	}
}