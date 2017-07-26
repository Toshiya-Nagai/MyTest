using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;

//using CSharpQueue;

public class DataModelAccess : AccessBase{

	private const string CONFIG_SELECT_SQL = "SELECT KEY, VALUE FROM CONFIG";
	
	public Error Connect(string path, out ConnectDataModel m) {
		m = null;
		try {
			Error err = Connect(path);
			if (err != null) {
				return err;
			}
			
			// Retreive the version number.
			string version = null;
			string assetUrl = null;
			
			OpenReader(CONFIG_SELECT_SQL, delegate(SqliteDataReader reader) {
				
				while(reader.Read()) {
					int i=0;
					string key = reader.GetString(i++);
					string val = reader.GetString(i++);	// the nature of the table means all are strings.
					switch(key) {
					case "version":
						version = val;
						break;
					case "asset_url":
						assetUrl = val;
						break;
						
						// Do *not* throw an exception here. Otherwise, we can't ever add new features
						// without a force update being required. This won't be useful until two releases
						// after Raid Boss, since that's when this was removed.
					default:
						Log.Info("Unknown key in CONFIG table: " + key);
						break;
					}
				}
			});
			
			if(version == null || assetUrl == null) {
				throw new Exception("Could not read required fields out of the config table");
			}
			
			// Fill out the data model.
			m = new ConnectDataModel();
//			m.hash = HashUtility.MD5(path);
			m.version = int.Parse(version);
			m.assetUrl = assetUrl;
			
			if (m.version < AppConfig.minDataModelVersion) {
				return Error.OldVersion();
			}

			Log.Info("Using database version "+m.version);
			Log.Info("Asset downloads will be from "+m.assetUrl);
			Error error = CacheRequiredTables();
//			Error error = null;
			if (error != null) {
				return error;
			}
			
			return null;
		} catch(Exception e) {
			connection.Close();
			connection = null;
			return Error.Exception(e);
		}
	}

	Error CacheRequiredTables(){
		Error error;
		Log.Info("Caching SkillDataModel");
		List<SkillDataModel> skillDM;
		error = GetSkill(out skillDM);
		if(error != null){return error;}
		CacheManager.skillDM = skillDM;
		Log.Info ("Caching CharaDataModel");
		List<CharaDataModel> charaDM;
		error = GetChara(out charaDM);
		if(error != null){return error;}
		CacheManager.charaDM = charaDM;
		List<CpuModel> cpuDM;
		error = GetCpuLevel(out cpuDM);
		if(error != null){return error;}
		CacheManager.cpuDM = cpuDM;
		return null;
	}


	private const string LEVEL_CURVE_SQL = "select level,score,speed from level_curve";
	public Error GetLevelCurve(out List<LevelModel> levels){
		string query = LEVEL_CURVE_SQL;

		return GetList<LevelModel>(query,out levels,(reader)=>{
			LevelModel m = new LevelModel();
			m.level = reader.GetInt32(0);
			m.rate = reader.GetInt32(1);
			m.speed = reader.GetInt32(2);
			return m;
		});
	}

	private const string CPU_LEVEL_SQL = "select * from cpu_level";
	public Error GetCpuLevel(out List<CpuModel> cpuList){
		string query = CPU_LEVEL_SQL;
		return GetList<CpuModel>(query,out cpuList,(reader)=>{
			CpuModel m = new CpuModel();
			m.id = reader.GetInt32(0);
			m.level = reader.GetInt32(1);
			m.raiseTime = reader.GetFloat(2);
			m.regulateInterval = reader.GetFloat(3);
			m.changeInterval = reader.GetFloat (4);
			m.orderInterval = reader.GetFloat (5);
			return m;
		});
	}

	private const string SKILL_SQL = "select id,name,comment,target_id from skill"; 
	public Error GetSkill(out List<SkillDataModel> skillList){
		string query = SKILL_SQL;
		return GetList<SkillDataModel>(query,out skillList,(reader)=>{
			SkillDataModel m = new SkillDataModel();
			m.id = reader.GetInt32(0);
			m.name = reader.GetString (1);
			m.comment = reader.GetString(2);
			m.targetId = reader.GetInt32(3);
			return m;
		});
	}

	private const string CHARA_SQL = "select id,name,comment,skillId,sprite_name from character";
	public Error GetChara(out List<CharaDataModel> charaList){
		string query = CHARA_SQL;
		return GetList<CharaDataModel>(query,out charaList,(reader)=>{
			CharaDataModel m = new CharaDataModel();
			m.id = reader.GetInt32(0);
			m.name = reader.GetString (1);
			m.comment = reader.GetString(2);
			m.skillId = reader.GetInt32(3);
			m.spriteName = reader.GetString(4);
			return m;
		});
	}

	private const string RANKING_SQL = "select rank,name,score from ranking";
	public Error GetRanking(out List<RankingDataModel> ranking){
		string query = RANKING_SQL;
		
		return GetList<RankingDataModel>(query,out ranking,(reader)=>{
			RankingDataModel m = new RankingDataModel();
			m.rank = reader.GetInt32(0);
			m.name = reader.GetString(1);
			m.score = reader.GetInt32(2);
			return m;
		});
	}

	private const string RANKING_UPDATE_SQL = "update ranking set score = ?,name = ? where rank = ?";
	Error updateRanking(RankingDataModel ranking) {
		try {
			if(!isConnected){ return Error.NotConnected();}
			SqliteParameter v;
			using (SqliteCommand cmd = connection.CreateCommand()) {
				//firts try an update
				cmd.CommandText = RANKING_UPDATE_SQL;
				
				v = cmd.CreateParameter();
				v.Value = ranking.score;
				cmd.Parameters.Add(v);
				
				v = cmd.CreateParameter();
				v.Value = ranking.name;
				cmd.Parameters.Add(v);

				v = cmd.CreateParameter();
				v.Value = ranking.rank;
				cmd.Parameters.Add(v);

				//update
				cmd.ExecuteNonQuery();
			}
			
			return null;
		} catch(Exception e) {
			return new Error(Error.Code.Exception, e.ToString());
		}
	}

	public Error UpdateRankings(List<RankingDataModel> ranking){
		Error e = null;
		foreach(var buf in ranking){
			e = updateRanking(buf);
		}
		return e;
	}

}
