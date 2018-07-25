using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

	public class AttachedGemstoneDisplay : MonoBehaviour {
      
		public Image gemstoneIcon;
		public Text attachedGemstoneDescription;


		public void SetUpAttachedSkillDisplay(PropertyGemstone propertyGemstone){
         
			Sprite s = GameManager.Instance.gameDataCenter.allPropertyGemstoneSprites.Find (delegate(Sprite obj) {
				return obj.name == propertyGemstone.spriteName;
			});

			gemstoneIcon.sprite = s;
			gemstoneIcon.enabled = s != null;

			attachedGemstoneDescription.text = propertyGemstone.finalDescription;

			this.gameObject.SetActive(true);
		}

	}
}