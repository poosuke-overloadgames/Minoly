using System;
using System.Collections;
using Minoly;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
	[TestFixture]
	public class TestObjectUpdater
	{
		private const string ClassName = "TestClass";
		private const string UserName = "aaa";
		private const int Score = 200;
		private const string ContentInJson = "{\"userName\": \"aaa\", \"score\": \"200\"}";
		private const string ContentInJsonOrg = "{\"userName\": \"aaa\", \"score\": \"100\"}";


		[UnityTest]
		public IEnumerator 正常系()
		{
			var applicationKey = EditorUserSettings.GetConfigValue("MinolyApplicationKey");
			var clientKey = EditorUserSettings.GetConfigValue("MinolyClientKey");
			var objectId = EditorUserSettings.GetConfigValue("MinolyObjectId");
			var objectGetter = new ObjectGetter(applicationKey, clientKey);
			var objectUpserter = new ObjectUpdater(applicationKey, clientKey);

			yield return objectUpserter.UpdateAsync(ClassName, objectId, ContentInJson);
			var upsertResult = objectUpserter.GetResult();
			Assert.That(upsertResult.Type, Is.EqualTo(RequestResultType.Success));
			Assert.That(upsertResult.HttpStatusCode, Is.EqualTo(200));
			Assert.That(upsertResult.ErrorResponse, Is.Null);
			Assert.That(upsertResult.UpdateDate, Is.Not.EqualTo(new DateTime()));

			yield return objectGetter.FetchAsync(ClassName, objectId);
			var getResult = objectGetter.GetResult();
			var testClass = JsonUtility.FromJson<TestClass>(getResult.Body);
			Assert.That(testClass.userName, Is.EqualTo(UserName));
			Assert.That(testClass.score, Is.EqualTo(Score));

			yield return objectUpserter.UpdateAsync(ClassName, objectId, ContentInJsonOrg);
		}

		[UnityTest]
		public IEnumerator 異常系()
		{
			var applicationKey = EditorUserSettings.GetConfigValue("MinolyApplicationKey");
			var clientKey = EditorUserSettings.GetConfigValue("MinolyClientKey");
			var objectId = "Detarame";
			var objectGetter = new ObjectGetter(applicationKey, clientKey);
			var objectUpserter = new ObjectUpdater(applicationKey, clientKey);

			yield return objectUpserter.UpdateAsync(ClassName, objectId, ContentInJson);
			var upsertResult = objectUpserter.GetResult();
			Assert.That(upsertResult.Type, Is.EqualTo(RequestResultType.ProtocolError));
			Assert.That(upsertResult.HttpStatusCode, Is.EqualTo(404));
			Assert.That(upsertResult.ErrorResponse.code, Is.EqualTo("E404001"));
			Assert.That(upsertResult.ErrorResponse.error, Is.EqualTo("No data available."));
		}
	}
}