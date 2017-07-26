using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharaSelector : Controller<List<Chara>> {

	public CharaOption charaOption;
	public CharaInfo detail;
	public State state;
	public enum State{
		Select1P,
		Select2P,
	}

	public override void Set (List<Chara> data){
		base.Set (data);
//		charaOption = new CharaOption();
		charaOption = CharaOption.Load();
		Select1P();
	}

	public void Select(int id){
		if(state == State.Select1P){
			charaOption.PlayerId = id;
		}else{
			charaOption.RivalId = id;
		}
	}

	public void Select1P(){
		state = State.Select1P;
		detail.Set(Data.Find(x=>x.info.id == charaOption.PlayerId));
		detail.Observe();
	}

	public void Select2P(){
		state = State.Select2P;
		detail.Set(Data.Find(x=>x.info.id == charaOption.RivalId));
		detail.Observe();
	}
}
