using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using U_DebugUtil;

public class PuzzleController : Controller<PuzzleModel>,IDebug {

	public enum State{
		Init,
		Setup,
		Ready,
		Play,
		End,
	}

	public State state;
	public bool IsUpdate;
	public TimerController Timer;

	#region State Controll
	public void Init(){
		state = State.Init;
	}
	public void Ready(){
		Timer.isEnable = true;;
		state = State.Ready;
		IsUpdate = true;
	}
	#endregion

	//call setup
	public override void Set (PuzzleModel data){
		base.Set (data);
		Timer.upperAction += PieceUp;
		Timer.isEnable = false;
		this.Data.puzzleEvent.FallEvent += fallStateAction;
	}

	void PieceUp(){
		Data.PieceUp();
	}

	void checkLimit(){
		if(Data.isLimitPieces()){
			Timer.isUpperMax = true;
		}else{
			Timer.isUpperMax = false;
		}
	}

	void Update(){
		if(IsUpdate){
			Data.Gravity();
			Data.Judge();
			checkLimit();
			if(Timer.isUpper ())
				Data.comboCount = 0;
		}

		if(Input.GetKeyDown(KeyCode.Escape)){
			U_DebugMenu.OpenMenu();
		}
	}

	void fallStateAction(int fallCount){
		//fall is time stop
		if(fallCount > 0){
			Timer.isFallPiece=true;
		}
		else{Timer.isFallPiece=false;}
	}


	#region IDebug implementation
	void Start(){
		U_DebugMenu.AddMenu(this);
	}

	public string DebugLog (){
		string state = "";
		for(int i = 0;i < Data.map.Count;i++){
			state += (int)Data.map[i].state;
			state += (i%Data.width != Data.width-1)?",":"\n";
		}

		state += "\n \n" + "type" + "  \n";
		for(int i = Data.map.Count-1;i >= 0;i--){
			state += (int)Data.map[i].type;
			state += (i%Data.width == 0)?"\n":",";
		}
		return state;
	}

	public string ButtonText {
		get {
			return "puzzle " + this.gameObject.name;
		}
	}
	List<DebugCommand> commandList = new List<DebugCommand>();
	public List<DebugCommand> CommandList {
		get {
			return commandList;
		}
		set {
		}
	}
	#endregion
}

