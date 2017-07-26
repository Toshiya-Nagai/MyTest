using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class RaycastEnable : MonoBehaviour,ICanvasRaycastFilter {
	public bool isRaycast;
	public bool IsRaycastLocationValid (Vector2 sp, Camera eventCamera){
		return isRaycast;
	}
}
