using UnityEngine;
using System.Collections;

public class MainEntity : MonoBehaviour {
	public PuzzleModel puzzle;

	public void Init(){
		puzzle = new PuzzleModel();
		puzzle.ReflectOption(SaveOption.Load());
		puzzle.Init();
	}
}
