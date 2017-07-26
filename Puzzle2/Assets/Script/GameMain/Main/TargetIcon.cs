using UnityEngine;
using System.Collections;

public class TargetIcon : MonoBehaviour {
	public GameObject icon;
	public void Set(PieceController piece){
		var view = piece.GetComponent<PieceView>();
		icon.SetActive(true);
		icon.transform.SetParent(view.Target.transform);
		icon.transform.localPosition = Vector3.zero;
	}

	public void UnSet(){
		icon.SetActive(false);
	}
}
