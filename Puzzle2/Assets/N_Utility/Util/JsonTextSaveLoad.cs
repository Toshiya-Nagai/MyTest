using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections;
using LitJson2;

//public class N_SaveLoad : MonoBehaviour {
//	public void Save(string path,object data){
//		saveFile(path,data);
//	}
//	
//	public JsonData Load(string path){
//		return loadFile(path);
//	}
//	
//	public T Load<T>(string path){
//		return loadFile<T>(path);
//	}
//	
//	private JsonData loadFile(string path){
//		var str = loadStr(path);
//		if(str == ""){return null;}
//		return JsonMapper.ToObject(str);
//	}
//	
//	private T loadFile<T>(string path){
//		var str = loadStr(path);
//		if(str == ""){return default(T);}
//		return JsonMapper.ToObject<T>(str);
//	}
//	
//	private void saveFile(string path,object data){
//		try{
//			using(StreamWriter sw = new StreamWriter(path,false,Encoding.UTF8)){
//				var str = JsonMapper.ToJson(data);
//				sw.Write(str);
//			}
//		}catch(Exception e){
//			Debug.LogError(e.Message);
//		}
//	}
//	
//	
//	private string loadStr(string path){
//		try{
//			using(StreamReader sr = new StreamReader(path,Encoding.UTF8)){
//				string str = sr.ReadToEnd();
//				Debug.Log(str);
//				return str;
//			}
//		}catch(Exception e){
//			Debug.LogError(e.Message);
//			return "";
//		}
//	}
//}


public static class JsonTextSaveLoad{
	static public void Save(string path,object data){
		saveFile(path,data);
	}
	
	static public JsonData Load(string path){
		return loadFile(path);
	}
	
	static public T Load<T>(string path){
		return loadFile<T>(path);
	}
	
	static private JsonData loadFile(string path){
		var str = loadStr(path);
		if(str == ""){return null;}
		return JsonMapper.ToObject(str);
	}
	
	static private T loadFile<T>(string path){
		var str = loadStr(path);
		if(str == ""){return default(T);}
		return JsonMapper.ToObject<T>(str);
	}
	
	static private void saveFile(string path,object data){
		try{
			using(StreamWriter sw = new StreamWriter(path,false,Encoding.UTF8)){
				var str = JsonMapper.ToJson(data);
				sw.Write(str);
			}
		}catch(Exception e){
			Debug.LogError(e.Message);
		}
	}
	
	
	static private string loadStr(string path){
		try{
			using(StreamReader sr = new StreamReader(path,Encoding.UTF8)){
				string str = sr.ReadToEnd();
				Debug.Log(str);
				return str;
			}
		}catch(Exception e){
			Debug.LogError(e.Message);
			return "";
		}
	}
}