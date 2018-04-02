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
		private CallBack quitCallBack;
		private CallBack refreshCallBack;
		private CallBack backHomeCallBack;
		private CallBack settingsCallBack;


		public void InitPauseHUD(bool quitWhenClickBackground,CallBack refreshCallBack,CallBack backHomeCallBack,CallBack quitCallBack,CallBack settingsCallBack){
			this.quitWhenClickBackground = quitWhenClickBackground;
			this.refreshCallBack = refreshCallBack;
			this.backHomeCallBack = backHomeCallBack;
			this.quitCallBack = quitCallBack;
			this.settingsCallBack = settingsCallBack;
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


		public void OnRefreshButtonClick(){

			queryType = QueryType.Refresh;

			if (refreshCallBack != null) {
				refreshCallBack ();
			}

			query.text = "是否确认重新开始本关？";

			queryContainer.gameObject.SetActive (true);

//			QuitPauseHUD ();

		}


		public void OnBackHomeButtonClick(){

			queryType = QueryType.BackHome;

			if (backHomeCallBack != null) {
				backHomeCallBack ();
			}

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

		public void OnConfirmButtonClick(){
			
			queryContainer.gameObject.SetActive (false);

			QuitPauseHUD ();

			ExploreManager exploreManager = ExploreManager.Instance;

			switch (queryType) {
			case QueryType.BackHome:
				exploreManager.QuitExploreScene (false);
				break;
			}
		}

		public void OnCancelButtonClick(){

			queryContainer.gameObject.SetActive (false);

			QuitPauseHUD ();

		}


		public void QuitPauseHUD(){

			Time.timeScale = 1f;

			if (quitCallBack != null) {
				quitCallBack ();
			}

			gameObject.SetActive (false);
		}

	}
}
