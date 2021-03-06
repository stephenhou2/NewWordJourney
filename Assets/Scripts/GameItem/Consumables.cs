﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;


namespace WordJourney
{
	
//	回复类别 - 回复生命或者回复魔法的
//	点金符石 - 可以将不是金色品质的装备提升为金色属性
//	重铸符石 - 可以把装备进行重置，随机变化为灰色（50%），蓝色（30%），金色（20%）。
//	退魔卷轴 - 清洗掉装备上附加的技能
//	隐身卷轴 - 进入隐身状态持续20步，不会被怪物发现。


	//public enum ConsumablesType{
	//	ShuXingTiSheng,
	//	DianJinShi,
	//	ChongZhuShi,
	//	XiaoMoJuanZhou,
	//	YinShenJuanZhou
	//}

    /// <summary>
    /// 消耗品类
    /// </summary>
	[System.Serializable]
	public class Consumables : Item {

        
        // 消耗品的属性增益

		public int healthGain;
		public int manaGain;
		public int experienceGain;

		public int maxHealthGain;
		public int maxManaGain;
		public int attackGain;
		public int magicAttackGain;
		public int armorGain;
		public int magicResistGain;
		public int armorDecreaseGain;
		public int magicResistDecreaseGain;
		public int moveSpeedGain;
		public float critGain;
		public float dodgeGain;
		public float critHurtScalerGain;
		public float physicalHurtScalerGain;
		public float magicalHurtScalerGain;
		public int extraGoldGain;
		public int extraExperienceGain;
		public int healthRecoveryGain;
		public int magicRecoveryGain;

		public int consumablesGrade;
        // 是否只能出现在背包栏中
		public bool isShowInBagOnly;
        // 使用该消耗品时的音频特效名称
		public string audioName;

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="itemModel">Item model.</param>
		public Consumables(ConsumablesModel consumablesModel,int itemCount){

			// 初始化物品基础属性
			InitBaseProperties (consumablesModel);

			this.itemType = ItemType.Consumables;
			this.itemCount = itemCount;

			this.healthGain = consumablesModel.healthGain;
			this.manaGain = consumablesModel.manaGain;
			this.experienceGain = consumablesModel.experienceGain;

			this.maxHealthGain = consumablesModel.maxHealthGain;
			this.maxManaGain = consumablesModel.maxManaGain;
			this.attackGain = consumablesModel.attackGain;
			this.magicAttackGain = consumablesModel.magicAttackGain;
			this.armorGain = consumablesModel.armorGain;
			this.magicResistGain = consumablesModel.magicResistGain;
			this.armorDecreaseGain = consumablesModel.armorDecreaseGain;
			this.magicResistDecreaseGain = consumablesModel.magicResistDecreaseGain;
			this.moveSpeedGain = consumablesModel.moveSpeedGain;
			this.critGain = consumablesModel.critGain;
			this.dodgeGain = consumablesModel.dodgeGain;
			this.critHurtScalerGain = consumablesModel.critHurtScalerGain;
			this.physicalHurtScalerGain = consumablesModel.physicalHurtScalerGain;
			this.magicalHurtScalerGain = consumablesModel.magicalHurtScalerGain;
			this.extraGoldGain = consumablesModel.extraGoldGain;
			this.extraExperienceGain = consumablesModel.extraExperienceGain;
			this.healthRecoveryGain = consumablesModel.healthRecoveryGain;
			this.magicRecoveryGain = consumablesModel.magicRecoveryGain;

			this.consumablesGrade = consumablesModel.consumablesGrade;
            
			this.isShowInBagOnly = consumablesModel.isShowInBagOnly;
          
			this.audioName = consumablesModel.audioName;
         
		}
      
