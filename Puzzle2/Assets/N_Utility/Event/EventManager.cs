#define DEBUG_MODE
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UniLinq;

public enum KeyEventName{
	//use BaseChart
	FlowEnd,
	//use main game
	SetTargetIcon,
	UnSetTargetIcon,
}

/// <summary>
/// N_ event manager.
/// event type is class type
/// </summary>
public class EventManager : Singleton<EventManager>{
	public EventDictionary<object> KeyEvent;
	public EventQueue BackEvent;
	void Awake(){
		KeyEvent = new EventDictionary<object>();
		BackEvent = new EventQueue();
	}
#if DEBUG_MODE
	public List<string> EntryNames;
	public int onBackCount;
	void Update(){
		EntryNames = KeyEvent.EntryNames.ToList();
		onBackCount = BackEvent.queueCount;
	}
#endif
	void OnLevelWasLoaded(){
		KeyEvent.notNull(x=>{x.Clear();});
		BackEvent.notNull(x=>{x.Clear();});
	}
}

public class EventDictionary<TModel>{
	private Dictionary<string,UnityAction<TModel>> _actionDic;
	public IEnumerable<string> EntryNames{
		get{return _actionDic.Keys;}
	}
	
	public EventDictionary(){
		_actionDic = new Dictionary<string, UnityAction<TModel>>();
	}
	/// <summary>
	/// Add the specified key and action. use Start method!!
	/// </summary>
	/// <param name="key">Key.</param>
	/// <param name="action">Action.</param>
	public void Add(string key,UnityAction<TModel> action){
		if(_actionDic.ContainsKey(key)){
			Log.Error("contains key : " + key);
		}
		_actionDic.Add(key,action);
	}
	public void Add(KeyEventName key,UnityAction<TModel> action){
		Add(key.ToString(),action);
	}
	//contains true : insert, false : add
	public void Insert(string key,UnityAction<TModel> action){
		if(ContainsKey(key)){
			_actionDic[key] += action;
		}else{
			Add (key,action);
		}
	}
	public void Insert(KeyEventName key,UnityAction<TModel> action){
		Insert(key.ToString(),action);
	}

	public void Release(string key,UnityAction<TModel> action){
		if(ContainsKey(key)){
			_actionDic[key] -= action;
		}else{
			Log.Error("not subscribe event : " + key);
		}
	}
	public void Release(KeyEventName key,UnityAction<TModel> action){
		Release(key.ToString(),action);
	}
	public void Remove(string key){
		if(_actionDic.ContainsKey(key)){
			_actionDic[key] = null;
			_actionDic.Remove(key);
		}else{
			Log.Error("not found event : " + key);
		}
	}
	public void Remove(KeyEventName key){
		Remove(key.ToString());
	}

	public bool ContainsKey(string key){
		return _actionDic.ContainsKey(key);
	}
	public bool ContainsKey(KeyEventName key){
		return ContainsKey (key.ToString());
	}
	public void ExecuteAll(TModel send){
		foreach(var buf in _actionDic){
			buf.Value(send);
		}
	}
	public void ExecuteAllOnce(TModel send){
		foreach(var buf in _actionDic){
			buf.Value(send);
		}
		Clear();
	}
	public void Execute(string key,TModel send){
		if(ContainsKey(key)){
			_actionDic[key](send);
		}else{
			Log.Error("Not Found Event Key : " + key);
		}
	}
	public void Execute(KeyEventName key,TModel send){
		Execute(key.ToString(),send);
	}
	public void ExecuteOnce(string key,TModel send){
		if(ContainsKey(key)){
			_actionDic[key](send);
			Remove(key);
		}else{
			Log.Error("Not Found Event Key : " + key);
		}
	}
	public void ExecuteOnce(KeyEventName key,TModel send){
		Execute(key.ToString(),send);
	}
	public void Clear(){
		_actionDic.Clear();
	}

}


/// <summary>
/// Event queue.
/// use Android Back Button Action
/// </summary>
public class EventQueue{
	private Queue<UnityAction> _actionQueue;
	public int queueCount{
		get{return _actionQueue.Count;}
	}

	public EventQueue(){
		_actionQueue = new Queue<UnityAction>();
	}

	public void Enqueue(UnityAction action){
		if(_actionQueue.Contains(action)){
			Log.Error("contains event : {0}",action.ToString());
		}else{
			_actionQueue.Enqueue(action);
		}
	}
	public void Dequeue(){
 		if(_actionQueue.Count <= 0){
			Log.Error("entry event count is 0");
		}else{
			var action = _actionQueue.Dequeue();
			action();
		}
	}

	public void Remove(int removeCount){
		for(int i = 0;i < removeCount;i++){
			if(_actionQueue.Count > 0){
				_actionQueue.Dequeue();
			}else{
				Log.Error("event count is 0. not remove event");
				break;
			}
		}
	}

	public void Clear(){
		_actionQueue.Clear();
	}

	public bool Contains(UnityAction action){
		return _actionQueue.Contains(action);
	}
}
