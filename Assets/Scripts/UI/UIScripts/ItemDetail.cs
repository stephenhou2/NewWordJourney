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

		public Text itemName;
		public Text itemDescription;
		public Text attachedDescription;
		public Image itemIcon;
		public Image itemIconBackground;
		public Image goldIcon;
		public Text priceText;

		public Transform equipButton;
		public Transform unloadButton;
		public Transform useButton;

		public SpecialOperationCell soCell;

		public AttachedGemstoneDisplay attachedSkillDisplay;
      
		public void SetUpItemDetail(Item item){

			ClearItemDetails ();

			Sprite itemSprite = GameManager.Instance.gameDataCenter.GetGameItemSprite (item);

			itemIcon.sprite = itemSprite;

			itemIcon.enabled = true;

			itemIconBackground.enabled = true;

			itemName.text = item.itemName;

			goldIcon.enabled = true;

			priceText.text = (item.price / 8).ToString ();

			switch(item.itemType){
    			case ItemType.Equipment:
    				Equipment eqp = item as Equipment;
    				itemDescription.text = item.itemDescription;
    				attachedDescription.text = eqp.attachedPropertyDescription;
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
    				SetUpOperationButtons (!eqp.equiped, eqp.equiped, false);
    				soCell.gameObject.SetActive (false);

    				//if (eqp.attachedSkillId > 0) {
    				//	attachedSkillDisplay.gameObject.SetActive (true);
    				//	Skill skill = GameManager.Instance.gameDataCenter.allSkills.Find (delegate (Skill obj) {
    				//		return obj.skillId == eqp.attachedSkillId;
    				//	});
    				//	attachedSkillDisplay.SetUpAttachedSkillDisplay (skill);
    				//} else {
    				//	attachedSkillDisplay.gameObject.SetActive (false);
    				//}
    				break;
    			case ItemType.Consumables:
    				Consumables cons = item as Consumables;
    				attachedDescription.text = item.itemDescription;
    				SetUpOperationButtons (false, false, true);
    				attachedSkillDisplay.gameObject.SetActive (false);
    				break;
				case ItemType.SkillScroll:
					SkillScroll skillScroll = item as SkillScroll;
    				attachedDescription.text = item.itemDescription;
    				attachedSkillDisplay.gameObject.SetActive (true);
    				Skill attachedSkill = GameManager.Instance.gameDataCenter.allSkills.Find (delegate(Skill obj) {
						return obj.skillId == skillScroll.skillId;
    				});
    				attachedSkillDisplay.SetUpAttachedSkillDisplay (attachedSkill);
    				SetUpOperationButtons (false, false, false);
    				soCell.gameObject.SetActive (false);
    				break;
				case ItemType.PropertyGemstone:
					SetUpOperationButtons(false, false, false);
					break;
				case ItemType.SpecialItem:
    				attachedDescription.text = item.itemDescription;
    				soCell.gameObject.SetActive (false);
    				attachedSkillDisplay.gameObject.SetActive (false);
					SpecialItem specialItem = item as SpecialItem;
					switch(specialItem.specialItemType){
						case SpecialItemType.TieYaoShi:
						case SpecialItemType.TongYaoShi:
						case SpecialItemType.JinYaoShi:
						case SpecialItemType.WanNengYaoShi:
						case SpecialItemType.QiaoZhen:
							SetUpOperationButtons(false, false, false);
							break;
						default:
							SetUpOperationButtons(false, false, true);
							break;

					}
    				
    				break;

			}

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

			soCell.ResetSpecialOperationCell ();
			soCell.gameObject.SetActive(false);

			attachedSkillDisplay.gameObject.SetActive(false);

		}

//		public Equipment SpecialOperationOnEquipment(SpecialOperation so,int attachedInfo,out int oriAttachedSkillId){
         
//			Equipment equipment = soCell.soDragControl.item as Equipment;

//			oriAttachedSkillId = equipment.attachedSkillId + 400;

//			if (equipment == null) {
//				return null;
//			}

//			switch (so) {
//			case SpecialOperation.ChongZhu:
//				equipment.RebuildEquipment ();

//				break;
//			case SpecialOperation.DianJin:
//				equipment.SetToGoldQuality ();

//				break;
//			case SpecialOperation.XiaoMo:
//				//equipment.RemoveAttachedSkill();               
//				break;
//			case SpecialOperation.XiangQianJiNeng:
//				//equipment.AddSkill (attachedInfo);

//				break;
//			}

          
            
//			if(equipment.equiped){
//				PropertyChange propertyChange = Player.mainPlayer.ResetBattleAgentProperties(false);
//				refreshCallBack(propertyChange);
//			}

////			Player.mainPlayer.AddItem (equipment);

		//	soCell.ResetSpecialOperationCell ();

		//	return equipment;

		//}



	}
}
