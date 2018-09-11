using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

	public class SkillGoodsInTrade : MonoBehaviour
    {
		public Image skillIcon;
		public Text skillNameText;
		public Text learnNeedText;
		//public Text learnPrice;
		public Image selectedIcon;

		private Skill skill;
		private CallBackWithSkill skillSelectCallBack;
        
        /// <summary>
        /// 初始化npc处的技能商品展示
        /// </summary>
        /// <param name="skill">Skill.</param>
        /// <param name="skillSelectCallBack">Skill select call back.</param>
		public void SetupSkillDetailInNPC(Skill skill,CallBackWithSkill skillSelectCallBack){

			this.skill = skill;
			this.skillSelectCallBack = skillSelectCallBack;

            
			Sprite skillSprite = GameManager.Instance.gameDataCenter.allSkillSprites.Find(delegate (Sprite obj)
			{
				return obj.name == skill.skillIconName;            
			});

			skillIcon.sprite = skillSprite;
			skillIcon.enabled = skillSprite != null;

			skillNameText.text = skill.skillName;

			learnNeedText.text = string.Format("学习要求:{0}个单词", skill.wordCountToLearn);
                     
			//bool skillLearned = Player.mainPlayer.CheckSkillHasLearned(skill);
         
			//learnPrice.text = skill.price.ToString();

		}
		                                  
		public void SetUpSelectedIcon(bool isVisible){

			selectedIcon.enabled = isVisible;
		}
                                        

        /// <summary>
        /// npc处卖的技能点击回调
        /// </summary>
		public void OnClick(){

			if(skillSelectCallBack != null){
				skillSelectCallBack(skill);            
			}

		}
    }


}
