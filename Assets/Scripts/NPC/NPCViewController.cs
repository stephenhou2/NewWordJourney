using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;
	using DG.Tweening;

	public class NPCViewController: MonoBehaviour {


		private HLHNPC npc;


		/**********  dialogPlane UI *************/
		public Text npcName;
		public Transform dialogPlane;
		public Text dialogText;


		public Transform choiceButtonModel;
		public InstancePool choiceButtonPool;
		public Transform choiceButtonContainer;

		public NPCGoodsTradeView goodsTradeView;

		public AddGemstoneView addGemstoneView;

		public SkillTradeView skillsTradeView;

		public DialogView dialogView;

		public Transform mainNPCView;

		public LevelTransportView levelTransportView;

		//public PropertyPromotionView propertyPromotionView;

		public TintHUD tintHUD;
      
		public void SetUpNPCView(HLHNPC npc){

			this.npc = npc;

            npcName.text = npc.npcName;

            int randomSeed = Random.Range(0, npc.regularGreetings.Count);

            HLHDialog greeting = npc.regularGreetings[randomSeed];

            dialogText.text = greeting.dialogContent;

            choiceButtonPool.AddChildInstancesToPool(choiceButtonContainer);

            if (npc.isChatTriggered)
            {
                AddChoice("交谈", SetUpDialogPlane);
            }

            if (npc.isTradeTriggered)
            {
                AddChoice("交易", SetUpTrade);
            }

            if (npc.isTransportTriggered)
            {
                AddChoice("传送", SetUpTransportLevelSelectView);
            }

            if (npc.isLearnSkillTriggered)
            {
                AddChoice("学习技能", SetupSkillLearningPlane);
            }

            if (npc.isAddGemStoneTriggered)
            {
                AddChoice("镶嵌宝石", SetUpSpecialOperation);
            }

            //if (npc.isPropertyPromotionTriggered)
            //{
            //    AddChoice("训练", SetUpPropertyPromotionView);
            //}

            AddChoice("离开", QuitNPCPlane);

			ExploreManager.Instance.expUICtr.ShowTopBarMask();


            GetComponent<Canvas>().enabled = true;

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

			QuitNPCPlane();

			//ExploreManager.Instance.expUICtr.transitionMask.gameObject.SetActive(true);

			ExploreManager.Instance.expUICtr.EnterLevelMaskShowAndHide(delegate
			{
				ExploreManager.Instance.EnterLevel(level, ExitType.NextLevel);
				//ExploreManager.Instance.expUICtr.transitionMask.gameObject.SetActive(false);
			});

		}


        /// <summary>
        /// 显示npc主界面
        /// </summary>
		private void ShowMainNpcView(){

			mainNPCView.gameObject.SetActive(true);
		}


		//public void SetUpPropertyPromotionView(){

		//	propertyPromotionView.SetUpPropertyPromotionView(npc, ShowMainNpcView);

		//	mainNPCView.gameObject.SetActive(false);
		//}
      
			
		public void SetupSkillLearningPlane(){

			mainNPCView.gameObject.SetActive(false);
         
			skillsTradeView.SetUpSkillLearningView(npc.npcSkillIds,ShowMainNpcView);

		}


		public void SetUpTrade(){

			mainNPCView.gameObject.SetActive(false);

			goodsTradeView.SetUpNPCTradeView(npc, ShowMainNpcView);
		}
        

		public void SetUpSpecialOperation(){

			mainNPCView.gameObject.SetActive(false);

			addGemstoneView.SetUpAddGemstoneView(npc, ShowMainNpcView);
		}


        
              

		public void QuitNPCPlane(){

			ExploreManager.Instance.expUICtr.HideTopBarMask();

			if (npc == null) {
				return;
			}
         
			dialogText.text = string.Empty;

			npc = null;

			MapNPC mn = ExploreManager.Instance.currentEnteredMapEvent as MapNPC;

			ExploreManager.Instance.battlePlayerCtr.isInEvent = false;
			ExploreManager.Instance.battlePlayerCtr.boxCollider.enabled = true;

			mn.isTriggered = false;

			ExploreManager exploreManager = ExploreManager.Instance;

			exploreManager.MapWalkableEventsStartAction ();

			mn.RefreshWalkableInfoWhenQuit (false);

			exploreManager.EnableExploreInteractivity ();

			GameManager.Instance.UIManager.HideCanvas("NPCCanvas");

		}

		public void DestroyInstances()
		{
			MyResourceManager.Instance.UnloadAssetBundle(CommonData.npcCanvasBundleName, true);

            Destroy(this.gameObject, 0.3f);
		}


	}
}
