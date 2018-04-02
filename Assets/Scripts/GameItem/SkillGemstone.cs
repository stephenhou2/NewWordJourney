using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

	[System.Serializable]
	public class SkillGemstone : Item {

		public int skillId;

//		public SkillType skillType;
//
//		public int manaConsume;
//
//		public float coolenTime;
//
//		public string skillName;
//
//		public string skillIconName;
//
//		public string skillDescription;

		public SkillGemstone(SkillGemstoneModel gemModel,int count){

			this.itemType = ItemType.Gemstone;

			this.itemId = gemModel.itemId;
			this.itemName = gemModel.itemName;
			this.spriteName = gemModel.spriteName;
			this.itemDescription = gemModel.itemDescription;

			this.skillId = gemModel.skillId;
//			this.skillType = gemModel.skillType;
//			this.manaConsume = gemModel.manaConsume;
//			this.coolenTime = gemModel.coolenTime;
//			this.skillName = gemModel.skillName;
//			this.skillIconName = gemModel.skillIconName;
//			this.skillDescription = gemModel.skillDescription;

		}

		public override string GetItemTypeString ()
		{
			return "技能宝石";
		}



	}
}
