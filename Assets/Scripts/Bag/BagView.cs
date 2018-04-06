using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

	using UnityEngine.UI;
	using DG.Tweening;
	using System.Text;

	public class BagView : MonoBehaviour {

		public PropertyDisplay propertyDisplay;

//		public HLHFillBar healthBar;
//		public HLHFillBar manaBar;
//		public HLHFillBar experienceBar;
//		public Text playerLevelText;
//		public Text coinCount;
//
//		public Transform playerMaxHealth;
//		public Transform playerMaxMana;
//		public Transform playerAttack;
//		public Transform playerMagicAttack;
//		public Transform playerArmor;
//		public Transform playerMagicResist;

		public Transform[] allEquipedEquipmentButtons;

		private Player player;

		public BagItemsDisplay bagItemsDisplay;


		public TintHUD tintHUD;

		public ItemDetail itemDetail;
//		public UnlockScrollDetailHUD unlockScrollDetail;
//		public CraftingRecipesHUD craftRecipesDetail;
		public Transform choiceHUDButtonsContainer;

//		public Transform purchasePlane;
		public PurchasePendingHUD purchaseHUD;
//		public SpecialOperationHUD specialOperationHUD;

		public Transform queryRemoveHUD;

//		private int minItemIndexOfCurrentBag {
//			get {
//				return currentBagIndex * singleBagItemVolume;
//			}
//		}
//
//		private int maxItemIndexOfCurrentBag {
//			get {
//				return minItemIndexOfCurrentBag + singleBagItemVolume - 1;
//			}
//		}

//		private ShortClickCallBack mShortClickCallBack;
//		private ShortClickCallBack shortClickCallBack{
//			get{
//				if (mShortClickCallBack == null) {
//					mShortClickCallBack = 
//				}
//				return mShortClickCallBack;
//			}
//		}

//		private int offsetX = 40;
//		private int offsetY = 70;
//		private int spacingX = 22;
//		private int spacingY = 23;
//		private int bagItemWidth = 148;
//		private int bagItemHeight = 148;


		/// <summary>
		/// 初始化背包界面
		/// </summary>
		public void SetUpBagView(bool setVisible){

			if (setVisible) {

				//获取所有item的图片
				//			this.sprites = GameManager.Instance.gameDataCenter.allItemSprites;
				this.player = Player.mainPlayer;

				PropertyChange propertyChange = player.ResetBattleAgentProperties (false);

				SetUpPlayerStatusPlane (propertyChange);

				SetUpEquipedEquipmentsPlane ();

				itemDetail.ClearItemDetails ();

				ShortClickCallBack shortClickCallback = GetComponent<BagViewController> ().OnItemInBagClick;
				CallBack initPurchaseBag = delegate {
					SetUpPurchasePlane(PurchaseManager.extra_bag_id);
				};

				bagItemsDisplay.InitBagItemsDisplayPlane (shortClickCallback, initPurchaseBag);

				// 默认初始化 背包一
				SetUpBagItemsPlane (0);

				this.GetComponent<Canvas> ().enabled = setVisible;

			}


		}
			
		public void HideAllItemSelectedTintIcon(){
			bagItemsDisplay.HideAllItemSelectedTintIcon ();
		}


		/// <summary>
		/// 初始化玩家属性界面
		/// </summary>
		public void SetUpPlayerStatusPlane(PropertyChange pc){

			propertyDisplay.UpdatePropertyDisplay (pc);

			if (ExploreManager.Instance != null) {
				ExploreManager.Instance.expUICtr.UpdatePlayerStatusBar ();
			}
		}

		public void SetUpItemDetail(Item item){
			itemDetail.SetUpItemDetail (item,SetUpCurrentBagItemsPlane);
		}

		public void ClearItemDetail(){
			itemDetail.ClearItemDetails ();
		}

//		public void RefreshStatusBars(){
//
//			healthBar.maxValue = player.maxHealth;
//			healthBar.value = player.health;
//
//			manaBar.maxValue = player.maxMana;
//			manaBar.value = player.mana;
//
//			experienceBar.maxValue = player.upgradeExprience;
//			experienceBar.value = player.experience;
//
//			// 玩家经验条
//			playerLevelText.text = "Lv." + player.agentLevel.ToString();
//
//			if (ExploreManager.Instance != null) {
//				ExploreManager.Instance.expUICtr.UpdatePlayerStatusBar ();
//			}
//
//		}


			



		/// <summary>
		/// 初始化已装备物品界面
		/// </summary>
		public void SetUpEquipedEquipmentsPlane(){

			for(int i = 0;i<player.allEquipedEquipments.Length;i++){

				Transform equipedEquipmentButton = allEquipedEquipmentButtons[i];

				Equipment equipment = player.allEquipedEquipments [i];

//				Debug.Log (equipment == null);

				bool equipmentSlotUnlocked = true;

				if (i == 6) {
					equipmentSlotUnlocked = BuyRecord.Instance.extraEquipmentSlotUnlocked;
				}

				equipedEquipmentButton.GetComponent<EquipedEquipmentCell> ().SetUpEquipedEquipmentCell (equipment, equipmentSlotUnlocked);

				equipedEquipmentButton.GetComponent<ItemDragControl> ().item = equipment;


			}

		}
			


		public void SetUpCurrentBagItemsPlane(){
			bagItemsDisplay.SetUpCurrentBagItemsPlane ();
		}
			
		/// <summary>
		/// 初始化物品详细介绍页面
		/// </summary>
		/// <param name="item">Item.</param>
//		public void SetUpItemDetailHUD(Item item){
//			itemDetail.SetUpItemDetailHUD (item);
//			SetUpOperationButtons (item);
//
//		}

		/// <summary>
		/// 解锁卷轴展示界面点击事件原本就会退出展示页面，一般情况下不用主动调用这个方法
		/// </summary>
//		public void QuitUnlockScrollHUD(){
//			unlockScrollDetail.QuitUnlockScrollDetailHUD ();
//		}

		private void SetUpOperationButtons(Item item){

			// 如果物品是装备
			switch (item.itemType) {

			case ItemType.Equipment:

				Equipment equipment = item as Equipment;

				if (equipment.equiped) {
					SetUpOperationButtonsActive (false, true, false, false);
				} else {
					SetUpOperationButtonsActive (true, false, false, false);
				}

				break;
			case ItemType.Consumables:
				Consumables csm = item as Consumables;
				switch(csm.type){
				case ConsumablesType.ShuXingTiSheng:
				case ConsumablesType.YinShenJuanZhou:
					SetUpOperationButtonsActive (false, false, true, false);
					break;
				case ConsumablesType.ChongZhuShi:
				case ConsumablesType.DianJinShi:
				case ConsumablesType.XiaoMoJuanZhou:
					SetUpOperationButtonsActive (false, false, false, true);
					break;
				}
				break;
			}

		}


		private void SetUpOperationButtonsActive(bool equipButton,bool unloadButton,bool useButton,bool confirmButton){

			choiceHUDButtonsContainer.Find ("EquipButton").gameObject.SetActive (equipButton);
			choiceHUDButtonsContainer.Find ("UnloadButton").gameObject.SetActive (unloadButton);
			choiceHUDButtonsContainer.Find ("UseButton").gameObject.SetActive (useButton);

		}

		public Equipment RebuildEquipment(){
			Equipment eqp = itemDetail.SpecialOperationOnEquipment (SpecialOperation.ChongZhu, 0);
			if (eqp == null) {
				return null;
			}
			SetUpCurrentBagItemsPlane ();
			itemDetail.SetUpItemDetail (eqp);
			return eqp;
		}

		public Equipment UpgradeEquipmentToGold(){
			Equipment eqp = itemDetail.SpecialOperationOnEquipment (SpecialOperation.DianJin, 0);
			if (eqp == null) {
				return null;
			}
			SetUpCurrentBagItemsPlane ();
			itemDetail.SetUpItemDetail (eqp);
			return eqp;
		}

		public Equipment RemoveEquipmentAttachedSkill(){
			Equipment eqp = itemDetail.SpecialOperationOnEquipment (SpecialOperation.XiaoMo, 0);
			if (eqp == null) {
				return null;
			}
			SetUpCurrentBagItemsPlane ();
			itemDetail.SetUpItemDetail (eqp);
			return eqp;
		}

		/// <summary>
		/// 初始化背包物品界面
		/// </summary>
		public void SetUpBagItemsPlane(int bagIndex){

			bagItemsDisplay.SetUpBagItemsPlane (bagIndex);
		}



		public void RemoveBagItemAt(int itemIndexInBag){
			bagItemsDisplay.RemoveBagItemAt (itemIndexInBag);
		}
			
		public void RemoveBagItem(Item item){

			bagItemsDisplay.RemoveBagItem (item);
		}

		private void AddSequenceItemsIfBagNotFull(){

			bagItemsDisplay.AddSequenceItemsIfBagNotFull ();

		}

		/// <summary>
		/// 背包中单个物品按钮的初始化方法,序号-1代表添加到背包尾部
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="btn">Button.</param>
		public void AddBagItem(Item item,int atIndex = -1,bool forceAdd = false){

			bagItemsDisplay.AddBagItem (item, atIndex, forceAdd);
		}


		public void SetUpResolveGainHUD(List<char> characters){

			StringBuilder tint = new StringBuilder();

			tint.Append ("获得字母碎片<color=orange>");

			for (int i = 0; i < characters.Count; i++) {

				tint.Append (" " + characters [i]);

			}

			tint.Append ("</color>");

			tintHUD.SetUpTintHUD (tint.ToString(),null);

		}
			

		public void ShowRemoveQueryHUD(){
			queryRemoveHUD.gameObject.SetActive (true);
		}

		public void HideRemoveQueryHUD(){
			queryRemoveHUD.gameObject.SetActive (false);
		}



		public void SetUpTintHUD(string tint,Sprite sprite){
			tintHUD.SetUpTintHUD (tint,sprite);
		}
			
		public void SetUpPurchasePlane(string productID){

			purchaseHUD.SetUpPurchasePendingHUD (productID,delegate {
				SetUpEquipedEquipmentsPlane();
				bagItemsDisplay.UpdateBagTabs();
			});

		}
			

		// 关闭背包界面
		public void QuitBagPlane(){

			tintHUD.QuitTintHUD ();

			bagItemsDisplay.QuitBagItemPlane ();

			itemDetail.soCell.ResetSpecialOperationCell ();

			propertyDisplay.ClearPropertyDisplay ();

			GetComponent<Canvas> ().enabled = false;
		}


	}
}
