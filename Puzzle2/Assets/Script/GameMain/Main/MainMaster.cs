using UnityEngine;
using System.Collections;

public class MainMaster : BaseMainMaster {
	public PuzzleActor Player;

	public override IEnumerator Initialize (){
		yield return StartCoroutine(Player.Initialize());
	}
	public override IEnumerator Setup (){
		yield return StartCoroutine(Player.Setup());
		Player.GameOverAction += (actor)=>{
			Player.entity.puzzle.NoticePieceSpread();
			nextState = true;
		};
	}
	public override IEnumerator Ready (){
		#region Ready Animation
		yield return new WaitForSeconds(2.0f);
		#endregion
		Player.Ready();
	}
	public override IEnumerator Play (){
		yield return null;
	}
	public override IEnumerator GameOver (){
		yield return new WaitForSeconds(2.0f);
		Debug.Log("Game Over");
	}
	public override IEnumerator End (){
		Debug.Log ("End");
		yield return Application.LoadLevelAsync("Ranking");
	}
}
