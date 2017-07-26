using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class UniSessionManager{
	#region Json Queue
	public static void GetJson(string url,Dictionary<string,string> queryParams,UnityAction<UniJsonQueue.Response> callback){
		UniJsonQueue.Request request = new UniJsonQueue.Request(url,queryParams,callback);
		UniJsonQueue.instance.Enqueue(request);
	}
	public static void GetJson(UniJsonQueue.Request request){UniJsonQueue.instance.Enqueue(request);}

	/// <summary>
	/// Gets the jsons.
	/// </summary>
	/// <returns>The jsons.</returns>
	/// <param name="requests">Requests.</param>
	/// <param name="progressCallback">Progress callback (one queue progress,all queue progress).</param>
	public static IEnumerator GetJsons(List<UniJsonQueue.Request> requests,UnityAction<float,float> progressCallback){
		int allCount = requests.Count;
		int completeCount = 0;
		var queue = UniJsonQueue.instance;
		foreach(var buf in requests){
			buf.Callback += (res)=>{
				completeCount++;
			};
			queue.Enqueue(buf);
		}
		while(completeCount < allCount){
			yield return null;
			//			progressCallback((1.0/allCount)*completeCount);
			Debug.Log(1.0-((allCount-completeCount)*(1.0f/allCount)));
			if(progressCallback != null){
				progressCallback(queue.WorkingQueueProgress,1.0f-((allCount-completeCount)*(1.0f/allCount)));
			}
		}
	}
	#endregion

	#region AssetBundle Queue
	public static void GetAssetBundle(string url,string hash,UnityAction<UniAssetBundleQueue.Response> callback){
		UniAssetBundleQueue queue = UniAssetBundleQueue.instance;
		UniAssetBundleQueue.Request request = new UniAssetBundleQueue.Request(url,hash,callback);
		queue.Enqueue(request);
	}

	public static void GetAssetBundle(UniAssetBundleQueue.Request request){
		UniAssetBundleQueue.instance.Enqueue(request);
	}
	
	public static IEnumerator GetAssetBundles(List<UniAssetBundleQueue.Request> requests,UnityAction<float,float> progressCallback){
		int allCount = requests.Count;
		int completeCount = 0;
		var queue = UniAssetBundleQueue.instance;
		foreach(var buf in requests){
			buf.Callback += (res)=>{
				completeCount++;
			};
			queue.Enqueue(buf);
		}
		while(completeCount < allCount){
			yield return null;
			Debug.Log(1.0-((allCount-completeCount)*(1.0f/allCount)));
			if(progressCallback != null){
				progressCallback(queue.WorkingQueueProgress,1.0f-((allCount-completeCount)*(1.0f/allCount)));
			}
		}
	}
	#endregion

	#region DataModel Update
	public static void UpdateDataModelFile(string url,UnityAction callback){
		Log.Debug("SessionManager.UpdateDataModelFile");
		int retries = 10;
		UnityAction<string> cb;
		cb = delegate(string error) {
			//error = "Debug Error Handling";
			if(error != null) {
				if (--retries >= 0) {
					Log.Warning("UpdateDataModelFile failed, will retry {0} more times. Error: {1}", retries+1, error);
					DataModelFile.instance.StartCoroutine (DataModelFile.instance.DownloadDataModel(url,cb));
				} else {
					//FIXME: what should be shown to the user
					Log.Error("UpdateDataModelFile failed: " + error);
					//FIXME: we are assuming all errors are related to network, but it may be possible that we are running out of disk space to download the new file.
//					connectionError = true;
					callback();
				}
			} else {
				Log.Debug("SessionManager.UpdateDataModelFileDone");
				SqliteDatabase.instance.ExecuteAfterCoroutine(SqliteDatabase.instance.DataModelConnect(), callback);
			}
		};
		DataModelFile.instance.StartCoroutine (DataModelFile.instance.DownloadDataModel(url,cb));
	}
	#endregion
}
