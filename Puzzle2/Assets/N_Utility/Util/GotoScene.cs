using UnityEngine;
using System.Collections;

public class GotoScene : MonoBehaviour {
	public SceneName scene;

	virtual public void OnClick(){
		SceneUtil.instance.BaseChangeScene(scene);
	}
}
