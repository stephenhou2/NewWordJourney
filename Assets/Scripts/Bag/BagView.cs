using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;


	public class BagView : MonoBehaviour
	{

        // 属性显示面板
		public PropertyDisplay propertyDisplay;

        // 所有装备栏按钮数组
		public Transform[] allEquipedEquipmentButtons;

        // 玩家对象
		private Player player;

        // 背包显示面板
		public BagItemsDisplay bagItemsDisplay;

        // 信息提示HUD
		public TintHUD hintHUD;

        // 物品详细信息面板
		public ItemDetail itemDetail;

        // 物品操作选择容器，内部存放装备按钮，卸载按钮，使用按钮等
		public Transform choiceHUDButtonsContainer;

        // 购买/广告页面
		public PurchasePendingHUD purchaseHUD;

        // 金币购买页面
		public BuyGoldView buyGoldView;

        // 询问是否丢弃物品的界面
		public Transform queryRemoveHUD;

        // 技能显示面板
		public SkillsView skillsView;

		// 0代表背包 1代表技能列表
		private int panelIndex;

        // 背包切换按钮上的文字显示
		public Text bagButtonTitle;
        // 技能切换按钮上的文字显示
		public Text skillButtonTitle;

        // 玩家等级文字显示
		public Text playerLevelText;



		/// <summary>
		/// 初始化背包界面
		/// </summary>
		public void SetUpBagView(bool setVisible)
		{
            // 默认显示背包界面
			panelIndex = 0;

			// 如果初始化后不显示背包界面
			if (!setVisible)
			{
				this.GetComponent<Canvas>().enabled = false;
				return;
			}
            
			this.player = Player.mainPlayer;

            // 创建一个空的属性变化，这样开背包时就不会显示属性变化了
			PropertyChange propertyChange = new PropertyChange();

            // 以空的属性变化（属性变化都是0）来初始化玩家状态面板
			SetUpPlayerStatusPlane(propertyChange);

            // 初始化装备面板
			SetUpEquipedEquipmentsPlane();

            // 获取物品点击响应回调
			CallBackWithItem shortClickCallback = GetComponent<BagViewController>().OnItemInBagClick;

            // 根据平台初始化背包面板
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
            // 初始化技能面板
            skillsView.InitSkillsView(SetUpPlayerStatusPlane);

            // 默认初始化 背包一
            SetUpBagItemsPlane(0);

            // reset物品详细信息面板
            itemDetail.ClearItemDetails();

            // 默认显示的是背包界面，将背包tab button的字体颜色改为选中的颜色
            bagButtonTitle.color = CommonData.tabBarTitleSelectedColor;
			skillButtonTitle.color = CommonData.tabBarTitleNormalColor;

            // 初始化技能面板
			skillsView.InitSkillsView(SetUpPlayerStatusPlane);
			// 但是不显示技能面板
			skillsView.QuitSkillsView();

			// 默认初始化 背包一
			SetUpBagItemsPlane(0);
     
            // reset物品详细信息面板
            itemDetail.ClearItemDetails();

            // 默认显示的是背包界面，将背包tab button的字体颜色改为选中的颜色
			bagButtonTitle.color = CommonData.tabBarTitleSelectedColor;
			skillButtonTitle.color = CommonData.tabBarTitleNormalColor;

			// 显示背包画布	
			this.GetComponent<Canvas>().enabled = true;
           

		}
        
        /// <summary>
        /// 点击购买背包的回调
        /// </summary>
        /// <param name="bagIndex">购买的背包序号.</param>
		private void PurchaseBagCallBack(int bagIndex)
		{

            // 根据欲购买的背包序号初始化购买/广告界面

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
        
        /// <summary>
        /// 安卓平台点击获取金币按钮的回调
        /// </summary>
		private void EnterGoldWatchAdOnAndroid()
		{

			SetUpPurchasePlaneOnAndroid(PurchaseManager.gold_100_id);

		}

        /// <summary>
        /// ios平台上显示购买界面
        /// </summary>
        /// <param name="productID">正在购买的产品id.</param>
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

        /// <summary>
        /// android平台上显示广告界面
        /// </summary>
        /// <param name="productID">Product identifier.</param>
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

                    // 根据想要获得的产品，传入不同的回调
                    // 获得金币的广告，观看之后获得100金币，并保存数据，更新人物状态栏
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
					}
                    // 获得装备槽，背包的广告，观看后更新背包和装备栏的显示
					else{
						purchaseHUD.SetUpPurchasePendingHUDOnAndroid(productID, adType, rewardType, delegate
                        {
                            SetUpEquipedEquipmentsPlane();
                            bagItemsDisplay.UpdateBagTabs();
                        }, null);
					}

					break;
			}
		}

        /// <summary>
        /// 隐藏所有背包物品的选中图标
        /// </summary>
		public void HideAllItemSelectedTintIcon()
		{
			bagItemsDisplay.HideAllItemSelectedTintIcon();
		}

        /// <summary>
        /// 底部技能tab button的点击响应
        /// </summary>
		public void OnSkillsViewButtonClick()
		{
            // 如果技能面板已经显示，则直接返回
			if (panelIndex == 1)
			{
				return;
			}

            // 更新当前显示的面板序号为1
			panelIndex = 1;

            // 初始化技能面板
			skillsView.SetUpSkillView();

            // 更新底部tab button的文字颜色
			skillButtonTitle.color = CommonData.tabBarTitleSelectedColor;
			bagButtonTitle.color = CommonData.tabBarTitleNormalColor;

		}

        /// <summary>
        /// 底部背包tab button的点击响应
        /// </summary>
		public void OnBagButtonClick()
		{
            // 如果当前显示的是背包面板，则直接返回
			if (panelIndex == 0)
			{
				return;
			}

            // 更新当前显示的面板序号为0
			panelIndex = 0;

            // 隐藏技能面板
			skillsView.QuitSkillsView();

            // 更新底部tabbutton上的文字颜色
			bagButtonTitle.color = CommonData.tabBarTitleSelectedColor;
			skillButtonTitle.color = CommonData.tabBarTitleNormalColor;


		}

		/// <summary>
		/// 初始化玩家属性界面
		/// </summary>
		public void SetUpPlayerStatusPlane(PropertyChange pc)
		{
            // 根据传入的属性变化，更新属性显示面板
			propertyDisplay.UpdatePropertyDisplay(pc);

            // 更新玩家状态面板
			if (ExploreManager.Instance != null)
			{
				ExploreManager.Instance.expUICtr.UpdatePlayerStatusBar();
			}

			playerLevelText.text = string.Format("等级: {0}", player.agentLevel);
		}

        /// <summary>
        /// 显示物品详细信息
        /// </summary>
        /// <param name="item">Item.</param>
		public void SetUpItemDetail(Item item)
		{
			itemDetail.SetUpItemDetail(item);
		}

        /// <summary>
        /// 清除物品详细信息
        /// </summary>
		public void ClearItemDetail()
		{
			itemDetail.ClearItemDetails();
		}


		/// <summary>
		/// 初始化已装备物品面板
		/// </summary>
		public void SetUpEquipedEquipmentsPlane()
		{

			for (int i = 0; i < player.allEquipedEquipments.Length; i++)
			{

				Transform equipedEquipmentButton = allEquipedEquipmentButtons[i];

				Equipment equipment = player.allEquipedEquipments[i];

				bool equipmentSlotUnlocked = true;

                // 如果是第6个装备槽，首先检查是否已经购买解锁
				if (i == 6)
				{
					equipmentSlotUnlocked = BuyRecord.Instance.extraEquipmentSlotUnlocked;
				}

                // 根据装备和解锁情况显示装备槽
				equipedEquipmentButton.GetComponent<EquipedEquipmentCell>().SetUpEquipedEquipmentCell(equipment, equipmentSlotUnlocked);

                //初始化装备槽上的拖拽组件            
				EquipedItemDragControl equipedItemDragControl = equipedEquipmentButton.GetComponent<EquipedItemDragControl>();

				equipedItemDragControl.SetUpEquipedItemDragControl(equipment);

			}

		}

        /// <summary>
        /// 隐藏所有的装备槽的选中图标
        /// </summary>
		public void HideAllEquipedEquipmentsSelectIcon()
		{
			for (int i = 0; i < allEquipedEquipmentButtons.Length; i++)
			{
				Transform equipedEquipmentButton = allEquipedEquipmentButtons[i];
				equipedEquipmentButton.Find("SelectedIcon").GetComponent<Image>().enabled = false;

			}
		}
        
        /// <summary>
        /// 更新当前背包的显示面板
        /// </summary>
		public void UpdateCurrentBagItemsPlane()
		{
			bagItemsDisplay.SetUpCurrentBagItemsPlane();
		}

