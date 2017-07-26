using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class BaseSceneMaster : MonoBehaviour {
	protected GameManager gameManager;
	protected virtual void Awake(){
		gameManager = GameManager.instance;
	}
}
