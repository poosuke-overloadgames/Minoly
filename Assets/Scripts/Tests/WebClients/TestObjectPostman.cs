using System;
using System.Collections;
using System.Collections.Generic;
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
	public class TestObjectPostman
	{
		private const string ClassName = "TestClass";
		private const string UserName = "bbb";
		private const int Score = 200;
		private const string ContentInJson = "{\"userName\": \"bbb\", \"score\": \"200\"}";
		private readonly List<string> _disposableObjectIds = new List<string>();
		private ObjectGetter _objectGetter;
		private ObjectPostman _objectPostman;
		private ObjectDeleter _objectDeleter;

		[SetUp]
		public void SetUp()
		{
			var applicationKey = EditorUserSettings.GetConfigValue("MinolyApplicationKey");
			var clientKey = EditorUserSettings.GetConfigValue("MinolyClientKey");
			_objectGetter = new ObjectGetter(applicationKey, clientKey);
			_objectPostman = new ObjectPostman(applicationKey, clientKey);
			_objectDeleter = new ObjectDeleter(applicationKey, clientKey);
		}

		[UnityTearDown]
		public IEnumerator TearDown()
		{
			foreach (var objectId in _disposableObjectIds)
			{
				yield return _objectDeleter.DeleteAsync(ClassName, objectId);
			}
			_objectGetter.Dispose();
			_objectPostman.Dispose();
			_objectDeleter.Dispose();
		}

		[UnityTest]
		public IEnumerator 正常系()
		{
			yield return _objectPostman.PostAsync(ClassName, ContentInJson);
			var postResult = _objectPostman.GetResult();
			Assert.That(postResult.Type, Is.EqualTo(RequestResultType.Success));
			Assert.That(postResult.HttpStatusCode, Is.EqualTo(201));
			Assert.That(postResult.ErrorResponse, Is.Null);
			Assert.That(postResult.ObjectId, Is.Not.EqualTo(""));
			Assert.That(postResult.CreateDate, Is.Not.EqualTo(new DateTime()));
			_disposableObjectIds.Add(postResult.ObjectId);

			yield return _objectGetter.FetchAsync(ClassName, postResult.ObjectId);
			var getResult = _objectGetter.GetResult();
			var testClass = JsonUtility.FromJson<TestClass>(getResult.Body);
			Assert.That(testClass.userName, Is.EqualTo(UserName));
			Assert.That(testClass.score, Is.EqualTo(Score));
		}

		[UnityTest]
		public IEnumerator 異常系()
		{
			var applicationKey = "Detarame";
			var clientKey = EditorUserSettings.GetConfigValue("MinolyClientKey");
			var objectPostman = new ObjectPostman(applicationKey, clientKey);

			yield return objectPostman.PostAsync(ClassName, ContentInJson);
			var postResult = objectPostman.GetResult();
			Assert.That(postResult.Type, Is.EqualTo(RequestResultType.ProtocolError));
			Assert.That(postResult.HttpStatusCode, Is.EqualTo(404));
			Assert.That(postResult.ErrorResponse.code, Is.EqualTo("E404005"));
			Assert.That(postResult.ErrorResponse.error, Is.EqualTo("No such application."));
			
			objectPostman.Dispose();
		}
		
		[UnityTest]
		public IEnumerator UniTaskによるPost() => UniTask.ToCoroutine(async () =>
		{
			var postResult = await _objectPostman.PostTask(ClassName, ContentInJson);
			var getResult = await _objectGetter.FetchTask(ClassName, postResult.ObjectId);
			var testClass = JsonUtility.FromJson<TestClass>(getResult.Body);
			Assert.That(testClass.userName, Is.EqualTo(UserName));
			Assert.That(testClass.score, Is.EqualTo(Score));
			_disposableObjectIds.Add(postResult.ObjectId);
		});
	}
}