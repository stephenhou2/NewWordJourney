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

		public Text npcName;

		public TintHUD tintHUD;

		public SimpleItemDetail simpleItemDetail;
        

		public Sprite wiseManSprite;
		public Sprite youXiaSprite;

		private HLHNPC npc;

		private HLHDialogGroup dialogGroup;

		private int currentDialogId;

		private CallBack quitCallBack;

		private bool dialogHasFinished;
        

		public void SetUpDialogView(HLHNPC npc,HLHDialogGroup dialogGroup,CallBack quitCallBack){

			this.npc = npc;

			this.dialogGroup = dialogGroup;

			this.quitCallBack = quitCallBack;
            
			bool hasNpcIcon = false;

			if(npc.npcName.Equals("智者·艾尔文")){
				hasNpcIcon = true;
				npcIcon.sprite = wiseManSprite;
			}

			if(npc.npcName.Equals("游侠·安科尔")){
				hasNpcIcon = true;
				npcIcon.sprite = youXiaSprite;
			}
         
			npcIcon.enabled = hasNpcIcon;

			npcName.text = npc.npcName;

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

						ExploreManager.Instance.newMapGenerator.SetUpRewardInMap(rewardItem,ExploreManager.Instance.battlePlayerCtr.transform.position);

						if(!Player.mainPlayer.CheckBagFull(rewardItem)){
							Player.mainPlayer.AddItem(rewardItem);
							simpleItemDetail.SetupSimpleItemDetail(rewardItem);
							ExploreManager.Instance.expUICtr.UpdateBottomBar();
						}
						break;
				}

				if (!dialogGroup.isMultiTimes)
                {
                    GameManager.Instance.persistDataManager.SaveChatRecords(npc.npcId, dialogGroup.dialogGroupId);
                    GameManager.Instance.persistDataManager.SaveCompletePlayerData();
                }

				dialogHasFinished = true;

				QuitDialogPlane();

				return;
			}
                     
			//if(Player.mainPlayer.currentLevelIndex == CommonData.maxLevel && dialogGroup.triggerLevel == CommonData.maxLevel){

   //             this.gameObject.SetActive(false);
            
			//	GameManager.Instance.UIManager.SetUpCanvasWith(CommonData.shareCanvasBundleName, "ShareCanvas", delegate
   //             {
   //                 TransformManager.FindTransform("ShareCanvas").GetComponent<ShareViewController>().SetUpShareView(ShareType.WeChat, null, null, null);
   //             });

			//	return;
			//}

			QuitDialogPlane();
         
		}
        

		public void QuitDialogPlane(){

			npcIcon.sprite = null;
			npcIcon.enabled = false;
            
			dialogText.text = string.Empty;

			currentDialogId = 0;

			dialogGroup = null;
         
			if(quitCallBack != null){

				quitCallBack();
			}

			this.gameObject.SetActive(false);

		}
        
    }

}

