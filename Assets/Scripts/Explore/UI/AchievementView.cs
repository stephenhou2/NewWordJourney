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



		public void SetUpAchievementView(LearnTitleQualification qualification){

			ExploreManager.Instance.MapWalkableEventsStopAction();

			achievementText.text = qualification.title;

			qualificaitonText.text = qualification.qualificationNeed;

			rewardText.text = "x" + qualification.rewardGold.ToString();

			Player.mainPlayer.totalGold += qualification.rewardGold;

			this.gameObject.SetActive(true);

			if(zoomCoroutine != null){
				StopCoroutine(zoomCoroutine);
			}

			zoomCoroutine = HUDZoomIn();

			StartCoroutine(zoomCoroutine);
		}

		public void QuitAchievementView(){

			ExploreManager.Instance.expUICtr.UpdatePlayerGold();

			if(zoomCoroutine != null){
				StopCoroutine(zoomCoroutine);
			}

			zoomCoroutine = HUDZoomOut(ExploreManager.Instance.MapWalkableEventsStartAction);

			StartCoroutine(zoomCoroutine);

		}
       
    }
}

