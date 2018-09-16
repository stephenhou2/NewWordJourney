using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

	public class EnterExitHUD : MonoBehaviour
    {

		private ExitType exitType;

		public Text queryText;

		public void SetUpEnterExitHUD(ExitType exitType){

			this.exitType = exitType;

			switch(exitType){
				case ExitType.ToLastLevel:
					//queryText.text = "是否确认返回上一层?";
					break;
				case ExitType.ToNextLevel:
					queryText.text = "是否确认进入下一层?";
					break;
			}

			this.gameObject.SetActive(true);

		}

		public void ConfirmEnter()      
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
			},from);
        }
        

        public void CancelEnter()
        {
			QuitEnterExitHUD();
            ExploreManager.Instance.battlePlayerCtr.isInEvent = false;
        }

		public void QuitEnterExitHUD(){

			this.gameObject.SetActive(false);

		}


    }
   
}

