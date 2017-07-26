#if UNITY_EDITOR || DEVELOPMENT_BUILD
#define DEBUG_ENABLE
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using U_DebugUtil;
using UniLinq;

/// <summary>
/// N_ debug menu.
/// debug start menu.[GC collect],[Resource UnLoad],[Caching Clean]
/// if add debug menu => N_DebugMenu.AddMenu(IDebugModel); N_DebugMenu.Create();
/// </summary>
public class U_DebugMenu : U_DebugController,IDebug {
	#region member
	//debug menu list
	static public U_WindowScale.WindowTransform mTransform;
	static List<IDebug> DebugMenuList{get;set;}
	static List<IDebug> defaultCommand{get;set;}
	#endregion
	//call - game start
	static U_DebugMenu(){
		Debug.Log("static constructor");
		DebugMenuList = new List<IDebug>();
		defaultCommand = new List<IDebug>();
		defaultCommand.Add(new U_DebugMemory());
		defaultCommand.Add (new U_DebugLogConsole());
		defaultCommand.Add(new U_DetailConsole());
	}
	
	new public void Awake(){
		DontDestroyOnLoad(this.gameObject);
		base.Awake();
	}
	void Start(){
		CheckDataNull();
		Set(this);
	}
	
	void OnDestroy(){
		U_DebugMenu.mTransform = WindowScale.getWindow();
	}
	
	
	public string ButtonText{
		get{return "Debug Menu";}
	}
	public string DebugLog(){
		return "Debug Menu";
	}
	public List<DebugCommand> CommandList{
		get{
			var list = DebugMenuList.Select(x=>new DebugCommand(x.ButtonText,()=>{
				var t = this.WindowScale.getWindow();
				U_DebugController.Open(x,t);
				Destroy(this.gameObject);
			})).ToList();
			list.Insert(0,new DebugCommand("Close",()=>{Destroy (this.gameObject);}));
			list.InsertRange(1,defaultCommand.Select(x=>convert(x)));
			return list;
		}
		set{}
	}
	
	DebugCommand convert(IDebug debug){
		return new DebugCommand(debug.ButtonText,()=>{
			var t = this.WindowScale.getWindow();
			U_DebugController.Open(debug,t);
			Destroy(this.gameObject);
		});
	}
	
	#region static method
	static public void AddMenu(IDebug anDebugModel){
		DebugMenuList.Add(anDebugModel);
	}
	static public void RemoveMenu(int index){
		DebugMenuList.RemoveAt(index);
	}
	
	static public void RemoveMenu(IDebug anDebugModel){
		DebugMenuList.Remove(anDebugModel);
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
		OpenMenu(mTransform);
		#endif
	}
	
	static public void OpenMenu(U_WindowScale.WindowTransform t){
		#if DEBUG_ENABLE
		if(_currentDebugMenu != null){return;}
		_currentDebugMenu = Instantiate((GameObject)Resources.Load("U_DebugMenu")) as GameObject;
		var controller = _currentDebugMenu.GetComponent<U_DebugController>();
		if(controller != null && t != null){
			controller.WindowScale.Set(t);
		}
		#endif
	}
	
	static public void CloseMenu(){
		if(_currentDebugMenu != null){
			Destroy(_currentDebugMenu);
			_currentDebugMenu = null;
		}
	}
	#endregion
}

