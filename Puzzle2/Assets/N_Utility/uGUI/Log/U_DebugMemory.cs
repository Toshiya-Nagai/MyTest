using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using U_DebugUtil;


public class U_DebugMemory :IDebug{
	
	private U_DebugMenuContent content;
	private List<DebugCommand> _commandList;
	public U_DebugMemory(){
		content = new U_DebugMenuContent();
		_commandList = new List<DebugCommand>();
		content.SetCommand(_commandList);
	}
	
	public string ButtonText{
		get{return "memory";}
	}
	
	public string DebugLog(){
		return content.LogText;
	}
	
	public List<DebugCommand> CommandList{
		get{return _commandList;}
		set{}
	}
}

//debug default menu
public class U_DebugMenuContent : BaseDebug{
	public const int    SIZE_KB = 1024;
	public const int    SIZE_MB = 1024 * 1024;
	public string WorkSpace{get;set;}
	public override string LogText {
		get {return getBaseLog() + "\n" + InfoText;}
	}
	
	public U_DebugMenuContent(){
		WorkSpace = "";
		
	}
	
	public string getBaseLog(){
		string text = "";
		text = "Used Memory : " + (System.GC.GetTotalMemory(false) / SIZE_MB).ToString() + " / " + SystemInfo.systemMemorySize.ToString() + " MB";
		text += "\n Used Program Heap : " + (Profiler.usedHeapSize/SIZE_MB).ToString() + " / " + SystemInfo.systemMemorySize.ToString() + " MB";
		text += "\n" + "Mono Heap : " + (Profiler.GetMonoUsedSize()/SIZE_MB).ToString() + " / " + (Profiler.GetMonoHeapSize()/SIZE_MB).ToString() + " MB";
		text += "\n Total Alloc Memory : " + ((float)Profiler.GetTotalAllocatedMemory() / SIZE_MB).ToString("0.00") + " MB";
		text += "\n System Memory : " + SystemInfo.systemMemorySize.ToString() + " MB \n";
		text += "\n Frame Count : " + Time.deltaTime;
		text += "\n TimeScale : " + Time.timeScale;
		return text;
	}
	
	void gcCollect(){
		System.GC.Collect();
		addLog("GC Collect Success");
	}
	
	void resourceUnLoad(){
		Resources.UnloadUnusedAssets();
		addLog("Resource Unload Success");
	}
	
	void cachingClean(){
		Caching.CleanCache();
		addLog("Caching Clean Success");
	}

	void timeScaleChange(string timeScaleStr){
		float timeScale = 0.0f;
		bool isCast = float.TryParse(timeScaleStr,out timeScale);
		Time.timeScale = (isCast)?timeScale:Time.timeScale;
		if(isCast)
			addLog ("Success");
		else
			addLog ("Failed Parse Float : " + timeScaleStr);
	}
	
	private void addLog(string anAddText){
		InfoText = anAddText;
	}
	
	public void SetCommand(List<DebugCommand> commandList){
		commandList.Add(new DebugCommand("GC Collect",gcCollect));
		commandList.Add(new DebugCommand("Res UnLoad",resourceUnLoad));
		commandList.Add(new DebugCommand("Caching Clean",cachingClean));
		commandList.Add(new InputCommand("TimeScale Change","Input TimeScale float value",timeScaleChange));
	}
}