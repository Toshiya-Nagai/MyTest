using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using N_DebugUtil;


public class N_DebugMemory :IDebug{

	private DebugMenuContent content;
	private List<DebugCommand> _commandList;
	public N_DebugMemory(){
		content = new DebugMenuContent();
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
public class DebugMenuContent : BaseDebug{
	public const int    SIZE_KB = 1024;
	public const int    SIZE_MB = 1024 * 1024;
	public string WorkSpace{get;set;}
	public override string LogText {
		get {return getBaseLog() + "\n" + InfoText;}
	}
	
	public DebugMenuContent(){
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
		return text;
	}
	
	public void GCCollect(){
		System.GC.Collect();
		addLog("GC Collect Success");
	}
	
	public void ResourceUnLoad(){
		Resources.UnloadUnusedAssets();
		addLog("Resource Unload Success");
	}
	
	public void CachingClean(){
		Caching.CleanCache();
		addLog("Caching Clean Success");
	}
	
	private void addLog(string anAddText){
		InfoText = anAddText;
	}

	public void SetCommand(List<DebugCommand> commandList){
		commandList.Add(new DebugCommand("GC Collect",GCCollect));
		commandList.Add(new DebugCommand("Res UnLoad",ResourceUnLoad));
		commandList.Add(new DebugCommand("Caching Clean",CachingClean));
	}
}