using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class SaveDataHintView : ZoomHUD
    {
      
		private CallBack zoomInCallBack;

		private CallBack zoomOutCallBack;

		public void SetUpSaveDataHintView(CallBack zoomInCallBack, CallBack zoomOutCallBack){

			this.zoomInCallBack = zoomInCallBack;
			this.zoomOutCallBack = zoomOutCallBack;

			this.gameObject.SetActive(true);

			if(zoomCoroutine != null){
				StopCoroutine(zoomCoroutine);
			}

			zoomCoroutine = HUDZoomIn(SaveExploreData);

			StartCoroutine(zoomCoroutine);
            
		}

		private void SaveExploreData(){
			
			if(zoomInCallBack != null){
				zoomInCallBack();
			}

			IEnumerator latelyQuit = LatelyQuitSaveDataHintView();

			StartCoroutine(latelyQuit);

		}

		private IEnumerator LatelyQuitSaveDataHintView(){

			yield return new WaitForSecondsRealtime(0.5f);

			QuitSaveDataHintView();
         
		}

		private void QuitSaveDataHintView(){
		
			if(zoomCoroutine != null){
				StopCoroutine(zoomCoroutine);

			}

			zoomCoroutine = HUDZoomOut(zoomOutCallBack);

			StartCoroutine(zoomCoroutine);

		}





        
    }
}

