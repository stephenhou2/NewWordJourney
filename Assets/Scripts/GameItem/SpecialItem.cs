using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
    // 特殊物品类型
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

    /// <summary>
    /// 特殊物品类
    /// </summary>
	[System.Serializable]
	public class SpecialItem : Item
    {
		// 特殊物品类型
		public SpecialItemType specialItemType;
        // 是否只在背包中显示
		public bool isShowInBagOnly;
        
        /// <summary>
        /// 特殊物品构造函数
        /// </summary>
        /// <param name="specialItemModel">Special item model.</param>
        /// <param name="itemCount">Item count.</param>
		public SpecialItem(SpecialItemModel specialItemModel,int itemCount){

			InitBaseProperties(specialItemModel);

			this.itemType = ItemType.SpecialItem;

			this.itemCount = itemCount;         

			this.specialItemType = specialItemModel.specialItemType;

			this.isShowInBagOnly = specialItemModel.isShowInBagOnly;
            
		}

        
        /// <summary>
        /// 使用特殊物品
        /// </summary>
        /// <returns>属性变化</returns>
        /// <param name="itemForSpecialOperation">进行特殊操作的物品</param>
        /// <param name="refreshItemDetailCallBack">使用完成后更新物品描述的回调</param>
		public PropertyChange UseSpecialItem(Item itemForSpecialOperation,CallBackWithItem refreshItemDetailCallBack){

			PropertyChange propertyChange = new PropertyChange();

			switch(specialItemType){
				case SpecialItemType.TuiMoJuanZhou:
					if(itemForSpecialOperation is Equipment){
						Equipment equipment = itemForSpecialOperation as Equipment;
                        // 使用退魔卷轴，移除装备上的所有属性宝石
						PropertyGemstone[] propertyGemstones = equipment.RemovePropertyGemstons();
                        // 属性宝石重新添加进背包
						for (int i = 0; i < propertyGemstones.Length;i++){
							Player.mainPlayer.AddItem(propertyGemstones[i]);
						}

                        // 刷新装备详细信息
						if(refreshItemDetailCallBack != null){
							refreshItemDetailCallBack(equipment);
						}  
						// 重算人物属性
						propertyChange = Player.mainPlayer.ResetBattleAgentProperties(false);
						GameManager.Instance.soundManager.PlayAudioClip(CommonData.xiaoMoAudioName);
					}
					break;
				case SpecialItemType.ChongZhuShi:
					if(itemForSpecialOperation is Equipment){
						Equipment equipment = itemForSpecialOperation as Equipment;
                        // 重铸装备
						equipment.RebuildEquipment();
                        // 刷新装备详细信息页面
						if(refreshItemDetailCallBack != null){
                            refreshItemDetailCallBack(equipment);
                        }  
                        // 重算人物属性
						propertyChange = Player.mainPlayer.ResetBattleAgentProperties(false);
						GameManager.Instance.soundManager.PlayAudioClip(CommonData.chongzhuAudioName);
					}
					break;
				case SpecialItemType.YinShenYuPai:
					int oriFadeStepLeft = ExploreManager.Instance.battlePlayerCtr.fadeStepsLeft;
                    // 人物隐身20步
					ExploreManager.Instance.battlePlayerCtr.fadeStepsLeft = Mathf.Max(oriFadeStepLeft, 20);
                    GameManager.Instance.soundManager.PlayAudioClip(CommonData.yinShenAudioName);
                    // 如果原来人物没有隐身，则播放隐身特效动画
					if (oriFadeStepLeft == 0)
					{
						ExploreManager.Instance.battlePlayerCtr.SetEffectAnim(CommonData.yinShenEffectName, null, 0, 0);
					}
					break;
				case SpecialItemType.DianJinFuShi:
					if(itemForSpecialOperation is Equipment){
						Equipment equipment = itemForSpecialOperation as Equipment;
                        // 将装备重铸为金色品质
						equipment.SetToGoldQuality();
                        // 刷新装备详细信息页面
						if(refreshItemDetailCallBack != null){
                            refreshItemDetailCallBack(equipment);
                        }  
                        // 重算人物属性
						propertyChange = Player.mainPlayer.ResetBattleAgentProperties(false);
						GameManager.Instance.soundManager.PlayAudioClip(CommonData.dianjinAudioName);
					}
					break;
				case SpecialItemType.TieYaoShi:
				case SpecialItemType.TongYaoShi:
				case SpecialItemType.JinYaoShi:
				case SpecialItemType.WanNengYaoShi:               
				case SpecialItemType.QiaoZhen:
					break;
				case SpecialItemType.QianDai:
					// 钱袋开出500金币
					Player.mainPlayer.totalGold += 500;
					GameManager.Instance.soundManager.PlayAudioClip(CommonData.goldAudioName);
					break;
				case SpecialItemType.ShenMiYaoJi:
					// 神秘药剂增加2个技能点
					Player.mainPlayer.skillNumLeft += 2;
					GameManager.Instance.soundManager.PlayAudioClip(CommonData.drinkAudioName);
					break;
				case SpecialItemType.ShenMiMianJu:
					// 神秘面具隐身30步
					oriFadeStepLeft = ExploreManager.Instance.battlePlayerCtr.fadeStepsLeft;
                    ExploreManager.Instance.battlePlayerCtr.fadeStepsLeft = Mathf.Max(oriFadeStepLeft, 30);
					if(oriFadeStepLeft == 0){
						ExploreManager.Instance.battlePlayerCtr.SetEffectAnim(CommonData.yinShenEffectName, null, 0, 0);
					}               
					GameManager.Instance.soundManager.PlayAudioClip(CommonData.yinShenAudioName);
					break;
				case SpecialItemType.JingYanZhiShu:
					// 经验之书直接升一级
					Player.mainPlayer.agentLevel++;
					ExploreManager.Instance.expUICtr.ShowLevelUpPlane();
					GameManager.Instance.soundManager.PlayAudioClip(CommonData.levelUpAudioName);
					break;
				case SpecialItemType.BaoXiang:
					// 宝箱开出1-3个高级宝石
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
					// 草药回复40%生命
					Player.mainPlayer.health += Mathf.RoundToInt(Player.mainPlayer.maxHealth * 0.4f);
					GameManager.Instance.soundManager.PlayAudioClip(CommonData.eatAudoiName);
                    break;
				case SpecialItemType.QuSanChangDi:
				case SpecialItemType.QuSanLingDang:
					// 消灭地图上30%的怪物
					ExploreManager.Instance.newMapGenerator.SomeMonstersToPool(0.3f);
                    break;
				case SpecialItemType.HuoBa:
				case SpecialItemType.YouDeng:
					// 环境变亮
					ExploreManager.Instance.newMapGenerator.SetUpExploreMask(1);
                    break;
				case SpecialItemType.KaiGuan:
					ExploreManager.Instance.newMapGenerator.AllTrapsOff();
                    break;
				case SpecialItemType.SiYeCao:
					// 四叶草提升开宝箱是开出好装备的概率
					Player.mainPlayer.luckInOpenTreasure = 1;
					//GameManager.Instance.persistDataManager.SaveCompletePlayerData();
					GameManager.Instance.soundManager.PlayAudioClip(CommonData.siYeCaoAudioName);
                    break;
				case SpecialItemType.XingYunYuMao:
					// 幸运羽毛提升怪物掉落物品的概率
					Player.mainPlayer.luckInMonsterTreasure = 1;
                    //GameManager.Instance.persistDataManager.SaveCompletePlayerData();
					GameManager.Instance.soundManager.PlayAudioClip(CommonData.xingYunYuMaoAudioName);
                    break;


			}
			return propertyChange;

		}

    }

}

