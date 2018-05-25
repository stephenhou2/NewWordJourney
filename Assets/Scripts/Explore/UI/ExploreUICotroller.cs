using System.Collections;
using UnityEngine;



namespace WordJourney
{
	using UnityEngine.UI;
	using DG.Tweening;

	public class ExploreUICotroller : MonoBehaviour {

		public TintHUD tintHUD;
		public PauseHUD pauseHUD;
		public WordHUD wordHUD;
        public WordDetailHUD wordDetailHUD;
        public BillboardHUD billboardHUD;

        public Text wordRecordText;
		public Transform exploreMask;// 小遮罩，不遮盖底部消耗品栏和背包按钮

		/**********  battlePlane UI *************/
		public Transform battlePlane;

		/**********  battlePlane UI *************/

		public NPCUIController npcUIController;

		public Text gameLevelLocationText;

		public Image transitionMask;

		public Transform tasksDescriptionContainer;

		public Text taskDescriptionModel;

		public InstancePool taskDescriptionPool;


		public Transform billboardPlane;
		public Transform billboard;


		public BattlePlayerUIController bpUICtr;
		public BattleMonsterUIController bmUICtr;


		public TransitionView transitionView;

		public SimpleItemDetail simpleItemDetail;

		public BuyLifeQueryView buyLifeQueryHUD;
		public Transform enterNextLevelQueryHUD;

		public CharacterFragmentsHUD characterFragmentsHUD;

		public SpellItemView spellItemView;

		public Transform fullMask;// 覆盖整个屏幕的遮罩，禁止一切点击响应

        // 记录上一个碰到的单词
        private HLHWord wordRecord;

		public void SetUpExploreCanvas(){

			transitionMask.gameObject.SetActive (true);

			pauseHUD.InitPauseHUD (true, ConfirmQuitToHomeView, null, null);

            wordHUD.InitWordHUD (true, QuitWordHUDCallBack,ChooseAnswerInWordHUDCallBack,ConfirmCharacterFillInWordHUDCallBack);

			if (!GameManager.Instance.UIManager.UIDic.ContainsKey ("BagCanvas")) {

				GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.bagCanvasBundleName, "BagCanvas", () => {
					Transform bagCanvas = TransformManager.FindTransform ("BagCanvas");
					bagCanvas.GetComponent<BagViewController> ().SetUpBagView (false);
				}, false,true);
					
			}

			gameLevelLocationText.text = string.Format("第 {0} 层",Player.mainPlayer.currentLevelIndex + 1);
            wordRecordText.text = string.Empty;

			bpUICtr.InitExploreAgentView ();
			bpUICtr.SetUpExplorePlayerView (Player.mainPlayer);
			bmUICtr.InitExploreAgentView ();

			GetComponent<Canvas> ().enabled = true;
			transitionMask.color = Color.black;


			if (!Player.mainPlayer.isNewPlayer) {
				bool playerDataExist = DataHandler.FileExist (CommonData.persistDataPath + "PlayerData.json");
				if (!playerDataExist) {
					GameManager.Instance.persistDataManager.SaveCompletePlayerData ();
				}
				ShowExploreSceneSlowly ();
			} else {
				Player.mainPlayer.isNewPlayer = false;
				GameManager.Instance.persistDataManager.SaveCompletePlayerData ();
				ExploreManager.Instance.AllWalkableEventsStopMove ();
				transitionView.PlayTransition (TransitionType.Introduce,ShowExploreSceneSlowly);
			}

			characterFragmentsHUD.InitCharacterFragmentsHUD(SetUpSpellItemView);

		}

		public void DisplayTransitionMaskAnim(CallBack cb){
			ExploreManager.Instance.AllWalkableEventsStopMove ();
			transitionMask.gameObject.SetActive (true);
			StartCoroutine ("TransitionMaskShowAndHide", cb);
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

			if (cb != null) {
				cb ();
			}

			while (tempAlpha > 0) {
				tempAlpha -= fadeSpeed * Time.deltaTime;
				transitionMask.color = new Color (0, 0, 0, tempAlpha);
				yield return null;
			}
			transitionMask.gameObject.SetActive (false);

			ExploreManager.Instance.AllWalkableEventsStartMove ();

		}

		//public void UpdateTasksDescription(){

		//	taskDescriptionPool.AddChildInstancesToPool (tasksDescriptionContainer);

		//	Player player = Player.mainPlayer;

		//	for (int i = 0; i < player.inProgressTasks.Count; i++) {

