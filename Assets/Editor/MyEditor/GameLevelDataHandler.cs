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
				Debug.LogFormat ("第{0}层", i-1);
				HLHGameLevelData gameLevelData = InitGameLevelDatas (gameLevelDataStrings [i]);
				gameLevelDatas.Add (gameLevelData);
			}
		}

		public HLHGameLevelData InitGameLevelDatas(string dataString){

			dataString = dataString.Replace ("\r", "");

			string[] dataStrings = dataString.Split (new char[]{ ','},System.StringSplitOptions.None);

			int gameLevelIndex = int.Parse (dataStrings [0]);

			Debug.LogFormat ("{0}####瓦罐", dataStrings [1]);
			List<int> itemIdsInPot = InitIntAndCountArrayWithString (dataStrings [1]);

			Debug.LogFormat ("{0}####木桶", dataStrings [2]);
			List<int> itemIdsInBucket = InitIntAndCountArrayWithString (dataStrings [2]);

			Debug.LogFormat ("{0}####普通宝箱", dataStrings [3]);
			List<int> itemIdsInNormalTreasureBox = InitIntAndCountArrayWithString (dataStrings [3]);

			//Debug.LogFormat("{0}####金色宝箱", dataStrings[4]);
            //List<int> itemIdsInGoldTreasureBox = InitIntArrayWithString(dataStrings[4]);

			Debug.LogFormat ("{0}####怪物", dataStrings [4]);
			List<int> monsterIds = InitIntAndCountArrayWithString (dataStrings [4]);

			int bossId = int.Parse (dataStrings [5]);

			Debug.LogFormat ("{0}####钱袋", dataStrings [6]);
			Count goldAmountRange = InitCountWithString (dataStrings [6]);

			Debug.LogFormat("{0}####本层怪物id", dataStrings[7]);
			List<int> monsterIdsOfCurrentLevel = InitIntArrayWithString(dataStrings[7]);

			HLHGameLevelData levelData = new HLHGameLevelData (gameLevelIndex, itemIdsInPot, itemIdsInBucket, itemIdsInNormalTreasureBox, monsterIds,bossId,goldAmountRange,monsterIdsOfCurrentLevel);

			return levelData;

		}

		private List<int> InitIntAndCountArrayWithString(string dataString){
			
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

		private List<int> InitIntArrayWithString(string dataString)
        {

            string[] idStrings = dataString.Split(new char[] { '_' }, System.StringSplitOptions.RemoveEmptyEntries);
            List<int> idList = new List<int>();
            for (int i = 0; i < idStrings.Length; i++)
            {
				int id = int.Parse(idStrings[i]);

                idList.Add(id);

            }
            return idList;
        }

		private Count InitCountWithString(string dataString){

			string[] countStrings = dataString.Split (new char[]{ '_' }, System.StringSplitOptions.RemoveEmptyEntries);

			int min = int.Parse (countStrings [0]);

			int max = int.Parse (countStrings [1]);

			return new Count (min, max);

		}


		public void SaveGameDatas(){

			string gameLevelDatasJson = JsonHelper.ToJson<HLHGameLevelData> (gameLevelDatas.ToArray ());

			File.WriteAllText (CommonData.originDataPath + "/GameLevelDatas.json",gameLevelDatasJson);

		}

	}
		
}
