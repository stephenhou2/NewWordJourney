﻿
namespace WordJourney
{

	using UnityEngine.UI;

	public class DiaryView : ZoomHUD
    {

		public Text diaryTextEN;

		public Text diaryTextCH;

		private CallBack quitCallBack;

		public void SetUpDiaryView(DiaryModel diaryModel,CallBack quitCallBack){

			GameManager.Instance.soundManager.PlayAudioClip(CommonData.paperAudioName);

			diaryTextEN.text = diaryModel.diaryEN;

			diaryTextCH.text = diaryModel.diaryCH;

			this.quitCallBack = quitCallBack;

			gameObject.SetActive(true);
         
			if(zoomCoroutine != null){
				StopCoroutine(zoomCoroutine);
			}

			zoomCoroutine = HUDZoomIn();

			StartCoroutine(zoomCoroutine);

		}

		public void QuitDiaryView(){

			if (inZoomingOut)
            {
                return;
            }

			if (zoomCoroutine != null)
            {
                StopCoroutine(zoomCoroutine);
            }

			zoomCoroutine = HUDZoomOut();

			StartCoroutine(zoomCoroutine);

			if(quitCallBack != null){
				quitCallBack();
			}
		}


    }


}

