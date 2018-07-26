using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;
	using DragonBones;
	using DG.Tweening;
	using Transform = UnityEngine.Transform;

	public delegate void SkillButtonClickCallBack(int indexInPanel);

	[RequireComponent(typeof(Button))]
	public class ActiveSkillButton : MonoBehaviour {

		public Button button;

		public Image skillIcon;
		public Text skillName;
		public Text manaConsume;
		public Image mask;

		private int indexInPanel;
		public ActiveSkill skill;

		private Tweener maskTweener;
		private Transform skillButtonContainer;

		public UnityArmatureComponent validTint;

		public void SetUpActiveSkillButton(ActiveSkill activeSkill, int index, Transform skillButtonContainer){
         
			this.skillButtonContainer = skillButtonContainer;

			if (maskTweener != null) {
				maskTweener.Kill ();
			}

			Sprite skillSprite = GameManager.Instance.gameDataCenter.allSkillSprites.Find (delegate(Sprite obj) {
				return obj.name == activeSkill.skillIconName;
			});

			skillIcon.sprite = skillSprite;

			skillName.text = activeSkill.skillName;

			manaConsume.text = activeSkill.manaConsume.ToString ();

			this.indexInPanel = index;

			this.skill = activeSkill;

			bool isManaEnough = Player.mainPlayer.mana >= skill.manaConsume;

			manaConsume.color = isManaEnough ? Color.white : Color.red;

			mask.fillAmount = isManaEnough ? 0 : 1;
			mask.enabled = !isManaEnough;

			float coolenPercentage = skill.coolenPercentage / 100f;

			mask.fillAmount = coolenPercentage;

			mask.enabled = !isManaEnough || skill.coolenPercentage > 0;

			button.interactable = isManaEnough && skill.coolenPercentage == 0 && !ExploreManager.Instance.battlePlayerCtr.isInEscaping;

			switch(skill.skillStatus){
				case ActiveSkillStatus.None:
					mask.enabled = false;
					mask.fillAmount = 0;
					if (isManaEnough)
                    {
						if(!validTint.gameObject.activeInHierarchy){
							validTint.gameObject.SetActive(true);
						}

						if(!validTint.animation.isPlaying){
							validTint.animation.Play("default", 0);
                        }
                       
					}else{
						validTint.gameObject.SetActive(false);
					}
					break;
				case ActiveSkillStatus.Waiting:
					mask.enabled = true;
					mask.fillAmount = 1;
					validTint.gameObject.SetActive(false);
					break;
				case ActiveSkillStatus.Cooling:
					mask.DOFillAmount(0, skill.skillCoolenTime * coolenPercentage).OnComplete(delegate {
						
						isManaEnough = Player.mainPlayer.mana >= skill.manaConsume;

						if (ExploreManager.Instance.battlePlayerCtr.isInSkillAttackAnimBeforeHit)
						{
							mask.enabled = true;
							mask.fillAmount = 1;
							skill.coolenPercentage = 0;
							button.interactable = false;
							skill.skillStatus = ActiveSkillStatus.Waiting;
							validTint.gameObject.SetActive(false);
						}
						else
						{
							mask.enabled = !isManaEnough;
							mask.fillAmount = 0;
							skill.coolenPercentage = 0;
							button.interactable = isManaEnough;
							skill.skillStatus = ActiveSkillStatus.None;
							if (isManaEnough)
							{
								if (!validTint.gameObject.activeInHierarchy)
								{
									validTint.gameObject.SetActive(true);
								}
								if (!validTint.animation.isPlaying)
								{
									validTint.animation.Play("default", 0);
								}

							}
							else
							{
								validTint.gameObject.SetActive(false);
							}

							manaConsume.color = isManaEnough ? Color.white : Color.red;
						}
                    });                  
					break;
			}

		}
      
		public void AddListener(SkillButtonClickCallBack cb){
			
			button.onClick.RemoveAllListeners ();

			button.onClick.AddListener (delegate{

				mask.fillAmount = 1.0f;
                mask.enabled = true;
                button.interactable = false;

				skill.skillStatus = ActiveSkillStatus.Cooling;

                cb(indexInPanel);

				if (maskTweener != null)
                {
                    maskTweener.Kill();
                }
            
				validTint.gameObject.SetActive(false);

                maskTweener = mask.DOFillAmount(0, skill.skillCoolenTime).OnComplete(delegate {

                    bool isManaEnough = Player.mainPlayer.mana >= skill.manaConsume;

					if(ExploreManager.Instance.battlePlayerCtr.isInSkillAttackAnimBeforeHit){
						mask.enabled = true;
                        mask.fillAmount = 1;
                        skill.coolenPercentage = 0;
                        button.interactable = false;
						skill.skillStatus = ActiveSkillStatus.Waiting;
						validTint.gameObject.SetActive(false);
					}else{
						mask.enabled = !isManaEnough;
                        mask.fillAmount = 0;
                        skill.coolenPercentage = 0;
                        button.interactable = isManaEnough;
						skill.skillStatus = ActiveSkillStatus.None;
						if (isManaEnough)
                        {
							if (!validTint.gameObject.activeInHierarchy)
                            {
                                validTint.gameObject.SetActive(true);
                            }
							if(!validTint.animation.isPlaying){
								validTint.animation.Play("default", 0);
							}                     
						}else{
							validTint.gameObject.SetActive(false);
						}
					}

					manaConsume.color = isManaEnough ? Color.white : Color.red;
                   
                });


				for (int i = 0; i < skillButtonContainer.childCount; i++)
                {               
                    ActiveSkill activeSkill = Player.mainPlayer.attachedActiveSkills[i];

                    if (activeSkill.skillId == skill.skillId)
                    {                  
                        continue;
                    }

					ActiveSkillButton activeSkillButton = skillButtonContainer.GetChild(i).GetComponent<ActiveSkillButton>();

                    bool manaEnough = Player.mainPlayer.mana >= activeSkill.manaConsume;

					activeSkillButton.manaConsume.color = manaEnough ? Color.white : Color.red;

					if(activeSkill.skillStatus != ActiveSkillStatus.Cooling){
						activeSkillButton.mask.enabled = true;
                        activeSkillButton.button.interactable = false;
                        activeSkillButton.mask.fillAmount = 1f;
                        activeSkill.coolenPercentage = 100;
						activeSkill.skillStatus = ActiveSkillStatus.Waiting;                  
					}
               
					activeSkillButton.validTint.gameObject.SetActive(false);

                }


			});
		}

		public void Reset()
		{
			if (maskTweener != null)
            {
                maskTweener.Kill();
            }

			//skill.skillStatus = ActiveSkillStatus.None;

			if(validTint.animation != null){
				validTint.animation.Stop();
			}

			validTint.gameObject.SetActive(false);

			mask.enabled = false;

			skillIcon.sprite = null;

			mask.fillAmount = 0;


		}

	}
}
