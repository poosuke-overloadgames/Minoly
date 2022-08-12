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
	public class TestMinolyDataStore
	{
		private const string ClassName = "TestClass";
		private readonly List<string> _objectIds = new List<string>();
		private MinolyDataStore _dataStore;

		[UnitySetUp]
		public IEnumerator SetUp()
		{
			var applicationKey = EditorUserSettings.GetConfigValue("MinolyApplicationKey");
			var clientKey = EditorUserSettings.GetConfigValue("MinolyClientKey");
			_dataStore = new MinolyDataStore(applicationKey, clientKey);
			_objectIds.Clear();
			var posts = new[]
			{
				new TestClassToPost { userName = "aaa", score = 100 },
				new TestClassToPost { userName = "aaa", score = 200 },
				new TestClassToPost { userName = "bbb", score = 100 },
			};
			var tasks = posts.Select(p => _dataStore.PostAsync(ClassName, JsonUtility.ToJson(p))).ToArray();
			var whenAll = UniTask.WhenAll(tasks).ContinueWith(results =>
			{
				_objectIds.AddRange(results.Select(r => r.ObjectId));
			});
			yield return whenAll.ToCoroutine();
		}

		[UnityTearDown]
		public IEnumerator TearDown()
		{
			var tasks = _objectIds.Select(id => _dataStore.DeleteAsync(ClassName, id)).ToArray();
			yield return UniTask.WhenAll(tasks).ToCoroutine();
		}

		[UnityTest]
		public IEnumerator オブジェクトの取得成功() => UniTask.ToCoroutine(async () =>
		{
			var result = await _dataStore.FetchAsync(ClassName, _objectIds[0]);
			Assert.That(result.Type, Is.EqualTo(RequestResultType.Success));
			Assert.That(result.HttpStatusCode, Is.EqualTo(200));
			Assert.That(result.ErrorResponse, Is.Null);
			var fetched = JsonUtility.FromJson<TestClass>(result.Body);
			Assert.That(fetched.userName, Is.EqualTo("aaa"));
			Assert.That(fetched.score, Is.EqualTo(100));
		});

		[UnityTest]
		public IEnumerator オブジェクトの取得失敗() => UniTask.ToCoroutine(async () =>
		{
			var result = await _dataStore.FetchAsync(ClassName, "Detarame");
			Assert.That(result.Type, Is.EqualTo(RequestResultType.ProtocolError));
			Assert.That(result.HttpStatusCode, Is.EqualTo(404));
			Assert.That(result.ErrorResponse.code, Is.EqualTo("E404001"));
			Assert.That(result.ErrorResponse.error, Is.EqualTo("No data available."));
			Assert.That(result.Body, Is.EqualTo("{\"code\":\"E404001\",\"error\":\"No data available.\"}"));
		});
		
		[UnityTest]
		public IEnumerator オブジェクトの登録削除成功() => UniTask.ToCoroutine(async () =>
		{
			var json = JsonUtility.ToJson(new TestClassToPost { userName = "ccc", score = 300 });
			var resultPost = await _dataStore.PostAsync(ClassName, json);
			Assert.That(resultPost.Type, Is.EqualTo(RequestResultType.Success));
			Assert.That(resultPost.HttpStatusCode, Is.EqualTo(201));
			Assert.That(resultPost.ErrorResponse, Is.Null);
			var objectId = resultPost.ObjectId;
			var resultDelete = await _dataStore.DeleteAsync(ClassName, objectId);
			Assert.That(resultDelete.Type, Is.EqualTo(RequestResultType.Success));
			Assert.That(resultDelete.HttpStatusCode, Is.EqualTo(200));
			Assert.That(resultDelete.ErrorResponse, Is.Null);
		});
		
		[UnityTest]
		public IEnumerator オブジェクトの登録失敗() => UniTask.ToCoroutine(async () =>
		{
			var result = await _dataStore.PostAsync(ClassName, "Detarame");
			Assert.That(result.Type, Is.EqualTo(RequestResultType.ProtocolError));
			Assert.That(result.HttpStatusCode, Is.EqualTo(400));
			Assert.That(result.ErrorResponse.code, Is.EqualTo("E400001"));
			Assert.That(result.ErrorResponse.error, Is.EqualTo("JSON is invalid format."));
		});
		
		[UnityTest]
		public IEnumerator オブジェクトの削除失敗() => UniTask.ToCoroutine(async () =>
		{
			var result = await _dataStore.DeleteAsync(ClassName, "Detarame");
			Assert.That(result.Type, Is.EqualTo(RequestResultType.ProtocolError));
			Assert.That(result.HttpStatusCode, Is.EqualTo(404));
			Assert.That(result.ErrorResponse.code, Is.EqualTo("E404001"));
			Assert.That(result.ErrorResponse.error, Is.EqualTo("No data available."));
		});

	}
}