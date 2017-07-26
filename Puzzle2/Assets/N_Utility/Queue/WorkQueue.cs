using System;

public class WorkQueue : AsyncQueue<WorkQueue, WorkQueue.Request, WorkQueue.Response> {
	
	public class Request : BaseRequest {
		public Func<Object> backgrundCallback;
		public Action backgrundNoReturnCallback;
	}
	
	public class Response: BaseResponse {
		public object retVal;
		public string error;
	}
	/// <summary>
	/// Do the specified backgroundCallback and foregroundCallback.
	/// </summary>
	/// <param name="backgroundCallback">Background callback. async thread call function</param>
	/// <param name="foregroundCallback">Foreground callback. it is main thread call after backgroundCallback</param>
	public static void Do(Func<Object> backgroundCallback, Callback foregroundCallback) {
		WorkQueue.instance.Enqueue(new WorkQueue.Request(){
			backgrundCallback = backgroundCallback,
			callback = foregroundCallback});
	}
	
	public static void Do(Action backgroundNoReturnCallback) {
		WorkQueue.instance.Enqueue(new WorkQueue.Request(){
			backgrundNoReturnCallback = backgroundNoReturnCallback});
	}
	
	protected override void ProcessRequest(Request request, Response response) {
		if (request.backgrundNoReturnCallback != null)
		try {request.backgrundNoReturnCallback();} catch (System.Exception e) {Log.Error("Error executing non returning callback. Error: " + e.ToString());}
		else
		try {response.retVal = request.backgrundCallback();} catch (System.Exception e) {response.error = e.ToString();}
	}
}
