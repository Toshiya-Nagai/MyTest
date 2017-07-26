using UnityEngine;
using System.Collections;
using System.Collections.Generic;


#region Controller
public interface IController<T>{
	void Set(T data);
	View<T> View{get;set;}
	void Observe();
	void Observe(T data);
}


public abstract class Controller<T> : MonoBehaviour,IController<T> where T : class{
	protected T Data{get;set;}
	public virtual View<T> View{get;set;}
	public void Awake(){View = this.GetComponent<View<T>>();}
	public virtual void Set(T data){
		this.Data = data;
		if(View != null){View.Set(data);}
	}
	public virtual void Observe(){
		if(View != null){View.UpdateUI();}
	}
	public virtual void Observe(T data){
		Set (data);
		Observe();
	}
	static public void Set(GameObject obj,T data){
		var controller = obj.GetComponent<Controller<T>>();
		if(controller != null){controller.Set (data);}
		else{Debug.LogWarning("controller is null");}
	}
}
#endregion

#region View
public interface IView{
	void UpdateUI();
}

public abstract class View<T> : MonoBehaviour,IView{
	public virtual T Data{protected get;set;}
	public abstract void UpdateUI();
	public virtual void Set(T data){this.Data = data;}
}


public abstract class Views<T> : View<T>{
	public List<GameObject> ViewObjects;
	protected List<View<T>> views;
	void Awake(){
		views = new List<View<T>>();
		ViewObjects.ForEach (x=>{
			var view = x.GetComponent<View<T>>();
			view.notNull(xx=>{views.Add(xx);});
			setView(view);
		});
	}
	public override void Set (T data){
		base.Set (data);
		views.notNull(x=>{x.ForEach (xx=>{xx.Set(data);});});
	}
	public override void UpdateUI (){
		views.notNull(x=>{x.ForEach (xx=>{xx.UpdateUI ();});});
	}
	protected virtual void setView(View<T> target){
	}
}



public class GaugeModel{
	public int max;
	public int current;
}
#endregion


namespace MvcSample2{
	public interface IController{
		void Set(object data);
		View view{get;set;}
		void update();
		void update(object data);
	}
	public abstract class Controller : MonoBehaviour,IController{
		protected object data{get;set;}
		public virtual View view{get;set;}
		public void Awake(){view = this.GetComponent<View>();}
		public virtual void Set(object data){
			this.data = data;
			view.notNull(x=>{x.setData(data);});
		}
		public virtual void update(){
			view.notNull(x=>{x.UpdateUI();});
		}
		public virtual void update(object data){
			Set (data);
			update();
		}
		static public void Set(GameObject obj,object data){
			var controller = obj.GetComponent<Controller>();
			controller.Set(data);
		}
	}

	public abstract class View : MonoBehaviour,IView{
		public virtual object Data{protected get;set;}
		public abstract void UpdateUI();
		public virtual void setData(object data){this.Data = data;}
	}
}