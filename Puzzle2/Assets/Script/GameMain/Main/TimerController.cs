using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using U_DebugUtil;

public class TimerController : MonoBehaviour,IDebug {
	[HideInInspector]public float upperTime;
	public float upperSpeed{private get;set;}
	float currentSpeed;
	readonly float UserUpSpeed = 256;

	public float upperLimit;
	public UnityAction upperAction;
	public bool isEnable = false;
	public bool isUpperMax;			//game over line
	public bool isFallPiece;		//pause timer

	readonly float gameoverTime = 3.0f;
	float currentGameOverTime;
	public UnityAction GameOverAction;
	public bool IsGameOver;

	int pauseTimerCount = 0;
	public void PauseTimer(){
		pauseTimerCount++;
	}
	public void ResumeTimer(){
		pauseTimerCount--;
	}

	public bool IsPauseTimer(){
		return pauseTimerCount > 0;
	}

	public void SetInitCurrentSpeed(float speed){
		currentSpeed = speed;
	}

	public void SetSpeed(float speed){
		upperSpeed = speed;
		SetInitCurrentSpeed(speed);
	}

	void Update(){
		//puzzle timer
		if(isUpper()){
			upperTime += currentSpeed/30;
			if(upperTime > upperLimit){
				if(upperAction != null){upperAction();}
				upperTime = 0;
			}
		}
		//upper max -> gameover timer start
		if(isMaxUpper()){
			currentGameOverTime += Time.deltaTime;
			if(currentGameOverTime > gameoverTime){
				IsGameOver = true;
				isEnable = false;
				if(GameOverAction != null)
					GameOverAction();
			}
		}else if(isEnable && !isUpperMax){
			//cancel gameover timer
			currentGameOverTime = 0.0f;
		}
		if(Input.GetKeyDown(KeyCode.S)){
			currentSpeed = UserUpSpeed;
		}
		if(Input.GetKeyUp(KeyCode.S)){
			currentSpeed = upperSpeed;
		}
	}
	public void SpeedUp(){
		currentSpeed = UserUpSpeed;
	}

	public void SpeedNormal(){
		currentSpeed = upperSpeed;
	}

	public bool isUpper(){
		return isEnable && !IsPauseTimer() && !isUpperMax && !isFallPiece;
	}

	public bool isEnableSkill(){
		return isEnable && !IsPauseTimer() && !isFallPiece;
	}

	public bool isMaxUpper(){
		return isEnable && !IsPauseTimer() && isUpperMax && !isFallPiece;
	}

	#region IDebug implementation
	void Start(){
		U_DebugMenu.AddMenu(this);
		commandList = new List<DebugCommand>();
		commandList.Add (new DebugCommand("Play",()=>{currentSpeed = upperSpeed;}));
		commandList.Add (new DebugCommand("Stop",()=>{currentSpeed = 0;}));
	}

	public string DebugLog (){
		return "isFallPiece : " + isFallPiece.ToString();
	}
	public string ButtonText {
		get {
			return "Timer " + this.gameObject.name;
		}
	}
//	List<DebugCommand> commandList = new List<DebugCommand>();
	List<DebugCommand> commandList;
	public System.Collections.Generic.List<DebugCommand> CommandList {
		get {
			return commandList;
		}
		set {
		}
	}
	#endregion
}
