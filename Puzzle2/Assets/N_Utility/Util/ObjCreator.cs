using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UniLinq;

namespace N_Creator{
	
	//grid item create master
	//T1 : DataModel	T2 : Component and have IInstanceObjects<DataModel> Interface
//	public class ObjCreateMaster<TModel,TController> where TModel : class where TController : Controller<TModel>{
	public class ObjCreator<TModel,TController> where TModel : class where TController : Controller<TModel>{
		protected ObjCreator creator;
		protected ObjDataSetter<TModel,TController> setter;
		
		public List<GameObject> ObjList{get;set;}
		/*
		public ObjCreateMaster(){
			creator = new ObjItemCreator();
			setter = new ObjDataSetter<T1, T2>();
		}
		*/
		public ObjCreator(GameObject parentObj,GameObject baseObj,int itemCount){
			creator = new ObjCreator(baseObj,parentObj,itemCount);
			setter = new ObjDataSetter<TModel, TController>();
		}
		public ObjCreator(MonoBehaviour parent,GameObject baseObj,int itemCount) : this(parent.gameObject,baseObj,itemCount){
		}

		public void SetItemCount(int itemCount){
			creator.ItemCount = itemCount;
		}
		
		#region main method
		public void Create(List<TModel> dataList){
			Create(dataList,creator.ItemCount);
		}
		
		public void Create(List<TModel> dataList,int itemCount){
			Create(dataList,itemCount,null);
		}

		public void Create(List<TModel> dataList,UnityAction<TController> uniqueSetter){
			Create (dataList,creator.ItemCount,uniqueSetter);
		}

		public void Create(List<TModel> dataList,int itemCount,UnityAction<TController> uniqueSetter){
			setter.DataList = dataList;
			creator.ItemCount = itemCount;
			ObjList = creator.Create();
//			setter.Set(ObjList);
			setter.Set(ObjList,uniqueSetter);
			update();
		}

		public void update(){
			setter.update();
		}
		
		public void DestroyItem(){
			creator.DestroyItem();
			ObjList.Clear();
		}
		#endregion
	}

	public abstract class BaseObjDataSetter{
		public abstract void Set(List<GameObject> setObjList);
	}
	
	/// <summary>
	/// Grid item controller.
	/// T1 : SetDataModel
	/// T2 : Set Obj Data Component(MonoBehaviour,IInstanceObjecs<DataModel>)
	/// </summary>
	[System.Serializable]
	public class ObjDataSetter<TModel,TController> : BaseObjDataSetter where TModel : class where TController : Controller<TModel>{
		
		public ObjDataSetter(){SetComponentList = new List<TController>();}
		public ObjDataSetter(List<TModel> dataList){
			this.DataList = dataList;
			SetComponentList = new List<TController>();
		}
		
		public List<TModel> DataList{get;set;}
		public List<TController> SetComponentList{get;set;}

//		public override void Set (List<GameObject> anInstanceList) {
//			SetComponentList.Clear();
//			for(int i = 0;i < DataList.Count;i++){
//				if(anInstanceList.Count > i){
//					T2 setComponent = anInstanceList[i].GetComponent<T2>();
//					setComponent.Set(DataList[i]);
//					SetComponentList.Add(setComponent);
//				}
//			}
//		}

//		public override void Set (List<GameObject> anInstanceList) {
//			SetComponentList.Clear();
//			var list = anInstanceList._Where(x=>x.GetComponent<TController>() != null)
//				._Select(x=>x.GetComponent<TController>())._Take(DataList.Count)._ToList();
//			for(int i = 0;i < DataList.Count;i++){
//				if(i < list.Count){
//					list[i].Set(DataList[i]);
//				}
//			}
//			SetComponentList = list;
//		}

		public override void Set (List<GameObject> instanceList) {
			Set(instanceList,null);
		}

		public void Set(List<GameObject> instanceList,UnityAction<TController> uniqueSetter){
			SetComponentList.Clear();
			var list = instanceList.Where(x=>x.GetComponent<TController>() != null)
				.Select(x=>x.GetComponent<TController>()).Take(DataList.Count).ToList();
			for(int i = 0;i < DataList.Count;i++){
				if(i < list.Count){
					list[i].Set(DataList[i]);
					if(uniqueSetter != null){
						uniqueSetter(list[i]);
					}
				}
			}
			SetComponentList = list;
		}

