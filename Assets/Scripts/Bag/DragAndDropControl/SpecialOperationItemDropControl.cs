using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.EventSystems;

	public enum ValidDropType{
		Equipment,
		EquipmentWithoutSkill,
		Consumables,
		SkillGemstone
	}

	public class SpecialOperationItemDropControl : ItemDropControl {

		public ValidDropType validDropType;

		public SpecialOperationItemDragControl soDragControl;

//		private CallBack dropCallBack;
//
//		public void InitSpecialOperationDropControl(CallBack dropCallBack){
//			this.dropCallBack = dropCallBack;
//		}


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
			case ValidDropType.EquipmentWithoutSkill:
				if (draggedItem.itemType != ItemType.Equipment) {
					isValid = false;
				} else {
					isValid = (draggedItem as Equipment).attachedSkillId == 0;
				}
				break;
			case ValidDropType.Consumables:
				if (draggedItem.itemType != ItemType.Consumables
					|| ((draggedItem as Consumables).type != ConsumablesType.ChongZhuShi
						&& (draggedItem as Consumables).type != ConsumablesType.DianJinShi
						&& (draggedItem as Consumables).type != ConsumablesType.XiaoMoJuanZhou)) {
					isValid = false;
				}
				break;
			case ValidDropType.SkillGemstone:
				if (draggedItem.itemType != ItemType.Gemstone) {
					isValid = false;
				}
				break;
			}
			return isValid;
		}

		protected override void OnUserPointerExit (PointerEventData eventData)
		{
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

			// 装备面板内的装备不能直接进行特殊操作
			if (draggedObject.GetComponent<EquipedItemDragControl> () != null) {
				SetDropResult (eventData, false);
				tintImage.enabled = false;
				return;
			}

			//从背包内拖拽出得物品可以进行特殊操作
			if (draggedObject.GetComponent<ItemInBagDragControl> () != null) {

				ItemInBagDragControl dc = draggedObject.GetComponent<ItemInBagDragControl> ();

				soDragControl.itemImage.sprite = dc.itemImage.sprite;
				soDragControl.itemImage.enabled = true;


//				int oriIndexInBag = Player.mainPlayer.GetItemIndexInBag (draggedItem);

//				if (soDragControl.item != null) {
//					Player.mainPlayer.AddItem (soDragControl.item, oriIndexInBag);
//				}

//				Player.mainPlayer.RemoveItem (draggedItem,1);
//
//				dropCallBack ();

				if (draggedItem.itemType == ItemType.Equipment) {
					soDragControl.item = draggedItem;
				} else {
					soDragControl.item = Item.NewItemWith (draggedItem.itemId, 1);
				}

				SetDropResult (eventData, true);
			}

			tintImage.enabled = false;
		}

		private Item GetItem(PointerEventData data){
			return data.pointerDrag.GetComponent<ItemDragControl> ().item;
		}


	}
}
