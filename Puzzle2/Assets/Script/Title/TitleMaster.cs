using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using U_DebugUtil;

public class TitleMaster : BaseSceneMaster,IDebug {
	public void MoveSolo(){
		SceneManager.LoadSceneAsync("Option");
	}

	public void MoveRanking(){
		SceneManager.LoadSceneAsync("Ranking");
	}

	public void MoveCharaSelect(){
		SceneManager.LoadSceneAsync("CharaSelect");
	}

	public void MoveMulti(){
		SceneManager.LoadSceneAsync("MultiOption");
	}


	protected override void Awake(){
		base.Awake ();
		U_DebugMenu.AddMenu (this);
	}
	#region IDebug implementation
	public string DebugLog (){
		throw new System.NotImplementedException ();
	}
	public string ButtonText {
		get {
			return "button";
		}
	}
	public System.Collections.Generic.List<DebugCommand> CommandList {
		get {
			throw new System.NotImplementedException ();
		}
		set {
			throw new System.NotImplementedException ();
		}
	}
	#endregion
}
