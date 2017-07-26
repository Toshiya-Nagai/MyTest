using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

abstract public class UniBaseQueue<TQueue,TRequest,TResponse> : Singleton<TQueue> where TQueue: class
where TRequest : UniBaseQueue<TQueue,TRequest,TResponse>.BaseRequest where TResponse : UniBaseQueue<TQueue,TRequest,TResponse>.BaseResponse,new(){
	public class BaseRequest{
		public bool Queued;		//repeat send request error;
		public float Progress;	//download working progress
		public bool Priority;	//priority working
		public bool Completed;
		public bool Cancelled;
		public UnityAction<TResponse> Callback;
	}
	public class BaseResponse{
		public bool IsError;
		public string ErrorMessage;
		public TRequest request;
	}
	private Queue<TRequest> requestQueue = new Queue<TRequest>();
	private Queue<TRequest> priorityRequestQueue = new Queue<TRequest>();
	private Queue<TResponse> responseQueue = new Queue<TResponse>();
	protected TRequest workingRequest;

	public float WorkingQueueProgress{
		get{
			if(workingRequest != null){return workingRequest.Progress;}
			else{Debug.Log("working is null");return 0.0f;}
		}
	}

	void Update(){
		//request
		if(workingRequest == null && (priorityRequestQueue.Count > 0 || requestQueue.Count > 0)){
			workingRequest = (priorityRequestQueue.Count > 0)?priorityRequestQueue.Dequeue():requestQueue.Dequeue();
			TResponse response = new TResponse();
			response.request = workingRequest;
			if(!response.request.Cancelled){
				ExecuteAfterCoroutine(ProcessRequest(workingRequest,response),()=>{
					response.request.Completed = true;
					responseQueue.Enqueue(response);
					workingRequest = null;
				});
			}
		}

		//response
		if(responseQueue.Count > 0){
			lock(responseQueue){
				for(int i = 0;i < responseQueue.Count;i++){
					var response = responseQueue.Dequeue();
					if(response.request.Callback != null && !response.request.Cancelled){
						response.request.Callback(response);
					}
				}
				responseQueue.Clear();
			}
		}
	}

	public void Enqueue(TRequest request){
		if(request.Queued){
			Debug.LogError ("This request has already been queued");
			return;
		}
		request.Queued = true;
		lock(requestQueue){
			if(request.Priority){
				priorityRequestQueue.Enqueue(request);
			}else{
				requestQueue.Enqueue(request);
			}
		}
	}

	public bool Contains(TRequest request){
		return requestQueue.Contains(request);
	}

	public void Cancel(TRequest request){
		request.Cancelled = true;
	}

	public void CancelAll(){
		lock(requestQueue){
			foreach(var buf in requestQueue){
				buf.Cancelled = true;
			}
		}
	}
	abstract public IEnumerator ProcessRequest(TRequest request,TResponse response);
}
