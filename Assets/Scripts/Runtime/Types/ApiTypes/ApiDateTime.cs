using System;
// ReSharper disable InconsistentNaming

namespace Minoly.ApiTypes
{
	[Serializable]
	public struct ApiDateTime
	{
		public string __type;
		public string iso;

		public DateTime DateTime => DateTime.TryParse(iso, out var d) ? d : new DateTime();

		public ApiDateTime(DateTime dateTime)
		{
			__type = "Date";
			iso = new Timestamp(dateTime).AsString;
		}
	}
}