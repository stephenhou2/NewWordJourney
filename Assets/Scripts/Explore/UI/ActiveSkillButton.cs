using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;
	using UnityEngine.Events;
	using DG.Tweening;

	public delegate void SkillButtonClickCallBack(int indexInPanel);

	[RequireComponent(typeof(Button))]
	public class ActiveSkillButton : MonoBehaviour {

		public Button button;

		public Image skillIcon;
		public Text skillName;
		public Text manaConsume;
		public Image mask;

		private int indexInPanel;
		private ActiveSkill skill;

		public void SetUpActiveSkillButton(ActiveSkill activeSkill, int index, Transform skilButtonContainer){

			Sprite skillSprite = GameManager.Instance.gameDataCenter.allSkillSprites.Find (delegate(Sprite obj) {
				return obj.name == activeSkill.skillIconName;
			});

			skillIcon.sprite = skillSprite;

			skillName.text = activeSkill.skillName;

			manaConsume.text = activeSkill.manaConsume.ToString ();


			this.indexInPanel = index;
			this.skill = activeSkill;

			mask.enabled = false;
			button.interactable = true;

			manaConsume.color = Player.mainPlayer.mana >= skill.manaConsume ? Color.blue : Color.red;


		}

		public void AddListener(SkillButtonClickCallBack cb){
			button.onClick.RemoveAllListeners ();
			button.onClick.AddListener (delegate() {
				mask.fillAmount = 1.0f;
				mask.enabled = true;
				button.interactable = false;
				mask.DOFillAmount(0,skill.skillCoolenTime).OnComplete(delegate{
					mask.enabled = false;
					button.interactable = true;
					manaConsume.color = Player.mainPlayer.mana >= skill.manaConsume ? Color.blue : Color.red;
				});
				cb(indexInPanel);
			});
		}

	}
}
