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

			//animEndCallBack = cb;

			GameManager.Instance.soundManager.PlayAudioClip(CommonData.treasureBoxAudioName);
         
			if(cb != null){
				cb();
			}
		}



		public override void InitializeWithAttachedInfo (int mapIndex,MapAttachedInfoTile attachedInfo)
		{
			this.mapIndex = mapIndex;

			transform.position = attachedInfo.position;

			isGoldTreasureBox = false;

			if (KVPair.ContainsKey ("type",attachedInfo.properties)) {
				string type = KVPair.GetPropertyStringWithKey ("type", attachedInfo.properties);
				isGoldTreasureBox = type == "1";
			}

			int rewardItemId = 0;

			if (!isGoldTreasureBox)
            {
                mapItemRenderer.sprite = normalTbSprite;
                treasureType = TreasureType.NormalTreasureBox;
            }
            else
            {
                mapItemRenderer.sprite = specialTbSprite;
                treasureType = TreasureType.GoldTreasureBox;
            }


			if(KVPair.ContainsKey("dropID",attachedInfo.properties)){
				rewardItemId = int.Parse(KVPair.GetPropertyStringWithKey("dropID", attachedInfo.properties));
			}else{
				rewardItemId = GetRandomItemIdFromGameLevelData();  
			}

			if(isGoldTreasureBox && MapEventsRecord.IsMapEventTriggered(mapIndex,attachedInfo.position)){
				AddToPool(ExploreManager.Instance.newMapGenerator.mapEventsPool);
			}
				

			rewardItem = Item.NewItemWith(rewardItemId, 1);	


			if (rewardItem.itemType == ItemType.Equipment) {

				Equipment eqp = rewardItem as Equipment;

				int randomSeed = Random.Range (0, 100);

				int graySeed = 0;
				int blueSeed = 0;

				switch(Player.mainPlayer.luckInOpenTreasure){
					case 0:
						graySeed = 65 - Player.mainPlayer.extraLuckInOpenTreasure;
						blueSeed = 95 - Player.mainPlayer.extraLuckInOpenTreasure;
						break;
					case 1:
						graySeed = 60 - Player.mainPlayer.extraLuckInOpenTreasure;
						blueSeed = 90 - Player.mainPlayer.extraLuckInOpenTreasure;
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
			//mapItemAnimator.gameObject.SetActive (false);
			mapItemRenderer.enabled = true;
			int sortingOrder = -(int)transform.position.y;
			SetSortingOrder (sortingOrder);
			//SetAnimationSortingOrder (sortingOrder);

		}


		public override void MapEventTriggered(bool isSuccess, BattlePlayerController bp)
		{

			//int posX = Mathf.RoundToInt(this.transform.position.x);
            //int posY = Mathf.RoundToInt(this.transform.position.y);
         
			base.MapEventTriggered(isSuccess, bp);
         
			if(isGoldTreasureBox){
				MapEventsRecord.AddEventTriggeredRecord(mapIndex, transform.position);
			}else{
				GameManager.Instance.gameDataCenter.currentMapEventsRecord.AddEventTriggeredRecord(mapIndex, transform.position);
			}

		}
	}
}
