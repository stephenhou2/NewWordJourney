using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.EventSystems;

	public class ConsumablesDropControl : ItemDropControl {

		public int consumablesIndexInPanel;

		private BattlePlayerUIController mExplorePlayerUICtr;
		private BattlePlayerUIController explorePlayerUICtr{
			get{
				if (mExplorePlayerUICtr == null) {
					mExplorePlayerUICtr = TransformManager.FindInParents<BattlePlayerUIController> (this.gameObject);
				}
				return mExplorePlayerUICtr;
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

			dragControl.detectReceiver = true;

			if (draggedItem.itemType == ItemType.Consumables) {
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

			if (dragControl == null) {
				tintImage.enabled = false;
				return;
			}

			dragControl.detectReceiver = false;

			tintImage.enabled = false;
		}



		protected override void OnUserDrop (UnityEngine.EventSystems.PointerEventData eventData)
		{
			// 获取拖拽中的物品
			Consumables consumablesPrepareToLoad = GetDraggedItem (eventData) as Consumables;

			// 没有拖拽中的物品或者物品类型不是装备直接返回
			if (consumablesPrepareToLoad == null || consumablesPrepareToLoad.itemType != ItemType.Consumables) {
				SetDropResult (eventData, false);
				tintImage.enabled = false;
				return;
			}

			Consumables consumablesPrepareToUnload = Player.mainPlayer.allConsumablesInBag [consumablesIndexInPanel];

			int indexOfConsumablesPrepareToLoad = GetConsumablesIndexInBag (consumablesPrepareToLoad);
			int indexOfConsumablesPrepareToUnload = consumablesIndexInPanel;

			Player.mainPlayer.allConsumablesInBag [indexOfConsumablesPrepareToLoad] = consumablesPrepareToUnload;
			Player.mainPlayer.allConsumablesInBag [indexOfConsumablesPrepareToUnload] = consumablesPrepareToLoad;

			explorePlayerUICtr.SetUpBottomConsumablesButtons ();
			explorePlayerUICtr.SetUpConsumablesInBagPlane ();

		
			SetDropResult (eventData, true);

			tintImage.enabled = false;

		}

		private int GetConsumablesIndexInBag(Consumables consumables){
			for (int i = 0; i < Player.mainPlayer.allConsumablesInBag.Count; i++) {
				if (Player.mainPlayer.allConsumablesInBag [i] == consumables) {
					return i;
				}
			}
			return -1;
		}

		void OnDestroy(){
//			tintImage = null;
//			mExplorePlayerUICtr = null;
		}

	}
}
