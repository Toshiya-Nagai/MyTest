using UnityEngine;
using System;
using System.Collections;

using Mono.Data.Sqlite;

public class KeyValueModel {
	public string key;
	public string value;
	
	public KeyValueModel() {
	}
	
	public KeyValueModel(string key, string val) {
		this.key = key;
		this.value = val;
	}
}

// Synchronous access to data models.
// This is private. The KeyValueStorage class is the primary interface.
public class KeyValueAccess : AccessBase {
	
	private const string KEY_VALUE_SCHEMA = "CREATE TABLE keyValue (" +
		"key       TEXT PRIMARY KEY," +
			"value     TEXT NOT NULL)";
	
	private const string KEY_VALUE_SELECT_SQL = "SELECT key, value FROM keyValue";
	private KeyValueModel ReadKeyValueDataMode(SqliteDataReader reader) {
		KeyValueModel m = new KeyValueModel();
		int i = 0;
		
		m.key = reader.GetString(i++);
		m.value = reader.GetString(i++);
		
		return m;
	}
	
	public Error Create() {
		try {
			using(SqliteCommand cmd = connection.CreateCommand()) {
				cmd.CommandText = KEY_VALUE_SCHEMA;
				cmd.ExecuteNonQuery();
			}
			return null;
		} catch (Exception e) {
			return Error.Exception(e);
		}
	}
	
	public Error GetSingle(string key, out KeyValueModel m) {
		m = null;
		try {
			if(!isConnected){ 
				Debug.LogError ("Is Not Connected");
				return Error.NotConnected();
			}
			
			KeyValueModel internalM = null;
			OpenReader(KEY_VALUE_SELECT_SQL + " where key='" + key + "'", delegate(SqliteDataReader reader) {
				if (reader.Read()) {
					// Fill out the data model.
					internalM = ReadKeyValueDataMode(reader);
				}
			});
			m = internalM;
			return null;
			
		} catch(Exception e) {
			return new Error(Error.Code.Exception, e.ToString());
		}
	}
	
	public Error GetAll(out IList list) {
		list = null;
		try {
			if(!isConnected) return Error.NotConnected();
			
			IList listInternal = null;
			OpenReader(KEY_VALUE_SELECT_SQL + " order by key asc", delegate(SqliteDataReader reader) {
				listInternal = new ArrayList();
				while(reader.Read()) {
					listInternal.Add(ReadKeyValueDataMode(reader));
				}
			});
			list = listInternal;
			return null;
		} catch(Exception e) {
			return new Error(Error.Code.Exception, e.ToString());
		}
	}
	
	public Error InsertOrUpdate(KeyValueModel m) {
		try {
			if(!isConnected){ return Error.NotConnected();}
			SqliteParameter v;
			using (SqliteCommand cmd = connection.CreateCommand()) {
				//firts try an update
				cmd.CommandText = "UPDATE keyValue SET value=? WHERE key=?";
				
				v = cmd.CreateParameter();
				v.Value = m.value;
				cmd.Parameters.Add(v);
				
				v = cmd.CreateParameter();
				v.Value = m.key;
				cmd.Parameters.Add(v);

				Debug.Log("update");

				//if no columns affected, then insert
				if (cmd.ExecuteNonQuery() == 0) {
					Debug.Log("Insert");
					cmd.CommandText = "INSERT INTO keyValue (value,key) VALUES(?,?)";
					cmd.ExecuteNonQuery();
				}

				Debug.Log(cmd.CommandText);
			}
			
			return null;
		} catch(Exception e) {
			return new Error(Error.Code.Exception, e.ToString());
		}
	}
	
	public Error Delete(string key) {
		try {
			if(!isConnected) return Error.NotConnected();
			SqliteParameter v;
			using (SqliteCommand cmd = connection.CreateCommand()) {
				cmd.CommandText = "DELETE FROM keyValue WHERE key=?";
				
				v = cmd.CreateParameter();
				v.Value = key;
				cmd.Parameters.Add(v);
				cmd.ExecuteNonQuery();
			}
			
			return null;
		} catch(Exception e) {
			return new Error(Error.Code.Exception, e.ToString());
		}
	}
	
}
