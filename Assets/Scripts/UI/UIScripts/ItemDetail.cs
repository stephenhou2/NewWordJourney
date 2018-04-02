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

	public class ItemDetail : MonoBehaviour {

		public Text itemName;
		public Text itemDescription;
		public Text attachedDescription;
		public Image itemIcon;
		public Image itemIconBackground;

		public Transform equipButton;
		public Transform unloadButton;
		public Transform useButton;

		public SpecialOperationCell soCell;

		public AttachedSkillDisplay attachedSkillDisplay;



		public void SetUpItemDetail(Item item,CallBack dropCallBack = null){

			ClearItemDetails ();

			Sprite itemSprite = GameManager.Instance.gameDataCenter.GetGameItemSprite (item);

			itemIcon.sprite = itemSprite;

			itemIcon.enabled = true;

			itemIconBackground.enabled = true;

			itemName.text = item.itemName;

			itemDescription.text = item.itemDescription;

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

				SetUpOperationButtons (!eqp.equiped, eqp.equiped, false);
				soCell.gameObject.SetActive (false);
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
				Consumables cons = item as Consumables;
				SetUpOperationButtons (false, false, true);
				attachedSkillDisplay.gameObject.SetActive (false);
				switch (cons.type) {
				case ConsumablesType.ShuXingTiSheng:
				case ConsumablesType.YinShenJuanZhou:
					soCell.gameObject.SetActive (false);
					break;
				case ConsumablesType.ChongZhuShi:
				case ConsumablesType.DianJinShi:
				case ConsumablesType.XiaoMoJuanZhou:
					soCell.gameObject.SetActive (true);
					soCell.InitSpecialOperationCell (dropCallBack);
					break;
				}
				break;
			case ItemType.Gemstone:
				SkillGemstone gemstone = item as SkillGemstone;
				itemDescription.text = item.itemDescription;
				attachedSkillDisplay.gameObject.SetActive (true);
				Skill attachedSkill = GameManager.Instance.gameDataCenter.allSkills.Find (delegate(Skill obj) {
					return obj.skillId == gemstone.skillId;
				});
				attachedSkillDisplay.SetUpAttachedSkillDisplay (attachedSkill);
				SetUpOperationButtons (false, false, false);
				soCell.gameObject.SetActive (false);
				break;
			case ItemType.Task:
				itemDescription.text = item.itemDescription;
				soCell.gameObject.SetActive (false);
				attachedSkillDisplay.gameObject.SetActive (false);
				SetUpOperationButtons (false, false, false);
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

			SetUpOperationButtons (false, false, false);

			soCell.gameObject.SetActive(false);

			attachedSkillDisplay.gameObject.SetActive(false);

		}

		public Equipment SpecialOperationOnEquipment(SpecialOperation so,int attachedInfo){

			Equipment equipment = soCell.soDragControl.item as Equipment;

			switch (so) {
			case SpecialOperation.ChongZhu:
				equipment.RebuildEquipment ();
				break;
			case SpecialOperation.DianJin:
				equipment.SetToGoldQuality ();
				break;
			case SpecialOperation.XiaoMo:
				equipment.RemoveAttachedSkill ();
				break;
			case SpecialOperation.XiangQianJiNeng:
				equipment.AddSkill (attachedInfo);
				break;
			}

			Player.mainPlayer.AddItem (equipment);

			soCell.ResetSpecialOperationCell ();

			return equipment;

		}



	}
}
