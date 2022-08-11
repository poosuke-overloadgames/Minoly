using System;
using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using Minoly;
using Minoly.UniTask;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.TestTools;

namespace Tests
{
	[TestFixture]
	public class TestGetObject
	{
		private const string ClassName = "TestClass";
		private const string UserName = "aaa";
		private const int Score = 100;

		[UnityTest]
		public IEnumerator 正常系()
		{
			var applicationKey = EditorUserSettings.GetConfigValue("MinolyApplicationKey");
			var clientKey = EditorUserSettings.GetConfigValue("MinolyClientKey");
			var objectId = EditorUserSettings.GetConfigValue("MinolyObjectId");
			var objectGetter = new ObjectGetter(applicationKey, clientKey);
			yield return objectGetter.FetchAsync(ClassName, objectId);
			var result = objectGetter.GetResult();
			Assert.That(result.Type, Is.EqualTo(RequestResultType.Success));
			Assert.That(result.HttpStatusCode, Is.EqualTo(200));
			Assert.That(result.ErrorResponse, Is.Null);
			var testClass = JsonUtility.FromJson<TestClass>(result.Body);
			Assert.That(testClass.userName, Is.EqualTo(UserName));
			Assert.That(testClass.score, Is.EqualTo(Score));
		}

		[UnityTest]
		public IEnumerator 異常系_clientKey間違い()
		{
			var applicationKey = EditorUserSettings.GetConfigValue("MinolyApplicationKey");
			var clientKey = "Detarame";
			var objectId = EditorUserSettings.GetConfigValue("MinolyObjectId");
			var objectGetter = new ObjectGetter(applicationKey, clientKey);
			yield return objectGetter.FetchAsync(ClassName, objectId);
			var result = objectGetter.GetResult();
			Assert.That(result.Type, Is.EqualTo(RequestResultType.ProtocolError));
			Assert.That(result.HttpStatusCode, Is.EqualTo(403));
			Assert.That(result.ErrorResponse.code, Is.EqualTo("E403002"));
			Assert.That(result.ErrorResponse.error, Is.EqualTo("Unauthorized operations for signature."));
		}

		[UnityTest]
		public IEnumerator 異常系_多重取得()
		{
			var applicationKey = EditorUserSettings.GetConfigValue("MinolyApplicationKey");
			var clientKey = EditorUserSettings.GetConfigValue("MinolyClientKey");
			var objectId = EditorUserSettings.GetConfigValue("MinolyObjectId");
			var objectGetter = new ObjectGetter(applicationKey, clientKey);
			var op1 = objectGetter.FetchAsync(ClassName, objectId);
			UnityWebRequestAsyncOperation op2 = null;
			var caught = false;
			try
			{
				//objectGetterを新しく作る必要がある。
				op2 = objectGetter.FetchAsync(ClassName, objectId);
			}
			catch (MinolyInProgressException)
			{
				caught = true;
			}

			yield return op1;
			yield return op2;
			Assert.That(caught, Is.True);
		}

		[UnityTest]
		public IEnumerator UniTaskによるFetch() => UniTask.ToCoroutine(async () =>
		{
			var applicationKey = EditorUserSettings.GetConfigValue("MinolyApplicationKey");
			var clientKey = EditorUserSettings.GetConfigValue("MinolyClientKey");
			var objectId = EditorUserSettings.GetConfigValue("MinolyObjectId");
			var objectGetter = new ObjectGetter(applicationKey, clientKey);
			var result = await objectGetter.FetchTask(ClassName, objectId);
			Assert.That(result.Type, Is.EqualTo(RequestResultType.Success));
			Assert.That(result.HttpStatusCode, Is.EqualTo(200));
			Assert.That(result.ErrorResponse, Is.Null);
			var testClass = JsonUtility.FromJson<TestClass>(result.Body);
			Assert.That(testClass.userName, Is.EqualTo(UserName));
			Assert.That(testClass.score, Is.EqualTo(Score));

		});

		[UnityTest]
		public IEnumerator UniTaskによるFetchをキャンセル() => UniTask.ToCoroutine(async () =>
		{
			var applicationKey = EditorUserSettings.GetConfigValue("MinolyApplicationKey");
			var clientKey = EditorUserSettings.GetConfigValue("MinolyClientKey");
			var objectId = EditorUserSettings.GetConfigValue("MinolyObjectId");
			var cts = new CancellationTokenSource();
			var objectGetter = new ObjectGetter(applicationKey, clientKey);
			var task = objectGetter.FetchTask(ClassName, objectId, null, PlayerLoopTiming.Update, cts.Token);
			cts.Cancel();
			try
			{
				await task;
			}
			catch(OperationCanceledException)
			{
				//なぜかこの時点ではInProgressもある。
				//OperationCanceledExceptionを投げるタイミングとキャンセル処理が終わるタイミングがリンクしてないっぽい
				Assert.That(objectGetter.GetResult().Type, Is.EqualTo(RequestResultType.InProgress).Or.EqualTo(RequestResultType.Aborted));
				await UniTask.Yield();
			}
			var result = objectGetter.GetResult();
			Assert.That(result.Type, Is.EqualTo(RequestResultType.Aborted));
		});
	}
}