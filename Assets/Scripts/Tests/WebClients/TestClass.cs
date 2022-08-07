using System;

namespace Tests
{
	[Serializable]
	public class TestClass
	{
		public string objectId;
		public string createDate; //UnityEngine.JsonUtilityではタイムスタンプをDateTimeに変換してくれない
		public string updateDate;
		//public string acl;
		public string userName;
		public int score;
	}
}