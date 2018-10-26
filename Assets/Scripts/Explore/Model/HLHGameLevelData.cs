using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{

    /// <summary>
    /// 关卡数据模型
    /// </summary>
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

        /// <summary>
        /// 本关是否出现老头npc
        /// </summary>
        /// <returns><c>true</c>, if wise man was hased, <c>false</c> otherwise.</returns>
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

        /// <summary>
        /// 判断本关是否是boss关
        /// </summary>
        /// <returns><c>true</c>, if boss level was ised, <c>false</c> otherwise.</returns>
        public static bool IsBossLevel(){
            return (Player.mainPlayer.currentLevelIndex + 1) % 5 == 0;
        }

        
        /// <summary>
		/// 判断本关是否出现日记
        /// </summary>
        /// <returns><c>true</c>, if diary paper was hased, <c>false</c> otherwise.</returns>
		public static bool HasDiaryPaper(){
			return Player.mainPlayer.currentLevelIndex == 0 ||
						 ((Player.mainPlayer.currentLevelIndex + 1) % 5 == 0 && Player.mainPlayer.currentLevelIndex < 45);
		}


        /// <summary>
        /// 关卡数据构造函数
        /// </summary>
        /// <param name="gameLevelIndex">关卡序号.</param>
        /// <param name="itemIdsInPot">瓦罐中可出现的物品id list.</param>
        /// <param name="itemIdsInBucket">木桶中可出现的物品id list.</param>
        /// <param name="itemIdsInNormalTreasureBox">宝箱中可出现的物品id list.</param>
        /// <param name="monsterIds">关卡中可出现的怪物id list.</param>
        /// <param name="bossId">关卡中的boss id.</param>
        /// <param name="goldAmountRange">关卡中钱袋中可开出的金币数量范围.</param>
        /// <param name="monsterIdsOfCurrentLevel">本关中所有怪物的id list【普通怪物+boss】.</param>
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
