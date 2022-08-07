using System;

namespace Minoly
{
	public enum RequestMethod
	{
		Get,
		Post
	}

	public static class RequestMethodExtension
	{
		public static string ToHttpString(this RequestMethod method) => method switch
		{
			RequestMethod.Get => "GET",
			RequestMethod.Post => "POST",
			_ => throw new NotImplementedException()
		};
	}
}