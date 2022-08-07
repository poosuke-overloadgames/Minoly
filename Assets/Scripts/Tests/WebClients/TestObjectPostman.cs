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
	public class TestObjectPostman
	{
		private const string ClassName = "TestClass";
		private const string UserName = "bbb";
		private const int Score = 200;
		private const string ContentInJson = "{\"userName\": \"bbb\", \"score\": \"200\"}";


		[UnityTest]
		public IEnumerator 正常系()
		{
			var applicationKey = EditorUserSettings.GetConfigValue("MinolyApplicationKey");
			var clientKey = EditorUserSettings.GetConfigValue("MinolyClientKey");
			var objectGetter = new ObjectGetter(applicationKey, clientKey);
			var objectPostman = new ObjectPostman(applicationKey, clientKey);

			yield return objectPostman.PostAsync(ClassName, ContentInJson);
			var postResult = objectPostman.GetResult();
			Assert.That(postResult.Type, Is.EqualTo(RequestResultType.Success));
			Assert.That(postResult.HttpStatusCode, Is.EqualTo(201));
			Assert.That(postResult.ErrorResponse, Is.Null);
			Assert.That(postResult.ObjectId, Is.Not.EqualTo(""));
			Assert.That(postResult.CreateDate, Is.Not.EqualTo(new DateTime()));

			Debug.Log("TestObjectPostman.正常系");
			Debug.Log($"objectId:{postResult.ObjectId}");
			Debug.Log($"createDate:{postResult.CreateDate}");

			yield return objectGetter.FetchAsync(ClassName, postResult.ObjectId);
			var getResult = objectGetter.GetResult();
			var testClass = JsonUtility.FromJson<TestClass>(getResult.Body);
			Assert.That(testClass.userName, Is.EqualTo(UserName));
			Assert.That(testClass.score, Is.EqualTo(Score));
			
			var objectDeleter = new ObjectDeleter(applicationKey, clientKey);
			yield return objectDeleter.DeleteAsync(ClassName, postResult.ObjectId);
		}

		[UnityTest]
		public IEnumerator 異常系()
		{
			var applicationKey = "Detarame";
			var clientKey = EditorUserSettings.GetConfigValue("MinolyClientKey");
			var objectGetter = new ObjectGetter(applicationKey, clientKey);
			var objectPostman = new ObjectPostman(applicationKey, clientKey);

			yield return objectPostman.PostAsync(ClassName, ContentInJson);
			var postResult = objectPostman.GetResult();
			Assert.That(postResult.Type, Is.EqualTo(RequestResultType.ProtocolError));
			Assert.That(postResult.HttpStatusCode, Is.EqualTo(404));
			Assert.That(postResult.ErrorResponse.code, Is.EqualTo("E404005"));
			Assert.That(postResult.ErrorResponse.error, Is.EqualTo("No such application."));
		}
		
		[UnityTest]
		public IEnumerator UniTaskによるPost() => UniTask.ToCoroutine(async () =>
		{
			var applicationKey = EditorUserSettings.GetConfigValue("MinolyApplicationKey");
			var clientKey = EditorUserSettings.GetConfigValue("MinolyClientKey");
			var objectPostman = new ObjectPostman(applicationKey, clientKey);
			var objectGetter = new ObjectGetter(applicationKey, clientKey);
			var postResult = await objectPostman.PostTask(ClassName, ContentInJson);
			var getResult = await objectGetter.FetchTask(ClassName, postResult.ObjectId);
			var testClass = JsonUtility.FromJson<TestClass>(getResult.Body);
			Assert.That(testClass.userName, Is.EqualTo(UserName));
			Assert.That(testClass.score, Is.EqualTo(Score));
			
			var objectDeleter = new ObjectDeleter(applicationKey, clientKey);
			await objectDeleter.DeleteTask(ClassName, postResult.ObjectId);
		});
	}
}