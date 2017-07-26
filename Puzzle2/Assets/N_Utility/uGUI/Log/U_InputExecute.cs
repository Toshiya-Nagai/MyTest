using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class U_InputExecute : U_InputButton {
	public InputField input;
	public Button button;

	public override void Set (U_DebugUtil.DebugCommand data){
		base.Set (data);
		input.text = command.DefaultFieldStr;
		input.ActivateInputField();
	}

	public override void Action (){
		command.inputStr = input.text;
		command.Command();
		this.gameObject.SetActive(false);
	}
}
