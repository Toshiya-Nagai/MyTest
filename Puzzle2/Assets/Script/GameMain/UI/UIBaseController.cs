using UnityEngine;
using System.Collections;

public abstract class UIBaseController : MonoBehaviour {
	protected virtual void Awake(){
		UIEvent = new PuzzleEvent();
	}
	public PuzzleEvent UIEvent{get;protected set;}

	public abstract IEnumerator Setup();
}
