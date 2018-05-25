using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

	public class PauseHUD : MonoBehaviour {

		private enum QueryType{
			Refresh,
			BackHome
		}


		public Transform queryContainer;

		private QueryType queryType;

		public Text query;


		private bool quitWhenClickBackground = true;
		private CallBack backHomeCallBack;
		private CallBack settingsCallBack;
		private CallBack quitPauseCallBack;


		public void InitPauseHUD(bool quitWhenClickBackground,CallBack backHomeCallBack,CallBack settingsCallBack,CallBack quitPauseCallBack){
			this.quitWhenClickBackground = quitWhenClickBackground;
			this.backHomeCallBack = backHomeCallBack;
			this.settingsCallBack = settingsCallBack;
			this.quitPauseCallBack = quitPauseCallBack;
		}


		public void SetUpPauseHUD(){

			Time.timeScale = 0f;

			gameObject.SetActive (true);

		}

		public void OnBackgroundClick(){
			if (quitWhenClickBackground) {
				QuitPauseHUD ();
			}
		}


//		public void OnRefreshButtonClick(){

//			queryType = QueryType.Refresh;

//			if (refreshCallBack != null) {
//				refreshCallBack ();
//			}

//			query.text = "是否确认重新开始本关？";

//			queryContainer.gameObject.SetActive (true);

////			QuitPauseHUD ();

		//}


		public void OnBackHomeButtonClick(){

			queryType = QueryType.BackHome;


			query.text = "本次探险进度将会丢失\n是否确认返回主界面？";

			queryContainer.gameObject.SetActive (true);

//			QuitPauseHUD ();

		}

		public void OnSettingsButtonClick(){

			if (settingsCallBack != null) {
				settingsCallBack ();
			}

			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.settingCanvasBundleName, "SettingCanvas", () => {
				TransformManager.FindTransform("SettingCanvas").GetComponent<SettingViewController>().SetUpSettingView();
			},false,true);

		}

		public void OnConfirmBackHomeButtonClick(){
			
			queryContainer.gameObject.SetActive (false);

			QuitPauseHUD ();

			if(backHomeCallBack != null){
				backHomeCallBack();
			}

		}

		public void OnCancelBackHomeButtonClick(){

			queryContainer.gameObject.SetActive (false);

			QuitPauseHUD ();

		}


		public void QuitPauseHUD(){

			Time.timeScale = 1f;

			if (quitPauseCallBack != null) {
				quitPauseCallBack ();
			}

			gameObject.SetActive (false);
		}

	}
}
