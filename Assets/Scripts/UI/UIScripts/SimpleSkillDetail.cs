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
		public Image selectedIcon;

		private Skill skill;
		private CallBackWithSkill callBackWithSkill;


		public void SetUpSimpleSkillDetail(Skill skill, CallBackWithSkill callBackWithSkill){

			if(skill == null){
				ClearSimpleSkillDetail();
			}

			this.skill = skill;

			this.callBackWithSkill = callBackWithSkill;

			skillNameText.text = skill.skillName;

			skillIcon.sprite = GameManager.Instance.gameDataCenter.allSkillSprites.Find(delegate (Sprite obj)
			{
				return obj.name == skill.skillIconName;
			});

			skillIcon.enabled = true;

			skillLevelText.text = string.Format("技能等级: {0}",skill.skillLevel);
                     
		}

		public void OnSkillClick(){

			if(skill == null){
				return;
			}

			if(callBackWithSkill != null){

				callBackWithSkill(skill);

			}

			selectedIcon.enabled = true;

		}

		public void SetUpSelectedIcon(bool enable){
			selectedIcon.enabled = enable;
		}

		public void ClearSimpleSkillDetail(){
			skillNameText.text = string.Empty;
			skillLevelText.text = string.Empty;
			skillIcon.sprite = null;
			skillIcon.enabled = false;
			skill = null;
		}

    }   

}
