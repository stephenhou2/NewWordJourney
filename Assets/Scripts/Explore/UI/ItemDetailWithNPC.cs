using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

	public enum ItemOperationType{
		Buy,
		Sell,
        Add,
        Remove,
        None
	}

	public class ItemDetailWithNPC : MonoBehaviour {


		public Text itemName;
		public Text itemDescription;
		public Text attachedDescription;
		public Image itemIcon;
		public Image itemIconBackground;


		public Transform buyButton;
		public Transform sellButton;
		public Transform addButton;
		public Transform removeButton;

		public AttachedGemstoneDisplay attachedSkillDisplay;

		public void SetUpItemDetail(Item item,ItemOperationType tradeType){

			ClearItemDetails ();

			switch (tradeType) {
			    case ItemOperationType.Buy:
					SetUpOperationButtons (true, false,false,false, item.price);
				    break;
			    case ItemOperationType.Sell:
					SetUpOperationButtons (false, true,false,false, item.price / 8);
				    break;
				case ItemOperationType.Add:
					SetUpOperationButtons (false, false,true,false, 0);
					break;
				case ItemOperationType.Remove:
					SetUpOperationButtons(false, false, false, true, 0);
					break;
				case ItemOperationType.None:
					SetUpOperationButtons(false, false, false, false, 0);
					break;
			}

			Sprite itemSprite = GameManager.Instance.gameDataCenter.GetGameItemSprite (item);

			itemIcon.sprite = itemSprite;

			itemIcon.enabled = true;

			itemIconBackground.enabled = true;

			itemName.text = item.itemName;

			switch(item.itemType){
    			case ItemType.Equipment:
    				Equipment eqp = item as Equipment;
    				switch (eqp.quality) {
    				case EquipmentQuality.Gray:
    					itemName.text = string.Format("<color=gray>{0}</color>",item.itemName);
    					break;
    				case EquipmentQuality.Blue:
    					itemName.text = string.Format("<color=blue>{0}</color>",item.itemName);
    					break;
    				case EquipmentQuality.Gold:
    					itemName.text = string.Format("<color=yellow>{0}</color>",item.itemName);
    					break;
    				}
    				itemDescription.text = item.itemDescription;
    				attachedDescription.text = eqp.attachedPropertyDescription;
					if (eqp.attachedPropertyGemstone != null) {
    					attachedSkillDisplay.gameObject.SetActive (true);
    					//Skill skill = GameManager.Instance.gameDataCenter.allSkills.Find (delegate (Skill obj) {
    					//	return obj.skillId == eqp.attachedSkillId;
    					//});
    					//attachedSkillDisplay.SetUpAttachedSkillDisplay (skill);
    				} else {
    					attachedSkillDisplay.gameObject.SetActive (false);
    				}
    				break;
    			case ItemType.Consumables:
    				attachedDescription.text = item.itemDescription;
    				attachedSkillDisplay.gameObject.SetActive (false);
    				break;
				case ItemType.PropertyGemstone:
					PropertyGemstone gemstone = item as PropertyGemstone;
    				//attachedSkillDisplay.gameObject.SetActive (true);
    				//Skill attachedSkill = GameManager.Instance.gameDataCenter.allSkills.Find (delegate(Skill obj) {
    				//	return obj.skillId == gemstone.skillId;
    				//});
    				//attachedDescription.text = item.itemDescription;
    				//attachedSkillDisplay.SetUpAttachedSkillDisplay (attachedSkill);
    				break;
    			case ItemType.SpellItem:
    				attachedDescription.text = item.itemDescription;
    				attachedSkillDisplay.gameObject.SetActive (false);
    				break;
			}

		}

		//public void AddButtonToRemoveButton(){
		//	SetUpOperationButtons(false, false, false, true, 0);
		//}

		public void SetUpOperationButtons(bool buyButtonEnable, bool sellButtonEnable, bool addButtonEnable, bool removeButtonEnable, int price)
		{

			buyButton.gameObject.SetActive(buyButtonEnable);
			sellButton.gameObject.SetActive(sellButtonEnable);
			addButton.gameObject.SetActive(addButtonEnable);
			removeButton.gameObject.SetActive(removeButtonEnable);

			if (buyButtonEnable)
			{
				buyButton.Find("Price").GetComponent<Text>().text = price.ToString();
			}

			if (sellButtonEnable)
			{
				sellButton.Find("Price").GetComponent<Text>().text = price.ToString();
			}

		}




		public void ClearItemDetails(){

			itemName.text = string.Empty;
			itemDescription.text = string.Empty;
			attachedDescription.text = string.Empty;
			itemIcon.sprite = null;
			itemIcon.enabled = false;
			itemIconBackground.enabled = false;

			SetUpOperationButtons (false, false,false,false, 0);

			attachedSkillDisplay.gameObject.SetActive(false);

		}


	}
}