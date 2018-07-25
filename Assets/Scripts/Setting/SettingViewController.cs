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

            GameManager.Instance.persistDataManager.ResetPlayerDataToOriginal();

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


            if (ExploreManager.Instance != null)
            {
				GameManager.Instance.UIManager.SetUpCanvasWith(CommonData.loadingCanvasBundleName, "LoadingCanvas", delegate
                {
                    TransformManager.FindTransform("LoadingCanvas").GetComponent<LoadingViewController>().SetUpLoadingView(LoadingType.QuitExplore, ExploreManager.Instance.QuitExploreScene, null);
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
