using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
    public class SettingViewController : MonoBehaviour
    {

        public SettingView settingView;

        //private bool isPointerUp;

        private bool settingChanged;

        private int currentSelectWordTypeIndex;


        public void SetUpSettingView()
        {         
            GameSettings settings = GameManager.Instance.gameDataCenter.gameSettings;

			settingView.SetUpSettingView(settings,ChangeVolume);

        }


		public void ChangeVolume(float volume)
        {

			if(volume<0){
				volume = 0;
			}else if(volume > 1f){
				volume = 1f;
			}

			GameManager.Instance.gameDataCenter.gameSettings.systemVolume = volume;

            GameManager.Instance.soundManager.UpdateVolume();

            settingChanged = true;

        }

        public void ChangePronunciationEnability()
        {

            bool isAutoPronounce = !GameManager.Instance.gameDataCenter.gameSettings.isAutoPronounce;

            GameManager.Instance.gameDataCenter.gameSettings.isAutoPronounce = isAutoPronounce;

            settingView.UpdatePronounceControl(isAutoPronounce);

            settingChanged = true;

        }


        public void ChangeWordType(int index)
        {
            int wordTypeIndex = (int)GameManager.Instance.gameDataCenter.gameSettings.wordType;
            if (wordTypeIndex != index)
            {
                currentSelectWordTypeIndex = index;
                settingView.ShowAlertHUD();
				settingView.UpdateDifficultySelectedIcons();

            }
        }

        public void OnConfirmButtonClick()
		{

            settingView.QuitAlertHUD();
         
            switch (currentSelectWordTypeIndex)
            {
                case 0:
                    GameManager.Instance.gameDataCenter.gameSettings.wordType = WordType.Simple;
                    break;
                case 1:
                    GameManager.Instance.gameDataCenter.gameSettings.wordType = WordType.Medium;
                    break;
                case 2:
                    GameManager.Instance.gameDataCenter.gameSettings.wordType = WordType.Master;
                    break;               
            }

			Player.mainPlayer.needChooseDifficulty = false;

			GameManager.Instance.persistDataManager.ResetPlayerDataToOriginal();
			GameManager.Instance.persistDataManager.ClearCurrentMapWordsRecordAndSave();

            if (ExploreManager.Instance != null)
            {
				GameManager.Instance.UIManager.SetUpCanvasWith(CommonData.loadingCanvasBundleName, "LoadingCanvas", delegate
                {
                    TransformManager.FindTransform("LoadingCanvas").GetComponent<LoadingViewController>().SetUpLoadingView(LoadingType.QuitExplore, delegate {
						ExploreManager.Instance.QuitExploreScene();                  
					}, null);
                });

            }



			settingView.UpdateDifficultySelectedIcons();

            settingChanged = true;

        }

        public void OnCancelButtonClick()
        {
            //settingView.SetUpSettingView(GameManager.Instance.gameDataCenter.gameSettings);
            settingView.QuitAlertHUD();
        }

        /// <summary>
        /// 恢复购买按钮点击响应
		/// 恢复非消耗类商品的购买状态
        /// </summary>
		public void OnRestoreButtonClick(){
			switch(Application.internetReachability){
				case NetworkReachability.NotReachable:
					settingView.hintHUD.SetUpSingleTextTintHUD("无网络连接");
					break;
				case NetworkReachability.ReachableViaCarrierDataNetwork:
				case NetworkReachability.ReachableViaLocalAreaNetwork:
					settingView.ShowRestoreMask();
                    GameManager.Instance.purchaseManager.RestoreItems(RestoreFinishCallBack);
					break;
			}

		}

		public void OnPrivacyButtonClick(){
			Application.OpenURL("http://www.lofter.com/lpost/1fc6f75f_12b25c509");
		}

		private void RestoreFinishCallBack(int result){
			settingView.HideRestoreMask();
			bool success = result == 1;
			if(success){
				settingView.hintHUD.SetUpSingleTextTintHUD("成功恢复已购买项");
			}else{
				settingView.hintHUD.SetUpSingleTextTintHUD("恢复失败，请稍后重试");
			}
				
		}

        public void OnQuitSettingViewButtonClick()
        {

            if (settingChanged)
            {
                ChangeSettingsAndSave();
            }

            settingChanged = false;

            settingView.QuitSettingView();

            Transform exploreCanvas = TransformManager.FindTransform("ExploreCanvas");
            if (exploreCanvas == null)
            {
                GameManager.Instance.UIManager.SetUpCanvasWith(CommonData.homeCanvasBundleName, "HomeCanvas", () =>
                {
                    TransformManager.FindTransform("HomeCanvas").GetComponent<HomeViewController>().SetUpHomeView();
                });
            }
            else
            {
                exploreCanvas.GetComponent<ExploreUICotroller>().QuitPauseHUD();
            }

            GameManager.Instance.UIManager.RemoveCanvasCache("SettingCanvas");


        }

        /// <summary>
        /// Changes the settings and save.
        /// </summary>
        private void ChangeSettingsAndSave()
        {

            //			GameManager.Instance.soundManager.effectAS.volume = GameManager.Instance.gameDataCenter.gameSettings.systemVolume;
            //			GameManager.Instance.soundManager.bgmAS.volume = GameManager.Instance.gameDataCenter.gameSettings.systemVolume;
            //
            //			GameManager.Instance.soundManager.pronunciationAS.enabled = GameManager.Instance.gameDataCenter.gameSettings.isPronunciationEnable;

            GameManager.Instance.persistDataManager.SaveGameSettings();

        }

        public void QuitAPP()
        {

#if UNITY_ANDROID
            Application.Quit();
#endif

		}



		public void DestroyInstances(){

			MyResourceManager.Instance.UnloadAssetBundle (CommonData.settingCanvasBundleName, true);

			Destroy (this.gameObject,0.3f);

		}

      
	}
}
