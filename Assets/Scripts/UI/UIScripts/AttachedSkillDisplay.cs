using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
    using UnityEngine.UI;

    public class AttachedSkillDisplay : MonoBehaviour
    {
    
        //public Image attachedSkillIcon;
		//public Text attachedSkillName;
        public Text attachedSkillDescription;
		public Text manaConsumeText;
		public Text coolenTimeText;


        public void SetUpAttachedSkillDisplay(Skill skill)
        {

            //attachedSkillName.text = skill.skillName;

			if(skill is ActiveSkill){
				ActiveSkill activeSkill = skill as ActiveSkill;
				manaConsumeText.text = string.Format("魔法消耗:{0}", activeSkill.manaConsume);
				coolenTimeText.text = string.Format("冷却时间:{0}s",activeSkill.skillCoolenTime);            
			}else{
				manaConsumeText.text = "被动技能";
				coolenTimeText.text = string.Empty;
			}
         
            //Sprite s = GameManager.Instance.gameDataCenter.allSkillSprites.Find(delegate (Sprite obj) {
            //    return obj.name == skill.skillIconName;
            //});

            //attachedSkillIcon.sprite = s;
            //attachedSkillIcon.enabled = s != null;

            this.attachedSkillDescription.text = skill.skillDescription;

			this.gameObject.SetActive(true);
        }

    }
}
