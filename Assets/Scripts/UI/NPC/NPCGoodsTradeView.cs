using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;
	using DG.Tweening;

	public class NPCGoodsTradeView : MonoBehaviour
    {

		/**********  tradePlane UI *************/
        public Transform goodsDisplayPlane;
        public Transform goodsContainer;
        public InstancePool goodsPool;
        public Transform goodsModel;
        /**********  tradePlane UI *************/

		//NPCTradeView
        
		public BagItemsDisplay bagItemsDisplay;

        public ItemDetailWithNPC itemDetail;

		public PurchasePendingHUD purchasePendingHUD;

		public BuyGoldView buyGoldView;

		public TintHUD hintHUD;
           

		public float flyDuration;

        public Vector3 goodsPlaneStartPos;

        public Vector3 itemDetailPlaneStartPos;

        public Vector3 bagItemsPlaneStartPos;

        public int goodsPlaneMoveEndX;

        public int itemDetailMoveEndX;

        public int bagItemsPlaneMoveEndY;

		private Item currentSelectedItem;

		private int currentSelectItemPrice;

		private HLHNPC npc;

		private CallBack quitCallBack;

		private int currentSelectGoodsIndex;

		public Transform quitTradeButton;
		public Transform quitAddGemstoneButton;

		public void SetUpNPCTradeView(HLHNPC npc,CallBack quitCallBack)
		{
			this.npc = npc;

			this.quitCallBack = quitCallBack;

			GameManager.Instance.soundManager.PlayAudioClip(CommonData.merchantAudioName);

			itemDetail.ClearItemDetails();

            UpdateGoodsDisplay();
            
#if UNITY_IOS
			bagItemsDisplay.InitBagItemsDisplayPlane(OnItemInBagClickInTrade, PurchaseBagCallBack, buyGoldView.SetUpBuyGoldView);
#elif UNITY_ANDROID
			bagItemsDisplay.InitBagItemsDisplayPlane(OnItemInBagClickInTrade, PurchaseBagCallBack, EnterGoldWatchAdOnAndroid);
#elif UNITY_EDITOR
            UnityEditor.BuildTarget buildTarget = UnityEditor.EditorUserBuildSettings.activeBuildTarget;

            switch (buildTarget) {
            case UnityEditor.BuildTarget.Android:
			    bagItemsDisplay.InitBagItemsDisplayPlane(OnItemInBagClickInTrade, PurchaseBagCallBack, EnterGoldWatchAdOnAndroid);
                break;
            case UnityEditor.BuildTarget.iOS:
			    bagItemsDisplay.InitBagItemsDisplayPlane(OnItemInBagClickInTrade, PurchaseBagCallBack, buyGoldView.SetUpBuyGoldView);
                break;
            }
#endif

			bagItemsDisplay.SetUpBagItemsPlane(0);
            
            EnterTradeDisplay();

			quitTradeButton.gameObject.SetActive(true);
			quitAddGemstoneButton.gameObject.SetActive(false);
		}

		private void UpdateGoodsDisplay()
        {

            goodsPool.AddChildInstancesToPool(goodsContainer);

            List<HLHNPCGoods> itemsAsGoods = npc.GetCurrentLevelGoods();

            for (int i = 0; i < itemsAsGoods.Count; i++)
            {

                HLHNPCGoods goods = itemsAsGoods[i];

                Transform goodsCell = goodsPool.GetInstance<Transform>(goodsModel.gameObject, goodsContainer);

                Item itemAsGoods = goods.GetGoodsItem();

				goodsCell.GetComponent<GoodsCell>().SetUpGoodsCell(goods);

                goodsCell.GetComponent<Button>().onClick.RemoveAllListeners();

                int goodsIndex = i;

                goodsCell.GetComponent<Button>().onClick.AddListener(delegate {
                    currentSelectedItem = itemAsGoods;
					currentSelectItemPrice = goods.GetGoodsPrice();
					itemDetail.SetUpItemDetail(itemAsGoods, currentSelectItemPrice, ItemOperationType.Buy);
                    UpdateGoodsSelection(goodsIndex);
					currentSelectGoodsIndex = goodsIndex;
					bagItemsDisplay.HideAllItemSelectedTintIcon();
                });

            }

        }

		private void OnItemInBagClickInTrade(Item item)
        {

			currentSelectedItem = item;

			if(item.itemType == ItemType.Equipment){
				Equipment equipment = item as Equipment;
				currentSelectItemPrice = equipment.price;
                // 装备的价格需要计算上镶嵌在装备上的宝石价格
				for (int i = 0; i < equipment.attachedPropertyGemstones.Count;i++){
					currentSelectItemPrice += equipment.attachedPropertyGemstones[i].price;
				}
			}else{
				currentSelectItemPrice = item.price;
			}
         
			itemDetail.SetUpItemDetail(item, currentSelectItemPrice, ItemOperationType.Sell);
         
            bagItemsDisplay.HideAllItemSelectedTintIcon();

			for (int i = 0; i < goodsContainer.childCount;i++){
				GoodsCell goodsCell = goodsContainer.GetChild(i).GetComponent<GoodsCell>();
				goodsCell.SetSelection(false);
			}

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

		public void SetUpPurchasePlaneOnIPhone(string productID)
        {

            switch (Application.internetReachability)
            {
                case NetworkReachability.NotReachable:
                    hintHUD.SetUpSingleTextTintHUD("无网络连接");
                    break;
                case NetworkReachability.ReachableViaCarrierDataNetwork:
                case NetworkReachability.ReachableViaLocalAreaNetwork:
                    purchasePendingHUD.SetUpPurchasePendingHUDOnIPhone(productID, delegate
                    {
                        bagItemsDisplay.UpdateBagTabs();
                    });
                    break;
            }
        }

		private void EnterGoldWatchAdOnAndroid()
        {

            SetUpPurchasePlaneOnAndroid(PurchaseManager.gold_100_id);

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
                    if (productID.Equals(PurchaseManager.gold_100_id))
                    {
                        purchasePendingHUD.SetUpPurchasePendingHUDOnAndroid(productID, adType, rewardType, delegate
                        {
							BuyRecord.Instance.RecordLastGoldAdTime();
                            Player.mainPlayer.totalGold += 100;
                            GameManager.Instance.persistDataManager.UpdateBuyGoldToPlayerDataFile();
							if (ExploreManager.Instance != null)
                            {
                                ExploreManager.Instance.expUICtr.UpdatePlayerGold();
                            }
                        }, null);
                    }
                    else
                    {
                        purchasePendingHUD.SetUpPurchasePendingHUDOnAndroid(productID, adType, rewardType, delegate
                        {
                            bagItemsDisplay.UpdateBagTabs();
                        }, null);
                    }

                    break;
            }
        }

            
		private void EnterTradeDisplay()
        {
            itemDetail.transform.localPosition = itemDetailPlaneStartPos;
            itemDetail.transform.DOLocalMoveX(itemDetailMoveEndX, flyDuration);
            goodsDisplayPlane.transform.localPosition = goodsPlaneStartPos;
            goodsDisplayPlane.transform.DOLocalMoveX(goodsPlaneMoveEndX, flyDuration);
            bagItemsDisplay.transform.localPosition = bagItemsPlaneStartPos;
            bagItemsDisplay.transform.DOLocalMoveY(bagItemsPlaneMoveEndY, flyDuration);

        }

		private void UpdateGoodsSelection(int selectedGoodsIndex){

            for (int i = 0; i < goodsContainer.childCount; i++) {

                Transform goodsCell = goodsContainer.GetChild (i);

                goodsCell.GetComponent<GoodsCell> ().SetSelection (i == selectedGoodsIndex);

            }

            List<HLHNPCGoods> goodsList = npc.GetCurrentLevelGoods ();

            HLHNPCGoods goods = goodsList [selectedGoodsIndex];

            Item itemAsGoods = goods.GetGoodsItem ();

			itemDetail.SetUpItemDetail (itemAsGoods, goods.GetGoodsPrice(), ItemOperationType.Buy);

        }
            

		public void OnBuyButtonClick()
        {

            if (currentSelectedItem == null)
            {
                return;
            }

            string tint = "";


			if (Player.mainPlayer.totalGold < currentSelectItemPrice)
            {
                tint = "金钱不足";
                hintHUD.SetUpSingleTextTintHUD(tint);
                return;
            }

			if (Player.mainPlayer.CheckBagFull(currentSelectedItem)){
				tint = "背包已满";
                hintHUD.SetUpSingleTextTintHUD(tint);
                return;
			}

            Player player = Player.mainPlayer;

			player.totalGold -= currentSelectItemPrice;
            GameManager.Instance.soundManager.PlayAudioClip(CommonData.goldAudioName);

            player.AddItem(currentSelectedItem);

			npc.SoldGoods(currentSelectGoodsIndex);
         
            ExploreManager.Instance.expUICtr.UpdateBottomBar();

            currentSelectedItem = null;

			itemDetail.ClearItemDetails();

            UpdateGoodsDisplay();

            bagItemsDisplay.SetUpCurrentBagItemsPlane();

			ExploreManager.Instance.expUICtr.UpdatePlayerGold();

        }

        public void OnSellButtonClick()
        {

            if (currentSelectedItem == null)
            {
                return;
            }

            Player player = Player.mainPlayer;

            player.totalGold += currentSelectedItem.price / 8;

            GameManager.Instance.soundManager.PlayAudioClip(CommonData.goldAudioName);

            bool totallyRemoveFromBag = player.RemoveItem(currentSelectedItem, 1);

            ExploreManager.Instance.expUICtr.UpdateBottomBar();

            if (totallyRemoveFromBag)
            {
                currentSelectedItem = null;
				itemDetail.ClearItemDetails();
			}
         
            bagItemsDisplay.SetUpCurrentBagItemsPlane();
            
			if(!totallyRemoveFromBag){
				for (int i = 0; i < player.allItemsInBag.Count;i++){
					Item itemInBag = player.allItemsInBag[i];
					if(itemInBag.Equals(currentSelectedItem)){
						int index = i;
						bagItemsDisplay.SetSelectionIcon(index, true);
						break;
					}
				}
			}

			ExploreManager.Instance.expUICtr.UpdatePlayerGold();

        }

		private void RefreshCurrentSelectItemDetailDisplay(Item item)
        {

            currentSelectedItem = item;

            if (currentSelectedItem != null)
            {
				if (currentSelectedItem.itemType == ItemType.Equipment || currentSelectedItem.itemType == ItemType.PropertyGemstone)
                {
					itemDetail.SetUpItemDetail(currentSelectedItem, currentSelectItemPrice, ItemOperationType.Remove);
                }
                else
                {
					itemDetail.SetUpItemDetail(currentSelectedItem, currentSelectItemPrice, ItemOperationType.None);
                }
                return;
            }


        }


		public void QuitTradePlane()
        {

            goodsPool.AddChildInstancesToPool(goodsContainer);

            ExploreManager.Instance.expUICtr.UpdateBottomBar();

            currentSelectedItem = null;

            QuitTradeDisplay();

			if(quitCallBack != null){
				quitCallBack();
			}
        }


		private void QuitTradeDisplay()
        {
            itemDetail.transform.DOLocalMoveX(itemDetailPlaneStartPos.x, flyDuration);
            goodsDisplayPlane.transform.DOLocalMoveX(goodsPlaneStartPos.x, flyDuration);
            bagItemsDisplay.transform.DOLocalMoveY(bagItemsPlaneStartPos.y, flyDuration);

        }
    }

}
