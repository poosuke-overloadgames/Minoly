using System;

namespace Minoly
{
	public readonly struct Timestamp
	{
		public Timestamp(DateTime dateTime)
		{
			AsString = dateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
			AsUrlEscaped = Uri.EscapeUriString(AsString);
			AsDateTime = dateTime;
		}
		public string AsString { get; }
		public string AsUrlEscaped { get; }
		public DateTime AsDateTime { get; }
	}
}