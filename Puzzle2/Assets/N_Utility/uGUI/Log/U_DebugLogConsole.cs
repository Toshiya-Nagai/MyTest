
#if (UNITY_IOS || UNITY_ANDROID)
#define MOBILE
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System.Collections;
using U_DebugUtil;
using System.Collections.Generic;
using System.Text;

public class U_DebugLogConsole : IDebug {

	static string log;
	public static string LogText{
		set{log = value;}
		get{return log;}
	}
	public U_DebugLogConsole(){
		commandList = new List<DebugCommand>();
		commandList.Add(new DebugCommand("Clear",()=>{
			log = "";
		}));
		commandList.Add (new DebugCommand("System",()=>{
			log = CMDSystemInfo();
		}));
	}
	public string DebugLog (){
		return log;
	}
	public string ButtonText {
		get {
			return "Log Console";
		}
	}
	private List<DebugCommand> commandList;
	public List<DebugCommand> CommandList {
		get {
			return commandList;
		}
		set {
			throw new System.NotImplementedException();
		}
	}

	string CMDSystemInfo() {
		var info = new StringBuilder();
		
		info.AppendFormat("Unity Ver: {0}\n", Application.unityVersion);
		info.AppendFormat("Platform: {0} Language: {1}\n", Application.platform, Application.systemLanguage);
		info.AppendFormat("Screen:({0},{1}) DPI:{2} Target:{3}fps\n", Screen.width, Screen.height, Screen.dpi, Application.targetFrameRate);
		info.AppendFormat("Level: {0} ({1} of {2})\n", SceneManager.GetActiveScene().name, SceneManager.GetActiveScene().buildIndex, SceneManager.sceneCountInBuildSettings);
		info.AppendFormat("Quality: {0}\n", QualitySettings.names[QualitySettings.GetQualityLevel()]);
		info.AppendLine();
		info.AppendFormat("Data Path: {0}\n", Application.dataPath);
		info.AppendFormat("Cache Path: {0}\n", Application.temporaryCachePath);
		info.AppendFormat("Persistent Path: {0}\n", Application.persistentDataPath);
		info.AppendFormat("Streaming Path: {0}\n", Application.streamingAssetsPath);
		#if UNITY_WEBPLAYER
		info.AppendLine();
		info.AppendFormat("URL: {0}\n", Application.absoluteURL);
		info.AppendFormat("srcValue: {0}\n", Application.srcValue);
		info.AppendFormat("security URL: {0}\n", Application.webSecurityHostUrl);
		#endif
		#if MOBILE
		info.AppendLine();
		info.AppendFormat("Net Reachability: {0}\n", Application.internetReachability);
		info.AppendFormat("Multitouch: {0}\n", Input.multiTouchEnabled);
		#endif
		#if UNITY_EDITOR
		info.AppendLine();
		info.AppendFormat("editorApp: {0}\n", UnityEditor.EditorApplication.applicationPath);
		info.AppendFormat("editorAppContents: {0}\n", UnityEditor.EditorApplication.applicationContentsPath);
		info.AppendFormat("scene: {0}\n", UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().name);
		#endif
		info.AppendLine();
		return info.ToString();
	}}
