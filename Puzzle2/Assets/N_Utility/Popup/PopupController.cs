using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PopupController : MonoBehaviour {
	public Text Title;
	public Text Message;

	public GameObject LeftButton;
	public Text LeftLabel;

	public GameObject RightButton;
	public Text RightLabel;

	protected PopupDataModel model;

	public void Awake(){
		this.transform.localPosition = new Vector3(0.0f,0.0f,0.0f);
	}

	void Start(){
		Title.text = model.title;
		Message.text = model.message != null ? model.message:string.Empty;

		LeftLabel.text = model.leftLabel;
		RightLabel.text = model.rightLabel;

		if(model.leftLabel == null || model.leftLabel == ""){
			LeftButton.SetActive(false);
			RightButton.transform.localPosition = new Vector3(0,RightButton.transform.localPosition.y,RightButton.transform.localPosition.z);
		}
	}


	//NG
	public virtual void OnLeftButton(){
		if(!LeftButton.activeInHierarchy){
			return;
		}
		if(model.leftAction != null){
			model.leftAction();
		}
	}

	//OK
	public virtual void OnRightButton(){
		if(model.rightAction != null){
			model.rightAction();
		}
	}

	public static PopupController CreateResources(string anResourceName,PopupDataModel anModel){
		GameObject popup = Instantiate(Resources.Load(anResourceName) as GameObject) as GameObject;
		PopupController controller = popup.GetComponent<PopupController>();
		controller.model = anModel;
		return controller;
	}

	public static TController CreateResources<TController>(string anResourceName,PopupDataModel anModel) where TController : PopupController{
		GameObject popup = Instantiate(Resources.Load(anResourceName) as GameObject) as GameObject;
		TController controller = popup.GetComponent<TController>();
		controller.model = anModel;
		return controller;
	}

}

