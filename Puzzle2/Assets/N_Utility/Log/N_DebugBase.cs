using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using N_DebugUtil;

abstract public class N_DebugBase : MonoBehaviour {
	#region member
	//Gui arrangement list
	abstract public List<GUIModel> GuiList{get;set;}
	
	#endregion
	abstract protected void Awake();
	abstract protected void InitGuiList();
	abstract protected void ArrangementGUI();

	virtual protected void OnGUI(){
		ArrangementGUI();
	}

	protected void Close(){
		Destroy(this.gameObject);
	}
}
