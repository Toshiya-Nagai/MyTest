using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Holoville.HOTween;

namespace N_NGUIUtil{
	public static class Alpha{
		public static float AutoFadeDelay = 0.15f;
		#region Fade In
		static public void FadeIn<T>(T target)where T : class{
			FadeIn(target,null);
		}
		static public void FadeIn<T>(T target,Action callback)where T : class{
			FadeIn(target,callback,AutoFadeDelay);
		}

		static public void FadeIn<T>(T target,Action callback,float delay)where T : class{
			FadeIn(target,callback,delay,Color.white);
		}

		static public void FadeIn<T>(T target,Action callback,float delay,Color color) where T : class{
			Fade (target,callback,delay,new Color(color.r,color.g,color.b,0.0f));
		}
		#endregion


		#region Fade Out
		static public void FadeOut<T>(T target)where T : class{
			FadeOut(target,null);
		}
		
		static public void FadeOut<T>(T target,Action callback)where T : class{
			FadeOut(target,callback,AutoFadeDelay);
		}
		
		static public void FadeOut<T>(T target,Action callback,float delay)where T : class{
			FadeOut(target,callback,delay,Color.white);
		}
		
		static public void FadeOut<T>(T target,Action callback,float delay,Color color) where T : class{
			Fade (target,callback,delay,new Color(color.r,color.g,color.b,1.0f));
		}
		#endregion
		static public void Fade<T>(T target,Action callback,float delay,Color color) where T : class{
			HOTween.To (target,delay,new TweenParms().Prop("color",color).Ease (EaseType.Linear).OnComplete (x=>{
				callback.notNull (xx=>{xx();});
			}));
		}

	}
}