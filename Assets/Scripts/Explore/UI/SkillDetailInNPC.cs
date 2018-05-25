using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

	public class SkillDetailInNPC : MonoBehaviour
    {
		public Image skillIcon;
		public Text skillNameText;
		//public Text skillTypeText;
		public Text skillDescriptionText;
		public Text manaConsumeText;
		public Text coolenTimeText;
		public Text learnedTint;
		public Image selectedIcon;

		private Skill skill;
		private CallBackWithSkill clickCallBack;


		public void SetupSkillDetailInNPC(Skill skill,CallBackWithSkill callBackWithSkill){

			this.skill = skill;
			this.clickCallBack = callBackWithSkill;


			skillIcon.sprite = GameManager.Instance.gameDataCenter.allSkillSprites.Find(delegate (Sprite obj)
			{
				return obj.name == skill.skillIconName;

			});

			skillNameText.text = skill.skillName;

			skillDescriptionText.text = skill.skillDescription;

			if(skill.skillType == SkillType.Active){

				ActiveSkill activeSkill = skill as ActiveSkill;

				manaConsumeText.text = string.Format("魔法消耗：{0}", activeSkill.manaConsume);

				coolenTimeText.text = string.Format("技能冷却：{0}s", activeSkill.skillCoolenTime);

			}else{

				manaConsumeText.text = "被动";

				coolenTimeText.text = string.Empty;

			}

			bool skillLearned = Player.mainPlayer.CheckSkillHasLearned(skill);

			if(skillLearned){
				learnedTint.enabled = true;
			}else{
				learnedTint.enabled = false;
			}

		}
		                                  
		public void SetUpSelectedIcon(bool isVisible){

			selectedIcon.enabled = isVisible;
		}
                                        

        /// <summary>
        /// 技能点击回调
        /// </summary>
		public void OnClick(){

			if(clickCallBack != null){
				clickCallBack(skill);            
			}

		}
    }


}
