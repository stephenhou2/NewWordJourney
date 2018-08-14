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

		public Transform restoreMask;

        public Image pronounceOnImage;
        public Image pronounceOffImage;

		public Button restoreItemsButton;

		public TintHUD hintHUD;


		public void SetUpSettingView(GameSettings settings,CallBackWithFloat volumeChangeCallBack)
        {

			float volume = settings.systemVolume;

			if(volume < 0){
				volume = 0;
			}else if(volume > 1f){
				volume = 1f;
			}

			volumeControl.value = (int)(volume * 100);

            UpdatePronounceControl(settings.isAutoPronounce);

			UpdateDifficultySelectedIcons();

			volumeControl.InitHLHSlider(volumeChangeCallBack);

#if UNITY_IPHONE || UNITY_EDITOR
			restoreItemsButton.gameObject.SetActive(true);
#else
			restoreItemsButton.gameObject.SetActive(false);
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

		public void ShowRestoreMask(){
			restoreMask.gameObject.SetActive(true);
		}

		public void HideRestoreMask(){
			restoreMask.gameObject.SetActive(false);
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
