using UnityEngine;
using UnityEngine.Events;
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class DataModelFile : Singleton<DataModelFile> {
	private static string DynamicDataModelKey = "DynamicDataModelName";
	
	
	private KeyValueStorage kvs;
	void Awake() {
		kvs = KeyValueStorage.Instance(KeyValueStorage.Storage.DATA_MODEL_FILE);
	}
	
	// Gets the path to the current data model.
	public string GetPath() {
		string dynamicPath = GetDynamicPath();
		if(dynamicPath != null) {
			return dynamicPath;
		} else {
			return Path.Combine(Application.streamingAssetsPath, "dataModel.db");
		}
	}
	
	// Revert to bundled data model. Use if there is a problem accessing
	// the dynamic data model.
	public void DiscardDynamic() {
		// Get the dynamic path. Bail if it is null.
		string dynamicPath = GetDynamicPath();
		if(dynamicPath == null)
			return;
		
		// Forget about this dynamic data model.
		kvs.Remove(DynamicDataModelKey);
		
		// Delete the file from disk.
		File.Delete(dynamicPath);
	}
	
	// Determines if Unbundle must be called.
	public bool UnbundleNeeded() {
		#if UNITY_ANDROID && !UNITY_EDITOR
		string path = GetPath();
		if(File.Exists(path))
			return false;
		else
			return true;
		#else
		return false;
		#endif
	}
	
	// Gets the path to the current dynamic data model if there is one.
	// Otherwise returns null.
	protected string GetDynamicPath() {
		Log.Info("GetDynamicPath");
		string path = kvs.GetValue<string>(DynamicDataModelKey);
		if (!string.IsNullOrEmpty(path)){
			Log.Info("return persistentDataPath");
			return Path.Combine(Application.persistentDataPath, path);
//			return Path.Combine(N_UrlUtil.instance.path.PersistentDataPath, path);
		}
		else
			return null;
	}

	//download server datamodel file
	//callback string is error
	public IEnumerator DownloadDataModel(string url,UnityAction<string> callback){
		WWW www = new WWW(url);
		yield return www;
		if(www.error != null){
			callback(www.error);
			yield break;
		}
		try {
			// Save it to disk.
			string newName = Path.GetRandomFileName();
			string newPath = Path.Combine(Application.persistentDataPath, newName);
//			string newPath = Path.Combine(N_UrlUtil.instance.path.PersistentDataPath, newName);
			File.WriteAllBytes(newPath, www.bytes);
			
			// Get the path to the previous dynamic data model.
			string oldPath = GetDynamicPath();
			
			// Save the name of the new data model.
			kvs.SetValue(DynamicDataModelKey, newName);
			
			// Delete the old dynamic data model if there is one.
			if(oldPath != null)
				File.Delete(oldPath);
			
			callback(null);
		} catch(Exception e) {
			callback("Exception: " + e.ToString());
		}
	}

	//download local datamodel file
	//callback string is error 
	public IEnumerator LoadDynamicDataModel(Action<string> callback){
		string path = GetPath();
		Debug.Log("load dataModel : " + path);
		WWW www = new WWW(GetPath());
		yield return www;
		if(www.error != null){
			callback(www.error);
			yield break;
		}
		Debug.Log("EndDownload");
		try {
			// Save it to disk.
			string newName = Path.GetRandomFileName();
			string newPath = Path.Combine(Application.persistentDataPath, newName);
//			string newPath = Path.Combine(N_UrlUtil.instance.path.PersistentDataPath, newName);
			File.WriteAllBytes(newPath, www.bytes);
			
			// Save the name of the new data model.
			kvs.SetValue(DynamicDataModelKey, newName);
			callback(null);
		} catch(Exception e) {
			callback("Exception: " + e.ToString());
		}
	}

}
