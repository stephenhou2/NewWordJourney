using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

	public class GuideViewController : MonoBehaviour
    {
        
		public Transform[] guideArray;

		//public Transform guideDialogView;
		//public Text guideDialogText;

		//public Transform guideHelpContainer;


		private int guideIndex;
        
		//private int guideDialogIndex;

		private CallBack guideFinishCallBack;
        

		public void ShowNewPlayerGuide(CallBack guideFinishCallBack){

			Time.timeScale = 0f;

			//guideDialogView.gameObject.SetActive(false);

			//guideHelpContainer.gameObject.SetActive(false);

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

				//for (int i = 0; i < guideArray.Length;i++){
				//	guideArray[i].gameObject.SetActive(false);
				//}

				//SetUpGuideDialogView();
				QuitNewPlayerGuide();
				return;
			}

			for (int i = 0; i < guideArray.Length;i++){            
				guideArray[i].gameObject.SetActive(guideIndex == i);
			}

		}

		//private void SetUpGuideDialogView(){
		//	guideDialogView.gameObject.SetActive(true);
		//	//guideDialogIndex = 0;
		//	//guideDialogText.text = guideDialogStrings[guideDialogIndex];

		//}



		//public void MoveToNextDialog(){
		//	guideDialogIndex++;
		//	if(guideDialogIndex >= guideDialogStrings.Length){
		//		guideDialogView.gameObject.SetActive(false);
		//		ShowHelpGuide();
		//		return;
		//	}
		//	guideDialogText.text = guideDialogStrings[guideDialogIndex];
		//}
        
		//public void ShowHelpGuide(){
		//	//guideHelpContainer.gameObject.SetActive(true);
		//}


		//public void HelpGuideConfirmAndQuitGuide(){
		//	//guideHelpContainer.gameObject.SetActive(false);
		//	QuitNewPlayerGuide();
		//}

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

