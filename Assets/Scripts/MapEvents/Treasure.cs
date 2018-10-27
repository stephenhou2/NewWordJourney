using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	//宝箱类型
	public enum TreasureType{
		Pot,
		Bucket,
		NormalTreasureBox,
        GoldTreasureBox
	}

    /// <summary>
    /// 宝箱类
    /// </summary>
	public class Treasure : MapEvent {

		public TreasureType treasureType;

		// 奖励的物品
		public Item rewardItem;

		protected int mapIndex;       
        

		public override void AddToPool (InstancePool pool)
		{
			bc2d.enabled = false;
			pool.AddInstanceToPool (this.gameObject);
		}


		/// <summary>
		/// 地图物品被破坏或开启
		/// </summary>
		/// <param name="cb">Cb.</param>
		public virtual void PlayAnimAndAudio(CallBack cb){
        
			switch(treasureType){
				case TreasureType.Pot:
					GameManager.Instance.soundManager.PlayAudioClip(CommonData.potAudioName);
					break;
				case TreasureType.Bucket:
					GameManager.Instance.soundManager.PlayAudioClip(CommonData.bucketAudioName);
					break;
				//case TreasureType.TreasuerBox:
					//GameManager.Instance.soundManager.PlayAudioClip(CommonData.treasureBoxAudioName);
					//break;
			}

			if(cb != null){
				cb();
			}
         
		}
  
		public override void InitializeWithAttachedInfo(int mapIndex,MapAttachedInfoTile attachedInfo)
		{
			transform.position = attachedInfo.position;

			this.mapIndex = mapIndex;

			int rewardItemId = GetRandomItemIdFromGameLevelData();

			rewardItem = Item.NewItemWith (rewardItemId, 1);

         
			CheckIsWordTriggeredAndShow ();

			bc2d.enabled = true;
			//mapItemAnimator.gameObject.SetActive (false);
			mapItemRenderer.enabled = true;
			int sortingOrder = -(int)transform.position.y;
			SetSortingOrder (sortingOrder);
			//SetAnimationSortingOrder (sortingOrder);

		}

        
        /// <summary>
        /// 从本层地图数据中随机读取出一个物品id
        /// </summary>
        /// <returns>The random item identifier from game level data.</returns>
		protected int GetRandomItemIdFromGameLevelData(){

			HLHGameLevelData levelData = GameManager.Instance.gameDataCenter.gameLevelDatas [Player.mainPlayer.currentLevelIndex];

			List<int> possibleItemIds = null;

			switch (treasureType) {
			case TreasureType.Pot:
				possibleItemIds = levelData.itemIdsInPot;
				break;
			case TreasureType.Bucket:
				possibleItemIds = levelData.itemIdsInBucket;
				break;
			case TreasureType.NormalTreasureBox:
				possibleItemIds = levelData.itemIdsInNormalTreasureBox;
				break;
			case TreasureType.GoldTreasureBox:
				break;
			}

			int randomSeed = Random.Range (0, possibleItemIds.Count);

			return possibleItemIds [randomSeed];


		}


		public override void EnterMapEvent(BattlePlayerController bp){
			ExploreManager.Instance.currentEnteredMapEvent = this;
			ExploreManager.Instance.ShowWordsChoosePlane (wordsArray);
		}


		public override void MapEventTriggered (bool isSuccess, BattlePlayerController bp)
		{
			bp.isInEvent = false;

			bc2d.enabled = false;

			ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray [Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y)] = 1;

			Vector3 droppedItemPos = transform.position;

			GameManager.Instance.gameDataCenter.currentMapEventsRecord.AddEventTriggeredRecord(mapIndex, transform.position);

			PlayAnimAndAudio (delegate {

				mapItemRenderer.enabled = false;

				if (isSuccess) {
					ExploreManager.Instance.newMapGenerator.SetUpRewardInMap (rewardItem,droppedItemPos);
				}else{
					Debug.Log("wrong answer");
				}
				AddToPool(ExploreManager.Instance.newMapGenerator.mapEventsPool);
			});


		}
	}
}
