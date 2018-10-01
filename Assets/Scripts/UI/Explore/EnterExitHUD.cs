using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

	public class EnterExitHUD : MonoBehaviour
	{

		private ExitType exitType;

		public Text queryTextOnIOS;
		public Text queryTextOnAndroid;

		public Transform exitQueryHUDOnIOS;
		public Transform exitQueryHUDOnAndroid;

		public PurchasePendingHUD purchasePendingHUD;

		public void SetUpEnterExitHUD(ExitType exitType)
		{

			this.exitType = exitType;

#if UNITY_IOS
			switch(exitType){
                case ExitType.ToLastLevel:
                    //queryTextOnIOS.text = "是否确认返回上一层?";
                    break;
                case ExitType.ToNextLevel:
                    queryTextOnIOS.text = "是否确认进入下一层?";
                    break;
            }     
			exitQueryHUDOnIOS.gameObject.SetActive(true);
			exitQueryHUDOnAndroid.gameObject.SetActive(false);
#elif UNITY_ANDROID
			switch(exitType){
                case ExitType.ToLastLevel:
					//queryTextOnAndroid.text = "观看广告可获得<color=orange>一个技能点</color>\n前往上一层前是否观看广告？";
                    break;
                case ExitType.ToNextLevel:
					queryTextOnAndroid.text = "观看广告可获得<color=orange>一个技能点</color>\n前往下一层前是否观看广告？";
                    break;
            }      
			exitQueryHUDOnIOS.gameObject.SetActive(false);
			exitQueryHUDOnAndroid.gameObject.SetActive(true);
#elif UNITY_EDITOR
			UnityEditor.BuildTarget buildTarget = UnityEditor.EditorUserBuildSettings.activeBuildTarget;

                switch (buildTarget) {
                case UnityEditor.BuildTarget.Android:
        			switch(exitType){
                        case ExitType.ToLastLevel:
			            //queryTextOnIOS.text = "观看广告可获得<color=orange>一个技能点</color>\n前往上一层前是否观看广告？?";
                            break;
                        case ExitType.ToNextLevel:
			                queryTextOnIOS.text = "观看广告可获得<color=orange>一个技能点</color>\n前往下一层前是否观看广告？?";
                            break;
                    }     
                    exitQueryHUDOnIOS.gameObject.SetActive(true);
                    exitQueryHUDOnAndroid.gameObject.SetActive(false);
                    break;
                case UnityEditor.BuildTarget.iOS:
        			switch(exitType){
                        case ExitType.ToLastLevel:
                            //queryTextOnIOS.text = "是否确认返回上一层?";
                            break;
                        case ExitType.ToNextLevel:
                            queryTextOnIOS.text = "是否确认进入下一层?";
                            break;
                    }     
                    exitQueryHUDOnIOS.gameObject.SetActive(true);
                    exitQueryHUDOnAndroid.gameObject.SetActive(false);
                    break;
                }
#endif         
			this.gameObject.SetActive(true);

		}

        /// <summary>
        /// 确认进入下一关
        /// </summary>
        /// <param name="delay">Delay.</param>
		public void ConfirmEnterWithDelay(float delay)      
        {
			//Debug.LogFormat("ConfirmEnterTime:{0}", Time.time);

			QuitEnterExitHUD();

			ExploreManager.Instance.exploreSceneReady = false;

			MapSetUpFrom from = MapSetUpFrom.LastLevel;

			ExploreManager.Instance.expUICtr.EnterLevelMaskShowAndHide(delegate{
				switch (exitType)
				{
					case ExitType.ToLastLevel:
						from = MapSetUpFrom.NextLevel;
						//ExploreManager.Instance.EnterLastLevel();
						break;
					case ExitType.ToNextLevel:
			            ExploreManager.Instance.EnterNextLevel();
						from = MapSetUpFrom.LastLevel;
						break;
				}
                ExploreManager.Instance.battlePlayerCtr.isInEvent = false;
			},from,delay);
        }
        
		public void WatchAdAndEnter(){

			purchasePendingHUD.SetUpPurchasePendingHUDOnAndroid(PurchaseManager.skill_point_id,MyAdType.CPAd,AdRewardType.SkillPoint,delegate(MyAdType adType){
			    Player.mainPlayer.skillNumLeft++;
			    GameManager.Instance.persistDataManager.AddOneSkillPointToPlayerDataFile();
				ExploreManager.Instance.DisableAllInteractivity();
			    ConfirmEnterWithDelay(1f);
			},null);

			QuitEnterExitHUD();
		}



      
		public void QuitEnterExitHUD(){

			this.gameObject.SetActive(false);
			ExploreManager.Instance.battlePlayerCtr.isInEvent = false;
		}


    }
   
}

