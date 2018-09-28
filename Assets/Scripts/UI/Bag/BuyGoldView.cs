using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class BuyGoldView : ZoomHUD
	{

		public PurchasePendingHUD purchasePendingHUD;

		public Transform buyGoldContainer;

		public BuyGoldFinishView buyGoldFinishView;
      

		public void SetUpBuyGoldView()
		{

			gameObject.SetActive(true);

			buyGoldFinishView.gameObject.SetActive(false);

#if !UNITY_IOS && !UNITY_EDITOR
			return;
#endif

			buyGoldContainer.gameObject.SetActive(true);         

			if (zoomCoroutine != null)
			{
				StopCoroutine(zoomCoroutine);
			}

			zoomCoroutine = HUDZoomIn();

			StartCoroutine(zoomCoroutine);

		}


        
              
		public void BuyGold(int index)
		{         
			string productID = string.Empty;

			int goldBuy = 0;

            switch (index)
            {
                case 0:
                    productID = PurchaseManager.gold_500_id;
					goldBuy = 500;
                    break;
                case 1:
                    productID = PurchaseManager.gold_1600_id;
					goldBuy = 1600;
                    break;
                case 2:
                    productID = PurchaseManager.gold_3500_id;
					goldBuy = 3500;
                    break;
                case 3:
                    productID = PurchaseManager.gold_5000_id;
					goldBuy = 5000;
                    break;
            }

            purchasePendingHUD.SetUpPurchasePendingHUDOnIPhone(productID, delegate {
				Player.mainPlayer.totalGold += goldBuy;
				buyGoldFinishView.SetUpBuyGoldFinishView(productID);
				GameManager.Instance.persistDataManager.UpdateBuyGoldToPlayerDataFile();
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
