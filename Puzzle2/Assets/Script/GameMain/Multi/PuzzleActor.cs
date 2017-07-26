using UnityEngine;
using System.Collections;
using UnityEngine.Events;

//public class PuzzleActor : MonoBehaviour {
//	public MainEntity entity;
//	public PieceCreator pieceCreator;
//	public PuzzleController puzzleController;
//	public UIBaseController uiController;
//	public TimerController timerController;
//
//	public UnityAction GameOverAction;
//
//	public IEnumerator Initialize(){
//		GameManager.Instantiate();
//		entity.Init();
//		puzzleController.Init();
//		yield return null;
//	}
//	
//	public IEnumerator Setup(){
//		bool isDbLoad = true;
//		SqliteDatabase.instance.ExecuteOnState(SqliteDatabase.State.BootReady,()=>{
//			isDbLoad = false;
//		});
//		while(isDbLoad){yield return null;}
//		pieceCreator.Set (entity.puzzle);
//		puzzleController.Set(entity.puzzle);
//		yield return StartCoroutine(uiController.Setup());
//		entity.puzzle.puzzleEvent.ChainEvent += uiController.UIEvent.ChainEvent;
//		timerController.SetSpeed(10);
//		timerController.GameOverAction += uiController.UIEvent.GameOverEvent;
//		timerController.GameOverAction += ()=>{
//			//GameOver
//			if(this.GameOverAction != null)
//				this.GameOverAction();
//		};
//	}
//
//	public void Ready(){
//		puzzleController.Ready();
//	}
//	
//	public IEnumerator Play(){
//		yield return null;
//	}
//	
//	public IEnumerator GameOver(){
//		yield return new WaitForSeconds(2);
//		Debug.Log("Game Over");
//	}
//	
//	public IEnumerator End (){
//		Debug.Log ("End");
//		yield return Application.LoadLevelAsync("Ranking");
//	}
//
//}

public class PuzzleActor : MonoBehaviour {
	public MainEntity entity;
	public PieceCreator pieceCreator;
	public PuzzleController puzzleController;
	public UIBaseController uiController;
	public TimerController timerController;
	public ChangeController changeController;
	
	public UnityAction<PuzzleActor> GameOverAction;
	
	public virtual IEnumerator Initialize(){
		GameManager.Instantiate();
		entity.Init();
		puzzleController.Init();
		yield return null;
	}
	
	public virtual IEnumerator Setup(){
		bool isDbLoad = true;
		SqliteDatabase.instance.ExecuteOnState(SqliteDatabase.State.BootReady,()=>{
			isDbLoad = false;
		});
		while(isDbLoad){yield return null;}
		pieceCreator.Set (entity.puzzle);
		puzzleController.Set(entity.puzzle);
		timerController.SetSpeed(10);
		yield return StartCoroutine(uiController.Setup());
		//event set
		entity.puzzle.puzzleEvent.ChainEvent += uiController.UIEvent.ChainEvent;
		timerController.upperAction += uiController.UIEvent.TimerUpperEvent;
		timerController.GameOverAction += uiController.UIEvent.GameOverEvent;
		timerController.GameOverAction += ()=>{
			//GameOver
			if(this.GameOverAction != null)
				this.GameOverAction(this);
		};
	}
	
	public virtual void Ready(){
		puzzleController.Ready();
	}
	
	public virtual IEnumerator Play(){
		yield return null;
	}
	
	public virtual IEnumerator GameOver(){
		yield return new WaitForSeconds(2);
		Debug.Log("Game Over");
	}

	public virtual void Showdown(){
		timerController.isEnable = false;
	}
}

