using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections;

public class CreateScrollEditor{
	[MenuItem("uGUI/UI/Scroll/Horizonal")]
	public static void CreateScrollHorizonal(){
		createScroll(false);
	}
	
	[MenuItem("uGUI/UI/Scroll/Vertical")]
	public static void CreateScrollVertical(){
		createScroll(true);
	}

	static void createScroll(bool isVertical){
		GameObject panel = new GameObject("ScrollRect");
		GameObject grid = new GameObject("grid");
		GameObject node = new GameObject("node");
		grid.transform.parent = panel.transform;
		node.transform.parent = grid.transform;
		panelSetup(panel,isVertical);
		gridSetup(grid,isVertical);
		nodeSetup(node,isVertical);
		setup(panel,grid);
	}

	static void panelSetup(GameObject panel,bool isVertical){
		var rect = panel.AddComponent<ScrollRect>();
		rect.horizontal = (isVertical)?false:true;
		rect.vertical = (isVertical)?true:false;
		panel.AddComponent<Mask>();
		panel.AddComponent<Image>();
	}

	static void gridSetup(GameObject grid,bool isVertical){
		HorizontalOrVerticalLayoutGroup layout = (isVertical)?grid.AddComponent<VerticalLayoutGroup>() as HorizontalOrVerticalLayoutGroup:grid.AddComponent<HorizontalLayoutGroup>() as HorizontalOrVerticalLayoutGroup;
		var fitter = grid.AddComponent<ContentSizeFitter>();
		layout.childAlignment = TextAnchor.UpperCenter;
		layout.childForceExpandHeight = (isVertical)?false:true;
		layout.childForceExpandWidth = (isVertical)?true:false;
		fitter.horizontalFit = (isVertical)?ContentSizeFitter.FitMode.Unconstrained:ContentSizeFitter.FitMode.PreferredSize;
		fitter.verticalFit = (isVertical)?ContentSizeFitter.FitMode.PreferredSize:ContentSizeFitter.FitMode.Unconstrained;
	}
	static void nodeSetup(GameObject node,bool isVertical){
		node.AddComponent<Image>();
		var layout = node.AddComponent<LayoutElement>();
		layout.minWidth = (isVertical)?0:60;
		layout.minHeight = (isVertical)?60:0;
	}

	static void setup(GameObject panel,GameObject grid){
		var rect = panel.GetComponent<ScrollRect>();
		rect.content = grid.transform as RectTransform;
	}

}