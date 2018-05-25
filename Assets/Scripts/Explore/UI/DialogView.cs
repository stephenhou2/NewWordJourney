using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;

	public class DialogView : MonoBehaviour
    {
		public Image npcIcon;

		public Text dialogText;

		public TintHUD tintHUD;

		private HLHNPC npc;

		private HLHDialogGroup dialogGroup;

		private int currentDialogId;

		private CallBack quitCallBack;

		private bool dialogHasFinished;
        

		public void SetUpDialogView(HLHNPC npc,HLHDialogGroup dialogGroup,CallBack quitCallBack){

			this.npc = npc;

			this.dialogGroup = dialogGroup;

			this.quitCallBack = quitCallBack;

			//npcIcon.sprite = GameManager.Instance.gameDataCenter.
			npcIcon.enabled = true;

			dialogText.text = dialogGroup.dialogs[0].dialogContent;

			currentDialogId = 0;

			dialogHasFinished = false;

			this.gameObject.SetActive(true);
         
		}

		public void NextDialog(){

			if(dialogHasFinished){
				return;
			}

			int nextDialogId = ++currentDialogId;

			if(nextDialogId < dialogGroup.dialogs.Count){

				HLHDialog dialog = dialogGroup.dialogs[nextDialogId];

				dialogText.text = dialog.dialogContent;

				return;
			}

			if(!dialogGroup.isMultiTimes){
				GameManager.Instance.persistDataManager.SaveChatRecords(npc.npcId, dialogGroup.dialogGroupId);
			}
         
			if(dialogGroup.isRewardTriggered){

				HLHNPCReward reward = dialogGroup.reward;

				switch(reward.rewardType){
					case HLHRewardType.Gold:
						int goldReward = reward.rewardValue;
						Player.mainPlayer.totalGold += goldReward;
						tintHUD.SetUpGoldTintHUD(goldReward);
						break;
					case HLHRewardType.Item:
						Item rewardItem = Item.NewItemWith(reward.rewardValue, reward.attachValue);

						if(rewardItem.itemType == ItemType.Equipment){

							Equipment equipment = rewardItem as Equipment;

							if(equipment.defaultQuality.Equals(EquipmentDefaultQuality.Random)){
								equipment.ResetPropertiesByQuality(EquipmentQuality.Gray);
							}

						}

						Player.mainPlayer.AddItem(rewardItem);
						string tint = string.Format("获得{0}x{1}", rewardItem.itemName, rewardItem.itemCount);
						tintHUD.SetUpSingleTextTintHUD(tint);
						break;
				}

				dialogHasFinished = true;

				StartCoroutine("LatelyQuitDialogPlane");

				return;
			}

			QuitDialogPlane();

		}

		private IEnumerator LatelyQuitDialogPlane(){

			yield return new WaitForSeconds(1.0f);

			QuitDialogPlane();
         
		}

		public void QuitDialogPlane(){

			npcIcon.sprite = null;
			npcIcon.enabled = false;
            
			dialogText.text = string.Empty;

			currentDialogId = 0;

			dialogGroup = null;

			this.gameObject.SetActive(false);

			if(quitCallBack != null){

				quitCallBack();
			}

		}
        
    }

}