		//		HLHTask task = player.inProgressTasks [i];

		//		Text taskDescription = taskDescriptionPool.GetInstance<Text> (taskDescriptionModel.gameObject, tasksDescriptionContainer);

		//		bool isTaskFinish = player.CheckTaskFinish (task);

		//		if (isTaskFinish) {
		//			taskDescription.text = string.Format ("<color=green>{0} {1}/{2}</color>", task.taskDescription, task.taskItemCount, task.taskItemCount);
		//		} else {
		//			Item taskItem = player.allTaskItemsInBag.Find (delegate(TaskItem obj) {
		//				return obj.itemId == task.taskItemId;
		//			});

		//			int taskItemCountInBag = 0;

		//			if (taskItem != null) {
		//				taskItemCountInBag = taskItem.itemCount;
		//			}

		//			taskDescription.text = string.Format("<color=white>{0} {1}/{2}</color>", task.taskDescription, taskItemCountInBag, task.taskItemCount);
		//		}
		//	}
		//}

		public void SetUpSpellItemView(){

			spellItemView.SetUpSpellView(ExploreManager.Instance.newMapGenerator.spellItemOfCurrentLevel);
		}

		public void UpdateCharacterFragmentsHUD(){
			characterFragmentsHUD.UpdateCharactersCollected();
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


		private void ShowExploreSceneSlowly(){
			transitionMask.gameObject.SetActive (true);
			transitionMask.color = Color.black;
			transitionMask.DOFade (0, 1f).OnComplete (delegate {
				HideExploreMask();
				transitionMask.gameObject.SetActive(false);
				ExploreManager.Instance.AllWalkableEventsStartMove();
			});
		}

		public void ShowFightPlane(){

			if (battlePlane.gameObject.activeInHierarchy) {
				return;
			}

			battlePlane.gameObject.SetActive (true);

			bpUICtr.SetUpFightPlane ();

		}

		public void UpdateActiveSkillButtons(){
			bpUICtr.SetUpActiveSkillButtons ();
		}

		public void HideFightPlane(){
			battlePlane.gameObject.SetActive (false);

			bpUICtr.QuitFightPlane ();
		}


		public void ShowLevelUpPlane(){
			bpUICtr.ShowLevelUpPlane ();
		}

		public void SetUpSingleTextTintHUD(string tint){
			tintHUD.SetUpSingleTextTintHUD (tint);
		}

		public void SetUpGoldGainTintHUD(int goldGain){
			tintHUD.SetUpGoldTintHUD (goldGain);
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
			npcUIController.SetUpNpcPlane (npc);
		}

		//public void SetUpNPCWhenWordChooseRight(HLHNPC npc){
		//	npcUIController.SetUpChooseRightDialog (npc);
		//}

		//public void SetUpNPCWhenWordChooseWrong(HLHNPC npc){
		//	npcUIController.SetUpChooseWrongDialog (npc);
		//}


		public void ShowNPCPlane(){
			npcUIController.ShowNPCPlane ();
		}

		/// <summary>
		/// 初始化公告牌
		/// </summary>
		public void SetUpBillboard(Billboard bb){
			
			ExploreManager.Instance.AllWalkableEventsStopMove ();

            billboardHUD.SetUpBillboard(bb, QuitBillboard);
		}

		/// <summary>
		/// 退出公告牌
		/// </summary>
        private void QuitBillboard(){
			ExploreManager.Instance.battlePlayerCtr.isInEvent = false;
			ExploreManager.Instance.AllWalkableEventsStartMove ();
		}


		/// <summary>
		/// 显示解锁卷轴详细信息
		/// </summary>
		/// <param name="item">Item.</param>
//		public void SetUpUnlockScrollHUD(Item item){
//			unlockScrollDetail.SetUpUnlockScrollDetailHUD (item);
//		}

//		/// <summary>
//		/// 解锁卷轴展示界面点击事件原本就会退出展示页面，一般情况下不用主动调用这个方法
//		/// </summary>
//		public void QuitUnlockScrollHUD(){
//			unlockScrollDetail.QuitUnlockScrollDetailHUD ();
//		}

//		/// <summary>
//		/// 显示合成界面
//		/// </summary>
//		/// <param name="item">Item.</param>
//		public void SetUpCraftingRecipesHUD(Item item){
////			craftingRecipesDetail.SetUpCraftingRecipesHUD (item);
//		}

//		/// <summary>
//		/// 合成界面点击事件原本就会退出展示页面，一般情况下不用主动调用这个方法
//		/// </summary>
//		public void QuitCraftingRecipesHUD(){
//			craftingRecipesDetail.QuitCraftingRecipesHUD ();
//		}




//		public void SetUpSpellView(){
//
//			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.spellCanvasBundleName, "SpellCanvas", () => {
//				EquipmentModel swordModel = GameManager.Instance.gameDataCenter.allItemModels.Find(delegate(EquipmentModel obj){
//					return obj.itemId == 0;
//				});
//				TransformManager.FindTransform ("SpellCanvas").GetComponent<SpellViewController> ().SetUpSpellViewForCreate (swordModel,null);
//			}, false, true);
//
//		}

		public void OnPauseButtonClick(){
			ShowPauseHUD ();
		}

		public void ShowPauseHUD(){
			pauseHUD.SetUpPauseHUD ();
		}

		public void QuitPauseHUD(){
			pauseHUD.QuitPauseHUD ();
		}

//		public void OnDefenceButtonUp(){
//
//			exploreManager.DefenceUp ();
//
//		}
//
//		public void OnDefenctButtonDown(){
//			exploreManager.DefenceDown ();
//		}


		public void SetUpWordHUD(HLHWord[] words,string extraInfo = null){
			wordHUD.SetUpWordHUDAndShow (words,extraInfo);
		}

		public void SetUpWordHUD(HLHWord word){
			wordHUD.SetUpWordHUDAndShow (word);
		}

        public void SetUpWordDetailHUD(){
            if (wordRecord != null)
            {
                ExploreManager.Instance.AllWalkableEventsStopMove();
                wordDetailHUD.SetUpWordDetailHUD(wordRecord,ExploreManager.Instance.AllWalkableEventsStartMove);
            }
        }

        private void ConfirmCharacterFillInWordHUDCallBack(bool isFillCorrect){

			ExploreManager.Instance.ConfirmFillCharactersInWordHUD (isFillCorrect);

		}

        private void QuitWordHUDCallBack(HLHWord word)
		{
            ExploreManager.Instance.AllWalkableEventsStartMove();

            if (word == null)
            {
                return;
            }

            wordRecordText.text = word.spell;

            wordRecord = word;
		}

		private void ChooseAnswerInWordHUDCallBack(bool isChooseCorrect){

			ExploreManager.Instance.ChooseAnswerInWordHUD (isChooseCorrect);

		}



		public void SetUpSimpleItemDetail(Item item){
			simpleItemDetail.SetupSimpleItemDetail (item);
		}

		public void ShowEnterNextLevelQueryHUD(){
			enterNextLevelQueryHUD.gameObject.SetActive (true);
		}

		public void HideEnterNextLevelQueryHUD(){
			enterNextLevelQueryHUD.gameObject.SetActive (false);
		}

		public void ConfirmEnterNextLevel(){
			HideEnterNextLevelQueryHUD ();
			ExploreManager.Instance.EnterNextLevel ();
			ExploreManager.Instance.battlePlayerCtr.isInEvent = false;
		}

		public void CancelEnterNextLevel(){
			HideEnterNextLevelQueryHUD ();
			ExploreManager.Instance.battlePlayerCtr.isInEvent = false;
		}


		public void ShowBuyLifeQueryHUD(){
			buyLifeQueryHUD.SetUpBuyLifeQueryView (ConfirmBuyLife, CancelBuyLife);
		}
			

		private void ConfirmBuyLife(){
			Debug.Log ("BUY LIFE");
			QuitFight ();
			ExploreManager.Instance.battlePlayerCtr.RecomeToLife ();
			HideFullMask ();
			bpUICtr.UpdateAgentStatusPlane ();
			ExploreManager.Instance.AllWalkableEventsStartMove ();
		}

		private void CancelBuyLife(){
			transitionView.PlayTransition (TransitionType.Death, delegate {
				transitionMask.gameObject.SetActive(true);
				transitionMask.color = Color.black;
				GameManager.Instance.persistDataManager.ResetPlayerDataToOriginal ();
				ExploreManager.Instance.QuitExploreScene (true);
			});
		}

		private void ConfirmQuitToHomeView(){
			transitionView.PlayTransition(TransitionType.Quit, delegate
			{
				transitionMask.gameObject.SetActive(true);
				transitionMask.color = Color.black;
				//GameManager.Instance.persistDataManager.ResetPlayerDataToOriginal();
				ExploreManager.Instance.QuitExploreScene(true);
			});
		}
			
//		public void OnRefreshButtonClick(){
//			queryType = QueryType.Refresh;
//			ShowQueryHUD ();
//		}

//		public void OnHomeButtonClick(){
//			queryType = QueryType.Quit;
//			ShowQueryHUD ();
//		}

//		public void OnSettingsButtonClick(){
//			GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.settingCanvasBundleName, "SettingCanvas", () => {
//				TransformManager.FindTransform("SettingCanvas").GetComponent<SettingViewController>().SetUpSettingView();
//			},false,true);
//			QuitPauseHUD ();
//		}

//		public void ShowQueryHUD(){
//			pauseHUD.SetUpPauseHUD ();
//		}
//		public void QuitQueryHUD(){
//			pauseHUD.QuitPauseHUD ();
//		}



//		private void UnlockItemCallBack(){
//			UnlockScroll currentSelectedUnlockScroll= unlockScrollDetail.unlockScroll;
//			currentSelectedUnlockScroll.unlocked = true;
//			Player.mainPlayer.RemoveItem (currentSelectedUnlockScroll,1);
//			string tint = string.Format ("解锁拼写 <color=orange>{0}</color>", currentSelectedUnlockScroll.itemName);
//			SetUpTintHUD (tint,null);
//		}
//
//		private void ResolveScrollCallBack(){
//			
//			UnlockScroll currentSelectUnlockScroll = unlockScrollDetail.unlockScroll;
//
////			List<char> charactersReturn = Player.mainPlayer.ResolveItemAndGetCharacters (currentSelectUnlockScroll,1);
//
////			StringBuilder tint = new StringBuilder ();
////
////			tint.Append ("获得字母碎片");
////
////			for (int i = 0; i < charactersReturn.Count; i++) {
////				tint.Append (charactersReturn [i]);
////			}
////
////			tintHUD.SetUpTintHUD (tint.ToString(),null);
//		}

//		private void CraftItemCallBack(){
//
//			CraftingRecipe craftingRecipe = craftingRecipesDetail.craftingRecipe;
//
//			EquipmentModel craftItemModel = GameManager.Instance.gameDataCenter.allItemModels.Find (delegate(EquipmentModel obj) {
//				return obj.itemId == craftingRecipe.craftItemId;
//			});
//
//			for (int i = 0; i < craftItemModel.itemInfosForProduce.Length; i++) {
//				EquipmentModel.ItemInfoForProduce itemInfo = craftItemModel.itemInfosForProduce [i];
//				for (int j = 0; j < itemInfo.itemCount; j++) {
//					Item item = Player.mainPlayer.allItemsInBag.Find (delegate (Item obj) {
//						return obj.itemId == itemInfo.itemId;
//					});
//					if (item == null) {
//						item = Player.mainPlayer.GetEquipedEquipment (itemInfo.itemId);
//					}
//					Player.mainPlayer.RemoveItem (item,1);
//				}
//			}
//
//			Item craftedItem = Item.NewItemWith (craftItemModel,1);
//			Player.mainPlayer.AddItem (craftedItem);
//
//			string tint = string.Format ("获得 <color=orange>{0}</color> x1", craftedItem.itemName);
//
//			Sprite itemSprite = GameManager.Instance.gameDataCenter.allItemSprites.Find (delegate(Sprite obj) {
//				return obj.name == craftedItem.spriteName;
//			});
//
//			SetUpTintHUD (tint,itemSprite);
//
//		}




		public void QuitFight(){
			bpUICtr.QuitFightPlane ();
			bmUICtr.QuitFightPlane ();
			HideFightPlane ();
		}


		public void QuitExplore(){
			
			GameManager.Instance.UIManager.RemoveCanvasCache ("ExploreCanvas");

		}



		public void DestroyInstances(){
			
//			GameManager.Instance.UIManager.RemoveCanvasCache ("ExploreCanvas");

//			StartCoroutine ("LatelyDestroyView");
//		}
//
//		private IEnumerator LatelyDestroyView(){
//
//			yield return new WaitForSeconds (0.3f);
			transitionMask.gameObject.SetActive(true);
			Destroy (this.gameObject,0.3f);
		}
			

		void OnDestroy(){
//			tintHUD = null;
//			unlockScrollDetail = null;
//			craftingRecipesDetail = null;
//			pauseHUD = null;
//			crystalQueryHUD = null;
//			npcUIController = null;
//			bpUICtr = null;
//			bmUICtr = null;
		}
	}
}
