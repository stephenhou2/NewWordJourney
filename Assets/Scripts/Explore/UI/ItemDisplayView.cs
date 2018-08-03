using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

	public class ItemDisplayView : ZoomHUD
    {
		public Image itemIconBackground;

		public Image itemIcon;

		public Text itemName;

		public Text itemDecription;

		public Text attachedItemDescription;

		public Sprite grayFrame;
		public Sprite blueFrame;
		public Sprite goldFrame;
		public Sprite purpleFrame;

		private Item rewardItem;

		public void SetUpItemDisplayView(Item item){

			this.rewardItem = item;

			this.gameObject.SetActive(true);

			itemIconBackground.sprite = grayFrame;

			if (item.itemType == ItemType.Equipment)
			{
				//itemIcon.sprite = GameManager.Instance.gameDataCenter.allEquipmentSprites.Find(delegate (Sprite obj)
				//{
				//    return obj.name == item.spriteName;
				//});

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

			Sprite itemSprite = GameManager.Instance.gameDataCenter.GetGameItemSprite(item);

			itemIcon.sprite = itemSprite;

			itemIcon.enabled = itemSprite != null;

			itemName.text = item.itemName;


			switch(item.itemType){
				case ItemType.Equipment:
					itemDecription.text = item.itemDescription;
					attachedItemDescription.text = (item as Equipment).attachedPropertyDescription;
					break;
				case ItemType.Consumables:
					attachedItemDescription.text = item.itemDescription;
					itemDecription.text = string.Empty;
					break;
				case ItemType.SkillScroll:
					attachedItemDescription.text = item.itemDescription;
                    itemDecription.text = string.Empty;
					break;
				case ItemType.PropertyGemstone:
					attachedItemDescription.text = item.itemDescription;
                    itemDecription.text = string.Empty;
					break;
				case ItemType.SpecialItem:
					attachedItemDescription.text = item.itemDescription;
                    itemDecription.text = string.Empty;
					break;
			}
            //itemDecription.text = item.itemDescription;

			if (zoomCoroutine != null)
            {
                StopCoroutine(zoomCoroutine);
            }

			zoomCoroutine = HUDZoomIn();

			StartCoroutine(zoomCoroutine);

		}

		public void OnConfirmButtonClick(){
         
			QuitItemDisplayView();

		}

		private void SetUpRewardInMap(){
			ExploreManager.Instance.newMapGenerator.SetUpRewardInMap(rewardItem, ExploreManager.Instance.battlePlayerCtr.transform.position);
			ExploreManager.Instance.MapWalkableEventsStartAction();
            ExploreManager.Instance.battlePlayerCtr.isInEvent = false;
		}

		private void QuitItemDisplayView(){
			
			if (inZoomingOut)
            {
                return;
            }

			if(zoomCoroutine != null){
				StopCoroutine(zoomCoroutine);
			}

			zoomCoroutine = HUDZoomOut(SetUpRewardInMap);

			StartCoroutine(zoomCoroutine);
         
		}

        
    }

}

