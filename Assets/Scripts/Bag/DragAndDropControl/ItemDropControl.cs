using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	using UnityEngine.UI;
	using UnityEngine.EventSystems;

	public abstract class ItemDropControl : MyDropControl {

		public Image tintImage;

		protected Item GetDraggedItem(PointerEventData data){
			
			ItemDragControl dragControl = data.pointerDrag.GetComponent<ItemDragControl> ();

			if (dragControl == null) {
				return null;
			}

			Item item = null;

			UserOperationType operationType = data.pointerDrag.GetComponent<ItemDragControl> ().operationType;
			if (operationType == UserOperationType.LongPress) {
				item = dragControl.item;
			}
			return item;
		}


		protected void SetDropResult(PointerEventData data,bool dropSucceed){
			ItemDragControl draggedControl = GetDraggedObject (data).GetComponent<ItemDragControl> ();
			if (draggedControl == null) {
				return;
			}
			draggedControl.dropSucceed = dropSucceed;
		}

	}
}
