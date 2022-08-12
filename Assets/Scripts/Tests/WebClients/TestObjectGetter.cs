using System;
using System.Collections;
using System.Collections.Generic;
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
	public class TestObjectGetter
	{
		private const string ClassName = "TestClass";
		private const string ContentInJson = "{\"userName\": \"aaa\", \"score\": \"100\"}";
		private const string UserName = "aaa";
		private const int Score = 100;
		private ObjectGetter _objectGetter;
		private ObjectPostman _objectPostman;
		private ObjectDeleter _objectDeleter;
		private string _objectId;
		
		[UnitySetUp]
		public IEnumerator SetUp()
		{
			var applicationKey = EditorUserSettings.GetConfigValue("MinolyApplicationKey");
			var clientKey = EditorUserSettings.GetConfigValue("MinolyClientKey");
			_objectGetter = new ObjectGetter(applicationKey, clientKey);
			_objectPostman = new ObjectPostman(applicationKey, clientKey);
			_objectDeleter = new ObjectDeleter(applicationKey, clientKey);
			yield return _objectPostman.PostAsync(ClassName, ContentInJson);
			_objectId = _objectPostman.GetResult().ObjectId;
		}

		[UnityTearDown]
		public IEnumerator TearDown()
		{
			yield return _objectDeleter.DeleteAsync(ClassName, _objectId);
			_objectDeleter.Dispose();
			_objectGetter.Dispose();
			_objectPostman.Dispose();
		}

		[UnityTest]
		public IEnumerator 正常系()
		{
			yield return _objectGetter.FetchAsync(ClassName, _objectId);
			var result = _objectGetter.GetResult();
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
			var objectGetter = new ObjectGetter(applicationKey, clientKey);
			yield return objectGetter.FetchAsync(ClassName, _objectId);
			var result = objectGetter.GetResult();
			Assert.That(result.Type, Is.EqualTo(RequestResultType.ProtocolError));
			Assert.That(result.HttpStatusCode, Is.EqualTo(403));
			Assert.That(result.ErrorResponse.code, Is.EqualTo("E403002"));
			Assert.That(result.ErrorResponse.error, Is.EqualTo("Unauthorized operations for signature."));
			objectGetter.Dispose();
		}

		[UnityTest]
		public IEnumerator 異常系_多重取得()
		{
			var op1 = _objectGetter.FetchAsync(ClassName, _objectId);
			UnityWebRequestAsyncOperation op2 = null;
			var caught = false;
			try
			{
				//objectGetterを新しく作る必要がある。
				op2 = _objectGetter.FetchAsync(ClassName, _objectId);
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
			var result = await _objectGetter.FetchTask(ClassName, _objectId);
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
			var cts = new CancellationTokenSource();
			var task = _objectGetter.FetchTask(ClassName, _objectId, null, PlayerLoopTiming.Update, cts.Token);
			cts.Cancel();
			try
			{
				await task;
			}
			catch(OperationCanceledException)
			{
				//なぜかこの時点ではInProgressもある。
				//OperationCanceledExceptionを投げるタイミングとキャンセル処理が終わるタイミングがリンクしてないっぽい
				while (RequestResultType.InProgress == _objectGetter.GetResult().Type)
				{
					await UniTask.Yield();
				}
			}
			var result = _objectGetter.GetResult();
			Assert.That(result.Type, Is.EqualTo(RequestResultType.Aborted));
		});
	}
}