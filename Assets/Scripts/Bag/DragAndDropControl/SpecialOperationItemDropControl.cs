using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.EventSystems;

	public enum ValidDropType{
		Equipment,
		EquipmentWithoutGemstone,
        PropertyGemstone
	}

	public class SpecialOperationItemDropControl : ItemDropControl {

		public ValidDropType validDropType;

		public SpecialOperationItemDragControl soDragControl;

	
		private Transform mBagCanvas;
		private Transform bagCanvas
        {
            get
            {
				if (mBagCanvas == null)
                {
					mBagCanvas = TransformManager.FindTransform("BagCanvas");
                }
				return mBagCanvas;
            }
        }

		protected override void OnUserPointerEnter (PointerEventData eventData)
		{

			if (!eventData.dragging) {
				return;
			}

			ItemDragControl dragControl = eventData.pointerDrag.GetComponent<ItemDragControl> ();
			Item draggedItem = GetDraggedItem (eventData);

			if (dragControl == null || draggedItem == null) {
				return;
			}

			bool isDraggingItemValid = CheckDraggedItemValid (draggedItem);

			if (isDraggingItemValid) {
				dragControl.detectReceiver = true;
				tintImage.enabled = true;
				tintImage.color = new Color (0f, 1f, 0f, 0.2f);
			} else {
				dragControl.detectReceiver = true;
				tintImage.enabled = true;
				tintImage.color = new Color (1f, 0f, 0f, 0.2f);
			}



		}

		private bool CheckDraggedItemValid(Item draggedItem){

			bool isValid = true;

			switch (validDropType) {
    			case ValidDropType.Equipment:
    				if (draggedItem.itemType != ItemType.Equipment) {
    					isValid = false;
    				}
    				break;
    			case ValidDropType.EquipmentWithoutGemstone:
    				if (draggedItem.itemType != ItemType.Equipment) {
    					isValid = false;
    				} else {
						Equipment equipment = draggedItem as Equipment;
						isValid = equipment.attachedPropertyGemstone.itemId == -1;
    				}
    				break;
    			case ValidDropType.PropertyGemstone:
    				if (draggedItem.itemType != ItemType.PropertyGemstone) {
    					isValid = false;
    				}
    				break;
			}
			return isValid;
		}

		protected override void OnUserPointerExit (PointerEventData eventData)
		{

			tintImage.enabled = false;

			if (!eventData.dragging) {
				return;
			}

			ItemDragControl dragControl = eventData.pointerDrag.GetComponent<ItemDragControl> ();

			if (dragControl == null) {
				return;
			}

			dragControl.detectReceiver = false;

			tintImage.enabled = false;
		}


		protected override void OnUserDrop (PointerEventData eventData)
		{

			GameObject draggedObject = GetDraggedObject (eventData);
			// 获取拖拽物体中的物品信息
			Item draggedItem = GetDraggedItem (eventData);

			// 如果拖拽游戏图中没有物品
			if (draggedItem == null) {
				SetDropResult (eventData, false);
				tintImage.enabled = false;
				return;
			}

			// 如果拖拽中的物品类型不对
			if (!CheckDraggedItemValid (draggedItem)) {
				SetDropResult (eventData, false);
				tintImage.enabled = false;
				return;
			}

			//bagCanvas.GetComponent<BagViewController>().itemForSpecialOperation = draggedItem;

			// 装备面板内的装备
			if (draggedObject.GetComponent<EquipedItemDragControl> () != null) {

				EquipedItemDragControl dc = draggedObject.GetComponent<EquipedItemDragControl> ();

				soDragControl.itemImage.sprite = dc.itemImage.sprite;
				soDragControl.itemImage.enabled = true;

				soDragControl.item = draggedItem;            

				SetDropResult (eventData, true);
			}

			//从背包内拖拽出得物品
			 else if (draggedObject.GetComponent<ItemInBagDragControl> () != null) {

				ItemInBagDragControl dc = draggedObject.GetComponent<ItemInBagDragControl> ();

				soDragControl.itemImage.sprite = dc.itemImage.sprite;
				soDragControl.itemImage.enabled = true;

				if (draggedItem.itemType == ItemType.Equipment) {
					soDragControl.item = draggedItem;
				} else {
					soDragControl.item = Item.NewItemWith (draggedItem.itemId, 1);
				}

				SetDropResult (eventData, true);
			}

            
            
			if(dropSucceedCallBack != null){
				dropSucceedCallBack(draggedItem);
			}

			tintImage.enabled = false;
		}

		private Item GetItem(PointerEventData data){
			return data.pointerDrag.GetComponent<ItemDragControl> ().item;
		}


	}
}
