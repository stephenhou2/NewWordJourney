using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	/// <summary>
    /// 设置界面控制器
    /// </summary>
    public class SettingViewController : MonoBehaviour
    {
        // 设置界面
        public SettingView settingView;
        // 标记游戏设置是否更改过
        private bool settingChanged;
        // 当前词库序号
        private int currentSelectWordTypeIndex;

        /// <summary>
        /// 初始化设置界面
        /// </summary>
        public void SetUpSettingView()
        {         
            GameSettings settings = GameManager.Instance.gameDataCenter.gameSettings;

			settingView.SetUpSettingView(settings,ChangeVolume);

        }
        
        /// <summary>
        /// 音量调整
        /// </summary>
        /// <param name="volume">Volume.</param>
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

        /// <summary>
        /// 更改自动发音设置
        /// </summary>
        public void ChangePronunciationEnability()
        {

            bool isAutoPronounce = !GameManager.Instance.gameDataCenter.gameSettings.isAutoPronounce;

            GameManager.Instance.gameDataCenter.gameSettings.isAutoPronounce = isAutoPronounce;

            settingView.UpdatePronounceControl(isAutoPronounce);

            settingChanged = true;

        }

        /// <summary>
        /// 更改词库类型
        /// </summary>
        /// <param name="index">Index.</param>
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

        /// <summary>
        /// 更改词库确认按钮点击响应
        /// </summary>
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

        /// <summary>
        /// 更改词库取消按钮点击响应
        /// </summary>
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

        /// <summary>
        /// 隐私策略按钮点击响应
        /// </summary>
		public void OnPrivacyButtonClick(){
			Application.OpenURL("http://www.lofter.com/lpost/1fc6f75f_12b25c509");
		}

        /// <summary>
        /// 恢复内购回调
        /// </summary>
        /// <param name="result">Result.</param>
		private void RestoreFinishCallBack(int result){
			settingView.HideRestoreMask();
			bool success = result == 1;
			if(success){
				settingView.hintHUD.SetUpSingleTextTintHUD("成功恢复已购买项");
			}else{
				settingView.hintHUD.SetUpSingleTextTintHUD("恢复失败，请稍后重试");
			}
				
		}

        /// <summary>
        /// 退出设置界面
        /// </summary>
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
        /// 游戏设置发生更改后保存游戏设置
        /// </summary>
        private void ChangeSettingsAndSave()
        {

            //			GameManager.Instance.soundManager.effectAS.volume = GameManager.Instance.gameDataCenter.gameSettings.systemVolume;
            //			GameManager.Instance.soundManager.bgmAS.volume = GameManager.Instance.gameDataCenter.gameSettings.systemVolume;
            //
            //			GameManager.Instance.soundManager.pronunciationAS.enabled = GameManager.Instance.gameDataCenter.gameSettings.isPronunciationEnable;

            GameManager.Instance.persistDataManager.SaveGameSettings();

        }

      
		public void DestroyInstances(){

			MyResourceManager.Instance.UnloadAssetBundle (CommonData.settingCanvasBundleName, true);

			Destroy (this.gameObject,0.3f);

		}

      
	}
}
