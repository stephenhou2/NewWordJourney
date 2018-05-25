using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney{

	using UnityEngine.UI;
   
	public delegate void CallBackWithSkill(Skill skill);

	public class SimpleSkillDetail : MonoBehaviour
    {

		public Text skillNameText;
		public Image skillIcon;
		public Text skillLevelText;
		public Text upgradeNumText;

		private Skill skill;
		private CallBackWithSkill callBackWithSkill;


		public void SetUpSimpleSkillDetail(Skill skill, CallBackWithSkill callBackWithSkill){

			this.skill = skill;

			this.callBackWithSkill = callBackWithSkill;

			skillNameText.text = skill.skillName;

			skillIcon.sprite = GameManager.Instance.gameDataCenter.allSkillSprites.Find(delegate (Sprite obj)
			{
				return obj.name == skill.skillIconName;
			});

			skillLevelText.text = string.Format("技能等级:{0}",skill.skillLevel);

			upgradeNumText.text = string.Format("升级所需技能点：{0}",skill.upgradeNum);
                     
		}

		public void OnSkillClick(){

			if(callBackWithSkill != null){

				callBackWithSkill(skill);

			}

		}

    }   

}
