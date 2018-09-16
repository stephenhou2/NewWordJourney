using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;


	public class AchievementView : ZoomHUD
    {
		public Text achievementText;

		public Text qualificaitonText;

		public Text rewardText;

		public Text shareHintText;

		private LearnTitleQualification currentQulification;

		public TintHUD tintHUD;

		public void SetUpAchievementView(LearnTitleQualification qualification){

			ExploreManager.Instance.MapWalkableEventsStopAction();

			currentQulification = qualification;

			achievementText.text = qualification.title;

			qualificaitonText.text = qualification.qualificationNeed;

			rewardText.text = "x" + qualification.rewardGold.ToString();

			shareHintText.text = string.Format("分享后可获得额外{0}金币奖励", qualification.rewardGold);

			Player.mainPlayer.totalGold += qualification.rewardGold;

			this.gameObject.SetActive(true);

			if(zoomCoroutine != null){
				StopCoroutine(zoomCoroutine);
			}

			zoomCoroutine = HUDZoomIn();

			StartCoroutine(zoomCoroutine);
		}

		public void ShareLearningInfo(){

			//if(Application.internetReachability == NetworkReachability.NotReachable){
			//	tintHUD.SetUpSingleTextTintHUD("无网络连接");
			//}else{
				GameManager.Instance.soundManager.PlayAudioClip(CommonData.buttonClickAudioName);
                GameManager.Instance.UIManager.SetUpCanvasWith(CommonData.shareCanvasBundleName, "ShareCanvas", () =>
                {
                    TransformManager.FindTransform("ShareCanvas").GetComponent<ShareViewController>().SetUpShareView(ShareType.WeChat, ShareSucceedCallBack, ShareFailedCallBack, null);
                    //QuitAchievementView();
                }, false, true);

			//}         
		}

		public void ShareSucceedCallBack(){

			Player.mainPlayer.totalGold += currentQulification.rewardGold;

			QuitAchievementView(delegate 
			{
				string hintStr = string.Format("分享成功，获得{0}金币", currentQulification.rewardGold);

                tintHUD.SetUpSingleTextTintHUD(hintStr);
			});         

		}

		public void ShareFailedCallBack(){

			QuitAchievementView(delegate
			{
				string hintStr = "分享失败，请稍后重试";

				tintHUD.SetUpSingleTextTintHUD(hintStr);
			});
		}
        
		private void QuitAchievementView(CallBack quitCallBack){
			
			ExploreManager.Instance.expUICtr.UpdatePlayerGold();

            if (zoomCoroutine != null)
            {
                StopCoroutine(zoomCoroutine);
            }

			zoomCoroutine = null;
         
			if(quitCallBack != null){
				quitCallBack();
			}

			ExploreManager.Instance.MapWalkableEventsStartAction();

			gameObject.SetActive(false);

		}

		public void OnQuitButtonClick(){

			ExploreManager.Instance.expUICtr.UpdatePlayerGold();

            if (zoomCoroutine != null)
            {
                StopCoroutine(zoomCoroutine);
            }

            zoomCoroutine = HUDZoomOut(ExploreManager.Instance.MapWalkableEventsStartAction);

            StartCoroutine(zoomCoroutine);

		}
       
    }
}

