using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;
	using UnityEngine.EventSystems;

	public class EquipedItemDragControl : ItemDragControl {

		public Image backgroundImage;

		public Image itemImage;

		public int equipmentIndexInPanel;

		public Image selectedIcon;

		private BagView mBagView;
		private BagView bagView{
			get{
				if (mBagView == null) {
					mBagView = TransformManager.FindInParents<BagView> (this.gameObject);
				}
				return mBagView;
			}
		}


		protected override void OnUserShortClick (PointerEventData eventData)
		{
         
			bagView.OnItemInEquipmentPlaneClick(item,equipmentIndexInPanel);

			selectedIcon.enabled = item != null && (item as Equipment).itemId >= 0;
		}



		protected override void OnUserLongPress (PointerEventData eventData)
		{
			if (canvas == null) {
				return;
			}

			if ((item as Equipment).itemId < 0) {
				return;
			}

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

			backgroundImage.enabled = true;
			itemImage.enabled = false;
		}

		protected override void OnUserDrag (PointerEventData eventData)
		{
			if ((item as Equipment).itemId < 0) {
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
			if (item.itemId >= 0) {
				itemImage.enabled = itemImage.sprite != null;
			}

			backgroundImage.enabled = true;
			detectReceiver = false;
		}

		public override void Reset ()
		{
			item = null;
			itemImage.enabled = item != null;
			backgroundImage.enabled = true;
			selectedIcon.enabled = false;
		}

      
	}
}
