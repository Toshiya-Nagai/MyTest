using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using N_Creator;

public class CharaSelectMaster : BaseSceneMaster {

	public CharaSelector selector;
	public List<Chara> charaEntity{get;private set;}

	void Start(){
		InputManager.StartBlockingInput ();
		charaEntity = new List<Chara>();
		SqliteDatabase.instance.ExecuteOnState(SqliteDatabase.State.BootReady,()=>{
			foreach(var buf in CacheManager.charaDM){
				charaEntity.Add(new Chara(buf,CacheManager.skillDM.Find(x=>x.id == buf.skillId)));
			}
			createNode();
			selector.Set(charaEntity);
			InputManager.StopBlockingInput ();
		});
	}

	private ObjCreator<Chara,CharaInfo> creator;
	public GameObject baseObj;
	public GameObject grid;
	void createNode(){
		creator = new ObjCreator<Chara,CharaInfo>(grid,baseObj,charaEntity.Count);
		creator.Create(charaEntity);
	}


	public void MoveTitle(){
		Application.LoadLevelAsync("Title");
	}

	public void MoveMultiOption(){
		CharaOption.Save(selector.charaOption);
		Application.LoadLevelAsync("MultiOption");
	}


}
