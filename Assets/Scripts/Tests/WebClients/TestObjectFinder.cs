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
		private const int Score = 100;
		private const string UserName = "aaa";
		
		[UnityTest]
		public IEnumerator 正常系1件ヒット()
		{
			var applicationKey = EditorUserSettings.GetConfigValue("MinolyApplicationKey");
			var clientKey = EditorUserSettings.GetConfigValue("MinolyClientKey");
			var objectFinder = new ObjectFinder(applicationKey, clientKey);
			yield return objectFinder.FindAsync(ClassName, new IQuery[]
			{
				new QueryWhereEqualTo("userName", UserName)
			});
			var result = objectFinder.GetResult();
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
			var applicationKey = EditorUserSettings.GetConfigValue("MinolyApplicationKey");
			var clientKey = EditorUserSettings.GetConfigValue("MinolyClientKey");
			var objectFinder = new ObjectFinder(applicationKey, clientKey);
			yield return objectFinder.FindAsync(ClassName, new IQuery[]
			{
				new QueryWhereEqualTo("userName", "Detarame")
			});
			var result = objectFinder.GetResult();
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
		}

		[UnityTest]
		public IEnumerator UniTaskによる正常系1件ヒット() => UniTask.ToCoroutine(async () =>
		{
			var applicationKey = EditorUserSettings.GetConfigValue("MinolyApplicationKey");
			var clientKey = EditorUserSettings.GetConfigValue("MinolyClientKey");
			var objectFinder = new ObjectFinder(applicationKey, clientKey);
			var result = await objectFinder.FindTask(ClassName, new IQuery[]
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