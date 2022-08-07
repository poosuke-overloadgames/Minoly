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

		[UnityTest]
		public IEnumerator 正常系()
		{
			var applicationKey = EditorUserSettings.GetConfigValue("MinolyApplicationKey");
			var clientKey = EditorUserSettings.GetConfigValue("MinolyClientKey");
			var objectFinder = new ObjectFinder(applicationKey, clientKey);
			yield return objectFinder.FindAsync(ClassName, new GetQuery("userName", "aaa"));
			var result = objectFinder.GetResult();
			Assert.That(result.Type, Is.EqualTo(RequestResultType.Success));
			Assert.That(result.HttpStatusCode, Is.EqualTo(200));
			Assert.That(result.ErrorResponse, Is.Null);
		}

	}
}