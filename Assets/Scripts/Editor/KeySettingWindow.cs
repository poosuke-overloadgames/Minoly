using UnityEditor;
using UnityEngine;

namespace Editor
{
	public class KeySettingWindow : EditorWindow
	{
		private string _applicationKey = "";
		private string _clientKey = "";

		[MenuItem("Tools/MinolyKeySetting")]
		private static void ShowWindow()
		{
			GetWindow<KeySettingWindow>("MinolyKeySetting");
		}

		private void OnEnable()
		{
			_applicationKey = EditorUserSettings.GetConfigValue("MinolyApplicationKey");
			_clientKey = EditorUserSettings.GetConfigValue("MinolyClientKey");
		}

		private void OnGUI()
		{
			GUILayout.Label("ApplicationKey");
			_applicationKey = EditorGUILayout.TextField(_applicationKey);
			GUILayout.Label("ClientKey");
			_clientKey = EditorGUILayout.TextField(_clientKey);
			if (GUILayout.Button("Apply"))
			{
				EditorUserSettings.SetConfigValue("MinolyApplicationKey", _applicationKey);
				EditorUserSettings.SetConfigValue("MinolyClientKey", _clientKey);
				AssetDatabase.SaveAssets(); 
				Debug.Log("<color=#00ff00ff>Minoly KetSetting Applied!</color>");
			}
		}
	}
}