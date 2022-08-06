using System;

namespace Minoly
{
	public class CurrentDateTime : ICurrentDateTime
	{
		public DateTime Get() => DateTime.Now;
	}
}