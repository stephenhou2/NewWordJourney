using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

	public class BuyLifeQueryView : MonoBehaviour
	{

		public Text queryText;

		public Text countDownText;

		public Transform queryContainer;

		public Transform warningContainer;

		public Transform unexpectedAdFailContainer;

		private CallBack confirmBuyCallBack;

		private CallBack cancelBuyCallBack;

		private IEnumerator queryCoroutine;// 询问购买的协程

		public void SetUpBuyLifeQueryView(CallBack confirmBuyCallBack, CallBack cancelBuyCallBack)
		{

			queryContainer.gameObject.SetActive(true);
			warningContainer.gameObject.SetActive(false);
			unexpectedAdFailContainer.gameObject.SetActive(false);

#if UNITY_IOS
			queryText.text = "使用复活卡可以\n<color=orange>复活并恢复全部生命和魔法</color>\n是否前往购买？";
#elif UNITY_ANDROID
			queryText.text = "观看广告后可以\n<color=orange>复活并恢复30%的生命和魔法</color>\n是否前往观看？";
#elif UNITY_EDITOR
			UnityEditor.BuildTarget buildTarget = UnityEditor.EditorUserBuildSettings.activeBuildTarget;

            switch (buildTarget) {
            case UnityEditor.BuildTarget.Android:
			    queryText.text = "观看广告后可以\n<color=orange>复活并恢复30%的生命和魔法</color>\n是否前往观看？";
                break;
            case UnityEditor.BuildTarget.iOS:
			    queryText.text = "使用复活卡可以\n<color=orange>复活并恢复全部生命和魔法</color>\n是否前往购买？";
                break;
            }
#endif

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



		public void SetUpWhenADUnexpectedFailOnAndroid(CallBack confirmBuyCallBack, CallBack cancelBuyCallBack)
        {         
			queryContainer.gameObject.SetActive(false);
            warningContainer.gameObject.SetActive(false);
			unexpectedAdFailContainer.gameObject.SetActive(true);

			this.confirmBuyCallBack = confirmBuyCallBack;

            this.cancelBuyCallBack = cancelBuyCallBack;
         
        }

		public void OnConfirmButtonClickWhenAdUnexpectedFail(){

			warningContainer.gameObject.SetActive(false);
			queryContainer.gameObject.SetActive(true);
			unexpectedAdFailContainer.gameObject.SetActive(false);

            queryText.text = "观看广告后可以\n<color=orange>复活并恢复30%的生命和魔法</color>\n是否前往观看？";

            countDownText.text = "取消(<color=orange>30</color>)";
            

            if (queryCoroutine != null)
            {
                StopCoroutine(queryCoroutine);
            }

            queryCoroutine = QueryCountDown();
            StartCoroutine(queryCoroutine);

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
			//isInCancelWarning = true;
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
