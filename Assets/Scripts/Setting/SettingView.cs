using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace WordJourney
{
	/// <summary>
    /// 设置界面
    /// </summary>
    public class SettingView : MonoBehaviour
    {
        // 音量控制bar
		public HLHSlider volumeControl;
        // 设置面板
        public Transform settingPlane;
        // 难度选择
		public Image[] difficultySelectedIcons;
        // 询问变换词库类型的提示框
        public Transform queryChangeWordHUD;
        // 回复内购时的遮罩
		public Transform restoreMask;
        // 自动发音开启图片
        public Image pronounceOnImage;
        // 自动发音关闭图片
        public Image pronounceOffImage;
        // 恢复内购的按钮
		public Button restoreItemsButton;
        // 隐私政策按钮
		public Button privacyButton;
        // 提示框
		public TintHUD hintHUD;

        /// <summary>
        /// 初始化设置界面
        /// </summary>
        /// <param name="settings">Settings.</param>
        /// <param name="volumeChangeCallBack">Volume change call back.</param>
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

            // 只有ios上有恢复内购和隐私政策按钮
#if UNITY_IOS
			restoreItemsButton.gameObject.SetActive(true);
			privacyButton.gameObject.SetActive(true);
#elif UNITY_ANDROID
			restoreItemsButton.gameObject.SetActive(false);
			privacyButton.gameObject.SetActive(false);
#elif UNITY_EDITOR
			restoreItemsButton.gameObject.SetActive(false);
			privacyButton.gameObject.SetActive(false);
#endif

			GetComponent<Canvas> ().enabled = true;
            
		}


        /// <summary>
        /// 更新自动发音控制
        /// </summary>
        /// <param name="enable">If set to <c>true</c> enable.</param>
		public void UpdatePronounceControl(bool enable){

			if (!enable) {
				pronounceOnImage.gameObject.SetActive (false);
				pronounceOffImage.gameObject.SetActive (true);
			} else {
				pronounceOnImage.gameObject.SetActive (true);
				pronounceOffImage.gameObject.SetActive (false);
			}

		}

        /// <summary>
        /// 更新词库难度显示
        /// </summary>
		public void UpdateDifficultySelectedIcons(){
			
			int diffucultyInt = (int)GameManager.Instance.gameDataCenter.gameSettings.wordType;

            for (int i = 0; i < difficultySelectedIcons.Length; i++)
            {

                difficultySelectedIcons[i].enabled = i == diffucultyInt;

            }

		}

        /// <summary>
        /// 激活恢复内购时的遮罩
        /// </summary>
		public void ShowRestoreMask(){
			restoreMask.gameObject.SetActive(true);
		}

        /// <summary>
        /// 隐藏恢复内购的遮罩
        /// </summary>
		public void HideRestoreMask(){
			restoreMask.gameObject.SetActive(false);
		}

        /// <summary>
        /// 显示警告框
        /// </summary>
		public void ShowAlertHUD(){
			queryChangeWordHUD.gameObject.SetActive (true);
		}

        /// <summary>
        /// 隐藏警告框
        /// </summary>
		public void QuitAlertHUD(){
			queryChangeWordHUD.gameObject.SetActive (false);
		}

        /// <summary>
        /// 退出设置界面
        /// </summary>
		public void QuitSettingView(){

			GetComponent<Canvas> ().enabled = false;

		}
        

	}
}
