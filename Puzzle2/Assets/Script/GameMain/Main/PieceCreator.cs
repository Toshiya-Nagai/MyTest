using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PieceCreator : Controller<PuzzleModel> {
	public GameObject BaseObj;
	public GameObject Grid;
//	public List<GameObject> pieces{get;private set;}

	public List<PieceController> pieces{get;private set;}

	public override void Set (PuzzleModel data){
		base.Set (data);
		createPiece(data);
	}

	void createPiece(PuzzleModel puzzle){
		pieces = new List<PieceController>();
		for(int i = 0;i < puzzle.height*puzzle.width;i++){
			var piece = Instantiate(BaseObj) as GameObject;
			piece.SetActive (true);
			piece.transform.SetParent(Grid.transform);
			piece.transform.Reset();
			piece.name = i.ToString();
			var controller = piece.GetComponent<PieceController>();
			controller.Set(puzzle.map[i]);
			pieces.Add(controller);
		}
	}


}
