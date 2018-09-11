using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

	public delegate void LevelTransportSelectCallBack(int levelSelected);
   
	public class LevelTransportView : MonoBehaviour
    {
		public InstancePool levelChoicePool;
		public Transform levelChoiceContainer;
		public Button levelChoiceModel;

		public Button upButton;
		public Button downButton;

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
            
			minLevelChoiceIndex = 0;
          
			levelChoiceContainer.localPosition = Vector3.zero;

			levelChoicePool.AddChildInstancesToPool(levelChoiceContainer);

			bool showUpAndDownButton = false;

			if(npc.transportLevelList == null || npc.transportLevelList.Count == 0){
				noValidLevelHint.enabled = true;
				return;
			}else{
				noValidLevelHint.enabled = false;
			}


			for (int i = 0; i < npc.transportLevelList.Count; i++)
			{            
				int transportLevel = npc.transportLevelList[i];

				if (transportLevel <= Player.mainPlayer.maxUnlockLevelIndex && transportLevel != Player.mainPlayer.currentLevelIndex){
					maxLevelChoiceIndex = i;
				}else{
					continue;
				}

				showUpAndDownButton = true;

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
                    
					if(levelSelectCallBack != null){
						levelSelectCallBack(transportLevel);
					}    

					this.gameObject.SetActive(false);
				});
            
			}

			upButton.gameObject.SetActive(showUpAndDownButton);
			downButton.gameObject.SetActive(showUpAndDownButton);
         
			this.gameObject.SetActive(true);

		}

		public void OnUpButtonClick(){

			if (minLevelChoiceIndex == 0)
            {
                return;
            }

			minLevelChoiceIndex--;         
         

			float choiceContainerPosY = levelChoiceContainer.localPosition.y;

			float newChoiceContainerPosY = choiceContainerPosY - cellHeight - cellSpacing;

			levelChoiceContainer.localPosition = new Vector3(0, newChoiceContainerPosY, 0);

		}

		public void OnDownButtonClick(){

			if(maxLevelChoiceIndex - minLevelChoiceIndex < choiceCountInOnePage){
				return;
			}
			
			minLevelChoiceIndex++;

			float choiceContainerPosY = levelChoiceContainer.localPosition.y;

            float newChoiceContainerPosY = choiceContainerPosY + cellHeight + cellSpacing;

            levelChoiceContainer.localPosition = new Vector3(0, newChoiceContainerPosY, 0);
         
		}


		public void OnBackButtonClick(){

			if(quitCallBack != null){
				quitCallBack();
			}

			this.gameObject.SetActive(false);

		}
        
    }
   
}
