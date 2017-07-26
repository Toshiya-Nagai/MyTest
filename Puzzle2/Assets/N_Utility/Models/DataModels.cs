using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UniLinq;


/// <summary>
/// Object Model
/// (Do not hold by the entity only on types) declare the type of data to be used in the game
/// ※ method that is not described process except that it is absolutely necessary
/// </summary>

public class ScoreModel{
	public int Score{get{return (int)calcScore;}}
	public int DifficultyBonus{set;private get;}
	float calcScore;
	float addValue;
	public bool IsAddAnimation{get{return addValue > 0.0f;}}
	public ScoreModel(){
		calcScore = 0f;
		addValue = 0f;
	}
	public void AddScore(int comboCount,int chainCount){
		addValue += (comboCount+1) * (DifficultyBonus) * (chainCount * 10);
	}

	public void UpdateScore(){
		calcScore += 10;
		addValue -= 10;
	}
}

public class HindModel{
	public int Hind;
	public void AddHind(int comboCount,int chainCount){
		Hind += comboCount*chainCount;
	}
}

public class RankingDataModel{
	public int rank;
	public string name;
	public int score;
}

public class SaveOption{
	public int Difficulty;
	public int CpuLevel;
	static public SaveOption Load(){
		SaveOption option = new SaveOption();
		option.Difficulty = PlayerPrefs.GetInt(PrefsKey.Difficulty.ToString(),3);
		option.CpuLevel = PlayerPrefs.GetInt(PrefsKey.CpuLevel.ToString(),5);
		return option;
	}
	static public void Save(SaveOption target){
		PlayerPrefs.SetInt(PrefsKey.Difficulty.ToString(),target.Difficulty);
		PlayerPrefs.SetInt(PrefsKey.CpuLevel.ToString(),target.CpuLevel);
	}
}
[System.Serializable]
public class CharaOption{
	public int PlayerId;
	public int RivalId;
	public CharaOption(){
	}
	static public CharaOption Load(){
		CharaOption option = new CharaOption();
		option.PlayerId = PlayerPrefs.GetInt(PrefsKey.PlayerId.ToString(),4);
		option.RivalId = PlayerPrefs.GetInt(PrefsKey.RivalId.ToString(),2);
		return option;
	}

	static public void Save(CharaOption target){
		PlayerPrefs.SetInt(PrefsKey.PlayerId.ToString(),target.PlayerId);
		PlayerPrefs.SetInt(PrefsKey.RivalId.ToString(),target.RivalId);
	}
}

public class TempScoreCache{
	public int Score;
	public TempScoreCache(){}
	public TempScoreCache(int score){
		this.Score = score;
	}
//	static public TempScoreCache Load(){
//		TempScoreCache m = new TempScoreCache();
//		m.Score = PlayerPrefs.GetInt(PrefsKey.ResultScore.ToString (),0);
//		return m;
//	}

	static public TempScoreCache OnceLoad(){
		TempScoreCache m = new TempScoreCache();
		m.Score = PlayerPrefs.GetInt(PrefsKey.ResultScore.ToString (),0);
		PlayerPrefs.DeleteKey(PrefsKey.ResultScore.ToString());
		return m;
	}

	static public void Save(TempScoreCache target){
		PlayerPrefs.SetInt(PrefsKey.ResultScore.ToString(),target.Score);
	}
}

public class Chara{
	public CharaDataModel info;
	public SkillDataModel skill;
	public Chara(CharaDataModel chara,SkillDataModel skill){
		this.info = chara;
		this.skill = skill;
	}
}

public class SkillDataModel{
	public int id;
	public string name;
	public string comment;
	public int targetId;
}

public class CharaDataModel{
	public int id;
	public string name;
	public string comment;
	public string spriteName;
	public int skillId;
}

public enum PrefsKey{
	None,
	Difficulty,
	CpuLevel,
	ResultScore,
	PlayerId,
	RivalId,
}

[System.Serializable]
public class LevelModel{
	public int level;
	public int rate;
	public int speed;
}


