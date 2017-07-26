using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;


namespace N_DebugUtil{
	public interface IDebug{
		string ButtonText{get;}
		string DebugLog();
		List<DebugCommand> CommandList{get;set;}
	}
	
	public class DebugCommand{
		public string CommandName;
		public UnityAction Command;
		public DebugCommand(string anCommandName,UnityAction anCommand){
			this.CommandName = anCommandName;
			this.Command = anCommand;
		}
	}

	#region gui controller
	/// <summary>
	/// GUI box.
	/// </summary>
	public class GUIBox : GUIModel{
		public GUIBox(){
		}
		public GUIBox(Rect anArea,string anText) : base(anArea,anText){
		}
		public override void Arrangement () {
			GUI.Box(this.Area,this.Text);
		}
	}

	/// <summary>
	/// GUI label.
	/// </summary>
	public class GUILabel : GUIModel{
		public GUILabel(Rect anArea,string anText) : base(anArea,anText){
			InitGUIStyle();
#if UNITY_EDITOR
			this.Style.fontSize = 10;
#elif UNITY_ANDROID
			Style.fontSize = 20;
#else
			this.Style.fontSize = 10;
#endif
		}
		public override void Arrangement () {
			GUI.Label(this.Area,this.Text,Style);
		}
	}

	/// <summary>
	/// GUI data log label.
	/// </summary>
	public class GUIDataLogLabel : GUIModel{
		private BaseDebug _logData;
		public BaseDebug LogData{
			set{this._logData = value;}
		}
		public GUIDataLogLabel(Rect anArea,BaseDebug anDataModel) : base(anArea,anDataModel.LogText){
			this.LogData = anDataModel;
		}
		public GUIDataLogLabel(BaseDebug anDataModel){	
			this.LogData = anDataModel;
			this.Text = anDataModel.LogText;
		}
		public override void Arrangement () {
			GUI.Label(this.Area,this._logData.LogText,Style);
		}
	}

	/// <summary>
	/// GUI button.
	/// </summary>
	public class GUIButton : GUIModel{
		public UnityAction ButtonEvent;
		public GUIButton(Rect anArea,string anText) : base(anArea,anText){
			#if UNITY_EDITOR
			Rect rect = new Rect(Area.x,Area.y,Area.width/2,Area.height/2);
			this.Area = rect;
			#endif
		}
		public GUIButton(UnityAction anButtonEvent){
			#if UNITY_EDITOR
			Rect rect = new Rect(Area.x,Area.y,Area.width/2,Area.height/2);
			this.Area = rect;
			#endif

			ButtonEvent = (anButtonEvent != null)?anButtonEvent : null;
		}

		protected bool arrangement(GUIButton anButtonModel){
			if(GUI.Button(anButtonModel.Area,anButtonModel.Text)){
				return true;
			}
			return false;
		}
		public override void Arrangement () {
			if(this.arrangement(this)){
				if(ButtonEvent != null){
					ButtonEvent();
				}
			}
		}
	}

	public class DebugMenuButton : GUIButton{
		public IDebug DebugData{get;private set;}
		public DebugMenuButton(Rect anArea,string anText,IDebug anDebugData) : base(anArea,anText){
			this.DebugData = anDebugData;
		}
		public override void Arrangement () {
			if(this.arrangement(this)){
				N_DebugMenu.CloseMenu();
				N_Debug.CreateDebugMenu(this.DebugData);
			}
		}
	}

	/// <summary>
	/// GUI model.
	/// </summary>
	public class GUIModel : GUIBaseModel{
		public override Rect Area {protected get;set;}
		public override string Text {protected get;set;}

		private GUIStyle _style;
		public override GUIStyle Style {protected get;set;}

		public void SetNormalStyleState(GUIStyleState anSetState){
			this.Style.normal = anSetState;
		}
		
		public GUIModel(){
			Text = "empty button";
			InitGUIStyle();
		}
		public GUIModel(Rect anArea,string anText){
			this.Area = anArea;
			this.Text = anText;
			InitGUIStyle();
		}
		public void InitGUIStyle(){
			this.Style = new GUIStyle();
#if UNITY_ANDROID && !UNITY_EDITOR
			this.Style.fontSize = 20;
#else
			this.Style.fontSize = 10;
#endif
			GUIStyleState styleState = new GUIStyleState();
			styleState.textColor = Color.white;
			SetNormalStyleState(styleState);
		}
		public virtual void Arrangement(){
		}

	}

	/// <summary>
	/// GUI base model.
	/// not instance class
	/// </summary>
	abstract public class GUIBaseModel{
		abstract public Rect Area{protected get;set;}
		abstract public string Text{protected get;set;}
		abstract public GUIStyle Style{protected get;set;}
	}
	#endregion

