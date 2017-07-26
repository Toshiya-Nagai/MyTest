using UnityEngine;
using System;
using System.IO;
using System.Net;
using System.Collections;
using System.Collections.Generic;

/*
	It does not work because the bug has occurred when
	'gzip' come to download the compression of 'Unity5.0 of Ios 64bit' In 'WebRequest Api'
*/
/// <summary>
/// Network queue.
/// </summary>
public class NetworkQueue : AsyncQueue<NetworkQueue, NetworkQueue.Request, NetworkQueue.Response> {
	public class Request : BaseRequest{
		public string url;
		public string method;		//get or post
		public string body;			//id=8&nickname=xx
		public bool retry = true;
		public int retries;
		public Request(string url){
			this.url = url;
		}
	}
	public class Response : BaseResponse{
		public string error;	
		public HttpStatusCode httpStatusCode = HttpStatusCode.Unused;
		public string jsonText;
	}

	protected override void ProcessRequest (Request request, Response response){
		//tutorial request----------------------------

		//--------------------------------------------
		ProcessNetworkRequest(request,response);
	}
	private void ProcessNetworkRequest(Request request, Response response) {
		try{
			System.Text.Encoding enc =
				System.Text.Encoding.GetEncoding("utf-8");
			HttpWebRequest httpRequest = HttpWebRequest.Create(request.url) as HttpWebRequest;
			httpRequest.ContentType = "application/x-www-form-urlencoded";
			httpRequest.Headers["Accept-Encoding"] = "gzip";
			using(HttpWebResponse httpResponse = httpRequest.GetResponse() as HttpWebResponse){
				response.httpStatusCode = httpResponse.StatusCode;
				if(response.httpStatusCode == HttpStatusCode.OK){
					Stream resStream = httpResponse.GetResponseStream();
					using(StreamReader sr = new StreamReader(resStream,enc)){
						response.jsonText = sr.ReadToEnd();
					}
					httpResponse.Close();
					resStream.Close();
				}else{
					response.error = "Http error: " + response.httpStatusCode;
				}
			}
		}catch(Exception e){
			WebException we = e as WebException;
			if(we != null) {
				if (request.retry && --request.retries >= 0) {
					Log.Warning("Connection error: {0}, will retry {1} times.", we.Status.ToString(), request.retries+1);
					ProcessRequest(request,response);
					return;
				} else {
					response.error = "WebException: " + e;
				}
			} else {
				response.error = "Exception: " + e;
			}
		}
	}
}