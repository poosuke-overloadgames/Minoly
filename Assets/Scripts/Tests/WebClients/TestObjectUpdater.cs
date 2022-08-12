using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using Minoly;
using Minoly.UniTask;
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
		private ObjectGetter _objectGetter;
		private ObjectUpdater _objectUpdater;
		private ObjectPostman _objectPostman;
		private ObjectDeleter _objectDeleter;
		private string _objectId;

		[UnitySetUp]
		public IEnumerator SetUp()
		{
			var applicationKey = EditorUserSettings.GetConfigValue("MinolyApplicationKey");
			var clientKey = EditorUserSettings.GetConfigValue("MinolyClientKey");
			_objectGetter = new ObjectGetter(applicationKey, clientKey);
			_objectUpdater = new ObjectUpdater(applicationKey, clientKey);
			_objectPostman = new ObjectPostman(applicationKey, clientKey);
			_objectDeleter = new ObjectDeleter(applicationKey, clientKey);
			yield return _objectPostman.PostAsync(ClassName, ContentInJsonOrg);
			_objectId = _objectPostman.GetResult().ObjectId;
		}

		[UnityTearDown]
		public IEnumerator TearDown()
		{
			yield return _objectDeleter.DeleteAsync(ClassName, _objectId);
			_objectGetter.Dispose();
			_objectPostman.Dispose();
			_objectUpdater.Dispose();
			_objectDeleter.Dispose();
		}


		[UnityTest]
		public IEnumerator 正常系()
		{
			yield return _objectUpdater.UpdateAsync(ClassName, _objectId, ContentInJson);
			var upsertResult = _objectUpdater.GetResult();
			Assert.That(upsertResult.Type, Is.EqualTo(RequestResultType.Success));
			Assert.That(upsertResult.HttpStatusCode, Is.EqualTo(200));
			Assert.That(upsertResult.ErrorResponse, Is.Null);
			Assert.That(upsertResult.UpdateDate, Is.Not.EqualTo(new DateTime()));

			yield return _objectGetter.FetchAsync(ClassName, _objectId);
			var getResult = _objectGetter.GetResult();
			var testClass = JsonUtility.FromJson<TestClass>(getResult.Body);
			Assert.That(testClass.userName, Is.EqualTo(UserName));
			Assert.That(testClass.score, Is.EqualTo(Score));
		}

		[UnityTest]
		public IEnumerator 異常系()
		{
			yield return _objectUpdater.UpdateAsync(ClassName, "Detarame", ContentInJson);
			var upsertResult = _objectUpdater.GetResult();
			Assert.That(upsertResult.Type, Is.EqualTo(RequestResultType.ProtocolError));
			Assert.That(upsertResult.HttpStatusCode, Is.EqualTo(404));
			Assert.That(upsertResult.ErrorResponse.code, Is.EqualTo("E404001"));
			Assert.That(upsertResult.ErrorResponse.error, Is.EqualTo("No data available."));
		}
		
		[UnityTest]
		public IEnumerator UniTaskによるUpdate() => UniTask.ToCoroutine(async () =>
		{
			await _objectUpdater.UpdateTask(ClassName, _objectId, ContentInJson);
			var getResult = await _objectGetter.FetchTask(ClassName, _objectId);
			var testClass = JsonUtility.FromJson<TestClass>(getResult.Body);
			Assert.That(testClass.userName, Is.EqualTo(UserName));
			Assert.That(testClass.score, Is.EqualTo(Score));
		});

	}
}