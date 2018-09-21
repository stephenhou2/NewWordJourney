using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

	using UnityEngine.UI;

	public class EquipmentCellInRecord : MonoBehaviour
    {

		public Image equipmentBackground;

		public Image equipmentIcon;

		public Text equipmentName;

		public Sprite emptySlotSprite;

		public Sprite graySlotSprite;

		public Sprite blueSlotSprite;

		public Sprite goldSlotSprite;

		public Sprite purpleSlotSprite;

		public void Reset()
		{
			equipmentBackground.sprite = emptySlotSprite;
			equipmentIcon.enabled = false;
			equipmentName.text = string.Empty;
		}

		public void SetUpEquipmentCellInRecord(Equipment equipment){

			switch(equipment.quality){
				case EquipmentQuality.Gray:
					equipmentBackground.sprite = graySlotSprite;
					break;
				case EquipmentQuality.Blue:
					equipmentBackground.sprite = blueSlotSprite;
					break;
				case EquipmentQuality.Gold:
					equipmentBackground.sprite = goldSlotSprite;
					break;
				case EquipmentQuality.Purple:
					equipmentBackground.sprite = purpleSlotSprite;
					break;
			}

			Sprite equipmentSprite = GameManager.Instance.gameDataCenter.GetGameItemSprite(equipment);
                     
			if(equipmentSprite != null){
				equipmentIcon.enabled = true;
				equipmentIcon.sprite = equipmentSprite;
			}else{
				equipmentIcon.enabled = false;
			}

			equipmentName.text = equipment.itemName;
			equipmentName.enabled = true;


		}


    }

}

