using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

	using UnityEngine.UI;
	using UnityEngine.EventSystems;

	//用户操作类型
	public enum UserOperationType
	{
		Cancel,
		ShortClick,
		LongPress
	}

	public abstract class MyDragControl : MonoBehaviour,IDragHandler,IPointerDownHandler,IPointerUpHandler{

		protected delegate void UserOperationCallBack(PointerEventData eventData);

		protected float minPressTimeToDrag = 0.2f;//点击阈值（超出算长按）

		protected IEnumerator checkOperationTypeCoroutine;

		private UserOperationType mOperationType;//用户操作类型【1.点击 2.长按】
		public UserOperationType operationType{ 
			get{ return mOperationType;}
		}



		protected abstract void OnUserShortClick (PointerEventData eventData);//用户点击事件响应
		protected abstract void OnUserLongPress (PointerEventData eventData);//用户长按事件响应
		protected abstract void OnUserDrag (PointerEventData eventData);//用户拖拽事件响应
		protected abstract void OnUserPointerUp (PointerEventData eventData);//用户手指或鼠标松开事件响应
        

		/// <summary>
		/// 检测到手指或鼠标按下时开启协程，判断用户本次操作类型
		/// </summary>
		/// <param name="data">Data.</param>
		public virtual void OnPointerDown(PointerEventData data){

			UserOperationCallBack longPressCallBack = OnUserLongPress;

			Rect detectSourceRect = (transform as RectTransform).rect;

			checkOperationTypeCoroutine = CheckUserOperationType (data,longPressCallBack);

			StartCoroutine (checkOperationTypeCoroutine);
		}

		public void OnPointerUp(PointerEventData data){
			if (checkOperationTypeCoroutine != null) {
				StopCoroutine (checkOperationTypeCoroutine);
			}
			OnUserPointerUp (data);
		}

		public void OnDrag(PointerEventData data){
			OnUserDrag (data);
		}


		/// <summary>
		/// 判断用户本次点击事件操作类型的协程
		/// </summary>
		/// <returns>The user operation type.</returns>
		/// <param name="data">点击起始时刻的点击信息数据</param>
		/// <param name="longPressCallBack">长按事件回调.</param>
		private IEnumerator CheckUserOperationType(PointerEventData data,UserOperationCallBack longPressCallBack){

			float pressTime = Time.realtimeSinceStartup;

			mOperationType = UserOperationType.ShortClick;

			Vector3 pointerWorldPos;

			Vector3[] worldCorners = new Vector3[4];

			(transform as RectTransform).GetWorldCorners (worldCorners);

			bool pointerInsideDetectSource = true;

			while (Time.realtimeSinceStartup - pressTime < minPressTimeToDrag) {

				RectTransformUtility.ScreenPointToWorldPointInRectangle (transform as RectTransform, Input.mousePosition, data.pressEventCamera ,out pointerWorldPos);

				pointerInsideDetectSource = CheckPointerInsideDetectSource (worldCorners,pointerWorldPos);

				if (!pointerInsideDetectSource) {
					mOperationType = UserOperationType.Cancel;
					break;
				}

				yield return null;
			}

			if (pointerInsideDetectSource) {

				mOperationType = UserOperationType.LongPress;

				OnUserLongPress (data);

			}

			checkOperationTypeCoroutine = null;
		}


		/// <summary>
		/// 判断用户在点击过程中手指或鼠标是否移出了目标有效点击区域
		/// </summary>
		/// <returns><c>true</c>, if pointer inside detect source was checked, <c>false</c> otherwise.</returns>
		/// <param name="sourceWorldCorners">Source world corners.</param>
		/// <param name="pointerPos">Pointer position.</param>
		private bool CheckPointerInsideDetectSource(Vector3[] sourceWorldCorners,Vector3 pointerPos){

			return	pointerPos.x > sourceWorldCorners [0].x &&
				pointerPos.x < sourceWorldCorners [2].x &&
				pointerPos.y > sourceWorldCorners [0].y &&
				pointerPos.y < sourceWorldCorners [2].y;

		}

		protected GameObject GetDraggedObject(PointerEventData data){
			return data.pointerDrag;
		}


	}
}
