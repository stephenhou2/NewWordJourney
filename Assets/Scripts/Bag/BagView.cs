using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;
	using DG.Tweening;
	using System.Text;


	public class BagView : MonoBehaviour
	{

		public PropertyDisplay propertyDisplay;

		public Transform[] allEquipedEquipmentButtons;

		private Player player;

		public BagItemsDisplay bagItemsDisplay;


		public TintHUD hintHUD;

		public ItemDetail itemDetail;

		public Transform choiceHUDButtonsContainer;


		public PurchasePendingHUD purchaseHUD;

		public BuyGoldView buyGoldView;

		public Transform queryRemoveHUD;

		public SkillsView skillsView;

		// 0代表背包 1代表技能列表
		private int panelIndex;

		public Text bagButtonTitle;

		public Text skillButtonTitle;

		public Text playerLevelText;

		public Sprite grayFrame;
		public Sprite blueFrame;
		public Sprite goldFrame;
		public Sprite purpleFrame;

		/// <summary>
		/// 初始化背包界面
		/// </summary>
		public void SetUpBagView(bool setVisible)
		{

			panelIndex = 0;

			if (setVisible)
			{

				//获取所有item的图片
				//			this.sprites = GameManager.Instance.gameDataCenter.allItemSprites;
				this.player = Player.mainPlayer;

				PropertyChange propertyChange = new PropertyChange();

				SetUpPlayerStatusPlane(propertyChange);

				SetUpEquipedEquipmentsPlane();

				itemDetail.ClearItemDetails();

				CallBackWithItem shortClickCallback = GetComponent<BagViewController>().OnItemInBagClick;
#if UNITY_IOS
				bagItemsDisplay.InitBagItemsDisplayPlane(shortClickCallback, PurchaseBagCallBack, buyGoldView.SetUpBuyGoldView);
#elif UNITY_ANDROID
				bagItemsDisplay.InitBagItemsDisplayPlane(shortClickCallback, PurchaseBagCallBack, EnterGoldWatchAdOnAndroid);
#elif UNITY_EDITOR
				UnityEditor.BuildTarget buildTarget = UnityEditor.EditorUserBuildSettings.activeBuildTarget;

                switch (buildTarget) {
				case UnityEditor.BuildTarget.Android:
    				bagItemsDisplay.InitBagItemsDisplayPlane(shortClickCallback, PurchaseBagCallBack, EnterGoldWatchAdOnAndroid);
                    break;
				case UnityEditor.BuildTarget.iOS:
    				bagItemsDisplay.InitBagItemsDisplayPlane(shortClickCallback, PurchaseBagCallBack, buyGoldView.SetUpBuyGoldView);
                    break;
                }
#endif

				skillsView.InitSkillsView(SetUpPlayerStatusPlane);

				// 默认初始化 背包一
				SetUpBagItemsPlane(0);

				bagButtonTitle.color = CommonData.tabBarTitleSelectedColor;
				skillButtonTitle.color = CommonData.tabBarTitleNormalColor;

				this.GetComponent<Canvas>().enabled = setVisible;

			}

			skillsView.QuitSkillsView();

			itemDetail.ClearItemDetails();

		}

		private void PurchaseBagCallBack(int bagIndex)
		{

#if UNITY_IOS
			switch(bagIndex){
				case 1:
			        SetUpPurchasePlaneOnIPhone(PurchaseManager.extra_bag_2_id);
					break;
				case 2:
			        SetUpPurchasePlaneOnIPhone(PurchaseManager.extra_bag_3_id);
					break;
			}
#elif UNITY_ANDROID
			switch (bagIndex)
			{
				case 1:
					SetUpPurchasePlaneOnAndroid(PurchaseManager.extra_bag_2_id);
					break;
				case 2:
					SetUpPurchasePlaneOnAndroid(PurchaseManager.extra_bag_3_id);
					break;
			}
            
#elif UNITY_EDITOR
			UnityEditor.BuildTarget buildTarget = UnityEditor.EditorUserBuildSettings.activeBuildTarget;

            switch (buildTarget) {
			    case UnityEditor.BuildTarget.Android:
        			switch (bagIndex)
                    {
                        case 1:
                            SetUpPurchasePlaneOnAndroid(PurchaseManager.extra_bag_2_id);
                            break;
                        case 2:
                            SetUpPurchasePlaneOnAndroid(PurchaseManager.extra_bag_3_id);
                            break;
                    }
                    break;
			    case UnityEditor.BuildTarget.iOS:
        			switch(bagIndex){
                        case 1:
                            SetUpPurchasePlaneOnIPhone(PurchaseManager.extra_bag_2_id);
                            break;
                        case 2:
                            SetUpPurchasePlaneOnIPhone(PurchaseManager.extra_bag_3_id);
                            break;
                    }
                    break;
            }
#endif

		}


		private void EnterGoldWatchAdOnAndroid()
		{

			SetUpPurchasePlaneOnAndroid(PurchaseManager.gold_100_id);

		}

		public void SetUpPurchasePlaneOnIPhone(string productID)
		{

			switch (Application.internetReachability)
			{
				case NetworkReachability.NotReachable:
					hintHUD.SetUpSingleTextTintHUD("无网络连接");
					break;
				case NetworkReachability.ReachableViaCarrierDataNetwork:
				case NetworkReachability.ReachableViaLocalAreaNetwork:
					purchaseHUD.SetUpPurchasePendingHUDOnIPhone(productID, delegate
					{
						SetUpEquipedEquipmentsPlane();
						bagItemsDisplay.UpdateBagTabs();
					});
					break;
			}
		}

		public void SetUpPurchasePlaneOnAndroid(string productID)
		{

			AdRewardType rewardType = AdRewardType.BagSlot_2;

			MyAdType adType = MyAdType.CPAd;

			if (productID.Equals(PurchaseManager.extra_bag_2_id))
			{
				rewardType = AdRewardType.BagSlot_2;
				adType = MyAdType.RewardedVideoAd;
			}
			else if (productID.Equals(PurchaseManager.extra_bag_3_id))
			{
				rewardType = AdRewardType.BagSlot_3;
				adType = MyAdType.RewardedVideoAd;
			}
			else if (productID.Equals(PurchaseManager.extra_equipmentSlot_id))
			{
				rewardType = AdRewardType.EquipmentSlot;
				adType = MyAdType.RewardedVideoAd;
			}
			else if (productID.Equals(PurchaseManager.gold_100_id))
			{
				rewardType = AdRewardType.Gold;
				adType = MyAdType.RewardedVideoAd;
			}

			switch (Application.internetReachability)
			{
				case NetworkReachability.NotReachable:
					hintHUD.SetUpSingleTextTintHUD("无网络连接");
					break;
				case NetworkReachability.ReachableViaCarrierDataNetwork:
				case NetworkReachability.ReachableViaLocalAreaNetwork:
					if(productID.Equals(PurchaseManager.gold_100_id)){
						purchaseHUD.SetUpPurchasePendingHUDOnAndroid(productID, adType, rewardType, delegate
						{                     
							Player.mainPlayer.totalGold += 100;
							BuyRecord.Instance.RecordLastGoldAdTime();
                            GameManager.Instance.persistDataManager.UpdateBuyGoldToPlayerDataFile();
				            if(ExploreManager.Instance != null)
							{
								ExploreManager.Instance.expUICtr.UpdatePlayerGold();
							}
						}, null);
					}else{
						purchaseHUD.SetUpPurchasePendingHUDOnAndroid(productID, adType, rewardType, delegate
                        {
                            SetUpEquipedEquipmentsPlane();
                            bagItemsDisplay.UpdateBagTabs();
                        }, null);
					}

					break;
			}
		}

		public void HideAllItemSelectedTintIcon()
		{
			bagItemsDisplay.HideAllItemSelectedTintIcon();
		}


		public void OnSkillsViewButtonClick()
		{

			if (panelIndex == 1)
			{
				return;
			}

			panelIndex = 1;

			skillsView.SetUpSkillView();

			skillButtonTitle.color = CommonData.tabBarTitleSelectedColor;
			bagButtonTitle.color = CommonData.tabBarTitleNormalColor;

		}

		public void OnBagButtonClick()
		{

			if (panelIndex == 0)
			{
				return;
			}


			panelIndex = 0;


			skillsView.QuitSkillsView();

			bagButtonTitle.color = CommonData.tabBarTitleSelectedColor;
			skillButtonTitle.color = CommonData.tabBarTitleNormalColor;


		}

		/// <summary>
		/// 初始化玩家属性界面
		/// </summary>
		public void SetUpPlayerStatusPlane(PropertyChange pc)
		{

			propertyDisplay.UpdatePropertyDisplay(pc);

			if (ExploreManager.Instance != null)
			{
				ExploreManager.Instance.expUICtr.UpdatePlayerStatusBar();
			}

			playerLevelText.text = string.Format("等级: {0}", player.agentLevel);
		}

		public void SetUpItemDetail(Item item)
		{
			itemDetail.SetUpItemDetail(item);
		}

		public void ClearItemDetail()
		{
			itemDetail.ClearItemDetails();
		}


		/// <summary>
		/// 初始化已装备物品界面
		/// </summary>
		public void SetUpEquipedEquipmentsPlane()
		{

			for (int i = 0; i < player.allEquipedEquipments.Length; i++)
			{

				Transform equipedEquipmentButton = allEquipedEquipmentButtons[i];

				Equipment equipment = player.allEquipedEquipments[i];

				bool equipmentSlotUnlocked = true;

				if (i == 6)
				{
					equipmentSlotUnlocked = BuyRecord.Instance.extraEquipmentSlotUnlocked;
				}

				equipedEquipmentButton.GetComponent<EquipedEquipmentCell>().SetUpEquipedEquipmentCell(equipment, equipmentSlotUnlocked);

				EquipedItemDragControl equipedItemDragControl = equipedEquipmentButton.GetComponent<EquipedItemDragControl>();

				if (equipment.itemId < 0)
				{
					equipedItemDragControl.Reset();
					equipedItemDragControl.backgroundImage.sprite = grayFrame;
					continue;
				}

				equipedItemDragControl.item = equipment;

				switch (equipment.quality)
				{
					case EquipmentQuality.Gray:
						equipedItemDragControl.backgroundImage.sprite = grayFrame;
						break;
					case EquipmentQuality.Blue:
						equipedItemDragControl.backgroundImage.sprite = blueFrame;
						break;
					case EquipmentQuality.Gold:
						equipedItemDragControl.backgroundImage.sprite = goldFrame;
						break;
					case EquipmentQuality.Purple:
						equipedItemDragControl.backgroundImage.sprite = purpleFrame;
						break;
				}

			}

		}


		public void HideAllEquipedEquipmentsSelectIcon()
		{
			for (int i = 0; i < allEquipedEquipmentButtons.Length; i++)
			{
				Transform equipedEquipmentButton = allEquipedEquipmentButtons[i];
				equipedEquipmentButton.Find("SelectedIcon").GetComponent<Image>().enabled = false;

			}
		}


		public void SetUpCurrentBagItemsPlane()
		{
			bagItemsDisplay.SetUpCurrentBagItemsPlane();
		}


		public void OnItemInEquipmentPlaneClick(Item item, int equipmentIndexInPanel)
		{

			if (equipmentIndexInPanel == 6 && !BuyRecord.Instance.extraEquipmentSlotUnlocked)
			{
#if UNITY_IOS
				SetUpPurchasePlaneOnIPhone(PurchaseManager.extra_equipmentSlot_id);
#elif UNITY_ANDROID
				SetUpPurchasePlaneOnAndroid(PurchaseManager.extra_equipmentSlot_id);
#elif UNITY_EDITOR
				UnityEditor.BuildTarget buildTarget = UnityEditor.EditorUserBuildSettings.activeBuildTarget;

                switch (buildTarget) {
                    case UnityEditor.BuildTarget.Android:
    				    SetUpPurchasePlaneOnAndroid(PurchaseManager.extra_equipmentSlot_id);
                        break;
                    case UnityEditor.BuildTarget.iOS:
    				    SetUpPurchasePlaneOnIPhone(PurchaseManager.extra_equipmentSlot_id);
                        break;
                }
#endif
			}

            if (item == null || item.itemId < 0)
            {
                return;
            }

            GetComponent<BagViewController>().currentSelectItem = item;

            SetUpItemDetail(item);

            HideAllEquipedEquipmentsSelectIcon();
            HideAllItemSelectedTintIcon();

        }


		private void SetUpOperationButtonsActive(bool equipButton,bool unloadButton,bool useButton,bool confirmButton){

			choiceHUDButtonsContainer.Find ("EquipButton").gameObject.SetActive (equipButton);
			choiceHUDButtonsContainer.Find ("UnloadButton").gameObject.SetActive (unloadButton);
			choiceHUDButtonsContainer.Find ("UseButton").gameObject.SetActive (useButton);

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

		//private void AddSequenceItemsIfBagNotFull(){

		//	bagItemsDisplay.AddSequenceItemsIfBagNotFull ();

		//}

		/// <summary>
		/// 背包中单个物品按钮的初始化方法,序号-1代表添加到背包尾部
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="btn">Button.</param>
		public void AddBagItem(Item item,int atIndex = -1,bool forceAdd = false){
            if(bagItemsDisplay.currentBagIndex == 4){
                bagItemsDisplay.SetUpEquipedEquipments();
            }else{
                bagItemsDisplay.AddBagItem(item, atIndex, forceAdd);
            }
			
		}



		public void ShowRemoveQueryHUD(){
			queryRemoveHUD.gameObject.SetActive (true);
		}

		public void HideRemoveQueryHUD(){
			queryRemoveHUD.gameObject.SetActive (false);
		}



		public void SetUpSingleTextTintHUD(string tint){
			hintHUD.SetUpSingleTextTintHUD (tint);
		}
			
      
		// 关闭背包界面
		public void QuitBagPlane(){

			hintHUD.QuitTintHUD ();

			bagItemsDisplay.QuitBagItemPlane ();

			itemDetail.soCell.ResetSpecialOperationCell ();

			propertyDisplay.ClearPropertyDisplay ();

			GetComponent<Canvas> ().enabled = false;
		}


	}
}
