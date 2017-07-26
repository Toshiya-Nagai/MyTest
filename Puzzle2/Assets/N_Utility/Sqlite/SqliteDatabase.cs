using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;


public class ConnectDataModel {
	public int version;
	public string hash;
	public string assetUrl;
}

/// <summary>
/// manage the functions of the database
/// datamodel.db conecct function
/// not datamodel.db file update function
/// </summary>
public class SqliteDatabase : Singleton<SqliteDatabase> {
//	public DataModelAccess dataModelAccess = null;
	
	public int dataModelVersion;
	public string dataModelHash;
	public string dataModelAssetUrl;
	
	public event Action DataModelUpdated;
	
	private event Action<State> stateChanged;
	public event Action<State> StateChanged {
		add {stateChanged += value;}
		remove {stateChanged -= value;}
	}
	
	
	public enum State{
		Booting,
		BootReady,
		OnlineReady
	}
	public State CurrentState{get;private set;}
	
	
	void Awake(){
		CurrentState = State.Booting;
		StartCoroutine(orchestrator());
	}
	
	
	IEnumerator orchestrator(){
		yield return StartCoroutine (bootSequence());
		onStateChanged(State.BootReady);
	}
	
	IEnumerator bootSequence(){
		yield return StartCoroutine(unbundleRequiredFiles());
		//queue instance
		DataModelQueue.Instantiate();
		WorkQueue.Instantiate();
//		AssetBundleQueue.Instantiate();
		UniAssetBundleQueue.Instantiate();

		yield return StartCoroutine(DataModelConnect());
	}
	
	//Copied from unless 'StreamingAssets' missing from 'PersistentDataPath'
	IEnumerator unbundleRequiredFiles() {
		String error = null;
		string from, to;
		
		to = Path.Combine(Application.persistentDataPath, KeyValueStorage.KEY_VALUE_DB);
		if (!File.Exists(to)) {
			from = Path.Combine(Application.streamingAssetsPath, KeyValueStorage.KEY_VALUE_DB);
			Log.Info("Unbundling file: " + from);
			yield return StartCoroutine(UnbundleUtility.CopyFile(from, to, (inError) => error = inError));
			if (!string.IsNullOrEmpty(error)) {
				//FIXME: what should we do??
				Log.Error("Error unbundling keyValue.db: " + error);
			} else {
				#if UNITY_IPHONE
				// Don't backup to iCloud.
				//				iPhone.SetNoBackupFlag(to);
				#endif
			}
		}
		
		KeyValueStorage.Init();
		yield return StartCoroutine(unbundleDataModelFile());
	}
	
	//Android Path Check
	IEnumerator unbundleDataModelFile() {
		string error = null;
		if (DataModelFile.instance.UnbundleNeeded()) {	//Android only file existence check
			bool done = false;
			//StreamingAssets/DataModel.db is copy to PesistentDataPath
			StartCoroutine(DataModelFile.instance.LoadDynamicDataModel((outError) => {done = true; error = outError;}));
			while(!done)
				yield return 0;
			if (!string.IsNullOrEmpty(error)) {
				//FIXME: what should we do??
				Log.Error("Error unbundling dataModel.db: " + error);
			}
		}
	}
	
	//Connect Database
	public IEnumerator DataModelConnect() {
		Log.Info("DataModelConnect");
		bool done = false;
		bool unbundleRequired = false;
		AccessBase.Error error = null;
//		ConnectDataModel m = new ConnectDataModel();
//		error = dataModelAccess.Connect(DataModelFile.instance.GetPath(),out m);
		DataModelQueue.instance.Enqueue(DataModelQueue.Request.Connect(DataModelFile.instance.GetPath(), delegate(DataModelQueue.Response response){
			if(response.error != null){
				DataModelFile.instance.DiscardDynamic();	//copy database is delete and initialize
				if(response.error.code == AccessBase.Error.Code.OldVersion){
					unbundleRequired = true;
				}else{
					error = response.error;
				}
			}else{
				ConnectDataModel connectDM = response.dataModel as ConnectDataModel;
				dataModelVersion = connectDM.version;
				dataModelHash = connectDM.hash;
				dataModelAssetUrl = connectDM.assetUrl;
			}
			done = true;
		}));
		while(!done){yield return 0;}

		if (unbundleRequired) {
			Log.Warning("DataModel too old, going to unbundle and try again.");
			yield return StartCoroutine(unbundleDataModelFile());	//android database copy
			yield return StartCoroutine(DataModelConnect());
			yield break;
		} else if (error != null) {
			Log.Error("Error connecting to DataModel. Error: " + error.description);
			//FIXME: We really need a way to tell the player to try uninstall/reinstall
			Log.Warning("Blocking the main thread for ever because of the previous error!");
			yield break;
		}
		
		if(DataModelUpdated != null)
			DataModelUpdated();
	}
	
	void OnApplicationQuit(){
		KeyValueStorage.DisConnect();
	}
	
	
	private void onStateChanged(State newState) {
		Log.Info("InitializationManager: State changed to: " + newState.ToString());
		CurrentState = newState;
		if (stateChanged != null) {
			System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
			Log.Info("InitializationManager.OnStateChanged: Calling " + newState.ToString()+ " callbacks");
			stateChanged(CurrentState);
			Log.Info("InitializationManager.OnStateChanged: {0} callbacks done, took: {1}", newState.ToString(), stopwatch.ElapsedMilliseconds);
		}
	}
	
	
	public void ExecuteOnState(State state, Action cb) {
		if (state <= CurrentState) {
			cb();
		} else {
			Action<State> handler = (newState)=>{
				if (state <= newState) {
					StateChanged -= handler;
					cb();
				}
			};
			StateChanged += handler;
		}
	}
}


// internal utility to the asset bundle system.
public static class UnbundleUtility {
	public delegate void UnbundleCallback(string error);
	public static IEnumerator CopyFile(string from, string to, UnbundleCallback cb) {
		#if UNITY_ANDROID && !UNITY_EDITOR
		WWW download = new WWW(from);
		yield return download;
		
		if(download.error != null) {
			cb(download.error);
			yield break;
		}
		
		try {
			// Save it to disk.
			System.IO.File.WriteAllBytes(to, download.bytes);
		} catch(System.Exception e) {
			cb("Exception: " + e.ToString());
		}
		cb(null);
	}
	#else
	try {
		System.IO.File.Copy(from, to);
		cb(null);
	} catch (System.Exception e) {
		cb(e.ToString());
	}
	yield break;
}
#endif

}
