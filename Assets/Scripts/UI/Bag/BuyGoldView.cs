using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class BuyGoldView : ZoomHUD
    {

		public PurchasePendingHUD purchasePendingHUD;

		public BuyGoldFinishView buyGoldFinishView;



		public void SetUpBuyGoldView(){

			gameObject.SetActive(true);

			if(zoomCoroutine != null){
				StopCoroutine(zoomCoroutine);
			}

			zoomCoroutine = HUDZoomIn();

			StartCoroutine(zoomCoroutine);
         
		}

		public void BuyGold(int index){

			string productID = string.Empty;

			switch(index){
				case 0:
					productID = PurchaseManager.gold_500_id;               
					break;
				case 1:
					productID = PurchaseManager.gold_1600_id;               
					break;
				case 2:
					productID = PurchaseManager.gold_3500_id;               
					break;
				case 3:
					productID = PurchaseManager.gold_5000_id;               
					break;
			}

			purchasePendingHUD.SetUpPurchasePendingHUD(productID, delegate {

				buyGoldFinishView.SetUpBuyGoldFinishView(productID);
			});

		}

		public void QuitBuyGoldView(){

			if (inZoomingOut)
            {
                return;
            }

			if(zoomCoroutine != null){
				StopCoroutine(zoomCoroutine);
			}

			zoomCoroutine = HUDZoomOut();

			StartCoroutine(zoomCoroutine);

		}

        
    }

}
