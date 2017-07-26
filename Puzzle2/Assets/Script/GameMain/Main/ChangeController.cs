using UnityEngine;
using System.Collections;

public class ChangeController : MonoBehaviour {
	PieceController prevData;
	private int moveCount;
	public bool MoveEnable{get{return (moveCount == 0);}}
	public bool isChangeEnd;

	void Start(){
		prevData = null;
	}

	public void Change(SendPieceData recive){
		//no change
		if(prevData == null && (recive.change == recive.current)){return;}
		//prev
//		else if(prevData != null && (recive.current == recive.change) && !MoveEnable){
//			recive.change = prevData;
//			prev(recive);
//		}
		//change
		else if(MoveEnable){
			change(recive);
			prevData = recive.change;
			moveCount++;
		}
	}
	
	public void ChangeEnd(){
		if(prevData == null){return;}
		prevData = null;
		moveCount = 0;
		isChangeEnd = true;
	}
	
	private void change(SendPieceData recive){
//		changeValue(recive);
//		changeView(recive);
		recive.change.ChangePiece(recive.current);
		recive.change.Observe();
		recive.current.Observe();
	}
	
	private void prev(SendPieceData recive){
		moveCount -= 1;
		change(recive);
		prevData = null;
	}
}

public class SendPieceData{
	public PieceController current;
	public PieceController change;
}
