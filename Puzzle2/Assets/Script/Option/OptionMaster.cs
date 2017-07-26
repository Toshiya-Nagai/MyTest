using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class OptionMaster : BaseSceneMaster {

	public SaveOption save;
	public readonly int[] difficultys = new int[]{4,5,6};
	public readonly int[] cpuLevels = new int[]{1,2,3,4,5};
	protected override void Awake(){
		base.Awake();
		save = SaveOption.Load();
		save.Difficulty = 5;
		save.CpuLevel = 3;
	}

	public void DecideDifficulty(float value){
		Debug.Log("value : " + value);
		save.Difficulty = difficultys[(int)value];
	}

	public void DecideCpuLevel(float value){
		Debug.Log("value : " + value);
		save.CpuLevel = cpuLevels[(int)value];
	}

	public void MoveMainScene(){
		SaveOption.Save(save);
		SceneManager.LoadSceneAsync("Main");
	}

	public void MoveTitle(){
		SceneManager.LoadSceneAsync("Title");
	}

	public void MoveCharaSelect(){
		SceneManager.LoadSceneAsync("CharaSelect");
	}

	public void MoveMulti(){
		SaveOption.Save(save);
		SceneManager.LoadSceneAsync("Multi");
	}
}
