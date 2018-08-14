using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace WordJourney
{
	using UnityEngine.UI;
	using DG.Tweening;

	public class ExploreUICotroller : MonoBehaviour {

		public TintHUD hintHUD;
		public PauseHUD pauseHUD;
		public WordHUD wordHUD;
		public WordDetailHUD wordDetailHUD;
        public BillboardHUD billboardHUD;

        public Text wordRecordText;
		public Transform exploreMask;// 小遮罩，不遮盖底部消耗品栏和背包按钮
		public Transform topBarMask;// 探索顶部栏地遮罩，只有进入npc界面时开启

		/**********  battlePlane UI *************/
		public Transform battlePlane;

		/**********  battlePlane UI *************/

		//public NPCUIController npcUIController;

		public Text gameLevelLocationText;

		public Image transitionMask;      

		public BattlePlayerUIController bpUICtr;
		public BattleMonsterUIController bmUICtr;

		public RawImage miniMap;
        private int miniMapWidth;
        private int miniMapHeight;

		private float miniCameraSize;

		public Transform bigMapView;
		public RawImage bigMap;



		public TransitionView transitionView;

		public SimpleItemDetail simpleItemDetail;

		public BuyLifeQueryView buyLifeQueryHUD;
		public EnterExitHUD enterExitHUD;

		public CreationView creationView;

		public SpellItemView spellItemView;

		public Transform fullMask;// 覆盖整个屏幕的遮罩，禁止一切点击响应

		public ToolSelectView toolSelectView;// 工具选择界面

		public AchievementView achievementView;// 成就达成展示界面

		public DiaryView diaryView;// 日记显示界面

		public HelpViewController helpHUD;//帮助界面

		public PurchasePendingHUD purchaseHUD;

		public CommentRecommendHUD commentRecommendHUD;// 评价引导弹框

		// 记录本关所有背过的单词
        public List<HLHWord> wordRecords = new List<HLHWord>();

		public void SetUpExploreCanvas(bool hasResetGame=false){

			transitionMask.gameObject.SetActive (true);

			pauseHUD.InitPauseHUD (true, ConfirmQuitToHomeView, null, null);

            wordHUD.InitWordHUD (true, QuitWordHUDCallBack,ChooseAnswerInWordHUDCallBack,ConfirmCharacterFillInWordHUDCallBack);

			creationView.ClearCharacterFragments();

			if (!GameManager.Instance.UIManager.UIDic.ContainsKey ("BagCanvas")) {

				GameManager.Instance.UIManager.SetUpCanvasWith(CommonData.bagCanvasBundleName, "BagCanvas",null, false, true,false);
					
			}

			if (!GameManager.Instance.UIManager.UIDic.ContainsKey("NPCCanvas"))
            {            
				GameManager.Instance.UIManager.SetUpCanvasWith(CommonData.npcCanvasBundleName, "NPCCanvas", null, false, true,false);            
            }

			gameLevelLocationText.text = string.Format("第{0}层",Player.mainPlayer.currentLevelIndex + 1);
            wordRecordText.text = string.Empty;

			bpUICtr.InitExploreAgentView ();
			bpUICtr.SetUpExplorePlayerView (Player.mainPlayer);
			bmUICtr.InitExploreAgentView ();

			GetComponent<Canvas> ().enabled = true;
			transitionMask.color = Color.black;


			if (!Player.mainPlayer.isNewPlayer) {

				bool playerDataExist = DataHandler.FileExist(CommonData.persistDataPath + "PlayerData.json");
                if (!playerDataExist)
                {
                    GameManager.Instance.persistDataManager.SaveCompletePlayerData();
                }
                transitionMask.gameObject.SetActive(false);

				if (hasResetGame)
				{
					//GameManager.Instance.persistDataManager.ResetPlayerDataToOriginal();

					transitionView.PlayTransition(TransitionType.ResetGameHint, delegate
					{
						transitionMask.gameObject.SetActive(false);
					});               
				}          
			} else {
				
				ExploreManager.Instance.MapWalkableEventsStopAction();

				transitionMask.gameObject.SetActive(true);

				transitionView.PlayTransition(TransitionType.Introduce, delegate
                {
                    GameManager.Instance.UIManager.SetUpCanvasWith(CommonData.guideCanvasBundleName, "GuideCanvas", delegate
                    {
                        TransformManager.FindTransform("GuideCanvas").GetComponent<GuideViewController>().ShowNewPlayerGuide(null);                  
                    });
                    transitionMask.gameObject.SetActive(false);

                });  

            
			}

			creationView.InitCharacterFragmentsHUD(SetUpSpellItemView);

			wordRecords.Clear();

			miniCameraSize = TransformManager.FindTransform("MiniMapCamera").GetComponent<Camera>().orthographicSize;

			LoadMiniMapTexture();

			UpdateMiniMapDisplay(ExploreManager.Instance.battlePlayerCtr.transform.position);

		}


		public void EnterLevelMaskShowAndHide(CallBack callBack){
			StopCoroutine("MyEnterLevelMaskShowAndHide");
			StartCoroutine("MyEnterLevelMaskShowAndHide",callBack);
		}
        
        
		private IEnumerator MyEnterLevelMaskShowAndHide(CallBack callBack){
			
			float tempAlpha = 0;
            float fadeSpeed = 3f;


			transitionMask.color = new Color(0, 0, 0, tempAlpha);

			transitionMask.gameObject.SetActive(true);


			while (tempAlpha < 1)
            {
				tempAlpha += fadeSpeed * Time.deltaTime;
                transitionMask.color = new Color(0, 0, 0, tempAlpha);
                yield return null;
            }

			tempAlpha = 1;

			if (callBack != null)
            {
                callBack();
            }

            yield return new WaitUntil(() => ExploreManager.Instance.exploreSceneReady);

			//Debug.Break();

			while (tempAlpha > 0)
            {
				tempAlpha -= fadeSpeed * Time.deltaTime;
                transitionMask.color = new Color(0, 0, 0, tempAlpha);
                yield return null;
            }

			transitionMask.gameObject.SetActive(false);
		}


		public void DisplayTransitionMaskAnim(CallBack cb){
			ExploreManager.Instance.MapWalkableEventsStopAction ();
			transitionMask.gameObject.SetActive (true);
			StartCoroutine ("TransitionMaskShowAndHide", cb);
		}

		public void SetUpToolSelectView(List<SpecialItem> tools,CallBackWithItem toolSelectCallBack){
			toolSelectView.SetUpToolSelectView(tools, toolSelectCallBack);
		}

		public void UpdateMiniMapDisplay(Vector3 playerPos)
		{
			int offsetBaseX = Mathf.RoundToInt(playerPos.x);
			int offsetBaseY = Mathf.RoundToInt(playerPos.y);

			if(offsetBaseX < 5){
				offsetBaseX = 5;
			}

			if(offsetBaseX > ExploreManager.Instance.newMapGenerator.columns - 5){
				offsetBaseX = ExploreManager.Instance.newMapGenerator.columns - 5;
			}

			if (offsetBaseY < 5)
            {
                offsetBaseY = 5;
            }

			if (offsetBaseY > ExploreManager.Instance.newMapGenerator.rows - 5)
            {
				offsetBaseY = ExploreManager.Instance.newMapGenerator.rows - 5;
            }
            
			float offsetX = (miniCameraSize - offsetBaseX) / (miniCameraSize * 2) * miniMapWidth;
			float offsetY = (miniCameraSize - offsetBaseY) / (miniCameraSize * 2) * miniMapHeight;
            

            miniMap.transform.localPosition = new Vector3(offsetX, offsetY, 0);

		}


		private void LoadMiniMapTexture()
        {
            miniMap.texture = Resources.Load("MiniMapTexture") as Texture;
            miniMapWidth = Mathf.RoundToInt(miniMap.rectTransform.rect.width);
            miniMapHeight = Mathf.RoundToInt(miniMap.rectTransform.rect.height);
        }
        

		public void ShowBigMap(){
			if(ExploreManager.Instance.battlePlayerCtr.isInEvent){
				return;
			}
			ExploreManager.Instance.battlePlayerCtr.isInEvent = true;
			ExploreManager.Instance.battlePlayerCtr.StopMoveAtEndOfCurrentStep();
			ExploreManager.Instance.MapWalkableEventsStopAction();
			bigMapView.gameObject.SetActive(true);
			bigMap.texture = Resources.Load("MiniMapTexture") as Texture;
			float bigMapWidth = bigMap.rectTransform.rect.width;
			float bigMapHeight = bigMap.rectTransform.rect.height;
			float offsetX = bigMapWidth * (44 - ExploreManager.Instance.newMapGenerator.columns) / 88;
			float offsetY = bigMapHeight * (44 - ExploreManager.Instance.newMapGenerator.rows) / 88;
			bigMap.transform.localPosition = new Vector3(offsetX, offsetY, 0);
		}

		public void QuitBigMap(){
			bigMapView.gameObject.SetActive(false);
			ExploreManager.Instance.battlePlayerCtr.isInEvent = false;
			ExploreManager.Instance.MapWalkableEventsStartAction();
		}


		public void UpdatePlayerGold(){
			bpUICtr.RefreshGold();
		}

		public void ShowTopBarMask(){
			topBarMask.gameObject.SetActive(true);
		}

		public void HideTopBarMask(){
			topBarMask.gameObject.SetActive(false);
		}

		private IEnumerator TransitionMaskShowAndHide(CallBack cb){

			float tempAlpha = 0;
			float fadeSpeed = 3f;

			while(tempAlpha < 1){
				tempAlpha += fadeSpeed * Time.deltaTime;
				transitionMask.color = new Color (0, 0, 0, tempAlpha);
				yield return null;
			}
            
			tempAlpha = 1;

			if (cb != null)
            {
                cb();
            }

			transitionMask.gameObject.SetActive(false);

			while (tempAlpha > 0) {
				tempAlpha -= fadeSpeed * Time.deltaTime;
				transitionMask.color = new Color (0, 0, 0, tempAlpha);
				yield return null;
			}

			ExploreManager.Instance.MapWalkableEventsStartAction ();
         
		}

		public void ClearCharacterFragments(){
			Player.mainPlayer.allCollectedCharacters.Clear();
			creationView.ClearCharacterFragments();
		}

		public void SetUpSpellItemView(){         
			spellItemView.SetUpSpellView(ExploreManager.Instance.newMapGenerator.spellItemOfCurrentLevel,delegate{
				SpellItemModel spellItemModel = GameManager.Instance.gameDataCenter.allSpellItemModels.Find(delegate(SpellItemModel obj)
				{
					return obj.itemId == ExploreManager.Instance.newMapGenerator.spellItemOfCurrentLevel.itemId;               
				});
				spellItemModel.hasUsed = true;
				GameManager.Instance.persistDataManager.RefreshSpellItem();
				creationView.HideCreateButton();
			});
		}



		public void UpdateCharacterFragmentsHUD(){
			creationView.UpdateCharactersCollected();
		}

		public void ShowExploreMask(){
			exploreMask.gameObject.SetActive (true);
		}

		public void HideExploreMask(){
			exploreMask.gameObject.SetActive (false);
		}


		public void ShowFullMask(){
			fullMask.gameObject.SetActive (true);
		}

		public void HideFullMask(){
			fullMask.gameObject.SetActive (false);
		}

       

		public void ShowExploreSceneSlowly(){
			if (Player.mainPlayer.isNewPlayer)
			{
				return;
			}
			ShowExploreMask();
			if (!GameManager.Instance.gameDataCenter.gameSettings.newPlayerGuideFinished)
			{
				GameManager.Instance.UIManager.SetUpCanvasWith(CommonData.guideCanvasBundleName, "GuideCanvas", delegate
				{
					TransformManager.FindTransform("GuideCanvas").GetComponent<GuideViewController>().ShowNewPlayerGuide(ExploreManager.Instance.MapWalkableEventsStartAction);
					HideExploreMask();
				});
			}
         
			transitionMask.gameObject.SetActive(true);
            transitionMask.color = Color.black;
			
                     
			StopCoroutine("MyTransitionMaskSlowlyHide");
			StartCoroutine("MyTransitionMaskSlowlyHide");
		}

		private IEnumerator MyTransitionMaskSlowlyHide(){

            float tempAlpha = 1;

			float fadeSpeed = 3f;

			bool hasEnable = GameManager.Instance.gameDataCenter.gameSettings.newPlayerGuideFinished;

			if (hasEnable)
            {
                HideExploreMask();
				transitionMask.gameObject.SetActive(false);
            }

			while (tempAlpha > 0)
            {
                tempAlpha -= fadeSpeed * Time.deltaTime;
                transitionMask.color = new Color(0, 0, 0, tempAlpha);
                
				if(tempAlpha < 0.5f && !hasEnable){
					
                    transitionMask.gameObject.SetActive(false);
                    if (GameManager.Instance.gameDataCenter.gameSettings.newPlayerGuideFinished)
                    {
                        ExploreManager.Instance.MapWalkableEventsStartAction();
                        //tintHUD.SetUpSingleTextTintHUD("一种神秘力量将你传送到了入口位置");
						hasEnable = true;
                    }
               
				}

                yield return null;
            }

			HideExploreMask();
            transitionMask.gameObject.SetActive(false);
            
		}


		public void ShowFightPlane(){

			if (battlePlane.gameObject.activeInHierarchy) {
				return;
			}

			battlePlane.gameObject.SetActive (true);

			bpUICtr.SetUpFightPlane ();

		}

		public void UpdateActiveSkillButtons(){
			if(battlePlane.gameObject.activeInHierarchy){
				bpUICtr.SetUpActiveSkillButtons();
			}

		}

		public void RemoveActiveSkillButton(Skill skill){
			bpUICtr.RemoveActiveSkillButton(skill);
		}

		public void OnHelpButtonClick(){
			helpHUD.SetUpHelpView();
		}


		public void ShowLevelUpPlane(){
			bpUICtr.ShowLevelUpPlane ();
		}

		public void SetUpSingleTextTintHUD(string tint){
			hintHUD.SetUpSingleTextTintHUD (tint);
		}

		public void SetUpGoldGainTintHUD(int goldGain){
			hintHUD.SetUpGoldTintHUD (goldGain);
		}

		/// <summary>
		/// 更新底部消耗品栏
		/// </summary>
		public void UpdateBottomBar(){
			bpUICtr.SetUpConsumablesButtons ();
		}


		public void ShowEscapeBar(float escapeTime,CallBack escapeCallBack){
			bpUICtr.EscapeDisplay (escapeTime, escapeCallBack);
		}

		public void UpdatePlayerStatusBar(){
			bpUICtr.UpdateAgentStatusPlane ();
		}





		public void EnterNPC(HLHNPC npc){
			GameManager.Instance.UIManager.SetUpCanvasWith(CommonData.npcCanvasBundleName, "NPCCanvas", delegate
			{
				TransformManager.FindTransform("NPCCanvas").GetComponent<NPCViewController>().SetUpNPCView(npc);
			});

		}



		/// <summary>
		/// 初始化公告牌
		/// </summary>
		public void SetUpBillboard(Billboard bb){
			
			ExploreManager.Instance.MapWalkableEventsStopAction ();

            billboardHUD.SetUpBillboard(bb, QuitBillboard);
		}

		/// <summary>
		/// 退出公告牌
		/// </summary>
        private void QuitBillboard(){
			ExploreManager.Instance.battlePlayerCtr.isInEvent = false;
			ExploreManager.Instance.MapWalkableEventsStartAction ();
		}


        /// <summary>
        /// 显示日记显示界面
        /// </summary>
		public void SetUpDiaryView(DiaryModel diaryModel){

			if(ExploreManager.Instance.battlePlayerCtr.isInEvent){
				return;
			}

			ExploreManager.Instance.battlePlayerCtr.isInEvent = true;

			ExploreManager.Instance.battlePlayerCtr.StopMoveAtEndOfCurrentStep();
            
			ExploreManager.Instance.MapWalkableEventsStopAction();
         
			if(diaryModel != null){
				diaryView.SetUpDiaryView(diaryModel,QuitDiaryViewCallBack);          
			}
			  
		}

		private void QuitDiaryViewCallBack(){
			ExploreManager.Instance.battlePlayerCtr.isInEvent = false;
            ExploreManager.Instance.MapWalkableEventsStartAction();
		}



		public void OnPauseButtonClick(){
			ShowPauseHUD ();
		}

		public void ShowPauseHUD(){
			pauseHUD.SetUpPauseHUD ();
		}

		public void QuitPauseHUD(){
			pauseHUD.QuitPauseHUD ();
		}
      

		public void SetUpWordHUD(HLHWord[] words,string extraInfo = null){
			wordHUD.SetUpWordHUDAndShow (words,extraInfo);
		}

		public void SetUpWordHUD(HLHWord word){
			wordHUD.SetUpWordHUDAndShow (word);
		}

        public void SetUpWordDetailHUD(){

			if (wordRecords.Count > 0)
            {
                ExploreManager.Instance.MapWalkableEventsStopAction();
				HLHWord word = wordRecords[wordRecords.Count - 1];
				wordDetailHUD.SetUpWordDetailHUD(wordRecords,ExploreManager.Instance.MapWalkableEventsStartAction);
            }
        }

        private void ConfirmCharacterFillInWordHUDCallBack(bool isFillCorrect){

			ExploreManager.Instance.ConfirmFillCharactersInWordHUD (isFillCorrect);

		}

        private void QuitWordHUDCallBack(HLHWord word)
		{
            ExploreManager.Instance.MapWalkableEventsStartAction();

            if (word == null)
            {
                return;
            }

            wordRecordText.text = word.spell;

			wordRecords.Add(word);
		}

		private void ChooseAnswerInWordHUDCallBack(bool isChooseCorrect){

			ExploreManager.Instance.ChooseAnswerInWordHUD (isChooseCorrect);

#if UNITY_IOS || UNITY_EDITOR
			bool pushRecommend = CheckPushCommentRecommend();

			if(pushRecommend){
				commentRecommendHUD.SetUpCommentRecommendHUD();
			}
#endif

			int qualificationIndex = CheckLearnTitleQualification();

			if(qualificationIndex == -1){
				return;
			}

			LearnTitleQualification qualification = CommonData.learnTitleQualifications[qualificationIndex];

			achievementView.SetUpAchievementView(qualification);

		}

        /// <summary>
        /// 检测是否到了推送
        /// </summary>
        /// <returns><c>true</c>, if push comment recommend was checked, <c>false</c> otherwise.</returns>
		private bool CheckPushCommentRecommend(){
			bool push = false;
			int totalLearnedWordCount = Player.mainPlayer.totalLearnedWordCount;
			if(totalLearnedWordCount == 300){
				push = true;
			}         
			return push;
				
		}

        /// <summary>
        /// 检查是否达成称号
		/// 如果没有新的称号达成，返回-1
		/// 如果有新的称号达成，返回称号序号
        /// </summary>
		private int CheckLearnTitleQualification(){
			
			int qualificationIndex = -1;

			int totalLearnedWordCount = Player.mainPlayer.totalLearnedWordCount;

			int totalUngraspWordCount = Player.mainPlayer.totalUngraspWordCount;

			int continuousCorrectWordCount = Player.mainPlayer.maxWordContinuousRightRecord;

			float correctPercentage = totalLearnedWordCount == 0 ? 0 : (float)(totalLearnedWordCount - totalUngraspWordCount) / totalLearnedWordCount;
         
			for (int i = 0; i < CommonData.learnTitleQualifications.Length;i++){

				bool titleQualified = Player.mainPlayer.titleQualifications[i];

				if(titleQualified){
					continue;
				}

				LearnTitleQualification qualification = CommonData.learnTitleQualifications[i];

				titleQualified = totalLearnedWordCount >= qualification.totalWordsCount 
				                 && correctPercentage >= qualification.totalCorrectPercentage - float.Epsilon 
				                 && continuousCorrectWordCount >= qualification.continuousCorrectWordCount;

				bool dataCorrect = false;

				if(titleQualified){

					ExploreManager.Instance.UpdateWordDataBase();

					int learnWordCountFromDB = LearningInfo.Instance.learnedWordCount;
					int ungraspWordCountFromDB = LearningInfo.Instance.ungraspedWordCount;
					dataCorrect = learnWordCountFromDB == totalLearnedWordCount && ungraspWordCountFromDB == totalUngraspWordCount;

					if (dataCorrect)
                    {
                        Player.mainPlayer.titleQualifications[i] = true;
                        qualificationIndex = i;
                        break;
                    }
                    else
                    {
						Player.mainPlayer.totalLearnedWordCount = learnWordCountFromDB;
						Player.mainPlayer.totalUngraspWordCount = ungraspWordCountFromDB;
						GameManager.Instance.persistDataManager.SaveCompletePlayerData();
						return CheckLearnTitleQualification();
   
                     }
				}          
			}         
			return qualificationIndex;
		}



		public void SetUpSimpleItemDetail(Item item){
			simpleItemDetail.SetupSimpleItemDetail (item);
		}

		public void ShowEnterExitQueryHUD(ExitType exitType){
			
			enterExitHUD.SetUpEnterExitHUD(exitType);
		}
      

		public void ShowBuyLifeQueryHUD(){
			buyLifeQueryHUD.SetUpBuyLifeQueryView (ConfirmBuyLife, CancelBuyLife);
		}
			

		private void ConfirmBuyLife(){

			switch (Application.internetReachability)
            {
                case NetworkReachability.NotReachable:
                    hintHUD.SetUpSingleTextTintHUD("无网络连接");
                    break;
                case NetworkReachability.ReachableViaCarrierDataNetwork:
                case NetworkReachability.ReachableViaLocalAreaNetwork:
					QuitFight();
                    purchaseHUD.SetUpPurchasePendingHUD(PurchaseManager.new_life_id, PlayerRecomeToLifeCallBack);
                    HideFullMask();
                    break;
            }


		}

		private void PlayerRecomeToLifeCallBack(){
			buyLifeQueryHUD.QuitBuyLifeView();
			ExploreManager.Instance.battlePlayerCtr.RecomeToLife();
			bpUICtr.UpdateAgentStatusPlane();
            ExploreManager.Instance.MapWalkableEventsStartAction();
		}

		private void CancelBuyLife(){
			transitionView.PlayTransition (TransitionType.Death, delegate {
				transitionMask.gameObject.SetActive(true);
				transitionMask.color = Color.black;
				GameManager.Instance.persistDataManager.ResetPlayerDataToOriginal ();
				GameManager.Instance.UIManager.SetUpCanvasWith(CommonData.loadingCanvasBundleName, "LoadingCanvas", delegate
				{
					TransformManager.FindTransform("LoadingCanvas").GetComponent<LoadingViewController>().SetUpLoadingView(LoadingType.QuitExplore, ExploreManager.Instance.QuitExploreScene, null);
				});

			});
		}

		private void ConfirmQuitToHomeView(){
			
			transitionView.PlayTransition(TransitionType.None, delegate
			{
				transitionMask.gameObject.SetActive(true);
				transitionMask.color = Color.black;
				GameManager.Instance.UIManager.SetUpCanvasWith(CommonData.loadingCanvasBundleName, "LoadingCanvas", delegate
                {
                    TransformManager.FindTransform("LoadingCanvas").GetComponent<LoadingViewController>().SetUpLoadingView(LoadingType.QuitExplore, ExploreManager.Instance.QuitExploreScene, null);
                });

			});
		}





		public void QuitFight(){
			bpUICtr.QuitFightPlane ();
			bmUICtr.QuitFightPlane ();
			battlePlane.gameObject.SetActive(false);
			//HideFightPlane ();
		}


		public void QuitExplore(){
			//GameManager.Instance.UIManager.RemoveCanvasCache ("ExploreCanvas");
			GameManager.Instance.UIManager.RemoveMultiCanvasCache(new string[]{"ExploreCanvas","BagCanvas","NPCCanvas"});

		}



		public void DestroyInstances(){
			transitionMask.gameObject.SetActive(true);
			Destroy (this.gameObject,0.3f);
		}

	}
}
