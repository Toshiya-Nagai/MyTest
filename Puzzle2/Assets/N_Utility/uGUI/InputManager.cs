using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class InputManager{

	private static EventSystem _eventSystem;
	public static EventSystem eventSystem{
		get{
			if(_eventSystem == null){
				_eventSystem = EventSystem.current;
			}
			return _eventSystem;
		}
		set{_eventSystem = value;}
	}

	public static void Process(){
		if(eventSystem == null){
			Debug.LogError("EventSystem Null");
			return;
		}
		if(blockingInputCount > 0 || pauseBlockingInput){eventSystem.enabled = false;}
		else{eventSystem.enabled = true;}
	}

	static int blockingInputCount = 0;
	// Temporarially blocks all input processing.
	public static void StartBlockingInput() {
		blockingInputCount++;
		Process();
	}
	// Resumes normal processing of all input.
	public static void StopBlockingInput() {
		if(blockingInputCount > 0){Debug.Log("blockingInputCount was zero when calling StopBlockingInput.");}
		blockingInputCount--;
		Process();
	}
	// Determins if input is currently being blocked.
	public static bool IsBlockingInput() {
		if(blockingInputCount > 0)
			return true;
		else
			return false;
	}

	private static bool pauseBlockingInput;
	public static void PauseBlockingInput() {
		pauseBlockingInput = true;
		Process();
	}
	public static void ContinueBlockingInput() {
		pauseBlockingInput = false;
		Process();
	}
}
