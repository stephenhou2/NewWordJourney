using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;


	public class IdentifyViewController : ZoomHUD
    {

		public Transform pendingView;

		public Transform pendingIcon;

		public Text pendingHint;

		public TintHUD hintHUD;

		public InputField infoInputField;

		public Image mask;

		private CallBack identifySuccessCallBack;

		private int rotateSpeed = 45;

        private float rotateInterval = 0.15f;

		private IEnumerator pendingRotateCoroutine;

		private IEnumerator identifyCoroutine;


		public void SetUpIdentifyView(CallBack identifySuccessCallBack){

			Time.timeScale = 0;

			this.identifySuccessCallBack = identifySuccessCallBack;

			if(zoomCoroutine != null){
				StopCoroutine(zoomCoroutine);
			}

			zoomCoroutine = HUDZoomIn();

			StartCoroutine(zoomCoroutine);
            
			mask.enabled = false;

			infoInputField.ActivateInputField();

			TouchScreenKeyboard.Open("");
         
		}


		public void OnCheckButtonClick(){

			mask.enabled = true;

			Pending();

			string qq = infoInputField.text;

			string deviceType = SystemInfo.deviceModel;
         
			if(identifyCoroutine != null){
				StopCoroutine(identifyCoroutine);
			}

			identifyCoroutine = Identify(qq, deviceType);

			StartCoroutine(identifyCoroutine);

		}

		private void Pending(){

			pendingView.gameObject.SetActive(true);

			pendingIcon.gameObject.SetActive(true);

			pendingHint.enabled = true;

			if(pendingRotateCoroutine != null){
				StopCoroutine(pendingRotateCoroutine);
			}

			pendingRotateCoroutine = PendingIconRotate();

			StartCoroutine(pendingRotateCoroutine);



		}

		private IEnumerator Identify(string qq,string deviceType){

			yield return new WaitForSecondsRealtime(1.0f);

			bool qualified = MyTool.CheckDeviceQulified(qq, deviceType);

			StopPending();

			string hint = qualified ? "验证成功" : "验证失败";

			hintHUD.SetUpSingleTextTintHUD(hint);

			yield return new WaitForSecondsRealtime(1.2f);

			QuitPending();

			if(qualified && identifySuccessCallBack != null){
				mask.enabled = true;
				identifySuccessCallBack();
			}else{
				mask.enabled = false;
			}         
		}

		private IEnumerator PendingIconRotate(){

			pendingIcon.localRotation = Quaternion.identity;

            yield return null;

            int count = 0;

            while (true)
            {

                Vector3 newRotation = new Vector3(0, 0, -rotateSpeed * count);

				pendingIcon.localRotation = Quaternion.Euler(newRotation);

                yield return new WaitForSecondsRealtime(rotateInterval);

                count++;

            }

		}

		private void StopPending(){

			if (pendingRotateCoroutine != null)
            {
                StopCoroutine(pendingRotateCoroutine);
            }

			pendingIcon.gameObject.SetActive(false);

			pendingHint.enabled = false;

		}

		private void QuitPending(){
			pendingView.gameObject.SetActive(false);
		}


		public void DestroyInstances(){
			GetComponent<Canvas>().enabled = false;

			MyResourceManager.Instance.UnloadAssetBundle(CommonData.identifyCanvasBundleName, true);

            Destroy(this.gameObject, 0.3f);
		}
        
    }
}

