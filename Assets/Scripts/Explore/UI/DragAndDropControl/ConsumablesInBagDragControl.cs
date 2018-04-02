using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	using UnityEngine.UI;
	using UnityEngine.EventSystems;

	public class ConsumablesInBagDragControl : ItemDragControl {

		public Image itemImage;
		public Image backgroundImage;
//		public Text itemName;
//		public Image newItemTintIcon;

		private BattlePlayerUIController mExplorePlayerUICtr;
		private BattlePlayerUIController explorePlayerUICtr{
			get{
				if (mExplorePlayerUICtr == null) {
					mExplorePlayerUICtr = TransformManager.FindInParents<BattlePlayerUIController> (this.gameObject);
				}
				return mExplorePlayerUICtr;
			}
		}


		protected override void OnUserShortClick (PointerEventData eventData)
		{
			if (item == null) {
				return;
			}
//			Debug.Log (string.Format("点击了{0}",item.itemName));

			explorePlayerUICtr.OnConsumablesButtonClick (item as Consumables);

			item.isNewItem = false;

//			newItemTintIcon.enabled = false;

		}

		protected override void OnUserLongPress (PointerEventData eventData)
		{
			if (canvas == null) {
				return;
			}

			if (item == null) {
				return;
			}

			item.isNewItem = false;

			tempDraggingObject = new GameObject ("TempObjectForDragging");

			tempDraggingObject.AddComponent<RectTransform> ();

			CanvasGroup group = tempDraggingObject.AddComponent<CanvasGroup> ();

			group.blocksRaycasts = false;

			tempDraggingObject.transform.SetParent (canvas.transform,false);

			tempDraggingObject.transform.position = transform.position;
			tempDraggingObject.transform.localScale = new Vector3 (1.2f, 1.2f, 1);
			tempDraggingObject.transform.localRotation = Quaternion.identity;

			if (backgroundImage != null) {
				GenerateDisplayImageObject ("BackgroundImage",backgroundImage);
			}

			if (itemImage!= null) {
				GenerateDisplayImageObject ("ItemImage",itemImage);
			}

			backgroundImage.enabled = false;
			itemImage.enabled = false;
//			itemName.enabled = false;
//			newItemTintIcon.enabled = false;
		}

		protected override void OnUserDrag (PointerEventData eventData)
		{
			if (item == null) {
				return;
			}

			if (operationType == UserOperationType.ShortClick || operationType == UserOperationType.Cancel) {
				return;
			} 
			SetDisplayGoPostion (eventData);
		}

		protected override void OnUserPointerUp (PointerEventData eventData)
		{

			switch (operationType) {
			case UserOperationType.Cancel:
				OnDropFailed ();
				break;
			case UserOperationType.ShortClick:
				OnUserShortClick (eventData);
				break;
			case UserOperationType.LongPress:
				if (!detectReceiver || !dropSucceed) {
					OnDropFailed ();
				}
				if (tempDraggingObject != null) {
					Destroy (tempDraggingObject);
				}
				break;
			}
				

		}

		public override void OnDropFailed ()
		{
			backgroundImage.enabled = true;
			itemImage.enabled = true;
//			itemName.enabled = true;
//			newItemTintIcon.enabled = false;

		}

		void OnDestroy(){
//			itemImage = null;
//			item = null;
//			backgroundImage = null;
//			mExplorePlayerUICtr = null;
		}

	}
}
