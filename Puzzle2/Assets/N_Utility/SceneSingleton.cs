//#define CheckInstance
using UnityEngine;
using System.Collections;


abstract public class SceneSingleton<T> : MonoBehaviour where T: MonoBehaviour  {
	public T SetInstance;
	private static T _instance;
	public static T Instance{
		get{
			if(_instance == null){
//				throw new System.NullReferenceException();
				Debug.LogError ("_instance is null!!");
			}
			return _instance;
		}
		private set{_instance = value;}
	}

	protected virtual void Awake(){
#if CheckInstance
		CheckInstance();
#endif
		if(SetInstance != null){
			Instance = SetInstance;
		}else{
			SetInstance = this as T;
			Instance = SetInstance;
		}
	}

	public void OnDestroy(){
		Release();
		if(_instance != null){
			Destroy(_instance);
		}
		_instance = null;
	}

	//Resource Unload Method
	public abstract void Release();

	#region debug
	protected void CheckInstance(){
		T[] objects = GameObject.FindObjectsOfType<T>();
		if(objects.Length > 1){
			throw new System.Exception("'Singleton' object has multiple presence in the scene.");
		}
	}
	#endregion
}
