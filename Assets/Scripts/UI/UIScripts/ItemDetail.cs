using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

	public enum SpecialOperation{
		ChongZhu,
		DianJin,
		XiaoMo,
		XiangQianJiNeng
	}

	public delegate void ItemChangeCallBack(PropertyChange propertyChange); 

	public class ItemDetail : MonoBehaviour {

		public Transform generalItemDetailContainer;
		public Text itemName;
		public Text itemDescription;
		public Text attachedDescription;
		public Image itemIcon;
		public Image itemIconBackground;
		public Image goldIcon;
		public Text priceText;
		public Text specialOperationHint;

		public Transform equipButton;
		public Transform unloadButton;
		public Transform useButton;

		public Transform specialOperationContainer;
		public SpecialOperationCell soCell;

		public AttachedGemstoneDisplay attachedGemstoneDisplay;
		//public AttachedSkillDisplay attachedSkillDisplay;

		public Sprite grayEquipmentFrame;
		public Sprite blueEquipmentFrame;
		public Sprite goldEquipmentFrame;
		public Sprite purpleEquipmentFrame;
      
		public void SetUpItemDetail(Item item){

			ClearItemDetails ();

			Sprite itemSprite = GameManager.Instance.gameDataCenter.GetGameItemSprite (item);

			itemIcon.sprite = itemSprite;

			itemIcon.enabled = true;

			itemIconBackground.enabled = true;

			itemName.text = item.itemName;

			itemIconBackground.sprite = grayEquipmentFrame;
			itemName.color = CommonData.grayEquipmentColor;

			goldIcon.enabled = true;

			priceText.text = (item.price / 8).ToString ();

			switch(item.itemType){
    			case ItemType.Equipment:
    				Equipment eqp = item as Equipment;
    				itemDescription.text = item.itemDescription;
					specialOperationHint.enabled = false;
    				attachedDescription.text = eqp.attachedPropertyDescription;
    				switch (eqp.quality) {
        				case EquipmentQuality.Gray:
    						itemName.color = CommonData.grayEquipmentColor;
							itemIconBackground.sprite = grayEquipmentFrame;
        					break;
        				case EquipmentQuality.Blue:
    						itemName.color = CommonData.blueEquipmentColor;
							itemIconBackground.sprite = blueEquipmentFrame;
        					break;
        				case EquipmentQuality.Gold:
    						itemName.color = CommonData.goldEquipmentColor;
							itemIconBackground.sprite = goldEquipmentFrame;
        				    break;
    					case EquipmentQuality.Purple:
    						itemName.color = CommonData.purpleEquipmentColor;
							itemIconBackground.sprite = purpleEquipmentFrame;
    						break;
    				}
    				SetUpOperationButtons (!eqp.equiped, eqp.equiped, false);

					specialOperationContainer.gameObject.SetActive (false);
           
					attachedGemstoneDisplay.SetUpAttachedSkillDisplay(eqp.attachedPropertyGemstones);
				
					attachedGemstoneDisplay.gameObject.SetActive(true);
    				break;
    			case ItemType.Consumables:
    				//Consumables cons = item as Consumables;
    				attachedDescription.text = item.itemDescription;
    				SetUpOperationButtons (false, false, true);
					specialOperationContainer.gameObject.SetActive(false);
					specialOperationHint.enabled = false;
    				//attachedSkillDisplay.gameObject.SetActive (false);
					attachedGemstoneDisplay.gameObject.SetActive(false);
    				break;
				case ItemType.SkillScroll:
					//SkillScroll skillScroll = item as SkillScroll;
					attachedDescription.text = item.itemDescription;
					itemDescription.text = string.Empty;
    				//attachedSkillDisplay.gameObject.SetActive (true);
    		//		Skill attachedSkill = GameManager.Instance.gameDataCenter.allSkills.Find (delegate(Skill obj) {
						//return obj.skillId == skillScroll.skillId;
    				//});
					SetUpOperationButtons (false, false, true);
					specialOperationContainer.gameObject.SetActive (false);
					specialOperationHint.enabled = false;
					//attachedSkillDisplay.SetUpAttachedSkillDisplay(attachedSkill);
					attachedGemstoneDisplay.gameObject.SetActive(false);
    				break;
				case ItemType.PropertyGemstone:
					attachedDescription.text = item.itemDescription;
					itemDescription.text = string.Empty;
					SetUpOperationButtons(false, false, false);
					specialOperationContainer.gameObject.SetActive(false);
					specialOperationHint.enabled = false;
					//attachedSkillDisplay.gameObject.SetActive(false);
					attachedGemstoneDisplay.gameObject.SetActive(false);
					break;
				case ItemType.SpecialItem:
    				attachedDescription.text = item.itemDescription;
					specialOperationHint.enabled = false;
					SpecialItem specialItem = item as SpecialItem;
					switch(specialItem.specialItemType){
						case SpecialItemType.TieYaoShi:
						case SpecialItemType.TongYaoShi:
						case SpecialItemType.JinYaoShi:
						case SpecialItemType.WanNengYaoShi:
						case SpecialItemType.QiaoZhen:
							SetUpOperationButtons(false, false, false);
							specialOperationContainer.gameObject.SetActive(false);
							break;
						case SpecialItemType.DianJinFuShi:
						case SpecialItemType.ChongZhuShi:
						case SpecialItemType.TuiMoJuanZhou:
							SetUpOperationButtons(false, false, true);
							specialOperationContainer.gameObject.SetActive(true);
							soCell.SetUpSpeicalOperationCell(null);
							soCell.InitSpecialOperaiton(null);
							specialOperationHint.enabled = true;
							break;
						default:
							SetUpOperationButtons(false, false, true);
							specialOperationContainer.gameObject.SetActive(false);
							break;

					}
					//attachedSkillDisplay.gameObject.SetActive(false);
                    attachedGemstoneDisplay.gameObject.SetActive(false);
    				break;

			}

			generalItemDetailContainer.gameObject.SetActive(true);

		}

		public void SetUpOperationButtons(bool equipButtonEnable,bool unloadButtonEnable,bool useButtonEnable){
			equipButton.gameObject.SetActive (equipButtonEnable);
			unloadButton.gameObject.SetActive (unloadButtonEnable);
			useButton.gameObject.SetActive (useButtonEnable);
		}


		public void ClearItemDetails(){

			itemName.text = string.Empty;
			itemDescription.text = string.Empty;
			attachedDescription.text = string.Empty;
			itemIcon.sprite = null;
			itemIcon.enabled = false;
			itemIconBackground.enabled = false;
			goldIcon.enabled = false;
			priceText.text = string.Empty;

			SetUpOperationButtons (false, false, false);

			specialOperationContainer.gameObject.SetActive(false);
			soCell.ResetSpecialOperationCell ();
			//soCell.gameObject.SetActive(false);

			generalItemDetailContainer.gameObject.SetActive(false);
			attachedGemstoneDisplay.gameObject.SetActive(false);
			//attachedSkillDisplay.gameObject.SetActive(false);

		}
      
      
	}
}
