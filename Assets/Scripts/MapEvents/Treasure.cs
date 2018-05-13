using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public enum TreasureType{
		Pot,
		Bucket,
		TreasuerBox
	}

	public class Treasure : MapEvent {

		public TreasureType treasureType;

		// 奖励的物品
		public Item rewardItem;

		public Animator mapItemAnimator;

		public float dropItemOffsetY;

		protected void SetAnimationSortingOrder(int order){
			mapItemAnimator.GetComponent<SpriteRenderer> ().sortingOrder = order;
		}

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

			animEndCallBack = cb;
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


			mapItemRenderer.enabled = false;

			mapItemAnimator.gameObject.SetActive (true);

			mapItemAnimator.SetTrigger ("Play");

			StartCoroutine ("ResetMapItemOnAnimFinished");

		}

		/// <summary>
		/// 动画结束后重置地图物品
		/// </summary>
		/// <returns>The map item on animation finished.</returns>
		protected IEnumerator ResetMapItemOnAnimFinished(){

			yield return null;

			AnimatorStateInfo stateInfo = mapItemAnimator.GetCurrentAnimatorStateInfo (0);

			while (stateInfo.normalizedTime < 1) {

				yield return null;

				stateInfo = mapItemAnimator.GetCurrentAnimatorStateInfo (0);

			}

			// 瓦罐和木桶解锁之后在图层内层级下调一级低层级（防止人物在上面走的时候遮挡住人物）
			int sortingOrder = mapItemRenderer.sortingOrder - 1;
			SetSortingOrder (sortingOrder);
			SetAnimationSortingOrder (sortingOrder);
			bc2d.enabled = false;


			AnimEnd ();

		}

		protected virtual void AnimEnd (){

			mapItemAnimator.ResetTrigger("Play");

			if (animEndCallBack != null) {
				animEndCallBack ();
			}
		}

		public override void InitializeWithAttachedInfo (MapAttachedInfoTile attachedInfo)
		{
			transform.position = attachedInfo.position;

			int rewardItemId = GetRandomItemIdFromGameLevelData();

			rewardItem = Item.NewItemWith (rewardItemId, 1);

			if (rewardItem.itemType == ItemType.Equipment) {

				Equipment eqp = rewardItem as Equipment;

				int randomSeed = Random.Range (0, 100);

				EquipmentQuality quality = EquipmentQuality.Gray;

				if (randomSeed < 80) {
					quality = EquipmentQuality.Gray;
				} else if (randomSeed < 95) {
					quality = EquipmentQuality.Blue;
				} else {
					quality = EquipmentQuality.Gold;
				}

				eqp.ResetPropertiesByQuality (quality);

			}

			CheckIsWordTriggeredAndShow ();

			bc2d.enabled = true;
			mapItemAnimator.gameObject.SetActive (false);
			mapItemRenderer.enabled = true;
			int sortingOrder = -(int)transform.position.y;
			SetSortingOrder (sortingOrder);
			SetAnimationSortingOrder (sortingOrder);

		}



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
			case TreasureType.TreasuerBox:
				possibleItemIds = levelData.itemIdsInTreasureBox;
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

			ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray [(int)transform.position.x, (int)transform.position.y] = 1;

			Vector3 droppedItemPos = transform.position + new Vector3 (0, dropItemOffsetY, 0);

			PlayAnimAndAudio (delegate {
				if (isSuccess) {
					ExploreManager.Instance.newMapGenerator.SetUpRewardInMap (rewardItem,droppedItemPos);
				}else{
					Debug.Log("wrong answer");
				}
			});

			AddToPool (ExploreManager.Instance.newMapGenerator.mapEventsPool);
		}
	}
}
