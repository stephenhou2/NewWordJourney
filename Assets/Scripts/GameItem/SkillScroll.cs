using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

	[System.Serializable]
	public class SkillScroll : Item {

		public int skillId;


		public SkillScroll(SkillScrollModel skillScrollModel,int itemCount){

			InitBaseProperties (skillScrollModel);

			this.itemType = ItemType.SkillScroll;

			this.itemCount = itemCount;

			this.skillId = skillScrollModel.skillId;

		}

		public PropertyChange UseSkillScroll(){

			Skill skill = SkillGenerator.GenerateSkill(skillId);

			return Player.mainPlayer.LearnSkill(skill);
		}
      
	}
}
