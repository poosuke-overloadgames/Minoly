using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Minoly;
using Minoly.UniTask;
using NUnit.Framework;
using UnityEditor;
using UnityEngine.TestTools;

namespace Tests
{
	[TestFixture]
	public class TestObjectFinderQuery
	{
		private const string ClassName = "TestClass";
		private async UniTask<string> FindAsync(IEnumerable<IQuery> queries)
		{
			var applicationKey = EditorUserSettings.GetConfigValue("MinolyApplicationKey");
			var clientKey = EditorUserSettings.GetConfigValue("MinolyClientKey");
			var objectFinder = new ObjectFinder(applicationKey, clientKey);
			var result = await objectFinder.FindTask(ClassName, queries);
			Assert.That(result.Type, Is.EqualTo(RequestResultType.Success));
			Assert.That(result.HttpStatusCode, Is.EqualTo(200));
			Assert.That(result.ErrorResponse, Is.Null);
			return result.Body;
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
	}
}