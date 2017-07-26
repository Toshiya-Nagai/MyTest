using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.Events;

public class SelectEvent : MonoBehaviour,IPointerClickHandler {
	public CharaInfo sendTarget;
	public CharaInfo sendData;
	public CharaSelector selector;
	public void OnPointerClick (PointerEventData eventData){
		sendTarget.Set(sendData);
		selector.Select(sendData.GetCharaId());
	}
}
