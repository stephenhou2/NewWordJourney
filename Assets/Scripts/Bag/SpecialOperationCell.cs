using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	using UnityEngine.UI;

	public class SpecialOperationCell : MonoBehaviour {

		public SpecialOperationItemDragControl soDragControl;

		public SpecialOperationItemDropControl soDropControl;

		public Item itemInCell;

		public Image itemIconBackground;

        public Sprite grayFrame;
        public Sprite blueFrame;
        public Sprite goldFrame;
        public Sprite purpleFrame;
        
		public void InitSpecialOperaiton(CallBackWithItem itemClickCallBack){
	
			soDragControl.InitItemDragControl(null, itemClickCallBack);

			soDropControl.InitItemDropCallBack(ItemDropSucceedCallBack);         
		}

		private void ItemDropSucceedCallBack(Item item){
			itemInCell = item;
			if (item is Equipment)
            {

                Equipment equipment = item as Equipment;

                switch (equipment.quality)
                {
                    case EquipmentQuality.Gray:
                        itemIconBackground.sprite = grayFrame;
                        break;
                    case EquipmentQuality.Blue:
                        itemIconBackground.sprite = blueFrame;
                        break;
                    case EquipmentQuality.Gold:
                        itemIconBackground.sprite = goldFrame;
                        break;
                    case EquipmentQuality.Purple:
                        itemIconBackground.sprite = purpleFrame;
                        break;
                }
            }

		}

		public void SetUpSpeicalOperationCell(Item item){

			soDragControl.item = item;

            itemInCell = item;

			itemIconBackground.sprite = grayFrame;

			if(item == null){
				return;
			}

			if(item is Equipment){
				
				Equipment equipment = item as Equipment;

				switch(equipment.quality){
					case EquipmentQuality.Gray:
						itemIconBackground.sprite = grayFrame;
						break;
					case EquipmentQuality.Blue:
						itemIconBackground.sprite = blueFrame;
						break;
					case EquipmentQuality.Gold:
						itemIconBackground.sprite = goldFrame;
						break;
					case EquipmentQuality.Purple:
						itemIconBackground.sprite = purpleFrame;
						break;
				}
			}

			if (item != null)
            {
                soDragControl.itemImage.sprite = GameManager.Instance.gameDataCenter.GetGameItemSprite(item);
                soDragControl.itemImage.enabled = true;
                //soDropControl.tintImage.enabled = false;
            }
		}
	
		public void ResetSpecialOperationCell(){

			itemIconBackground.sprite = grayFrame;

			soDragControl.itemImage.sprite = null;
			soDragControl.itemImage.enabled = false;

			soDragControl.item = null;
			itemInCell = null;

			//soDropControl.tintImage.enabled = false;

		}


	}
}
