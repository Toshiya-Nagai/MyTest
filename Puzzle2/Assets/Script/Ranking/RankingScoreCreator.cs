using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using N_Creator;

public class RankingScoreCreator : Controller<List<RankingDataModel>> {

	ObjCreator<RankingDataModel,RankingScoreController> creator;

	public GameObject ParentObj;
	public GameObject BaseObj;
	public override void Set (List<RankingDataModel> data){
		base.Set (data);
		creator = new ObjCreator<RankingDataModel, RankingScoreController>(ParentObj,BaseObj,10);
		creator.Create(data);
	}

	public override void Observe (){
		base.Observe ();
	}
}
