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

			countDownText.text = "否(5)";

			gameObject.SetActive (true);

			StartCoroutine ("QueryCountDown");

		}

		public void ConfirmBuyLife(){
			StopCoroutine ("QueryCountDown");
			confirmBuyCallBack ();
			QuitBuyLifeView ();
		}

		public void CancelBuyLife(){
			StopCoroutine ("QueryCountDown");
			cancelBuyCallBack ();
			QuitBuyLifeView ();
		}

		private void QuitBuyLifeView(){
			gameObject.SetActive (false);
		}


		private IEnumerator QueryCountDown(){

			int timer = 5;

			while (timer > 0) {

				yield return new WaitForSeconds (1f);

				timer--;

				countDownText.text = string.Format ("否({0})", timer);

			}

			cancelBuyCallBack ();

			QuitBuyLifeView ();

		}
	}
}
