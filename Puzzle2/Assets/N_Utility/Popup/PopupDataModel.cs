using UnityEngine;
using UnityEngine.Events;
using System.Collections;


public class PopupDataModel{
	public string title;
	public string message;
	public string leftLabel;
	public string rightLabel;
	public UnityAction leftAction = null;
	public UnityAction rightAction = null;
	public int id;
	
	public static PopupDataModel Full(string anTitle,string anMessage,string anLeftLabel,UnityAction anLeftAction,string anRightLabel,UnityAction anRightAction){
		PopupDataModel model = new PopupDataModel();
		model.title  = anTitle;
		model.message = anMessage;
		model.leftLabel = anLeftLabel;
		model.leftAction = anLeftAction;
		model.rightLabel = anRightLabel;
		model.rightAction = anRightAction;
		return model;
	}
	
	public static PopupDataModel CancelOk(string anTitle,string anMessage,UnityAction anOkAction,UnityAction anCancelAction = null){
		return Full(anTitle,anMessage,"Cancel",anCancelAction,"OK",anOkAction);
	}
	
	public static PopupDataModel NoYes(string anTitle,string anMessage,UnityAction anYesAction,UnityAction anNoAction = null){
		return Full(anTitle,anMessage,"No",anNoAction,"Yes",anYesAction);
	}
	
	public static PopupDataModel One(string anTitle,string anMessage,string anButtonLabel,UnityAction anButtonAction){
		return Full(anTitle,anMessage,null,null,anButtonLabel,anButtonAction);
	}
	
	public static PopupDataModel Ok(string anTitle,string anMessage){
		return Full(anTitle,anMessage,null,null,"OK",null);
	}
	public static PopupDataModel Ok(string anTitle,string anMessage,UnityAction anOkAction){
		return Full(anTitle,anMessage,null,null,"OK",anOkAction);
	}

	public static PopupDataModel Retry(string anTitle,string anMessage,UnityAction anRetryAction,UnityAction anCancelAction){
		return Full(anTitle,anMessage,"Cancel",anCancelAction,"Retry",anRetryAction);
	}
	public static PopupDataModel Maintenance(string message){
		return PopupDataModel.Ok("Maintenance",message,null);
	}
}


public interface IPopupModel{
	string title{get;set;}
	string message{get;set;}
	string leftLabel{get;set;}
	string rightLabel{get;set;}
	UnityAction leftAction{get;set;}
	UnityAction rightAction{get;set;}
}