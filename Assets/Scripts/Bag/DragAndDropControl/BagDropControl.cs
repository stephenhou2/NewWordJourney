using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;
	using UnityEngine.EventSystems;

	public class BagDropControl : ItemDropControl {

		private BagView mBagView;
		private BagView bagView{
			get{
				if (mBagView == null) {
					mBagView = TransformManager.FindInParents<BagView> (this.gameObject);
				}
				return mBagView;
			}
		}

		protected override void OnUserPointerEnter (PointerEventData eventData)
		{
			
			if (!eventData.dragging) {
				return;
			}

			ItemDragControl dragControl = eventData.pointerDrag.GetComponent<ItemDragControl> ();
			Item draggedItem = GetDraggedItem (eventData);

			if (dragControl == null || draggedItem == null || draggedItem.itemId < 0) {
				return;
			}

			dragControl.detectReceiver = true;

			tintImage.enabled = true;
			tintImage.color = new Color (0f, 1f, 0f, 0.2f);
		
		}

		protected override void OnUserPointerExit (PointerEventData eventData)
		{
			if (!eventData.dragging) {
				return;
			}

			ItemDragControl dragControl = eventData.pointerDrag.GetComponent<ItemDragControl> ();
//			Item draggedItem = GetDraggedItem (eventData);

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
			if (draggedItem == null || draggedItem.itemId < 0) {
				SetDropResult (eventData, false);
				tintImage.enabled = false;
				return;
			}

			// 如果是从背包中拖拽出来的物品
			if (draggedObject.GetComponent<ItemInBagDragControl> () != null) {
				SetDropResult (eventData, false);
				tintImage.enabled = false;
				return;
			}

			// 如果是从已装备面板拖拽过来的物品
			if (draggedObject.GetComponent<EquipedItemDragControl>() != null) {

				if (Player.mainPlayer.CheckBagFull (draggedItem)) {
					SetDropResult (eventData, false);
					tintImage.enabled = false;
					bagView.SetUpTintHUD ("背包已满",null);
					return;
				}
	
//				EquipedItemDragControl equipmentDragControl = draggedObject.GetComponent<EquipedItemDragControl> ();

				Equipment equipment = draggedItem as Equipment;

				int equipmentIndexInPanel = Player.mainPlayer.GetEquipmentIndexInPanel (equipment);

				PropertyChange propertyChange = Player.mainPlayer.UnloadEquipment (equipment, equipmentIndexInPanel);

				bagView.SetUpEquipedEquipmentsPlane ();

				if (equipment.itemId >= 0) {
					bagView.AddBagItem (draggedItem);
					bagView.SetUpEquipedEquipmentsPlane ();
					bagView.SetUpPlayerStatusPlane ();
					SetDropResult (eventData, true);
				} else {
					SetDropResult (eventData, false);
				}

			}

			// 如果是从已装备面板拖拽过来的物品
			if (draggedObject.GetComponent<SpecialOperationItemDragControl> () != null) {

				if (Player.mainPlayer.CheckBagFull (draggedItem)) {
					SetDropResult (eventData, false);
					tintImage.enabled = false;
					bagView.SetUpTintHUD ("背包已满",null);
					return;
				}

				Equipment equipment = draggedItem as Equipment;

				int equipmentIndexInPanel = Player.mainPlayer.GetEquipmentIndexInPanel (equipment);

				PropertyChange propertyChange = Player.mainPlayer.UnloadEquipment (equipment, equipmentIndexInPanel);

				bagView.SetUpEquipedEquipmentsPlane ();

				if (equipment.itemId >= 0) {
					bagView.AddBagItem (draggedItem);
					bagView.SetUpEquipedEquipmentsPlane ();
					bagView.SetUpPlayerStatusPlane ();
					SetDropResult (eventData, true);
				} else {
					SetDropResult (eventData, false);
				}


			}

			tintImage.enabled = false;
		}

		private Item GetItem(PointerEventData data){
			return data.pointerDrag.GetComponent<ItemDragControl> ().item;
		}



		/// <summary>
		/// 获取装备在人物装备栏内的序号，如果不在人物装备栏内，返回-1
		/// </summary>
//		private int GetEquipmentIndexInPanel(Equipment equipment){
//			for (int i = 0; i < Player.mainPlayer.allEquipedEquipments.Length; i++) {
//				if (equipment == Player.mainPlayer.allEquipedEquipments [i]) {
//					return i;
//				}
//			}
//			return - 1;
//		}


		void OnDestroy(){
//			mBagView = null;
//			tintImage = null;
		}

		/*************总的背包界面不实现物品拖拽功能，该功能放在单个背包格子中**************/
//		protected override void OnUserShortClick (PointerEventData eventData)
//		{
//			return;
//		}
//		protected override void OnUserLongPress (PointerEventData eventData)
//		{
//			return;
//		}
//		protected override void OnUserDrag (PointerEventData eventData)
//		{
//			return;
//		}
//		protected override void OnUserPointerUp (PointerEventData eventData)
//		{
//			return;
//		}



	}
}
