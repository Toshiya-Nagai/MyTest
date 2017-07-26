using UnityEngine;
using System.Collections;

abstract public class BaseMainMaster : BaseSceneMaster {

	public bool nextState{get;set;}

	public abstract IEnumerator Initialize();
	public abstract IEnumerator Setup();
	public abstract IEnumerator Ready();
	public abstract IEnumerator Play();
	public abstract IEnumerator GameOver();
	public abstract IEnumerator End();
}
