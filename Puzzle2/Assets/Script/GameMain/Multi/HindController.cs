using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using U_DebugUtil;
using System.Collections.Generic;

public class HindController : Controller<HindModel>,IDebug {
	public HindController Rival;
	public MainEntity entity;
	public Text HindText;

	public void AddHindRival(int comboCount,int chainCount){
		Rival.Data.AddHind(comboCount,chainCount);
	}

	public void InstanceHindPiece(){
		if(Data.Hind > 0){
			int hindCount = (Data.Hind > entity.puzzle.width)?entity.puzzle.width:Data.Hind;
			int instanceCount = entity.puzzle.InstanceHindPiece(hindCount);
			Data.Hind -= instanceCount;
		}
	}

	public override void Observe (){
		HindText.text = U_Localization.GetLocalizeText("HindNum",Data.Hind);
	}

	void Update(){
		this.Observe();
	}
	
	#region IDebug implementation
	List<DebugCommand> commandList;
	void Start(){
		commandList = new List<DebugCommand>(){new DebugCommand("Add Hind",addHind)};
		U_DebugMenu.AddMenu(this);
	}

	public string DebugLog (){
		return "Hind Count : " + Data.Hind.ToString();
	}
	public string ButtonText {
		get {
			return "Hind "+this.gameObject.name;
		}
	}
	public List<DebugCommand> CommandList {
		get {
			return commandList;
		}
		set {
		}
	}

	void addHind(){
		this.Data.Hind += 10;
	}
	#endregion
}
