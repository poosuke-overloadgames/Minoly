using System;
using Minoly.ApiTypes;

namespace Tests
{
	[Serializable]
	public class TestClassToPost
	{
		public string userName;
		public int score;
		public ApiDateTime dateTime;
	}
}