        /// <summary>
        /// 使用消耗品
        /// </summary>
        /// <returns>返回角色属性变化</returns>
        /// <param name="battleAgentController">Battle agent controller.</param>
		public PropertyChange UseConsumables(BattleAgentController battleAgentController){

			Player player = Player.mainPlayer;

			PropertyChange propertyChange = new PropertyChange();

            // 如果提升最大生命值
            if (maxHealthGain > 0)
            {
                int maxHealthRecord = player.maxHealth;
				player.maxHealth += maxHealthGain;
                player.originalMaxHealth += maxHealthGain;
                // 按照最大生命值的提升比例同时提升实际生命值
                player.health = Mathf.RoundToInt((player.health * (float)player.maxHealth / maxHealthRecord));
                propertyChange.maxHealthChange = maxHealthGain;
            }

            // 如果提升最大魔法值
            if (maxManaGain > 0)
            {
                int maxManaRecord = player.maxMana;
                player.maxMana += maxManaGain;
                player.originalMaxMana += maxManaGain;
				// 按照最大魔法值的提升比例同时提升实际魔法值
                player.mana = Mathf.RoundToInt(player.mana * (float)player.maxMana / maxManaRecord);
                propertyChange.maxManaChange = maxManaGain;
            
            }
         
            // 如果提升实际生命值
            if (healthGain > 0)
            {            
				if (battleAgentController != null)
                {
					battleAgentController.AddHealthGainAndShow(healthGain + player.healthRecovery);
					battleAgentController.SetEffectAnim(CommonData.healthHealEffecttName);
				}else{
					player.health += healthGain + player.healthRecovery;
				}
            }

            // 如果提升实际魔法值
            if (manaGain > 0)
            {            
				if (battleAgentController != null)
                {
					battleAgentController.AddManaGainAndShow(manaGain + player.magicRecovery);
					battleAgentController.SetEffectAnim(CommonData.magicHealEffectName);
				}else{
					player.mana += manaGain + player.magicRecovery;
				}
			}else if(manaGain < 0){
				player.mana += manaGain;
			}

            // 如果提升实际经验值
			if(experienceGain > 0){
				player.experience += experienceGain;   
				bool isLevelUp = Player.mainPlayer.LevelUpIfExperienceEnough();
                if (isLevelUp)
                {
                    ExploreManager.Instance.battlePlayerCtr.SetEffectAnim(CommonData.levelUpEffectName);
                    GameManager.Instance.soundManager.PlayAudioClip(CommonData.levelUpAudioName);
                    ExploreManager.Instance.expUICtr.ShowLevelUpPlane();
                }

			}

            // 提升物理攻击力
            if (attackGain > 0)
            {
                player.attack += attackGain;
                player.originalAttack += attackGain;
                propertyChange.attackChange = attackGain;
            }

            // 提升魔法攻击力
            if (magicAttackGain > 0)
            {
                player.magicAttack += magicAttackGain;
                player.originalMagicAttack += magicAttackGain;
                propertyChange.magicAttackChange = magicAttackGain;
            }

            // 提升护甲
            if (armorGain > 0)
            {
                player.armor += armorGain;
                player.originalArmor += armorGain;
                propertyChange.armorChange = armorGain;
            }

            // 提升抗性
            if (magicResistGain > 0)
            {
                player.magicResist += magicResistGain;
                player.originalMagicResist += magicResistGain;
                propertyChange.magicResistChange = magicResistGain;
            }

            // 提升护甲穿透
            if (armorDecreaseGain > 0)
            {
                player.armorDecrease += armorDecreaseGain;
                player.originalArmorDecrease += armorDecreaseGain;
                propertyChange.armorDecreaseChange = armorDecreaseGain;
            }

            // 提升抗性穿透
            if (magicResistDecreaseGain > 0)
            {
                player.magicResistDecrease += magicResistDecreaseGain;
                player.originalMagicResistDecrease += magicResistDecreaseGain;
                propertyChange.magicResistDecreaseChange = magicResistDecreaseGain;
            }

            // 提升移动速度
            if (moveSpeedGain > 0)
            {
                player.moveSpeed += moveSpeedGain;
                player.originalMoveSpeed += moveSpeedGain;
            }

            // 提升暴击
            if (critGain > 0)
            {
				float myCritGain = (float)critGain / 100;
				player.crit += myCritGain;
				player.originalCrit += myCritGain;
				propertyChange.critChange = myCritGain;
            }

            // 提升闪避
            if (dodgeGain > 0)
            {
                float myDodgeGain = (float)dodgeGain / 100;
				player.dodge += myDodgeGain;
				player.originalDodge += myDodgeGain;
				propertyChange.dodgeChange = myDodgeGain;
            }

            // 提升暴击伤害倍率
            if (critHurtScalerGain > 0)
            {
                float myCritHurtScalerGain = (float)critHurtScalerGain / 100;
				player.critHurtScaler += myCritHurtScalerGain;
				player.originalCritHurtScaler += myCritHurtScalerGain;
            }

            // 提升物理伤害系数
            if (physicalHurtScalerGain > 0)
            {
                float myPhysicalHurtScalerGain = (float)physicalHurtScalerGain / 100;
				player.physicalHurtScaler += myPhysicalHurtScalerGain;
				player.originalPhysicalHurtScaler += myPhysicalHurtScalerGain;
            }

            // 提升魔法伤害系数
            if (magicalHurtScalerGain > 0)
            {
                float myMagicalHurtScalerGain = (float)magicalHurtScalerGain / 100;
				player.magicalHurtScaler += myMagicalHurtScalerGain;
				player.originalMagicalHurtScaler += myMagicalHurtScalerGain;
            }

            // 提升额外金币
            if (extraGoldGain > 0)
            {
                player.extraGold += extraGoldGain;
                player.originalExtraGold += extraGoldGain;
                propertyChange.extraGoldChange = extraGoldGain;
            }

            // 提升额外经验
            if (extraExperienceGain > 0)
            {
                player.extraExperience += extraExperienceGain;
                player.originalExtraExperience += extraExperienceGain;
                propertyChange.extraExperienceChange = extraExperienceGain;
            }

            // 提升生命回复
            if (healthRecoveryGain > 0)
            {
                player.healthRecovery += healthRecoveryGain;
                player.originalHealthRecovery += healthRecoveryGain;
                propertyChange.healthRecoveryChange = healthRecoveryGain;
            }

            // 提升魔法回复
            if (magicRecoveryGain > 0)
            {
                player.magicRecovery += magicRecoveryGain;
                player.originalMagicRecovery += magicRecoveryGain;
                propertyChange.magicRecoveryChange = magicRecoveryGain;
            }



			return propertyChange;

		}

	}




}