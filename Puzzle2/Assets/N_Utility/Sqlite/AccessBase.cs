using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;

public class AccessBase{
	protected SqliteConnection connection;
	public bool isConnected{
		get{return (connection != null);}
	}
	
	public Error Connect(string path) {
		try {
			if (connection != null) {
				connection.Close();
			}
			// Open the DB.
			string connectionString = "URI=file:" + path;
			Log.Info("Connection string: " + connectionString);
			connection = new SqliteConnection(connectionString);
			connection.Open();
			
			return null;
		} catch(Exception e) {
			connection = null;
			return Error.Exception(e);
		}
	}
	
	protected void OpenReader(string query,System.Action<SqliteDataReader> read){
		using(var command = connection.CreateCommand()){
			command.CommandText = query;
			using(SqliteDataReader reader = command.ExecuteReader()) {
				read(reader);
			}
		}
	}
	
	protected Error GetSingle<TModel>(string query,out TModel model,Func<SqliteDataReader,TModel> getFunc) where TModel : class{
		model = null;
		if(!isConnected){return Error.NotConnected();}
		try{
			TModel checkModel = null;
			OpenReader(query,(reader)=>{
				if(reader.Read()){
					checkModel = getFunc(reader);
				}
			});
			if(checkModel == null){
				return Error.EmptyResult();
			}
			model = checkModel;
			return null;
		}catch(Exception e){
			return new Error(Error.Code.Exception, e.ToString());
		}
	}
	
	protected Error GetList<TModel>(string query,out List<TModel> modelList,Func<SqliteDataReader,TModel> getFunc)where TModel : class{
		modelList = null;
		if(!isConnected){return Error.NotConnected();}
		try{
			List<TModel> checkModel = null;
			OpenReader(query,(reader)=>{
				checkModel = new List<TModel>();
				while(reader.Read()){
					checkModel.Add(getFunc(reader));
				}
			});
			modelList = checkModel;
			return null;
		}catch(Exception e){
			return new Error(Error.Code.Exception, e.ToString());
		}
	}
	
	public void Disconnect() {
		if (connection != null) {
			connection.Close();
			connection = null;
		}
	}
	
	public class Error {
		public enum Code {
			None,
			Unknown,
			NotConnected,
			Exception,
			EmptyResult,
			OldVersion
		}
		public Code code;
		public string description;
		
		public Error(Code code, string description) {
			this.code = code;
			this.description = description;
		}
		
		public static Error NotConnected() {
			return new Error(Code.NotConnected, "Not connected to database");
		}
		
		public static Error Exception(Exception e) {
			return new Error(Code.Exception, e.ToString());
		}
		
		public static Error EmptyResult() {
			return new Error(Code.EmptyResult, "Empty result set");
		}
		
		public static Error OldVersion() {
			return new Error(Code.OldVersion, "The current DataModel db is too old for this client");
		}
	}
}