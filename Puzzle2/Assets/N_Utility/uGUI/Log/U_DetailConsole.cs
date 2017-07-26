using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using U_DebugUtil;

using UniLinq;

public class U_DetailConsole : IDebug {
	string log;
	public string LogText{
		set{log = value;}
		get{return log;}
	}

	public U_DetailConsole(){
		commandList = new List<DebugCommand>();
		commandList.Add(new DebugCommand("Clear All",()=>{
			log = "";
		}));
		commandList.Add (new DebugCommand("Clear Log",()=>{
			var split = log.Split(new string[]{"//"},System.StringSplitOptions.RemoveEmptyEntries)
				.Where (x=>x.IndexOf("<color=white>") <= 0 && x.Length > 10).Select(x=>"//"+x+"//").Aggregate((now,next)=>now+next);
			log = split;
		}));
		commandList.Add (new DebugCommand("Clear Warning",()=>{
			var split = log.Split(new string[]{"//"},System.StringSplitOptions.RemoveEmptyEntries)
				.Where (x=>x.IndexOf("<color=yellow>") <= 0 && x.Length > 10).Select(x=>"//"+x+"//").Aggregate((now,next)=>now+next);
			log = split;
		}));
		commandList.Add (new DebugCommand("Clear Exception",()=>{
			var split = log.Split(new string[]{"//"},System.StringSplitOptions.RemoveEmptyEntries)
				.Where (x=>x.IndexOf("<color=red>") <= 0 && x.Length > 10).Select(x=>"//"+x+"//").Aggregate((now,next)=>now+next);
			log = split;
		}));
		Application.logMessageReceived += HandleLog;
	}

	void HandleLog (string condition, string stackTrace, LogType type){
		string color = (type == LogType.Log)?"<color=white>":(type == LogType.Warning)?"<color=yellow>":"<color=red>";
		log += "// \n" + color + "condition : " + condition + "\n stack : " + stackTrace + "\n type : " + type.ToString() + "</color> \n // ";
		if(log.Length > 15000){
			log = "";
		}
	}


	public string DebugLog (){
		return log;
	}
	public string ButtonText {
		get {
			return "Detail Console";
		}
	}
	private List<DebugCommand> commandList;
	public List<DebugCommand> CommandList {
		get {
			return commandList;
		}
		set {
			throw new System.NotImplementedException();
		}
	}

}
