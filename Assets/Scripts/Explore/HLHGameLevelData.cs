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

		// 宝箱中可能会开出来的物品id数组
		public List<int> itemIdsInTreasureBox = new List<int>();

		// 怪物id列表
		public List<int> monsterIds = new List<int>();

		public int bossId;

		public Count goldAmountRange;

        public static bool HasWiseMan(){

            return Player.mainPlayer.currentLevelIndex == 0 
                         || Player.mainPlayer.currentLevelIndex == 9
                         || Player.mainPlayer.currentLevelIndex == 19
                         || Player.mainPlayer.currentLevelIndex == 39
                         || Player.mainPlayer.currentLevelIndex == 49;

        }

        public static bool IsBossLevel(){
            return (Player.mainPlayer.currentLevelIndex + 1) % 10 == 0;
        }

		public HLHGameLevelData(int gameLevelIndex, List<int> itemIdsInPot,List<int> itemIdsInBucket,List<int> itemIdsInTreasureBox,List<int> monsterIds,int bossId,Count goldAmountRange){
			this.gameLevelIndex = gameLevelIndex;
			this.itemIdsInPot = itemIdsInPot;
			this.itemIdsInBucket = itemIdsInBucket;
			this.itemIdsInTreasureBox = itemIdsInTreasureBox;
			this.monsterIds = monsterIds;
			this.bossId = bossId;
			this.goldAmountRange = goldAmountRange;
		}


	
	}
}