	#region data controller
	/// <summary>
	/// debug data class.
	/// </summary>
	public class BaseDebug{
		private string _logText;
		virtual public string LogText{
			get{
				if(DebugModel != null){
					return DebugModel.DebugLog() +"\n"+ InfoText;
				}else{
					return "DataModel Empty";
				}
			}
		}
		public string InfoText;

		private IDebug _debugModel;
		public IDebug DebugModel{
			set{_debugModel = value;}
			get{return _debugModel;}
		}
		
		public BaseDebug(){
			DebugModel = null;
		}
		public BaseDebug(IDebug anDebugModel){
			DebugModel = anDebugModel;
		}
		public List<DebugCommand> CommandList{
			get{return DebugModel.CommandList;}
			set{DebugModel.CommandList = value;}
		}
	}
	#endregion

	#region gui data reader
	/// <summary>
	/// Debug text reader.
	/// gui placement read text
	/// </summary>
	public class DebugTextReader{
		private string path;
		public string Path{
			get{return path;}
			private set{path = value;}
		}
		
		private TextAsset _guiPlacementText;
		public TextAsset GuiPlacementText{
			get{return _guiPlacementText;}
			private set{_guiPlacementText = value;}
		}
		
		
		public DebugTextReader(string anPath){
			Path = anPath;
		}
		public DebugTextReader(TextAsset anTextAsset){
			this.GuiPlacementText = anTextAsset;
		}
		//not using 
		/*
		public List<GUIModel> GetTextFileSerialize(List<GUIModel> anModelSetList){
			using (StreamReader sr = new StreamReader(Path, Encoding.GetEncoding("utf-8"))) {
				string line = "";
				int index = 0;
				while ((line = sr.ReadLine()) != null) {
					if(isCommentLine(line) == false){
	//					Debug.Log(line);
						if(index < anModelSetList.Count){
							anModelSetList[index] = getOneSerialize(line,anModelSetList[index]);
						}
						index++;
					}
				}
			}
			return anModelSetList;
		}
		*/
		
		public List<GUIModel> GetTextAssetSerialize(List<GUIModel> anModelSetList){
			string[] splitStr = GuiPlacementText.text.Split('\n');
			int index = 0;
			for(int i = 0;i < splitStr.Length;i++){
	//			Debug.Log(splitStr[i]);
				if(isCommentLine(splitStr[i]) == false){
					if(index < anModelSetList.Count){
						anModelSetList[index] = getOneSerialize(splitStr[i],anModelSetList[index]);
						index++;
					}
				}
			}
			return anModelSetList;
		}
		
		private bool isCommentLine(string anLineStr){
			if(anLineStr.Contains("///") == false && anLineStr != "\n" && anLineStr != ""){
				return false;
			}
			return true;
		}
		
		private GUIModel getOneSerialize(string anLineStr,GUIModel anSetModel){
			string[] splitData = anLineStr.Split(',');
			int startX = getStringToParam(splitData[(int)LineSplitData.StartX]);
			int startY = getStringToParam(splitData[(int)LineSplitData.StartY]);
			int width = getStringToParam(splitData[(int)LineSplitData.Width]);
			int height = getStringToParam(splitData[(int)LineSplitData.Height]);
			anSetModel.Area = new Rect(startX,startY,width,height);
			if(splitData[(int)LineSplitData.HeaderText] != null){
				anSetModel.Text = splitData[(int)LineSplitData.HeaderText].Replace("\"","");
			}
			return anSetModel;
		}
		
		private int getStringToParam(string anParamStr){ 
			if(anParamStr.Contains("[screenW/2]")){
				anParamStr = anParamStr.Replace("[screenW/2]","");
				int param = (anParamStr != "")?System.Convert.ToInt32(anParamStr):0;
				return (Screen.width/2) + param;
			}
			
			if(anParamStr.Contains("screenW")){
				anParamStr = anParamStr.Replace("screenW","");
				int param = System.Convert.ToInt32(anParamStr);
				return param + Screen.width;
			}
			if(anParamStr.Contains("[screenH/2]")){
				anParamStr = anParamStr.Replace("[screenH/2]","");
				int param = (anParamStr != "")?System.Convert.ToInt32(anParamStr):0;
				return (Screen.height/2) + param;
			}
			if(anParamStr.Contains("screenH")){
				anParamStr = anParamStr.Replace("screenH","");
				int param = System.Convert.ToInt32(anParamStr);
				return param + Screen.height;
			}
			return System.Convert.ToInt32(anParamStr);
		}
		
		public enum LineSplitData : int{
			StartX,
			StartY,
			Width,
			Height,
			HeaderText,
		}
	}
	#endregion
}