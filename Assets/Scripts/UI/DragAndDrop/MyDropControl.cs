using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;
	using UnityEngine.EventSystems;

	public abstract class MyDropControl : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,IDropHandler {


		protected abstract void OnUserDrop (PointerEventData eventData);//用户拖拽结束事件响应
		protected abstract void OnUserPointerEnter (PointerEventData eventData);//用户手指或鼠标进入事件响应
		protected abstract void OnUserPointerExit (PointerEventData eventData);//用户手指或鼠标移出事件响应


		public void OnPointerEnter(PointerEventData data){
			OnUserPointerEnter (data);
		}

		public void OnPointerExit(PointerEventData data){
			OnUserPointerExit (data);
		}

		public void OnDrop(PointerEventData data){
//			Debug.Log ("drop");
			OnUserDrop (data);
		}

		protected GameObject GetDraggedObject(PointerEventData data){
			return data.pointerDrag;
		}



	}
}
