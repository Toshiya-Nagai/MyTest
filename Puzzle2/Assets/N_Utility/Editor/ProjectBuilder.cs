using UnityEngine;
using UnityEditor;
using System.Collections;

/// <summary>
/// Project builder.
/// this script is command line call.
/// use jenkins tool function
/// </summary>
public class ProjectBuilder : MonoBehaviour {
	private static void BuildAndroid(){
		string[] scene = {"Assets/NotExport/test.unity"};

		string dstDevice = "C:/Jenkins/jobs/test/workspace/Device.apk";
//		string dstSimulator = "Simulator";

		BuildOptions opt = BuildOptions.SymlinkLibraries | BuildOptions.Development | BuildOptions.AllowDebugging;

		EditorUserBuildSettings.symlinkLibraries = true;
		EditorUserBuildSettings.development = true;

		PlayerSettings.bundleIdentifier = "jp.co.mytest";
		PlayerSettings.statusBarHidden = true;

		string errorMsg_Device = BuildPipeline.BuildPlayer(scene,dstDevice,BuildTarget.Android,opt);
		if (string.IsNullOrEmpty(errorMsg_Device)){
			Debug.Log("///////////	device build succeeded ///////////");
		} else {
			Debug.Log("///////////	device build failure	 ///////////");
			Debug.LogError(errorMsg_Device);
		}
	}

	private static void Test(){
//		Debug.Log("/////////////// test ////////////////");
		print("//////////////// test///////////////////");
	}
}
