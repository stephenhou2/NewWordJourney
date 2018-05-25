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

		private LevelTransportSelectCallBack levelSelectCallBack;
		private CallBack quitCallBack;

		private int transportCost = 100;

		private int choiceCountInOnePage = 10;

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

			for (int i = 0; i < npc.transportLevelList.Count; i++)
			{
				

				int transportLevel = npc.transportLevelList[i];

				if (transportLevel <= Player.mainPlayer.maxUnlockLevelIndex && transportLevel != Player.mainPlayer.currentLevelIndex){
					maxLevelChoiceIndex = i;
				}else{
					continue;
				}

				Button levelChoice = levelChoicePool.GetInstance<Button>(levelChoiceModel.gameObject, levelChoiceContainer);

				levelChoice.GetComponentInChildren<Text>().text = string.Format("第{0}层", transportLevel+1);

				levelChoice.onClick.RemoveAllListeners();
                            
				levelChoice.onClick.AddListener(delegate
				{
					Player.mainPlayer.totalGold -= transportCost;
                    
					if(levelSelectCallBack != null){
						levelSelectCallBack(transportLevel);
					}               
				});
            
			}

			upButton.gameObject.SetActive(false);

			if(maxLevelChoiceIndex - minLevelChoiceIndex > choiceCountInOnePage){
				downButton.gameObject.SetActive(true);
			}else{
				downButton.gameObject.SetActive(false);
			}


			this.gameObject.SetActive(true);

		}

		public void OnUpButtonClick(){

			minLevelChoiceIndex--;


			if(minLevelChoiceIndex == 0){
				upButton.gameObject.SetActive(false);
			}

			if (maxLevelChoiceIndex - minLevelChoiceIndex > choiceCountInOnePage)
            {
                downButton.gameObject.SetActive(true);
            }

            else
            {
                downButton.gameObject.SetActive(false);
            }


			float choiceContainerPosY = levelChoiceContainer.localPosition.y;

			float newChoiceContainerPosY = choiceContainerPosY - cellHeight - cellSpacing;

			levelChoiceContainer.localPosition = new Vector3(0, newChoiceContainerPosY, 0);

		}

		public void OnDownButtonClick(){
			
			minLevelChoiceIndex++;

			upButton.gameObject.SetActive(true);

			float choiceContainerPosY = levelChoiceContainer.localPosition.y;

            float newChoiceContainerPosY = choiceContainerPosY + cellHeight + cellSpacing;

            levelChoiceContainer.localPosition = new Vector3(0, newChoiceContainerPosY, 0);

			if (maxLevelChoiceIndex - minLevelChoiceIndex > choiceCountInOnePage)
            {
                downButton.gameObject.SetActive(true);
            }
            else
            {
                downButton.gameObject.SetActive(false);
            }

		}


		public void OnBackButtonClick(){

			if(quitCallBack != null){
				quitCallBack();
			}

			this.gameObject.SetActive(false);

		}
        
    }
   
}
