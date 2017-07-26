using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using U_DebugUtil;
using System.Collections.Generic;

using UniLinq;

public class SkillController : Controller<Skill>,IDebug {
	public Text Text;
	bool waitUseSkill;
	State state;
	enum State{
		None,
		Effect,
	}
	public PuzzleActor Player;
	public PuzzleActor Rival;

	PuzzleActor skillTarget;		//skill target is player or rival
	public SkillParticle skillParticle;

	void Start(){
		commandList = new List<DebugCommand>(){new DebugCommand("Max Skill",()=>{
				Data.skillEnergy = 100.0f;
			})};
		U_DebugMenu.AddMenu(this);
	}

	public void Set(SkillDataModel skillDM){
		setSkillTarget(skillDM);
		base.Set(new Skill(skillDM,skillTarget.entity.puzzle));
	}

	void setSkillTarget(SkillDataModel skillDM){
		this.skillTarget = (skillDM.targetId == 1)?Rival:Player;
	}

	void Update(){
		if(waitUseSkill && skillTarget.timerController.isEnableSkill() && state == State.None){
			StartCoroutine(skillExecute());
		}
		this.Observe();
	}

	public override void Observe (){
		if(Data != null){
			Text.text = U_Localization.GetLocalizeText("SkillNum",(int)Data.skillEnergy);
		}
	}

	public void SetupSkill(){
		if(Data.skillEnergy >= 100.0f){
			waitUseSkill = true;
		}
	}

	IEnumerator skillExecute(){
		skillTarget.timerController.isEnable = false;
		state = State.Effect;
		waitUseSkill = false;
		Data.skillEnergy = 0.0f;
		var targetIndexs = Data.GetSkillTarget();
		List<GameObject> targets = new List<GameObject>();
		targetIndexs.ForEach (x=>{
			targets.Add(skillTarget.pieceCreator.pieces[x].pieceView.Target.gameObject);
		});
		skillParticle.EmitMany(Data.skillDM.id,targets);
		yield return new WaitForSeconds(2.0f);
		Data.Execute();
		skillTarget.timerController.isEnable = true;
		state = State.None;
		Debug.Log ("name : " + this.gameObject.name);
	}

	public void AddEnergy(int comboCount,int chainCount){
		Data.AddSkillEnergy(comboCount,chainCount);
	}

	#region IDebug implementation
	public string DebugLog (){
		return U_Localization.GetLocalizeText("SkillDebug",Data.skillEnergy);
	}
	public string ButtonText {
		get {
			return "Skill "+this.gameObject.name;
		}
	}
	List<DebugCommand> commandList;
	public List<DebugCommand> CommandList {
		get {
			return commandList;
		}
		set {
		}
	}
	#endregion

}
