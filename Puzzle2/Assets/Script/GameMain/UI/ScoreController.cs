using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreController : Controller<ScoreModel> {
	public Text ScoreText;
	public LevelController levelController;

	int difficulty;
	public override void Set (ScoreModel data){
		base.Set (data);
		Observe();
		var option = SaveOption.Load();
		Data.DifficultyBonus = option.Difficulty;
		Debug.Log(option.Difficulty);
	}

	public void AddScore(int comboCount,int chainCount){
		Data.AddScore(comboCount,chainCount);
//		Observe();
	}

	public override void Observe (){
		ScoreText.text = U_Localization.GetLocalizeText("Score",Data.Score);
	}

	void Update(){
		if(Data.IsAddAnimation){
			Data.UpdateScore();
			Observe();
		}
	}

}
