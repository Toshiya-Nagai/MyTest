using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class LevelController : Controller<List<LevelModel>> {

	public ScoreModel score{private get;set;}
	public TimerController timer;
	public Text levelText;

	public IEnumerator Load(){
		bool isLoad = true;
		DataModelQueue.instance.Enqueue(DataModelQueue.Request.Multi(null,delegate(DataModelAccess db, object inputs, out object outputs) {
			List<LevelModel> levels = null;
			var error = db.GetLevelCurve(out levels);
			if(error != null){Debug.LogError(error.description);}
			outputs = levels;
			return error;	
		},delegate(DataModelQueue.Response response) {
			List<LevelModel> levels = response.dataModel as List<LevelModel>;
			Debug.Log(levels);
			if(levels != null){
				Set(levels);
			}
			Observe();
			isLoad = false;
		}));
		while(isLoad){yield return null;}
	}

	int getLevel(){
		if(score == null || Data == null){
			Debug.Log("score : " + score + "  data : " + Data);
			return 0;
		}
		var index = Data.FindIndex(level=>level.rate > score.Score);
		if(index == -1){Debug.LogError("error search level");}
		return index;
	}

	public void CalcLevel(int comboCount,int chainCount){
		Observe();
	}

	public override void Observe (){
		int level = getLevel();
		levelText.text = U_Localization.GetLocalizeText("Level",level);
		setTimerSpeed(level);
	}

	void setTimerSpeed(int level){
		int levelIndex = level-1;
		timer.upperSpeed = Data[levelIndex].speed;
		timer.SetInitCurrentSpeed(Data[levelIndex].speed);
	}

}
