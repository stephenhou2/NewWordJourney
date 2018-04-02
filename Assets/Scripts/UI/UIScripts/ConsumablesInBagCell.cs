using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace WordJourney
{

	using UnityEngine.UI;

	public class ConsumablesInBagCell : MonoBehaviour {

		public Image itemIcon;
//		public Image NewItemTintIcon;
		public Text itemCount;

		public Button consumablesButton;

		public void SetUpConsumablesInBagCell(Consumables consumables){

			if (consumables == null) {
				itemIcon.enabled = false;
//				NewItemTintIcon.enabled = false;
				itemCount.text = "";
				consumablesButton.interactable = false;
				return;
			}

			consumablesButton.interactable = true;

			ConsumablesInBagDragControl dragControl = gameObject.GetComponent<ConsumablesInBagDragControl> ();

			if (dragControl != null) {
				dragControl.item = consumables;
			}

			Sprite itemSprite = GameManager.Instance.gameDataCenter.GetGameItemSprite (consumables);
			itemIcon.sprite = itemSprite;

			itemIcon.enabled = itemSprite != null;

//			NewItemTintIcon.enabled = consumables.isNewItem;

			itemCount.text = consumables.itemCount.ToString ();


		}


	}
}
