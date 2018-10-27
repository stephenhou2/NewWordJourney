using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	/// <summary>
    /// 属性变化模型
    /// </summary>
	public struct PropertyChange {
		
		public int maxHealthChange;
		public int maxManaChange;
		public int attackChange;
		public int magicAttackChange;
		public int armorChange;
		public int magicResistChange;
		public int armorDecreaseChange;
		public int magicResistDecreaseChange;
		public float dodgeChange;
		public float critChange;
		public int healthRecoveryChange;
		public int magicRecoveryChange;
		public int extraGoldChange;
		public int extraExperienceChange;

        /// <summary>
        /// 构造函数
        /// </summary>
		public PropertyChange(int maxHealthChange,int maxManaChange,int attackChange,
			int magicAttackChange,int armorChange,int magicResistChange,int armorDecreaseChange,
			int magicResistDecreaseChange,float dodgeChage,float critChange,int healthRecoveryChange,
			int magicRecoveryChange,int extraGoldChange,int extraExperienceChange){

			this.maxHealthChange = maxHealthChange;
			this.maxManaChange = maxManaChange;
			this.attackChange = attackChange;
			this.magicAttackChange = magicAttackChange;
			this.armorChange = armorChange;
			this.magicResistChange = magicResistChange;
			this.armorDecreaseChange = armorDecreaseChange;
			this.magicResistDecreaseChange = magicResistDecreaseChange;
			this.dodgeChange = dodgeChage;
			this.critChange = critChange;
			this.healthRecoveryChange = healthRecoveryChange;
			this.magicRecoveryChange = magicRecoveryChange;
			this.extraGoldChange = extraGoldChange;
			this.extraExperienceChange = extraExperienceChange;

		}

        /// <summary>
        /// 合并两个属性变化
        /// </summary>
        /// <returns>The two property change.</returns>
        /// <param name="arg1">Arg1.</param>
        /// <param name="arg2">Arg2.</param>
		public static PropertyChange MergeTwoPropertyChange(PropertyChange arg1,PropertyChange arg2){
			PropertyChange mergedPropertyChange = new PropertyChange ();
			mergedPropertyChange.maxHealthChange= arg1.maxHealthChange + arg2.maxHealthChange;
			mergedPropertyChange.maxManaChange = arg1.maxManaChange + arg2.maxManaChange;
			mergedPropertyChange.attackChange = arg1.attackChange + arg2.attackChange;
			mergedPropertyChange.magicAttackChange = arg1.magicAttackChange + arg2.magicAttackChange;
			mergedPropertyChange.armorChange = arg1.armorChange + arg2.armorChange;
			mergedPropertyChange.magicResistChange = arg1.magicResistChange + arg2.magicResistChange;
			mergedPropertyChange.armorDecreaseChange = arg1.armorDecreaseChange + arg2.armorDecreaseChange;
			mergedPropertyChange.magicResistDecreaseChange = arg1.magicResistDecreaseChange + arg2.magicResistDecreaseChange;
			mergedPropertyChange.dodgeChange = arg1.dodgeChange + arg2.dodgeChange;
			mergedPropertyChange.critChange = arg1.critChange + arg2.critChange;
			mergedPropertyChange.healthRecoveryChange = arg1.healthRecoveryChange + arg2.healthRecoveryChange;
			mergedPropertyChange.magicRecoveryChange = arg1.magicRecoveryChange + arg2.magicRecoveryChange;
			mergedPropertyChange.extraGoldChange = arg1.extraGoldChange + arg2.extraGoldChange;
			mergedPropertyChange.extraExperienceChange = arg1.extraExperienceChange = arg2.extraExperienceChange;
			return mergedPropertyChange;
		}

	}

}
