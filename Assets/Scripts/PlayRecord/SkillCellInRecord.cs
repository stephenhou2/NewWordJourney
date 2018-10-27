using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

    /// <summary>
    /// 技能槽模型
    /// </summary>
	public class SkillCellInRecord : MonoBehaviour
    {

		public Image skillIcon;

		public Text skillNameText;

		public Text skillLevelText;

        /// <summary>
        /// 重置技能槽
        /// </summary>
		public void Reset()
		{
			skillIcon.enabled = false;
			skillNameText.text = string.Empty;
			skillLevelText.text = string.Empty;
		}

        /// <summary>
        /// 初始化通关记录中的技能槽
        /// </summary>
        /// <param name="skill">Skill.</param>
        /// <param name="skillLevel">Skill level.</param>
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
			skillNameText.enabled = true;

			skillLevelText.text = string.Format("Lv.{0}", skillLevel);
			skillLevelText.enabled = true;
		}
        
    }
}

