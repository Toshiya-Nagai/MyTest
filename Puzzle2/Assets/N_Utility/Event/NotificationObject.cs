using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// change value send event class
/// used singleton object
/// public N_NotificationObject<int> score = new NotificationObject<int>(0);
/// control class :
/// GameController.Instance.score.action += ChangeScore;
/// void ChangeScore(int score){}		//change score call function
/// GameController.Instance.score += 100; 	->	GameController.Instance.score.action();
/// </summary>
public class NotificationObject<T> : IDisposable where T : System.IComparable{
	public delegate void NotificationAction(T t);
	private T data;

	//constructor
	public NotificationObject(T t){
		Value = t;
	}	
	public NotificationObject(){}

	//deathtructor
	~NotificationObject(){
		Dispose();
	}
	//event
	public event NotificationAction action;
	//prop
	public T Value{
		get{
			return data;
		}
		set{
			if( data.CompareTo(value) != 0){
				data = value;
				Notice ();
			}
		}
	}

	//change message
	private void Notice (){
		if (action != null)
			action (data);
	}

	//destroy call
	public void Dispose (){
		action = null;
	}
}
