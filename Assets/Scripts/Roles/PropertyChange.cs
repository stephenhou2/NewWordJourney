using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public struct PropertyChange {
		
		public int maxHealthChange;
		public int maxManaChange;
		public int attackChange;
		public int magicAttackChange;
		public int armorChange;
		public int magicResistChange;

		public PropertyChange(int maxHealthChange,int maxManaChange,int attackChange,
			int magicAttackChange,int armorChange,int magicResistChange){

			this.maxHealthChange = maxHealthChange;
			this.maxManaChange = maxManaChange;
			this.attackChange = attackChange;
			this.magicAttackChange = magicAttackChange;
			this.armorChange = armorChange;
			this.magicResistChange = magicResistChange;

		}

		public static PropertyChange MergeTwoPropertyChange(PropertyChange arg1,PropertyChange arg2){
			PropertyChange mergedPropertyChange = new PropertyChange ();
			mergedPropertyChange.maxHealthChange= arg1.maxHealthChange + arg2.maxHealthChange;
			mergedPropertyChange.maxManaChange = arg1.maxManaChange + arg2.maxManaChange;
			mergedPropertyChange.attackChange = arg1.attackChange + arg2.attackChange;
			mergedPropertyChange.magicAttackChange = arg1.magicAttackChange + arg2.magicAttackChange;
			mergedPropertyChange.armorChange = arg1.armorChange + arg2.armorChange;
			mergedPropertyChange.magicResistChange = arg1.magicResistChange + arg2.magicResistChange;

			return mergedPropertyChange;
		}

	}

}
