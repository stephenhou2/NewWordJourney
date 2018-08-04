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

			bagItemsDisplay.InitBagItemsDisplayPlane(OnItemInBagClickInSpecialOperation, PurchaseBagCallBack);

            bagItemsDisplay.SetUpBagItemsPlane(0);

			specialOperationHUD.InitSpecialOperationHUD(RefreshCurrentSelectItemDetailDisplay, AddGemstoneCallBack);

            EnterSpecialOperationDisplay();

			quitTradeButton.gameObject.SetActive(false);
			quitAddGemstoneButton.gameObject.SetActive(true);

		}

		private void PurchaseBagCallBack(int bagIndex){
			switch (bagIndex)
            {
                case 1:
                    SetUpPurchasePlane(PurchaseManager.extra_bag_2_id);
                    break;
                case 2:
                    SetUpPurchasePlane(PurchaseManager.extra_bag_3_id);
                    break;
                case 3:
                    SetUpPurchasePlane(PurchaseManager.extra_bag_4_id);
                    break;
            }
        }

		private void SetUpPurchasePlane(string productID)
        {

			purchasePendingHUD.SetUpPurchasePendingHUD(productID, delegate {
                bagItemsDisplay.UpdateBagTabs();
            });

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

        }

		private void RefreshCurrentSelectItemDetailDisplay(Item item)
        {

            currentSelectedItem = item;

            if (currentSelectedItem != null)
            {
				if (currentSelectedItem.itemType == ItemType.Equipment || currentSelectedItem.itemType == ItemType.PropertyGemstone)
                {
					itemDetail.SetUpItemDetail(currentSelectedItem, currentSelectedItem.price, ItemOperationType.Remove);
                }
                else
                {
					itemDetail.SetUpItemDetail(currentSelectedItem, currentSelectedItem.price, ItemOperationType.None);
                }
                return;
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
            specialOperationHUD.QuitSpecialOperationHUD();
        }
            

        
    }
}

