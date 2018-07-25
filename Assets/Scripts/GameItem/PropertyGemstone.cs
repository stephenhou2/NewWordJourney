using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	using System.Text;

	public enum GemstoneGrade{
		Low,
        Medium,
        High
	}

	[System.Serializable]
	public class PropertyGemstone : Item
    {      

		public int maxHealthGainBase;
        public int maxManaGainBase;
        public int attackGainBase;
        public int magicAttackGainBase;
        public int armorGainBase;
        public int magicResistGainBase;
        public int armorDecreaseGainBase;
        public int magicResistDecreaseGainBase;
        public int moveSpeedGainBase;
		public int critGainBase;
		public int dodgeGainBase;
		public int critHurtScalerGainBase;
		public int physicalHurtScalerGainBase;
		public int magicalHurtScalerGainBase;
        public int extraGoldGainBase;
        public int extraExperienceGainBase;
        public int healthRecoveryGainBase;
        public int magicRecoveryGainBase;

        public int maxHealthGain;
        public int maxManaGain;
        public int attackGain;
        public int magicAttackGain;
        public int armorGain;
        public int magicResistGain;
        public int armorDecreaseGain;
        public int magicResistDecreaseGain;
        public int moveSpeedGain;
		public int critGain;
		public int dodgeGain;
		public int critHurtScalerGain;
		public int physicalHurtScalerGain;
		public int magicalHurtScalerGain;
        public int extraGoldGain;
        public int extraExperienceGain;
        public int healthRecoveryGain;
        public int magicRecoveryGain;

		public GemstoneGrade grade;

		public string finalDescription;
              
        // 默认构造函数
        // 没有数据的宝石物品id为-1
		public PropertyGemstone(){
			this.itemId = -1;
		}

		public PropertyGemstone(PropertyGemstoneModel pgModel,int itemCount){

			InitBaseProperties(pgModel);


			this.itemType = ItemType.PropertyGemstone;
			this.itemCount = itemCount;

			this.maxHealthGainBase = pgModel.maxHealthGainBase;
			this.maxManaGainBase = pgModel.maxManaGainBase;
			this.attackGainBase = pgModel.attackGainBase;
			this.magicAttackGainBase = pgModel.magicAttackGainBase;
			this.armorGainBase = pgModel.armorGainBase;
			this.magicResistGainBase = pgModel.magicResistGainBase;
			this.armorDecreaseGainBase = pgModel.armorDecreaseGainBase;
			this.magicResistDecreaseGainBase = pgModel.magicResistDecreaseGainBase;
			this.moveSpeedGainBase = pgModel.moveSpeedGainBase;
			this.critGainBase = pgModel.critGainBase;
			this.dodgeGainBase = pgModel.dodgeGainBase;
			this.critHurtScalerGainBase = pgModel.critHurtScalerGainBase;
			this.physicalHurtScalerGainBase = pgModel.physicalHurtScalerGainBase;
			this.magicalHurtScalerGainBase = pgModel.magicalHurtScalerGainBase;
			this.extraGoldGainBase = pgModel.extraGoldGainBase;
			this.extraExperienceGainBase = pgModel.extraExperienceGainBase;
			this.healthRecoveryGainBase = pgModel.healthRecoveryGainBase;
			this.magicRecoveryGainBase = pgModel.magicRecoveryGainBase;

			this.grade = pgModel.grade;
		}

		public void GemStonePropertyConfigure(){

			//PropertyChange propertyChange = new PropertyChange();

			StringBuilder stringBuilder = new StringBuilder();

            if (maxHealthGainBase > 0)
            {
				maxHealthGain = Mathf.RoundToInt(maxHealthGainBase * Random.Range(0.7f, 1.3f));
				stringBuilder.AppendFormat("生命 +{0}\n", maxHealthGain);
            }
            else if (maxManaGainBase > 0)
            {
				maxManaGain = Mathf.RoundToInt(maxManaGainBase * Random.Range(0.7f, 1.3f));
			    stringBuilder.AppendFormat("魔法 +{0}\n", maxManaGain);
            }
            else if (attackGainBase > 0)
            {
				attackGain = Mathf.RoundToInt(attackGainBase * Random.Range(0.7f, 1.3f));
				stringBuilder.AppendFormat("攻击 +{0}\n", attackGain);
            }
            else if (magicAttackGainBase > 0)
            {
				magicAttackGain = Mathf.RoundToInt(magicAttackGainBase * Random.Range(0.7f, 1.3f));
				stringBuilder.AppendFormat("魔法攻击 +{0}\n", magicAttackGain);
            }
            else if (armorGainBase > 0)
            {
				armorGain = Mathf.RoundToInt(armorGainBase * Random.Range(0.7f, 1.3f));
				stringBuilder.AppendFormat("护甲 +{0}\n", armorGain);
            }
            else if (magicResistGainBase > 0)
            {
				magicResistGain = Mathf.RoundToInt(magicResistGainBase * Random.Range(0.7f, 1.3f));
				stringBuilder.AppendFormat("抗性 +{0}\n", magicResistGain);
            }
            else if (armorDecreaseGainBase > 0)
            {
				armorDecreaseGain = Mathf.RoundToInt(armorDecreaseGainBase * Random.Range(0.7f, 1.3f));
				stringBuilder.AppendFormat("护甲穿透 +{0}\n", armorDecreaseGain);
            }
            else if (magicResistDecreaseGainBase > 0)
            {
				magicResistGain = Mathf.RoundToInt(magicResistGainBase * Random.Range(0.7f, 1.3f));
				stringBuilder.AppendFormat("抗性穿透 +{0}\n", magicResistGain);
            }
            else if (moveSpeedGainBase > 0)
            {
				moveSpeedGain = Mathf.RoundToInt(moveSpeedGainBase * Random.Range(0.7f, 1.3f));
				stringBuilder.AppendFormat("移速 +{0}\n", moveSpeedGain);
            }
            else if (critGainBase > 0)
            {
				critGain = Mathf.RoundToInt(critGainBase * Random.Range(0.7f, 1.3f));
				stringBuilder.AppendFormat("暴击 +{0}%\n", critGain);
            }
			else if (dodgeGainBase> 0)
            {
				dodgeGain = Mathf.RoundToInt(dodgeGainBase * Random.Range(0.7f, 1.3f));
				stringBuilder.AppendFormat("闪避 +{0}%\n", dodgeGain);
            }
            else if (critHurtScalerGainBase > 0)
            {
				critHurtScalerGain = Mathf.RoundToInt(critHurtScalerGainBase * Random.Range(0.7f, 1.3f));
				stringBuilder.AppendFormat("暴击倍率 +{0}%\n", critHurtScalerGain);
            }
            else if (physicalHurtScalerGainBase > 0)
            {
				physicalHurtScalerGain = Mathf.RoundToInt(physicalHurtScalerGainBase * Random.Range(0.7f, 1.3f));
                stringBuilder.AppendFormat("物理伤害 +{0}%\n", physicalHurtScalerGain);
            }
            else if (magicalHurtScalerGainBase > 0)
            {
				magicalHurtScalerGain = Mathf.RoundToInt(magicalHurtScalerGainBase * Random.Range(0.7f, 1.3f));	
                stringBuilder.AppendFormat("魔法伤害 +{0}%\n", magicalHurtScalerGain);
            }
            else if (extraGoldGainBase > 0)
            {
				extraGoldGain = Mathf.RoundToInt(extraGoldGainBase * Random.Range(0.7f, 1.3f));
				stringBuilder.AppendFormat("额外金钱 +{0}\n", extraGoldGain);
            }
            else if (extraExperienceGainBase > 0)
            {
				extraExperienceGain = Mathf.RoundToInt(extraExperienceGainBase * Random.Range(0.7f, 1.3f));
				stringBuilder.AppendFormat("额外经验 +{0}\n", extraExperienceGain);
            }
            else if (healthRecoveryGainBase > 0)
            {
				healthRecoveryGain = Mathf.RoundToInt(healthRecoveryGainBase * Random.Range(0.7f, 1.3f));
				stringBuilder.AppendFormat("生命回复 +{0}\n", healthRecoveryGain);
            }
            else if (magicRecoveryGainBase > 0)
            {
				magicRecoveryGain = Mathf.RoundToInt(magicRecoveryGainBase * Random.Range(0.7f, 1.3f));
				stringBuilder.AppendFormat("魔法回复 +{0}\n", magicRecoveryGain);
            }

			if(stringBuilder.Length > 0){
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
			}

			finalDescription = stringBuilder.ToString();

		}



    }


}

