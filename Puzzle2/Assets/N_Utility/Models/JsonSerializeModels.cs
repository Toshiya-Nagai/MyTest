using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using LitJson2;

/// <summary>
/// json serialize class
/// call when you serialize to form the specified Json
/// SampleUser user = User_Serialize.GetUser<SampleUser>(Json);
/// </summary>

#region Sample
/*
static public class User_Serialize{
	static public T GetUser<T>(JsonData anJson)where T : SampleUser,new(){
		T user = new T();
		user.id = JsonSerialize.Get<int>(anJson,"id");
		user.nickname = JsonSerialize.Get<string>(anJson,"name");
		return user;
	}
}
static public class SampleSerialize{
	static public T GetSample<T>(JsonData anJson)where T : SampleClass,new(){
		HoldJsonSerialize serialize = new HoldJsonSerialize(anJson);
		T sample = new T();
		sample.point = serialize.Get<int>("count");
		sample.rarity = serialize.Get<int>("rank");
		return sample;
	}
}
*/
#endregion


/// <summary>
/// Json serialize.
/// return the value of the default key or value Json does not return an error be null
/// </summary>
static public class JsonSerialize{
	static public T Get<T>(JsonData anJson,string anKey)where T : IConvertible{
		if(anJson == null){
			Debug.LogError("json is Null!! return default value");
			return default(T);
		}
		if(anJson.KeyExists(anKey)){
			if(anJson[anKey] == null){	
				Debug.LogError("json["+anKey+"] is Null!! return default value");
				return default(T);
			}
			string valueStr = anJson[anKey].ToString();
			T retValue = ConvertValue<T>(valueStr);
			return (T)retValue;
		}else{
			Debug.LogError("json["+anKey+"] is key not found!! return default value");
		}
		return default(T);
	}

	public static T ConvertValue<T>(string value)where T : IConvertible{
		IConvertible retValue = default(T);
		try{
			if(typeof(T) == typeof(long)){
				retValue = Convert.ToUInt32(value);
			}else if(typeof(T) == typeof(float) || typeof(T) == typeof(double)){
				retValue = Convert.ToDouble(value);
			}else if(typeof(T) == typeof(int)){
				retValue = Convert.ToInt32(value);
			}else if(typeof(T) == typeof(bool)){
				retValue = Convert.ToBoolean(value);
			}else if(typeof(T) == typeof(string)){
				retValue = value;
			}else{
				Debug.LogError("Not Serialize Support Type = " + typeof(T));
			}
		}catch(FormatException){
			Debug.LogError("Not Exist Type To Value -> " + value + " To " + typeof(T));
		}
		return (T)retValue;
	}

	public static T GetConvert<T>(JsonData json)where T : class,new(){
		try{
			var data = JsonMapper.ToObject<T>(json.ToJson());
			return data;
		}catch(JsonException e){
			Debug.LogError(e);
			return new T();
		}
	}
}

/// <summary>
/// Hold json serialize.
/// from json that holds, it returns the value given the key.
/// </summary>
public class HoldJsonSerialize{
	private JsonData _holdJson;
	public JsonData HoldJson{
		private get{
			if(_holdJson == null){
				Debug.LogError("_holdJson is Null");
				return null;
			}else{
				return _holdJson;
			}
		}
		set{_holdJson = value;}
	}
	public HoldJsonSerialize(JsonData anHoldJson){
		this.HoldJson = anHoldJson;
	}
	public T Get<T>(string anKey)where T : IConvertible{
		return JsonSerialize.Get<T>(HoldJson,anKey);
	}
}


