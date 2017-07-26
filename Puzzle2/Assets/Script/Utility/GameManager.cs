using UnityEngine;
using System.Collections;

public class GameManager : Singleton<GameManager> {

	void Awake(){
		SqliteDatabase.Instantiate();
		U_Localization.language = "Japanese";
		U_DebugManager.Instantiate();
	}

	void OnApplicationQuit(){
		PlayerPrefs.DeleteAll();
	}
}
