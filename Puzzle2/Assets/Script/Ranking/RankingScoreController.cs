using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RankingScoreController : Controller<RankingDataModel> {
	public Text target;

	public override void Observe (){
		target.text = U_Localization.GetLocalizeText("Result",Data.rank,Data.name,Data.score);
	}
}
