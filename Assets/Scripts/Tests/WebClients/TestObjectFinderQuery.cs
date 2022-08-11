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
	public class TestObjectFinderQuery
	{
		private const string ClassName = "TestClass";
		private ObjectFinder _objectFinder;
		private ObjectPostman _objectPostman;
		private ObjectDeleter _objectDeleter;
		private async UniTask<string> FindAsync(IEnumerable<IQuery> queries)
		{
			var result = await _objectFinder.FindTask(ClassName, queries);
			Assert.That(result.Type, Is.EqualTo(RequestResultType.Success));
			Assert.That(result.HttpStatusCode, Is.EqualTo(200));
			Assert.That(result.ErrorResponse, Is.Null);
			return result.Body;
		}

		private async UniTask<string> PostAsync(TestClassForPost testClass)
		{
			var json = JsonUtility.ToJson(testClass);
			var result = await _objectPostman.PostTask(ClassName, json);
			Assert.That(result.Type, Is.EqualTo(RequestResultType.Success));
			Assert.That(result.HttpStatusCode, Is.EqualTo(201));
			Assert.That(result.ErrorResponse, Is.Null);
			return result.ObjectId;
		}

		private async UniTask DeleteAsync(string objectId)
		{
			var result = await _objectDeleter.DeleteTask(ClassName, objectId);
			Assert.That(result.Type, Is.EqualTo(RequestResultType.Success));
			Assert.That(result.HttpStatusCode, Is.EqualTo(200));
			Assert.That(result.ErrorResponse, Is.Null);
		}

		[SetUp]
		public void Init()
		{
			var applicationKey = EditorUserSettings.GetConfigValue("MinolyApplicationKey");
			var clientKey = EditorUserSettings.GetConfigValue("MinolyClientKey");
			_objectFinder = new ObjectFinder(applicationKey, clientKey);
			_objectPostman = new ObjectPostman(applicationKey, clientKey);
			_objectDeleter = new ObjectDeleter(applicationKey, clientKey);
		}

		[UnityTest]
		public IEnumerator クエリ重複エラー() => UniTask.ToCoroutine(async () =>
		{
			try
			{
				await FindAsync(new[]
				{
					new QueryWhereEqualTo("userName", "aaa"),
					new QueryWhereEqualTo("score", "100"),
				});
			}
			catch (MinolyDuplicateQueryException)
			{
				Assert.Pass();
				return;
			}
			Assert.Fail();
		});

		[UnityTest]
		public IEnumerator LimitSkipWhere複合テスト() => UniTask.ToCoroutine(async () =>
		{
			var a2 = await PostAsync(new TestClassForPost { userName = "aaa", score = 200 });
			var a3 = await PostAsync(new TestClassForPost { userName = "aaa", score = 300 });
			var a0 = await PostAsync(new TestClassForPost { userName = "aaa", score = 0 });
			var b1 = await PostAsync(new TestClassForPost { userName = "bbb", score = 150 });
			var body = await FindAsync(new IQuery[]
			{
				new QueryWhereEqualTo("userName", "aaa"),
				new QueryLimit(2),
				new QuerySkip(1),
				new QueryOrder("score", false)
			});
			var users = JsonUtility.FromJson<FoundTestClass>(body).results;
			
			Assert.That(users.Length, Is.EqualTo(2));
			Assert.That(users[0].userName, Is.EqualTo("aaa"));
			Assert.That(users[0].score, Is.EqualTo(200));
			Assert.That(users[1].userName, Is.EqualTo("aaa"));
			Assert.That(users[1].score, Is.EqualTo(100));

			await DeleteAsync(a2);
			await DeleteAsync(a3);
			await DeleteAsync(a0);
			await DeleteAsync(b1);
		});
	}
}