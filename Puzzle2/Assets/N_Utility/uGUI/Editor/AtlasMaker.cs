using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.UI;
using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;

public class AtlasMaker : EditorWindow {

	#region member
	public Texture Target;
	public U_UIAtlas Atlas;
	AtlasOption option;
	#endregion

	enum AtlasOption{
		Generate,
		Override,
	}

	[MenuItem("uGUI/Open/AtlasMaker")]
	static void Init(){
		EditorWindow.GetWindow(typeof(AtlasMaker));
	}

	void OnGUI(){
		option = (AtlasOption)EditorGUILayout.EnumPopup("Option",option);
		EditorGUILayout.Space();
		if(option == AtlasOption.Generate){
			Target = (Texture)EditorGUILayout.ObjectField("Texture",Target,typeof(Texture),false);
			if(GUILayout.Button("Generate")){
				var texturePath = AssetDatabase.GetAssetPath(Target);
				Debug.Log("texture Path : " + texturePath);
				var ti = (TextureImporter)TextureImporter.GetAtPath(texturePath);
				createPrefab (ti);
				Resources.UnloadAsset(ti);
			}
		}else{
			Atlas  = (U_UIAtlas)EditorGUILayout.ObjectField("Atlas",Atlas,typeof(U_UIAtlas),false);
			Target = (Texture)EditorGUILayout.ObjectField("Texture",Target,typeof(Texture),false);
			if(GUILayout.Button("Override")){
				var texturePath = AssetDatabase.GetAssetPath(Target);
				Debug.Log("texture Path : " + texturePath);
				var ti = (TextureImporter)TextureImporter.GetAtPath(texturePath);
				overridePrefab (ti,Atlas);
				Resources.UnloadAsset(ti);
			}
		}
	}

	void createPrefab(TextureImporter ti){
		GameObject placeObj = new GameObject();
		Sprite[] sprites = AssetDatabase.LoadAllAssetRepresentationsAtPath(ti.assetPath).Select(x => x as Sprite).Where(x => x != null).ToArray();
		var targetPrefab = PrefabUtility.CreateEmptyPrefab(Path.GetDirectoryName(ti.assetPath) + "/" + Target.name +".prefab");
		var atlas = placeObj.AddComponent<U_UIAtlas>();
		atlas.Texture = Target;
		atlas.Sprites = new List<Sprite>(sprites);
		PrefabUtility.ReplacePrefab(placeObj,targetPrefab);
		DestroyImmediate(placeObj);
	}

	void overridePrefab(TextureImporter ti,U_UIAtlas targetPrefab){
		Sprite[] sprites = AssetDatabase.LoadAllAssetRepresentationsAtPath(ti.assetPath).Select(x => x as Sprite).Where(x => x != null).ToArray();
//		var path = AssetDatabase.GetAssetPath(targetPrefab);
		var obj = (GameObject)PrefabUtility.InstantiatePrefab(targetPrefab.gameObject);
		var atlas = obj.GetComponent<U_UIAtlas>();
		atlas.Sprites = new List<Sprite>(sprites);
		PrefabUtility.ReplacePrefab(obj,targetPrefab);
		DestroyImmediate(obj);
	}
}
