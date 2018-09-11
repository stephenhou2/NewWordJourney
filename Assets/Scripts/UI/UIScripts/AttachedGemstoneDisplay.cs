using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

	public class AttachedGemstoneDisplay : MonoBehaviour {
        
		public Image[] attachedGemstoneIcons;
		public Text[] attachedGemstoneDescriptions;


		public void SetUpAttachedSkillDisplay(List<PropertyGemstone> propertyGemstones){


			for (int i = 0; i < attachedGemstoneIcons.Length;i++){
				Image attachedGemstoneIcon = attachedGemstoneIcons[i];
				if (i < propertyGemstones.Count)
                {
                    PropertyGemstone propertyGemstone = propertyGemstones[i];
					Sprite s = GameManager.Instance.gameDataCenter.GetGameItemSprite(propertyGemstone);
					if(s != null){
						attachedGemstoneIcon.sprite = s;
						attachedGemstoneIcon.enabled = true;
					}else{
						attachedGemstoneIcon.enabled = false;
					}
                }
                else
                {
					attachedGemstoneIcon.enabled = false;
                }
			}

			for (int i = 0; i < attachedGemstoneDescriptions.Length;i++){

				Text gemstoneDesc = attachedGemstoneDescriptions[i];

				if(i<propertyGemstones.Count){
					PropertyGemstone propertyGemstone = propertyGemstones[i];
					gemstoneDesc.text = propertyGemstone.propertyDescription;
				}else{
					gemstoneDesc.text = string.Empty;
				}

			}
         
			this.gameObject.SetActive(true);
		}

	}
}