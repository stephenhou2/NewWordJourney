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
				HLHGameLevelData gameLevelData = InitGameLevelDatas (gameLevelDataStrings [i]);
				gameLevelDatas.Add (gameLevelData);
			}
		}

		public HLHGameLevelData InitGameLevelDatas(string dataString){

			dataString = dataString.Replace ("\r", "");

			string[] dataStrings = dataString.Split (new char[]{ ','},System.StringSplitOptions.None);

			int gameLevelIndex = int.Parse (dataStrings [0]);

			List<int> itemIdsInPot = InitIntArrayWithString (dataStrings [1]);

			List<int> itemIdsInBucket = InitIntArrayWithString (dataStrings [2]);

			List<int> itemIdsInTreasureBox = InitIntArrayWithString (dataStrings [3]);

			List<int> monsterIds = InitIntArrayWithString (dataStrings [4]);

			int bossId = int.Parse (dataStrings [5]);

			HLHGameLevelData levelData = new HLHGameLevelData (gameLevelIndex, itemIdsInPot, itemIdsInBucket, itemIdsInTreasureBox, monsterIds,bossId);

			return levelData;

		}

		private List<int> InitIntArrayWithString(string dataString){
			string[] idStrings = dataString.Split (new char[]{ '_' }, System.StringSplitOptions.RemoveEmptyEntries);
			List<int> idList = new List<int> ();
			for (int i = 0; i < idStrings.Length; i++) {
				string idAndCount = idStrings [i];
				string[] data = idAndCount.Split (new char[]{ '^' }, System.StringSplitOptions.None);
				int id = int.Parse (data [0]);
				int count = int.Parse (data [1]);
				for (int j = 0; j < count; j++) {
					idList.Add (id);
				}
			}
			return idList;
		}


		public void SaveGameDatas(){

			string gameLevelDatasJson = JsonHelper.ToJson<HLHGameLevelData> (gameLevelDatas.ToArray ());

			File.WriteAllText (CommonData.originDataPath + "/GameLevelDatas.json",gameLevelDatasJson);

		}

	}
		
}
