using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
	public class TestObjectFinder
	{
		private const string ClassName = "TestClass";
		private const string ContentInJsonOrg = "{\"userName\": \"aaa\", \"score\": \"100\"}";
		private const int Score = 100;
		private const string UserName = "aaa";

		private ObjectFinder _objectFinder;
		private ObjectPostman _objectPostman;
		private ObjectDeleter _objectDeleter;
		private string _objectId;

		[UnitySetUp]
		public IEnumerator SetUp()
		{
			var applicationKey = EditorUserSettings.GetConfigValue("MinolyApplicationKey");
			var clientKey = EditorUserSettings.GetConfigValue("MinolyClientKey");
			_objectPostman = new ObjectPostman(applicationKey, clientKey);
			_objectDeleter = new ObjectDeleter(applicationKey, clientKey);
			_objectFinder = new ObjectFinder(applicationKey, clientKey);
			yield return _objectPostman.PostAsync(ClassName, ContentInJsonOrg);
			_objectId = _objectPostman.GetResult().ObjectId;
		}

		[UnityTearDown]
		public IEnumerator TearDown()
		{
			yield return _objectDeleter.DeleteAsync(ClassName, _objectId);
			_objectPostman.Dispose();
			_objectDeleter.Dispose();
			_objectFinder.Dispose();
		}

		[UnityTest]
		public IEnumerator 正常系1件ヒット()
		{
			yield return _objectFinder.FindAsync(ClassName, new IQuery[]
			{
				new QueryWhereEqualTo("userName", UserName)
			});
			var result = _objectFinder.GetResult();
			Assert.That(result.Type, Is.EqualTo(RequestResultType.Success));
			Assert.That(result.HttpStatusCode, Is.EqualTo(200));
			Assert.That(result.ErrorResponse, Is.Null);
			var testClasses = JsonUtility.FromJson<FoundTestClass>(result.Body).results;
			Assert.That(testClasses.Length, Is.EqualTo(1));
			var testClass = testClasses[0];
			Assert.That(testClass.score, Is.EqualTo(Score));
		}
		
		[UnityTest]
		public IEnumerator 正常系ノーヒット()
		{
			yield return _objectFinder.FindAsync(ClassName, new IQuery[]
			{
				new QueryWhereEqualTo("userName", "Detarame")
			});
			var result = _objectFinder.GetResult();
			Assert.That(result.Type, Is.EqualTo(RequestResultType.Success));
			Assert.That(result.HttpStatusCode, Is.EqualTo(200));
			Assert.That(result.ErrorResponse, Is.Null);
			var testClasses = JsonUtility.FromJson<FoundTestClass>(result.Body).results;
			Assert.That(testClasses.Length, Is.EqualTo(0));
		}

		[UnityTest]
		public IEnumerator 異常系()
		{
			var applicationKey = EditorUserSettings.GetConfigValue("MinolyApplicationKey");
			var clientKey = "Detarame";
			var objectFinder = new ObjectFinder(applicationKey, clientKey);
			yield return objectFinder.FindAsync(ClassName, new IQuery[]
			{
				new QueryWhereEqualTo("userName", UserName)
			});
			var result = objectFinder.GetResult();
			Assert.That(result.Type, Is.EqualTo(RequestResultType.ProtocolError));
			Assert.That(result.HttpStatusCode, Is.EqualTo(403));
			Assert.That(result.ErrorResponse.code, Is.EqualTo("E403002"));
			Assert.That(result.ErrorResponse.error, Is.EqualTo("Unauthorized operations for signature."));
			objectFinder.Dispose();
		}

		[UnityTest]
		public IEnumerator UniTaskによる正常系1件ヒット() => UniTask.ToCoroutine(async () =>
		{
			var result = await _objectFinder.FindTask(ClassName, new IQuery[]
			{
				new QueryWhereEqualTo("userName", UserName)
			});
			Assert.That(result.Type, Is.EqualTo(RequestResultType.Success));
			Assert.That(result.HttpStatusCode, Is.EqualTo(200));
			Assert.That(result.ErrorResponse, Is.Null);
			var testClasses = JsonUtility.FromJson<FoundTestClass>(result.Body).results;
			Assert.That(testClasses.Length, Is.EqualTo(1));
			var testClass = testClasses[0];
			Assert.That(testClass.score, Is.EqualTo(Score));
		});

	}
}