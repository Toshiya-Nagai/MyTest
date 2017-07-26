using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UniLinq;

public class PuzzleModel{
	public readonly int width;
	public readonly int height;
	public int PieceTypeMax;
	public readonly int initPieceCount;
	
	public int comboCount;
	
	public List<PieceModel> map;
	public List<PieceModel> addPieceMap{get{return map.GetRange(0,width);}}
	public List<PieceModel> limitMap{get{return map.GetRange(map.Count-width,width);}}
	
	public PuzzleEvent puzzleEvent;
	
	public PuzzleModel(){
		width = 6;
		height = 14;
		initPieceCount = width*4;
		puzzleEvent = new PuzzleEvent();
	}
	public void Init(){
		map = new List<PieceModel>();
		for(int i = 0;i < width*height;i++){
			PieceModel m = new PieceModel();
			m.type = (i < initPieceCount)?UnityEngine.Random.Range((int)PieceType.Red,PieceTypeMax+1):0;
			m.state = (i < initPieceCount)?PieceState.Stand:PieceState.None;
			m.comboState = ComboState.None;
			#region debug
//			m.type = UnityEngine.Random.Range((int)PieceType.Red,(int)PieceType.Magenta);
//			m.state = PieceState.Stand;
//			m.type = (int)PieceType.Hind;
			#endregion
			m.isHideArea = (i < width)?true:false;
			m.index = i;
			map.Add(m);
		}
		initPiece(this);
	}
	
	void initPiece(PuzzleModel puzzle){
		for(int i = puzzle.addPieceMap.Count;i < puzzle.width*puzzle.height;i++){
			if(puzzle.map[i].type != (int)PieceType.None){
				setQuarrelType(puzzle,i,PieceTypeMax+1);
			}
		}
	}
	
	void setQuarrelType(PuzzleModel puzzle,int index,int PieceTypeMax){
		bool success = true;
		while(success){
			success = false;
			puzzle.map[index].type = UnityEngine.Random.Range((int)PieceType.Red,PieceTypeMax);
			if(getEqualDir(puzzle,index,1,Dir.Right) && getEqualDir(puzzle,index,2,Dir.Right)){
				success = true;
			}
			if(success == false && (getEqualDir(puzzle,index,1,Dir.Down) && getEqualDir(puzzle,index,2,Dir.Down))){
				success = true;
			}
		}
	}
	
	
	bool getEqualDir(PuzzleModel puzzle,int index,int range,Dir dir){
		if(dir == Dir.Right || dir == Dir.Left){
			if(PuzzleUtil.isEqualHeight(puzzle,index,index+((int)dir*range))){
				return puzzle.map[index].type == puzzle.map[index+((int)dir*range)].type;
			}
		}else{
			if(!PuzzleUtil.isOutDir(puzzle,index,dir)){
				return puzzle.map[index].type == puzzle.map[index+((int)dir*range)].type;
			}
		}
		return false;
	}
	
	//check piece max upper
	public bool isLimitPieces(){
		return limitMap.Any(x=>x.type != (int)PieceType.None);
	}
	
	public void PieceUp(){
		for(int i = width*height-1;i >= 0;i--){
			if(i-width >= 0){
				map[i].type = map[i-width].type;
				map[i].state = map[i-width].state;
			}
		}
		foreach(var buf in addPieceMap){
			buf.type = UnityEngine.Random.Range((int)PieceType.Red,PieceTypeMax+1);
		}
	}
	
	//this function call Update()
	public void Gravity(){
		int fallCount = 0;
		for(int i = addPieceMap.Count;i < map.Count;i++){
			//fall update
			if(map[i].state == PieceState.Fall){
				map[i].comboState = ComboState.Fall;		//combo check
				map[i].gravityY += 16;		//4 frame fall speed
				if(map[i].gravityY >= 64){
					map[i].gravityY = 0;
					map[i+(int)Dir.Down].type = map[i].type;
					map[i+(int)Dir.Down].state = PieceState.Stand;
					map[i+(int)Dir.Down].comboState = ComboState.Fall;
					map[i].SetNone();
				}else{
					fallCount++;
				}
			}
			//combo
			else if (map[i].state == PieceState.Stand){
				map[i].comboState = ComboState.None;
			}
		}
		//fall check
		for(int i = addPieceMap.Count;i < map.Count;i++){
			if(map[i].type == (int)PieceType.None){
				for(int height = i+width;height < map.Count && map[height].type != (int)PieceType.None;height += (int)Dir.Up){
					if(map[height].state == PieceState.Stand){
						map[height].state = PieceState.Fall;
						fallCount++;
					}else{break;}
				}
			}
		}
		//fall is time stop
		puzzleEvent.FallEvent(fallCount);
	}
	
