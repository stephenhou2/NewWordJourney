using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;
	
	public class EquipedEquipmentCell : MonoBehaviour {
    
		public Image itemIcon;
		public Image equipmentTintIcon;      
		public Image lockIcon;
		public Image selectedIcon;

		public void SetUpEquipedEquipmentCell(Equipment equipment,bool equipmentSlotUnlocked){

			selectedIcon.enabled = false;

			if (!equipmentSlotUnlocked) {
				lockIcon.enabled = true;
				equipmentTintIcon.enabled = true;
                itemIcon.enabled = false;
				return;
			}

			lockIcon.enabled = false;

			if (equipment.itemId < 0) {
				equipmentTintIcon.enabled = true;
				itemIcon.enabled = false;

			} else {
				equipmentTintIcon.enabled = false;
				Sprite itemSprite = GameManager.Instance.gameDataCenter.GetGameItemSprite (equipment);
				itemIcon.sprite = itemSprite;
				itemIcon.enabled = itemSprite != null;
			}
		}


	}
}
