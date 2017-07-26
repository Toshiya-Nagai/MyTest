using UnityEngine;
using System.Collections;

public class MultiUIController : UIBaseController {
	public HindController hind;
	public SkillController skill;
	public bool IsPlayer;
	CharaOption charaOption;
	public PuzzleActor Rival;

	protected override void Awake (){
		base.Awake();
		UIEvent.ChainEvent += hind.AddHindRival;
		UIEvent.TimerUpperEvent += hind.InstanceHindPiece;
		UIEvent.ChainEvent += skill.AddEnergy;
		hind.Set(new HindModel());
	}

	public override IEnumerator Setup (){
		bool isLoad = true;
		charaOption = CharaOption.Load();
		SqliteDatabase.instance.ExecuteOnState(SqliteDatabase.State.BootReady,()=>{
			int charaId = (IsPlayer)?charaOption.PlayerId:charaOption.RivalId;
			var m = CacheManager.charaDM.Find (chara=>chara.id == charaId);		//chara data model
			var skillDM = CacheManager.skillDM.Find(x=>x.id == m.skillId);		//skill data model
			skill.Set (skillDM);
			isLoad = false;
		});
		while(isLoad)yield return null;
	}
}
