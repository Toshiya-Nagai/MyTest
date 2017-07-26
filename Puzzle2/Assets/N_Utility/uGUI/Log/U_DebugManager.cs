#if UNITY_EDITOR || DEVELOPMENT_BUILD
#define DEBUG_ENABLE
#endif

using UnityEngine;
using System.Collections;

public class U_DebugManager : Singleton<U_DebugManager> {
	void Update(){
#if DEBUG_ENABLE
		if(Input.GetKeyDown(KeyCode.Escape)){
			U_DebugMenu.OpenMenu();
		}
#endif
	}

	void OnLevelWasLoaded(){
		U_DebugMenu.RemoveAll();
	}
}
