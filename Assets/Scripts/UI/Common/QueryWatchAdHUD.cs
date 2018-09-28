using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

	public class QueryWatchAdHUD : MonoBehaviour
    {

		public Text queryText;
        
		private CallBack watchAdCallBack;

		private CallBack quitCallBack;

        // 确认观看广告按钮
		public Button confirmWatchButton;

        // 取消观看按钮
		public Button cancelWatchButton;

        // 无法观看是的确认按钮
		public Button confirmCantWatchButton;

		private IEnumerator timeCountDownCoroutine;

		public void SetUpQueryWatchAdHUD(string query,CallBack watchAdCallBack,CallBack quitCallBack){

			queryText.text = query;

			this.watchAdCallBack = watchAdCallBack;

			this.quitCallBack = quitCallBack;

			confirmWatchButton.gameObject.SetActive(true);
			cancelWatchButton.gameObject.SetActive(true);

			confirmCantWatchButton.gameObject.SetActive(false);

			this.gameObject.SetActive(true);


		}

        /// <summary>
        /// 无法观看是初始化界面的方法
        /// </summary>
        /// <param name="query">Query.</param>
        /// <param name="timeSpane">距离上次看过获取金币的视频广告的时间间隔.</param>
		public void SetUpQueryWatchAdHUDWhenCantWatch(string query,double timeSpan,CallBack timeCanWatchCallBack){

			queryText.text = query;

			confirmWatchButton.gameObject.SetActive(false);
			cancelWatchButton.gameObject.SetActive(false);

			confirmCantWatchButton.gameObject.SetActive(true);

			this.gameObject.SetActive(true);

			if(timeCountDownCoroutine != null){
				StopCoroutine(timeCountDownCoroutine);            
			}

			timeCountDownCoroutine = TimeCountDown((int)timeSpan,timeCanWatchCallBack);

			StartCoroutine(timeCountDownCoroutine);


		}

		private IEnumerator TimeCountDown(int totalSecondsLeft,CallBack callBack){

			int timer = totalSecondsLeft;

			while(timer < 1800){

				yield return new WaitForSecondsRealtime(1f);

				timer++; 

				int minuteLeft = (1800 - timer) / 60;
				int secondsLeft = (1800 - timer) % 60;
				queryText.text = string.Format("<color=orange>{0}{1}:{2}{3}</color>后可以重新观看广告",
                                      minuteLeft < 10 ? "0" : "",
                                      minuteLeft,
                                      secondsLeft < 10 ? "0" : "",
                                      secondsLeft);

			}


			if(callBack != null){
				callBack();
			}

		}


		public void OnConfirmButtonClick(){

			if(watchAdCallBack != null){
				watchAdCallBack();
			}

			QuitQueryWatchAdHUD();

		}

		public void OnCancelButtonClick(){
			QuitQueryWatchAdHUD();

			if(quitCallBack != null){
				quitCallBack();
			}
		}

		private void QuitQueryWatchAdHUD(){

			if(timeCountDownCoroutine != null){
				StopCoroutine(timeCountDownCoroutine);
			}

			this.gameObject.SetActive(false);
		}

        
    }

}

