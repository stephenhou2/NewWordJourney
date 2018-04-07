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

		public Transform onChoicePlane;
		public Transform twoChoicePlane;
		public Transform threeChoicePlane;

		public Button choiceBtnInOnChoicePlane;
		public Button[] choiceBtnsInTwoChoicePlane;
		public Button[] choiceBtnsInThreeChoicePlane;

		/**********  dialogPlane UI *************/


		/**********  tradePlane UI *************/
		public Transform goodsDisplayPlane;
		public Transform goodsContainer;
		public InstancePool goodsPool;
		public Transform goodsModel;
		/**********  tradePlane UI *************/

		public BagItemsDisplay bagItemsDisplay;

		public ItemDetailInTrade itemDetail;

		public SpecialOperationHUD specialOperationHUD;

		public TintHUD tintHUD;

		private Item currentSelectedItem;

		public float flyDuration;

		public Vector3 goodsPlaneStartPos;

		public Vector3 itemDetailPlaneStartPos;

		public Vector3 specialOperationHUDStartPos;

		public Vector3 bagItemsPlaneStartPos;

		public int goodsPlaneMoveEndX;

		public int itemDetailMoveEndX;

		public int specialOpeartionHUDMoveEndX;

		public int bagItemsPlaneMoveEndY;

		public Button quitTradeButton;
		public Button quitSpecialOperationButton;

		public PurchasePendingHUD purchaseHUD;


		public void ShowNPCPlane(){
			gameObject.SetActive (true);
		}

		public void HideNPCPlane(){
			gameObject.SetActive (false);
		}

		public void SetUpNpcPlane(HLHNPC npc){

			this.npc = npc;

			npcName.text = npc.npcName;

			HLHDialogGroup dg = npc.FindQulifiedDialogGroup (Player.mainPlayer);

			HLHDialog dialog = dg.dialogs [0];

			SetUpDialogPlane (dialog,dg);
			ResetTradeAndSpecialOperationPlane ();



			gameObject.SetActive (true);

		}
			
		public void SetUpDialogPlane(HLHDialog dialog,HLHDialogGroup dg){

			EnterDialogDisplay ();

			dialogText.text = dialog.dialogContent;

			HLHChoice[] choices = dg.GetChoices (dialog);

			if (choices.Length == 1) {

				HLHChoice choice = choices [0];

				onChoicePlane.gameObject.SetActive (true);
				twoChoicePlane.gameObject.SetActive (false);
				threeChoicePlane.gameObject.SetActive (false);

				SetUpChoiceButton (choiceBtnInOnChoicePlane, choice, dg);

			} else if (choices.Length == 2) {
				
				onChoicePlane.gameObject.SetActive (false);
				twoChoicePlane.gameObject.SetActive (true);
				threeChoicePlane.gameObject.SetActive (false);

				for (int i = 0; i < 2; i++) {

					Button choiceButton = choiceBtnsInTwoChoicePlane [i];

					HLHChoice choice = choices [i];

					SetUpChoiceButton (choiceButton, choice, dg);

				}
			} else if (choices.Length == 3) {

				onChoicePlane.gameObject.SetActive (false);
				twoChoicePlane.gameObject.SetActive (false);
				threeChoicePlane.gameObject.SetActive (true);

				for (int i = 0; i < 3; i++) {

					Button choiceButton = choiceBtnsInThreeChoicePlane [i];

					HLHChoice choice = choices [i];

					SetUpChoiceButton (choiceButton, choice, dg);

				}

			}
		}
			
		private void SetUpChoiceButton(Button choiceButton,HLHChoice choice,HLHDialogGroup dg){
			
			Text choiceText = choiceButton.transform.Find ("ChoiceText").GetComponent<Text> ();

			string choiceContent = choice.choiceContent;

			choiceButton.interactable = true;

			bool hideDialogPlane = false;

			if (choice.isHandInTaskTriggered) {
				HLHTask task = npc.GetTask (choice.triggeredTaskId);
				bool isTaskFinish = Player.mainPlayer.CheckTaskFinish (task);

				if (isTaskFinish) {
					choiceContent = string.Format ("{0}(<color=green>{1}/{2}</color>)", choice.choiceContent, task.taskItemCount, task.taskItemCount);

				} else {
					Item taskItem = Player.mainPlayer.allTaskItemsInBag.Find (delegate(TaskItem obj) {
						return obj.itemId == task.taskItemId;
					});

					int taskItemCountInBag = 0;

					if (taskItem != null) {
						taskItemCountInBag = taskItem.itemCount;
					}

					choiceContent = string.Format ("{0}(<color=red>{1}/{2}</color>)", choice.choiceContent, taskItemCountInBag, task.taskItemCount);
					choiceButton.interactable = false;
				}
			}

			choiceText.text = choiceContent;

			choiceButton.onClick.RemoveAllListeners ();

			choiceButton.onClick.AddListener (delegate {

				if(choice.isHiddenPropertyChangeTriggered){
					Player.mainPlayer.HiddenPropertyChange(choice.playerJusticeChange,choice.playerPowerChange);
				}
				if(choice.isReceiveTaskTriggered){
					HLHTask task = npc.GetTask(choice.triggeredTaskId);
					Player.mainPlayer.ReceiveTask(task);
					ExploreManager.Instance.expUICtr.UpdateTasksDescription();
				}
				if(choice.isHandInTaskTriggered){
					HLHTask task = npc.GetTask(choice.triggeredTaskId);
					Player.mainPlayer.HandInTask(task);
					ExploreManager.Instance.expUICtr.UpdateTasksDescription();
				}
				if(choice.isRewardTriggered){
					int randomSeed = Random.Range(0,choice.possibleRewards.Count);
					HLHNPCReward reward = choice.possibleRewards[randomSeed];
					Player.mainPlayer.GainFromNPCReward(reward);
				}
				if(choice.isTradeTriggered){
					SetUpTrade();
					hideDialogPlane = true;
				}
				if(choice.isAddSkillTriggered){
					SetUpSpecialOperation();
					hideDialogPlane = true;
				}
				if(choice.isFightTriggered){
					HideNPCPlane();
					Transform mapNpc = ExploreManager.Instance.currentEnteredMapEvent.transform;
					ExploreManager.Instance.EnterFight(mapNpc);
					ExploreManager.Instance.PlayerAndMonsterStartFight();
					hideDialogPlane = true;
				}
					
				if(choice.isEnd){

					if(dg.isOneOff){
						dg.isFinish = true;
					}

					QuitNPCPlane();

					return;
				}

				HLHDialog newDialog =  dg.GetDialog(choice);

				SetUpDialogPlane(newDialog,dg);

				if(hideDialogPlane){
					QuitDialogDisplay();
				}

			});


		}

		public void InitPurchaseExtraBag(){
			purchaseHUD.SetUpPurchasePendingHUD (PurchaseManager.extra_bag_id,delegate {
				bagItemsDisplay.UpdateBagTabs ();
			});
		}
			
			

		public void SetUpTrade(){

			SoundManager.Instance.PlayAudioClip ("UI/sfx_UI_Trader");

			ClearItemDetail ();
			UpdateGoodsDisplay ();

			bagItemsDisplay.InitBagItemsDisplayPlane (OnItemInBagClickInTrade, InitPurchaseExtraBag);
			bagItemsDisplay.SetUpBagItemsPlane (0);

			quitTradeButton.gameObject.SetActive (true);
			quitSpecialOperationButton.gameObject.SetActive (false);
			EnterTradeDisplay ();
		}

		private void RefreshCurrentSelectItemDetailDisplay(){
			if (currentSelectedItem == null) {
				return;
			}
			itemDetail.SetUpItemDetail (currentSelectedItem, TradeType.None);
		}

		public void SetUpSpecialOperation(){
			
			SoundManager.Instance.PlayAudioClip ("UI/sfx_UI_Trader");

			ClearItemDetail ();

			bagItemsDisplay.InitBagItemsDisplayPlane (OnItemInBagClickInSpecialOperation, InitPurchaseExtraBag);

			bagItemsDisplay.SetUpBagItemsPlane (0);
			quitTradeButton.gameObject.SetActive (false);
			quitSpecialOperationButton.gameObject.SetActive (true);

			specialOperationHUD.InitSpecialOperationHUD (bagItemsDisplay.SetUpCurrentBagItemsPlane,RefreshCurrentSelectItemDetailDisplay);
		
			EnterSpecialOperationDisplay();

		}

		private void UpdateGoodsDisplay(){
			
			goodsPool.AddChildInstancesToPool (goodsContainer);

			List<HLHNPCGoods> itemsAsGoods = npc.npcGoodsList;

			for (int i = 0; i < itemsAsGoods.Count; i++) {

				HLHNPCGoods goods = itemsAsGoods [i];

//				if (goods.totalCount == 0 && goods.isFixedCount) {
//					continue;
//				}

				Transform goodsCell = goodsPool.GetInstance<Transform> (goodsModel.gameObject, goodsContainer);

				Item itemAsGoods = goods.GetGoodsItem ();

				goodsCell.GetComponent<GoodsCell> ().SetUpGoodsCell (itemAsGoods);

				goodsCell.GetComponent<Button>().onClick.RemoveAllListeners ();

				int goodsIndex = i;

				goodsCell.GetComponent<Button>().onClick.AddListener (delegate {
					currentSelectedItem = itemAsGoods;
					itemDetail.SetUpItemDetail (itemAsGoods,TradeType.Buy);
					UpdateGoodsSelection(goodsIndex);
				});

			}

		}

		private void UpdateGoodsSelection(int selectedGoodsIndex){

			for (int i = 0; i < goodsContainer.childCount; i++) {

				Transform goodsCell = goodsContainer.GetChild (i);

				goodsCell.GetComponent<GoodsCell> ().SetSelection (i == selectedGoodsIndex);

			}

			HLHNPCGoods goods = npc.npcGoodsList [selectedGoodsIndex];

			Item itemAsGoods = goods.GetGoodsItem ();

			itemDetail.SetUpItemDetail (itemAsGoods, TradeType.Buy);

		}
			

		private void OnItemInBagClickInTrade(Item item){

			itemDetail.SetUpItemDetail (item,TradeType.Sell);

			currentSelectedItem = item;

			bagItemsDisplay.HideAllItemSelectedTintIcon ();

		}

		private void OnItemInBagClickInSpecialOperation(Item item){

			itemDetail.SetUpItemDetail (item,TradeType.None);

			currentSelectedItem = item;

			bagItemsDisplay.HideAllItemSelectedTintIcon ();

		}

		public void OnBuyButtonClick(){

			if (currentSelectedItem == null) {
				return;
			}

			string tint = "";


			if (Player.mainPlayer.totalGold < currentSelectedItem.price) {
				tint = "水晶不足";
				tintHUD.SetUpTintHUD (tint,null);
				return;
			}

			Player player = Player.mainPlayer;

			player.totalGold -= currentSelectedItem.price;

			player.AddItem (currentSelectedItem);

			npc.SoldGoods (currentSelectedItem.itemId);

			if (Player.mainPlayer.CheckBagFull (currentSelectedItem)) {
				GameManager.Instance.UIManager.SetUpCanvasWith (CommonData.bagCanvasBundleName, "BagCanvas", () => {
					
					TransformManager.FindTransform ("BagCanvas").GetComponent<BagViewController>().AddBagItemWhenBagFull (currentSelectedItem);

				}, false, true);
					
			}

			ExploreManager.Instance.expUICtr.GetComponent<BattlePlayerUIController>().SetUpConsumablesButtons ();

			currentSelectedItem = null;

			ClearItemDetail ();

			UpdateGoodsDisplay ();

			bagItemsDisplay.SetUpCurrentBagItemsPlane ();

		}

		public void OnSellButtonClick(){

			if (currentSelectedItem == null) {
				return;
			}

			Player player = Player.mainPlayer;

			player.totalGold += currentSelectedItem.price / 8;

			bool totallyRemoveFromBag = player.RemoveItem (currentSelectedItem, 1);

//			npc.BuyGoods (currentSelectedItem.itemId);

			ExploreManager.Instance.expUICtr.GetComponent<BattlePlayerUIController>().SetUpConsumablesButtons ();

			if (totallyRemoveFromBag) {
				currentSelectedItem = null;
				ClearItemDetail ();
			}

			UpdateGoodsDisplay ();

			bagItemsDisplay.SetUpCurrentBagItemsPlane ();
				
		}


		private void ClearItemDetail(){
			itemDetail.ClearItemDetails ();
		}

		private void EnterDialogDisplay(){
			dialogPlane.gameObject.SetActive (true);
			itemDetail.transform.localPosition = itemDetailPlaneStartPos;
			goodsDisplayPlane.transform.localPosition = goodsPlaneStartPos;
			specialOperationHUD.transform.localPosition = specialOperationHUDStartPos;
			bagItemsDisplay.transform.localPosition = bagItemsPlaneStartPos;
		}

		private void EnterTradeDisplay(){
			dialogPlane.gameObject.SetActive (false);
			itemDetail.transform.localPosition = itemDetailPlaneStartPos;
			itemDetail.transform.DOLocalMoveX (itemDetailMoveEndX, flyDuration);
			goodsDisplayPlane.transform.localPosition = goodsPlaneStartPos;
			goodsDisplayPlane.transform.DOLocalMoveX (goodsPlaneMoveEndX, flyDuration);
			specialOperationHUD.transform.localPosition = specialOperationHUDStartPos;
			bagItemsDisplay.transform.localPosition = bagItemsPlaneStartPos;
			bagItemsDisplay.transform.DOLocalMoveY (bagItemsPlaneMoveEndY, flyDuration);
		}

		private void EnterSpecialOperationDisplay(){
			dialogPlane.gameObject.SetActive (false);
			itemDetail.transform.DOLocalMoveX (itemDetailMoveEndX, flyDuration);
			goodsDisplayPlane.transform.localPosition = goodsPlaneStartPos;
			specialOperationHUD.transform.localPosition = specialOperationHUDStartPos;
			specialOperationHUD.transform.DOLocalMoveX (specialOpeartionHUDMoveEndX, flyDuration);
			bagItemsDisplay.transform.localPosition = bagItemsPlaneStartPos;
			bagItemsDisplay.transform.DOLocalMoveY (bagItemsPlaneMoveEndY, flyDuration);
		}

		private void QuitDialogDisplay(){
			dialogPlane.gameObject.SetActive (false);
		}

		private void QuitTradeDisplay(){
			dialogPlane.gameObject.SetActive (true);
			itemDetail.transform.DOLocalMoveX (itemDetailPlaneStartPos.x, flyDuration);
			goodsDisplayPlane.transform.DOLocalMoveX (goodsPlaneStartPos.x, flyDuration);
			specialOperationHUD.transform.localPosition = specialOperationHUDStartPos;
			bagItemsDisplay.transform.DOLocalMoveY (bagItemsPlaneStartPos.y, flyDuration);
		}

		private void QuitSpecialOperationDisplay(){
			dialogPlane.gameObject.SetActive (true);
			itemDetail.transform.DOLocalMoveX (itemDetailPlaneStartPos.x, flyDuration);
			goodsDisplayPlane.transform.localPosition = goodsPlaneStartPos;
			specialOperationHUD.transform.DOLocalMoveX (specialOperationHUDStartPos.x, flyDuration);
			bagItemsDisplay.transform.DOLocalMoveY (bagItemsPlaneStartPos.y, flyDuration);
		}


