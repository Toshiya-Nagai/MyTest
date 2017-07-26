using UnityEngine;
using System;
using System.Collections;



namespace N_FlagUtil{
	public static class FlagUtil{
		static public bool Check(int mainFlag,int flag){
			return ((mainFlag & flag) == flag);
		}
		static public void Reset(ref int mainFlag){
			mainFlag = 0;
		}
		static public void On(ref int mainFlag,int flag){
			if(!Check(mainFlag,flag)){mainFlag ^= flag;}
		}
		static public void Off(ref int mainFlag,int flag){
			if(Check(mainFlag,flag)){mainFlag ^= flag;}
		}
	}

	/// <summary>
	/// Flag control wrapper.
	/// Generics T is Enum Type.
	/// Flag<SampleEnum> test = new Flag<SampleEnum>();
	/// </summary>
	public class Flag<T> where T : struct,IConvertible{
		public int flag{get;private set;}
		public Flag(){
			if(!typeof(T).IsEnum){
//				throw new Exception("invalid data type is : " + typeof(T));
				Debug.LogError("invalid data type is : " + typeof(T));
			}
			this.flag = 0;
		}
		public T GetEnum(){
			try{
				var buf = Enum.Parse(typeof(T),this.flag.ToString());
				return (T)buf;
			}catch(ArgumentException){
				Debug.LogError("failed cast flag : " + this.flag.ToString());
				return default(T);
			}
		}
		public bool Check(T check){
			try{
				int checkValue = Convert.ToInt32(check);
				return ((this.flag & checkValue)==checkValue);
			}catch(ArgumentException){
				Debug.LogError("failed cast checkFlag : " + check);
				return false;
			}
		}
		public bool Check(int check){
			return ((this.flag & check) == check);
		}
		public void On(T onFlag){
			try{
				int flag = System.Convert.ToInt32(onFlag);
				if(!Check(onFlag)){this.flag ^= flag;}
			}catch(ArgumentException){
				Debug.LogError("failed cast onFlag : " + onFlag);
			}
		}
		public void Off(T offFlag){
			try{
				int flag = System.Convert.ToInt32(offFlag);
				if(Check(offFlag)){this.flag ^= flag;}
			}catch(ArgumentException){
				Debug.LogError("failed cast offFlag : " + offFlag);
			}
		}
		public void Reset(){
			this.flag = 0;
		}
	}

	/*
	[System.Flags]
	public enum SampleFlag : int{
		None = 0x0000,
		Atk = 0x0001,
		Magnetic = 0x0002,
		Recover = 0x0004,
		Warp = 0x0008,
	}
	*/

}