using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	//using UnityEngine.UI;
	using DG.Tweening;

	public class AddGemstoneView : MonoBehaviour   
    {
		
		public BagItemsDisplay bagItemsDisplay;

        public ItemDetailWithNPC itemDetail;

        public SpecialOperationHUD specialOperationHUD;

		public PurchasePendingHUD purchasePendingHUD;

		public BuyGoldView buyGoldView;

		public TintHUD hintHUD;

		//private HLHNPC npc;
              
		public float flyDuration;

        public Vector3 itemDetailPlaneStartPos;

        public Vector3 specialOperationHUDStartPos;

        public Vector3 bagItemsPlaneStartPos;

        public int itemDetailMoveEndX;

        public int specialOpeartionHUDMoveEndX;

        public int bagItemsPlaneMoveEndY;

		private CallBack quitCallBack;

		public Transform quitTradeButton;
        public Transform quitAddGemstoneButton;


		private Item currentSelectedItem;
        


		public void SetUpAddGemstoneView(HLHNPC npc,CallBack quitCallBack){

			//this.npc = npc;

			this.quitCallBack = quitCallBack;

			GameManager.Instance.soundManager.PlayAudioClip(CommonData.merchantAudioName);

			itemDetail.ClearItemDetails();


#if UNITY_IOS
			bagItemsDisplay.InitBagItemsDisplayPlane(OnItemInBagClickInSpecialOperation, PurchaseBagCallBack, buyGoldView.SetUpBuyGoldView);
#elif UNITY_ANDROID
			bagItemsDisplay.InitBagItemsDisplayPlane(OnItemInBagClickInSpecialOperation, PurchaseBagCallBack, EnterGoldWatchAdOnAndroid);
#elif UNITY_EDITOR
            UnityEditor.BuildTarget buildTarget = UnityEditor.EditorUserBuildSettings.activeBuildTarget;

            switch (buildTarget) {
            case UnityEditor.BuildTarget.Android:
		        bagItemsDisplay.InitBagItemsDisplayPlane(OnItemInBagClickInSpecialOperation, PurchaseBagCallBack, EnterGoldWatchAdOnAndroid);
                break;
            case UnityEditor.BuildTarget.iOS:
		        bagItemsDisplay.InitBagItemsDisplayPlane(OnItemInBagClickInSpecialOperation, PurchaseBagCallBack, buyGoldView.SetUpBuyGoldView);
                break;
            }
#endif
                     
            bagItemsDisplay.SetUpBagItemsPlane(0);

			specialOperationHUD.InitSpecialOperationHUD(RefreshCurrentSelectItemDetailDisplay, AddGemstoneCallBack);

            EnterSpecialOperationDisplay();

			quitTradeButton.gameObject.SetActive(false);
			quitAddGemstoneButton.gameObject.SetActive(true);

            

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
                            Player.mainPlayer.totalGold += 100;
                            GameManager.Instance.persistDataManager.UpdateBuyGoldToPlayerDataFile();
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

		private void OnItemInBagClickInSpecialOperation(Item item)
        {

			if (item.itemType == ItemType.Equipment)
            {
				bool hasAdd = specialOperationHUD.equipmentCell.itemInCell != null && specialOperationHUD.equipmentCell.itemInCell == item;

				//Debug.Log(hasAdd);
				if(!hasAdd){
					itemDetail.SetUpItemDetail(item, item.price, ItemOperationType.Add);
				}else{
					itemDetail.SetUpItemDetail(item, item.price, ItemOperationType.Remove);
				}
                
            }
			else if (item.itemType == ItemType.PropertyGemstone)
            {
				bool hasAdd = specialOperationHUD.functionalItemCell.itemInCell != null && specialOperationHUD.functionalItemCell.itemInCell.itemId == item.itemId;

				//Debug.Log(hasAdd);

                if (!hasAdd)
                {
					itemDetail.SetUpItemDetail(item, item.price, ItemOperationType.Add);
                }
                else
                {
					itemDetail.SetUpItemDetail(item, item.price, ItemOperationType.Remove);
                }
            }
            else
            {
				itemDetail.SetUpItemDetail(item,item.price, ItemOperationType.None);
            }

            currentSelectedItem = item;

            bagItemsDisplay.HideAllItemSelectedTintIcon();

        }


            
		public void OnAddButtonClick()
        {
            bool addSucceed = specialOperationHUD.SetUpHUDWhenAddItem(currentSelectedItem);
			itemDetail.SetUpOperationButtons(false, false, !addSucceed, addSucceed, 0);

        }

		private void AddGemstoneCallBack(Item item)
        {

            bagItemsDisplay.SetUpCurrentBagItemsPlane();

            RefreshCurrentSelectItemDetailDisplay(item);

			ExploreManager.Instance.expUICtr.UpdatePlayerStatusBar();

            

        }


        public void OnRemoveButtonClick()
        {
            specialOperationHUD.SetUpHUDWhenRemoveItem(currentSelectedItem);
            itemDetail.ClearItemDetails();
            currentSelectedItem = null;
			bagItemsDisplay.HideAllItemSelectedTintIcon();
        }

		private void RefreshCurrentSelectItemDetailDisplay(Item item)
        {

            currentSelectedItem = item;

            if (currentSelectedItem != null)
            {
				//if (currentSelectedItem.itemType == ItemType.Equipment || currentSelectedItem.itemType == ItemType.PropertyGemstone)
     //           {
					//itemDetail.SetUpItemDetail(currentSelectedItem, currentSelectedItem.price, ItemOperationType.Remove);
                //}
                //else
                //{
				itemDetail.SetUpItemDetail(currentSelectedItem, currentSelectedItem.price, ItemOperationType.Remove);
                //}
            //    return;
            }


        }

		private void EnterSpecialOperationDisplay(){
			itemDetail.ClearItemDetails();
			itemDetail.transform.DOLocalMoveX(itemDetailMoveEndX, flyDuration);
			specialOperationHUD.transform.DOLocalMoveX(specialOpeartionHUDMoveEndX, flyDuration);
			bagItemsDisplay.transform.DOLocalMoveY(bagItemsPlaneMoveEndY, flyDuration);
            specialOperationHUD.QuitSpecialOperationHUD();
		}

		public void QuitAddGemStoneView(){

			QuitSpecialOperationDisplay();

			if(quitCallBack != null){
				quitCallBack();
			}
		}

		private void QuitSpecialOperationDisplay(){
            itemDetail.transform.DOLocalMoveX (itemDetailPlaneStartPos.x, flyDuration);
            specialOperationHUD.transform.DOLocalMoveX (specialOperationHUDStartPos.x, flyDuration);
            bagItemsDisplay.transform.DOLocalMoveY (bagItemsPlaneStartPos.y, flyDuration);
			specialOperationHUD.equipmentCell.ResetSpecialOperationCell();
            specialOperationHUD.QuitSpecialOperationHUD();
        }
            

        
    }
}

