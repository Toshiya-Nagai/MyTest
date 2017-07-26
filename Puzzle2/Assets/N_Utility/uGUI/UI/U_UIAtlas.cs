using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class U_UIAtlas : MonoBehaviour {
	public Texture Texture;
	public List<Sprite> Sprites;
	public Sprite GetSprite(string spriteName){
		var sprite = Sprites.Find(sp=>sp.name == spriteName);
		return sprite;
	}
}
