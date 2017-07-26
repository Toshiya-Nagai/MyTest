using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using LitJson2;

public class UniJsonQueue : UniBaseQueue<UniJsonQueue,UniJsonQueue.Request,UniJsonQueue.Response> {
	public class Request : BaseRequest {
		public float TimeOut = 60f;
		public string Url;
		public Dictionary<string, string> QueryParams;
		public Request(string url) : this(url,null,null){}
		public Request(string url,Dictionary<string,string> queryParams) : this(url,queryParams,null){}
		public Request(string url,Dictionary<string,string> queryParams,UnityAction<Response> callback){
			Url = url;
			QueryParams = queryParams;
			Callback = callback;
		}
	}
	public class Response : BaseResponse{
		public string checkUrl = "";
//		public JsonData Json;
		public string JsonText;
	}

	public override IEnumerator ProcessRequest (Request request, Response response){
		float StartAt = Time.time;
		WWW _loadingWWW = null;
		if ((request.QueryParams != null) && (request.QueryParams.Count > 0)) {
			WWWForm wwwForm = new WWWForm();
			string Query = "?";
			foreach (KeyValuePair<string,string> tempParams in request.QueryParams) {
				wwwForm.AddField(tempParams.Key, tempParams.Value);
				Query += tempParams.Key.ToString() + "="+ tempParams.Value.ToString() + "&";
			}
#if UNITY_EDITOR
			Debug.Log (request.Url + Query);
#endif 
			_loadingWWW = new WWW(request.Url, wwwForm);
		} else {
#if UNITY_EDITOR
			Debug.Log (request.Url);
#endif 
			_loadingWWW = new WWW(request.Url);
		}
		bool isTimeout = false;
		while (!_loadingWWW.isDone && !isTimeout) {
			if  ((Time.time - StartAt) > request.TimeOut) {
				isTimeout = true;
			}
			//current progress
			request.Progress = _loadingWWW.progress;
			yield return 0;
		}
		request.Progress = _loadingWWW.progress;
		if (isTimeout) {
			response.IsError = true;
			response.ErrorMessage = "timeout";
		} else if (_loadingWWW.error != null) {
			response.IsError = true;
			response.ErrorMessage = _loadingWWW.error;
		} else {
			response.IsError = false;
			response.ErrorMessage = "";
//			response.Json = JsonMapper.ToObject<JsonData> (_loadingWWW.text);
			response.JsonText = _loadingWWW.text;
		}
		_loadingWWW.Dispose();
		_loadingWWW = null;
	}
}
