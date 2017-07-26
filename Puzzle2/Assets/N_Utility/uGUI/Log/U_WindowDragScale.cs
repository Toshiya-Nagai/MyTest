using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class U_WindowDragScale : MonoBehaviour,IPointerDownHandler,IPointerUpHandler {

	public RectTransform window;
	bool isDrag;
//	public bool isWidth;
	public bool isRight;
	public bool isLeft;
	public bool isHeight;

	Vector2 mMousePosition;
	Vector3 cacheLocalPosition;
	Vector2 cacheAnchor;
	Vector2 cacheDelta;

	void Start(){
		setCache(window);
	}
	void Update(){
		if(isDrag){
			if(isRight){
				setWidth(Input.mousePosition.x - mMousePosition.x);
			}else if(isLeft){
				setWidth(mMousePosition.x - Input.mousePosition.x);
			}else{
				setHeight(mMousePosition.y- Input.mousePosition.y );
			}
		}
	}

	void setHeight(float val){
		window.transform.localPosition = cacheLocalPosition + new Vector3(0f,-val/2f,0f);
		window.anchoredPosition = cacheAnchor + new Vector2(0,-val/2f);
		window.sizeDelta = cacheDelta + new Vector2(0,val);
	}

	void setWidth(float val){
		window.sizeDelta = cacheDelta + new Vector2(val,0);
	}

	#region IPointerClickHandler implementation
	public void OnPointerDown (PointerEventData eventData){
		isDrag = true;
		mMousePosition = eventData.pressPosition;
		setCache(window);
	}
	#endregion

	#region IPointerUpHandler implementation
	public void OnPointerUp (PointerEventData eventData){
		isDrag = false;
		setCache(window);
	}
	#endregion

	void setCache(RectTransform target){
		cacheLocalPosition = target.localPosition;
		cacheAnchor = target.anchoredPosition;
		cacheDelta = target.sizeDelta;
	}

}
