#if UNITY_EDITOR || DEVELOPMENT_BUILD
#define DEBUG_ENABLE
#endif

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using U_DebugUtil;

public class U_InputButton : U_DebugButton {
	public U_InputExecute inputExecute;
	protected InputCommand command;

	public override void Set (DebugCommand data){
		base.Set (data);
		command = data as InputCommand;
	}

	public override void Action(){
		#if DEBUG_ENABLE
		inputExecute.gameObject.SetActive (true);
		inputExecute.Set(command);
		#endif
	}
}
