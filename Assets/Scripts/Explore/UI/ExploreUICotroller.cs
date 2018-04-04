using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace WordJourney
{
	using UnityEngine.UI;
	using DG.Tweening;
	using System.Text;

	public class ExploreUICotroller : MonoBehaviour {

		public TintHUD tintHUD;
		public UnlockScrollDetailHUD unlockScrollDetail;
		public CraftingRecipesHUD craftingRecipesDetail;
		public PauseHUD pauseHUD;
		public WordHUD wordHUD;

		public Transform mask;

		/**********  battlePlane UI *************/
		public Transform battlePlane;

		/**********  battlePlane UI *************/

		public NPCUIController npcUIController;

		public Text gameLevelLocationText;

		public Transform transitionMask;

		public Transform tasksDescriptionContainer;

		public Text taskDescriptionModel;

		public InstancePool taskDescriptionPool;


		public Transform billboardPlane;
		public Transform billboard;


		public BattlePlayerUIController bpUICtr;
		public BattleMonsterUIController bmUICtr;

//		private ExploreManager mExploreManager;
//		private ExploreManager exploreManager{
//			get{
//				if (mExploreManager == null) {
//					mExploreManager = ExploreManager.Instance.GetComponent<ExploreManager>();
//				}
//
//				return mExploreManager;
//			}
//		}

		public IntroductionView introductionPlane;

		public SimpleItemDetail simpleItemDetail;


		public void SetUpExploreCanvas(){


//			unlockScrollDetail.InitUnlockScrollDetailHUD (true, null, UnlockItemCallBack, ResolveScrollCallBack);
//			craftingRecipesDetail.InitCraftingRecipesHUD (true, UpdateBottomBar, CraftItemCallBack);
//			npcUIController.InitNPCHUD (gameLevelIndex);
			pauseHUD.InitPauseHUD (true, null, null, null, null);

			wordHUD.InitWordHUD (true, ExploreManager.Instance.AllWalkableEventsStartMove,ChooseAnswerInWordHUD,ConfirmCharacterFillInWordHUD);

			if (!GameManager.Instance.UIManager.UIDic.ContainsKey ("BagCanvas")) {

				GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.bagCanvasBundleName, "BagCanvas", () => {
					Transform bagCanvas = TransformManager.FindTransform ("BagCanvas");
					bagCanvas.GetComponent<BagViewController> ().SetUpBagView (false);
				}, false,true);
					
			}
				
//			string gameLevelInCurrentLocation = MyTool.NumberToChinese(gameLevelIndex % 5 + 1);
			gameLevelLocationText.text = string.Format("第 {0} 层",Player.mainPlayer.currentLevelIndex);


			bpUICtr.InitExploreAgentView ();
			bpUICtr.SetUpExplorePlayerView (Player.mainPlayer);
			bmUICtr.InitExploreAgentView ();

			GetComponent<Canvas> ().enabled = true;
			transitionMask.gameObject.SetActive (false);

			if (!Player.mainPlayer.isNewPlayer) {
				HideMask ();
			} else {
				introductionPlane.InitIntroductionView (HideMask);
				introductionPlane.PlayIntroductionTransition ();
			}

		}

		public void UpdateTasksDescription(){

			taskDescriptionPool.AddChildInstancesToPool (tasksDescriptionContainer);

			Player player = Player.mainPlayer;

			for (int i = 0; i < player.inProgressTasks.Count; i++) {

				HLHTask task = player.inProgressTasks [i];

				Text taskDescription = taskDescriptionPool.GetInstance<Text> (taskDescriptionModel.gameObject, tasksDescriptionContainer);

				bool isTaskFinish = player.CheckTaskFinish (task);

				if (isTaskFinish) {
					taskDescription.text = string.Format ("<color=green>{0} {1}/{2}</color>", task.taskDescription, task.taskItemCount, task.taskItemCount);
				} else {
					Item taskItem = player.allTaskItemsInBag.Find (delegate(TaskItem obj) {
						return obj.itemId == task.taskItemId;
					});

					int taskItemCountInBag = 0;

					if (taskItem != null) {
						taskItemCountInBag = taskItem.itemCount;
					}

					taskDescription.text = string.Format("<color=white>{0} {1}/{2}</color>", task.taskDescription, taskItemCountInBag, task.taskItemCount);
				}
			}
		}

		public void ShowMask(){
			mask.gameObject.SetActive (true);
		}

		public void HideMask(){
			mask.gameObject.SetActive (false);
		}

		public void ShowFightPlane(){

			if (battlePlane.gameObject.activeInHierarchy) {
				return;
			}

			battlePlane.gameObject.SetActive (true);

			bpUICtr.SetUpFightPlane ();

		}

		public void HideFightPlane(){
			battlePlane.gameObject.SetActive (false);

			bpUICtr.QuitFightPlane ();
		}


		public void ShowLevelUpPlane(){
			bpUICtr.ShowLevelUpPlane ();
		}

		public void SetUpTintHUD(string tint,Sprite sprite){
			tintHUD.SetUpTintHUD (tint,sprite);
		}

		/// <summary>
		/// 更新底部消耗品栏
		/// </summary>
		public void UpdateBottomBar(){
			bpUICtr.SetUpConsumablesButtons ();
		}

		public void UpdatePlayerStatusBar(){
			bpUICtr.UpdateAgentStatusPlane ();
		}



//		private IEnumerator PlayTintTextAnim(Transform tintText){
//
//			yield return new WaitForSeconds (1f);
//
//
//
//		}

		public void EnterNPC(HLHNPC npc,int currentLevelIndex){

			npcUIController.SetUpNpcPlane (npc);

		}

		public void ShowNPCPlane(){
			npcUIController.ShowNPCPlane ();
		}

		/// <summary>
		/// 初始化公告牌
		/// </summary>
		public void SetUpBillboard(Billboard bb){
			billboardPlane.gameObject.SetActive (true);
			Text billboardContent = billboard.Find ("Content").GetComponent<Text> ();
			billboardContent.text = bb.content;
			ExploreManager.Instance.AllWalkableEventsStopMove ();
		}

		/// <summary>
		/// 退出公告牌
		/// </summary>
		public void QuitBillboard(){
			billboardPlane.gameObject.SetActive (false);
			ExploreManager.Instance.AllWalkableEventsStartMove ();
		}


		/// <summary>
		/// 显示解锁卷轴详细信息
		/// </summary>
		/// <param name="item">Item.</param>
//		public void SetUpUnlockScrollHUD(Item item){
//			unlockScrollDetail.SetUpUnlockScrollDetailHUD (item);
//		}

		/// <summary>
		/// 解锁卷轴展示界面点击事件原本就会退出展示页面，一般情况下不用主动调用这个方法
		/// </summary>
		public void QuitUnlockScrollHUD(){
			unlockScrollDetail.QuitUnlockScrollDetailHUD ();
		}

		/// <summary>
		/// 显示合成界面
		/// </summary>
		/// <param name="item">Item.</param>
		public void SetUpCraftingRecipesHUD(Item item){
//			craftingRecipesDetail.SetUpCraftingRecipesHUD (item);
		}

		/// <summary>
		/// 合成界面点击事件原本就会退出展示页面，一般情况下不用主动调用这个方法
		/// </summary>
		public void QuitCraftingRecipesHUD(){
			craftingRecipesDetail.QuitCraftingRecipesHUD ();
		}




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


		public void SetUpWordHUD(LearnWord[] words){
			wordHUD.SetUpWordHUDAndShow (words);
		}

		public void SetUpWordHUD(LearnWord word){
			wordHUD.SetUpWordHUDAndShow (word);
		}

		private void ConfirmCharacterFillInWordHUD(bool isFillCorrect){

			ExploreManager.Instance.ConfirmFillCharactersInWordHUD (isFillCorrect);

		}

		private void ChooseAnswerInWordHUD(bool isChooseCorrect){

			ShowMask ();

			StartCoroutine ("ShowChooseResultForAWhile",isChooseCorrect);

//			exploreManager.ChooseAnswerInWordHUD (isChooseCorrect);
		}

		private IEnumerator ShowChooseResultForAWhile(bool isChooseCorrect){

			yield return new WaitForSeconds (1.0f);

			HideMask ();

			wordHUD.QuitWordHUD ();

			ExploreManager.Instance.ChooseAnswerInWordHUD (isChooseCorrect);

		}

		public void SetUpSimpleItemDetail(Item item){
			simpleItemDetail.SetupSimpleItemDetail (item);
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


//		public void PrepareForRefreshment(){
//			bpUICtr.PrepareForRefreshment ();
//			bmUICtr.PrepareForRefreshment ();
//			npcUIController.QuitNPCPlane ();
//			QuitFight ();
//			QuitCrystalQuery ();
//			craftingRecipesDetail.QuitCraftingRecipesHUD ();
//			unlockScrollDetail.QuitUnlockScrollDetailHUD ();
//			tintHUD.QuitTintHUD ();
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
