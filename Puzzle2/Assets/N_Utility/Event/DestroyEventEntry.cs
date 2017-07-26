using UnityEngine;
using System.Collections;

public class DestroyEventEntry : MonoBehaviour {
	private EventManager _event;
	public GameObject Target;
	private string _targetInstanceId;

	void Start(){
		_event = EventManager.instance;
		_targetInstanceId = Target.GetInstanceID().ToString();
		_event.KeyEvent.Add(_targetInstanceId+"Close",x=>{
			Destroy(Target);
		});
	}

	public void Close(){
		_event.KeyEvent.Execute(_targetInstanceId+"Close",null);
	}

	void OnDestroy(){
		_event.KeyEvent.Remove(_targetInstanceId+"Close");
	}

}
