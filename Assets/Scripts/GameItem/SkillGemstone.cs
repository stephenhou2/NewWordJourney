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

			InitBaseProperties (gemModel);

			this.itemType = ItemType.Gemstone;

			this.skillId = gemModel.skillId;

		}

		public override string GetItemTypeString ()
		{
			return "技能宝石";
		}



	}
}
