using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

	public class BuyLifeQueryView : MonoBehaviour {

		public Text countDownText;

		private CallBack confirmBuyCallBack;

		private CallBack cancelBuyCallBack;

		public void SetUpBuyLifeQueryView(CallBack confirmBuyCallBack,CallBack cancelBuyCallBack){

			this.confirmBuyCallBack = confirmBuyCallBack;

			this.cancelBuyCallBack = cancelBuyCallBack;

			countDownText.text = "取消(<color=orange>30</color>)";

			gameObject.SetActive (true);

			StartCoroutine ("QueryCountDown");

		}

		public void ConfirmBuyLife(){
			confirmBuyCallBack ();
		}

		public void CancelBuyLife(){
			cancelBuyCallBack ();
			QuitBuyLifeView ();
		}

		public void QuitBuyLifeView(){
			StopCoroutine("QueryCountDown");
			gameObject.SetActive (false);
		}


		private IEnumerator QueryCountDown(){

			int timer = 30;

			while (timer > 0) {

				yield return new WaitForSeconds (1f);

				timer--;

				countDownText.text = string.Format ("取消(<color=orange>{0}</color>)", timer);

			}

			cancelBuyCallBack ();

			QuitBuyLifeView ();

		}
	}
}
