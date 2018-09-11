using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WordJourney
{

	using UnityEngine.UI;

	public class BuyGoldFinishView : ZoomHUD
    {

		public Image goldItemIcon;
		public Text buyResultHint;

		public Sprite goldIcon_500;
		public Sprite goldIcon_1600;
		public Sprite goldIcon_3500;
		public Sprite goldIcon_5000;


		public void SetUpBuyGoldFinishView(string productID){

			Sprite buyItemSprite = null;

			int goldBuy = 0;
         
			if(productID == PurchaseManager.gold_500_id){
				buyItemSprite = goldIcon_500;
				goldBuy = 500;
			}else if(productID == PurchaseManager.gold_1600_id){
				buyItemSprite = goldIcon_1600;
				goldBuy = 1600;
			}else if(productID == PurchaseManager.gold_3500_id){
				buyItemSprite = goldIcon_3500;
				goldBuy = 3500;
			}else if(productID == PurchaseManager.gold_5000_id){
				buyItemSprite = goldIcon_5000;
				goldBuy = 5000;
            }

			Player.mainPlayer.totalGold += goldBuy;

			ExploreManager.Instance.expUICtr.UpdatePlayerGold();

			string buyResult = string.Format("成功购买<color=orange>{0}</color>金币", goldBuy);
			         
			goldItemIcon.sprite = buyItemSprite;
			buyResultHint.text = buyResult;

			this.gameObject.SetActive(true);

			if(zoomCoroutine != null){
				StopCoroutine(zoomCoroutine);
			}

			zoomCoroutine = HUDZoomIn();

			StartCoroutine(zoomCoroutine);

		}
      
		public void QuitBuyGoldFinishView(){

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
