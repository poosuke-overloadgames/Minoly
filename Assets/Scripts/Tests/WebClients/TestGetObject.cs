using System;
using System.Collections;
using System.Collections.Generic;
using Minoly;
using Minoly.Types;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
	[TestFixture]
	public class TestGetObject
	{
		private const string ClassName = "TestClass";
		private const string UserName = "aaa";
		private const int Score = 100;

		[Serializable]
		private class TestClass
		{
			public string objectId;
			public string createDate; //UnityEngine.JsonUtilityではタイムスタンプをDateTimeに変換してくれない
			public string updateDate;
			//public string acl;
			public string userName;
			public int score;
		}

		[UnityTest]
		public IEnumerator テスト()
		{
			var applicationKey = EditorUserSettings.GetConfigValue("MinolyApplicationKey");
			var clientKey = EditorUserSettings.GetConfigValue("MinolyClientKey");
			var objectId = EditorUserSettings.GetConfigValue("MinolyObjectId");
			var objectGetter = new ObjectGetter(applicationKey, clientKey);
			yield return objectGetter.Fetch(ClassName, objectId);
			var testClass = JsonUtility.FromJson<TestClass>(objectGetter.Response);
			Assert.That(testClass.userName, Is.EqualTo(UserName));
			Assert.That(testClass.score, Is.EqualTo(Score));
		}
	}
}