#pragma warning disable 0618
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

public class BitMapFontMaker : EditorWindow {
	public Font customFontObj;
	public TextAsset fontPosTbl;
	public Texture fontTexture;

	#region window property
	enum SettingOption{
		Generate,
		Override,
	}
	SettingOption option = SettingOption.Generate;
	#endregion
	
	
	// メニューに登録
	[MenuItem("uGUI/Open/BitMapFontMaker")]
	static void Init() {
		EditorWindow.GetWindow(typeof(BitMapFontMaker));
	}
	
	// 表示ウインドウの内容
	void OnGUI() {
		option = (SettingOption)EditorGUILayout.EnumPopup("option",option);
		//override
		if(option == SettingOption.Override){
			// Custom Font 登録欄
			customFontObj = (Font)EditorGUILayout.ObjectField("Custom Font", customFontObj, typeof(Font), false);
			
			// フォント画像指定欄
			fontTexture = (Texture)EditorGUILayout.ObjectField("Font Texture", fontTexture, typeof(Texture), false);
			
			// 文字テーブルファイル登録欄
			//		EditorGUILayout.Space();
			EditorGUILayout.LabelField("Use Font Table Text File", EditorStyles.boldLabel);
			fontPosTbl = (TextAsset)EditorGUILayout.ObjectField("Font Table Text File", fontPosTbl, typeof(TextAsset), false);
			
			// 実行ボタン
			if (GUILayout.Button("Override Custom Font")) {
				if (customFontObj == null) this.ShowNotification(new GUIContent("No Custom Font selected"));
				else if (fontTexture == null) this.ShowNotification(new GUIContent("No Font Texture selected"));
				else if (fontPosTbl == null) this.ShowNotification(new GUIContent("No Font Position Table file selected"));
				else OverrideFont(customFontObj, fontTexture, fontPosTbl);
			}
		}else{	//generate

			// フォント画像指定欄
			fontTexture = (Texture)EditorGUILayout.ObjectField("Font Texture", fontTexture, typeof(Texture), false);
			
			// 文字テーブルファイル登録欄
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Use Font Table Text File", EditorStyles.boldLabel);
			fontPosTbl = (TextAsset)EditorGUILayout.ObjectField("Font Table Text File", fontPosTbl, typeof(TextAsset), false);
			
			// 実行ボタン
			if (GUILayout.Button("Generate Custom Font")) {
				if (fontTexture == null) this.ShowNotification(new GUIContent("No Font Texture selected"));
				else if (fontPosTbl == null) this.ShowNotification(new GUIContent("No Font Position Table file selected"));
				else GenerateFont(fontTexture,fontPosTbl);
			}
		}
	}


	void GenerateFont(Texture texture,TextAsset fontText){
		Font font = new Font();
		byte[] textBinary = System.Text.Encoding.UTF8.GetBytes(fontText.text);
		Load (ref font,textBinary,texture);
		var path = Path.GetDirectoryName(AssetDatabase.GetAssetPath(texture));
		var exportPath = path + "/" + Path.GetFileNameWithoutExtension(texture.name);

		// Create material
		Shader shader = Shader.Find("UI/Unlit/Transparent");
		Material material = new Material(shader);
		material.mainTexture = texture;
		AssetDatabase.CreateAsset(material, exportPath + ".mat");

		// Create font
		font.material = material;
		AssetDatabase.CreateAsset(font,exportPath+".fontsettings");
	}

	void OverrideFont(Font font,Texture texture,TextAsset fontText){
		byte[] textBinary = System.Text.Encoding.UTF8.GetBytes(fontText.text);
		Load (ref font,textBinary,texture);
	}


	/// <summary>
	/// Helper function that retrieves the string value of the key=value pair.
	/// </summary>
	
	string GetString (string s){
		int idx = s.IndexOf('=');
		return (idx == -1) ? "" : s.Substring(idx + 1);
	}
	
	/// <summary>
	/// Helper function that retrieves the integer value of the key=value pair.
	/// </summary>
	
	int GetInt (string s){
		int val = 0;
		string text = GetString(s);
		#if UNITY_FLASH
		try { val = int.Parse(text); } catch (System.Exception) { }
		#else
		int.TryParse(text, out val);
		#endif
		return val;
	}
	
	/// <summary>
	/// Reload the font data.
	/// </summary>
	
	void Load (ref Font font,byte[] bytes,Texture texture){
		List<CharacterInfo> characters = new List<CharacterInfo>();
		float texW = texture.width;
		float texH = texture.height;
		if (bytes != null){
			ByteReader reader = new ByteReader(bytes);
			char[] separator = new char[] { ' ' };
			int index = 0;
			while (reader.canRead){
				string line = reader.ReadLine();
				if (string.IsNullOrEmpty(line)) break;
				string[] split = line.Split(separator, System.StringSplitOptions.RemoveEmptyEntries);
				int len = split.Length;
				
				if (split[0] == "char")
				{
					// Expected data style:
					// char id=13 x=506 y=62 width=3 height=3 xoffset=-1 yoffset=50 xadvance=0 page=0 chnl=15
					
					//					int channel = (len > 10) ? GetInt(split[10]) : 15;
					
					if (len > 9 && GetInt(split[9]) > 0){
						Debug.LogError("Your font was exported with more than one texture. Only one texture is supported by NGUI.\n" +
						               "You need to re-export your font, enlarging the texture's dimensions until everything fits into just one texture.");
						break;
					}
					
					if (len > 8){
						CharacterInfo info = new CharacterInfo();
						info.index = GetInt(split[1]);
						info.width = GetInt(split[8]);
						info.flipped = false;
						Rect rect = new Rect();
						rect.x			= (float)GetInt(split[2]) / texW;
						rect.y			= (float)GetInt(split[3]) / texH;
						rect.width		= (float)GetInt(split[4]) / texW;
						rect.height	= (float)GetInt(split[5]) / texH;
						rect.y = 1f - rect.y - rect.height;
						info.uv = rect;
						rect = new Rect();
						rect.x = GetInt(split[6]);
						rect.y	= GetInt(split[7]);
						rect.width	= GetInt(split[4]);
						rect.height	= GetInt(split[5]);
						rect.y = -rect.y;
						rect.height = -rect.height;
						info.vert = rect;
						characters.Add(info);
						
					}
					else{
						Debug.LogError ("Failed Font");
						break;
					}
				}
				
				else if (split[0] == "page"){
					// Expected data style:
					// page id=0 file="textureName.png"
					
					if (len >= 2){
						font.name = GetString(split[2]);
					}
				}
				index++;
			}
			font.characterInfo = characters.ToArray();
		}
	}
}