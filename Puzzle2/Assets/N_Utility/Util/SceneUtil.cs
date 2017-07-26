using UnityEngine;
using System.Collections;

//set necessarily scene name
public enum SceneName{
	Title,
	Main,
	MAX,
}

/// <summary>
/// scene control class
/// Intended to be to clarify the scene transition, to simplify the configuration
/// scene move :
/// ChangeScene(SCENE_NAME.Mypage);
/// </summary>
public class SceneUtil : Singleton<SceneUtil>{
	#region member
	[SerializeField]private string _currentSceneName;
	public string CurrentSceneName{
		get{return _currentSceneName;}
		set{_currentSceneName = value;}
	}
	[SerializeField]private string _prevSceneName;
	public string PrevSceneName{
		get{return _prevSceneName;}
		set{_prevSceneName = value;}
	}
	#endregion


	void Awake(){
		CurrentSceneName = Application.loadedLevelName;
		PrevSceneName = CurrentSceneName;
		//Test
		DontDestroyOnLoad(this.gameObject);
	}

	/// <summary>
	/// change scene (base)
	/// </summary>
	/// <param name="anSceneName">scene name.</param>
	public void BaseChangeScene(SceneName anSceneName){
		Application.LoadLevelAsync(anSceneName.ToString());
	}

	/// <summary>
	/// change scene (main thread)
	/// </summary>
	/// <param name="anSceneName">An scene name.</param>
	/// <param name="anBeforeCallback">call before change scene(not argument)</param>
	/// <param name="anEndCallback">call after change scene(not argument)</param>
	public void ChangeScene(SceneName anSceneName,System.Action anBeforeCallback = null,System.Action anEndCallback = null){
		if(anBeforeCallback != null){
			anBeforeCallback();
		}
		BaseChangeScene(anSceneName);
		if(anEndCallback != null){
			anEndCallback();
		}
	}

	/// <summary>
	/// change scene (sub thread)
	/// </summary>
	/// <returns>The scene.</returns>
	/// <param name="anBeforeCallback">call coroutine before change scene(not argument)</param>
	/// <param name="anEndCallback">call coroutine after change scene(not argument)</param>

	public IEnumerator ChangeScene(SceneName anSceneName,IEnumerator anBeforeCallback = null,IEnumerator anEndCallback = null){
		if(anBeforeCallback != null){
			yield return StartCoroutine(anBeforeCallback);
		}
		BaseChangeScene(anSceneName);
		if(anEndCallback != null){
			yield return StartCoroutine(anEndCallback);
		}
	}

	/// <summary>
	/// change scene name
	/// </summary>
	/// <param name="anCurrentSceneName">current scene name.</param>
	public void SetChangeSceneName(string anCurrentSceneName){
		PrevSceneName = CurrentSceneName;
		CurrentSceneName = anCurrentSceneName;
	}

	/// <summary>
	/// scene load complete call
	/// </summary>
	public void OnLevelWasLoaded(){
		SetChangeSceneName(Application.loadedLevelName);
	}

}