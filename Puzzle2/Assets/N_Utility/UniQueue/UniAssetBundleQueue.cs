using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Uni asset bundle queue.
/// asset bundle caution web page
/// http://sassembla.github.io/Public/2015:02:04%2012-47-46/2015:02:04%2012-47-46.html
/// </summary>
public class UniAssetBundleQueue : UniBaseQueue<UniAssetBundleQueue,UniAssetBundleQueue.Request,UniAssetBundleQueue.Response> {
	public class Request : BaseRequest{
		public float TimeOut = 60f;
		public string Url;
		public string Hash;
		public Request(string url) : this(url,"",null){}
		public Request(string url,string hash) : this(url,hash,null){}
		public Request(string url,string hash,UnityAction<Response> callback){
			Url = url;
			Hash = hash;
			Callback = callback;
		}
	}
	public class Response : BaseResponse{
		public UnityEngine.Object AstObject;
		public bool isNoCache = false;
	}

	public override IEnumerator ProcessRequest (Request request, Response response){
		float StartAt = Time.time;
		WWW _loadingWWW = null;
		uint tempHash = 0;
		if (request.Hash.ToString()  != null &&  request.Hash.ToString()  != "") {
			try {
				tempHash =  System.Convert.ToUInt32(request.Hash);
			}  catch {
				Debug.Log ("ErrorFile  :  " + request.Url + "  :  "+ request.Hash.ToString());
				tempHash = 0;
			}
		}
		
		if (tempHash > 0) {
			_loadingWWW = WWW.LoadFromCacheOrDownload (request.Url, 1, tempHash);
		} else {
			_loadingWWW = WWW.LoadFromCacheOrDownload (request.Url, 1);
		}
		
		bool isTimeout = false;
		while (!_loadingWWW.isDone && !isTimeout) {
			if  ((Time.time - StartAt) > request.TimeOut) {
				isTimeout = true;
			}
			yield return 0;
		}
		if (isTimeout) {
			response.IsError = true;
			response.ErrorMessage = "timeout";
		} else if (_loadingWWW.error != null) {
			Debug.Log (request.Url);
			Debug.Log (_loadingWWW.error);
			response.IsError = true;
			response.ErrorMessage = _loadingWWW.error;
		} else {
			if (_loadingWWW.assetBundle != null) {
				if (response.isNoCache){
					if ((UnityEngine.Object)_loadingWWW.assetBundle.mainAsset != null) {
						_loadingWWW.assetBundle.Unload(true);
					}
				} else {
					Debug.Log(_loadingWWW.assetBundle.mainAsset);
					response.AstObject  = (UnityEngine.Object)_loadingWWW.assetBundle.mainAsset;
					Debug.Log(response.AstObject);
					if (response.AstObject != null) {
						_loadingWWW.assetBundle.Unload(false);
					}
				}
			} else {
				response.IsError = true;
				response.ErrorMessage = "don't have assetbundle";
			}
		}
		_loadingWWW.Dispose();
		_loadingWWW = null;
	}
}
