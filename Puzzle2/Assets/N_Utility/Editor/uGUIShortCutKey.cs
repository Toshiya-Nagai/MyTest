#define ENABLE_SHORTCUTKEY
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Text.RegularExpressions;
using System.Reflection;

public class uGUIShortCutKey : EditorWindow {
#if ENABLE_SHORTCUTKEY
	[MenuItem("uGUI/Create/Image #&s")]
	static void createImage(){
		CommonExecuteMenuItem("GameObject/UI/Image");
	}

	[MenuItem("uGUI/Create/Text #&l")]
	static void createText(){
		CommonExecuteMenuItem("GameObject/UI/Text");
	}

	[MenuItem("uGUI/Create/RawImage #&t")]
	static void createRawImage(){
		CommonExecuteMenuItem("GameObject/UI/RawImage");
	}

	static void CommonExecuteMenuItem(string iStr){
		EditorApplication.ExecuteMenuItem(iStr);
	}
#endif
}
