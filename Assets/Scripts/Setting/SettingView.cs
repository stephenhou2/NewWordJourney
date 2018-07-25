using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace WordJourney
{
    public class SettingView : MonoBehaviour
    {

		public HLHSlider volumeControl;

        public Transform settingPlane;

        public Transform quitAPPButton;

		public Image[] difficultySelectedIcons;

        public Transform queryChangeWordHUD;

        public Image pronounceOnImage;
        public Image pronounceOffImage;

		public void SetUpSettingView(GameSettings settings,CallBackWithFloat volumeChangeCallBack)
        {

            volumeControl.value = (int)(settings.systemVolume * 100);

            UpdatePronounceControl(settings.isAutoPronounce);

			UpdateDifficultySelectedIcons();

			volumeControl.InitHLHSlider(volumeChangeCallBack);

#if UNITY_ANDROID
            quitAPPButton.gameObject.SetActive(true);
#else
            quitAPPButton.gameObject.SetActive(false);
#endif

            GetComponent<Canvas> ().enabled = true;

		}

		public void UpdatePronounceControl(bool enable){

			if (!enable) {
				pronounceOnImage.gameObject.SetActive (false);
				pronounceOffImage.gameObject.SetActive (true);
			} else {
				pronounceOnImage.gameObject.SetActive (true);
				pronounceOffImage.gameObject.SetActive (false);
			}

		}

		public void UpdateDifficultySelectedIcons(){
			
			int diffucultyInt = (int)GameManager.Instance.gameDataCenter.gameSettings.wordType;

            for (int i = 0; i < difficultySelectedIcons.Length; i++)
            {

                difficultySelectedIcons[i].enabled = i == diffucultyInt;

            }

		}



		public void ShowAlertHUD(){
			queryChangeWordHUD.gameObject.SetActive (true);
		}

		public void QuitAlertHUD(){
			queryChangeWordHUD.gameObject.SetActive (false);
		}


		public void QuitSettingView(){

			GetComponent<Canvas> ().enabled = false;

		}
        

	}
}
