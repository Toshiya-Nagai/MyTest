using UnityEngine;
using System.Collections;

abstract public class BaseProxy<T> : MonoBehaviour where T:class {
	protected string assetName = null;
//	protected int bundleId = -1;
//	protected AssetBundle assetBundle = null;
	protected Object AstObject = null;
	protected string url;
	protected string hash;
	protected float startTime;
	
	virtual protected IEnumerator UpdateAsset(bool retry) {
		bool isDone = false;
		string error = null;
		startTime = Time.realtimeSinceStartup;
		
//		AssetBundleManager.instance.GetAssetBundle(bundleId, delegate(string err, AssetBundle ab, AssetBundleDataModel m) {
//			assetBundle = ab;
//			error = err;
//			isDone = true;
//		});

		UniSessionManager.GetAssetBundle(url,hash,delegate(UniAssetBundleQueue.Response res) {
			AstObject = res.AstObject;
			error = res.ErrorMessage;
			isDone = true;
		});
		
		//Not busy waiting because we are yielding on every frame.
		while(!isDone)
			yield return 0;
		
		// Check for errors.
		if(error != null || AstObject == null) {
			Log.Error("BaseProxy: {0} could not be downloaded from {1}", url, hash);
			if(retry) {
				yield return StartCoroutine(UpdateAsset(false));
			}
			yield break;
		}
		
		T asset = AstObject as T;
		if(asset == null) {
			if(retry) {
				yield return StartCoroutine(UpdateAsset(false));
			} else {
				Log.Error("BaseProxy: {0} not found on asset bundle id {1}", url, hash);
			}
			yield break;
		}
		
		yield return StartCoroutine(ProcessAsset(asset));
	}
	
	abstract protected IEnumerator ProcessAsset(T asset);	
}
