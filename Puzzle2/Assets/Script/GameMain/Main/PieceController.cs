using UnityEngine;
using System.Collections;

public class PieceController : Controller<PieceModel> {

	public PieceView pieceView{
		get{return this.View as PieceView;}
	}

	public void ChangePiece(PieceController target){
		if(Data.changeEnable(target.Data.index) && (stateCheck() && target.stateCheck())){
			this.Data.TypeChange(target.Data);
			this.Data.StateChange(target.Data);
		}
	}
	bool stateCheck(){
		return (this.Data.state == PieceState.Stand || Data.state == PieceState.None);
	}
}
