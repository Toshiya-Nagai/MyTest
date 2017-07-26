using System.Linq;
using UnityEngine;
using UnityEditor.AnimatedValues;
using UnityEngine.UI;

namespace UnityEditor.UI
{
    /// <summary>
    /// Editor class used to edit UI Sprites.
    /// </summary>

    [CustomEditor(typeof(U_UISprite), true)]
    [CanEditMultipleObjects]
    public class UISpriteInspector : ImageEditor{
		public override void OnInspectorGUI (){
			var sprite = target as U_UISprite;
			sprite.atlas = (U_UIAtlas)EditorGUILayout.ObjectField("Atlas",sprite.atlas,typeof(U_UIAtlas),false);
			base.OnInspectorGUI ();
		}
    }
}
