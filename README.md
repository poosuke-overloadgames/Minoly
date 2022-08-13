# Minoly
![Release](https://img.shields.io/badge/dynamic/json.svg?label=version&query=$.version&uri=https%3A%2F%2Fraw.githubusercontent.com%2Fpoosuke-overloadgames%2FMinoly%2Fmaster%2FAssets%2FScripts%2FRuntime%2Fpackage.json)
![Unity](https://img.shields.io/badge/dynamic/json.svg?label=Unity&query=$.unity&uri=https%3A%2F%2Fraw.githubusercontent.com%2Fpoosuke-overloadgames%2FMinoly%2Fmaster%2FAssets%2FScripts%2FRuntime%2Fpackage.json&suffix=%2B%28%E5%A4%9A%E5%88%86%29)
![License](https://img.shields.io/badge/license-MIT-red)

Unity C#からNifcloud mobile backendのデータストアに接続するライブラリです。

 - コルーチンでアクセスを待てます。
 - UniTaskにも対応しています。
 - MonoBehaviour不使用。
 - ニフクラ mobile backend Unity SDK不使用（一からコード書いた）。
 
## インストール
Window > PackageManager > + > Add package from git URL...<br>
から<br>
`https://github.com/poosuke-overloadgames/Minoly.git?path=Assets/Scripts/Runtime`<br>
を入力してください。<br>
<br>
あるいは直接<br>
`Packages/manifest.json`<br>
のdependencies内に<br>
`"com.poosuke.minoly": "https://github.com/poosuke-overloadgames/Minoly.git?path=Assets/Scripts/Runtime",`<br>
を追加してください。<br>

## クイックスタート
### コルーチンで使用する場合
```
public IEnumerator FetchObject(string className, string objectId)
{
	var dataStore = new MinolyDataStore("<アプリケーションキー>", "<クライアントキー>");
	var getter = dataStore.CreateGetter();
	yield return getter.FetchAsync(className, objectId);
	ObjectGetResult result = getter.GetResult(); //取得結果・内容が入ります。
	getter.Dispose();
}
```
### UniTaskがある場合
```
public UniTask<ObjectGetResult> FetchObjectAsync(string className, string objectId)
{
	var dataStore = new MinolyDataStore("<アプリケーションキー>", "<クライアントキー>");
	return dataStore.FetchAsync(className, objectId);
}
```

## 初期化
まず、MinolyDataStoreを作ります。
```
var dataStore = new MinolyDataStore("<アプリケーションキー>", "<クライアントキー>");
```

## オブジェクトの取得
### コルーチンで取得
```
//ObjectGetterを取得します。
ObjectGetter getter = dataStore.CreateGetter();

//取得できるまで待ちます。
//ObjectGetter.FetchAsyncはUnityWebRequestAsyncOperationを返します。
yield return getter.FetchAsync(className, objectId);

//ObjectGetResultは後述。
ObjectGetResult result = getter.GetResult();

//使い終わったら忘れずにDisposeします。
getter.Dispose();
```
 - ObjectGetterの使用制限<br>
繰り返し使用可能ですが、同時並行で取得させることはできません。例えば以下の場合は例外を投げます。
```
var op1 = getter.FetchAsync(className, objectId1);
var op2 = getter.FetchAsync(className, objectId2); //MinolyInProgressExceptionを投げる
yield return op1;
yield return op2;
```
この時は以下のようにします。
```
ObjectGetter getter1 = dataStore.CreateGetter();
ObjectGetter getter2 = dataStore.CreateGetter();
var op1 = getter1.FetchAsync(className, objectId1);
var op2 = getter2.FetchAsync(className, objectId2);
yield return op1;
yield return op2;
```
また、同一のObjectGetterを繰り返し使用した場合、GetResultは常に最新のものを返します。
これら使用制限は他(ObjectPostman,ObjectUpdater,ObjectDeleter,ObjectFinder)でも同じです。

### UniTaskで取得
```
//オブジェクトの取得
ObjectGetResult result = await dataStore.FetchAsync(className, objectId);
//同時に取得
ObjectGetResult[] results = await UniTask.WhenAll(new[]
{
	dataStore.FetchAsync(className, objectId1),
	dataStore.FetchAsync(className, objectId2),
});
```
Disposeの必要はありません。同時並行で取得できます。<br>
これらの特徴は他(ObjectPostman,ObjectUpdater,ObjectDeleter,ObjectFinder)でも同じです。

## オブジェクトの登録
### コルーチンで登録
```
//ObjectPostmanを取得します。
var postman = dataStore.CreatePostman();

//取得できるまで待ちます。登録内容はJsonで指定します。
yield return postman.PostAsync(className, "{\"userName\": \"aaa\", \"score\": 200}");

//存在しないクラスを指定すると新しくクラスが作られます。（NCMBの仕様、生成しないオプションはあるが未検証）
yield return postman.PostAsync("newClass", "{\"userName\": \"aaa\", \"score\": 200}");

//取得結果・objectIdが入ります。
var result = postman.GetResult(); 

//使い終わったらDisposeします。
postman.Dispose();
```
### Jsonの形式について
まずNCMBの仕様なのですが、クラスの**レコード毎**(≠フィールド毎)に違う型を持たせられます。<br>
例えばscoreのフィールドに数値と文字列を混在させることもできます。<br>
(一部型はフィールドで統一させられるようですが)<br>
型指定が異なるとObjectFinderでの比較が正しくできなくなるので注意してください。<br>
<br>
Jsonは以下のように指定します。<br>

```
{"userName": "bbb", "score": 200}
```

フィールド名をキーに、もう一方に値を指定します。<br>
この時、"(ダブルクォーテーション)で囲むと文字列型として、<br>
囲まない数字を数値型として登録されるようです。<br>
(実際の動作による。公式リファレンスにそれらしい記載は見つからず)<br>
またダブルクォーテーションを一文字として登録するときはバックスラッシュでエスケープできます。

```
{"userName": "\""} -> userNameは「"」になる
```

逆にいえばこの処理を怠るとインジェクション操作が可能になります。<br>
例えばuserNameを「あ","score":999,...」などと入力した時です。<br>
<br>
また、日付型は以下のようにします。

```
"dateField":{ "__type": "Date", "iso": "2022-08-01T10:00:00.000Z" }
```

これらの対応は骨が折れるので、<br>
UnityEngine.JsonUtility等のJsonパーサを用いるのをお勧めします。

```
[Serializable]
private class TestClass
{
	public string userName;
	public int score;
	//ApiDateTimeというstructをMinoly.ApiTypesに用意しておきました。
	public ApiDateTime dateTime;
}

public void Register()
{
	var testClass = new TestClass
	{
		userName = "a\"", 
		score = 100, 
		dateTime = new ApiDateTime(new DateTime(2022, 8, 1))
	};
	
	var json = UnityEngine.JsonUtility.ToJson(testClass);
	//Newtonsoftの方でも可
	//var json = Newtonsoft.Json.JsonConvert.SerializeObject(testClass);
	Debug.Log(json);
	//{"userName":"a\"","score":100,"dateTime":{"__type":"Date","iso":"2022-08-01T00:00:00.000Z"}}
}
```


### UniTaskで登録
```
//オブジェクトの登録
ObjectPostResult result = await dataStore.PostAsync(className, "{\"userName\": \"bbb\", \"score\": \"200\"}");
```
