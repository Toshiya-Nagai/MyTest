using UnityEngine;
using System;
using System.Net;
using System.Collections;
using System.Net.Cache;
using System.IO;

/*
	It does not work because the bug has occurred when
	'gzip' come to download the compression of 'Unity5.0 of Ios 64bit' In 'WebRequest Api'
*/
/// <summary>
/// Asset bundle queue.
/// </summary>

namespace CSharpQueue{
	public class AssetBundleQueue : AsyncQueue<AssetBundleQueue, AssetBundleQueue.Request, AssetBundleQueue.Response> {
		private string basePath = Application.persistentDataPath;
		
		public class Request : BaseRequest {
			public int assetId;
			public string assetURL;
			public string assetName;
			public string hash;
			public IList callbacks = ArrayList.Synchronized(new ArrayList());
			public int retries = 10;
			public Request(){}
			public Request(int assetId, bool prioritized, System.Delegate cb) {
				this.assetId = assetId;
				this.prioritized = prioritized;
				retries = 10;
				callbacks.Add(cb);
			}
		}
		
		public class Response : BaseResponse {
			public bool dowmloadComplete = false;
			public bool equalHash = false;
			public string assetBundlePath = null;
			
			public HttpStatusCode responseCode;
			public string error = null;
		}

		protected override void ProcessRequest(Request request, Response response) {
			try {
				Log.Info("Starting download of assetId: {0}, from: {1}", request.assetId, request.assetURL);
				HttpWebRequest httpRequest = HttpWebRequest.Create(request.assetURL) as HttpWebRequest;
				httpRequest.CachePolicy = new System.Net.Cache.HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
				
				httpRequest.Timeout = 60;
				httpRequest.Headers["Accept-Encoding"] = "gzip";
				
				HttpWebResponse httpResponse = httpRequest.GetResponse() as HttpWebResponse;
				response.responseCode = httpResponse.StatusCode;
				if (httpResponse.StatusCode != HttpStatusCode.OK) {
					response.error = string.Format("{0}({1})", httpResponse.StatusDescription, httpResponse.StatusCode);
					return;
				}
				
				string assetBundlePath = basePath + "/" + request.assetName;
				System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
				using (Stream responseStream = httpResponse.GetResponseStream(), fileStream = new FileStream(assetBundlePath, FileMode.Create)) {
					
					Stream readStream = responseStream;
					if (httpResponse.ContentEncoding.ToLower() == "gzip") {
	//					if ( AppConfig.logAssetBundles )
	//						Log.Debug("Using GZIP");
	//					readStream = new GZipInputStream(responseStream);
					}
					
					int read;
					byte[] buffer = new byte[1024 * 16];
					
					while ((read = readStream.Read(buffer,0,buffer.Length)) != 0) {
						//update md5
						md5.TransformBlock(buffer, 0, read, buffer, 0);
						//write the file
						fileStream.Write(buffer, 0, read);
						#if UNITY_ANDROID
						//throttle the writing on android
						System.Threading.Thread.Sleep(16);
						#endif
					}
					
					fileStream.Close();
					response.dowmloadComplete = true;
					//final update
					md5.TransformFinalBlock(buffer, 0, 0);
	//				string hash = md5.Hash.ToHex();
					string hash = "";
					if (string.Compare(hash, request.hash, System.StringComparison.InvariantCultureIgnoreCase) != 0) {
						//delete the file if the hashes are different
						File.Delete(assetBundlePath);
						if (--request.retries >= 0) {
							Log.Warning("Hash is different for: {0}, expected: {1}, calculated: {2}, will try downloading again {3} more times.", request.assetURL, request.hash, hash, request.retries+1);
							ProcessRequest(request, response);
							return;
						}
						response.equalHash = false;
						response.error = string.Format("Hash is different for: {0}, expected: {1}, calculated: {2}", request.assetURL, request.hash, hash);
					} else {
						response.equalHash = true;
						response.assetBundlePath = assetBundlePath;
					}
				}
			} catch (System.Exception e) {
				WebException we = e as WebException;
				if (we != null) {
					if (we.Status == WebExceptionStatus.Timeout && --request.retries >= 0) {
						Log.Warning("Connection timeout while downloading asset: {0}, will retry {1} more times.", request.assetURL, request.retries+1);
						ProcessRequest(request,response);
						return;
					}
					response.error = string.Format("Error downloading asset: {0}, error: {1}", request.assetURL, we.Status.ToString());
				} else {
					response.error = string.Format("Exception downloading asset: {0}, error: {1}", request.assetURL, e.ToString());
				}
			}
		}
	}
}