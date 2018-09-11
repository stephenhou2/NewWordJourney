using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

	public class LearnedSkillFromQuizHUD : ZoomHUD {

		public Text skillNameText;

		public Image skillIcon;

		public Text coolenTimeText;

		public Text manaConsumeText;

		public Text skillDescriptionText;

		private CallBack quitCallBack;


        
		public void SetUpLearnedSkillHUD(Skill skill, CallBack quitCallBack){

			GameManager.Instance.soundManager.PlayAudioClip(CommonData.paperAudioName);
                     
			this.quitCallBack = quitCallBack;

			skillNameText.text = skill.skillName;

			skillIcon.sprite = GameManager.Instance.gameDataCenter.allSkillSprites.Find(delegate (Sprite obj)
			{
				return obj.name == skill.skillIconName;
			});
            
			if (skill is ActiveSkill)
            {
                ActiveSkill activeSkill = skill as ActiveSkill;
                manaConsumeText.text = string.Format("魔法消耗: {0}", activeSkill.manaConsume);
                coolenTimeText.text = string.Format("冷却时间: {0}s", activeSkill.skillCoolenTime);
            }
            else
            {
                manaConsumeText.text = "被动";
                coolenTimeText.text = "";
            }

			skillDescriptionText.text = skill.skillDescription;

			if(zoomCoroutine != null){
				StopCoroutine(zoomCoroutine);
			}

			zoomCoroutine = HUDZoomIn();
            
			gameObject.SetActive(true);

			StartCoroutine(zoomCoroutine);

		}

		public void QuitLearnedSkillHUD(){

			if (inZoomingOut)
            {
                return;
            }

			if(zoomCoroutine != null){
				StopCoroutine(zoomCoroutine);
			}

			zoomCoroutine = HUDZoomOut(delegate{
				if(quitCallBack != null){
					quitCallBack();
                }

				gameObject.SetActive(false);
			});

			StartCoroutine(zoomCoroutine);

		}

		
	}
    
}
