using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;
	using DG.Tweening;

	public class NPCUIController: MonoBehaviour {


		private HLHNPC npc;


		/**********  dialogPlane UI *************/
		public Text npcName;
		public Transform dialogPlane;
		public Text dialogText;


		public Transform choiceButtonModel;
		public InstancePool choiceButtonPool;
		public Transform choiceButtonContainer;

		public NPCTradeView tradeView;

		public AddGemstoneView addGemstoneView;

		public SkillLearningView npcSkillsPlane;

		public DialogView dialogView;

		public Transform mainNPCView;

		public LevelTransportView levelTransportView;

		public PropertyPromotionView propertyPromotionView;


		//private Item currentSelectedItem;

		//public float flyDuration;

		//public Vector3 goodsPlaneStartPos;

		//public Vector3 itemDetailPlaneStartPos;

		//public Vector3 specialOperationHUDStartPos;

		//public Vector3 bagItemsPlaneStartPos;

		//public int goodsPlaneMoveEndX;

		//public int itemDetailMoveEndX;

		//public int specialOpeartionHUDMoveEndX;

		//public int bagItemsPlaneMoveEndY;




		public void ShowNPCPlane(){
			gameObject.SetActive (true);
		}

		public void HideNPCPlane(){
			gameObject.SetActive (false);
		}

		public void SetUpNpcPlane(HLHNPC npc){

			this.npc = npc;

			npcName.text = npc.npcName;

			int randomSeed = Random.Range(0, npc.regularGreetings.Count);

			HLHDialog greeting = npc.regularGreetings[randomSeed];

			dialogText.text = greeting.dialogContent;

			choiceButtonPool.AddChildInstancesToPool(choiceButtonContainer);

			if(npc.isChatTriggered){
				AddChoice("交谈",SetUpDialogPlane);
			}

			if(npc.isTradeTriggered){
				AddChoice("交易", SetUpTrade);
			}

			if(npc.isTransportTriggered){
				AddChoice("传送", SetUpTransportLevelSelectView);
			}

			if(npc.isLearnSkillTriggered){
				AddChoice("学习技能", SetupSkillLearningPlane);
			}

			if(npc.isAddGemStoneTriggered){
				AddChoice("镶嵌宝石", SetUpSpecialOperation);
			}

			if(npc.isPropertyPromotionTriggered){
				AddChoice("训练", SetUpPropertyPromotionView);
			}

			AddChoice("离开",QuitNPCPlane);

			gameObject.SetActive (true);

			mainNPCView.gameObject.SetActive(true);
            
		}

		private void AddChoice(string choiceContent, CallBack choiceSelectCallBack){
                 
			Button choiceButton = choiceButtonPool.GetInstance<Button>(choiceButtonModel.gameObject, choiceButtonContainer);

			choiceButton.GetComponentInChildren<Text>().text = choiceContent;

			choiceButton.onClick.RemoveAllListeners();

			choiceButton.onClick.AddListener(delegate
			{
				choiceSelectCallBack();
			});

		}
        
			
        /// <summary>
        /// 初始化对话界面
        /// </summary>
		public void SetUpDialogPlane(){

			HLHDialogGroup dialogGroup = npc.FindQulifiedDialogGroup(Player.mainPlayer);

			dialogView.SetUpDialogView(npc, dialogGroup,ShowMainNpcView);

			mainNPCView.gameObject.SetActive(false);

		}

        /// <summary>
        /// 初始化关卡传送选择界面
        /// </summary>
		public void SetUpTransportLevelSelectView(){

			levelTransportView.SetUpLevelTransportView(npc, SelectLevelCallBack, ShowMainNpcView);
         
			mainNPCView.gameObject.SetActive(false);

		}

        /// <summary>
        /// 选择要传送的关卡后的回调
        /// </summary>
        /// <param name="level">Level.</param>
		private void SelectLevelCallBack(int level){

			ExploreManager.Instance.EnterLevel(level);

			QuitNPCPlane();
		}

        /// <summary>
        /// 显示npc主界面
        /// </summary>
		private void ShowMainNpcView(){

			mainNPCView.gameObject.SetActive(true);
		}


		public void SetUpPropertyPromotionView(){

			propertyPromotionView.SetUpPropertyPromotionView(npc, ShowMainNpcView);

			mainNPCView.gameObject.SetActive(false);
		}
      
			
		public void SetupSkillLearningPlane(){

			mainNPCView.gameObject.SetActive(false);
         
			npcSkillsPlane.SetUpSkillLearningView(npc.npcSkillIds,ShowMainNpcView);

		}


		public void SetUpTrade(){

			mainNPCView.gameObject.SetActive(false);

			tradeView.SetUpNPCTradeView(npc, ShowMainNpcView);
		}
        

		public void SetUpSpecialOperation(){

			mainNPCView.gameObject.SetActive(false);

			addGemstoneView.SetUpAddGemstoneView(npc, ShowMainNpcView);
		}



              
      


		private void PlayerGainFromNPCReward(HLHNPCReward reward){
			Player player = Player.mainPlayer;
			string tint = string.Empty;
			switch (reward.rewardType)
			{
				case HLHRewardType.Gold:
					player.totalGold += reward.rewardValue;
					ExploreManager.Instance.expUICtr.SetUpGoldGainTintHUD(reward.rewardValue);
					GameManager.Instance.soundManager.PlayAudioClip(CommonData.goldAudioName);
					break;
				case HLHRewardType.Item:
					if (reward.attachValue >= 0)
					{
						Item rewardItem = Item.NewItemWith(reward.rewardValue, 1);
						if (rewardItem.itemType == ItemType.Equipment)
						{
							Equipment eqp = rewardItem as Equipment;
							EquipmentQuality quality = (EquipmentQuality)reward.attachValue;
							eqp.ResetPropertiesByQuality(quality);
						}
						player.AddItem(rewardItem);
						ExploreManager.Instance.expUICtr.SetUpSimpleItemDetail(rewardItem);
						ExploreManager.Instance.expUICtr.UpdateBottomBar();
					}
					else
					{
						Item removeItem = Item.NewItemWith(reward.rewardValue, 1);
						player.RemoveItem(removeItem, removeItem.itemCount);
						ExploreManager.Instance.expUICtr.UpdateBottomBar();
					}
					break;
			}

		}
              

		public void QuitNPCPlane(){

			if (npc == null) {
				return;
			}
         
			dialogText.text = string.Empty;

			gameObject.SetActive (false);

			npc = null;

			MapNPC mn = ExploreManager.Instance.currentEnteredMapEvent as MapNPC;

			ExploreManager.Instance.battlePlayerCtr.isInEvent = false;

			mn.isTriggered = false;

			ExploreManager exploreManager = ExploreManager.Instance;

			exploreManager.AllWalkableEventsStartMove ();

			mn.RefreshWalkableInfoWhenQuit (false);

			exploreManager.EnableExploreInteractivity ();

		}

		
	}
}
