using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{

	public class TreasureBox: Treasure {

		public bool isGoldTreasureBox;

		public Sprite normalTbSprite;
		public Sprite specialTbSprite;



		/// <summary>
		/// 地图物品被破坏或开启
		/// </summary>
		/// <param name="cb">Cb.</param>
		public override void PlayAnimAndAudio(CallBack cb){

			animEndCallBack = cb;

			GameManager.Instance.soundManager.PlayAudioClip ("MapEffects/" + audioClipName);

			mapItemRenderer.enabled = false;

			mapItemAnimator.gameObject.SetActive (true);

			// 播放对应动画
			if (!isGoldTreasureBox) {
				mapItemAnimator.SetTrigger ("PlayNormal");
			} else {
				mapItemAnimator.SetTrigger ("PlaySpecial");
			}

			StartCoroutine ("ResetMapItemOnAnimFinished");

		}
			

		public override void InitializeWithAttachedInfo (MapAttachedInfoTile attachedInfo)
		{
			transform.position = attachedInfo.position;

			isGoldTreasureBox = false;

			if (KVPair.ContainsKey ("type",attachedInfo.properties)) {
				string type = KVPair.GetPropertyStringWithKey ("type", attachedInfo.properties);
				isGoldTreasureBox = type == "1";
			}
				
			if (!isGoldTreasureBox) {
				mapItemRenderer.sprite = normalTbSprite;
			} else {
				mapItemRenderer.sprite = specialTbSprite;
			}

				
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

			CheckIsWordTriggeredAndShow ();

			bc2d.enabled = true;
			mapItemAnimator.gameObject.SetActive (false);
			mapItemRenderer.enabled = true;
			int sortingOrder = -(int)transform.position.y;
			SetSortingOrder (sortingOrder);
			SetAnimationSortingOrder (sortingOrder);

		}


//		public override void MapEventTriggered (bool isSuccess, BattlePlayerController bp)
//		{
//
//			bc2d.enabled = false;
//
//			ExploreManager.Instance.newMapGenerator.mapWalkableInfoArray [(int)transform.position.x, (int)transform.position.y] = 1;
//
//			Vector3 droppedItemPos = transform.position + new Vector3 (0, dropItemOffsetY, 0);
//
//			PlayTreasureBoxAnimAndAudio (delegate {
//				if (isSuccess) {
//					ExploreManager.Instance.newMapGenerator.SetUpRewardInMap (rewardItem,droppedItemPos);
//				}else{
//					Debug.Log("wrong answer");
//				}
//			});
//
//			AddToPool (ExploreManager.Instance.newMapGenerator.mapEventsPool);
//		}
	}
}
