using UnityEngine;
using System.Collections;
using U_DebugUtil;

public class TitleMaster : BaseSceneMaster,IDebug {
	public void MoveSolo(){
		Application.LoadLevelAsync("Option");
	}

	public void MoveRanking(){
		Application.LoadLevelAsync("Ranking");
	}

	public void MoveCharaSelect(){
		Application.LoadLevelAsync("CharaSelect");
	}

	public void MoveMulti(){
		Application.LoadLevelAsync("MultiOption");
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
