using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class DragAndDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler,IDropHandler,ICanvasRaycastFilter{
	public RectTransform target;
	public bool isRaycastHit;

	public UnityEvent OnDropCallback;

	void Awake(){
		isRaycastHit = true;
	}

	public void OnBeginDrag(PointerEventData e){
		target = this.gameObject.transform as RectTransform;
		target.SetAsLastSibling();
		isRaycastHit = false;
	}
	public void OnDrag(PointerEventData e){
		target.position = e.position;
	}
	public void OnEndDrag(PointerEventData e){
		target.SetAsFirstSibling();
		isRaycastHit = true;
	}
	public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera){
		return isRaycastHit;
	}

	public void OnDrop(PointerEventData e){
	}

	public void Log(){
		Debug.Log("AAAAAAAAAAAAAA");
	}
}