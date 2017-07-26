using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class U_UISprite : Image {
	public U_UIAtlas atlas;

	public string spriteName{
		get{return this.sprite.name;}
		set{
			if(atlas != null){
				var sp = atlas.GetSprite(value);
				this.sprite = sp;
			}else{
				Debug.LogError("Sprite Atlas is Null");
			}
		}
	}
}
