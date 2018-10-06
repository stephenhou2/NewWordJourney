using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace WordJourney
{
	using UnityEngine.UI;

	public class MonsterDataWithUIDipslay
	{
		public MonsterData monsterData;
		public Transform monsterUI;
		public MonsterUIInfo monsterUIInfo;
		public MonsterDataWithUIDipslay(MonsterData monsterData, Transform monsterUI, MonsterUIInfo monsterUIInfo)
		{
			this.monsterData = monsterData;
			this.monsterUI = monsterUI;
			this.monsterUIInfo = monsterUIInfo;
		}
	}

	public class ExploreUICotroller : MonoBehaviour
	{

		public TintHUD hintHUD;
		public PauseHUD pauseHUD;
		public WordHUD wordHUD;
		public WordDetailHUD wordDetailHUD;
		public BillboardHUD billboardHUD;

		public Transform wordRecordContainer;
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

		public Transform topBarContainer;
		public Transform bottomBarContainer;

		public Transform miniMapContainer;
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

		public KeyDoorOperatorView keyDoorOperatorView;// 开锁界面

		public AchievementView achievementView;// 成就达成展示界面

		public DiaryView diaryView;// 日记显示界面

		//public HelpViewController helpHUD;//帮助界面

		public PurchasePendingHUD purchaseHUD;

		public CommentRecommendHUD commentRecommendHUD;// 评价引导弹框

		public SaveDataHintView saveDataHintView;


		// 记录本关所有背过的单词
		public List<HLHWord> wordRecords;

		private IEnumerator enterLevelMaskShowAndHideCoroutine;
		private IEnumerator myTransitionMaskHideCoroutine;

		public MonstersDisplayView monstersDisplayView;

		public InstancePool monsterUIPool;

		private List<MonsterDataWithUIDipslay> monstersInfoWithDisplayUI = new List<MonsterDataWithUIDipslay>();

		public WordRecordQuizView wordRecordQuizView;

		public PuzzleView puzzleView;

		public Transform finishGameQueryHUD;


		public void SetUpExploreCanvas()
		{

			wordRecords = GameManager.Instance.gameDataCenter.currentMapWordRecords;

			transitionMask.gameObject.SetActive(true);

			pauseHUD.InitPauseHUD(true, ConfirmQuitToHomeView, null, null);

			wordHUD.InitWordHUD(true, QuitWordHUDCallBack, ChooseAnswerInWordHUDCallBack, ConfirmCharacterFillInWordHUDCallBack);

			creationView.ClearCharacterFragments();

			if (!GameManager.Instance.UIManager.UIDic.ContainsKey("BagCanvas"))
			{

				GameManager.Instance.UIManager.SetUpCanvasWith(CommonData.bagCanvasBundleName, "BagCanvas", null, false, true, false);

			}

			if (!GameManager.Instance.UIManager.UIDic.ContainsKey("NPCCanvas"))
			{
				GameManager.Instance.UIManager.SetUpCanvasWith(CommonData.npcCanvasBundleName, "NPCCanvas", null, false, true, false);
			}

			gameLevelLocationText.text = string.Format("第{0}层", Player.mainPlayer.currentLevelIndex + 1);
			wordRecordText.text = string.Empty;

			if (wordRecords.Count > 0)
			{
				wordRecordText.text = wordRecords[wordRecords.Count - 1].spell;
			}


			bpUICtr.InitExploreAgentView();
			bpUICtr.SetUpExplorePlayerView(Player.mainPlayer);
			bmUICtr.InitExploreAgentView();

			GetComponent<Canvas>().enabled = true;
			transitionMask.color = Color.black;


			if (!Player.mainPlayer.isNewPlayer)
			{

				bool playerDataExist = DataHandler.FileExist(CommonData.playerDataFilePath);
				if (!playerDataExist)
				{
					GameManager.Instance.persistDataManager.SaveCompletePlayerData();
				}
				transitionMask.gameObject.SetActive(false);

				//if (hasResetGame)
				//{
				//	//GameManager.Instance.persistDataManager.ResetPlayerDataToOriginal();

				//	transitionView.PlayTransition(TransitionType.ResetGameHint, delegate
				//	{
				//		transitionMask.gameObject.SetActive(false);
				//	});
				//}
			}
			else
			{

				ExploreManager.Instance.MapWalkableEventsStopAction();

				transitionMask.gameObject.SetActive(true);

				TransitionType transitionType = GameManager.Instance.persistDataManager.versionUpdateWhenLoad ? TransitionType.VersionUpdate : TransitionType.Introduce;

				transitionView.PlayTransition(transitionType, delegate
				{
					GameManager.Instance.persistDataManager.versionUpdateWhenLoad = false;

					GameManager.Instance.UIManager.SetUpCanvasWith(CommonData.guideCanvasBundleName, "GuideCanvas", delegate
					{
						TransformManager.FindTransform("GuideCanvas").GetComponent<GuideViewController>().ShowNewPlayerGuide(null);
						ExploreManager.Instance.MapWalkableEventsStartAction();
					});
					transitionMask.gameObject.SetActive(false);

				});


			}

			creationView.InitCharacterFragmentsHUD(SetUpSpellItemView);

			//wordRecords.Clear();

			miniCameraSize = TransformManager.FindTransform("MiniMapCamera").GetComponent<Camera>().orthographicSize;

			LoadMiniMapTexture();

			UpdateMiniMapDisplay(ExploreManager.Instance.battlePlayerCtr.transform.position);

			if (Player.mainPlayer.currentLevelIndex < CommonData.maxLevelIndex)
			{

				for (int i = 0; i < monstersInfoWithDisplayUI.Count; i++)
				{

					Transform monsterUI = monstersInfoWithDisplayUI[i].monsterUI;

					monsterUIPool.AddInstanceToPool(monsterUI.gameObject);
				}

				monstersInfoWithDisplayUI.Clear();

				GetMonstersDataAndUI();

				monstersDisplayView.InitMonsterDisplayView(monstersInfoWithDisplayUI);

				monsterUIPool.ClearInstancePool();

			}
		}

		public void HideUpAndBottomUIs()
		{
			topBarContainer.gameObject.SetActive(false);
			miniMapContainer.gameObject.SetActive(false);
			wordRecordContainer.gameObject.SetActive(false);
			bottomBarContainer.gameObject.SetActive(false);
		}

		public void EnterLevelMaskShowAndHide(CallBack callBack, MapSetUpFrom from,float delay = 0)
		{
			if (enterLevelMaskShowAndHideCoroutine != null)
			{
				StopCoroutine(enterLevelMaskShowAndHideCoroutine);
			}
			enterLevelMaskShowAndHideCoroutine = MyEnterLevelMaskShowAndHide(callBack, from, delay);
			StartCoroutine(enterLevelMaskShowAndHideCoroutine);
		}


		public void SetUpSaveDataHintViewAndSave(CallBack saveCallBack, CallBack saveFinishCallBack)
		{

			saveDataHintView.SetUpSaveDataHintView(saveCallBack, saveFinishCallBack);

		}

		/// <summary>
		/// 显示最终的通关询问界面
		/// </summary>
		public void ShowFinalQuitQueryHUD()
		{
			finishGameQueryHUD.gameObject.SetActive(true);
		}

		public void OnConfirmFinishGame()
		{
			finishGameQueryHUD.gameObject.SetActive(false);
			transitionMask.gameObject.SetActive(true);
			transitionMask.color = new Color(0, 0, 0, 1);

			GameManager.Instance.soundManager.StopBgm();

			transitionView.PlayTransition(TransitionType.End, delegate
			{
				Player.mainPlayer.needChooseDifficulty = true;

				GameManager.Instance.persistDataManager.ResetPlayerDataToOriginal();

				GameManager.Instance.UIManager.SetUpCanvasWith(CommonData.loadingCanvasBundleName, "LoadingCanvas", delegate
				{
					ExploreManager.Instance.QuitExploreScene();

					TransformManager.FindTransform("LoadingCanvas").GetComponent<LoadingViewController>().SetUpLoadingView(LoadingType.QuitExplore, null, delegate
					{
						GameManager.Instance.UIManager.SetUpCanvasWith(CommonData.homeCanvasBundleName, "HomeCanvas", null);
					});

				});

			});
		}

		public void OnCancelFinishGame()
		{
			finishGameQueryHUD.gameObject.SetActive(false);
			ExploreManager.Instance.battlePlayerCtr.isInEvent = false;
		}

		/// <summary>
		/// 获取本层怪物UI龙骨
		/// </summary>
		private void GetMonstersDataAndUI()
		{

			HLHGameLevelData levelData = GameManager.Instance.gameDataCenter.gameLevelDatas[Player.mainPlayer.currentLevelIndex];
			List<MonsterData> monsterDatas = GameManager.Instance.gameDataCenter.monsterDatas;

			for (int i = 0; i < levelData.monsterIdsOfCurrentLevel.Count; i++)
			{
				int monsterId = levelData.monsterIdsOfCurrentLevel[i];
				string monsterUIName = MyTool.GetMonsterUIName(monsterId);
				Transform monsterUI = monsterUIPool.GetInstanceWithName<Transform>(monsterUIName);
				if (monsterUI == null)
				{
					monsterUI = GameManager.Instance.gameDataCenter.LoadMonsterUI(monsterUIName).transform;
				}

				monsterUI.gameObject.SetActive(false);

				MonsterData monsterData = monsterDatas.Find(delegate (MonsterData obj)
				{
					return obj.monsterId == monsterId;

				});

				MonsterUIInfo monsterUIInfo = monsterUI.GetComponent<MonsterUIInfo>();

				MonsterDataWithUIDipslay monsterDataWithUIDipslay = new MonsterDataWithUIDipslay(monsterData, monsterUI, monsterUIInfo);

				monstersInfoWithDisplayUI.Add(monsterDataWithUIDipslay);
			}

			//if(HLHGameLevelData.IsBossLevel()){

			//	int monsterId = levelData.bossId;
			//	string monsterUIName = MyTool.GetMonsterUIName(monsterId);
			//	Transform monsterUI = monsterUIPool.GetInstanceWithName<Transform>(monsterUIName);
			//	if(monsterUI == null){
			//		monsterUI = GameManager.Instance.gameDataCenter.LoadMonsterUI(monsterUIName).transform;
			//	}

			//	MonsterData monsterData = monsterDatas.Find(delegate (MonsterData obj)
			//             {
			//                 return obj.monsterId == monsterId;

			//             });

			//	MonsterUIInfo monsterUIInfo = monsterUI.GetComponent<MonsterUIInfo>();
			//	MonsterDataWithUIDipslay monsterDataWithUIDipslay = new MonsterDataWithUIDipslay(monsterData, monsterUI,monsterUIInfo);

			//	monstersInfoWithDisplayUI.Add(monsterDataWithUIDipslay);
			//}

		}

		private IEnumerator MyEnterLevelMaskShowAndHide(CallBack callBack, MapSetUpFrom from,float delay)
		{
			yield return new WaitForSeconds(delay);


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

			switch (from)
			{
				case MapSetUpFrom.LastLevel:
				case MapSetUpFrom.Home:
					hintHUD.SetUpSingleTextTintHUD("数据已存档");
					GameManager.Instance.soundManager.PlayAudioClip(CommonData.skillUpgradeAudioName);
					ExploreManager.Instance.newMapGenerator.ManuallyPlaySavePointAnim();
					break;
				case MapSetUpFrom.NextLevel:
					break;

			}
		}


		public void DisplayTransitionMaskAnim(CallBack cb)
		{
			ExploreManager.Instance.MapWalkableEventsStopAction();
			transitionMask.gameObject.SetActive(true);
			IEnumerator transitionMaskDisplayCoroutine = TransitionMaskShowAndHide(cb);
			StartCoroutine(transitionMaskDisplayCoroutine);
		}

		public void SetUpUnlockDoorView(List<SpecialItem> keys, int doorDifficulty, CallBack unlockSuccessCallBack, CallBack unlockFailCallBack)
		{
			ExploreManager.Instance.MapWalkableEventsStopAction();
			keyDoorOperatorView.SetUpKeyDoorOperatorView(keys, doorDifficulty, unlockSuccessCallBack, unlockFailCallBack);
		}

		public void UpdateMiniMapDisplay(Vector3 playerPos)
		{
			int offsetBaseX = Mathf.RoundToInt(playerPos.x);
			int offsetBaseY = Mathf.RoundToInt(playerPos.y);

			if (offsetBaseX < 5)
			{
				offsetBaseX = 5;
			}

			if (offsetBaseX > ExploreManager.Instance.newMapGenerator.columns - 5)
			{
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


		public void ShowBigMap()
		{
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

		public void QuitBigMap()
		{
			bigMapView.gameObject.SetActive(false);
			ExploreManager.Instance.battlePlayerCtr.isInEvent = false;
			ExploreManager.Instance.MapWalkableEventsStartAction();
		}


		public void UpdatePlayerGold()
		{
			bpUICtr.RefreshGold();
		}

		public void ShowTopBarMask()
		{
			topBarMask.gameObject.SetActive(true);
		}

		public void HideTopBarMask()
		{
			topBarMask.gameObject.SetActive(false);
		}

		private IEnumerator TransitionMaskShowAndHide(CallBack cb)
		{

			float tempAlpha = 0;
			float fadeSpeed = 3f;

			while (tempAlpha < 1)
			{
				tempAlpha += fadeSpeed * Time.deltaTime;
				transitionMask.color = new Color(0, 0, 0, tempAlpha);
				yield return null;
			}

			tempAlpha = 1;

			if (cb != null)
			{
				cb();
			}

			transitionMask.gameObject.SetActive(false);

			while (tempAlpha > 0)
			{
				tempAlpha -= fadeSpeed * Time.deltaTime;
				transitionMask.color = new Color(0, 0, 0, tempAlpha);
				yield return null;
			}

			ExploreManager.Instance.MapWalkableEventsStartAction();

		}

		public void ClearCharacterFragments()
		{
			Player.mainPlayer.allCollectedCharacters.Clear();
			creationView.ClearCharacterFragments();
		}

		public void SetUpSpellItemView()
		{
			//Debug.Break();
			ExploreManager.Instance.MapWalkableEventsStopAction();
			spellItemView.SetUpSpellView(ExploreManager.Instance.newMapGenerator.spellItemOfCurrentLevel, delegate
			{
				Player.mainPlayer.spellRecord.Add(ExploreManager.Instance.newMapGenerator.spellItemOfCurrentLevel.spell);
				creationView.HideCreateButton();
			});
		}



		public void UpdateCharacterFragmentsHUD()
		{
			creationView.UpdateCharactersCollected();
		}

		public void ShowExploreMask()
		{
			exploreMask.gameObject.SetActive(true);
		}

		public void HideExploreMask()
		{
			exploreMask.gameObject.SetActive(false);
		}


		public void ShowFullMask()
		{
			fullMask.gameObject.SetActive(true);
		}

		public void HideFullMask()
		{
			fullMask.gameObject.SetActive(false);
		}



		public void ShowExploreSceneSlowly()
		{
			if (Player.mainPlayer.isNewPlayer)
			{
				return;
			}
			ShowExploreMask();

			bool newPlayerGuideFinished = GameManager.Instance.gameDataCenter.gameSettings.newPlayerGuideFinished;

			if (!newPlayerGuideFinished)
			{
				GameManager.Instance.UIManager.SetUpCanvasWith(CommonData.guideCanvasBundleName, "GuideCanvas", delegate
				{
					TransformManager.FindTransform("GuideCanvas").GetComponent<GuideViewController>().ShowNewPlayerGuide(ExploreManager.Instance.MapWalkableEventsStartAction);
					HideExploreMask();
					GameManager.Instance.soundManager.PlayAudioClip(CommonData.skillUpgradeAudioName);
					hintHUD.SetUpSingleTextTintHUD("数据已存档");
					ExploreManager.Instance.newMapGenerator.ManuallyPlaySavePointAnim();
				});
			}

			transitionMask.gameObject.SetActive(true);
			transitionMask.color = Color.black;


			if (myTransitionMaskHideCoroutine != null)
			{
				StopCoroutine(myTransitionMaskHideCoroutine);
			}
			myTransitionMaskHideCoroutine = MyTransitionMaskSlowlyHide(newPlayerGuideFinished);
			StartCoroutine(myTransitionMaskHideCoroutine);
		}

		private IEnumerator MyTransitionMaskSlowlyHide(bool showSaveHint)
		{

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

				if (tempAlpha < 0.5f && !hasEnable)
				{

					transitionMask.gameObject.SetActive(false);
					if (GameManager.Instance.gameDataCenter.gameSettings.newPlayerGuideFinished)
					{
						ExploreManager.Instance.MapWalkableEventsStartAction();

						hasEnable = true;
					}

				}

				yield return null;
			}

			HideExploreMask();
			ExploreManager.Instance.battlePlayerCtr.isInEvent = false;
			transitionMask.gameObject.SetActive(false);
			if (showSaveHint)
			{
				GameManager.Instance.soundManager.PlayAudioClip(CommonData.skillUpgradeAudioName);
				hintHUD.SetUpSingleTextTintHUD("数据已存档");
				ExploreManager.Instance.newMapGenerator.ManuallyPlaySavePointAnim();
			}
		}


		public void ShowFightPlane()
		{

			if (battlePlane.gameObject.activeInHierarchy)
			{
				return;
			}

			battlePlane.gameObject.SetActive(true);

			bpUICtr.SetUpFightPlane();

			ExploreManager.Instance.EnableExploreInteractivity();

		}

		public void UpdateActiveSkillButtons()
		{
			if (battlePlane.gameObject.activeInHierarchy)
			{
				bpUICtr.SetUpActiveSkillButtons();
			}

		}

		public void RemoveActiveSkillButton(Skill skill)
		{
			bpUICtr.RemoveActiveSkillButton(skill);
		}

		public void OnMonsterDisplayButtonClick()
		{
			ExploreManager.Instance.battlePlayerCtr.isInEvent = true;
			ExploreManager.Instance.MapWalkableEventsStopAction();
			monstersDisplayView.SetUpmonstersDisplayView(CallBackOnQuitMonsterDisplay);
		}

		private void CallBackOnQuitMonsterDisplay(Transform trans)
		{
			ExploreManager.Instance.battlePlayerCtr.isInEvent = false;
			ExploreManager.Instance.MapWalkableEventsStartAction();
			monsterUIPool.AddChildInstancesToPool(trans);
		}


		public void ShowLevelUpPlane()
		{
			bpUICtr.ShowLevelUpPlane();
		}

		public void SetUpSingleTextTintHUD(string tint)
		{
			hintHUD.SetUpSingleTextTintHUD(tint);
		}

		public void SetUpGoldGainTintHUD(int goldGain)
		{
			hintHUD.SetUpGoldTintHUD(goldGain);
		}

		/// <summary>
		/// 更新底部消耗品栏
		/// </summary>
		public void UpdateBottomBar()
		{
			bpUICtr.SetUpConsumablesButtons();
		}


		public void ShowEscapeBar(float escapeTime, CallBack escapeCallBack)
		{
			bpUICtr.EscapeDisplay(escapeTime, escapeCallBack);
		}

		public void UpdatePlayerStatusBar()
		{
			bpUICtr.UpdateAgentStatusPlane();
		}


		public void UpdateMonsterStatusPlane(Monster monster)
		{
			// 初始化怪物状态栏
			bmUICtr.SetUpMonsterStatusPlane(monster);
		}

		public void SetUpPuzzleView(CallBack answerRightCallBack, CallBack answerWrongCallBack)
		{

			ExploreManager.Instance.MapWalkableEventsStopAction();

			puzzleView.SetUpMonsterPuzzleView(answerRightCallBack, answerWrongCallBack);

		}


		public void EnterNPC(HLHNPC npc)
		{

			ExploreManager.Instance.MapWalkableEventsStopAction();

			GameManager.Instance.UIManager.SetUpCanvasWith(CommonData.npcCanvasBundleName, "NPCCanvas", delegate
			{
				TransformManager.FindTransform("NPCCanvas").GetComponent<NPCViewController>().SetUpNPCView(npc);
			});

		}



		public void QueryEnterWordRecordQuizView(CallBack enterQuizCallBack)
		{
			ExploreManager.Instance.MapWalkableEventsStopAction();
			wordRecordQuizView.SetUpwordRecordQuizView(wordRecords, enterQuizCallBack);
		}

		/// <summary>
		/// 初始化公告牌
		/// </summary>
		public void SetUpBillboard(Billboard bb)
		{

			ExploreManager.Instance.MapWalkableEventsStopAction();

			billboardHUD.SetUpBillboard(bb, QuitBillboard);
		}

		/// <summary>
		/// 退出公告牌
		/// </summary>
		private void QuitBillboard()
		{
			ExploreManager.Instance.battlePlayerCtr.isInEvent = false;
			ExploreManager.Instance.MapWalkableEventsStartAction();
		}


		/// <summary>
		/// 显示日记显示界面
		/// </summary>
		public void SetUpDiaryView(DiaryModel diaryModel)
		{

			ExploreManager.Instance.battlePlayerCtr.isInEvent = true;

			ExploreManager.Instance.battlePlayerCtr.StopMoveAtEndOfCurrentStep();

			ExploreManager.Instance.MapWalkableEventsStopAction();

			if (diaryModel != null)
			{
				diaryView.SetUpDiaryView(diaryModel, QuitDiaryViewCallBack);
			}

		}

		private void QuitDiaryViewCallBack()
		{
			ExploreManager.Instance.battlePlayerCtr.isInEvent = false;
			ExploreManager.Instance.MapWalkableEventsStartAction();
		}



		public void OnPauseButtonClick()
		{
			ShowPauseHUD();
		}

		public void ShowPauseHUD()
		{
			pauseHUD.SetUpPauseHUD();
		}

		public void QuitPauseHUD()
		{
			pauseHUD.QuitPauseHUD();
		}


		public void SetUpWordHUD(HLHWord[] words, string extraInfo = null)
		{
			ExploreManager.Instance.MapWalkableEventsStopAction();
			wordHUD.SetUpWordHUDAndShow(words, extraInfo);
		}

		public void SetUpWordHUD(HLHWord word)
		{
			ExploreManager.Instance.MapWalkableEventsStopAction();
			wordHUD.SetUpWordHUDAndShow(word);
		}

		public void OnWordRecordDetailButtonClick()
		{
			ExploreManager.Instance.MapWalkableEventsStopAction();
			SetUpWordDetailHUD(true, null);
		}

		public void SetUpWordDetailHUD(bool controlMoveWhenQuit, CallBack quitCallBack)
		{

			if (wordRecords.Count > 0)
			{
				ExploreManager.Instance.MapWalkableEventsStopAction();
				HLHWord word = wordRecords[wordRecords.Count - 1];
				wordDetailHUD.SetUpWordDetailHUD(wordRecords, delegate
				{
					if (quitCallBack != null)
					{
						quitCallBack();
					}
					if (!controlMoveWhenQuit)
					{
						ExploreManager.Instance.MapWalkableEventsStartAction();
					}
				});
			}
		}

		private void ConfirmCharacterFillInWordHUDCallBack(bool isFillCorrect)
		{

			ExploreManager.Instance.ConfirmFillCharactersInWordHUD(isFillCorrect);

			ExploreManager.Instance.MapWalkableEventsStartAction();

		}



		private void QuitWordHUDCallBack(HLHWord word)
		{

			if (word == null)
			{
				return;
			}

			wordRecordText.text = word.spell;

			//bool update = true;
			for (int j = 0; j < wordRecords.Count; j++)
			{
				if (wordRecords[j].wordId == word.wordId)
				{
					//update = false;
					wordRecords.RemoveAt(j);
					break;
				}
			}
			//if(update){
			wordRecords.Add(word);
			//}            

		}

		public void UpdateWordRecords(List<HLHWord> words)
		{

			for (int i = 0; i < words.Count; i++)
			{
				HLHWord word = words[i];

				if (word == null)
				{
					continue;
				}

				bool update = true;
				for (int j = 0; j < wordRecords.Count; j++)
				{
					if (wordRecords[j].wordId == word.wordId)
					{
						update = false;
						break;
					}
				}
				if (update)
				{
					wordRecords.Add(words[i]);
				}
			}
			if (words.Count > 0 && words[words.Count - 1] != null)
			{
				wordRecordText.text = words[words.Count - 1].spell;
			}
		}


		private void ChooseAnswerInWordHUDCallBack(bool isChooseCorrect)
		{

			if (!isChooseCorrect)
			{

				bool showWordDetail = ExploreManager.Instance.NeedShowFullWordDetailWhenChooseWrong();

				if (showWordDetail)
				{

					bool controlMoveWhenQuit = true;

					bool pushRecommend = false;

#if UNITY_IOS || UNITY_EDITOR
					pushRecommend = CheckPushCommentRecommend();

					if (pushRecommend)
					{
						controlMoveWhenQuit = false;

					}
#endif

					int qualificationIndex = LearnTitleQualification.CheckLearnTitleQualification();

					if (qualificationIndex != -1)
					{
						controlMoveWhenQuit = false;
					}

					SetUpWordDetailHUD(controlMoveWhenQuit, delegate
					{
						if (pushRecommend)
						{
							commentRecommendHUD.SetUpCommentRecommendHUD();
						}
						if (qualificationIndex != -1)
						{
							LearnTitleQualification qualification = CommonData.learnTitleQualifications[qualificationIndex];

							achievementView.SetUpAchievementView(qualification);
						}
					});
				}

			}
			else
			{            
				bool pushRecommend = CheckPushCommentRecommend();

				if (pushRecommend)
				{
					commentRecommendHUD.SetUpCommentRecommendHUD();
				}


				int qualificationIndex = LearnTitleQualification.CheckLearnTitleQualification();

				if (qualificationIndex != -1)
				{
					LearnTitleQualification qualification = CommonData.learnTitleQualifications[qualificationIndex];

					achievementView.SetUpAchievementView(qualification);
				}

			}

			ExploreManager.Instance.ChooseAnswerInWordHUD(isChooseCorrect);

			ExploreManager.Instance.MapWalkableEventsStartAction();

		}

		/// <summary>
		/// 检测是否到了推送
		/// </summary>
		/// <returns><c>true</c>, if push comment recommend was checked, <c>false</c> otherwise.</returns>
		private bool CheckPushCommentRecommend()
		{
			bool push = false;
			int totalLearnedWordCount = Player.mainPlayer.totalLearnedWordCount;
			if (totalLearnedWordCount == 300)
			{
				push = true;
			}
			return push;

		}




		public void SetUpSimpleItemDetail(Item item)
		{
			simpleItemDetail.SetupSimpleItemDetail(item);
		}

		public void ShowEnterExitQueryHUD(ExitType exitType)
		{
			ExploreManager.Instance.MapWalkableEventsStopAction();
			enterExitHUD.SetUpEnterExitHUD(exitType);
		}


		public void ShowBuyLifeQueryHUD()
		{         
#if UNITY_IOS
			buyLifeQueryHUD.SetUpBuyLifeQueryView(delegate {
                SetUpPurchasePlaneOnIPhone(PurchaseManager.new_life_id);
            }, CancelBuyLife);
#elif UNITY_ANDROID
			buyLifeQueryHUD.SetUpBuyLifeQueryView(delegate {
				SetUpPurchasePlaneOnAndroid(PurchaseManager.new_life_id);
            }, CancelBuyLife);
#elif UNITY_EDITOR
			UnityEditor.BuildTarget buildTarget = UnityEditor.EditorUserBuildSettings.activeBuildTarget;

            switch (buildTarget) {
            case UnityEditor.BuildTarget.Android:
    			buyLifeQueryHUD.SetUpBuyLifeQueryView(delegate {
                    SetUpPurchasePlaneOnAndroid(PurchaseManager.new_life_id);
                }, CancelBuyLife);
                break;
            case UnityEditor.BuildTarget.iOS:
    			buyLifeQueryHUD.SetUpBuyLifeQueryView(delegate {
                    SetUpPurchasePlaneOnIPhone(PurchaseManager.new_life_id);
                }, CancelBuyLife);
                break;
            }
#endif
		}
        
			

		private void SetUpPurchasePlaneOnIPhone(string productID)
        {

			switch (Application.internetReachability)
            {
                case NetworkReachability.NotReachable:
                    hintHUD.SetUpSingleTextTintHUD("无网络连接");
                    break;
                case NetworkReachability.ReachableViaCarrierDataNetwork:
                case NetworkReachability.ReachableViaLocalAreaNetwork:
						QuitFight();
                        purchaseHUD.SetUpPurchasePendingHUDOnIPhone(PurchaseManager.new_life_id, PlayerFullyRecomeToLifeCallBack);
                        HideFullMask();            
                    break;
            }         
		}

		public void SetUpPurchasePlaneOnAndroid(string productID)
        {
            
			AdRewardType rewardType = AdRewardType.Life;

			MyAdType adType = MyAdType.RewardedVideoAd;

			if(productID.Equals(PurchaseManager.skill_point_id)){
				rewardType = AdRewardType.SkillPoint;
				adType = MyAdType.CPAd;
			}else if(productID.Equals(PurchaseManager.new_life_id)){
				rewardType = AdRewardType.Life;
				adType = MyAdType.RewardedVideoAd;
			}

            switch (Application.internetReachability)
            {
                case NetworkReachability.NotReachable:
                    hintHUD.SetUpSingleTextTintHUD("无网络连接");
                    break;
                case NetworkReachability.ReachableViaCarrierDataNetwork:
                case NetworkReachability.ReachableViaLocalAreaNetwork:
					if(productID.Equals(PurchaseManager.new_life_id)){
						QuitFight();
                        HideFullMask();
						purchaseHUD.SetUpPurchasePendingHUDOnAndroid(productID, adType, rewardType, delegate (MyAdType myAdType) {
							PlayerPartlyRecomeToLifeCallBack();
						}, delegate {
							buyLifeQueryHUD.SetUpWhenADUnexpectedFailOnAndroid(delegate {
                                SetUpPurchasePlaneOnAndroid(PurchaseManager.new_life_id);
                            }, CancelBuyLife);                     
						});
					}else if(productID.Equals(PurchaseManager.skill_point_id)){
						enterExitHUD.QuitEnterExitHUD();
						purchaseHUD.SetUpPurchasePendingHUDOnAndroid(productID, adType, rewardType, delegate (MyAdType myAdType) {
							PlayerGetASkillPointReward();
                        }, null);

					}
                    break;
            }
        }

        private void PlayerFullyRecomeToLifeCallBack(){
			buyLifeQueryHUD.QuitBuyLifeView();
			ExploreManager.Instance.battlePlayerCtr.RecomeToLife();
			bpUICtr.UpdateAgentStatusPlane();
            ExploreManager.Instance.MapWalkableEventsStartAction();
		}

		private void PlayerPartlyRecomeToLifeCallBack(){
			buyLifeQueryHUD.QuitBuyLifeView();
		    ExploreManager.Instance.battlePlayerCtr.PartlyRecomeToLife();
            bpUICtr.UpdateAgentStatusPlane();
            ExploreManager.Instance.MapWalkableEventsStartAction();
		}

		private void PlayerGetASkillPointReward(){
         
			Player.mainPlayer.skillNumLeft++;

			GameManager.Instance.persistDataManager.AddOneSkillPointToPlayerDataFile();
		}

		private void CancelBuyLife(){
			
			Player.mainPlayer.needChooseDifficulty = true;

			transitionView.PlayTransition (TransitionType.Death, delegate {
				
				transitionMask.gameObject.SetActive(true);

				transitionMask.color = Color.black;

				GameManager.Instance.persistDataManager.ResetDataWhenPlayerDie();
                            
				GameManager.Instance.UIManager.SetUpCanvasWith(CommonData.loadingCanvasBundleName, "LoadingCanvas", delegate
				{
					TransformManager.FindTransform("LoadingCanvas").GetComponent<LoadingViewController>().SetUpLoadingView(LoadingType.EnterExplore, delegate {
						ExploreManager.Instance.SetUpExploreView(MapSetUpFrom.Home);                  
			        }, ExploreManager.Instance.MapWalkableEventsStartAction);
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
