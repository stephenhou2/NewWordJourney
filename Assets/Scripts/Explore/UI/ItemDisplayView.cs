using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

	public class ItemDisplayView : ZoomHUD
    {

		public Image itemIcon;

		public Text itemName;

		public Text itemDecription;

		public void SetUpItemDisplayView(Item item){

			this.gameObject.SetActive(true);

			if(item.itemType == ItemType.Equipment){            
				itemIcon.sprite = GameManager.Instance.gameDataCenter.allEquipmentSprites.Find(delegate (Sprite obj)
                {
                    return obj.name == item.spriteName;
                });

			}else if(item.itemType == ItemType.Consumables){
				itemIcon.sprite = GameManager.Instance.gameDataCenter.allConsumablesSprites.Find(delegate (Sprite obj)
				{
					return obj.name == item.spriteName;
				});
			}else if(item.itemType == ItemType.PropertyGemstone){
				itemIcon.sprite = GameManager.Instance.gameDataCenter.allPropertyGemstoneSprites.Find(delegate (Sprite obj)
				{
					return obj.name == item.spriteName;
				});

			}else if(item.itemType == ItemType.SkillScroll){
				itemIcon.sprite = GameManager.Instance.gameDataCenter.allSkillScrollSprites.Find(delegate (Sprite obj)
				{
					return obj.name == item.spriteName;
				});
			}else if(item.itemType == ItemType.SpecialItem){            
				itemIcon.sprite = GameManager.Instance.gameDataCenter.allSpecialItemSprites.Find(delegate (Sprite obj)
                {
                    return obj.name == item.spriteName;
                });            
			}

			itemName.text = item.itemName;

            itemDecription.text = item.itemDescription;

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

		private void QuitItemDisplayView(){

			if(zoomCoroutine != null){
				StopCoroutine(zoomCoroutine);
			}

			zoomCoroutine = HUDZoomOut();

			StartCoroutine(zoomCoroutine);

		}

        
    }

}

