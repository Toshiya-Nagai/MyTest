using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace U_DebugUtil{
	public class DebugCommand{
		public string CommandName;
		public UnityAction Command;
		public DebugCommand(string anCommandName,UnityAction anCommand){
			this.CommandName = anCommandName;
			this.Command = anCommand;
		}
	}

	public class InputCommand : DebugCommand{
		UnityAction<string> inputCommand;
		public string inputStr{private get;set;}
		public string DefaultFieldStr;
		public InputCommand(string anCommandName,UnityAction<string> anCommand) : this(anCommandName,"Enter text...",anCommand){}
		public InputCommand(string anCommandName,string defaultFieldStr,UnityAction<string> anCommand) : base(anCommandName,()=>{
			anCommand(inputStr);
		}){
			this.DefaultFieldStr = defaultFieldStr;
		}
	}

	public interface IDebug{
		string ButtonText{get;}
		string DebugLog();
//		string DebugLog{get;set;}
		List<DebugCommand> CommandList{get;set;}
	}


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
}

