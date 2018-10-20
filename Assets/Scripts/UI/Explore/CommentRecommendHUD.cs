using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{

	public class CommentRecommendHUD : ZoomHUD
	{


		public void SetUpCommentRecommendHUD()
		{

			gameObject.SetActive(true);

			ExploreManager.Instance.MapWalkableEventsStopAction();

			ExploreManager.Instance.battlePlayerCtr.StopMoveAtEndOfCurrentStep();

			ExploreManager.Instance.battlePlayerCtr.isInEvent = true;

			if (zoomCoroutine != null)
			{
				StopCoroutine(zoomCoroutine);
			}

			zoomCoroutine = HUDZoomIn(null);

			StartCoroutine(zoomCoroutine);

			ApplicationInfo.Instance.hasRecommentPushed = true;

			GameManager.Instance.persistDataManager.SaveApplicationInfo();
         
		}

		/// <summary>
		/// 评论按钮点击响应
		/// </summary>
		public void GoComment()
		{
#if UNITY_IOS
			const string APP_ID = "1340788161";
			var url = string.Format("itms-apps://itunes.apple.com/cn/app/id{0}?mt=8&action=write-review", APP_ID);
			Application.OpenURL(url);
			DirectlyQuit();
#elif UNITY_ANDROID
			Application.OpenURL("https://www.taptap.com/app/141656");
			DirectlyQuit();
#elif UNITY_EDITOR
			UnityEditor.BuildTarget buildTarget = UnityEditor.EditorUserBuildSettings.activeBuildTarget;
			switch (buildTarget)
            {
                case UnityEditor.BuildTarget.Android:
        			Application.OpenURL("https://www.taptap.com/app/141656");
                    DirectlyQuit();
                    break;
                case UnityEditor.BuildTarget.iOS:
        			const string APP_ID = "1340788161";
                    var url = string.Format("itms-apps://itunes.apple.com/cn/app/id{0}?mt=8&action=write-review", APP_ID);
                    Application.OpenURL(url);
                    DirectlyQuit();
                    break;
            }
#endif
		}

		private void DirectlyQuit(){
			if (zoomCoroutine != null)
            {
                StopCoroutine(zoomCoroutine);
            }

			ExploreManager.Instance.MapWalkableEventsStartAction();

			ExploreManager.Instance.battlePlayerCtr.isInEvent = false;

			gameObject.SetActive(false);
		}

		public void QuitCommentRecommendHUD(){

			if (inZoomingOut)
            {
                return;
            }

			if(zoomCoroutine != null){
				StopCoroutine(zoomCoroutine);
			}

			zoomCoroutine = HUDZoomOut(ExploreManager.Instance.MapWalkableEventsStartAction);

			StartCoroutine(zoomCoroutine);

			ExploreManager.Instance.battlePlayerCtr.isInEvent = false;
         
		}


	}

}
