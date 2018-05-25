using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney{

	using UnityEngine.UI;


	public class SkillDetail : MonoBehaviour
    {

		public Image skillIcon;

		public Text skillNameText;

		public Text skillTypeText;

		public Text skillDescriptionText;

		public Text manaConsumeText;

		public Text coolenTimeText;

		public Transform upgradeButton;

		public Transform forgetButton;

		public Text upgradeNumText;
       
		public void SetupSkillDetail(Skill skill){

			skillIcon.sprite = GameManager.Instance.gameDataCenter.allSkillSprites.Find(delegate (Sprite obj)
			{
				return obj.name == skill.skillIconName;
			});

			skillIcon.enabled = true;

			skillNameText.text = skill.skillName;

			skillTypeText.text = skill.skillType == SkillType.Active ? "技能类型：主动" : "技能类型：被动";

			skillDescriptionText.text = skill.skillDescription;
            
			if(skill.skillType == SkillType.Active){
				ActiveSkill activeSkill = skill as ActiveSkill;
				manaConsumeText.text = string.Format("魔法消耗：{0}", activeSkill.manaConsume);
				coolenTimeText.text = string.Format("冷却时间：{0}", activeSkill.skillCoolenTime);
			}else{
				manaConsumeText.text = "魔法消耗：-";
				coolenTimeText.text = "冷却时间：-";
			}

			upgradeButton.gameObject.SetActive(true);
			forgetButton.gameObject.SetActive(true);

			//Debug.Log(skill.upgradeNum);
			upgradeNumText.text = skill.upgradeNum.ToString();

		}

		public void ClearSkillDetail(){
			skillNameText.text = string.Empty;
			skillIcon.sprite = null;
			skillIcon.enabled = false;
			skillTypeText.text = string.Empty;
			skillDescriptionText.text = string.Empty;
			manaConsumeText.text = string.Empty;
			coolenTimeText.text = string.Empty;
			upgradeButton.gameObject.SetActive(false);
			forgetButton.gameObject.SetActive(false);
		}      

    }


}

