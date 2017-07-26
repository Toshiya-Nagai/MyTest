using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class CharaInfo : Controller<Chara> {
	public Text nameText;
	public Text commentText;
	public Text skillText;
	public Text skillComment;
	public Image CharaImage;

	public override void Set (Chara data){
		base.Set (data);
	}

	//send message call
	public void Set(CharaInfo info){
		if(this.Data != info.Data){
			this.Data = info.Data;
			Observe();
			Debug.Log("Data Update");
		}else{
			Debug.Log("Equal Data");
		}
	}

	public override void Observe (){
		nameText.text = Data.info.name;
		commentText.text = Data.info.comment;
		skillText.text = Data.skill.name;
		skillComment.text = Data.skill.comment;
	}

	public int GetCharaId(){
		return (Data != null)?Data.info.id:-1;
	}
}
