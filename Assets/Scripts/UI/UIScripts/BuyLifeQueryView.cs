using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

	public class BuyLifeQueryView : MonoBehaviour {

		public Text countDownText;

		public Transform warningContainer;

		private CallBack confirmBuyCallBack;

		private CallBack cancelBuyCallBack;

        // 是否在取消购买的警告界面
		private bool isInCancelWarning;

		private IEnumerator queryCoroutine;// 询问购买的协程

		public void SetUpBuyLifeQueryView(CallBack confirmBuyCallBack,CallBack cancelBuyCallBack){

			warningContainer.gameObject.SetActive(false);

			isInCancelWarning = false;

			this.confirmBuyCallBack = confirmBuyCallBack;

			this.cancelBuyCallBack = cancelBuyCallBack;

			countDownText.text = "取消(<color=orange>30</color>)";

			gameObject.SetActive (true);

			if(queryCoroutine != null){
				StopCoroutine(queryCoroutine);
			}

			queryCoroutine = QueryCountDown();
			StartCoroutine (queryCoroutine);

		}

		public void OnConfirmBuyLifeButtonClick()
		{
			if(confirmBuyCallBack != null){
				confirmBuyCallBack();
			}
		}
        
		public void OnCancelBuyLifeButtonClick(){
			Time.timeScale = 0;
			warningContainer.gameObject.SetActive(true);
		}

		public void OnConfirmButtonInWarningClick(){
			Time.timeScale = 1f;
			warningContainer.gameObject.SetActive(false);
			CancelBuyLife();
		}

		public void OnCancelButtonInWarningClick(){
			Time.timeScale = 1f;
			warningContainer.gameObject.SetActive(false);
		}

		public void CancelBuyLife(){
			isInCancelWarning = true;
			cancelBuyCallBack ();
			QuitBuyLifeView ();
		}

		public void QuitBuyLifeView(){
			if (queryCoroutine != null)
            {
                StopCoroutine(queryCoroutine);
            }
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
