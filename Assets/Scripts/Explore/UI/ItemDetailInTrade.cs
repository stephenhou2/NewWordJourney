using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

	public enum TradeType{
		Buy,
		Sell,
		None
	}

	public class ItemDetailInTrade : MonoBehaviour {


		public Text itemName;
		public Text itemDescription;
		public Text attachedDescription;
		public Image itemIcon;
		public Image itemIconBackground;


		public Transform buyButton;
		public Transform sellButton;

		public AttachedSkillDisplay attachedSkillDisplay;

		public void SetUpItemDetail(Item item,TradeType tradeType){

			ClearItemDetails ();

			switch (tradeType) {
			case TradeType.Buy:
				SetUpTradeButtons (true, false, item.price);
				break;
			case TradeType.Sell:
				SetUpTradeButtons (false, true, item.price / 8);
				break;
			case TradeType.None:
				SetUpTradeButtons (false, false, 0);
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
				if (eqp.attachedSkillId > 0) {
					attachedSkillDisplay.gameObject.SetActive (true);
					Skill skill = GameManager.Instance.gameDataCenter.allSkills.Find (delegate (Skill obj) {
						return obj.skillId == eqp.attachedSkillId;
					});
					attachedSkillDisplay.SetUpAttachedSkillDisplay (skill);
				} else {
					attachedSkillDisplay.gameObject.SetActive (false);
				}
				break;
			case ItemType.Consumables:
				attachedDescription.text = item.itemDescription;
				attachedSkillDisplay.gameObject.SetActive (false);
				break;
			case ItemType.Gemstone:
				SkillGemstone gemstone = item as SkillGemstone;
				attachedSkillDisplay.gameObject.SetActive (true);
				Skill attachedSkill = GameManager.Instance.gameDataCenter.allSkills.Find (delegate(Skill obj) {
					return obj.skillId == gemstone.skillId;
				});
				attachedDescription.text = item.itemDescription;
				attachedSkillDisplay.SetUpAttachedSkillDisplay (attachedSkill);
				break;
			case ItemType.Task:
				attachedDescription.text = item.itemDescription;
				attachedSkillDisplay.gameObject.SetActive (false);
				break;
			}

		}


		private void SetUpTradeButtons(bool buyButtonEnable,bool sellButtonEnable,int price){

			buyButton.gameObject.SetActive (buyButtonEnable);
			sellButton.gameObject.SetActive (sellButtonEnable);

			if (buyButtonEnable) {
				buyButton.Find ("Price").GetComponent<Text> ().text = price.ToString ();
			} 

			if (sellButtonEnable) {
				sellButton.Find ("Price").GetComponent<Text> ().text = price.ToString ();
			}
				
		}




		public void ClearItemDetails(){

			itemName.text = string.Empty;
			itemDescription.text = string.Empty;
			attachedDescription.text = string.Empty;
			itemIcon.sprite = null;
			itemIcon.enabled = false;
			itemIconBackground.enabled = false;

			SetUpTradeButtons (false, false, 0);

			attachedSkillDisplay.gameObject.SetActive(false);

		}


	}
}