	public void Judge(){
		for(int i = addPieceMap.Count;i < map.Count;i++){
			bool isCombo = false;
			int chainCount = 0;
			int resultChainCount = 0;
			if(!map[i].isChain && map[i].isDeletePiece() && map[i].state == PieceState.Stand){
				chain(this,ref chainCount,ref isCombo,i,Dir.Up,map[i].type);
				resultChainCount += (chainCount >= 3)?chainCount:0;
				chainCount = 0;
				chain(this,ref chainCount,ref isCombo,i,Dir.Down,map[i].type);
				resultChainCount += (chainCount >= 3)?chainCount:0;
				chainCount = 0;
				chain(this,ref chainCount,ref isCombo,i,Dir.Right,map[i].type);
				resultChainCount += (chainCount >= 3)?chainCount:0;
				chainCount = 0;
				chain(this,ref chainCount,ref isCombo,i,Dir.Left,map[i].type);
				resultChainCount += (chainCount >= 3)?chainCount:0;
			}
			if(isCombo){comboCount++;}
			if(resultChainCount >=3 && puzzleEvent.ChainEvent != null){
				puzzleEvent.ChainEvent(comboCount,resultChainCount);
				Debug.Log("comboCount : " + comboCount + "   chainCount : " + resultChainCount);
			}
		}
	}
	
//	void chain(PuzzleModel puzzle,ref int chainCount,ref bool isCombo,int currentIndex,Dir dir,int chainType){
//		chainCount++;
//		//default dir chain
//		if(PuzzleUtil.checkChain(puzzle,chainType,currentIndex,dir)){
//			chain(puzzle,ref chainCount,ref isCombo,currentIndex+(int)dir,dir,chainType);
//		}
//		if(chainCount >= 3){
//			puzzle.map[currentIndex].isChain = true;
//			puzzle.map[currentIndex].state = PieceState.Chain;
//			//combo
//			if(puzzle.map[currentIndex].comboState == ComboState.Fall){
//				isCombo = true;
//				puzzle.map[currentIndex].comboState = ComboState.None;
//			}
//			#region bug fix log
//			chainDepth(puzzle,ref chainCount,currentIndex,dir,chainType);
//			setHindToChain(puzzle,currentIndex);
//			#endregion
//		}
//	}

	void chain(PuzzleModel puzzle,ref int chainCount,ref bool isCombo,int currentIndex,Dir dir,int chainType){
		chainCount++;
		//default dir chain
		if(PuzzleUtil.checkChain(puzzle,chainType,currentIndex,dir)){
			chain(puzzle,ref chainCount,ref isCombo,currentIndex+(int)dir,dir,chainType);
		}
		if(chainCount >= 3){
			//combo
			if(puzzle.map[currentIndex].comboState == ComboState.Fall){
				isCombo = true;
				puzzle.map[currentIndex].comboState = ComboState.None;
			}
			#region bug fix log
			chainDepth(puzzle,ref chainCount,currentIndex,dir,chainType);
			setHindToChain(puzzle,currentIndex);
			#endregion
			puzzle.map[currentIndex].isChain = true;
			puzzle.map[currentIndex].state = PieceState.Chain;
		}
	}
	
