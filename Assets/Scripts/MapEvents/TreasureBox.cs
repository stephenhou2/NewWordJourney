using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	public enum TreasureBoxType{
		Pot,
		Bucket,
		TreasuerBox
	}

	public class TreasureBox: MapEvent {

		// 地图物品状态变化之后是否可以行走
		public bool walkableAfterChangeStatus;

		public TreasureBoxType tbType;

		// 奖励的物品
		public Item rewardItem;

		public Animator mapItemAnimator;

		public float dropItemOffsetY;

		public bool isGoldTreasureBox;

		/// <summary>
		/// 初始化箱子类道具
		/// </summary>
		public override void InitMapItem ()
		{
			bc2d.enabled = true;
			mapItemAnimator.gameObject.SetActive (false);
			mapItemRenderer.enabled = true;
			int sortingOrder = -(int)transform.position.y;
			SetSortingOrder (sortingOrder);
			SetAnimationSortingOrder (sortingOrder);

		}

		private void SetAnimationSortingOrder(int order){
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
		public void PlayTreasureBoxAnimAndAudio(CallBack cb){

			animEndCallBack = cb;

			SoundManager.Instance.PlayAudioClip ("MapEffects/" + audioClipName);

			mapItemRenderer.enabled = false;

			mapItemAnimator.gameObject.SetActive (true);

			// 播放对应动画
			mapItemAnimator.SetTrigger ("Play");

			StartCoroutine ("ResetMapItemOnAnimFinished");

//			if (walkableAfterChangeStatus) {
//				isDroppable = true;
//			}
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

			// 如果开启或破坏后是可以行走的，动画结束后将包围盒设置为not enabled
			if (walkableAfterChangeStatus) {
				// 瓦罐和木桶解锁之后在图层内层级下调一级低层级（防止人物在上面走的时候遮挡住人物）
				int sortingOrder = mapItemRenderer.sortingOrder - 1;
				SetSortingOrder (sortingOrder);
				SetAnimationSortingOrder (sortingOrder);
				bc2d.enabled = false;
			}

			AnimEnd ();

		}

		protected void AnimEnd (){
			if (animEndCallBack != null) {
				animEndCallBack ();
			}
		}

		public override void InitializeWithAttachedInfo (MapAttachedInfoTile attachedInfo)
		{
			transform.position = attachedInfo.position;

			isGoldTreasureBox = false;

			if (KVPair.ContainsKey ("type",attachedInfo.properties)) {
				string type = KVPair.GetPropertyStringWithKey ("type", attachedInfo.properties);
				isGoldTreasureBox = type == "1";
			}

//			string rewardItemIdsString = KVPair.GetPropertyStringWithKey ("IDRange", attachedInfo.properties);

//			if (rewardItemIdsString.Equals ("-1")) {
//				rewardItem = Item.GetRandomItem ();
//			} else {
				
				int rewardItemId = GetRandomItemIdFromGameLevelData();

				rewardItem = Item.NewItemWith (rewardItemId, 1);

				if (rewardItem.itemType == ItemType.Equipment) {

					Equipment eqp = rewardItem as Equipment;

					int randomSeed = Random.Range (0, 100);

					EquipmentQuality quality = EquipmentQuality.Gray;
					if (!isGoldTreasureBox) {
						if (randomSeed < 80) {
							quality = EquipmentQuality.Gray;
						} else if (randomSeed < 95) {
							quality = EquipmentQuality.Blue;
						} else {
							quality = EquipmentQuality.Gold;
						}
					} else {
						if (randomSeed < 30) {
							quality = EquipmentQuality.Blue;
						} else {
							quality = EquipmentQuality.Gold;
						}
					}

					eqp.ResetPropertiesByQuality (quality);

				}
//			}
				
			CheckIsWordTriggeredAndShow ();

			bc2d.enabled = true;
			mapItemAnimator.gameObject.SetActive (false);
			mapItemRenderer.enabled = true;
			int sortingOrder = -(int)transform.position.y;
			SetSortingOrder (sortingOrder);
			SetAnimationSortingOrder (sortingOrder);

		}



		private int GetRandomItemIdFromGameLevelData(){

			HLHGameLevelData levelData = GameManager.Instance.gameDataCenter.gameLevelDatas [Player.mainPlayer.currentLevelIndex];

			List<int> possibleItemIds = null;

			switch (tbType) {
			case TreasureBoxType.Pot:
				possibleItemIds = levelData.itemIdsInPot;
				break;
			case TreasureBoxType.Bucket:
				possibleItemIds = levelData.itemIdsInBucket;
				break;
			case TreasureBoxType.TreasuerBox:
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

			bc2d.enabled = false;

			ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray [(int)transform.position.x, (int)transform.position.y] = 1;

			Vector3 droppedItemPos = transform.position + new Vector3 (0, dropItemOffsetY, 0);

			PlayTreasureBoxAnimAndAudio (delegate {
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
