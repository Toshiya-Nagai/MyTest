using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#region enum
public enum _URL{
	LOCAL,
	Sample,
	NONE,
	//user
	Get_User,

}
#endregion


/// <summary>
/// URL control class.
/// url control is simply.
/// use sample
/// 	URL_NAME entry free name	SAMPLE
/// 	UrlFactory add url	 		{URL_NAME.SAMPLE,"http://localhost/php/test.php"},
/// 	call script					N_UrlUtil.GetUrl(URL_NAME.SAMPLE);
/// </summary>
public class UrlUtil : Singleton<UrlUtil>{

	public PathModel PathModel = new PathModel();

	void Awake(){
		PathModel = PathModel.Init();
	}

	private static readonly Dictionary<_URL,string> UrlFactory = new Dictionary<_URL, string>(){
		{_URL.LOCAL,"http://localhost/php/myTest/getUnityChan.php"},
		{_URL.Sample,"http://localhost/php/TouchGame/get_ranking.php?id=1"},
		{_URL.Get_User,"http://127.0.0.1/framework/public/user/getUnit"},
	};

	public static string GetUrl(_URL anUrlName){
		if(UrlFactory.ContainsKey(anUrlName)){
			return UrlUtil.UrlFactory[anUrlName];
		}else{
			Debug.LogError("Not Found Key = " + anUrlName);
			return "";
		}
	}
}

[System.Serializable]
public class PathModel{
	#region PathModel Member
	[SerializeField]private string _serverAssetsPath;
	[SerializeField]private string _serverProgramPath;
	[SerializeField]private string _serverImagePath;
	[SerializeField]private string _serverHtmlPath;
	[SerializeField]private string _persistentDataPath;
	[SerializeField]private string _streamingAssetsPath;
	[SerializeField]private string _serverAssetsPathPF;

	public string ServerAssetPath{get{return _serverAssetsPath;} private set{_serverAssetsPath = value;}}
	public string ServerProgramPath{get{return _serverProgramPath;} private set{_serverProgramPath = value;}}
	public string ServerImagePath{get{return _serverImagePath;} private set{_serverImagePath = value;}}
	public string PersistentDataPath{get{return _persistentDataPath;} private set{_persistentDataPath = value;}}
	public string ServerHtmlPath{get{return _serverHtmlPath;} private set{_serverHtmlPath = value;}}
	public string StreamingAssetsPath{get{return _streamingAssetsPath;} private set{_streamingAssetsPath = value;}}
	public string ServerAssetPathPF{get{return _serverAssetsPathPF;} private set{_serverAssetsPathPF = value;}}
	#endregion

	public PathModel Init(){
		StreamingAssetsPath = Application.streamingAssetsPath;
		return this;
	}

	#region PathModel public method
	public void SetLocalizePath(U_Localization anLocalize){
		_serverAssetsPath = U_Localization.Get("TestAssetPath");
		_serverProgramPath = U_Localization.Get("TestProgramPath");
		_serverImagePath = U_Localization.Get("TestImagePath");
		_serverHtmlPath = U_Localization.Get("TestHtmlPath");
	}
	private string GetAndroidPersistentPath(){
		return Application.persistentDataPath;
	}
	private string GetIosPersistentPath(){
		string persistentDataPath = "";
		persistentDataPath = Application.dataPath.Substring (0, Application.dataPath.Length - 5); 
		persistentDataPath = persistentDataPath.Substring(0, persistentDataPath.LastIndexOf('/')); 
		persistentDataPath += "/Documents/tmp"; 
		return persistentDataPath;
	}

	public string GetPersistentPath(bool isAndroid){
		return (isAndroid)?GetAndroidPersistentPath():GetIosPersistentPath();
	}
	public string GetAssetsPathPF(bool isAndroid){
		string assetsPathPf = (isAndroid) ? "/android":"/ios";
		ServerAssetPathPF = assetsPathPf;
		return ServerAssetPathPF;
	}
	#endregion
}