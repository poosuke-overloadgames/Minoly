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
		public IEnumerator 正常系()
		{
			var applicationKey = EditorUserSettings.GetConfigValue("MinolyApplicationKey");
			var clientKey = EditorUserSettings.GetConfigValue("MinolyClientKey");
			var objectId = EditorUserSettings.GetConfigValue("MinolyObjectId");
			var objectGetter = new ObjectGetter(applicationKey, clientKey);
			yield return objectGetter.Fetch(ClassName, objectId);
			var result = objectGetter.GetResult();
			Assert.That(result.Type, Is.EqualTo(RequestResultType.Success));
			Assert.That(result.HttpStatusCode, Is.EqualTo(200));
			Assert.That(result.ErrorResponse, Is.Null);
			var testClass = JsonUtility.FromJson<TestClass>(result.Body);
			Assert.That(testClass.userName, Is.EqualTo(UserName));
			Assert.That(testClass.score, Is.EqualTo(Score));
		}

		[UnityTest]
		public IEnumerator 異常系()
		{
			var applicationKey = EditorUserSettings.GetConfigValue("MinolyApplicationKey");
			var clientKey = "Detarame";
			var objectId = EditorUserSettings.GetConfigValue("MinolyObjectId");
			var objectGetter = new ObjectGetter(applicationKey, clientKey);
			yield return objectGetter.Fetch(ClassName, objectId);
			var result = objectGetter.GetResult();
			Assert.That(result.Type, Is.EqualTo(RequestResultType.ProtocolError));
			Assert.That(result.HttpStatusCode, Is.EqualTo(403));
			Assert.That(result.ErrorResponse.code, Is.EqualTo("E403002"));
			Assert.That(result.ErrorResponse.error, Is.EqualTo("Unauthorized operations for signature."));
		}
	}
}