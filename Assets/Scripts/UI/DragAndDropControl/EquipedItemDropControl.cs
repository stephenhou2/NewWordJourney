using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;
	using UnityEngine.EventSystems;

	public class EquipedItemDropControl : ItemDropControl {


		private BagView mBagView;
		private BagView bagView{
			get{
				if (mBagView == null) {
					mBagView = TransformManager.FindInParents<BagView> (this.gameObject);
				}
				return mBagView;
			}
		}

		public EquipedItemDragControl[] allEquipedItemDragControls;

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

			bool canEquip = draggedItem.itemType == ItemType.Equipment;

//			if (draggedItem.itemType != ItemType.Equipment) {
//				canEquip = false;
//			} else {
//				Equipment eqp = draggedItem as Equipment;
//				canEquip = Player.mainPlayer.CheckCanEquiped (eqp.equipmentType);
//			}

			if (canEquip) {
				tintImage.enabled = true;
				tintImage.color = new Color (0f, 1f, 0f, 0.2f);
			} else {
				tintImage.enabled = true;
				tintImage.color = new Color (1f, 0f, 0f, 0.2f);
			}
		}

		protected override void OnUserPointerExit (PointerEventData eventData){


			if (!eventData.dragging) {
				tintImage.enabled = false;
				return;
			}

			ItemDragControl dragControl = eventData.pointerDrag.GetComponent<ItemDragControl> ();

//			Item draggedItem = GetDraggedItem (eventData);

			if (dragControl == null) {
				tintImage.enabled = false;
				return;
			}

			dragControl.detectReceiver = false;

			tintImage.enabled = false;
		}



		protected override void OnUserDrop (PointerEventData eventData)
		{
			// 获取拖拽中的物品
			Item itemInBag = GetDraggedItem (eventData);

			if (itemInBag == null || itemInBag.itemId < 0) {
				return;
			}

			bool canEquip = itemInBag.itemType == ItemType.Equipment;

//			if (itemInBag.itemType != ItemType.Equipment) {
//				canEquip = false;
//			} else {
//				Equipment eqp = itemInBag as Equipment;
//				canEquip = Player.mainPlayer.CheckCanEquiped (eqp.equipmentType);
//			}


			// 没有拖拽中的物品或者该装备栏没有解锁或者物品类型不是装备直接返回
			if (!canEquip) {
				SetDropResult (eventData, false);
				tintImage.enabled = false;
				return;
			}
            
			GameObject draggedObject = GetDraggedObject (eventData);

			// 准备装上的装备
			Equipment equipmentPrepareToLoad = itemInBag as Equipment;


			ItemDragControl dragControl = draggedObject.GetComponent<ItemDragControl>();

			if(dragControl == null){
				return;
			}

			if(bagView != null && !(dragControl is SpecialOperationItemDragControl)){
				
				bagView.GetComponent<BagViewController>().currentSelectItem = equipmentPrepareToLoad;
            }

			int indexInPanel = (int)equipmentPrepareToLoad.equipmentType;

            // 如果额外装备槽已解锁，并且想要装上的装备是戒指，并且原有戒指槽已经有装备，则该戒指撞到额外装备槽上
            if (equipmentPrepareToLoad.equipmentType == EquipmentType.Ring && BuyRecord.Instance.extraEquipmentSlotUnlocked && Player.mainPlayer.allEquipedEquipments[5].itemId >= 0 && Player.mainPlayer.allEquipedEquipments[6].itemId < 0)
            {
                indexInPanel = 6;
            }

			// 准备换下的装备
			Equipment equipmentPrepareToUnload = Player.mainPlayer.allEquipedEquipments[indexInPanel];

			// 如果是从装备栏中拖拽出来的物品
			if (dragControl is EquipedItemDragControl) {

				SetDropResult (eventData, true);

				tintImage.enabled = false;
				return;
			}

			// 如果是从背包中拖拽出来的物品
			else if (dragControl is ItemInBagDragControl) {

				PropertyChange propertyChangeFromUnload = new PropertyChange();

				int oriItemIndexInBag = Player.mainPlayer.GetItemIndexInBag (equipmentPrepareToLoad);

				if (oriItemIndexInBag == -1) {
					Debug.LogError("背包中没有找到该物品");
				}

				if (equipmentPrepareToUnload.itemId >= 0) {
					// 该装备移入背包中
					propertyChangeFromUnload = Player.mainPlayer.UnloadEquipment (equipmentPrepareToUnload, indexInPanel,oriItemIndexInBag);

					bagView.AddBagItem (equipmentPrepareToUnload, oriItemIndexInBag,true);

				}
					
				// 背包中的装备移入已装备列表
				PropertyChange propertyChangeFromLoad = Player.mainPlayer.EquipEquipment(equipmentPrepareToLoad,indexInPanel);

				PropertyChange propertyChange = PropertyChange.MergeTwoPropertyChange (propertyChangeFromLoad,propertyChangeFromUnload);

				bagView.SetUpEquipedEquipmentsPlane ();

				bagView.SetUpPlayerStatusPlane (propertyChange);

				// 对应格子中的装备数据更换
				allEquipedItemDragControls[indexInPanel].item = equipmentPrepareToLoad;

				Player.mainPlayer.ResetBattleAgentProperties (false);

				bagView.RemoveBagItemAt (oriItemIndexInBag + (equipmentPrepareToUnload.itemId >= 0 ? 1 : 0));

				SetDropResult (eventData, true);
            
				bagView.SetUpItemDetail (equipmentPrepareToLoad);

				tintImage.enabled = false;

				return;

			}

			//else if(dragControl is SpecialOperationItemDragControl){
			//	SpecialOperationCell specialOperationCell = dragControl.GetComponent<SpecialOperationCell>();            
   //             specialOperationCell.ResetSpecialOperationCell();
   //             SetDropResult(eventData, true);            
			//}

			tintImage.enabled = false;

		}



		/// <summary>
		/// 获取装备在人物装备栏内的序号，如果不在人物装备栏内，返回-1
		/// </summary>
		private int GetEquipmentIndexInPanel(Equipment equipment){
			for (int i = 0; i < Player.mainPlayer.allEquipedEquipments.Length; i++) {
				if (equipment == Player.mainPlayer.allEquipedEquipments [i]) {
					return i;
				}
			}
			return - 1;
		}

		void OnDestroy(){
//			tintImage = null;
//			mBagView = null;
		}

	}
}
