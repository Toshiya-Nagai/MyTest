#define ENABLE_DEBUG
using UnityEngine;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using N_DebugUtil;

/// <summary>
/// N_ debug.
/// data model log display.
/// log label line 5,
/// BaseDataModel -> N_DataModels.cs
/// [DebugLabel] prefab arrangement resources folder
/// </summary>

public class N_Debug : N_DebugBase {
	#region member

	public BaseDebug DebugController{protected get;set;}
	
	//Gui arrangement list
	public override List<GUIModel> GuiList {get;set;}

	public TextAsset PlacementText;
	private readonly Rect BaseButtonArea = new Rect(55,40,160,60);		//close button area
	#endregion
	
	override protected void Awake(){
		this.DebugController = new BaseDebug();
	}

	void Start(){
	}

	void OnDestroy(){
	}
	
	override protected void InitGuiList(){
		GuiList = new List<GUIModel>(){
			//command area (top left)
			new GUIBox(),			//back
			//log area (top right)
			new GUIBox(),			//back
			new GUIDataLogLabel(this.DebugController),
		};
		DebugTextReader reader = new DebugTextReader(this.PlacementText);
		GuiList = reader.GetTextAssetSerialize(GuiList);
#if UNITY_EDITOR
//		SetEditorGUI(GuiList);
#endif
		//command list set
		SetCommandList(DebugController.CommandList);
	}

	protected void SetCommandList(List<DebugCommand> anCommandList){
		//close
		GUIButton close = new GUIButton(BaseButtonArea,"Close");
		close.ButtonEvent = Close;
		GuiList.Add(close);
		if(anCommandList != null){
			for(int i = 0;i < anCommandList.Count;i++){
#if UNITY_ANDROID
				GUIButton button = new GUIButton(new Rect(BaseButtonArea.x,BaseButtonArea.y+((i+1)*100),BaseButtonArea.width,BaseButtonArea.height),anCommandList[i].CommandName);
#else
				GUIButton button = new GUIButton(new Rect(BaseButtonArea.x,BaseButtonArea.y+((i+1)*50),BaseButtonArea.width,BaseButtonArea.height),anCommandList[i].CommandName);
#endif
				button.ButtonEvent = anCommandList[i].Command;
				GuiList.Add(button);
			}
		}
	}

	
	override protected void ArrangementGUI(){
		updateArrangementGui();
	}
	
	private void updateArrangementGui(){
		foreach(var buf in GuiList){
			buf.Arrangement();
		}
	}

	override protected void OnGUI(){
		if(DebugController.DebugModel == null){
			Close();
		}
		ArrangementGUI();
	}


	static GameObject _currentDebugMenu;
	static public void CreateDebugMenu(IDebug anDebugData){
		#if ENABLE_DEBUG
		if(anDebugData == null){return;}
		if(_currentDebugMenu != null){DestroyDebugMenu();}
//		GameObject debugObj = Instantiate((GameObject)Resources.Load("DebugLabel")) as GameObject;
		_currentDebugMenu = Instantiate((GameObject)Resources.Load("DebugLabel")) as GameObject;
		if(_currentDebugMenu != null){
			N_Debug debug = _currentDebugMenu.GetComponent<N_Debug>();
			debug.DebugController.DebugModel = anDebugData;
			debug.InitGuiList();
		}
		#endif
	}

	static public void DestroyDebugMenu(){
		if(_currentDebugMenu != null){
			Destroy(_currentDebugMenu);
			_currentDebugMenu = null;
		}
	}
}
