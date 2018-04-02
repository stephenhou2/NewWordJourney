using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;
	
	public class EquipedEquipmentCell : MonoBehaviour {

		public Transform itemContainer;
		public Image itemIcon;

		public Transform emptyIcon;
		public Transform lockIcon;

		public void SetUpEquipedEquipmentCell(Equipment equipment,bool equipmentSlotUnlocked){

			if (!equipmentSlotUnlocked) {
				lockIcon.gameObject.SetActive (true);
				emptyIcon.gameObject.SetActive (true);
				itemContainer.gameObject.SetActive (false);
				return;
			}

			lockIcon.gameObject.SetActive (false);

			if (equipment.itemId < 0) {
//				emptyIcon.gameObject.SetActive (true);
				itemContainer.gameObject.SetActive (false);

			} else {
//				emptyIcon.gameObject.SetActive (false);
				itemContainer.gameObject.SetActive (true);
				Sprite itemSprite = GameManager.Instance.gameDataCenter.GetGameItemSprite (equipment);
				itemIcon.sprite = itemSprite;
				itemIcon.enabled = itemSprite != null;
			}
		}


	}
}
