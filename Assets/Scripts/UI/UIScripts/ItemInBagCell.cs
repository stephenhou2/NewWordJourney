using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

	public class ItemInBagCell : MonoBehaviour {

		public Image itemIcon;
		public Image itemIconBackground;
		public Image newItemTintIcon;
		public Text itemCount;
		public Image selectedTint;

		public Sprite grayFrame;
		public Sprite blueFrame;
		public Sprite goldFrame;


		public void SetUpItemInBagCell(Item item){

			itemIcon.enabled = false;
			newItemTintIcon.enabled = false;


			if (item.itemType == ItemType.Consumables) {
				itemCount.text = item.itemCount.ToString ();
				itemCount.enabled = true;
			} else {
				itemCount.text = string.Empty;
				itemCount.enabled = false;
			}

			if (item.itemType == ItemType.Equipment) {
				Equipment eqp = item as Equipment;
				switch (eqp.quality) {
				case EquipmentQuality.Gray:
					itemIconBackground.sprite = grayFrame;
					break;
				case EquipmentQuality.Blue:
					itemIconBackground.sprite = blueFrame;
					break;
				case EquipmentQuality.Gold:
					itemIconBackground.sprite = goldFrame;
					break;
				}
			} else {
				itemIconBackground.sprite = grayFrame;
			}

			Sprite itemSprite = GameManager.Instance.gameDataCenter.GetGameItemSprite (item);

			itemIcon.sprite = itemSprite;

			itemIcon.enabled = itemSprite != null;

			// 如果是新物品，则显示新物品提示图片
			newItemTintIcon.enabled = item.isNewItem;

			selectedTint.enabled = false;

		}

	}
}
