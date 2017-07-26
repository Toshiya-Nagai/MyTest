using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;

static public class GameObjectExtension{
	// Given a GameObject (this), get all instances of the given interface from the components.
	public static List<T> GetInterfacesInGameObject<T>( this GameObject obj ) where T : class{
		List<T>	ret = new List<T>();
		foreach ( Component c in obj.GetComponents<Component>())
		{
			T	it = c as T;
			if ( it != null )
				ret.Add(it);
		}
		return ret;
	}
	
	// Given a GameObject (this), get the first instance of a given interface from the components.
	public static T GetInterfaceInGameObject<T>( this GameObject obj ) where T : class{

		foreach ( Component c in obj.GetComponents<Component>() ){
			T	it = c as T;
			if ( it != null )
				return it;
		}
		return null;
	}
}

public static class NullExtension{
	public static void notNull<T>(this T obj,System.Action<T> action) where T : class{
		if(obj != null){action(obj);}
	}
	public static TResult notNull<TSource,TResult>(this TSource obj,System.Func<TSource,TResult> func) where TSource : class{
		return (obj == null)?default(TResult) : func(obj);
	}
}

public static class Coroutine{
	static public void ExecuteAfterCoroutine(MonoBehaviour bootup,IEnumerator coroutine, UnityAction action) {
		bootup.StartCoroutine(ExecuteAfterCoroutineActual(bootup,coroutine, action));
	}
	
	static public IEnumerator ExecuteAfterCoroutineActual(MonoBehaviour bootup,IEnumerator coroutine, UnityAction action) {
		yield return bootup.StartCoroutine(coroutine);
		action();
	}
}