//        /// <summary>
//        /// 装备栏中的装备的点击响应方法
//        /// </summary>
//        /// <param name="item">Item.</param>
//        /// <param name="equipmentIndexInPanel">Equipment index in panel.</param>
//		public void OnItemInEquipmentPlaneClick(Item item, int equipmentIndexInPanel)
//		{
//            // 如果点击的是装备槽6，并且还没有解锁，则显示购买界面
//			if (equipmentIndexInPanel == 6 && !BuyRecord.Instance.extraEquipmentSlotUnlocked)
//			{
//#if UNITY_IOS
//				SetUpPurchasePlaneOnIPhone(PurchaseManager.extra_equipmentSlot_id);
//#elif UNITY_ANDROID
//				SetUpPurchasePlaneOnAndroid(PurchaseManager.extra_equipmentSlot_id);
//#elif UNITY_EDITOR
//				UnityEditor.BuildTarget buildTarget = UnityEditor.EditorUserBuildSettings.activeBuildTarget;

//                switch (buildTarget) {
//                    case UnityEditor.BuildTarget.Android:
//    				    SetUpPurchasePlaneOnAndroid(PurchaseManager.extra_equipmentSlot_id);
//                        break;
//                    case UnityEditor.BuildTarget.iOS:
//    				    SetUpPurchasePlaneOnIPhone(PurchaseManager.extra_equipmentSlot_id);
//                        break;
//                }
//#endif
			//}

   //         // 装备槽中没有装备，则直接返回
   //         // 默认装备槽中都放了一个空装备占位，空装备的id为-1
   //         if (item == null || item.itemId < 0)
   //         {
   //             return;
   //         }

   //         // 更新当前选中物品
   //         GetComponent<BagViewController>().currentSelectItem = item;

   //         // 显示物品详细信息
   //         SetUpItemDetail(item);

			//// 隐藏所有装备和物品的选中图标
        //    HideAllSelectedIcon();

        //}

		public void HideAllSelectedIcon()
		{
			
            HideAllEquipedEquipmentsSelectIcon();
            HideAllItemSelectedTintIcon();
		}


        /// <summary>
        /// 设置不同按钮的显示状态
        /// </summary>
        /// <param name="equipButton">If set to <c>true</c> equip button.</param>
        /// <param name="unloadButton">If set to <c>true</c> unload button.</param>
        /// <param name="useButton">If set to <c>true</c> use button.</param>
        /// <param name="confirmButton">If set to <c>true</c> confirm button.</param>
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


        /// <summary>
        /// 移除背包中指定序号的物品
        /// </summary>
        /// <param name="itemIndexInBag">Item index in bag.</param>
		public void RemoveBagItemAt(int itemIndexInBag){
			bagItemsDisplay.RemoveBagItemAt (itemIndexInBag);
		}

        /// <summary>
        /// 从背包中移除指定物品
        /// </summary>
        /// <param name="item">Item.</param>
		public void RemoveBagItem(Item item){

			bagItemsDisplay.RemoveBagItem (item);
		}



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


        /// <summary>
        /// 显示移除提示框
        /// </summary>
		public void ShowRemoveQueryHUD(){
			queryRemoveHUD.gameObject.SetActive (true);
		}

        /// <summary>
        /// 隐藏移除提示框
        /// </summary>
		public void HideRemoveQueryHUD(){
			queryRemoveHUD.gameObject.SetActive (false);
		}


        /// <summary>
        /// 显示文字提示
        /// </summary>
        /// <param name="tint">Tint.</param>
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