//		private void ShowDialogPlane(){
//			dialogPlane.gameObject.SetActive (true);
//		}
//
//		private void HideDialogPlane(){
//			dialogPlane.gameObject.SetActive (false);
//		}
//
//
//
//		private void ShowItemDetail(){
//			itemDetail.transform.localPosition = itemDetailPlaneStartPos;
//			itemDetail.transform.DOLocalMoveX (itemDetailMoveEndX, flyDuration);
//		}
//
//		private void HideItemDetail(){
//			itemDetail.transform.DOLocalMoveX (itemDetailPlaneStartPos.x, flyDuration);
//		}
//
//		private void ShowGoodsDisplayPlane(){
//			goodsDisplayPlane.transform.localPosition = goodsPlaneStartPos;
//			goodsDisplayPlane.transform.DOLocalMoveY (goodsPlaneMoveEndY, flyDuration);
//		}
//
//		private void HideGoodsDisplayPlane(){
//			goodsDisplayPlane.transform.DOLocalMoveY (goodsPlaneStartPos.y, flyDuration);
//		}
//
//		private void ShowSpecialOperationHUD(){
//			specialOperationHUD.transform.localPosition = specialOperationHUDStartPos;
//			specialOperationHUD.transform.DOLocalMoveX (specialOpeartionHUDMoveEndX, flyDuration);
//		}
//
//		private void HideSpecialOperationHUD(){
//			specialOperationHUD.transform.DOLocalMoveX (specialOperationHUDStartPos.x, flyDuration);
//		}
//
//		private void ShowBagItemsPlane(){
//			bagItemsDisplay.transform.localPosition = bagItemsPlaneStartPos;
//			bagItemsDisplay.transform.DOLocalMoveY (bagItemsPlaneMoveEndY, flyDuration);
//		}
//
//		private void HideBagItemPlane(){
//			bagItemsDisplay.transform.DOLocalMoveY (bagItemsPlaneStartPos.y, flyDuration);
//		}

		private void ResetTradeAndSpecialOperationPlane(){
			itemDetail.transform.localPosition = itemDetailPlaneStartPos;
			goodsDisplayPlane.transform.localPosition = goodsPlaneStartPos;
			specialOperationHUD.transform.localPosition = specialOperationHUDStartPos;
			bagItemsDisplay.transform.localPosition = bagItemsPlaneStartPos;
		}

		public void QuitTradePlane(){

			goodsPool.AddChildInstancesToPool(goodsContainer);

			ExploreManager.Instance.expUICtr.GetComponent<BattlePlayerUIController>().SetUpConsumablesButtons ();

			currentSelectedItem = null;

			QuitTradeDisplay ();
		}

		public void QuitSpecialOperationPlane(){
			QuitSpecialOperationDisplay ();
		}

		public void QuitNPCPlane(){

			if (npc == null) {
				return;
			}
			QuitDialogDisplay ();
			ResetTradeAndSpecialOperationPlane ();

			dialogText.text = string.Empty;

			gameObject.SetActive (false);

			npc = null;

			ExploreManager.Instance.AllWalkableEventsStartMove ();

			(ExploreManager.Instance.currentEnteredMapEvent as MapNPC).EnableAllAlertAreas ();

			ExploreManager.Instance.EnableInteractivity ();

		}

		
	}
}
