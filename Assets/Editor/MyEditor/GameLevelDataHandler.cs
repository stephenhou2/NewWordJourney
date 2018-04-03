using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	using System;
	using System.IO;
	using UnityEditor;

	public class GameLevelDataHandler{


		[MenuItem("EditHelper/GameLevelDatasHelper")]
		public static void GameLevelDatasHelper(){
			GameLevelDataHandler gldh = new GameLevelDataHandler ();
			gldh.LoadGameDatas ();
			gldh.SaveGameDatas ();
		}

		public List<HLHGameLevelData> gameLevelDatas = new List<HLHGameLevelData>();

		public void LoadGameDatas(){
			
			string gameDataSource = DataHandler.LoadDataString ("/Users/houlianghong/Desktop/MyGameData/关卡原始数据.csv");

			string[] gameLevelDataStrings = gameDataSource.Split (new string[]{ "\n" },System.StringSplitOptions.RemoveEmptyEntries);

			for(int i = 1;i<gameLevelDataStrings.Length;i++){
				HLHGameLevelData gameLevelData = new HLHGameLevelData (gameLevelDataStrings [i],i-1);
				gameLevelDatas.Add (gameLevelData);
			}
		}

		public void SaveGameDatas(){

			string gameLevelDatasJson = JsonHelper.ToJson<HLHGameLevelData> (gameLevelDatas.ToArray ());

			File.WriteAllText (CommonData.originDataPath + "/GameLevelDatas.json",gameLevelDatasJson);

		}

	}
		
}
