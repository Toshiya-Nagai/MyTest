using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class U_Drag : MonoBehaviour,IPointerDownHandler,IPointerUpHandler {
	public GameObject MoveTarget;
	Vector3 mPosition;
	bool isDrag;

	void Update(){
		if(isDrag){
			MoveTarget.transform.position = Input.mousePosition - mPosition;
		}
	}



	#region IPointerDownHandler implementation
	public void OnPointerDown (PointerEventData eventData){
		isDrag = true;
		mPosition = (Vector3)eventData.pressPosition - MoveTarget.transform.position;
	}
	#endregion

	#region IPointerUpHandler implementation
	public void OnPointerUp (PointerEventData eventData){
		isDrag = false;
	}
	#endregion
}
