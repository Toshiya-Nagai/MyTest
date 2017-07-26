#pragma warning disable 0618	
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class AssetBundleCreator : MonoBehaviour {

//	[MenuItem("Assets/Build AssetBundle Android UnCompress")]
//	static void CreateAndroidAssetBundle(){
//		foreach(var buf in Selection.objects){
//			BuildAssetBundle(1,new List<string>{buf.name},new List<Object>{buf},BuildAssetBundleOptions.UncompressedAssetBundle | BuildAssetBundleOptions.CollectDependencies,BuildTarget.Android,Application.dataPath+"/AssetBundle");
//		}
//	}
//	[MenuItem("Assets/Build AssetBundle Android Compress")]
//	static void BuildAndroidAssetBundleCompress(){
//		foreach(var buf in Selection.objects){
//			BuildAssetBundle(1,new List<string>{buf.name},new List<Object>{buf},BuildAssetBundleOptions.CompleteAssets | BuildAssetBundleOptions.CollectDependencies,BuildTarget.Android,Application.dataPath+"/AssetBundle");
//		}
//	}

	[MenuItem("Assets/Build AssetBundle Android UnCompress")]
	static void CreateAndroidAssetBundle(){
		Object[] selection = 
			Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
		foreach(var buf in selection){
			BuildAssetBundle(buf,null,BuildAssetBundleOptions.UncompressedAssetBundle | BuildAssetBundleOptions.CollectDependencies,BuildTarget.Android);
		}
	}
	[MenuItem("Assets/Build AssetBundle Android Compress")]
	static void BuildAndroidAssetBundleCompress(){
		Object[] selection = 
			Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
		foreach(var buf in selection){
			BuildAssetBundle(buf,null,BuildAssetBundleOptions.CompleteAssets | BuildAssetBundleOptions.CollectDependencies,BuildTarget.Android);
		}
	}


	private static string BuildAssetBundle(int bundleId, List<string> assetNames, List<UnityEngine.Object> unityAssets, BuildAssetBundleOptions options, BuildTarget buildTarget, string outputFolder) {
		string assetPath = string.Format("{0}/{1}_{2}", outputFolder, bundleId, buildTarget.ToString());
		System.Console.WriteLine("Starting to build "+assetPath);
		//PrefabDeflator
		bool done = BuildPipeline.BuildAssetBundleExplicitAssetNames(unityAssets.ToArray(), assetNames.ToArray(), assetPath,options,buildTarget);
		if (!done)
			return null;
		string hash = HashUtility.MD5(assetPath);
		string destinationPath = string.Format("{0}/{1}{2}", outputFolder, hash, bundleId);
		if(System.IO.File.Exists(destinationPath))
			System.IO.File.Delete(destinationPath);
		System.IO.File.Move(assetPath,destinationPath);
		Debug.Log("assetPath : " + assetPath + "  destinationPath : " + destinationPath);
		return hash;
	}



	private static void BuildAssetBundle(Object mainAsset, UnityEngine.Object[] unityAssets, BuildAssetBundleOptions options, BuildTarget buildTarget) {
		//PrefabDeflator
		bool done = BuildPipeline.BuildAssetBundle(mainAsset, unityAssets,Application.dataPath+"/AssetBundle/"+mainAsset.name,options,buildTarget);
			if (!done){
			Debug.LogError("failed build assetbundle");
		}
	}
}
