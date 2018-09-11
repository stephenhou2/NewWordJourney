using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace WordJourney
{

	public class Crystal : MapEvent {

		public bool isExausted;

		public Transform blinkAnim;

		public Sprite crystalShiningSprite;
		public Sprite crystalExaustedSprite;

		private int mapIndex;

		//private PropertyType propertyType;
		//private int gainAmount;

		//private string extraInfo;

		public void CrystalExausted(){

			isExausted = true;
            
			mapItemRenderer.sprite = crystalExaustedSprite;

			blinkAnim.gameObject.SetActive(false);

			tmPro.enabled = false;

			GameManager.Instance.gameDataCenter.currentMapEventsRecord.AddEventTriggeredRecord(mapIndex, transform.position);
         
		}

		public override void  AddToPool(InstancePool pool){
			bc2d.enabled = false;
			pool.AddInstanceToPool (this.gameObject);
		}

		public override void InitializeWithAttachedInfo(int mapIndex,MapAttachedInfoTile attachedInfo)
		{
			transform.position = attachedInfo.position;

			this.mapIndex = mapIndex;

			isExausted = false;
			bc2d.enabled = true;
			mapItemRenderer.sprite = crystalShiningSprite;
			SetSortingOrder (-(int)transform.position.y);
			blinkAnim.gameObject.SetActive(true);
			CheckIsWordTriggeredAndShow ();

			if (GameManager.Instance.gameDataCenter.currentMapEventsRecord.IsMapEventTriggered(mapIndex, attachedInfo.position))
            {
                CrystalExausted();
            }
		}


		public override void EnterMapEvent(BattlePlayerController bp)
		{
			if (isExausted) {
				bp.isInEvent = false;
				return;
			}
            
			ExploreManager.Instance.ShowWordsChoosePlane (wordsArray,"技能点+1");


		}

		public override void MapEventTriggered (bool isSuccess, BattlePlayerController bp)
		{
			bp.isInEvent = false;

			if (isSuccess) {

				Player.mainPlayer.skillNumLeft++;

				ExploreManager.Instance.expUICtr.SetUpSingleTextTintHUD("技能点+1");

				GameManager.Instance.soundManager.PlayAudioClip(CommonData.crystalAudioName);
                
				ExploreManager.Instance.battlePlayerCtr.SetEffectAnim(CommonData.skillPointUpEffectName);


			} 

			isExausted = true;

			blinkAnim.gameObject.SetActive(false);

			tmPro.enabled = false;

			mapItemRenderer.sprite = crystalExaustedSprite;

			GameManager.Instance.gameDataCenter.currentMapEventsRecord.AddEventTriggeredRecord(mapIndex, transform.position);
		}


		//private void InitRandomPropertyAndAccordValue(){

		//	int randomSeed = Random.Range (0, 100);

		//	propertyType = (PropertyType)randomSeed;

		//	// 10%机率：生命，魔法，物理伤害，魔法伤害，移动速度，护甲，抗性，护甲穿刺，魔法穿刺
		//	// 5%机率：额外金钱，额外经验
		//	// 2%机率：暴击，闪避，暴击倍率，生命回复效果，魔法回复效果

		//	if (randomSeed >= 0 && randomSeed < 10) {
		//		propertyType = PropertyType.MaxHealth;//生命
		//		gainAmount = Random.Range (1, 6);
		//	} else if (randomSeed >= 10 && randomSeed < 20) {
		//		propertyType = PropertyType.MaxMana;//魔法
		//		gainAmount = Random.Range (1, 3);
		//	} else if (randomSeed >= 20 && randomSeed < 30) {
		//		propertyType = PropertyType.Attack;//物理伤害
		//		gainAmount = Random.Range (1, 3);
		//	} else if (randomSeed >= 30 && randomSeed < 40) {
		//		propertyType = PropertyType.MagicAttack;//魔法伤害
		//		gainAmount = Random.Range (1, 3);
		//	} else if (randomSeed >= 40 && randomSeed < 50) {
		//		propertyType = PropertyType.MoveSpeed;//移动速度
		//		gainAmount = Random.Range (1, 5);
		//	} else if (randomSeed >= 50 && randomSeed < 60) {
		//		propertyType = PropertyType.Armor;//护甲
		//		gainAmount = Random.Range (1, 3);
		//	} else if (randomSeed >= 60 && randomSeed < 30) {
		//		propertyType = PropertyType.MagicResist;//抗性
		//		gainAmount = Random.Range (1, 3);
		//	} else if (randomSeed >= 20 && randomSeed < 70) {
		//		propertyType = PropertyType.ArmorDecrease;//护甲穿刺
		//		gainAmount = Random.Range (1, 3);
		//	} else if (randomSeed >= 70 && randomSeed < 80) {
		//		propertyType = PropertyType.MagicResistDecrease;//魔法穿刺
		//		gainAmount = Random.Range (1, 3);
		//	} else if (randomSeed >= 80 && randomSeed < 82) {
		//		propertyType = PropertyType.Crit;//暴击率
		//		gainAmount = Random.Range (1, 3);
		//	} else if (randomSeed >= 82 && randomSeed < 84) {
		//		propertyType = PropertyType.Dodge;//闪避率
		//		gainAmount = Random.Range (1, 3);
		//	} else if (randomSeed >= 84 && randomSeed < 86) {
		//		propertyType = PropertyType.CritHurtScaler;//暴击倍率
		//		gainAmount = Random.Range (1, 3);
		//	} else if (randomSeed >= 86 && randomSeed < 91) {
		//		propertyType = PropertyType.ExtraGold;//额外金钱
		//		gainAmount = Random.Range (1, 3);
		//	} else if (randomSeed >= 91 && randomSeed < 96) {
		//		propertyType = PropertyType.ExtraExperience;//额外经验
		//		gainAmount = Random.Range (1, 3);
		//	} else if (randomSeed >= 96 && randomSeed < 98) {
		//		propertyType = PropertyType.HealthRecovery;//生命回复效果
		//		gainAmount = Random.Range (1, 3);
		//	} else if (randomSeed >= 98 && randomSeed < 100) {
		//		propertyType = PropertyType.MagicRecovery;//魔法回复效果
		//		gainAmount = Random.Range (1, 3);
		//	}
		//}


	}
}
