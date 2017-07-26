#define ENABLE_DEBUG
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using U_DebugUtil;

public class U_DebugController : Controller<IDebug> {

	public GameObject Window;
	public U_WindowScale WindowScale;
	void Start(){
	}

	void OnDestroy(){
		RemoveDefaultCommand(Data.CommandList);
		U_DebugMenu.mTransform = WindowScale.getWindow();
	}

	public override void Set (IDebug data){
		InsertDefaultCommand(data.CommandList);
		base.Set (data);
	}

	void InsertDefaultCommand(List<DebugCommand> commandList){
		commandList.notNull(x=>{
			commandList.Insert((int)DefaultCommand.Close,new DebugCommand("Close",()=>{Destroy (this.gameObject);}));
			commandList.Insert((int)DefaultCommand.PrevMenu,new DebugCommand("Prev Menu",()=>{
				DestroyImmediate (this.gameObject);
				U_DebugMenu.OpenMenu();
			}));
		});	
	}

	void RemoveDefaultCommand(List<DebugCommand> commandList){
		commandList.RemoveRange(0,(int)DefaultCommand.Max);
	}

	#region command
	protected static GameObject _currentDebugMenu;
	static public void Open(IDebug debugData){
		Open(debugData,U_DebugMenu.mTransform);
	}

	static public void Open(IDebug debugData,U_WindowScale.WindowTransform t){
		#if ENABLE_DEBUG
		if(debugData == null){return;}
		if(_currentDebugMenu != null){DestroyDebugMenu();}
		_currentDebugMenu = Instantiate((GameObject)Resources.Load("U_Debug")) as GameObject;
		if(_currentDebugMenu != null){
			U_DebugController debug = _currentDebugMenu.GetComponent<U_DebugController>();
			debug.Set(debugData);
			debug.Observe ();
			if(t.worldPosition != Vector3.zero){
				debug.WindowScale.Set(t);
			}
		}
		#endif
	}
	static public void DestroyDebugMenu(){
		if(_currentDebugMenu != null){
			Destroy(_currentDebugMenu);
			_currentDebugMenu = null;
		}
	}
	#endregion
}

public enum DefaultCommand{
	Close,
	PrevMenu,
	Max,
}