	void chainDepth(PuzzleModel puzzle,ref int chainCount,int currentIndex,Dir dir,int chainType){
		#region bug fix
		//FIXME
		//bug is judge piece L type
		if(dir == Dir.Up || dir == Dir.Down){
			int depthChainCount = 0;
			chainAction(puzzle,ref depthChainCount,currentIndex,Dir.Left,chainType);
			chainCount += (depthChainCount >= 3)?depthChainCount:0;
		}else{
			int depthChainCount = 0;
			chainAction(puzzle,ref depthChainCount,currentIndex,Dir.Up,chainType);
			chainCount += (depthChainCount >= 3)?depthChainCount:0;
		}
		#endregion	
	}
	//check depth chain.
	//1 end equal piece index.  2 count check equal type
	void chainAction(PuzzleModel puzzle,ref int chainCount,int currentIndex,Dir dir,int chainType){
		//default dir chain
		if(PuzzleUtil.checkChain(puzzle,chainType,currentIndex,dir)){
			chainAction(puzzle,ref chainCount,currentIndex+(int)dir,dir,chainType);
		}else{	//this equal side end 
			int depthChainCount = 0;
			bool isCombo = false;
			chain(puzzle,ref depthChainCount,ref isCombo,currentIndex,PuzzleUtil.GetReflectDir(dir),chainType);
		}
	}
	
	public void ReflectOption(SaveOption save){
		this.PieceTypeMax = save.Difficulty;
	}
	
	#region cpu chain check
	void changeChain(ref bool isChain,Dir checkDir,Dir changeDir,int index){
		int chainCount = 1;
		checkChainCount(this,ref chainCount,index,checkDir,map[(int)changeDir+index].type);
		if(chainCount >= 3){
			isChain = true;
		}
	}
	//count check only
	void checkChainCount(PuzzleModel puzzle,ref int chainCount,int currentIndex,Dir dir,int chainType){
		//default dir chain
		if(PuzzleUtil.checkChain(puzzle,chainType,currentIndex,dir)){
			checkChainCount(puzzle,ref chainCount,currentIndex+(int)dir,dir,chainType);
		}else{	//this equal side end 
			int index = -(int)dir;
			while(PuzzleUtil.checkChain(puzzle,chainType,currentIndex+index,PuzzleUtil.GetReflectDir(dir))){
				chainCount++;
				index += -(int)dir;
			}
			//			Debug.Log("chainCount : " + chainCount);
		}
	}
	
	public bool ChangeSideChain(ref bool isChain,int index,Dir checkDir){
		if(map[index].ChangeEnableState()){
			isChain = false;
			changeChain(ref isChain,Dir.Up,checkDir,index);
			changeChain(ref isChain,Dir.Down,checkDir,index);
			changeChain(ref isChain,Dir.Right,checkDir,index);
			changeChain(ref isChain,Dir.Left,checkDir,index);
			return isChain;
		}
		return false;
	}
	#endregion
	
	#region cpu action check
	public int GetMaxHeight(){
		for(int i = map.Count-1;i >= width;i--){
			if(!map[i].isTypeNone()){
				return i/width;
			}
		}
		return 1;
	}
	
	public int GetMinHeight(){
		for(int i = width;i < map.Count;i++){
			if(map[i].isTypeNone()){
				return i/width;
			}
		}
		return 1;
	}
	#endregion
	
	public void NoticePieceSpread(){
		map.ForEach (piece=>{
			piece.state = PieceState.Spread;
		});
	}
	
	#region Multi
	public int InstanceHindPiece(int hindCount){
		int count = 0;
		for(int i = 0;i < limitMap.Count;i++){
			if(limitMap[i].type == (int)PieceType.None){
				//				limitMap[i].type = UnityEngine.Random.Range((int)PieceType.Red,(int)PieceTypeMax+1);
				limitMap[i].type = (int)PieceType.Hind;
				limitMap[i].state = PieceState.Stand;
				count++;
				if(hindCount <= count)break;
			}
		}
		return count;
	}
	
	bool getEqualPieceType(PuzzleModel puzzle,int currentIndex,Dir dir,int equalPieceType){
		if(dir == Dir.Right || dir == Dir.Left){
			if(PuzzleUtil.isEqualHeight(puzzle,currentIndex,currentIndex+(int)dir)){
				return puzzle.map[currentIndex+(int)dir].type == equalPieceType;
			}
		}
		if(dir == Dir.Up || dir == Dir.Down){
			if(!PuzzleUtil.isOutDir(puzzle,currentIndex,dir)){
				return puzzle.map[currentIndex+(int)dir].type == equalPieceType;
			}
		}
		return false;
	}
	
