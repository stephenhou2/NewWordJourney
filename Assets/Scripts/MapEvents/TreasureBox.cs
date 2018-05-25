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

			GameManager.Instance.soundManager.PlayAudioClip(CommonData.treasureBoxAudioName);

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

		protected override void AnimEnd()
		{
			if (!isGoldTreasureBox)
            {
				mapItemAnimator.ResetTrigger("PlayNormal");
            }
            else
            {
				mapItemAnimator.ResetTrigger("PlaySpecial");
            }

			if (animEndCallBack != null)
            {
                animEndCallBack();
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

				int graySeed = 0;
				int blueSeed = 0;

				switch(Player.mainPlayer.luckInOpenTreasure){
					case 0:
						graySeed = 65;
						blueSeed = 95;
						break;
					case 1:
						graySeed = 60;
						blueSeed = 90;
						break;
				}

				EquipmentQuality quality = EquipmentQuality.Gray;

				if (!isGoldTreasureBox) {
					if (randomSeed < graySeed) {
						quality = EquipmentQuality.Gray;
					} else if (randomSeed < blueSeed) {
						quality = EquipmentQuality.Blue;
					} else {
						quality = EquipmentQuality.Gold;
					}
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

	}
}
