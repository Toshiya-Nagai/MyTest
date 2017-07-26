using UnityEngine;

using System;
using System.IO;
using System.Collections;
using System.ComponentModel;

using Mono.Data.Sqlite;
using LitJson2;

/*
	Access to the local device's read/write database. 
	This is where all local user preferences and current device configurations are stored.
	For example, every downloaded asset has an entry in this database to get the "real" local filename
	used after downloading.
	The "base" version of this is downloaded with the app, and then amended.
	
	The read-only database of game controlling values is available from DataModelQueue.
*/
public class KeyValueStorage {
	
	public const string KEY_VALUE_DB = "keyValue.db";
	
	private static System.Collections.Generic.Dictionary<Storage,KeyValueStorage> storages = new System.Collections.Generic.Dictionary<Storage,KeyValueStorage>(4);
	
	private static KeyValueAccess db;
	private static string dbPath;
	
	public static void Init () {
		Log.Info("KeyValueStorage.Init");
		long ini = System.DateTime.UtcNow.Ticks/10000;
		db = new KeyValueAccess();
		KeyValueAccess.Error err;
		
		dbPath = Path.Combine(Application.persistentDataPath,KEY_VALUE_DB);
		Log.Info ("KVS: " + dbPath);

		err = db.Connect(dbPath);
		if (err != null) {
			//pretty bad error, just throw a grenade :/
			throw new Exception(err.description);
		}
		
		long end = System.DateTime.UtcNow.Ticks/10000;
		Log.Info("KeyValueStorage.Init took: " + (end-ini));
	}
	
	public enum Storage {
		ASSET_BUNDLE,
		USER_PROFILE,
		PREFERENCES,
		TUTORIAL,
		DATA_MODEL_FILE,
		FRIEND_INVITE,
	}
	
	public static KeyValueStorage Instance(Storage storage) {
//		Assert.NotNull(db,"Instance should be called after Init");
		lock (storages) {
			KeyValueStorage kvs;
			if (!storages.TryGetValue(storage, out kvs)) {
				storages[storage] = kvs = new KeyValueStorage(storage);
			}
			return kvs;
		}
	}
	
	private Storage storage;
	
	private KeyValueStorage(Storage storage) {
		this.storage = storage;
	}


	private class Value<T> {
		private T _value;
		private string _data;
		
		private Value() {
		}
		
		private Value(string data) {
			_data = data;
//			_value = LitJson2.JsonMapper.ToObject<T>(_data);
			if(typeof(T).IsValueType){
				_value = N_Json.ToObject<Container<T>>(_data).GetValue();
			}else{
				_value = N_Json.ToObject<T>(_data);
			}
		}
		
		public static Value<T> FromRaw(string data) {
			return new Value<T>(data);
		}
		
		public static Value<T> FromValue(T val) {
			return new Value<T>().Set(val);
		}
		
		public T Get() {
			return (T)_value;
		}
		
		public Value<T> Set(T val) {
			_value = val;
//			_data = LitJson2.JsonMapper.ToJson(val);
			if(val.GetType ().IsValueType){
				_data = N_Json.ToJson(new Container<T>(val));
			}else{
				_data = N_Json.ToJson(val);
			}
			return this;
		}
		
		public string GetRaw() {
			return _data;
		}
		
	}
	
	private string CreateStorageKey(string key) {
		return storage.ToString() + "_" + key;
	}
	
	public T GetValue<T>(string key) {
		KeyValueModel m;
		KeyValueAccess.Error err;
		lock(db) {
			err = db.GetSingle(CreateStorageKey(key), out m);
		}
		if (err != null) {
			Log.Error("Error getting value for key: {0}, Error: {1}",key,err);
			//what should be returned?
			return default (T);
		}
		
		if (m != null) {
			Type type = typeof(T);
			#region optimizations 
			if (type == typeof(LitJson2.JsonData)) {
				//casting to object is required to avoid a compilation error
				return (T)(object)LitJson2.JsonMapper.ToObject(m.value);
			}
			
			
			if (type == typeof(string)) {
				//casting to object is required to avoid a compilation error
				return (T)(object)m.value;
			}
			#endregion
			
			if (typeof(IList).IsAssignableFrom(type)) {
				if (type.IsArray) {
					return Value<T>.FromRaw(m.value).Get();
				} else {
					IList array = Value<object[]>.FromRaw(m.value).Get();
					return (T)array;
				}
			}
			return Value<T>.FromRaw(m.value).Get();
		} else {
			return default (T);
		}
	}
	
	public void SetValueAsync<T>(string key, T val) {
		WorkQueue.Do(delegate() {
			SetValue<T>(key, val);
		});
	}
	
	public void SetValueAsync<T>(string key, T val, WorkQueue.Callback doneCallback) {
		WorkQueue.Do(delegate() {
			SetValue<T>(key, val);
			return null;
		}, doneCallback);
	}
	
	public void SetValue<T>(string key, T val) {
		Type type = typeof(T);
		string raw;
		if (type == typeof(Double)) {
//			throw new Exception("Double is not supported by this storage");
			raw = Value<T>.FromValue(val).GetRaw();
		} else if (type == typeof(string)) {
			raw = (string)(object)val;
		} else {
			raw = Value<T>.FromValue(val).GetRaw();
		}
		KeyValueAccess.Error err;
		lock(db) {
			err = db.InsertOrUpdate(new KeyValueModel(CreateStorageKey(key),raw));
		}
		if (err != null) {
			//TODO: Handle Error
			throw new Exception(err.description);
		}
	}
	
	public KeyValueAccess.Error Remove(string key) {
		KeyValueAccess.Error err;
		lock(db) {
			err = db.Delete(CreateStorageKey(key));
		}
		return err;
	}
	
	public static void DeleteDBFile() {
		lock (db) {
			db.Disconnect();
			try {
				File.Delete(dbPath);
			} catch {}
		}
	}

	public static void DisConnect(){
		lock(db){
			if(db != null){
				db.Disconnect();
			}
		}
	}

	//JsonWrapper
	private class Container<T>{
		public T value;
		public Container(){}
		public Container(T val){
			value = val;
		}
		public T GetValue(){
			return value;
		}
	}
}


public class N_Json{
	static N_Json(){
		JsonMapper.RegisterExporter<float>( (obj, writer) => { writer.Write( System.Convert.ToDouble( obj ) ); } );
		JsonMapper.RegisterImporter<double,float>( (input) => { return System.Convert.ToSingle( input ); } );
		JsonMapper.RegisterImporter<System.Int32,long>( (input) => { return System.Convert.ToInt64( input ); } );
	}
	
	public static T ToObject<T>( string json ){
		return JsonMapper.ToObject<T>( json );
	}
	
	public static string ToJson( object obj ){
		return JsonMapper.ToJson ( obj );
	}
}