	void setHindToChain(PuzzleModel puzzle,int currentIndex){
		if(getEqualPieceType(puzzle,currentIndex,Dir.Up,(int)PieceType.Hind))
			puzzle.map[currentIndex+(int)Dir.Up].state = PieceState.ChainHind;
		if(getEqualPieceType(puzzle,currentIndex,Dir.Down,(int)PieceType.Hind))
			puzzle.map[currentIndex+(int)Dir.Down].state = PieceState.ChainHind;
		if(getEqualPieceType(puzzle,currentIndex,Dir.Right,(int)PieceType.Hind))
			puzzle.map[currentIndex+(int)Dir.Right].state = PieceState.ChainHind;
		if(getEqualPieceType(puzzle,currentIndex,Dir.Left,(int)PieceType.Hind))
			puzzle.map[currentIndex+(int)Dir.Left].state = PieceState.ChainHind;
	}
	#endregion
}

#region Multi Skill

/// <summary>
/// multi character skill.
/// this code is logic only.
/// </summary>
public class Skill{
	public float skillEnergy;
	public SkillDataModel skillDM;
	BaseSkill skill;
	public List<int> skillTargetIndex{get;protected set;}
	public Skill(SkillDataModel dm,PuzzleModel puzzle){
		this.skillDM = dm;
		skillEnergy = 100.0f;
		skillTargetIndex = new List<int>();
		skill = SkillFactory.getSkill(skillDM.id,puzzle);
	}
	
	public List<int> GetSkillTarget(){
		skillTargetIndex = skill.GetSkillTarget();
		return skillTargetIndex;
	}
	
	public void Execute(){
		skill.Execute(skillTargetIndex);
	}
	
	public void AddSkillEnergy(int comboCount,int chainCount){
		skillEnergy += comboCount*(chainCount);
		if(skillEnergy > 100.0f){skillEnergy = 100.0f;}
	}
}

static public class SkillFactory{
	static public BaseSkill getSkill(int skillId){
		switch(skillId){
		case (int)SkillType.None:
			return null;
		case (int)SkillType.RandomHind:
			return new RandomHind();
		case (int)SkillType.UnderHind2:
			return new UnderHind2();
		case (int)SkillType.RandomBroken:
			return new RandomBroken();
		case (int)SkillType.AllHindToNormal:
			return new AllHindToNormal();
		default:
			return null;
		}
	}
	static public BaseSkill getSkill(int skillId,PuzzleModel puzzle){
		var skill = getSkill (skillId);
		skill.puzzle = puzzle;
		return skill;
	}
}

public enum SkillType : int{
	None,
	RandomHind,
	UnderHind2,
	RandomBroken,
	AllHindToNormal,
}


abstract public class BaseSkill{
	//	public PuzzleModel puzzle;
	public PuzzleModel puzzle{protected get;set;}
	public BaseSkill(){}
	public BaseSkill(PuzzleModel puzzle){
		this.puzzle = puzzle;
	}
	public abstract List<int> GetSkillTarget();
	public abstract void Execute(List<int> targetList);
}

public class RandomHind : BaseSkill{
	//	public RandomHind(PuzzleModel puzzle) : base(puzzle){}
	readonly int changePiece = 12;
	public override List<int> GetSkillTarget (){
		List<int> targetList = new List<int>();
		List<PieceModel> p = puzzle.map.GetRange(puzzle.width,puzzle.map.Count-puzzle.width);
		if(p.Count(piece=>piece.isDeletePiece()) < changePiece){
			targetList.AddRange(p.Where(piece=>piece.isDeletePiece()).Select (piece=>piece.index));
			Debug.Log("piece < 12");
		}else{
			int count = 0;
			while(count < changePiece){
				int target = UnityEngine.Random.Range(puzzle.width,puzzle.map.Count);
				if(puzzle.map[target].isDeletePiece() && !targetList.Contains(target)){
					targetList.Add(target);
					count++;
				}
			}
			Debug.Log("piece > 12");
		}
		return targetList;
	}
	
	public override void Execute (List<int> targetList){
		targetList.ForEach (index=>{
			puzzle.map[index].type = (int)PieceType.Hind;
			Debug.Log("index : " + index);
		});
	}
}

