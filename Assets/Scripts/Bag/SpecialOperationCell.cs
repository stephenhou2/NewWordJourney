using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{
	using UnityEngine.UI;

	public class SpecialOperationCell : MonoBehaviour {

		public SpecialOperationItemDragControl soDragControl;

		public SpecialOperationItemDropControl soDropControl;

//		private CallBack dropCallBack;

//		public void InitSpecialOperationCell(CallBack dropCallBack){
//			soDropControl.InitSpecialOperationDropControl (dropCallBack);
//		}
			
	
		public void ResetSpecialOperationCell(){

			soDragControl.itemImage.sprite = null;
			soDragControl.itemImage.enabled = false;

			soDragControl.item = null;

			soDropControl.tintImage.enabled = false;

		}


	}
}
