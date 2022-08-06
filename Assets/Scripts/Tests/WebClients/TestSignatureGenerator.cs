using System;
using System.Collections.Generic;
using Minoly;
using Minoly.Types;
using NUnit.Framework;

namespace Tests
{
	[TestFixture]
	public class TestSignatureGenerator
	{
		//ClientKeyは以下公式のもの
		//https://mbaas.nifcloud.com/doc/current/rest/common/signature.html
		private const string SignatureSrc =
			"GET\n" +
			"mbaas.api.nifcloud.com\n" +
			"/2013-09-01/classes/TestClass\n" +
			"SignatureMethod=HmacSHA256&SignatureVersion=2&X-NCMB-Application-Key=6145f91061916580c742f806bab67649d10f45920246ff459404c46f00ff3e56&X-NCMB-Timestamp=2013-12-02T02:44:35.452Z&where=%7B%22testKey%22%3A%22testValue%22%7D";
		private const string Signature = "AltGkQgXurEV7u0qMd+87ud7BKuueldoCjaMgVc9Bes=";
		private const string ClientKey = "1343d198b510a0315db1c03f3aa0e32418b7a743f8e4b47cbff670601345cf75";
		private const string ApplicationKey = "6145f91061916580c742f806bab67649d10f45920246ff459404c46f00ff3e56";
		
		[TestCase(SignatureSrc, ClientKey, Signature)]
		public void シグネチャ生成(string src, string clientKey, string expected)
		{
			var generator = new SignatureGenerator();
			Assert.That(generator.Generate(src, clientKey), Is.EqualTo(expected));
		}
		
		[Test]
		public void 各種パラメータからシグネチャ生成()
		{
			var generator = new SignatureGenerator();
			var method = RequestMethod.Get;
			var uri = new Uri("https://mbaas.api.nifcloud.com/2013-09-01/classes/TestClass");
			var timeStamp = new Timestamp(new DateTime(2013, 12, 2, 2, 44, 35, 452));
			var queries = new List<GetQuery> { new GetQuery("testKey", "testValue") };
			Assert.That(generator.Generate(method, uri, ApplicationKey, ClientKey, queries, timeStamp), Is.EqualTo(Signature));
		}
	}
}