#if UNITY_EDITOR || DEVELOPMENT_BUILD
#define DEBUG_ENABLE
#endif

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using U_DebugUtil;

//public class U_DebugButton : Controller<U_DebugUtil.DebugCommand> {
//	public Text buttonText;
//
//	public override void Set (U_DebugUtil.DebugCommand data){
//		base.Set (data);
//		buttonText.text = data.CommandName;
//	}
//	public void Action(){
//#if DEBUG_ENABLE
//		if(Data == null){Debug.LogWarning("command is null");}
//		Data.notNull(x=>{Data.Command();});
//#endif
//	}
//}


public class U_DebugButton : Controller<U_DebugUtil.DebugCommand> {
	public Text buttonText;
	public override void Set (U_DebugUtil.DebugCommand data){
		base.Set (data);
		buttonText.text = data.CommandName;
	}
	public virtual void Action(){
		#if DEBUG_ENABLE
		if(Data == null){Debug.LogWarning("command is null");}
		Data.notNull(x=>{Data.Command();});
		#endif
	}
}
