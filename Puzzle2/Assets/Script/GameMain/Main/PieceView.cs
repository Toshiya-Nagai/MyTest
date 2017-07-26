using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class PieceView : View<PieceModel> {
	public U_UISprite Target;
	public PieceBlink Blink;
	public PieceBroken Broken;
	public PieceSpread Spread;
	public TimerController timer;
	public override void Set (PieceModel data){
		base.Set (data);
		UpdateUI();
		if(data.isHideArea){
			Target.color *= Color.gray;
		}
	}

	public override void UpdateUI (){
		//position
		if(Data.state != PieceState.Spread){
			Target.transform.localPosition = Vector3.up * timer.upperTime;
			//fall
			if(Data.gravityY > 0){Target.transform.localPosition += new Vector3(0.0f,-Data.gravityY,0.0f);}
		}
		//piece type
		if(Data.type > (int)PieceType.None){
			Target.spriteName = PieceModel.PieceSpriteNames[Data.type];
			if(!Data.isHideArea){Target.color = Color.white;}
		}else{
			if(!Data.isHideArea){Target.color = Color.clear;}
		}
		//chain
		if(Data.isChain && Data.state == PieceState.Chain){
			StartCoroutine(Break ());
		}
		if(Data.state == PieceState.ChainHind){
			StartCoroutine(HindToNormal());
		}
		//skill broken
		if(Data.state == PieceState.BrokenImmediate){
			broken();
		}
		//gameover	-> piece spread
		if(Data.state == PieceState.Spread){
			if(!Spread.IsEnable)
//				Spread.Spread(new Vector3(3-(Data.index%6),2.0f,0.0f));
				Spread.Spread(new Vector3(Random.Range(-4.0f,4.0f),Random.Range(2.0f,5.0f),0.0f));
		}
	}

	void Update(){
		UpdateUI();
	}


	IEnumerator Break(){
		Data.isChain = false;
		Data.state = PieceState.Broken;
		timer.PauseTimer();
		yield return StartCoroutine (blink());
		broken();
		timer.ResumeTimer();
	}

	IEnumerator blink(){
		int index = Blink.Emit(Target.transform.position);
		yield return new WaitForSeconds(2);
		Blink.DisableEmit(index);
	}

	void broken(){
		Broken.Emit(Target.transform.position,PieceModel.PieceSpriteColor[Data.type]);
		Data.Broken();
	}

	IEnumerator HindToNormal(){
		Data.isChain = false;
		Data.state = PieceState.ChangeHind;
		timer.PauseTimer();
		int index = Blink.Emit(Target.transform.position);
		yield return new WaitForSeconds(2);
		Blink.DisableEmit(index);
		this.Data.type = Random.Range((int)PieceType.Red,(int)PieceType.Yellow+1);
		Data.state = PieceState.Stand;
		timer.ResumeTimer();
	}
}
