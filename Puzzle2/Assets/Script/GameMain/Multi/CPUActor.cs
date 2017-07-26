using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CPUActor : PuzzleActor {
	public CPUController cpuController;
	public CpuModel cpuModel;

	public override IEnumerator Setup (){
		yield return StartCoroutine (base.Setup());
		yield return StartCoroutine(cpuLevelLoad());
		cpuModel = getCpuLevel(CacheManager.cpuDM);
		cpuController.Set(cpuModel);
		cpuController.IsEnable = false;
	}

	public override void Ready (){
		base.Ready ();
		cpuController.IsEnable = true;
	}

	public override void Showdown (){
		base.Showdown ();
		cpuController.StopCpu();
	}

	IEnumerator cpuLevelLoad(){
		bool isLoad = true;
		SqliteDatabase.instance.ExecuteOnState(SqliteDatabase.State.BootReady,()=>{
			isLoad = false;
		});
		while(isLoad)yield return null;
	}

	CpuModel getCpuLevel(List<CpuModel> levels){
		int level = SaveOption.Load().CpuLevel;
		CpuModel m = levels.Find(cpu=>cpu.level == level);
		if(m == null){m = new CpuModel();}
		return m;
	}
}
