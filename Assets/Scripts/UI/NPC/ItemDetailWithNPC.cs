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

		public Transform itemDetailContainer;

		public Text itemName;
		public Text itemDescription;
		public Text attachedDescription;
		public Image itemIcon;
		public Image itemIconBackground;


		public Transform buyButton;
		public Transform sellButton;
		public Transform addButton;
		public Transform removeButton;

		public Sprite grayFrame;
		public Sprite blueFrame;
		public Sprite goldFrame;
		public Sprite purpleFrame;

		public Image goldIcon;
		public Text priceText;

		public AttachedGemstoneDisplay attachedGemstoneDisplay;
		//public AttachedSkillDisplay attachedSkillDisplay;

		public void SetUpItemDetail(Item item,int price, ItemOperationType tradeType){

			ClearItemDetails ();

			if(item == null){
				return;
			}

			itemDetailContainer.gameObject.SetActive(true);

			switch (tradeType) {
			    case ItemOperationType.Buy:
					SetUpOperationButtons (true, false,false,false, price);
				    break;
			    case ItemOperationType.Sell:
					SetUpOperationButtons (false, true,false,false, price / 8);
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

			itemIconBackground.sprite = grayFrame;
			itemIconBackground.enabled = true;

			itemName.text = item.itemName;
			itemName.color = CommonData.grayEquipmentColor;

			switch(item.itemType){
    			case ItemType.Equipment:
    				Equipment eqp = item as Equipment;
    				switch (eqp.quality) {
        				case EquipmentQuality.Gray:
							itemName.color = CommonData.grayEquipmentColor;
							itemIconBackground.sprite = grayFrame;
        					break;
        				case EquipmentQuality.Blue:
							itemName.color = CommonData.blueEquipmentColor;
							itemIconBackground.sprite = blueFrame;
        					break;
        				case EquipmentQuality.Gold:
							itemName.color = CommonData.goldEquipmentColor;
							itemIconBackground.sprite = goldFrame;
        					break;
						case EquipmentQuality.Purple:
							itemName.color = CommonData.purpleEquipmentColor;
							itemIconBackground.sprite = purpleFrame;
							break;

    				}
    				itemDescription.text = item.itemDescription;

    				attachedDescription.text = eqp.attachedPropertyDescription;

					//attachedSkillDisplay.gameObject.SetActive(false);

					if (eqp.attachedPropertyGemstone.itemId != -1)
                    {
                        attachedGemstoneDisplay.SetUpAttachedSkillDisplay(eqp.attachedPropertyGemstone);
                    }
                    else
                    {
                        attachedGemstoneDisplay.gameObject.SetActive(false);
                    }
    				break;
    			case ItemType.Consumables:
    				attachedDescription.text = item.itemDescription;
    				//attachedSkillDisplay.gameObject.SetActive (false);
                    attachedGemstoneDisplay.gameObject.SetActive(false);
    				break;
				case ItemType.SkillScroll:
                    //SkillScroll skillScroll = item as SkillScroll;
					attachedDescription.text = item.itemDescription;
					itemDescription.text = string.Empty;
                    //attachedSkillDisplay.gameObject.SetActive(true);
                    //Skill attachedSkill = GameManager.Instance.gameDataCenter.allSkills.Find(delegate (Skill obj) {
                    //    return obj.skillId == skillScroll.skillId;
                    //});
                    //attachedSkillDisplay.SetUpAttachedSkillDisplay(attachedSkill);
                    attachedGemstoneDisplay.gameObject.SetActive(false);
                    break;
                case ItemType.PropertyGemstone:
					attachedDescription.text = item.itemDescription;
					itemDescription.text = string.Empty;
                    //attachedSkillDisplay.gameObject.SetActive(false);
                    attachedGemstoneDisplay.gameObject.SetActive(false);
                    break;
                case ItemType.SpecialItem:
                    attachedDescription.text = item.itemDescription;
                    //attachedSkillDisplay.gameObject.SetActive(false);
                    attachedGemstoneDisplay.gameObject.SetActive(false);
                    break;

			}

		}


		public void SetUpOperationButtons(bool buyButtonEnable, bool sellButtonEnable, bool addButtonEnable, bool removeButtonEnable, int price)
		{

			buyButton.gameObject.SetActive(buyButtonEnable);
			sellButton.gameObject.SetActive(sellButtonEnable);
			addButton.gameObject.SetActive(addButtonEnable);
			removeButton.gameObject.SetActive(removeButtonEnable);

			if (buyButtonEnable || sellButtonEnable)
			{
				priceText.text = price.ToString();
				goldIcon.enabled = true;
			}
			else
			{
				priceText.text = string.Empty;
				goldIcon.enabled = false;
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

			attachedGemstoneDisplay.gameObject.SetActive(false);
			//attachedSkillDisplay.gameObject.SetActive(false);

			itemDetailContainer.gameObject.SetActive(false);
                              

		}


	}
}