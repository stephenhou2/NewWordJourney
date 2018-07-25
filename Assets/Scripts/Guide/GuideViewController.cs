using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	
	public class GuideViewController : MonoBehaviour
    {

		public Transform[] guideArray;

		private int guideIndex;

		private CallBack guideFinishCallBack;

		public void ShowNewPlayerGuide(CallBack guideFinishCallBack){

			Time.timeScale = 0f;

			this.guideFinishCallBack = guideFinishCallBack;

			guideIndex = 0;

			for (int i = 0; i < guideArray.Length; i++)
            {
                guideArray[i].gameObject.SetActive(guideIndex == i);
            }


		}

		public void ShowNextGuide(){

			guideIndex++;

			if(guideIndex >= guideArray.Length){
				QuitNewPlayerGuide();
				return;
			}

			for (int i = 0; i < guideArray.Length;i++){            
				guideArray[i].gameObject.SetActive(guideIndex == i);
			}

		}


		private void QuitNewPlayerGuide(){

			Time.timeScale = 1f;

			GameManager.Instance.gameDataCenter.gameSettings.newPlayerGuideFinished = true;

			GameManager.Instance.persistDataManager.SaveGameSettings();

			GameManager.Instance.UIManager.RemoveCanvasCache("GuideCanvas");

			if(guideFinishCallBack != null){
				guideFinishCallBack();
			}
		}

		public void DestroyInstances(){
			
			GetComponent<Canvas>().enabled = false;

			MyResourceManager.Instance.UnloadAssetBundle(CommonData.guideCanvasBundleName, true);

            Destroy(this.gameObject, 0.3f);

		}



        
    }


}

