using UnityEngine;
using System.Collections;

public class UIController : UIBaseController {
	public ScoreController score;
	public LevelController level;

	public ScoreModel scoreEntity;

	protected override void Awake (){
		U_Localization.language = "Japanese";
		base.Awake ();
		UIEvent.ChainEvent += score.AddScore;
		UIEvent.ChainEvent += level.CalcLevel;
		UIEvent.GameOverEvent += TempScore;
		scoreEntity = new ScoreModel();
		score.Set(scoreEntity);
	}

	public override IEnumerator Setup (){
		level.score = scoreEntity;
		yield return StartCoroutine(level.Load());
	}

	void TempScore(){
		TempScoreCache.Save(new TempScoreCache(scoreEntity.Score));
		Debug.Log("Temp Save Score : " + scoreEntity.Score);
	}
}
