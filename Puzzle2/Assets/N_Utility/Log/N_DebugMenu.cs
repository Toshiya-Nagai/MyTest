#define DEBUG_ENABLE
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using N_DebugUtil;

/// <summary>
/// N_ debug menu.
/// debug start menu.[GC collect],[Resource UnLoad],[Caching Clean]
/// if add debug menu => N_DebugMenu.AddMenu(IDebugModel); N_DebugMenu.Create();
/// </summary>
public class N_DebugMenu : N_DebugBase {
	#region member
	//debug menu list
	static public List<IDebug> DebugMenuList{get;set;}
	static private GameObject _currentMenu;
	//Gui arrangement list
	public override List<GUIModel> GuiList {get;set;}
	public TextAsset PlacementText;
	#endregion
	//call - game start
	static N_DebugMenu(){
//		Debug.Log("static constructor");
		DebugMenuList = new List<IDebug>();
		N_DebugMemory menu = new N_DebugMemory();
		AddMenu(menu);
	}

	override protected void Awake(){
		DontDestroyOnLoad(this.gameObject);
		InitGuiList();
	}
	void Start(){
		CheckDataNull();
		setMenuButton();
	}

	void OnDestroy(){
	}

	override protected void InitGuiList(){
		DebugMenuContent baseContent = new DebugMenuContent();		//log disp only
		GuiList = new List<GUIModel>(){
			//command area (top left)
			new GUIBox(),			//back
			//log area (top right)
			new GUIBox(),			//back
			new GUIDataLogLabel(baseContent),
		};
		DebugTextReader reader = new DebugTextReader(PlacementText);
		GuiList = reader.GetTextAssetSerialize(GuiList);
	}
	override protected void ArrangementGUI(){
		updateArrangementGui();
	}
	private void updateArrangementGui(){
		foreach(var buf in GuiList){
			buf.Arrangement();
		}
	}

	private void setMenuButton(){
		//close
		GUIButton close = new GUIButton(new Rect(55,40,160,60),"Close");
		close.ButtonEvent = Close;
		GuiList.Add(close);

		for(int i = 0;i < DebugMenuList.Count;i++){
#if UNITY_ANDROID && !UNITY_EDITOR
			DebugMenuButton button = new DebugMenuButton(new Rect(55,140+(i*100),160,60),DebugMenuList[i].ButtonText,DebugMenuList[i]);
#else
			DebugMenuButton button = new DebugMenuButton(new Rect(55,100+(i*60),160,60),DebugMenuList[i].ButtonText,DebugMenuList[i]);
#endif
			GuiList.Add(button);
		}
	}
	#region static method
	static public void AddMenu(IDebug anDebugModel){
		DebugMenuList.Add(anDebugModel);
	}
	static public void RemoveMenu(int index){
		DebugMenuList.RemoveAt(index);
	}

	static public void RemoveMenu(IDebug anDebugModel){
		if(DebugMenuList.Contains(anDebugModel)){
			DebugMenuList.Remove(anDebugModel);
		}else{
			Log.Warning("not found remove contains");
		}
	}
	static public void RemoveAll(){
		DebugMenuList.Clear();
	}

	static private List<IDebug> CheckDataNull(){
		for(int i = 0;i < DebugMenuList.Count;i++){
			if(DebugMenuList[i] == null){
				RemoveMenu(i);
			}
		}
		return DebugMenuList;
	}

	static public void OpenMenu(){
#if DEBUG_ENABLE
		if(_currentMenu != null){return;}
		_currentMenu = Instantiate((GameObject)Resources.Load("DebugMenu")) as GameObject;
#endif
	}
	static public void CloseMenu(){
		if(_currentMenu != null){
			Destroy(_currentMenu);
			_currentMenu = null;
		}
	}
	#endregion
}

