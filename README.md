# Minoly
![Release](https://img.shields.io/badge/dynamic/json.svg?label=version&query=$.version&uri=https%3A%2F%2Fraw.githubusercontent.com%2Fpoosuke-overloadgames%2FMinoly%2Fmaster%2FAssets%2FScripts%2FRuntime%2Fpackage.json)
![Unity](https://img.shields.io/badge/dynamic/json.svg?label=Unity&query=$.unity&uri=https%3A%2F%2Fraw.githubusercontent.com%2Fpoosuke-overloadgames%2FMinoly%2Fmaster%2FAssets%2FScripts%2FRuntime%2Fpackage.json&suffix=%2B%28%E5%A4%9A%E5%88%86%29)
![License](https://img.shields.io/badge/license-MIT-red)

Nifcloud mobile backendのデータストアに接続するライブラリです。

 - コルーチンでアクセスを待てます。
 - UniTaskにも対応しています。
 - MonoBehaviour不使用。
 
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
