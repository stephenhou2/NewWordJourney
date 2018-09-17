using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	[System.Serializable]
	public class HLHGameLevelData{

		// 关卡序号
		public int gameLevelIndex;

		// 瓦罐中能开出来的物品id数组
		public List<int> itemIdsInPot = new List<int>();

		// 木桶中能开出来的物品id数组
		public List<int> itemIdsInBucket = new List<int>();

		// 普通宝箱中可能会开出来的物品id数组
		public List<int> itemIdsInNormalTreasureBox = new List<int>();
      
		// 怪物id列表[这个列表中一个id就代表一个怪物]
		public List<int> monsterIds = new List<int>();

		public int bossId;

		// 怪物id列表【这个列表中只存放本层所有怪物的id(包括boss)信息，不重复】
		public List<int> monsterIdsOfCurrentLevel = new List<int>();

		public Count goldAmountRange;

        public static bool HasWiseMan(){

            return Player.mainPlayer.currentLevelIndex == 0 
				         || Player.mainPlayer.currentLevelIndex == 2
				         || Player.mainPlayer.currentLevelIndex == 4
				         || Player.mainPlayer.currentLevelIndex == 6
                         || Player.mainPlayer.currentLevelIndex == 9
				         || Player.mainPlayer.currentLevelIndex == 12
				         || Player.mainPlayer.currentLevelIndex == 14
				         || Player.mainPlayer.currentLevelIndex == 17
				         || Player.mainPlayer.currentLevelIndex == 19
				         || Player.mainPlayer.currentLevelIndex == 21
                         || Player.mainPlayer.currentLevelIndex == 24
				         || Player.mainPlayer.currentLevelIndex == 26
                         || Player.mainPlayer.currentLevelIndex == 29
				         || Player.mainPlayer.currentLevelIndex == 31
				         || Player.mainPlayer.currentLevelIndex == 34
				         || Player.mainPlayer.currentLevelIndex == 37
				         || Player.mainPlayer.currentLevelIndex == 39
				         || Player.mainPlayer.currentLevelIndex == 42
				         || Player.mainPlayer.currentLevelIndex == 44
				         || Player.mainPlayer.currentLevelIndex == 47
                         || Player.mainPlayer.currentLevelIndex == 49
				         || Player.mainPlayer.currentLevelIndex == 50;

        }

        public static bool IsBossLevel(){
            return (Player.mainPlayer.currentLevelIndex + 1) % 5 == 0;
        }

		public static bool HasDiaryPaper(){
			return Player.mainPlayer.currentLevelIndex == 0 ||
						 ((Player.mainPlayer.currentLevelIndex + 1) % 5 == 0 && Player.mainPlayer.currentLevelIndex < 45);
		}

		public HLHGameLevelData(int gameLevelIndex, List<int> itemIdsInPot,List<int> itemIdsInBucket,List<int> itemIdsInNormalTreasureBox,
		                        List<int> monsterIds,int bossId,Count goldAmountRange,List<int>monsterIdsOfCurrentLevel){
			this.gameLevelIndex = gameLevelIndex;
			this.itemIdsInPot = itemIdsInPot;
			this.itemIdsInBucket = itemIdsInBucket;
			this.itemIdsInNormalTreasureBox = itemIdsInNormalTreasureBox;
			//this.itemIdsInGoldTreasureBox = itemIdsInGoldTreasureBox;
			this.monsterIds = monsterIds;
			this.bossId = bossId;
			this.goldAmountRange = goldAmountRange;
			this.monsterIdsOfCurrentLevel = monsterIdsOfCurrentLevel;
		}


	
	}
}
