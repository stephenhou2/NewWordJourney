using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public abstract class Trap : TriggeredGear {

		// 陷阱已经关闭
		public bool isTrapOn;

		public void ChangeTrapStatus(){
			if (isTrapOn) {
				SetTrapOff ();
				isTrapOn = false;
			} else {
				SetTrapOn ();
				isTrapOn = true;
			}
		}

		public abstract void SetTrapOn ();
		public abstract void SetTrapOff ();


		/// <summary>
		/// 进入陷阱
		/// </summary>
		/// <param name="col">Col.</param>
		public abstract void OnTriggerEnter2D(Collider2D col);


	}
}
