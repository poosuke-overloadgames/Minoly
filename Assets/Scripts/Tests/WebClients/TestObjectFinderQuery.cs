using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Minoly;
using Minoly.ApiTypes;
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

		private async UniTask<string> PostAsync(TestClassToPost testClass)
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
			var a2 = await PostAsync(new TestClassToPost { userName = "aaa", score = 200 });
			var a3 = await PostAsync(new TestClassToPost { userName = "aaa", score = 300 });
			var a0 = await PostAsync(new TestClassToPost { userName = "aaa", score = 0 });
			var b1 = await PostAsync(new TestClassToPost { userName = "bbb", score = 150 });
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
		
		[UnityTest]
		public IEnumerator WhereEqualToテスト() => UniTask.ToCoroutine(async () =>
		{
			var body = await FindAsync(new IQuery[]
			{
				QueryWhere.Create(new WhereEqualTo("userName", "aaa"))
			});
			var users = JsonUtility.FromJson<FoundTestClass>(body).results;
			Assert.That(users.Length, Is.EqualTo(1));
			Assert.That(users[0].userName, Is.EqualTo("aaa"));
			Assert.That(users[0].score, Is.EqualTo(100));
		});
		
		[UnityTest]
		public IEnumerator WhereGreaterThanテスト() => UniTask.ToCoroutine(async () =>
		{
			var b = await PostAsync(new TestClassToPost
			{
				userName = "bbb", 
				score = 300, 
				dateTime = new ApiDateTime(new DateTime(2022,8,3))
			});
			var c = await PostAsync(new TestClassToPost
			{
				userName = "ccc",
				score = 200,
				dateTime = new ApiDateTime(new DateTime(2022, 8, 2))
			});
			var d = await PostAsync(new TestClassToPost
			{
				userName = "ddd", 
				score = 400,
				dateTime = new ApiDateTime(new DateTime(2022,8,1))
			});
			
			
			
			var body = await FindAsync(new IQuery[]
			{
				QueryWhere.Create(new WhereGreaterThan("score", 300))
			});
			var users = JsonUtility.FromJson<FoundTestClass>(body).results;
			Assert.That(users.Select(u => u.userName), Is.EquivalentTo(new[] { "bbb", "ddd" }));

			
			
			body = await FindAsync(new IQuery[]
			{
				QueryWhere.Create(new WhereGreaterThan("score", 300, false))
			});
			users = JsonUtility.FromJson<FoundTestClass>(body).results;
			Assert.That(users.Select(u => u.userName), Is.EquivalentTo(new[] { "ddd" }));

			
			
			body = await FindAsync(new IQuery[]
			{
				QueryWhere.Create(new WhereGreaterThan("userName", "ccc"))
			});
			users = JsonUtility.FromJson<FoundTestClass>(body).results;
			Assert.That(users.Select(u => u.userName), Is.EquivalentTo(new[] { "ccc", "ddd" }));

			

			body = await FindAsync(new IQuery[]
			{
				QueryWhere.Create(new WhereGreaterThan("dateTime", new DateTime(2022,8,2)))
			});
			users = JsonUtility.FromJson<FoundTestClass>(body).results;
			Assert.That(users.Select(u => u.userName), Is.EquivalentTo(new[] { "bbb", "ccc" }));

			await DeleteAsync(b);
			await DeleteAsync(c);
			await DeleteAsync(d);

		});

		[UnityTest]
		public IEnumerator WhereAndOrテスト() => UniTask.ToCoroutine(async () =>
		{
			var a2 = await PostAsync(new TestClassToPost { userName = "aaa", score = 200 });
			var b1 = await PostAsync(new TestClassToPost { userName = "bbb", score = 100 });
			var b2 = await PostAsync(new TestClassToPost { userName = "bbb", score = 200 });
			var body = await FindAsync(new IQuery[]
			{
				QueryWhere.Create(new WhereAnd(new IWhereCondition[]
					{
						new WhereEqualTo("userName", "aaa"),
						new WhereLessThan("score", 150)
					}
				))
			});
			var users = JsonUtility.FromJson<FoundTestClass>(body).results;
			Assert.That(users.Select(u => u.userName), Is.EquivalentTo(new[] { "aaa" }));
			
			body = await FindAsync(new IQuery[]
			{
				QueryWhere.Create(new WhereOr(new IWhereCondition[]
					{
						new WhereEqualTo("userName", "aaa"),
						new WhereLessThan("score", 150)
					}
				))
			});
			users = JsonUtility.FromJson<FoundTestClass>(body).results;
			Assert.That(users.Select(u => u.userName), Is.EquivalentTo(new[] { "aaa", "aaa", "bbb" }));
			
			await DeleteAsync(a2);
			await DeleteAsync(b1);
			await DeleteAsync(b2);
		});

		[UnityTest]
		public IEnumerator WhereInRangeテスト() => UniTask.ToCoroutine(async () =>
		{
			var a0 = await PostAsync(new TestClassToPost { userName = "aaa", score = 0 });
			var a2 = await PostAsync(new TestClassToPost { userName = "aaa", score = 200 });
			var body = await FindAsync(new IQuery[]
			{
				QueryWhere.Create(new WhereAnd(new IWhereCondition[]
					{
						new WhereInRange("score", 50, 150)
					}
				))
			});
			var users = JsonUtility.FromJson<FoundTestClass>(body).results;
			Assert.That(users.Select(u => u.score), Is.EquivalentTo(new[] { 100 }));
			
			await DeleteAsync(a0);
			await DeleteAsync(a2);
		});
	}
}