using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;


public class PieceDrag : MonoBehaviour,IPointerDownHandler,IDragHandler,IPointerUpHandler,IPointerEnterHandler {
	public PieceController controller;
	public ChangeController changeController;
	public TargetIcon targetIcon;

	public void OnDrag (PointerEventData eventData){
		if(eventData.pointerPress == null || eventData.pointerEnter == null){return;}
		Debug.Log("press : " + eventData.pointerPress + "   enter : " + eventData.pointerEnter);
		if(eventData.pointerPress != eventData.pointerEnter){
			var target = eventData.pointerPress.GetComponent<PieceDrag>();
			var current = eventData.pointerEnter.GetComponent<PieceDrag>();
			if(target != null && current != null){
				SendPieceData send = new SendPieceData();
				send.current = current.controller;
				send.change = target.controller;
				changeController.Change(send);
			}
		}
	}
	public void OnPointerDown (PointerEventData eventData){}

	public void OnPointerUp (PointerEventData eventData){
		changeController.ChangeEnd();
		targetIcon.UnSet();
	}

	public void OnPointerEnter (PointerEventData eventData){
		targetIcon.Set(controller);
	}

}
