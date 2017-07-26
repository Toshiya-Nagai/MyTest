using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class U_WindowScale : MonoBehaviour {
	
	public RectTransform Window;
	//	readonly Vector2 minimumAnchor = new Vector2(0,225);
	//	readonly Vector2 minimumDelta = new Vector2(-160,-750);
	readonly float ChangeValue = 50;
	
	
	public void Set(WindowTransform t){
		Window.position = t.worldPosition;
		Window.localPosition = t.localPosition;
		Window.anchoredPosition = t.anchor;
		Window.sizeDelta = t.delta;
	}
	
	public void Reduce(){
		if(Screen.orientation == ScreenOrientation.Portrait){
			changePortrait(-ChangeValue);
		}else{
			changeLandscape(-ChangeValue);
		}
	}
	
	public void Magnify(){
		if(Screen.orientation == ScreenOrientation.Portrait){
			changePortrait(ChangeValue);
		}else{
			changeLandscape(ChangeValue);
		}
	}
	
	void changePortrait(float val){
		Window.transform.localPosition += new Vector3(0f,-val/2f,0f);
		Window.anchoredPosition += new Vector2(0,-val/2f);
		Window.sizeDelta += new Vector2(val/2f,val*2f);
	}
	
	void changeLandscape(float val){
		Window.transform.localPosition += new Vector3(0f,-val/4f,0f);
		Window.anchoredPosition += new Vector2(0,-val/4f);
		Window.sizeDelta += new Vector2(val*4,val);
	}
	
	
	
	public WindowTransform getWindow(){
		WindowTransform copy = new WindowTransform();
		copy.worldPosition = Window.position;
		copy.localPosition = Window.localPosition;
		copy.anchor = Window.anchoredPosition;
		copy.delta = Window.sizeDelta;
		return copy;
	}
	
	
	public class WindowTransform{
		public Vector3 worldPosition;
		public Vector3 localPosition;
		public Vector2 anchor;
		public Vector2 delta;
	}
	
}
