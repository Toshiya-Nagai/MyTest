using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class CPUController : Controller<CpuModel> {
	PuzzleModel puzzle{get{return actor.entity.puzzle;}}
	public CPUActor actor;
	public CpuState CurrentState;
	public bool IsEnable;

	System.Func<IEnumerator>[] action;
	
	public override void Set (CpuModel data){
		base.Set (data);
		action = new System.Func<IEnumerator>[]{Wait,Raise,Chain,Order,HeightRegulate};
		CurrentState = CpuState.Judge;
		StartCoroutine(loop());
	}
	
	IEnumerator loop(){
		while(true){
			if(IsEnable){
				if(CurrentState == CpuState.Judge){
					CurrentState = NextState();
				}
//				Debug.Log(CurrentState);
				yield return StartCoroutine(action[(int)CurrentState]());
//				CurrentState = NextState();
			}else{
				yield return null;
			}
		}
	}
	
	#region CPU Action
	public IEnumerator Wait(){
		yield return new WaitForSeconds(Data.waitTime);
	}
	
	public IEnumerator Raise(){
		actor.timerController.SpeedUp();
		yield return new WaitForSeconds(Data.raiseTime);
		actor.timerController.SpeedNormal();
		CurrentState = CpuState.Judge;
	}
	
	public IEnumerator Chain(){
		//judge
		bool isChain = false;
		for(int i = puzzle.map.Count-1;i >= puzzle.addPieceMap.Count;i--){
			if(!PuzzleUtil.isOutRight(puzzle,i) && puzzle.map[i].type != puzzle.map[i+(int)Dir.Right].type){
				isChain = false;
				if(puzzle.ChangeSideChain(ref isChain,i,Dir.Right)){
					change(i,Dir.Right);
					yield return new WaitForSeconds(Data.changeInterval);
				}
			}
			if(!PuzzleUtil.isOutLeft(puzzle,i) && puzzle.map[i].type != puzzle.map[i+(int)Dir.Left].type){
				isChain = false;
				if(puzzle.ChangeSideChain(ref isChain,i,Dir.Left)){
					change(i,Dir.Left);
					yield return new WaitForSeconds(Data.changeInterval);
				}
			}
		}
		CurrentState = CpuState.Order;
	}

	public IEnumerator Order(){
		for(int i = puzzle.addPieceMap.Count;i < puzzle.map.Count;i += 2){
			if(puzzle.map[i].ChangeEnableState()){
				change(i,Dir.Left);
				yield return new WaitForSeconds(Data.orderInterval);
			}
		}
		CurrentState = CpuState.Judge;
	}
	
	public IEnumerator HeightRegulate(){
		for(int i = (puzzle.map.Count-1)-puzzle.width;i >= puzzle.addPieceMap.Count;i--){
			if(puzzle.map[i].ChangeEnableState()){
				if(!PuzzleUtil.isOutLeft(puzzle,i)){
					if(regulateEnable(i,Dir.Left)){
						change(i,Dir.Left);
						yield return new WaitForSeconds(Data.regulateInterval);
					}
				}
				if(!PuzzleUtil.isOutRight(puzzle,i)){
					if(regulateEnable(i,Dir.Right)){
						change (i,Dir.Right);
						yield return new WaitForSeconds(Data.regulateInterval);
					}
				}
			}
		}
		CurrentState = CpuState.Judge;
	}

	#endregion

	void change(int index,Dir dir){
//		Debug.Log("index : " + index.ToString());
		SendPieceData send = new SendPieceData();
		send.current = actor.pieceCreator.pieces[index].GetComponent<PieceController>();
		send.change = actor.pieceCreator.pieces[index+(int)dir].GetComponent<PieceController>();
		actor.changeController.Change(send);
		actor.changeController.ChangeEnd();
	}
	
	bool regulateEnable(int index,Dir dir){
		return puzzle.map[index+(int)dir+(int)Dir.Down].isTypeNone() &&
			puzzle.map[index+(int)dir].isTypeNone() && 
				puzzle.map[index+(int)dir+(int)Dir.Up].isTypeNone();
	}
	
	public CpuState NextState(){
		//		return CpuState.Raise;
		//		return CpuState.Chain;
		//		return CpuState.Order;
		//		return CpuState.Regulate;
		
		int maxHeight = puzzle.GetMaxHeight();
		if(maxHeight < 6){return CpuState.Raise;}
		int minHeight = puzzle.GetMinHeight();
		if(maxHeight - minHeight > 3){return CpuState.Regulate;}
		return CpuState.Chain;
	}

	public void StopCpu(){
		IsEnable = false;
		StopCoroutine(loop());
	}

	void OnApplicationQuit(){
		StopCoroutine(loop());
	}
}

[System.Serializable]
public class CpuModel{
	public int id;
	public int level;
	public float raiseTime;
	public float waitTime;
	public float regulateInterval;
	public float changeInterval;
	public float orderInterval;
	public CpuModel(){
		raiseTime = 1.0f;
		waitTime = 2.0f;
		regulateInterval = 0.05f;
		changeInterval = 0.5f;
		orderInterval = 0.05f;
	}
}

public enum CpuState{
	Wait,
	Raise,
	Chain,
	Order,
	Regulate,
	Judge,
	Max,
}
