using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UpdateNotification : MonoBehaviour {
	public List<GameObject> viewObjects;

	public void update(){
		viewObjects.ForEach (x=>{
			x.notNull(xx=>{
				var view = xx.GetInterfaceInGameObject<IView>();
				view.notNull(v=>{v.UpdateUI();});
			});
		});
	}
}
