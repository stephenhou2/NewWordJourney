using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

    /// <summary>
    /// 用户引导页控制器
    /// </summary>
	public class GuideViewController : MonoBehaviour
    {
        
        // 引导队列【引导为对UI和玩法的一个一个的显示】
		public Transform[] guideArray;
      
        // 当前引导序号
		private int guideIndex;
      
        // 引导结束的回调
		private CallBack guideFinishCallBack;
        
        /// <summary>
        /// 显示用户引导界面
        /// </summary>
        /// <param name="guideFinishCallBack">Guide finish call back.</param>
		public void ShowNewPlayerGuide(CallBack guideFinishCallBack){

			Time.timeScale = 0f;

			this.guideFinishCallBack = guideFinishCallBack;

            // 从第0个引导开始显示
			guideIndex = 0;

			for (int i = 0; i < guideArray.Length; i++)
            {
                guideArray[i].gameObject.SetActive(guideIndex == i);
            }


		}

        /// <summary>
        /// 显示下一个引导
        /// </summary>
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

        /// <summary>
        /// 退出用户引导界面
        /// </summary>
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

