using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace CSharpQueue{

	public class AssetBundleManager : Singleton<AssetBundleManager> {
		private class LoadedBundle {
			public AssetBundleDataModel m;
			public AssetBundle ab;
		}
		
		private class BundleCacheEntry {
			public string path;
			public string hash;
			public string[] intern;
			
			public static BundleCacheEntry Parse(string[] values) {
				if (values != null) {
					return new BundleCacheEntry(){path=values[0], hash=values[1], intern=values};
				}
				return null;
			}
			
			public BundleCacheEntry() {
			}
			
			public BundleCacheEntry(string path, string hash) {
				this.path = path;
				this.hash = hash;
				this.intern = new string[]{path,hash};
			}
			
			public object Serialize() {
				intern[0] = path;
				intern[1] = hash;
				return intern;
			}
		}
		
		private IDictionary<string,LoadedBundle> loadedBundles = new Dictionary<string,LoadedBundle>();
		private IDictionary<string,AssetBundleQueue.Request> downloadingBundles = new Dictionary<string,AssetBundleQueue.Request>();

		private KeyValueStorage abCache;

		// save it for use by background threads
		private static string persistentDataPath = Application.persistentDataPath;

		public System.Collections.IEnumerator Init() {
			abCache = KeyValueStorage.Instance(KeyValueStorage.Storage.ASSET_BUNDLE);
			SqliteDatabase.instance.DataModelUpdated += DataModelUpdated;
			yield return null;
			//tutorial--------------------------
	//		if (abCache.GetValue<bool>("TUTORIAL_AB_UNBUNDLED"))
	//			yield break;
	//		yield return StartCoroutine(UnbundleTutorialAssetBundle());
	//		
	//		bool done = false;
	//		abCache.SetValueAsync<bool>("TUTORIAL_AB_UNBUNDLED", true, (response) => done = true);
	//		while(!done)
	//			yield return 0;
			//----------------------------------
		}

		private void DataModelUpdated() {
			Log.Warning("DataModelChanged");
			DataModelQueue.instance.Enqueue(DataModelQueue.Request.AllAssetBundles(delegate(DataModelQueue.Response response){
				if (response.error != null) {
					//TODO: handle error
					Log.Error("Failed to get AllAssetBundles. Error: " + response.error.description);
					return;
				}
				
				string assetKey;
				LoadedBundle loadedBundle;
				foreach (AssetBundleDataModel item in response.dataModel as IList<AssetBundleDataModel>) {
					assetKey = CreateAssetKey(item.id);
					loadedBundle = loadedBundles.ContainsKey(assetKey) ? loadedBundles[assetKey] : null;
					if (loadedBundle != null && PlatformHash(loadedBundle.m) != PlatformHash(item)) {
						//Log.Info("AssetBundle needs to be unloaded. Id: " + item.id);
						loadedBundle.ab.Unload(false);
						loadedBundles.Remove(assetKey);
					}
					if (downloadingBundles.ContainsKey(assetKey)) {
						AssetBundleQueue.instance.Cancel(downloadingBundles[assetKey]);
						downloadingBundles.Remove(assetKey);
					}
				}
				
				//unload the assets that were loaded but are not present on the new db.
				foreach(LoadedBundle item in loadedBundles.Values) {
					item.ab.Unload(false);
				}
				loadedBundles.Clear();
				downloadingBundles.Clear();
				
				//start downloading the new bundles.
				DownloadMissingAssetBundles();
			}));
		}

		public void DownloadMissingAssetBundles() {
			DataModelQueue.Request req = DataModelQueue.Request.Multi(
				null,
				delegate(DataModelAccess db, object inputs, out object outputs) {
				outputs = null;
				
				List<AssetBundleDataModel> assetBundles = null;
//				DataModelAccess.Error err = db.GetAllAssetBundles(out assetBundles);
				DataModelAccess.Error err = null;
				if (err != null) {
					Log.Warning("Couldn't retrieve assetBundle list. Error: " + err.description);
					return null;
				}
				
	//			if ( AppConfig.logAssetBundles )
	//				Log.Info("DownloadMissingAssetBundles.Multi: Going to check: {0} assets.", assetBundles.Count);
				foreach(AssetBundleDataModel assetBundleDM in assetBundles) {
					int currentId = assetBundleDM.id;
					
					BundleCacheEntry diskCache = BundleCacheEntry.Parse(abCache.GetValue<string[]>(CreateAssetKey(currentId)));
					if (diskCache != null) {
						if (diskCache.hash == PlatformHash(assetBundleDM)) {
							//Log.Info("DownloadingMissingAssetBundles.Multi: asset id: {0} already downloaded, skeeping it", currentId);
							continue;
						} else {
							//Log.Info("DownloadingMissingAssetBundles.Multi: asset id: {0} already downloaded but with different hash, downloading it again", currentId);
							RemoveFromDiskCacheAsync(currentId, CreateAssetKey(currentId), diskCache.path);
						}
					}
					
					lock (downloadingBundles) {
						string assetKey = CreateAssetKey(currentId);
						
						if (!downloadingBundles.ContainsKey(assetKey)) {
							AssetBundleQueue.Request abRequest = new AssetBundleQueue.Request(currentId,false,(GetAssetBundleCallback)delegate(string error, AssetBundle assetBundle, AssetBundleDataModel dm){/*NOOP*/});
							downloadingBundles.Add(assetKey,abRequest);
							
							//Log.Info("Adding asset id: {0} to be downloaded in background.", currentId);
							DownloadAssetBundle(abRequest, delegate(string error, AssetBundleQueue.Request cbRequest, AssetBundleDataModel m, string assetPath) {
								if (error != null) {
	//								if ( AppConfig.logAssetErrors )
										Log.Error("Couldn't download asset id: {0} in background, because of error: {1}", currentId,error);
								} else if (cbRequest.callbacks.Count > 1) {
									Load(assetKey,m,assetPath,cbRequest.callbacks);
								}
							});
						}
					}
				}
				return null;
			},
			null//NOOP on main Thread
			);
			
			DataModelQueue.instance.Enqueue(req);
		}
		private delegate void DowmloadAssetBundleCallback(string error, AssetBundleQueue.Request abRequest, AssetBundleDataModel m, string assetPath);
		private void DownloadAssetBundle(AssetBundleQueue.Request abRequest, DowmloadAssetBundleCallback cb) {
			DataModelQueue.Request request = DataModelQueue.Request.SingleAssetBundle(abRequest.assetId, delegate(DataModelQueue.Response response) {
				if (response.error != null) {
					DownloadingDone(abRequest.assetId);
					cb(response.error.description, abRequest, null, null);
					return;
				}
				
				AssetBundleDataModel assetBundleDM = response.dataModel as AssetBundleDataModel;
				
				string hash = PlatformHash(assetBundleDM);
				if (hash == null) {
					DownloadingDone(abRequest.assetId);
					cb("Platform not known!: " + Application.platform.ToString(), abRequest, null, null);
					return;
				}
				
				abRequest.assetURL = SqliteDatabase.instance.dataModelAssetUrl + hash + assetBundleDM.id;	
				abRequest.hash = hash;
				abRequest.assetName = hash + assetBundleDM.id;
				
	//			if ( AppConfig.logAssetBundles )
	//				Log.Info("AssetBundle id: {0} enqueued for download from: {1}", assetBundleDM.id,abRequest.assetURL);
				abRequest.callback = delegate (AssetBundleQueue.Response abResponse) {
					if (abResponse.error != null) {
						DownloadingDone(abRequest.assetId);
						cb(abResponse.error, abRequest, null, null);
						return;
					}
					#if UNITY_IPHONE && !UNITY_EDITOR
					// Don't backup the data model to iCloud.
	//				iPhone.SetNoBackupFlag(abResponse.assetBundlePath);
					#endif
					abCache.SetValueAsync(CreateAssetKey(abRequest.assetId),new BundleCacheEntry(abRequest.assetName,abRequest.hash).Serialize());
					
					DownloadingDone(abRequest.assetId);
	//				if ( AppConfig.logAssetBundles )
	//					Log.Info("AssetBundle id: {0}, downloaded successfully to pah:{1}", assetBundleDM.id,abResponse.assetBundlePath);
					cb(null,abRequest,assetBundleDM,abResponse.assetBundlePath);
				};
				AssetBundleQueue.instance.Enqueue(abRequest);
			});
			
			DataModelQueue.instance.Enqueue(request);
		}


		private void Load(string assetKey, AssetBundleDataModel assetBundleDM, string assetPath, System.Collections.IList callbacks) {
			//check if we have it loaded
			if (loadedBundles.ContainsKey(assetKey)) {
				//Log.Info("Retrieving asset from memory cache. Id: " + assetBundleDM.id);
				LoadedBundle loadedBundle = loadedBundles[assetKey];
				
				foreach (GetAssetBundleCallback cb in callbacks) {
					cb(null, loadedBundle.ab, loadedBundle.m);
				}
				return;
			}
			
			string fullPath = System.IO.Path.Combine(persistentDataPath, assetPath);
			AssetBundle assetBundle = AssetBundle.LoadFromFile(fullPath);
			if (assetBundle == null) {
				bool fileExisted = System.IO.File.Exists(assetPath);
				RemoveFromDiskCacheAsync(assetBundleDM.id, assetKey, assetPath);
				foreach (GetAssetBundleCallback cb in callbacks) {
					if (fileExisted) {
						cb(string.Format("AssetBundle id: {0} couldn't be loaded by the system. Uknown error. Path: {1}", assetBundleDM.id, fullPath),null,assetBundleDM);
					} else {
						Log.Warning("AssetBundle id: {0} was deleted from the file system, trying downloading it. Path: {1}", assetBundleDM.id, fullPath);
	//					GetAssetBundle(assetBundleDM.id, cb);
					}
				}
				return;
			}
			
			/*Log.Info("Examining asset bundle: " + assetPath + ", id: " + assetBundleDM.id);
			Object[] res = assetBundle.LoadAll();
			foreach(Object o in res)
				Log.Info(o.name);
			Log.Info("Done");*/
			
			//add to loaded dictionary
			loadedBundles.Add(assetKey, new LoadedBundle(){ab=assetBundle,m=assetBundleDM});
			
			//execute callbacks
			foreach (GetAssetBundleCallback cb in callbacks) {
				cb(null,assetBundle,assetBundleDM);
			}
		}
		public AssetBundle LoadFromLocalPath(string localPath){
			string fullPath = persistentDataPath+"/"+localPath;
			Debug.Log(fullPath);
			AssetBundle assetBundle = AssetBundle.LoadFromFile(fullPath);
			if (assetBundle == null) {
				Debug.LogError("not found local path : " + localPath);
				return null;
			}
			return assetBundle;
		}
		public T LoadFromLocalPath<T>(string localPath,string assetName) where T : UnityEngine.Object{
			var assetBundle = LoadFromLocalPath(localPath);
			//return assetBundle.Load(assetName) as T;
			return assetBundle.LoadAsset<T>(assetName);
		}


		public delegate void GetAssetBundleCallback(string error, AssetBundle assetBundle, AssetBundleDataModel dm);
		private void RemoveFromDiskCacheAsync(int id, string assetCacheKey, string assetPath) {
			WorkQueue.Do(delegate() {
				abCache.Remove(assetCacheKey);
				try {
					string fullPath = System.IO.Path.Combine(persistentDataPath, assetPath);
					System.IO.File.Delete(fullPath);
				} catch (System.Exception e) {
					Log.Error("Couldn't delete assetBundle id: {0} at path: {1}. Error: {2}", id, assetPath, e.ToString());
				}
			});
		}

		private string CreateAssetKey(int id) {
			return "AssetId_" + id;
		}

		private string PlatformHash(AssetBundleDataModel assetBundleDM) {
			#if UNITY_IPHONE
			return assetBundleDM.iPhoneHash;
			#else
			return assetBundleDM.androidHash;
			#endif
		}

		private void DownloadingDone(int assetId) {
			lock(downloadingBundles) {
				downloadingBundles.Remove(CreateAssetKey(assetId));
			}
		}

	}

	public class AssetBundleDataModel {
		public int id;
		public int priority;
		public string androidHash;
		public string iPhoneHash;
	}
}