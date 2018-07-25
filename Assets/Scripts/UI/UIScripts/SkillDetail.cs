using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney{

	using UnityEngine.UI;


	public class SkillDetail : MonoBehaviour
    {

		public Transform skillDetailContainer;

		public Image skillIcon;
        
		public Text skillNameText;

		public Text passiveSkillTint;

		public Text skillDescriptionText;

		public Text manaConsumeText;

		public Text coolenTimeText;

		public Text skillLevelText;
		public Text upgradeSkillNumText;

		public Transform upgradeButton;

		public Transform forgetButton;
       
		public void SetupSkillDetail(Skill skill){

			skillIcon.sprite = GameManager.Instance.gameDataCenter.allSkillSprites.Find(delegate (Sprite obj)
			{
				return obj.name == skill.skillIconName;
			});

			skillIcon.enabled = true;

			skillNameText.text = skill.skillName;

			if(skillLevelText != null){
				skillLevelText.text = string.Format("Lv.{0}", skill.skillLevel);
			}

			if(upgradeSkillNumText != null){
				upgradeSkillNumText.text = string.Format("升级需要技能点: {0}", skill.upgradeNum);
			}

			passiveSkillTint.enabled = skill.skillType != SkillType.Active;

			skillDescriptionText.text = skill.GetDisplayDescription();
            
			if(skill.skillType == SkillType.Active){
				ActiveSkill activeSkill = skill as ActiveSkill;
				manaConsumeText.text = string.Format("魔法消耗：{0}", activeSkill.manaConsume);
				coolenTimeText.text = string.Format("冷却时间：{0}", activeSkill.skillCoolenTime);
			}else{
				manaConsumeText.text = string.Empty;
				coolenTimeText.text = string.Empty;
			}

			upgradeButton.gameObject.SetActive(true);
			forgetButton.gameObject.SetActive(true);

			skillDetailContainer.gameObject.SetActive(true);

		}

		public void ClearSkillDetail(){
			skillNameText.text = string.Empty;
			skillIcon.sprite = null;
			skillIcon.enabled = false;         
			skillDescriptionText.text = string.Empty;
			manaConsumeText.text = string.Empty;
			coolenTimeText.text = string.Empty;
			upgradeButton.gameObject.SetActive(false);
			forgetButton.gameObject.SetActive(false);
			skillDetailContainer.gameObject.SetActive(false);
		}      

    }


}

