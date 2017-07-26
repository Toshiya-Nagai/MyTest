using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class U_DebugView : View<U_DebugUtil.IDebug> {
	public RectTransform grid;
	public GameObject baseButton;
	public GameObject InputButton;
	public Text LogText;

	public override void Set (U_DebugUtil.IDebug data){
		setButton(data.CommandList);
		base.Set (data);
	}

	void Update(){
		UpdateUI ();
	}

	public override void UpdateUI (){
		LogText.text = Data.DebugLog();
	}

	private void setButton(List<U_DebugUtil.DebugCommand> commandList){
		foreach(var buf in commandList){
			GameObject button = createButton (buf);
			button.transform.SetParent(grid);
			button.GetComponent<U_DebugButton>().Set(buf);
			button.transform.localScale = Vector3.one;
		}
	}

	GameObject createButton(U_DebugUtil.DebugCommand command){
		GameObject button = null;
		if(command is U_DebugUtil.InputCommand){
			button = Instantiate (InputButton) as GameObject;
		}else{
			button = Instantiate (baseButton) as GameObject;
		}
		button.SetActive (true);
		return button;
	}
}
