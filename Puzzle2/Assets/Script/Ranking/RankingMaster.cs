using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class RankingMaster : BaseSceneMaster {

	public List<RankingDataModel> ranking;
	public RankingScoreCreator rankingScoreCreator;
	public GameObject UICanvas;

//	IEnumerator Start(){
//		yield return StartCoroutine(load());
//
//		var result = LoadSoloResult();
//		if(result.Score != 0)
//			yield return StartCoroutine(UpdateRanking(ranking,result));
//
//		if(ranking != null){
//			rankingScoreCreator.Set(ranking);
//		}
//	}

	IEnumerator Start(){
		yield return StartCoroutine(load());
		
		var result = LoadSoloResult();
		int rank = ReflectResult(ref ranking,result);
		if(rank != -1){
			createRankInPopup(ranking,rank);
		}else if(ranking != null){
			rankingScoreCreator.Set(ranking);
		}
	}


	IEnumerator load(){
		bool isLoad = true;
		SqliteDatabase.instance.ExecuteOnState(SqliteDatabase.State.BootReady,()=>{
			DataModelQueue.instance.Enqueue(DataModelQueue.Request.Multi(null,delegate(DataModelAccess db, object inputs, out object outputs) {
				List<RankingDataModel> rank = null;
				var error = db.GetRanking(out rank);
				outputs = rank;
				return error;
			},delegate(DataModelQueue.Response response) {
				if(response.error != null){Debug.LogError(response.error);}
				ranking = response.dataModel as List<RankingDataModel>;
				isLoad = false;
			}));
		});
		while(isLoad){yield return null;}
	}

	TempScoreCache LoadSoloResult(){
		return TempScoreCache.OnceLoad();
	}

	IEnumerator UpdateRanking(List<RankingDataModel> ranks,TempScoreCache result){
		int rank = ranks.FindIndex(x=>x.score < result.Score);
		if(rank != -1){
			for(int i = ranks.Count-1;i > rank;i--){
				ranks[i].name = ranks[i-1].name;
				ranks[i].score = ranks[i-1].score;
			}
			ranks[rank].name = "";
			ranks[rank].score = result.Score;
		}
		bool isSave = true;
		DataModelQueue.instance.Enqueue(DataModelQueue.Request.Multi(null,delegate(DataModelAccess db, object inputs, out object outputs) {
			var error = db.UpdateRankings(ranks);
			outputs = null;
			return error;
		},delegate(DataModelQueue.Response response) {
			if(response.error != null){Debug.LogError(response.error.description);}	
			isSave = false;
			Debug.Log("Save");
		}));
		while(isSave){yield return null;}
	}

	IEnumerator ReflectRanking(List<RankingDataModel> ranks){
		bool isSave = true;
		DataModelQueue.instance.Enqueue(DataModelQueue.Request.Multi(null,delegate(DataModelAccess db, object inputs, out object outputs) {
			var error = db.UpdateRankings(ranks);
			outputs = null;
			return error;
		},delegate(DataModelQueue.Response response) {
			if(response.error != null){Debug.LogError(response.error.description);}	
			isSave = false;
			Debug.Log("Save");
		}));
		while(isSave){yield return null;}
	}

	int ReflectResult(ref List<RankingDataModel> ranks,TempScoreCache result){
		int rank = ranks.FindIndex(x=>x.score < result.Score);
		if(rank != -1){
			for(int i = ranks.Count-1;i > rank;i--){
				ranks[i].name = ranks[i-1].name;
				ranks[i].score = ranks[i-1].score;
			}
			ranks[rank].name = "";
			ranks[rank].score = result.Score;
		}
		return rank;
	}

	void createRankInPopup(List<RankingDataModel> ranks,int rank){
		string title = U_Localization.Get("RankInPopupTitle");
		string message = U_Localization.GetLocalizeText("RankInPopupMessage",rank+1);
		RankInPopupController popup = null;
		PopupDataModel m = PopupDataModel.Ok(title,message,()=>{
			ranks[rank].name = popup.input.text;
			if(popup.gameObject != null)
				Destroy(popup.gameObject);
			Coroutine.ExecuteAfterCoroutine(this,ReflectRanking(ranks),()=>{
				rankingScoreCreator.Set(ranking);
			});
//			StartCoroutine(ReflectRanking(ranks));
//			rankingScoreCreator.Set(ranking);
		});
		popup = RankInPopupController.CreateResources<RankInPopupController>("RankInPopup",m);
		popup.transform.SetParent(UICanvas.transform);
		popup.transform.Reset();
	}

	public void MoveTitle(){
		SceneManager.LoadSceneAsync("Title");
	}
}