public class UnderHind2 : BaseSkill{
	//	public UnderHind2(PuzzleModel puzzle) : base(puzzle){}
	readonly int changePiece = 12;
	public override List<int> GetSkillTarget (){
		List<int> targetList = new List<int>();
		for(int i = puzzle.width;i < changePiece+puzzle.width;i++){
			if(puzzle.map[i].isDeletePiece()){
				targetList.Add(i);
			}
		}
		return targetList;
	}
	
	public override void Execute (List<int> targetList){
		targetList.ForEach (index=>{
			puzzle.map[index].type = (int)PieceType.Hind;
		});
	}
}

public class RandomBroken : BaseSkill{
	readonly int brokenPiece = 12;
	
	public override List<int> GetSkillTarget (){
		List<int> targetList = new List<int>();
		List<PieceModel> p = puzzle.map.GetRange(puzzle.width,puzzle.map.Count-puzzle.width);
		if(p.Count(piece=>piece.isDeletePiece()) < brokenPiece){
			targetList.AddRange(p.Where(piece=>!piece.isTypeNone()).Select (piece=>piece.index));
			Debug.Log("piece < 12");
		}else{
			int count = 0;
			while(count < brokenPiece){
				int target = UnityEngine.Random.Range(puzzle.width,puzzle.map.Count);
				if(!puzzle.map[target].isTypeNone() && !targetList.Contains(target)){
					targetList.Add(target);
					count++;
				}
			}
			Debug.Log("piece > 12");
		}
		return targetList;
	}
	
	public override void Execute (List<int> targetList){
		targetList.ForEach (index=>{
			//			puzzle.map[index].state = PieceState.Chain;
			//			puzzle.map[index].isChain = true;
			puzzle.map[index].state = PieceState.BrokenImmediate;
			Debug.Log("index : " + index);
		});
	}
}

public class AllHindToNormal : BaseSkill{
	public override List<int> GetSkillTarget (){
		List<int> targetList = new List<int>();
		for(int i = puzzle.width;i < puzzle.map.Count;i++){
			if(puzzle.map[i].type == (int)PieceType.Hind){
				targetList.Add (i);
			}
		}
		return targetList;
	}
	
	public override void Execute (List<int> targetList){
		targetList.ForEach (index=>{
			puzzle.map[index].type = UnityEngine.Random.Range((int)PieceType.Red,puzzle.PieceTypeMax+1);
		});
	}
}
#endregion


public class PieceModel{
	public int index;
	public int type;
	public float gravityY;
	public PieceState state;
	public ComboState comboState;
	public bool isHideArea;		//range map[0 ~ width]
	public bool isChain;
	
	public static readonly string[] PieceSpriteNames = new string[]{"dummy","red","green","blue","yellow","magenta","cyan","hind"};
	public static readonly Color[] PieceSpriteColor = new Color[]{Color.black,Color.red,Color.green,Color.blue,Color.yellow,Color.magenta,Color.cyan,Color.gray};
	
	public PieceModel(){}
	public PieceModel(int index){
		this.index = index;
		type = 0;
		isChain = false;
	}
	
	
	public void SetNone(){
		type = (int)PieceType.None;
		state = PieceState.None;
		comboState = ComboState.None;
	}
	
	public void Broken(){
		type = (int)PieceType.None;
		state = PieceState.None;
	}
	
	public void TypeChange(PieceModel change){
		this.type ^= change.type;
		change.type ^= this.type;
		this.type ^= change.type;
	}
	
	public void StateChange(PieceModel change){
		var s = this.state;
		this.state = change.state;
		change.state = s;
	}
	
	public void SetPiece(PieceType type,PieceState state){
		this.type = (int)type;
		this.state = state;
	}
	
	public bool changeEnable(int targetIndex){
		return (targetIndex == this.index+1 || targetIndex == this.index-1);
	}
	
	public bool ChangeEnableState(){
		return !isChain && !isTypeNone() && state == PieceState.Stand;
	}
	
	public bool isDeletePiece(){
		return (type != (int)PieceType.None && type != (int)PieceType.Hind);
	}
	
	public bool isTypeNone(){
		return type == (int)PieceType.None;
	}
}

[System.Serializable]
public class TimerModel{
	[HideInInspector]public float upperTime;
	//	public float upperSpeed;
	public float upperSpeed{private get;set;}
	float currentSpeed;
	
