using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;



namespace WordJourney
{
	[System.Serializable]
	public struct MonsterInfo{
		public int monsterId;
		public int monsterCount;

		public MonsterInfo(int monsterId,int monsterCount){
			this.monsterId = monsterId;
			this.monsterCount = monsterCount;
		}
	}

	
	[System.Serializable]
	public class GameLevelData {

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

		// 商人处卖的商品信息组
//		public List<GoodsGroup> goodsGroups;

		// 关卡中怪物相对与prefab的提升比例
		public float monsterScaler;

		// 关卡中boss的id（-1代表本关不出现boss）
		public int bossId;



		// 关卡的瓦罐一定会出现的物品组
		public List<Item> mustAppearItemsInUnlockedBox = new List<Item>();

		// 关卡的瓦罐中可能会出现的物品组
		public List<Item> possiblyAppearItemsInUnlockedBox = new List<Item> ();

		// 关卡的宝箱中可能会出现的物品组
		public List<Item> possiblyAppearItemsInLockedBox = new List<Item> ();



		public Transform LoadMonster(int monsterId){
			
			string monsterName = MyTool.GetMonsterName (monsterId);

			Transform monster = GameManager.Instance.gameDataCenter.LoadMonster (monsterName).transform;

			return monster;
		}

		public void LoadAllData(){
			LoadAllItemsData ();
			LoadNPCData ();
		}


		public GameLevelData Copy(){
			
			GameLevelData copy = new GameLevelData ();

			copy.bossId = this.bossId;
			copy.chapterName = this.chapterName;
			copy.gameLevelIndex = this.gameLevelIndex;
			copy.monsterScaler = this.monsterScaler;

			copy.monsterInfos = this.monsterInfos.Clone() as MonsterInfo[];

			copy.mustAppearItemIdsInUnlockedBox = this.mustAppearItemIdsInUnlockedBox.Clone() as int[];

			copy.possiblyAppearItemIdsInLockedBox = this.possiblyAppearItemIdsInLockedBox.Clone() as int[];

			copy.possiblyAppearItemIdsInUnlockedBox = this.possiblyAppearItemIdsInUnlockedBox.Clone() as int[];

			return copy;
		}


		/// <summary>
		/// 加载所有本关卡物品数据
		/// </summary>
		private void LoadAllItemsData(){

//			for (int i = 0; i < mustAppearItemIdsInUnlockedBox.Length; i++) {
//				int itemId = InitItemIdWithOriginalData(mustAppearItemIdsInUnlockedBox [i]);
//				Item item = Item.NewItemWith(itemId,1);
//				mustAppearItemsInUnlockedBox.Add (item);
//			}
//			for (int i = 0; i < possiblyAppearItemIdsInUnlockedBox.Length; i++) {
//				int itemId = InitItemIdWithOriginalData (possiblyAppearItemIdsInUnlockedBox [i]);
//				Item item = Item.NewItemWith (itemId, 1);
//				possiblyAppearItemsInUnlockedBox.Add (item);
//			}
//			for (int i = 0; i < possiblyAppearItemIdsInLockedBox.Length; i++) {
//				int itemId = InitItemIdWithOriginalData (possiblyAppearItemIdsInLockedBox [i]);
//				Item item = Item.NewItemWith (itemId, 1);
//				possiblyAppearItemsInLockedBox.Add (item);
//			}
		
		}


		public int InitItemIdWithOriginalData(int oriId){

			int targetId = oriId;

//			switch (targetId) {
//			case -2:
//				targetId = GetRandomUnlockScrollId ();
//				break;
//			case -3:
//				targetId = GetRandomCraftingRecipeId ();
//				break;
//			default:
//				break;
//
//			}

			return targetId;

		}


//		private int GetRandomUnlockScrollId(){
//
//			int randomUnlockScrollId = 0;
//
//			int type = Random.Range (0, 2);
//
//			switch (type) {
//			case 0:
//				randomUnlockScrollId = 200 + Random.Range (Equipment.minProducableEquipmentId, Equipment.maxProducableEquipmentId + 1);
//				break;
//			case 1:
//				randomUnlockScrollId = 200 + Random.Range (Consumables.minProducableConsumablesId, Consumables.maxProducableConsumablesId + 1);
//				break;
//			}
//
//			return randomUnlockScrollId;
//		}
//
//		private int GetRandomCraftingRecipeId(){
//			int randomCraftingRecipeId = 400 + Random.Range (Equipment.minCraftingEquipmentId, Equipment.maxCraftingEquipmentId + 1);
//			return randomCraftingRecipeId;
//		}


		/// <summary>
		/// 加载所有本关卡怪物
		/// </summary>
//		private void LoadMonsters(){
//
//			for (int i = 0; i < monsterInfos.Length; i++) {
//
//				MonsterInfo info = monsterInfos [i];
//
//				string monsterName = MyTool.GetMonsterName (info.monsterId);
//
//				Transform monster = GameManager.Instance.gameDataCenter.LoadMonster (monsterName).transform;
//
//				for (int j = 0; j < info.monsterCount; j++) {
//					monsters.Add (monster);
//				}
//			}
//		}

		/// <summary>
		/// 加载所有本关卡npc
		/// </summary>
		private void LoadNPCData(){

//			for (int i = 0; i < npcIds.Length; i++) {
//				
//				NPC npc = GameManager.Instance.gameDataCenter.allNpcs.Find (delegate(NPC obj) {
//					return obj.npcId == npcIds[i];
//				});
//
//				if (npc != null) {
//					npcs.Add (npc);
//				} else {
//					Debug.LogError ("npc null when load level info");
//				}
//			}
		}
			

		public void Clear(){
			
//			monsterInfos = null;
//
//			mustAppearItemIdsInUnlockedBox = null;
//
//			possiblyAppearItemIdsInUnlockedBox = null;
//
//			possiblyAppearItemIdsInLockedBox = null;
//
//			mustAppearItemsInUnlockedBox = null;
//
//			possiblyAppearItemsInUnlockedBox = null;
//
//			possiblyAppearItemsInLockedBox = null;
//
//			monsters = null;
		}

		public override string ToString ()
		{
			return string.Format ("[chapterIndex:]" + gameLevelIndex +
			"[\nchapterLocation:]" + chapterName);
		}
	}
}
