using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;
    
    /// <summary>
    /// npc交互界面控制器
    /// </summary>
	public class NPCViewController: MonoBehaviour {


		private HLHNPC npc;


		/**********  dialogPlane UI *************/
		public Text npcName;
		public Transform dialogPlane;
		public Text dialogText;

        // npc主界面选择按钮模型
		public Transform choiceButtonModel;
        // 选择按钮缓存池
		public InstancePool choiceButtonPool;
        // 选择按钮容器
		public Transform choiceButtonContainer;

        // 商品交易界面
		public NPCGoodsTradeView goodsTradeView;

        // 镶嵌宝石界面
		public AddGemstoneView addGemstoneView;

        // 学习技能界面
		public SkillTradeView skillsTradeView;

        // 对话界面
		public DialogView dialogView;

        // npc交互主界面
		public Transform mainNPCView;

        // 关卡传送界面
		public LevelTransportView levelTransportView;

        // 提示文本
		public TintHUD tintHUD;
      
        // 初始化npc界面
		public void SetUpNPCView(HLHNPC npc){

			this.npc = npc;

            npcName.text = npc.npcName;

            // 随机npc的问候语序号
            int randomSeed = Random.Range(0, npc.regularGreetings.Count);

            // 获取npc的问候语
            HLHDialog greeting = npc.regularGreetings[randomSeed];

            dialogText.text = greeting.dialogContent;

            // npc交互主界面上的选项卡先全都放入缓存池
            choiceButtonPool.AddChildInstancesToPool(choiceButtonContainer);

            // 有交谈功能的话添加一个交谈选项卡
            if (npc.isChatTriggered)
            {
                AddChoice("交谈", SetUpDialogPlane);
            }
            // 有交易功能的话添加一个交易选项卡
            if (npc.isTradeTriggered)
            {
                AddChoice("交易", SetUpTrade);
            }
            // 有传送功能的话添加一个传送选项卡
            if (npc.isTransportTriggered)
            {
                AddChoice("传送", SetUpTransportLevelSelectView);
            }
            // 有学习技能功能的话添加一个学习技能选项卡
            if (npc.isLearnSkillTriggered)
            {
                AddChoice("学习技能", SetupSkillLearningPlane);
            }
            // 有镶嵌宝石功能的话添加一个镶嵌宝石选项卡
            if (npc.isAddGemStoneTriggered)
            {
                AddChoice("镶嵌宝石", SetUpSpecialOperation);
            }
            
            // 添加离开选项卡
            AddChoice("离开", QuitNPCPlane);

            // 显示探索场景顶部遮罩【由于npc画布高于探索画布，且正常显示范围在顶部bar以下，所以只在npc画布上设置遮罩会导致探索界面中顶部有部分区域是高亮的】
			ExploreManager.Instance.expUICtr.ShowTopBarMask();


            GetComponent<Canvas>().enabled = true;

            mainNPCView.gameObject.SetActive(true);
            
		}

        /// <summary>
        /// 添加选项卡
        /// </summary>
        /// <param name="choiceContent">选项卡显示名称.</param>
        /// <param name="choiceSelectCallBack">点击响应/param>
		private void AddChoice(string choiceContent, CallBack choiceSelectCallBack){
                 
			Button choiceButton = choiceButtonPool.GetInstance<Button>(choiceButtonModel.gameObject, choiceButtonContainer);

			choiceButton.GetComponentInChildren<Text>().text = choiceContent;

            // 移除原有事件监听
			choiceButton.onClick.RemoveAllListeners();

            // 添加事件监听
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
				ExploreManager.Instance.EnterLevel(level, ExitType.ToNextLevel);
				//ExploreManager.Instance.expUICtr.transitionMask.gameObject.SetActive(false);
			},MapSetUpFrom.LastLevel);

		}


		/// <summary>
		/// 显示npc主界面
		/// </summary>
		private void ShowMainNpcView()
		{
			mainNPCView.gameObject.SetActive(true);
			int randomSeed = Random.Range(0, npc.regularGreetings.Count);

			HLHDialog greeting = npc.regularGreetings[randomSeed];

			dialogText.text = greeting.dialogContent;
		}
        
		/// <summary>
        /// 初始化技能学习界面
        /// </summary>
		public void SetupSkillLearningPlane(){

            // 查询本层是否已经教过技能了
			if(!npc.hasTeachedASkill){
				
				mainNPCView.gameObject.SetActive(false);

                skillsTradeView.SetUpSkillLearningView(npc, ShowMainNpcView);
			}else{// 如果本层已经教过技能了，则直接显示无法再教技能
				GameManager.Instance.soundManager.PlayAudioClip(CommonData.wrongTintAudioName);
				dialogText.text = "<color=white>我现在有点累了，暂时无法再教你任何技能，你可以先去其他地方探索一下。</color>";            
			}
         
		}
        

        /// <summary>
        /// 初始化交易界面
        /// </summary>
		public void SetUpTrade(){

			bool goodsSoldOut = npc.CheckGolldSoldOut();

			if(!goodsSoldOut){
				mainNPCView.gameObject.SetActive(false);

                goodsTradeView.SetUpNPCTradeView(npc, ShowMainNpcView);
			}else{
				GameManager.Instance.soundManager.PlayAudioClip(CommonData.wrongTintAudioName);
                dialogText.text = "<color=white>我已经没有东西可以卖给你了，你可以先去其他地方探索一下。</color>";   
			}

		}
        

        /// <summary>
        /// 初始化特殊操作界面【镶嵌宝石界面】
        /// </summary>
		public void SetUpSpecialOperation(){

			mainNPCView.gameObject.SetActive(false);

			addGemstoneView.SetUpAddGemstoneView(npc, ShowMainNpcView);
		}


        
              
        /// <summary>
        /// 退出npc交互界面
        /// </summary>
		public void QuitNPCPlane(){
                 
			ExploreManager.Instance.expUICtr.HideTopBarMask();

			ExploreManager.Instance.expUICtr.rejectNewUI = false;

			if (npc == null) {
				return;
			}

			for (int i = 0; i < GameManager.Instance.gameDataCenter.currentMapEventsRecord.npcArray.Length;i++){
				HLHNPC tempNpc = GameManager.Instance.gameDataCenter.currentMapEventsRecord.npcArray[i];
				if(tempNpc != null && tempNpc.npcId == npc.npcId){
					GameManager.Instance.gameDataCenter.currentMapEventsRecord.npcArray[i] = npc;
					break;
				}
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
