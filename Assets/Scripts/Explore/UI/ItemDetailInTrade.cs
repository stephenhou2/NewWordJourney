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
		public Image goldIcon;
		public Text priceText;


		public Transform buyButton;
		public Transform sellButton;

		public AttachedSkillDisplay attachedSkillDisplay;

		public void SetUpItemDetail(Item item,TradeType tradeType){

			ClearItemDetails ();

			switch (tradeType) {
			case TradeType.Buy:
				SetUpTradeButtons (true, false);
				goldIcon.enabled = true;
				priceText.text = item.price.ToString ();
				break;
			case TradeType.Sell:
				SetUpTradeButtons (false, true);
				goldIcon.enabled = true;
				priceText.text = (item.price / 8).ToString ();
				break;
			case TradeType.None:
				SetUpTradeButtons (false, false);
				goldIcon.enabled = false;
				priceText.text = string.Empty;
				break;
			}

			Sprite itemSprite = GameManager.Instance.gameDataCenter.GetGameItemSprite (item);

			itemIcon.sprite = itemSprite;

			itemIcon.enabled = true;

			itemIconBackground.enabled = true;

			itemName.text = item.itemName;

			itemDescription.text = item.itemDescription;

			goldIcon.enabled = true;

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
				attachedSkillDisplay.gameObject.SetActive (false);
				break;
			case ItemType.Gemstone:
				SkillGemstone gemstone = item as SkillGemstone;
				attachedSkillDisplay.gameObject.SetActive (true);
				Skill attachedSkill = GameManager.Instance.gameDataCenter.allSkills.Find (delegate(Skill obj) {
					return obj.skillId == gemstone.skillId;
				});
				attachedSkillDisplay.SetUpAttachedSkillDisplay (attachedSkill);
				break;
			case ItemType.Task:
				attachedSkillDisplay.gameObject.SetActive (false);
				break;
			}

		}


		private void SetUpTradeButtons(bool buyButtonEnable,bool sellButtonEnable){
			buyButton.gameObject.SetActive (buyButtonEnable);
			sellButton.gameObject.SetActive (sellButtonEnable);
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

			SetUpTradeButtons (false, false);

			attachedSkillDisplay.gameObject.SetActive(false);

		}


	}
}