using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class DataModelQueue : AsyncQueue<DataModelQueue, DataModelQueue.Request, DataModelQueue.Response> { 
	public class Request : BaseRequest {
		public enum Type {
			Connect,
			Multi,
			SingleAssetBundle,
			AllAssetBundles,
		}
		public Type type;
		
		// Connect variables.
		public string connectPath;
		
		// Multi variables.
		public delegate DataModelAccess.Error MultiDelegate(DataModelAccess db, object inputs, out object outputs);
		public object multiInputs;
		public MultiDelegate multiDelegate;
		
		// Shared variables.
		public object id;
		public object subid;
		
		public static Request Connect(string path, Callback callback) {
			Request r = new Request();
			r.callback = callback;
			r.type = Type.Connect;
			r.connectPath = path;
			return r;
		}
		
		public static Request Multi(object multiInputs, MultiDelegate multiDelegate, Callback callback) {
			Request r = new Request();
			r.callback = callback;
			r.type = Type.Multi;
			r.multiInputs = multiInputs;
			r.multiDelegate = multiDelegate;
			return r;
		}

		public static Request SingleAssetBundle(int id, Callback callback) {
			Request r = new Request();
			r.callback = callback;
			r.type = Type.SingleAssetBundle;
			r.id = id;
			return r;
		}
		
		public static Request AllAssetBundles(Callback callback) {
			Request r = new Request();
			r.callback = callback;
			r.type = Type.AllAssetBundles;
			return r;
		}
	}
	
	public class Response : BaseResponse {
		public object dataModel = null;
		public DataModelAccess.Error error;
	}

	private DataModelAccess dataModelAccess = null;

	public override void Start() {
		dataModelAccess = new DataModelAccess();
		base.Start();
	}

	protected override void ProcessRequest(Request request, Response response) {
		switch(request.type) {
		case Request.Type.Connect:
			ProcessConnectRequest(request, response);
			break;
		case Request.Type.Multi:
			ProcessMultiRequest(request, response);
			break;
//		case Request.Type.SingleCard:
//			ProcessCardRequest(request, response);
//			break;
//		case Request.Type.SingleAssetBundle:
//			ProcessAssetBundleRequest(request, response);
//			break;
//		case Request.Type.AllAssetBundles:
//			ProcessAssetBundleRequest(request, response);
//			break;
		}
	}

	#region Process Request Async Method
	private void ProcessConnectRequest(Request request, Response response) {
		ConnectDataModel m;
		response.error = dataModelAccess.Connect(request.connectPath, out m);
		
		if(response.error == null)
			response.dataModel = m;
	}
	
	private void ProcessMultiRequest(Request request, Response response) {
		try {
			response.error = request.multiDelegate(dataModelAccess, request.multiInputs, out response.dataModel);
		} catch(Exception e) {
			response.error = DataModelAccess.Error.Exception(e);
		}
	}

	#endregion

	void OnApplicationQuit(){
		if(dataModelAccess != null){
			dataModelAccess.Disconnect();
		}
	}
}
