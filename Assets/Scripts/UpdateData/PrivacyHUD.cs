using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

	public class PrivacyHUD : ZoomHUD
    {

		private CallBack agreeCallBack;

		private CallBack disagreeCallBack;


		public void SetUpPrivacyHUD(CallBack agreeCallBack,CallBack disagreeCallBack){

			this.agreeCallBack = agreeCallBack;

			this.disagreeCallBack = disagreeCallBack;

			this.gameObject.SetActive(true);

			if(zoomCoroutine != null){
				StopCoroutine(zoomCoroutine);
			}

			zoomCoroutine = HUDZoomIn();

			StartCoroutine(zoomCoroutine);

		}

		public void OnAgreeButtonClick(){

			ApplicationInfo.Instance.agreePrivacyStrategyOnIos = true;

            DataHandler.SaveInstanceDataToFile<ApplicationInfo>(ApplicationInfo.Instance, CommonData.applicationInfoFilePath);

			QuitPrivacyHUD(true);
		}

		public void OnDisagreeButtonClick(){

			QuitPrivacyHUD(false);

		}

		public void OnPrivacyLinkageClick(){

			Application.OpenURL("http://www.lofter.com/lpost/1fc6f75f_12b25c509");

		}

		private void QuitPrivacyHUD(bool agree){

			if(zoomCoroutine != null){
				StopCoroutine(zoomCoroutine);
			}

			zoomCoroutine = HUDZoomOut(delegate {
			
				if(agree){
					if(agreeCallBack != null){
						agreeCallBack();
					}
				}else{
					if(disagreeCallBack != null){
						disagreeCallBack();
					}
				}

			});

			StartCoroutine(zoomCoroutine);

		}

    }
}

