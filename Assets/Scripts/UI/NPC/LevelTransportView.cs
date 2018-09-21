using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

	public delegate void LevelTransportSelectCallBack(int levelSelected);
   
	public class LevelTransportView : ZoomHUD
    {
		public InstancePool levelChoicePool;
		public Transform levelChoiceContainer;
		public Button levelChoiceModel;

		//public Button upButton;
		//public Button downButton;

		public TintHUD tintHUD;

		public Text noValidLevelHint;

		private LevelTransportSelectCallBack levelSelectCallBack;
		private CallBack quitCallBack;

		private int transportCost = 100;

		private int choiceCountInOnePage = 4;

		private int minLevelChoiceIndex = 0;
		private int maxLevelChoiceIndex = 0;
       

		public int cellHeight;

		public int cellSpacing;
       

		public void SetUpLevelTransportView(HLHNPC npc,LevelTransportSelectCallBack levelSelectCallBack, CallBack quitCallBack){
                 
			this.levelSelectCallBack = levelSelectCallBack;
			this.quitCallBack = quitCallBack;

			this.gameObject.SetActive(true);

			List<int> validTravelLevelIds = npc.GetValidTravelLevelIds();
          
			levelChoiceContainer.localPosition = Vector3.zero;

			levelChoicePool.AddChildInstancesToPool(levelChoiceContainer);
                     
			bool hasValidTranportLevel = validTravelLevelIds.Count > 0;

			for (int i = 0; i < validTravelLevelIds.Count; i++)
			{            
				int transportLevel = validTravelLevelIds[i];

				Button levelChoice = levelChoicePool.GetInstance<Button>(levelChoiceModel.gameObject, levelChoiceContainer);

				levelChoice.GetComponentInChildren<Text>().text = string.Format("第{0}层", transportLevel+1);

				levelChoice.onClick.RemoveAllListeners();
                            
				levelChoice.onClick.AddListener(delegate
				{
					if(Player.mainPlayer.totalGold < transportCost){
						tintHUD.SetUpSingleTextTintHUD("金币不足");
						return;
					}

					Player.mainPlayer.totalGold -= transportCost;

					GameManager.Instance.gameDataCenter.currentMapEventsRecord.Reset();
					GameManager.Instance.gameDataCenter.currentMapWordRecords.Clear();
                    
					if(levelSelectCallBack != null){
						levelSelectCallBack(transportLevel);
					}    

					this.gameObject.SetActive(false);
				});
            
			}


			noValidLevelHint.enabled = !hasValidTranportLevel;
			//upButton.gameObject.SetActive(hasValidTranportLevel);
			//downButton.gameObject.SetActive(hasValidTranportLevel);

			if(zoomCoroutine != null){
				StopCoroutine(zoomCoroutine);
			}

			zoomCoroutine = HUDZoomIn();

			StartCoroutine(zoomCoroutine);
         
		}

		//public void OnUpButtonClick(){

		//	if (minLevelChoiceIndex == 0)
  //          {
  //              return;
  //          }

		//	minLevelChoiceIndex--;         
         

		//	float choiceContainerPosY = levelChoiceContainer.localPosition.y;

		//	float newChoiceContainerPosY = choiceContainerPosY - cellHeight - cellSpacing;

		//	levelChoiceContainer.localPosition = new Vector3(0, newChoiceContainerPosY, 0);

		//}

		//public void OnDownButtonClick(){

		//	if(maxLevelChoiceIndex - minLevelChoiceIndex < choiceCountInOnePage){
		//		return;
		//	}
			
		//	minLevelChoiceIndex++;

		//	float choiceContainerPosY = levelChoiceContainer.localPosition.y;

  //          float newChoiceContainerPosY = choiceContainerPosY + cellHeight + cellSpacing;

  //          levelChoiceContainer.localPosition = new Vector3(0, newChoiceContainerPosY, 0);
         
		//}


		public void OnBackButtonClick(){

			if(zoomCoroutine != null){
				StopCoroutine(zoomCoroutine);
			}

			zoomCoroutine = HUDZoomOut(quitCallBack);

			StartCoroutine(zoomCoroutine);

		}
        
    }
   
}
