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
	public class TestObjectDeleter
	{
		private const string ClassName = "TestClass";
		private const string UserName = "ccc";
		private const int Score = 300;
		private const string ContentInJson = "{\"userName\": \"ccc\", \"score\": \"300\"}";


		[UnityTest]
		public IEnumerator 正常系()
		{
			var applicationKey = EditorUserSettings.GetConfigValue("MinolyApplicationKey");
			var clientKey = EditorUserSettings.GetConfigValue("MinolyClientKey");
			var objectDeleter = new ObjectDeleter(applicationKey, clientKey);
			var objectPostman = new ObjectPostman(applicationKey, clientKey);

			yield return objectPostman.PostAsync(ClassName, ContentInJson);
			var postResult = objectPostman.GetResult();
			Assert.That(postResult.Type, Is.EqualTo(RequestResultType.Success));

			yield return objectDeleter.DeleteAsync(ClassName, postResult.ObjectId);
			var result = objectDeleter.GetResult();
			Assert.That(result.Type, Is.EqualTo(RequestResultType.Success));
			Assert.That(result.HttpStatusCode, Is.EqualTo(200));
			Assert.That(result.ErrorResponse, Is.Null);
		}
		
		[UnityTest]
		public IEnumerator 異常系()
		{
			var applicationKey = EditorUserSettings.GetConfigValue("MinolyApplicationKey");
			var clientKey = EditorUserSettings.GetConfigValue("MinolyClientKey");
			var objectDeleter = new ObjectDeleter(applicationKey, clientKey);

			yield return objectDeleter.DeleteAsync(ClassName, "Detarame");
			var result = objectDeleter.GetResult();
			Assert.That(result.Type, Is.EqualTo(RequestResultType.ProtocolError));
			Assert.That(result.HttpStatusCode, Is.EqualTo(404));
			Assert.That(result.ErrorResponse.code, Is.EqualTo("E404001"));
			Assert.That(result.ErrorResponse.error, Is.EqualTo("No data available."));
		}
		
		[UnityTest]
		public IEnumerator UniTaskによるDelete() => UniTask.ToCoroutine(async () =>
		{
			var applicationKey = EditorUserSettings.GetConfigValue("MinolyApplicationKey");
			var clientKey = EditorUserSettings.GetConfigValue("MinolyClientKey");
			var objectPostman = new ObjectPostman(applicationKey, clientKey);
			var objectDeleter = new ObjectDeleter(applicationKey, clientKey);
			var postResult = await objectPostman.PostTask(ClassName, ContentInJson);
			var deleteResult = await objectDeleter.DeleteTask(ClassName, postResult.ObjectId);
			Assert.That(deleteResult.Type, Is.EqualTo(RequestResultType.Success));
			Assert.That(deleteResult.HttpStatusCode, Is.EqualTo(200));
		});

	}
}