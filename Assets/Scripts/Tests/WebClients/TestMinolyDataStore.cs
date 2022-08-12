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
		
		[UnityTest]
		public IEnumerator オブジェクトの更新成功() => UniTask.ToCoroutine(async () =>
		{
			var testClass = new TestClassToPost { userName = "ccc", score = 100 };
			var resultPost = await _dataStore.PostAsync(ClassName, JsonUtility.ToJson(testClass));
			var objectId = resultPost.ObjectId;
			_objectIds.Add(resultPost.ObjectId);
			Assert.That(resultPost.Type, Is.EqualTo(RequestResultType.Success));

			testClass.score = 300;
			var resultUpdate = await _dataStore.UpdateAsync(ClassName, objectId, JsonUtility.ToJson(testClass));
			Assert.That(resultUpdate.Type, Is.EqualTo(RequestResultType.Success));
			Assert.That(resultUpdate.HttpStatusCode, Is.EqualTo(200));
			Assert.That(resultUpdate.ErrorResponse, Is.Null);

			var resultGet = await _dataStore.FetchAsync(ClassName, objectId);
			var fetched = JsonUtility.FromJson<TestClass>(resultGet.Body);
			Assert.That(fetched.userName, Is.EqualTo("ccc"));
			Assert.That(fetched.score, Is.EqualTo(300));
		});

		[UnityTest]
		public IEnumerator オブジェクトの更新失敗() => UniTask.ToCoroutine(async () =>
		{
			var testClass = new TestClassToPost { userName = "ccc", score = 100 };
			var resultUpdate = await _dataStore.UpdateAsync(ClassName, "Detarame", JsonUtility.ToJson(testClass));
			Assert.That(resultUpdate.Type, Is.EqualTo(RequestResultType.ProtocolError));
			Assert.That(resultUpdate.HttpStatusCode, Is.EqualTo(404));
			Assert.That(resultUpdate.ErrorResponse.code, Is.EqualTo("E404001"));
			Assert.That(resultUpdate.ErrorResponse.error, Is.EqualTo("No data available."));
		});

		[UnityTest]
		public IEnumerator オブジェクトの検索成功() => UniTask.ToCoroutine(async () =>
		{
			var query = QueryWhere.Create(new WhereAnd(new IWhereCondition[]
			{
				new WhereEqualTo("userName", "aaa"),
				new WhereEqualTo("score", 100),
			}));
			var result = await _dataStore.FindAsync(ClassName, new[] { query });
			Assert.That(result.Type, Is.EqualTo(RequestResultType.Success));
			Assert.That(result.HttpStatusCode, Is.EqualTo(200));
			Assert.That(result.ErrorResponse, Is.Null);
			var testClass = JsonUtility.FromJson<FoundTestClass>(result.Body).results[0];
			Assert.That(testClass.userName, Is.EqualTo("aaa"));
			Assert.That(testClass.score, Is.EqualTo(100));
		});

		[UnityTest]
		public IEnumerator オブジェクトの検索成功0件() => UniTask.ToCoroutine(async () =>
		{
			var query = QueryWhere.Create(new WhereAnd(new IWhereCondition[]
			{
				new WhereEqualTo("userName", "ccc"),
			}));
			var result = await _dataStore.FindAsync(ClassName, new[] { query });
			Assert.That(result.Type, Is.EqualTo(RequestResultType.Success));
			Assert.That(result.HttpStatusCode, Is.EqualTo(200));
			Assert.That(result.ErrorResponse, Is.Null);
			var testClasses = JsonUtility.FromJson<FoundTestClass>(result.Body).results;
			Assert.That(testClasses.Length, Is.EqualTo(0));
		});

		[UnityTest]
		public IEnumerator オブジェクトの検索失敗() => UniTask.ToCoroutine(async () =>
		{
			var applicationKey = "Detarame";
			var clientKey = EditorUserSettings.GetConfigValue("MinolyClientKey");
			var dataStore = new MinolyDataStore(applicationKey, clientKey);
			var query = QueryWhere.Create(new WhereAnd(new IWhereCondition[]
			{
				new WhereEqualTo("userName", "aaa"),
				new WhereEqualTo("score", 100),
			}));
			var result = await dataStore.FindAsync(ClassName, new[] { query });
			Assert.That(result.Type, Is.EqualTo(RequestResultType.ProtocolError));
			Assert.That(result.HttpStatusCode, Is.EqualTo(404));
			Assert.That(result.ErrorResponse.code, Is.EqualTo("E404005"));
			Assert.That(result.ErrorResponse.error, Is.EqualTo("No such application."));
		});
	}
}