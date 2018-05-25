using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;
	using DG.Tweening;

	public class AddGemstoneView : MonoBehaviour
    {
		public BagItemsDisplay bagItemsDisplay;

        public ItemDetailWithNPC itemDetail;

        public SpecialOperationHUD specialOperationHUD;

		public PurchasePendingHUD purchasePendingHUD;

		private HLHNPC npc;

		private Item currentSelectedItem;

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


		public void SetUpAddGemstoneView(HLHNPC npc,CallBack quitCallBack){

			this.npc = npc;

			this.quitCallBack = quitCallBack;

			GameManager.Instance.soundManager.PlayAudioClip(CommonData.merchantAudioName);

			itemDetail.ClearItemDetails();

            bagItemsDisplay.InitBagItemsDisplayPlane(OnItemInBagClickInSpecialOperation, InitPurchaseExtraBag);

            bagItemsDisplay.SetUpBagItemsPlane(0);


            specialOperationHUD.SetUpHUDWhenAddItem(currentSelectedItem, RefreshCurrentSelectItemDetailDisplay, AddSkillCallBack);

            EnterSpecialOperationDisplay();

			quitTradeButton.gameObject.SetActive(false);
			quitAddGemstoneButton.gameObject.SetActive(true);

		}

		private void OnItemInBagClickInSpecialOperation(Item item)
        {

			if (item.itemType == ItemType.Equipment && (item as Equipment).attachedPropertyGemstone == null)
            {
                itemDetail.SetUpItemDetail(item, ItemOperationType.Add);
            }
			else if (item.itemType == ItemType.PropertyGemstone)
            {
                itemDetail.SetUpItemDetail(item, ItemOperationType.Add);
            }
            else
            {
                itemDetail.SetUpItemDetail(item, ItemOperationType.None);
            }

            currentSelectedItem = item;

            bagItemsDisplay.HideAllItemSelectedTintIcon();

        }

		public void InitPurchaseExtraBag(){
            purchasePendingHUD.SetUpPurchasePendingHUD (PurchaseManager.extra_bag_id,delegate {
                bagItemsDisplay.UpdateBagTabs ();
            });
        }
            
		public void OnAddButtonClick()
        {
            specialOperationHUD.SetUpHUDWhenAddItem(currentSelectedItem, RefreshCurrentSelectItemDetailDisplay, AddSkillCallBack);
            itemDetail.SetUpOperationButtons(false, false, false, true, 0);

        }

		private void AddSkillCallBack(Item item)
        {

            bagItemsDisplay.SetUpCurrentBagItemsPlane();

            RefreshCurrentSelectItemDetailDisplay(item);

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
                    itemDetail.SetUpItemDetail(currentSelectedItem, ItemOperationType.Remove);
                }
                else
                {
                    itemDetail.SetUpItemDetail(currentSelectedItem, ItemOperationType.None);
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

