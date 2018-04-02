using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;
	using UnityEngine.EventSystems;


	public class ItemInBagDragControl : ItemDragControl {

		public Image itemImage;
		public Image backgroundImage;
		public Image newItemTintIcon;
		public Image selectedTintIcon;
		public Text extroInfo;


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
			if (item == null) {
				return;
			}
//			Debug.Log (string.Format("点击了{0}",item.itemName));

			item.isNewItem = false;

			newItemTintIcon.enabled = false;

			if (shortClickCallBack != null) {
				shortClickCallBack (item);
				selectedTintIcon.enabled = true;
			}
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
			newItemTintIcon.enabled = false;
			extroInfo.enabled = false;
			selectedTintIcon.enabled = false;
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

//			GameObject enteredObject = GetEnteredObject (eventData);
//			if (enteredObject != null) {
//				enteredObject.GetComponent<ItemDropControl> ().tintImage.enabled = false;
//			}

		}

		public override void OnDropFailed ()
		{
			backgroundImage.enabled = true;
			itemImage.enabled = itemImage.sprite != null;
			newItemTintIcon.enabled = false;
			extroInfo.enabled = true;

		}

		void OnDestroy(){

//			if (checkOperationTypeCoroutine != null) {
//				StopCoroutine (checkOperationTypeCoroutine);
//				checkOperationTypeCoroutine = null;
//			}
//			mBagView = null;
//			itemImage = null;
//			itemName = null;
//			itemNameBackgound = null;
//			newItemTintIcon = null;
//			extroInfo = null;
		}


		//*******背包内按钮不实现单独的接收物品功能*******//
//		protected override void OnUserDrop (PointerEventData eventData)
//		{
//			return;
//		}
//
//		protected override void OnUserPointerEnter (PointerEventData eventData)
//		{
//			return;
//		}
//
//		protected override void OnUserPointerExit (PointerEventData eventData)
//		{
//			return;
//		}



	}
}