		public void update(){
			SetComponentList.ForEach (x=>{
				x.Observe();
			});
		}
		
		#region check method
		public bool checkDataListNull(){
			return (DataList == null)?true:false;
		}
		
		public bool checkSetComponentListNull(){
			return (SetComponentList == null)?true:false;
		}
		#endregion
	}
	
	/// <summary>
	/// Abstract grid object creator.
	/// BaseObj Create Data Number
	/// </summary>
	public abstract class BaseObjItemCreator{
		public abstract GameObject BaseObj{set;protected get;}
		public abstract int ItemCount{get;set;}
		public abstract GameObject ParentObj{set;protected get;}
		public abstract List<GameObject> Create();
	}
	
	[System.Serializable]
	public class ObjCreator : BaseObjItemCreator{
		public override GameObject BaseObj {protected get;set;}
		public override int ItemCount{get;set;}
		public override GameObject ParentObj{set;protected get;}
		public List<GameObject> InstanceCache{get;set;}
		
		protected bool checkBaseObjNull{
			get{return(BaseObj == null);}
		}
		protected bool checkParentObjNull{
			get{return(ParentObj == null);}
		}
		
		public ObjCreator(){
			this.BaseObj = null;
			this.ParentObj = null;
			this.ItemCount = 0;
			this.InstanceCache = new List<GameObject>();
		}
		public ObjCreator(GameObject baseObj,GameObject parentObj,int itemCount){
			this.BaseObj = baseObj;
			this.ParentObj = parentObj;
			this.ItemCount = itemCount;
			this.InstanceCache = new List<GameObject>();
		}
		
		#region main method
		public override List<GameObject> Create(){
			//Null Check
			if(checkCreateObjError()){return InstanceCache;}
			int newInstanceNum = ItemCount - InstanceCache.Count;
			if(newInstanceNum > 0){createNewItemList(newInstanceNum);}
			setActiveItemCount(this.ItemCount);
			return InstanceCache;
		}
		//item count set active
		private List<GameObject> setActiveItemCount(int itemCount){
			for(int i = 0;i < InstanceCache.Count;i++){
				bool active = (i < itemCount)?true:false;
				InstanceCache[i].SetActive(active);
			}
			return InstanceCache;
		}
		
		private List<GameObject> createNewItemList(int newItemCount){
			for(int i = 0;i < newItemCount;i++){
				GameObject item = createNewItem(this.BaseObj);
				item.name = i.ToString();
				if(item != null){
					InstanceCache.Add(item);
				}
			}
			return InstanceCache;
		}
		
		
		private GameObject createNewItem(GameObject baseObj){
			GameObject item = GameObject.Instantiate(baseObj) as GameObject;
			if(item != null){
				if(checkParentObjNull == false){
//					item.transform.parent = ParentObj.transform;
					item.transform.SetParent(ParentObj.transform);
					initTransform(item);
				}
			}else{

				Log.Warning("item is Null");
			}
			return item;
		}
		
		public void DestroyItem(){
//			if(checkParentObjNull == false){
//				var child = ParentObj.GetComponentsInChildren<Transform>();
////				var child = ParentObj.GetComponentsInChildren<Transform>()._Where(x=>x.gameObject != this.ParentObj);
//				foreach(var buf in child._Where(x=>x.gameObject != this.ParentObj)){
//					GameObject.Destroy(buf.gameObject);
//				}
//			}
			foreach(var buf in InstanceCache){
				buf.notNull(x=>{
					GameObject.Destroy(x);
					x = null;
				});
			}
			InstanceCache.Clear();
		}
		#endregion
		
		
		#region check method
		protected bool checkCreateObjError(){
			if(checkBaseObjNull || checkParentObjNull){
				Log.Info("BaseObj = " + this.BaseObj + "  ParentObj = " + this.ParentObj);
				return true;
			}
			return false;
		}
		#endregion
		
		private GameObject initTransform(GameObject item){
			item.SetActive(true);
			item.transform.localPosition = Vector3.zero;
			item.transform.localScale = new Vector3(1.0f,1.0f,1.0f);
			item.transform.localRotation = Quaternion.identity;
			return item;
		}	
	}
}