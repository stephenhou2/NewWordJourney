using System.Collections;
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


	[System.Serializable]
	public class Consumables : Item {

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

		public bool isShowInBagOnly;

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
            
			this.isShowInBagOnly = consumablesModel.isShowInBagOnly;
          
			this.audioName = consumablesModel.audioName;
         
		}
      

		public PropertyChange UseConsumables(){

			Player player = Player.mainPlayer;

			PropertyChange propertyChange = new PropertyChange();

            if (maxHealthGain > 0)
            {
                int maxHealthRecord = player.maxHealth;
                player.maxHealth += maxHealthGain;
                player.originalMaxHealth += maxHealthGain;
                player.health = Mathf.RoundToInt((player.health * (float)player.maxHealth / maxHealthRecord));
                propertyChange.maxHealthChange = maxHealthGain;
            }

            if (maxManaGain > 0)
            {
                int maxManaRecord = player.maxMana;
                player.maxMana += maxManaGain;
                player.originalMaxMana += maxManaGain;
                player.mana = Mathf.RoundToInt(player.mana * (float)player.maxMana / maxManaRecord);
                propertyChange.maxManaChange = maxManaGain;
            }

            if (healthGain > 0)
            {
                player.health += healthGain;
            }

            if (manaGain > 0)
            {
                player.mana += manaGain;
            }

            if (attackGain > 0)
            {
                player.attack += attackGain;
                player.originalAttack += attackGain;
                propertyChange.attackChange = attackGain;
            }

            if (magicAttackGain > 0)
            {
                player.magicAttack += magicAttackGain;
                player.originalMagicAttack += magicAttackGain;
                propertyChange.magicAttackChange = magicAttackGain;
            }

            if (armorGain > 0)
            {
                player.armor += armorGain;
                player.originalArmor += armorGain;
                propertyChange.armorChange = armorGain;
            }

            if (magicResistGain > 0)
            {
                player.magicResist += magicResistGain;
                player.originalMagicResist += magicResistGain;
                propertyChange.magicResistChange = magicResistGain;
            }

            if (armorDecreaseGain > 0)
            {
                player.armorDecrease += armorDecreaseGain;
                player.originalArmorDecrease += armorDecreaseGain;
                propertyChange.armorDecreaseChange = armorDecreaseGain;
            }

            if (magicResistDecreaseGain > 0)
            {
                player.magicResistDecrease += magicResistDecreaseGain;
                player.originalMagicResistDecrease += magicResistDecreaseGain;
                propertyChange.magicResistDecreaseChange = magicResistDecreaseGain;
            }

            if (moveSpeedGain > 0)
            {
                player.moveSpeed += moveSpeedGain;
                player.originalMoveSpeed += moveSpeedGain;
            }

            if (critGain > 0)
            {
				float myCritGain = (float)critGain / 100;
				player.crit += myCritGain;
				player.originalCrit += myCritGain;
				propertyChange.critChange = myCritGain;
            }

            if (dodgeGain > 0)
            {
                float myDodgeGain = (float)dodgeGain / 100;
				player.dodge += myDodgeGain;
				player.originalDodge += myDodgeGain;
				propertyChange.dodgeChange = myDodgeGain;
            }

            if (critHurtScalerGain > 0)
            {
                float myCritHurtScalerGain = (float)critHurtScalerGain / 100;
				player.critHurtScaler += myCritHurtScalerGain;
				player.originalCritHurtScaler += myCritHurtScalerGain;
            }

            if (physicalHurtScalerGain > 0)
            {
                float myPhysicalHurtScalerGain = (float)physicalHurtScalerGain / 100;
				player.physicalHurtScaler += myPhysicalHurtScalerGain;
				player.originalPhysicalHurtScaler += myPhysicalHurtScalerGain;
            }

            if (magicalHurtScalerGain > 0)
            {
                float myMagicalHurtScalerGain = (float)magicalHurtScalerGain / 100;
				player.magicalHurtScaler += myMagicalHurtScalerGain;
				player.originalMagicalHurtScaler += myMagicalHurtScalerGain;
            }

            if (extraGoldGain > 0)
            {
                player.extraGold += extraGoldGain;
                player.originalExtraGold += extraGoldGain;
                propertyChange.extraGoldChange = extraGoldGain;
            }

            if (extraExperienceGain > 0)
            {
                player.extraExperience += extraExperienceGain;
                player.originalExtraExperience += extraExperienceGain;
                propertyChange.extraExperienceChange = extraExperienceGain;
            }

            if (healthRecoveryGain > 0)
            {
                player.healthRecovery += healthRecoveryGain;
                player.originalHealthRecovery += healthRecoveryGain;
                propertyChange.healthRecoveryChange = healthRecoveryGain;
            }

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