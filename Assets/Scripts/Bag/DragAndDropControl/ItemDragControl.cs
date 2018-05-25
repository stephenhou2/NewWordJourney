using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;
	using UnityEngine.EventSystems;

	public delegate void CallBackWithItem(Item item);


	public abstract class ItemDragControl : MyDragControl,IPointerDownHandler {

		public Item item;

		protected GameObject tempDraggingObject;

		[HideInInspector]public bool detectReceiver;
		[HideInInspector]public bool dropSucceed;

		private Canvas mCanvas;
		protected Canvas canvas{
			get{
				if (mCanvas == null) {
					mCanvas = TransformManager.FindInParents<Canvas>(gameObject);
				}
				return mCanvas;
			}
		}

		public abstract void OnDropFailed ();

		protected CallBackWithItem shortClickCallBack;
		//protected CallBackWithItem longPressCallBack; 

		public void InitItemDragControl(Item item,CallBackWithItem shortClickCallBack){
			this.item = item;
			this.shortClickCallBack = shortClickCallBack;
			//this.longPressCallBack = longPressCallBack;
		}


		public override void OnPointerDown(PointerEventData data){
			base.OnPointerDown (data);
			detectReceiver = false;
			dropSucceed = false;
		}

		/// <summary>
		/// 生成一个暂时用于拖拽的图片
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="oriImage">Ori image.</param>
		protected void GenerateDisplayImageObject(string name,Image oriImage){

			GameObject imageDisplay = new GameObject (name);

			Image img = imageDisplay.AddComponent<Image> ();

			img.rectTransform.pivot = new Vector2 (0.5f, 0.5f);

			img.rectTransform.sizeDelta = new Vector2 (oriImage.rectTransform.rect.width, oriImage.rectTransform.rect.height);

			img.sprite = oriImage.sprite;

			imageDisplay.transform.SetParent (tempDraggingObject.transform);

			Vector3[] corners = new Vector3[4];
			oriImage.rectTransform.GetWorldCorners (corners);

			Vector3 oriCenter = (corners[0] + corners[2])/2;

			imageDisplay.transform.position = oriCenter;
			imageDisplay.transform.localScale = Vector3.one;
			imageDisplay.transform.localRotation = Quaternion.identity;
		}

		/// <summary>
		/// 设置拖拽物体的位置
		/// </summary>
		/// <param name="data">Data.</param>
		protected void SetDisplayGoPostion(PointerEventData data){
			Vector3 globalMousePos;
			RectTransformUtility.ScreenPointToWorldPointInRectangle (tempDraggingObject.transform as RectTransform, data.position, data.pressEventCamera,out globalMousePos);
			tempDraggingObject.transform.position = globalMousePos;
		}

		public abstract void Reset ();


	}
}