	readonly float UserUpSpeed = 256;
	public float upperLimit;
	public UnityAction upperAction;
	public bool isEnable = false;
	public bool isUpperMax;		//game over line
	public bool isFallPiece;		//pause timer
	
	int pauseTimerCount = 0;
	public void PauseTimer(){
		pauseTimerCount++;
	}
	public void ResumeTimer(){
		pauseTimerCount--;
	}
	
	public bool IsPauseTimer(){
		return pauseTimerCount > 0;
	}
	
	public void SetInitCurrentSpeed(float speed){
		currentSpeed = speed;
	}
	
	public void TimerUpdate(){
		if(isEnable && pauseTimerCount == 0 && !isUpperMax && !isFallPiece){
			upperTime += currentSpeed/30;
			if(upperTime > upperLimit){
				if(upperAction != null){upperAction();}
				upperTime = 0;
			}
		}
	}
	
	public void SpeedUp(){
		currentSpeed = UserUpSpeed;
	}
	
	public void SpeedNormal(){
		currentSpeed = upperSpeed;
	}
}


public class PuzzleUtil{
	public static readonly int BlockTypeMax = 4;
	public static bool checkChain(PuzzleModel puzzle,int chainType,int current,Dir dir){
		if(current+(int)dir < 0 || current+(int)dir >= puzzle.map.Count){return false;}
		return (!isOutDir(puzzle,current,dir) &&
		        puzzle.map[current+(int)dir].type == chainType && 
		        puzzle.map[current+(int)dir].state == PieceState.Stand);
	}
	public static bool isOutLeft(PuzzleModel puzzle,int current){
		return ((current % puzzle.width) == puzzle.width-1);
		//		return ((current % puzzle.width)==0);
	}
	public static bool isOutRight(PuzzleModel puzzle,int current){
		//		return ((current % puzzle.width) == puzzle.width-1);
		return ((current % puzzle.width)==0);
	}
	public static bool isOutUp(PuzzleModel puzzle,int current){
		return ((current / puzzle.width) >= puzzle.height-1);
	}
	public static bool isOutDown(PuzzleModel puzzle,int current){
		return ((current / puzzle.width) <= 1);		//0 is addPieceMap Area
	}		
	public static bool isEqualHeight(PuzzleModel puzzle,int index1,int index2){
		return (index1 / puzzle.width) == (index2 / puzzle.width);
	}
	public static bool isOutDir(PuzzleModel puzzle,int current,Dir dir){
		if(dir == Dir.Up){return isOutUp(puzzle,current);}
		if(dir == Dir.Down){return isOutDown(puzzle,current);}
		if(dir == Dir.Right){return isOutRight(puzzle,current);}
		if(dir == Dir.Left){return isOutLeft(puzzle,current);}
		else{Debug.LogError("invalid dir type : " + dir);return true;}
	}
	
	public static Dir GetReflectDir(Dir dir){
		if(dir == Dir.Up)return Dir.Down;
		if(dir == Dir.Down)return Dir.Up;
		if(dir == Dir.Right)return Dir.Left;
		if(dir == Dir.Left)return Dir.Right;
		return Dir.None;
	}
}

public class PuzzleEvent{
	/// <summary>
	/// The chain event.
	/// send value is (comboCount,chainCount)
	/// </summary>
	public UnityAction<int,int> ChainEvent;
	/// <summary>
	/// The fall event.
	/// send value is (fallCount)
	/// </summary>
	public UnityAction<int> FallEvent;
	/// <summary>
	/// The game over event.
	/// </summary>
	public UnityAction GameOverEvent;
	/// <summary>
	/// The timer upper event.
	/// </summary>
	public UnityAction TimerUpperEvent;
}

public enum PieceType : int{
	None,
	Red,
	Green,
	Blue,
	Yellow,
	Magenta,
	Cyan,
	Hind,
}

public enum PieceState : int{
	None,
	Stand,
	Fall,
	Chain,
	Broken,
	BrokenImmediate,
	ChainHind,
	ChangeHind,
	Spread,
}


public enum ComboState : int{
	None,
	Fall,
}

public enum Dir : int{
	None = 0,
	Up = 6,
	Right = -1,
	Left = 1,
	Down = -6,
}