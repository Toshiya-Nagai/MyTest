using UnityEngine;
using System.Collections;

public class BaseChart : SceneSingleton<BaseChart>{
//	public T master;
	public BaseMainMaster master;
	public BaseFlow flow;
	public FlowChart chart;
	
	void Start(){
		flow = FlowFactory.getFlow<BaseMainMaster>(FlowChart.Initialize,master);
		flow.Init(master);
		StartCoroutine(loop());
	}
	
	public IEnumerator loop(){
		while(true){
			yield return StartCoroutine(flow.Execute());
			if(flow.isEnd){
				flow.Death();
				if(flow.NextChart == FlowChart.End){break;}
				else{chart = flow.NextChart;flow = FlowFactory.getFlow<BaseMainMaster>(flow.NextChart,master);}
			}
		}
		End();
	}
	public void End(){
		Debug.Log("Chart End");
		StartCoroutine(master.End());
	}
	public override void Release (){
	}
	
	void OnApplicationQuit(){
		StopCoroutine(this.loop());
	}
}


abstract public class BaseFlow{
	protected BaseMainMaster master;
	public bool isEnd;
	public virtual void Init(BaseMainMaster master){
		this.master = master;
		master.nextState = false;
		isEnd = false;
	}
	public virtual void Death(){
	}
	abstract public FlowChart NextChart{get;}
	abstract public IEnumerator Execute();
}

public class Initialize : BaseFlow{
	public override FlowChart NextChart {
		get {return FlowChart.Setup;}
	}
	override public IEnumerator Execute (){
		yield return master.StartCoroutine(master.Initialize());
		isEnd = true;
	}
}

public class Setup : BaseFlow{
	public override FlowChart NextChart {
		get {return FlowChart.Ready;}
	}
	override public IEnumerator Execute (){
		yield return master.StartCoroutine(master.Setup());
		isEnd = true;
	}
}

public class Ready : BaseFlow{
	public override FlowChart NextChart {
		get {return FlowChart.Play;}
	}
	override public IEnumerator Execute (){
		yield return master.StartCoroutine(master.Ready());
		isEnd = true;
	}
}

public class Play : BaseFlow{
	public override FlowChart NextChart {
		get {return FlowChart.GameOver;}
	}
	override public IEnumerator Execute (){
		yield return master.StartCoroutine(master.Play());
		isEnd = master.nextState;
	}
}

public class GameOver : BaseFlow{
	public override FlowChart NextChart {
		get {return FlowChart.End;}
	}
	override public IEnumerator Execute (){
		yield return master.StartCoroutine(master.GameOver());
		isEnd = true;
	}
}


public enum FlowChart : int{
	Initialize,
	Setup,
	Ready,
	Play,
	GameOver,
	End,
}


static public class FlowFactory{
	private static Initialize init{get{return new Initialize();}}
	private static Setup setup{get{return new Setup();}}
	private static Ready ready{get{return new Ready();}}
	private static Play play{get{return new Play();}}
	private static GameOver gameover{get{return new GameOver();}}
	private static BaseFlow[] flows = new BaseFlow[]{init,setup,ready,play,gameover};
	static public BaseFlow getFlow(FlowChart flow){
		return flows[(int)flow];
	}
	static public BaseFlow getFlow<T>(FlowChart flow,T master) where T : BaseMainMaster{
		Debug.Log("Current Chart is : " + flow.ToString());
		var buf = flows[(int)flow];
		buf.Init (master);
		return buf;
	}
}