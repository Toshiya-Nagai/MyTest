using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PieceSpread : MonoBehaviour {
	public Image Target;
	public bool IsEnable;
	public float Gravity = 9.8f;
	public Vector3 velocity;

	void Update(){
		if(IsEnable){
			Target.transform.localPosition += velocity;
			velocity += Vector3.up * -Gravity * Time.deltaTime;
		}
	}

	public void Spread(Vector3 velocity){
		this.velocity = velocity;
		IsEnable = true;
	}
}
