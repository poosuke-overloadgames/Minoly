using System;
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
	public class TestObjectDeleter
	{
		private const string ClassName = "TestClass";
		private const string ContentInJson = "{\"userName\": \"ccc\", \"score\": \"300\"}";
		private readonly List<string> _disposableObjectIds = new List<string>();
		private ObjectDeleter _objectDeleter;
		private ObjectPostman _objectPostman;

		[SetUp]
		public void SetUp()
		{
			_disposableObjectIds.Clear();
			var applicationKey = EditorUserSettings.GetConfigValue("MinolyApplicationKey");
			var clientKey = EditorUserSettings.GetConfigValue("MinolyClientKey");
			_objectDeleter = new ObjectDeleter(applicationKey, clientKey);
			_objectPostman = new ObjectPostman(applicationKey, clientKey);
		}

		[UnityTearDown]
		public IEnumerator TearDown()
		{
			foreach (var objectId in _disposableObjectIds)
			{
				yield return _objectDeleter.DeleteAsync(ClassName, objectId);
			}
			_objectDeleter.Dispose();
			_objectPostman.Dispose();
		}

		[UnityTest]
		public IEnumerator 正常系()
		{
			yield return _objectPostman.PostAsync(ClassName, ContentInJson);
			var postResult = _objectPostman.GetResult();
			Assert.That(postResult.Type, Is.EqualTo(RequestResultType.Success));
			_disposableObjectIds.Add(postResult.ObjectId);

			yield return _objectDeleter.DeleteAsync(ClassName, postResult.ObjectId);
			var result = _objectDeleter.GetResult();
			Assert.That(result.Type, Is.EqualTo(RequestResultType.Success));
			Assert.That(result.HttpStatusCode, Is.EqualTo(200));
			Assert.That(result.ErrorResponse, Is.Null);
			_disposableObjectIds.Clear();
		}
		
		[UnityTest]
		public IEnumerator 異常系()
		{
			yield return _objectDeleter.DeleteAsync(ClassName, "Detarame");
			var result = _objectDeleter.GetResult();
			Assert.That(result.Type, Is.EqualTo(RequestResultType.ProtocolError));
			Assert.That(result.HttpStatusCode, Is.EqualTo(404));
			Assert.That(result.ErrorResponse.code, Is.EqualTo("E404001"));
			Assert.That(result.ErrorResponse.error, Is.EqualTo("No data available."));
		}
		
		[UnityTest]
		public IEnumerator UniTaskによるDelete() => UniTask.ToCoroutine(async () =>
		{
			var postResult = await _objectPostman.PostTask(ClassName, ContentInJson);
			_disposableObjectIds.Add(postResult.ObjectId);
			var deleteResult = await _objectDeleter.DeleteTask(ClassName, postResult.ObjectId);
			Assert.That(deleteResult.Type, Is.EqualTo(RequestResultType.Success));
			Assert.That(deleteResult.HttpStatusCode, Is.EqualTo(200));
			_disposableObjectIds.Clear();
		});

	}
}