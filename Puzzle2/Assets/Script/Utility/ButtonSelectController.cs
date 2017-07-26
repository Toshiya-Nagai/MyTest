using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class ButtonSelectController : MonoBehaviour {
	public Button SelectButton;
	public List<Button> Buttons;

	void Start(){
		foreach(var buf in Buttons){
			deselect(buf);
		}
		if(Buttons[0] != null){
			Select(Buttons[0].gameObject);
		}
	}


	public void Select (GameObject target){
		var button = target.GetComponent<Button>();
		if(button == null){return;}
		if(button != SelectButton){
			if(SelectButton != null)
				deselect(SelectButton);
			select(button);
		}
	}

	void select(Button select){
		SelectButton = select;
		SelectButton.image.color = Color.white;
		SelectButton.enabled = false;
	}

	void deselect(Button deselect){
		deselect.image.color = Color.gray;
		deselect.enabled = true;
	}
}
