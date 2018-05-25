using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;
	using DG.Tweening;

	public class NPCTradeView : MonoBehaviour
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

		public TintHUD tintHUD;
           

		public float flyDuration;

        public Vector3 goodsPlaneStartPos;

        public Vector3 itemDetailPlaneStartPos;

        public Vector3 bagItemsPlaneStartPos;

        public int goodsPlaneMoveEndX;

        public int itemDetailMoveEndX;

        public int bagItemsPlaneMoveEndY;

		private Item currentSelectedItem;


		private HLHNPC npc;

		private CallBack quitCallBack;

		public Transform quitTradeButton;
		public Transform quitAddGemstoneButton;

		public void SetUpNPCTradeView(HLHNPC npc,CallBack quitCallBack)
		{
			this.npc = npc;

			this.quitCallBack = quitCallBack;

			GameManager.Instance.soundManager.PlayAudioClip(CommonData.merchantAudioName);

			itemDetail.ClearItemDetails();

            UpdateGoodsDisplay();

            bagItemsDisplay.InitBagItemsDisplayPlane(OnItemInBagClickInTrade, InitPurchaseExtraBag);
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

                goodsCell.GetComponent<GoodsCell>().SetUpGoodsCell(itemAsGoods);

                goodsCell.GetComponent<Button>().onClick.RemoveAllListeners();

                int goodsIndex = i;

                goodsCell.GetComponent<Button>().onClick.AddListener(delegate {
                    currentSelectedItem = itemAsGoods;
                    itemDetail.SetUpItemDetail(itemAsGoods, ItemOperationType.Buy);
                    UpdateGoodsSelection(goodsIndex);
                });

            }

        }

		private void OnItemInBagClickInTrade(Item item)
        {

            itemDetail.SetUpItemDetail(item, ItemOperationType.Sell);

            currentSelectedItem = item;

            bagItemsDisplay.HideAllItemSelectedTintIcon();

        }

		public void InitPurchaseExtraBag(){
			purchasePendingHUD.SetUpPurchasePendingHUD (PurchaseManager.extra_bag_id,delegate {
                bagItemsDisplay.UpdateBagTabs ();
            });
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

            itemDetail.SetUpItemDetail (itemAsGoods, ItemOperationType.Buy);

        }
            

		public void OnBuyButtonClick()
        {

            if (currentSelectedItem == null)
            {
                return;
            }

            string tint = "";


            if (Player.mainPlayer.totalGold < currentSelectedItem.price)
            {
                tint = "金钱不足";
                tintHUD.SetUpSingleTextTintHUD(tint);
                return;
            }

            Player player = Player.mainPlayer;

            player.totalGold -= currentSelectedItem.price;
            GameManager.Instance.soundManager.PlayAudioClip(CommonData.goldAudioName);

            player.AddItem(currentSelectedItem);

            npc.SoldGoods(currentSelectedItem.itemId);

            if (Player.mainPlayer.CheckBagFull(currentSelectedItem))
            {
                GameManager.Instance.UIManager.SetUpCanvasWith(CommonData.bagCanvasBundleName, "BagCanvas", () => {

                    TransformManager.FindTransform("BagCanvas").GetComponent<BagViewController>().AddBagItemWhenBagFull(currentSelectedItem);

                }, false, true);

            }

            ExploreManager.Instance.expUICtr.UpdateBottomBar();

            currentSelectedItem = null;

			itemDetail.ClearItemDetails();

            UpdateGoodsDisplay();

            bagItemsDisplay.SetUpCurrentBagItemsPlane();

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

            UpdateGoodsDisplay();

            bagItemsDisplay.SetUpCurrentBagItemsPlane();

        }

		private void RefreshCurrentSelectItemDetailDisplay(Item item)
        {

            currentSelectedItem = item;

            if (currentSelectedItem != null)
            {
				if (currentSelectedItem.itemType == ItemType.Equipment || currentSelectedItem.itemType == ItemType.PropertyGemstone)
                {
                    itemDetail.SetUpItemDetail(currentSelectedItem, ItemOperationType.Remove);
                }
                else
                {
                    itemDetail.SetUpItemDetail(currentSelectedItem, ItemOperationType.None);
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
