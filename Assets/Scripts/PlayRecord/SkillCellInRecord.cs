using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

	public class SkillCellInRecord : MonoBehaviour
    {

		public Image skillIcon;

		public Text skillNameText;

		public Text skillLevelText;

		public void SetUpSkillCellInRecord(Skill skill,int skillLevel){

			Sprite skillSprite = GameManager.Instance.gameDataCenter.allSkillSprites.Find(delegate (Sprite obj)
			{
				return obj.name == skill.skillIconName;
			});

			if(skillSprite != null){
				skillIcon.enabled = true;
				skillIcon.sprite = skillSprite;
			}else{
				skillIcon.enabled = false;
			}

			skillNameText.text = skill.skillName;

			skillLevelText.text = skillLevel.ToString();

		}
        
    }
}

