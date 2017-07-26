using UnityEngine;
using System.Collections;

public class GotoURL : MonoBehaviour {
	[SerializeField]
	private string _url;
	public string Url{
		get{return this._url;}
		set{this._url = value;}
	}

	void OnClick(){
		Application.OpenURL(Url);
	}
}
