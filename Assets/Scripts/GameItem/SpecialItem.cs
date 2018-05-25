using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
   
	public delegate Equipment ReturnEquipmentCallBack();

	public enum SpecialItemType{
		TuiMoJuanZhou,
        ChongZhuShi,
        YinShenYuPai,
        DianJinFuShi,
        TieYaoShi,
        TongYaoShi,
        JinYaoShi,
        WanNengYaoShi,
        QiaoZhen,
		QianDai,
        ShenMiYaoJi,
        ShenMiMianJu,
        JingYanZhiShu,
        BaoXiang,
        CaoYao,
        QuSanChangDi,
        QuSanLingDang,
        HuoBa,
        YouDeng,
        KaiGuan,
        SiYeCao,
        XingYunYuMao

	}

	[System.Serializable]
	public class SpecialItem : Item
    {
		public SpecialItemType specialItemType;

		public bool isShowInBagOnly;
        
		public SpecialItem(SpecialItemModel specialItemModel,int itemCount){

			InitBaseProperties(specialItemModel);

			this.itemType = ItemType.SpecialItem;

			this.itemCount = itemCount;

			this.isShowInBagOnly = specialItemModel.isShowInBagOnly;
            
		}

		public void UseSpecialItem(Item itemForSpecialOperation,CallBackWithItem refreshItemDetailCallBack){
                     
			switch(specialItemType){
				case SpecialItemType.TuiMoJuanZhou:
					if(itemForSpecialOperation is Equipment){
						Equipment equipment = itemForSpecialOperation as Equipment;
						PropertyGemstone propertyGemstone = equipment.RemovePropertyGemstone();
						Player.mainPlayer.AddItem(propertyGemstone);
						refreshItemDetailCallBack(equipment);
					}
					break;
				case SpecialItemType.ChongZhuShi:
					if(itemForSpecialOperation is Equipment){
						Equipment equipment = itemForSpecialOperation as Equipment;
						equipment.RebuildEquipment();
						refreshItemDetailCallBack(equipment);
					}
					break;
				case SpecialItemType.YinShenYuPai:
					ExploreManager.Instance.battlePlayerCtr.fadeStepsLeft = 20;
                    GameManager.Instance.soundManager.PlayAudioClip(CommonData.yinShenAudioName);
					break;
				case SpecialItemType.DianJinFuShi:
					if(itemForSpecialOperation is Equipment){
						Equipment equipment = itemForSpecialOperation as Equipment;
						equipment.SetToGoldQuality();
						refreshItemDetailCallBack(equipment);
					}
					break;
				case SpecialItemType.TieYaoShi:
				case SpecialItemType.TongYaoShi:
				case SpecialItemType.JinYaoShi:
				case SpecialItemType.WanNengYaoShi:               
				case SpecialItemType.QiaoZhen:
					break;
				case SpecialItemType.QianDai:
					Player.mainPlayer.totalGold += 500;
					GameManager.Instance.soundManager.PlayAudioClip(CommonData.goldAudioName);
					break;
				case SpecialItemType.ShenMiYaoJi:
					Player.mainPlayer.skillNumLeft += 2;
					GameManager.Instance.soundManager.PlayAudioClip(CommonData.drinkAudioName);
					break;
				case SpecialItemType.ShenMiMianJu:
					ExploreManager.Instance.battlePlayerCtr.fadeStepsLeft = 30;
					GameManager.Instance.soundManager.PlayAudioClip(CommonData.yinShenAudioName);
					break;
				case SpecialItemType.JingYanZhiShu:
					Player.mainPlayer.agentLevel++;
					GameManager.Instance.soundManager.PlayAudioClip(CommonData.levelUpAudioName);
					break;
				case SpecialItemType.BaoXiang:
					int gemstoneCount = Random.Range(1, 4);
					List<PropertyGemstoneModel> allHighGradeGemstones = GameManager.Instance.gameDataCenter.allPropertyGemstoneModels.FindAll(delegate (PropertyGemstoneModel obj)
					{
						return obj.grade == GemstoneGrade.High;
					});
					for (int i = 0; i < gemstoneCount;i++){
						int randomSeed = Random.Range(0, allHighGradeGemstones.Count);
						PropertyGemstoneModel propertyGemstoneModel = allHighGradeGemstones[randomSeed];
						PropertyGemstone propertyGemstone = new PropertyGemstone(propertyGemstoneModel, 1);
						Player.mainPlayer.AddItem(propertyGemstone);                  
					}
					GameManager.Instance.soundManager.PlayAudioClip(CommonData.gemstoneAudioName);

					break;
				case SpecialItemType.CaoYao:
					Player.mainPlayer.health += Mathf.RoundToInt(Player.mainPlayer.maxHealth * 0.4f);
					GameManager.Instance.soundManager.PlayAudioClip(CommonData.healthHealAudiotName);
                    break;
				case SpecialItemType.QuSanChangDi:
				case SpecialItemType.QuSanLingDang:
					ExploreManager.Instance.newMapGenerator.SomeMonstersToPool(0.3f);
                    break;
				case SpecialItemType.HuoBa:
				case SpecialItemType.YouDeng:
					Player.mainPlayer.exploreMaskStatus = 1;
					ExploreManager.Instance.newMapGenerator.SetUpExploreMask();
					GameManager.Instance.persistDataManager.SaveCompletePlayerData();
                    break;
				case SpecialItemType.KaiGuan:
					ExploreManager.Instance.newMapGenerator.AllTrapsOff();
                    break;
				case SpecialItemType.SiYeCao:
					Player.mainPlayer.luckInOpenTreasure = 1;
					GameManager.Instance.persistDataManager.SaveCompletePlayerData();
					GameManager.Instance.soundManager.PlayAudioClip(CommonData.propertyIncreaseAudioName);
                    break;
				case SpecialItemType.XingYunYuMao:
					Player.mainPlayer.luckInMonsterTreasure = 1;
                    GameManager.Instance.persistDataManager.SaveCompletePlayerData();
					GameManager.Instance.soundManager.PlayAudioClip(CommonData.propertyIncreaseAudioName);
                    break;
									
			}

		}

    }

}

