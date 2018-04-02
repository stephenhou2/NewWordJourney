using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{

	using System;
	using System.IO;

	public class GameLevelDataHandler{

		public List<MyGameLevelData> gameLevelDatas = new List<MyGameLevelData>();

		// 商人处卖的商品信息组
//		private List<GoodsGroup> allGoodsGroup = new List<GoodsGroup>();

		public void LoadGameDatas(){
			
			string gameDataSource = DataHandler.LoadDataString ("/Users/houlianghong/Desktop/MyGameData/GameLevelData.csv");

			string[] gameLevelDataStrings = gameDataSource.Split (new string[]{ "\n" },System.StringSplitOptions.RemoveEmptyEntries);

			for(int i = 1;i<gameLevelDataStrings.Length;i++){
//				MyGameLevelData gameLevelData = new MyGameLevelData (gameLevelDataStrings [i],i-1,allGoodsGroup);
//				gameLevelDatas.Add (gameLevelData);
			}

			SaveGoodsInfoOfTrader ();

		}

		private void SaveGoodsInfoOfTrader(){

//			string traderDataPath = Path.Combine (CommonData.originDataPath, "NPCs/Trader_TraderMan.json");
//
//			Trader trader = DataHandler.LoadDataToSingleModelWithPath<Trader> (traderDataPath);
//
//			trader.goodsGroupList = allGoodsGroup;
//
//			DataHandler.SaveInstanceDataToFile <Trader> (trader, traderDataPath);

		}

		public void SaveGameDatas(){

			string gameLevelDatasJson = JsonHelper.ToJson<MyGameLevelData> (gameLevelDatas.ToArray ());

			Debug.Log (gameLevelDatasJson);

			File.WriteAllText (CommonData.originDataPath + "/GameLevelDatas.json",gameLevelDatasJson);

		}

	}

	[System.Serializable]
	public class MyGameLevelDatas{
		public List<MyGameLevelData> Items = new List<MyGameLevelData> ();
	}

	[System.Serializable]
	public class MyGameLevelData{

		// 关卡序号
		public int gameLevelIndex;

		// 所在章节名称(5关一个章节)
		public string chapterName;

		// 关卡中的所有怪物信息
		public MonsterInfo[] monsterInfos;

		// 瓦罐中一定能开出来的物品id数组
		public int[] mustAppearItemIdsInUnlockedBox;

		// 瓦罐中可能会开出来的物品id数组
		public int[] possiblyAppearItemIdsInUnlockedBox;

		// 宝箱中可能会开出来的物品id数组
		public int[] possiblyAppearItemIdsInLockedBox;



		// 关卡中怪物相对与prefab的提升比例
		public float monsterScaler;

		// 关卡中boss的id（-1代表本关不出现boss）
		public int bossId;


		public MyGameLevelData(string dataString,int currenLevel){

			dataString = dataString.Replace ("\r", "");

			string[] dataStrings = dataString.Split (new char[]{ ','},System.StringSplitOptions.None);

			gameLevelIndex = Convert.ToInt16 (dataStrings [0]);
			chapterName = dataStrings [1];

			monsterInfos = InitMonsterInfoWith (dataStrings [2], dataStrings [3]);

			mustAppearItemIdsInUnlockedBox = InitIntArrayWithString (dataStrings [4]);

			possiblyAppearItemIdsInUnlockedBox = InitIntArrayWithString (dataStrings [5]);

			possiblyAppearItemIdsInLockedBox = InitIntArrayWithString (dataStrings [6]);

//			GoodsGroup goodsGroups = InitGoodsGroupArrayWith (currenLevel,dataStrings [7], dataStrings [8], dataStrings [9], dataStrings [10], dataStrings [11]);

//			allGoodsGroup.Add (goodsGroups);

			monsterScaler = Convert.ToSingle (dataStrings [12]);

			bossId = FromStringToInt (dataStrings [13]);

		}



		private MonsterInfo[] InitMonsterInfoWith(string monsterIdsString,string monsterCountsString){
			
			int[] monsterIds = InitIntArrayWithString (monsterIdsString);
			int[] monsterCounts = InitIntArrayWithString (monsterCountsString);

			monsterInfos = new MonsterInfo[monsterIds.Length];
			for (int i = 0; i < monsterInfos.Length; i++) {
				monsterInfos [i] = new MonsterInfo (monsterIds [i], monsterCounts [i]);
			}

			return monsterInfos;

		}

//		private GoodsGroup InitGoodsGroupArrayWith(int currentLevel,params string[] goodsStrings){
//
//			List<Goods> goodsList = new List<Goods> ();
//
//			for (int i = 0; i < goodsStrings.Length; i++) {
//
//				string goodsString = goodsStrings [i];
//
//				Goods goods = new Goods (InitIntArrayWithString (goodsString));
//
//				goodsList.Add (goods);
//			}
//
//			GoodsGroup gg = new GoodsGroup (goodsList, currentLevel);
//
//			return gg;
//		}

		private bool[] InitBoolArrayWithString(string dataString){
			string[] boolStrings = dataString.Split (new char[]{ '_' }, System.StringSplitOptions.RemoveEmptyEntries);
			bool[] boolArray = new bool[boolStrings.Length];
			for (int i = 0; i < boolStrings.Length; i++) {
				boolArray [i] = Convert.ToInt16(boolStrings[i]) == 0 ? false : true;
			}
			return boolArray;
		}

		private int[] InitIntArrayWithString(string dataString){
			if (dataString == "") {
				return null;
			}
			string[] idStrings = dataString.Split (new char[]{ '_' }, System.StringSplitOptions.RemoveEmptyEntries);
			int[] idArray = new int[idStrings.Length];
			for (int i = 0; i < idStrings.Length; i++) {
				idArray [i] = Convert.ToInt16(idStrings[i]);
			}
			return idArray;
		}

		private int FromStringToInt(string str){
			
			if (str == "") {
				return -1;
			}

			return Convert.ToInt16 (str);
		}

//		private Count InitCountWithString(string dataString){
//			string[] countStrings = dataString.Split (new char[]{ '_' }, System.StringSplitOptions.RemoveEmptyEntries);
//			return new Count (Convert.ToInt16(countStrings [0]), Convert.ToInt16(countStrings [1]));
//		}

	}

}
