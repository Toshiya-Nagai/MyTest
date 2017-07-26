using UnityEngine;
using System.Collections;

public class InstanceEventEntry : MonoBehaviour {
	private EventManager _event;
	private string _targetInstanceId;
	public GameObject Target;
	public GameObject Instance;

	void Start(){
		_event = EventManager.instance;
		_targetInstanceId = Target.GetInstanceID().ToString();
		_event.KeyEvent.Add(_targetInstanceId+"Create",x=>{
			Instance = Instantiate(Target) as GameObject;
		});
	}
	
	public GameObject Create(){
		_event.KeyEvent.Execute(_targetInstanceId+"Create",null);
		return Instance;
	}

	void OnDestroy(){
		_event.KeyEvent.Remove(_targetInstanceId+"Create");
	}
}
