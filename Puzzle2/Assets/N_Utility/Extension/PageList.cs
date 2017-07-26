using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UniLinq;

public class PageList<T>{
	public int PageIndex{get;protected set;}
	public int PageColumn{get;protected set;}
	public int MaxPage{
		get{
			if(errorParams()){
				Log.Error("error list null or page column is 0");
				return 0;
			}else{return calcMaxPage();}
		}
	}
	protected List<T> TargetList{get;private set;}
	List<T> cache;
	public bool IsEndPage{
		get{return ((PageIndex+1) >= MaxPage);}
	}
	public bool IsFirstPage{
		get{return (PageIndex <= 0);}
	}
	
	public PageList(List<T> targetList,int pageColumn){
		PageIndex = 0;
		PageColumn = pageColumn;
		this.TargetList = targetList;
		cache = new List<T>();
		Log.Info("PageList Count : {0}",TargetList.Count);
	}

	public PageList(ICollection<T> targetList,int pageColumn) : this(targetList.ToList(),pageColumn){
	}
	
	public List<T> GetCurrentPage(){
		return getList(PageIndex);
	}
	
	public List<T> GetNextPage(){
		int maxPage = MaxPage;
		if(PageIndex+1 >= maxPage){
			Log.Warning("error current max page. return max page");
			PageIndex = maxPage-1;
			return GetCurrentPage();
		}
		PageIndex++;
		return getList (PageIndex);
	}
	
	public List<T> GetPrevPage(){
		if(IsFirstPage){
			Log.Warning("error current min page. return min page");
			return GetCurrentPage();
		}
		PageIndex--;
		return getList(PageIndex);
	}
	
	public List<T> GetSelectPage(int pageIndex){
		if(pageIndex >= MaxPage || pageIndex <= 0){
			Log.Warning("error select page is out of min or max. select is : " + pageIndex + "  max : " + MaxPage);
			return GetCurrentPage();
		}
		PageIndex = pageIndex;
		return getList(PageIndex);
	}
	
	
	protected List<T> getList(int pageIndex){
		cache.Clear();
		if(errorParams()){Log.Error("error list null or page column is 0");return cache;}
		int currentIndex = PageIndex*PageColumn;
		for(int i = currentIndex;i < (currentIndex + PageColumn);i++){
			if(i < TargetList.Count){
				cache.Add(TargetList[i]);
			}
		}
		return cache;
	}

	private bool errorParams(){
		return (TargetList == null)?true:(PageColumn == 0)?true:false;
	}
	
	private int calcMaxPage(){
		if(TargetList.Count / PageColumn == 0){return 1;}		//start page
		if(TargetList.Count % PageColumn == 0){return TargetList.Count / PageColumn;}
		else{return (TargetList.Count / PageColumn)+1;}
	}
	
	public string GetPageToString(){
		return (PageIndex+1).ToString() + " / " + MaxPage.ToString();
	}
}


/*
public class N_PageEvent<T> : N_PageList<T>{
	public event System.Action FirstPageAction;
	public event System.Action EndPageAction;
	public N_PageEvent(List<T> anTargetList,int anPageColumn) : base(anTargetList,anPageColumn){
	}

	public new List<T> GetNextPage(){
		if(IsEndPage){
			Debug.Log("End");
			return GetCurrentPage();
		}
		PageIndex++;
		if(IsEndPage && EndPageAction != null){EndPageAction();}
		return getList (PageIndex);
	}
	public new List<T> GetPrevPage(){
		if(IsFirstPage){
			Debug.Log("First");
			return GetCurrentPage();
		}
		PageIndex--;
		if(IsFirstPage && FirstPageAction != null){FirstPageAction();}
		return getList(PageIndex);
	}
}
*/