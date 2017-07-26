using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using MiniJSON;

public class SplitSpriteMaker : EditorWindow {

	public Texture Target;
	public TextAsset JsonText;

	// メニューに登録
	[MenuItem("uGUI/Open/SpriteSplitMaker")]
	static void Init(){
		EditorWindow.GetWindow (typeof(SplitSpriteMaker));
	}

	// 表示ウインドウの内容
	void OnGUI() {
		JsonText = (TextAsset)EditorGUILayout.ObjectField("Split Json",JsonText,typeof(TextAsset),false);
		Target = (Texture)EditorGUILayout.ObjectField("Target Texture",Target,typeof(Texture),false);
		if(GUILayout.Button("Split Sprite")){
			if(Target == null){this.ShowNotification(new GUIContent("No Target Texture Selected"));}
			else if(JsonText == null){this.ShowNotification(new GUIContent("No JsonText Selected"));}
			else{SplitSprite(Target,JsonText);}
		}
	}

	void SplitSprite(Texture target,TextAsset json){
		var texturePath = AssetDatabase.GetAssetPath(target);
		Debug.Log("texture Path : " + texturePath);
		var importer = (TextureImporter)TextureImporter.GetAtPath(texturePath);
		if(importer.textureType != TextureImporterType.Sprite){
			this.ShowNotification(new GUIContent("No Texture ImportType is Sprite"));
			return;
		}

		// read frame
		var jsonData = (Dictionary<string, object>)Json.Deserialize (json.text);
		var spriteFrames = (Dictionary<string, object>)jsonData ["frames"];
		
		// read meta file
		var meta = (Dictionary<string, object>)jsonData ["meta"];
		var size = (Dictionary<string, object>)meta ["size"];
		var height = int.Parse (size ["h"].ToString ());
		
		// json to meta
		List<SpriteMetaData> metas = new List<SpriteMetaData> ();
		foreach (string spriteName in spriteFrames.Keys) {
			var dic = (Dictionary<string, object>)spriteFrames [spriteName];
			var metadata = JsonToSpriteMetadata (spriteName, height, (Dictionary<string, object>)dic ["frame"]);
			JsonBorderSettings(importer,ref metadata);
			metas.Add (metadata);
		}
		// import
		importer.spriteImportMode = SpriteImportMode.Multiple;
		importer.spritesheet = metas.ToArray ();

		// appry
		EditorUtility.SetDirty (importer);
		AssetDatabase.ImportAsset (texturePath, ImportAssetOptions.ForceUncompressedImport);
		Resources.UnloadAsset(importer);
	}

	static SpriteMetaData JsonToSpriteMetadata (string spriteName, int height, Dictionary<string, object> frame)
	{
		SpriteMetaData meta = new SpriteMetaData ();
		int x = int.Parse (frame ["x"].ToString ());
		int y = int.Parse (frame ["y"].ToString ());
		int w = int.Parse (frame ["w"].ToString ());
		int h = int.Parse (frame ["h"].ToString ());
		meta.name = spriteName;
		meta.rect = new Rect (x, height - (y + h), w, h);
		meta.pivot = new Vector2 (0.5f, 0.5f);
		return meta;
	}
	
	static void JsonBorderSettings(TextureImporter importer,ref SpriteMetaData meta){
		string name = meta.name;
		var metas = importer.spritesheet.Where(x=>x.name == name);
		if(metas.Count() > 1){Debug.LogError("name exist is " + metas.Count());}
		foreach(var buf in metas){
			meta.border = buf.border;
		}
	}
}
