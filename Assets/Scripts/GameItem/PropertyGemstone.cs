using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{

	public enum GemstoneGrade{
		Low,
        Medium,
        High
	}

	[System.Serializable]
	public class PropertyGemstone : Item
    {      
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

		public GemstoneGrade grade;


		public PropertyGemstone(PropertyGemstoneModel pgModel,int itemCount){

			InitBaseProperties(pgModel);


			this.itemType = ItemType.PropertyGemstone;
			this.itemCount = itemCount;

			this.maxHealthGain = pgModel.maxHealthGain;
			this.maxManaGain = pgModel.maxManaGain;
			this.attackGain = pgModel.attackGain;
			this.magicAttackGain = pgModel.magicAttackGain;
			this.armorGain = pgModel.armorGain;
			this.magicResistGain = pgModel.magicResistGain;
			this.armorDecreaseGain = pgModel.armorDecreaseGain;
			this.magicResistDecreaseGain = pgModel.magicResistDecreaseGain;
			this.moveSpeedGain = pgModel.moveSpeedGain;
			this.critGain = pgModel.critGain;
			this.dodgeGain = pgModel.dodgeGain;
			this.critHurtScalerGain = pgModel.critHurtScalerGain;
			this.physicalHurtScalerGain = pgModel.physicalHurtScalerGain;
			this.magicalHurtScalerGain = pgModel.magicalHurtScalerGain;
			this.extraGoldGain = pgModel.extraGoldGain;
			this.extraExperienceGain = pgModel.extraExperienceGain;
			this.healthRecoveryGain = pgModel.healthRecoveryGain;
			this.magicRecoveryGain = pgModel.magicRecoveryGain;

			this.grade = pgModel.grade;
		}


    }


}

