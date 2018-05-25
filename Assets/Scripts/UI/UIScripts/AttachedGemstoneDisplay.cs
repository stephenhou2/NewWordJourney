using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

	public class AttachedGemstoneDisplay : MonoBehaviour {

		public Text attachedSkillName;
		public Image attachedSkillIcon;
		public Text manaConsume;
		public Text coolenTime;
		public Text attachedSkillDescription;


		public void SetUpAttachedSkillDisplay(Skill skill){

			attachedSkillName.text = skill.skillName;

			if (skill.skillType != SkillType.Active) {
				this.manaConsume.text = "被动";
				this.coolenTime.text = "-";
			} else {
				ActiveSkill activeSk = skill as ActiveSkill;
				this.manaConsume.text = activeSk.manaConsume.ToString ();
				this.coolenTime.text = activeSk.skillCoolenTime.ToString ();
			}

			Sprite s = GameManager.Instance.gameDataCenter.allSkillSprites.Find (delegate(Sprite obj) {
				return obj.name == skill.skillIconName;
			});

			attachedSkillIcon.sprite = s;
			attachedSkillIcon.enabled = s != null;

			this.attachedSkillDescription.text = skill.skillDescription;
		}

	}
}