using UnityEngine;
using System.Collections;

public class MultiMaster : BaseMainMaster {

	public PuzzleActor Player;
	public PuzzleActor Rival;

	public int Win;		//0 -> battle, 1 -> 1PWin , 2 -> 2PWin

	public override IEnumerator Initialize(){
#if (UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR
		Screen.orientation = ScreenOrientation.LandscapeLeft;
#endif
		yield return StartCoroutine(Player.Initialize());
		yield return StartCoroutine(Rival.Initialize());
	}
	
	public override IEnumerator Setup(){
		yield return StartCoroutine(Player.Setup());
		yield return StartCoroutine(Rival.Setup());
		Player.GameOverAction += (actor)=>{
			actor.Showdown();
			actor.entity.puzzle.NoticePieceSpread();
			Rival.Showdown();
			Win = 1;
			nextState = true;
		};
		Rival.GameOverAction += (cpuActor) =>{
			Player.Showdown();
			cpuActor.Showdown();
			cpuActor.entity.puzzle.NoticePieceSpread();
			Win = 2;
			nextState = true;
		};
	}
	
	public override IEnumerator Ready(){
		yield return new WaitForSeconds(2);
		Player.Ready();
		Rival.Ready();
		Debug.Log("Ready GO");
	}
	
	public override IEnumerator Play(){
		yield return null;
	}
	
	public override IEnumerator GameOver(){
		yield return new WaitForSeconds(2.0f);
		Debug.Log("Game Over");
	}
	
	public override IEnumerator End (){
		Debug.Log ("End");
#if (UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR
		Screen.orientation = ScreenOrientation.Portrait;
#endif
		yield return Application.LoadLevelAsync("Title");
	}
}
