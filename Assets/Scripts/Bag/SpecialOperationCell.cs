using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	using UnityEngine.UI;

	public class SpecialOperationCell : MonoBehaviour {

		public SpecialOperationItemDragControl soDragControl;

		public SpecialOperationItemDropControl soDropControl;
			
        
		public void InitCell(Item item,CallBackWithItem itemClickCallBack){
			
			soDragControl.item = item;
            
			soDragControl.InitItemDragControl(item, itemClickCallBack);

			soDropControl.InitItemDropCallBack(itemClickCallBack);

			if (item != null)
            {
                soDragControl.itemImage.sprite = GameManager.Instance.gameDataCenter.GetGameItemSprite(item);
                soDragControl.itemImage.enabled = true;
                soDropControl.tintImage.enabled = false;
            }
		}
	
		public void ResetSpecialOperationCell(){

			soDragControl.itemImage.sprite = null;
			soDragControl.itemImage.enabled = false;

			soDragControl.item = null;

			soDropControl.tintImage.enabled = false;

		}


	}
}
