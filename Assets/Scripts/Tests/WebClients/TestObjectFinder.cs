using System;
using System.Collections;
using Minoly;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.TestTools;

namespace Tests
{
	[TestFixture]
	public class TestObjectFinder
	{
		private const string ClassName = "TestClass";

		private int CountOf(string src, string word)
		{
			var count = 0;
			var index = 0;
			while (true)
			{
				index = src.IndexOf(word, index + 1, StringComparison.Ordinal);
				if (-1 == index) return count;
				count++;
			}
		}


		[UnityTest]
		public IEnumerator 正常系()
		{
			var applicationKey = EditorUserSettings.GetConfigValue("MinolyApplicationKey");
			var clientKey = EditorUserSettings.GetConfigValue("MinolyClientKey");
			var objectFinder = new ObjectFinder(applicationKey, clientKey);
			yield return objectFinder.FindAsync(ClassName, new IQuery[]
			{
				new QueryWhereEqualTo("userName", "aaa"), 
				new QueryLimit(2)
			});
			var result = objectFinder.GetResult();
			Assert.That(result.Type, Is.EqualTo(RequestResultType.Success));
			Assert.That(result.HttpStatusCode, Is.EqualTo(200));
			Assert.That(result.ErrorResponse, Is.Null);
			Assert.That(CountOf(result.Body, "objectId"), Is.EqualTo(2));
		}

	}
}