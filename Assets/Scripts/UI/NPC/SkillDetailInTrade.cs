using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

	public class SkillDetailInTrade : MonoBehaviour
    {
		public Transform skillDetailContainer;

		public Image skillIcon;

		public Text skillNameText;

		public Text skillDescriptionText;

		public Text manaConsumeText;

		public Text coolenTimeText;

		public Button learnButton;

		public Button forgetButton;

		public TintHUD tintHUD;


		public void SetUpSkillDetailInTrade(Skill skill){

			if(skill == null){
				ClearSkillDetailInTrade();
				return;
			}

			skillDetailContainer.gameObject.SetActive(true);

			skillIcon.sprite = GameManager.Instance.gameDataCenter.allSkillSprites.Find(delegate (Sprite obj)
			{
				return obj.name == skill.skillIconName;
			});

			skillIcon.enabled = true;

			skillNameText.text = skill.skillName;

			skillDescriptionText.text = skill.skillDescription;
            
			if(skill is ActiveSkill){
				ActiveSkill activeSkill = skill as ActiveSkill;
				manaConsumeText.text = string.Format("魔法消耗: {0}", activeSkill.manaConsume);
				coolenTimeText.text = string.Format("冷却时间: {0}s", activeSkill.skillCoolenTime);
			}else{
				manaConsumeText.text = "被动";
				coolenTimeText.text = "";
			}

			bool skillHasLearned = Player.mainPlayer.CheckSkillHasLearned(skill.skillId);

         
			learnButton.gameObject.SetActive(!skillHasLearned);
			forgetButton.gameObject.SetActive(skillHasLearned);
                     

		}





		public void ClearSkillDetailInTrade(){

			skillIcon.enabled = false;

			skillNameText.text = string.Empty;

			skillDescriptionText.text = string.Empty;

			manaConsumeText.text = string.Empty;

			coolenTimeText.text = string.Empty;

			skillDetailContainer.gameObject.SetActive(false);

			learnButton.gameObject.SetActive(false);

			forgetButton.gameObject.SetActive(false);

		}

    }
}

